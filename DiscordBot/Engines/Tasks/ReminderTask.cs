using System;
using DiscordBot.Classes;


namespace DiscordBot.Engines.Tasks
{
    public class ReminderTask : IServerTask
    {
        public ulong TaskID { get; set; }

        public ulong ServerID { get; set;  }

        public DateTime ExecutionTime { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public ReminderTask(ulong serverID, Reminder reminder)
        {
            ServerID = serverID;
            ExecutionTime = reminder.SendTime;
            TaskID = (ulong)reminder.ID;
        }

        public void Execute()
        {
            ReminderEngine.CurrentEngine.SendReminder(ServerID, (int)TaskID);
        }

        public void Log()
        {
            //TODO
        }

        public bool ReadyToRun() 
        {
            return false;
        }
    }
}
