using DiscordBot.Classes;
using DSharpPlus.Entities;
using System;

namespace DiscordBot.Engines.Tasks
{
    public class ChristmasReminderTask : IServerTask
    {

        public ulong ServerID { get; set; }
        public ulong TaskID { get; set; }
        public DateTime ExecutionTime { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serverID"></param>
        public ChristmasReminderTask(ulong serverID)
        {
            ServerID = serverID;
            TaskID = TaskEngine.CurrentEngine.GenerateRandomTaskID();
            ExecutionTime = DateTime.Now.Date.AddDays(1);
            ExecutionTime = TimeZoneInfo.ConvertTime(ExecutionTime, TimeZoneInfo.Local, TimeZoneInfo.Utc);
        }

        /// <summary>
        /// Executes the Christmas Reminder
        /// </summary>
        public async void Execute()
        {
            DateTime today = DateTime.Now;
            DateTime christmas = new DateTime(today.Year, 12, 25);

            TimeSpan diffDates = christmas - today;

            string message;
            if(diffDates.Days < 0)
            {
                christmas.AddYears(1);
                diffDates = christmas - today;
            }

            if(diffDates.Days == 0)
            {
                message = "Merry Christmas!";
            }
            else
            {
                message = $"{diffDates.Days} until Christmas!";
            }


            DiscordGuild discordGuild = await Program.Client.GetGuildAsync(ServerID);
            DiscordServerEngine engine = new DiscordServerEngine(discordGuild);
            engine.SendBotMessage(message);

            ExecutionTime = ExecutionTime.AddDays(1);
            TaskEngine.CurrentEngine.AddTask(this);
        }

    }
}
