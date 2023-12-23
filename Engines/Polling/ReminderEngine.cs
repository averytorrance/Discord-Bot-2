using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordBot.Models;
using Newtonsoft.Json;
using System.IO;
using DiscordBot.Config;

namespace DiscordBot.Engines
{
    public class ReminderEngine
    {
        private static Dictionary<ulong, ReminderState> reminderStates = new Dictionary<ulong, ReminderState>();

        public static bool TaskRunning = false;

        /// <summary>
        /// Loads all reminder state files
        /// </summary>
        /// <param name="serverID"></param>
        public static void Load(ulong serverID)
        {
            if (!reminderStates.ContainsKey(serverID))
            {
                reminderStates.Add(serverID, ReminderState.Load<ReminderState>(new ReminderState(serverID)));
            }
        }

        /// <summary>
        /// Saves the state of all reminder states
        /// </summary>
        public static void Save()
        {
            ReminderState state;
            foreach (ulong serverID in reminderStates.Keys)
            {
                reminderStates.TryGetValue(serverID, out state);
                state.SaveState();
            }
        }

        /// <summary>
        /// Creates a reminder for a specific server
        /// </summary>
        /// <param name="serverID"></param>
        /// <param name="message"></param>
        /// <param name="ownerID"></param>
        /// <param name="sendTime"></param>
        public static void CreateReminder(ulong serverID, string message, ulong ownerID, DateTime sendTime)
        {
            Load(serverID);
            ReminderState state;
            reminderStates.TryGetValue(serverID, out state);
            state.CreateReminder(message, ownerID, sendTime);
        }

        /// <summary>
        /// Triggers a send reminder for all servers
        /// </summary>
        public static async void SendReminders()
        {
            await Task.Run(() =>
            {
                TaskRunning = true;
                foreach (ReminderState state in reminderStates.Values)
                {
                    state.SendReminder();
                }
                TaskRunning = false;
            });
        }

        /// <summary>
        /// Triggers send stale remidners for all servers
        /// </summary>
        public static async void SendStaleReminders()
        {
            await Task.Run(() =>
            {
                TaskRunning = true;
                foreach (ReminderState state in reminderStates.Values)
                {
                    state.SendStaleReminders();
                }
                TaskRunning = false;
            });
        }

        /// <summary>
        /// Trigers a send reminder for a specific discord server
        /// </summary>
        /// <param name="serverID"></param>
        public static void SendReminders(ulong serverID)
        {
            ReminderState state;
            if (reminderStates.TryGetValue(serverID, out state))
            {
                state.SendReminder();
            }
        }

        public class ReminderState : EngineState
        {
            /// <summary>
            /// ID of the next reminder that is created
            /// </summary>
            public int _CurrentID = 0;

            /// <summary>
            /// A list of scheduled reminders
            /// </summary>
            public List<Reminder> _reminders { get; set; } = new List<Reminder>();

            /// <summary>
            /// A list of sent reminders
            /// </summary>
            public List<Reminder> _sentReminders { get; set; } = new List<Reminder>();

            [JsonIgnore]
            public override string StateFile_ { get; } = "Reminders.JSON";

            public ReminderState(ulong serverID)
            {
                ServerID = serverID;
            }

            /// <summary>
            /// Creates a reminder
            /// </summary>
            /// <param name="message">message of the reminder</param>
            /// <param name="ownerID">Discord ID of the owner</param>
            /// <param name="sendTime">DateTime of when to send the reminder, in server time</param>
            /// <returns>the ID of the created reminder</returns>
            public int CreateReminder(string message, ulong ownerID, DateTime sendTime)
            {
                Reminder reminder = new Reminder(_CurrentID, message, ownerID, sendTime);
                
                if (reminder.IsStale())
                {
                    throw new Exception("Attempted to create a reminder for a time in the past.");
                }
                else
                {
                    _addReminder(reminder);
                }
                return reminder.ID;
            }

            /// <summary>
            /// Adds a reminder to the reminders list and resorts it.
            /// </summary>
            /// <param name="reminder">reminder to add to the list</param>
            private void _addReminder(Reminder reminder)
            {
                _reminders.Add(reminder);
                _reminders = _reminders.OrderByDescending(x => x.SendTime).ThenBy(x => x.ID).ToList();
                _CurrentID++;
                SaveState();
            }

            /// <summary>
            /// Sends a reminder and saves the state
            /// </summary>
            public void SendReminder()
            {
                Reminder reminder = _reminders.Last();
                while (reminder.ReadyToSend())
                {
                    _sendReminder(reminder);
                    reminder = _reminders.Last();
                }
                SaveState();
            }

            /// <summary>
            /// Sends a reminder
            /// </summary>
            /// <param name="reminder"></param>
            private void _sendReminder(Reminder reminder)
            {
                reminder.Send();
                _reminders.Remove(reminder);
                _sentReminders.Add(reminder);
            }

            /// <summary>
            /// Sends all of the stale reminders
            /// </summary>
            public void SendStaleReminders()
            {
                List<Reminder> staleReminders = _reminders.Where(x => x.IsStale()).ToList();

                foreach (Reminder reminder in staleReminders)
                {
                    reminder.Send();
                    _reminders.Remove(reminder);
                    _sentReminders.Add(reminder);
                }
                SaveState();
            }

            /// <summary>
            /// Validates the Engine State
            /// </summary>
            /// <returns>a list with validation warnings. </returns>
            public List<ValidationWarnings> Validate()
            {
                List<ValidationWarnings> warnings = new List<ValidationWarnings>();

                if (_reminders.Where(x => x.Sent).ToList().Count() > 0)
                {
                    warnings.Add(ValidationWarnings.UnsentSentReminders);
                }

                if (_sentReminders.Where(x => !x.Sent).ToList().Count() > 0)
                {
                    warnings.Add(ValidationWarnings.SentUnsentReminders);
                }

                return warnings;

            }

            /// <summary>
            /// Validation Warnings
            /// </summary>
            public enum ValidationWarnings
            {
                UnsentSentReminders,
                SentUnsentReminders,
            }

        }

    }
}
