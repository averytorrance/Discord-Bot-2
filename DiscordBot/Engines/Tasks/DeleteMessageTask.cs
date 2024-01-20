using DSharpPlus.Entities;
using System;

namespace DiscordBot.Engines.Tasks
{
    /// <summary>
    /// Startup task to verify that no watch entries were deleted. 
    /// This task should not be queued outside of program startup. 
    /// The idea is that a watch rating could be deleted while the bot is down, so after startup,
    /// we need to validate that all the watch entries exist, otherwise archive the data. 
    /// </summary>
    public class DeleteMessageTask : IServerTask
    {
        public ulong ServerID { get; set; }
        public ulong TaskID { get; set; }
        public DateTime ExecutionTime { get; set; }
        public DiscordMessage Message { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serverID"></param>
        public DeleteMessageTask(DiscordMessage message, int minutesToDelete = 10)
        {
            ServerID = (ulong)message.Channel.GuildId;
            TaskID = message.Id;
            ExecutionTime = DateTime.UtcNow.AddMinutes(1); // 1 hour after creation
            Message = message;
        }

        /// <summary>
        /// Execute the Archive Watch Ratings task. This task should only be created upon startup when guilds are being
        /// loaded into the client. 
        /// </summary>
        public async void Execute()
        {
            await Message.DeleteAsync();
        }

    }
}
