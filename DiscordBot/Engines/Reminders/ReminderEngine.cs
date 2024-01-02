using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using DiscordBot.Config;
using DSharpPlus.Entities;
using DiscordBot.Engines.Tasks;
using DiscordBot.Classes;

namespace DiscordBot.Engines
{
    public class ReminderEngine : ServerEngine
    {
        public static ReminderEngine CurrentEngine;

        public static bool TaskRunning;

        public override Type EngineStateType { get; } = typeof(ReminderState);


        public ReminderEngine() : base()
        {
            CurrentEngine = this;
        }

        public static async Task<DiscordChannel> GetReminderChannel(ulong serverID)
        {
            ServerConfig config = ServerConfig.GetServerConfig(serverID);
            if(config.ReminderChannelID != null)
            {
                return await Program.Client.GetChannelAsync(config.ReminderChannelID);
            }
            else if(config.BotChannelID != null)
            {
                return await Program.Client.GetChannelAsync(config.BotChannelID);
            }

            return null;

        }

        /// <summary>
        /// Load reminder states
        /// </summary>
        /// <param name="serverID"></param>
        public override void Load(ulong serverID)
        {
            Load(serverID, false);
        }

        /// <summary>
        /// Load reminder states
        /// </summary>
        /// <param name="serverID"></param>
        public void Load(ulong serverID, bool loadTasks)
        {
            ReminderState state;
            if (TryGetValue(serverID, out state))
            {
                serverStates.Remove(serverID);
            }
            state = ServerEngineState.Load<ReminderState>(new ReminderState(serverID));
            serverStates.Add(serverID, state);
            if (loadTasks)
            {
                state.LoadTasks();
            }
        }

        /// <summary>
        /// Creates a reminder for a specific server
        /// </summary>
        /// <param name="serverID"></param>
        /// <param name="message"></param>
        /// <param name="ownerID"></param>
        /// <param name="sendTime"></param>
        /// <param name="frequency"></param>
        /// <param name="frequencyFactor"></param>
        /// <returns>The ID of the new reminder</returns>
        public int CreateReminder(ulong serverID, string message, ulong ownerID, DateTime sendTime, Freq frequency = Freq.None, int frequencyFactor = 1)
        {
            Load(serverID);
            ReminderState state;
            TryGetValue(serverID, out state);
            return state.CreateReminder(message, ownerID, sendTime, frequency, frequencyFactor);
        }

        /// <summary>
        /// Triggers a send reminder for all servers
        /// </summary>
        public async void SendReminder(ulong serverID, int reminderID)
        {
            ReminderState state;
            if(TryGetValue(serverID, out state))
            {
                await state.SendReminder(reminderID);
            }
        }

        /// <summary>
        /// Triggers send stale remidners for all servers
        /// </summary>
        public void SendStaleReminders(ulong serverID)
        {
            if (serverStates == null)
            {
                return;
            }

            ReminderState state;
            if (TryGetValue(serverID, out state))
            {
                state.SendStaleReminders();
            }
        }

        public List<Reminder> Search(ReminderSearch search, ulong serverID)
        {
            ReminderState state;
            TryGetValue(serverID, out state);

            return state.Search(search);

        }

    }

    public class ReminderState : ServerEngineState
    {
        /// <summary>
        /// ID of the next reminder that is created
        /// </summary>
        public int _CurrentID = 0;

        /// <summary>
        /// A list of scheduled reminders
        /// </summary>
        public Dictionary<int, Reminder> _reminders { get; set; } = new Dictionary<int, Reminder>();

        /// <summary>
        /// A list of sent reminders
        /// </summary>
        public List<Reminder> _sentReminders { get; set; } = new List<Reminder>();

        /// <summary>
        /// A list of deleted reminders
        /// </summary>
        public List<Reminder> _deletedReminders { get; set; } = new List<Reminder>();

        [JsonIgnore]
        public override string StateFile_ { get; } = "Reminders.JSON";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serverID"></param>
        public ReminderState(ulong serverID) : base(serverID)
        {

        }

        /// <summary>
        /// Loads reminder tasks into the Task Engine
        /// </summary>
        public void LoadTasks()
        {
            foreach(Reminder reminder in _reminders.Values)
            {
                LoadReminderTask(reminder);
            }
        }

