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
        /// Run Method
        /// </summary>
        void Execute();

        /// <summary>
        /// Logs the task after execution
        /// </summary>
        void Log();

        /// <summary>
        /// Returns true if the task should run, false otherwise
        /// </summary>
        /// <returns></returns>
        bool ReadyToRun();


    }

    /// <summary>
    /// Interface for server specific tasks that can run with the poller
    /// </summary>
    interface IServerTask : ITask
    {
        ulong ServerID { get; set; }
    }
}
