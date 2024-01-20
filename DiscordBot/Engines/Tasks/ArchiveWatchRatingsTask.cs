using System;

namespace DiscordBot.Engines.Tasks
{
    /// <summary>
    /// Startup task to verify that no watch entries were deleted. 
    /// This task should not be queued outside of program startup. 
    /// The idea is that a watch rating could be deleted while the bot is down, so after startup,
    /// we need to validate that all the watch entries exist, otherwise archive the data. 
    /// </summary>
    public class ArchiveWatchRatingsTask : IServerTask
    {
        public ulong ServerID { get; set; }
        public ulong TaskID { get; set; }
        public DateTime ExecutionTime { get; set; }
        public TaskPriority Priority { get; set; } = TaskPriority.Default;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serverID"></param>
        public ArchiveWatchRatingsTask(ulong serverID)
        {
            ServerID = serverID;
            TaskID = TaskEngine.CurrentEngine.GenerateRandomTaskID();
            ExecutionTime = DateTime.UtcNow.AddHours(1); // 1 hour after creation
        }

        /// <summary>
        /// Execute the Archive Watch Ratings task. This task should only be created upon startup when guilds are being
        /// loaded into the client. 
        /// </summary>
        public async void Execute()
        {
            await WatchRatingsEngine.CurrentEngine.ArchiveMissingMessages(ServerID);
        }

    }
}
