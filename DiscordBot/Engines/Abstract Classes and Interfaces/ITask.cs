using System;

namespace DiscordBot.Engines.Tasks
{
    /// <summary>
    /// Interface for tasks that can run with the poller
    /// </summary>
    public interface ITask
    {
        /// <summary>
        /// ID for the Task
        /// </summary>
        ulong TaskID { get; set; }

        /// <summary>
        /// The time to run the tasks
        /// </summary>
        DateTime ExecutionTime { get; set; }

        /// <summary>
        /// Priority of the Task
        /// </summary>
        TaskPriority Priority { get; set; }

        /// <summary>
        /// Run Method
        /// </summary>
        void Execute();

    }

    /// <summary>
    /// Interface for server specific tasks that can run with the poller
    /// </summary>
    interface IServerTask : ITask
    {
        ulong ServerID { get; set; }
    }

    public enum TaskPriority
    {
        Crucial = 0,
        Default = 99999
    }
}
