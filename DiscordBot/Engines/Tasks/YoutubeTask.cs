using System;

namespace DiscordBot.Engines.Tasks
{
    public class YoutubeTask : IServerTask
    {
        public ulong ServerID { get; set; }
        public ulong TaskID { get; set ; }
        public DateTime ExecutionTime { get; set; }

        public readonly int MinutesToWait =180;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serverID"></param>
        public YoutubeTask(ulong serverID)
        {
            ServerID = serverID;
            TaskID = TaskEngine.CurrentEngine.GenerateRandomTaskID();
            ExecutionTime = DateTime.UtcNow.AddMinutes(5);
        }

        /// <summary>
        /// Execute the task
        /// </summary>
        public void Execute()
        {
            YoutubeAPIEngine engine = new YoutubeAPIEngine();
            engine.SendVideos(ServerID);
            ExecutionTime = ExecutionTime.AddMinutes(MinutesToWait);
            TaskEngine.CurrentEngine.AddTask(this);
        }

    }
}