        /// <summary>
        /// Loads reminders as tasks into the task engine
        /// </summary>
        /// <param name="reminder"></param>
        public void LoadReminderTask(Reminder reminder)
        {
            ReminderTask reminderTask = new ReminderTask(ServerID, reminder);
            if (!TaskEngine.CurrentEngine.HasTask(reminderTask.TaskID))
            {
                TaskEngine.CurrentEngine.AddTask(reminderTask);
            }
        }

        /// <summary>
        /// Creates a reminder
        /// </summary>
        /// <param name="message">message of the reminder</param>
        /// <param name="ownerID">Discord ID of the owner</param>
        /// <param name="sendTime">DateTime of when to send the reminder, in server time</param>
        /// <returns>the ID of the created reminder</returns>
        public int CreateReminder(string message, ulong ownerID, DateTime sendTime, Freq frequency = Freq.None, int frequencyFactor = 1)
        {
            Reminder reminder = new Reminder(_CurrentID, message, ownerID, sendTime, frequency, frequencyFactor);

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
        /// Deletes a Reminder
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        public bool DeleteReminder(ulong userID, int ID)
        {
            ///TODO: Add Handling for if the remidner is schduled
            Reminder reminder;
            if (_reminders.TryGetValue(ID, out reminder))
            {
                if(reminder.OwnerId == userID)
                {
                    _reminders.Remove(ID);
                    _deletedReminders.Add(reminder);
                    return true;
                }

            }
            return false;
        }

        /// <summary>
        /// Gets User Owned Reminders
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public List<Reminder> GetUserOwnedReminder(ulong userID)
        {
            List<Reminder> reminders = _reminders.Values
                .Where(x => x.OwnerId == userID)
                .OrderBy(x => x.SendTime)
                .ToList();

            return reminders;
        }

        /// <summary>
        /// Gets User Subscribed Reminders
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public List<Reminder> GetUserSubscribedReminder(ulong userID)
        {
            List<Reminder> reminders = _reminders.Values
                .Where(x => x.IsUserSubscribed(userID))
                .OrderBy(x => x.SendTime)
                .ToList();

            return reminders;
        }

        /// <summary>
        /// Adds a reminder to the reminders list and resorts it.
        /// </summary>
        /// <param name="reminder">reminder to add to the list</param>
        private void _addReminder(Reminder reminder)
        {
            _reminders.Add(_CurrentID, reminder);
            _CurrentID++;
            SaveState();
            LoadReminderTask(reminder);
        }

        /// <summary>
        /// Sends a reminder and saves the state
        /// </summary>
        public async Task<bool> SendReminder(int ID)
        {
            Reminder reminder;
            if(_reminders.TryGetValue(ID, out reminder))
            {
                await _sendReminder(reminder);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Sends a reminder
        /// </summary>
        /// <param name="reminder"></param>
        private async Task<bool> _sendReminder(Reminder reminder)
        {
            try
            {
                //TODO: Add Handling for recurring reminders
                DiscordChannel channel = await ReminderEngine.GetReminderChannel(ServerID);

                await channel.SendMessageAsync(reminder.ReminderMessage());

                _reminders.Remove(reminder.ID);
                _sentReminders.Add(reminder);
                SaveState();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Sends all of the stale reminders
        /// </summary>
        public async void SendStaleReminders()
        {
            List<Reminder> staleReminders = _reminders.Values.Where(x => x.IsStale()).ToList();

            foreach (Reminder reminder in staleReminders)
            {
                await _sendReminder(reminder);
            }
        }

        /// <summary>
        /// Gets a list of all reminders
        /// </summary>
        /// <returns></returns>
        public List<Reminder> GetReminders()
        {
            return _reminders.Values.ToList();
        }

        /// <summary>
        /// Searches for reminders using the criteria in a reminder search object
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        public List<Reminder> Search(ReminderSearch searchParams)
        {
            return Search(searchParams.SearchFunction());
        }

        /// <summary>
        /// Searches for reminders where a specific criteria is true
        /// </summary>
        /// <param name="func">delegate function where the input parameter is a reminder and the output is a boolean if the reminder should be included</param>
        /// <returns></returns>
        public List<Reminder> Search(Func<Reminder, bool> func)
        {
            return GetReminders().Where(x => func(x)).ToList();
        }

        /// <summary>
        /// Validates the Engine State
        /// </summary>
        /// <returns>a list with validation warnings. </returns>
        public List<ValidationWarnings> Validate()
        {
            List<ValidationWarnings> warnings = new List<ValidationWarnings>();

            if (_reminders.Values.Where(x => x.Sent).ToList().Count() > 0)
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
