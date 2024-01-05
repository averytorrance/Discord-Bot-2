﻿using System;

namespace DiscordBot.Engines.Tasks
{
    public class YoutubeTask : IServerTask
    {
        public ulong ServerID { get; set; }
        public ulong TaskID { get; set ; }
        public DateTime ExecutionTime { get; set; }

        public readonly int MinutesToWait = 30;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serverID"></param>
        public YoutubeTask(ulong serverID)
        {
            ServerID = serverID;
            TaskID = TaskEngine.CurrentEngine.GenerateRandomTaskID();
            ExecutionTime = DateTime.UtcNow.AddMinutes(1);
        }

        /// <summary>
        /// Execute the task
        /// </summary>
        public void Execute()
        {
            YoutubeAPIEngine engine = new YoutubeAPIEngine();
            engine.SendVideos(ServerID);
            ExecutionTime.AddMinutes(MinutesToWait);
            TaskEngine.CurrentEngine.AddTask(this);
        }

        public void Log()
        {
            throw new NotImplementedException();
        }

        public bool ReadyToRun()
        {
            throw new NotImplementedException();
        }
    }
}
