using System;
using DiscordBot.Engines;
using DiscordBot.Engines.Tasks;

namespace DiscordBotUnitTests
{
    public class TestTask : ITask
    {
        public ulong TaskID { get; set ; }
        public DateTime ExecutionTime { get; set; }

        public readonly int MinutesToWait =180;
        public TaskPriority Priority { get; set; } = TaskPriority.Default;

        public bool TaskRan { get; set; } = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serverID"></param>
        public TestTask(DateTime? time = null)
        {
            TaskID = TaskEngine.CurrentEngine.GenerateRandomTaskID();
            if(time == null)
            {
                ExecutionTime = DateTime.UtcNow.AddMinutes(10);
            }
            else 
            {
                ExecutionTime = (DateTime)time;
            }
            
        }

        /// <summary>
        /// Execute the task
        /// </summary>
        public void Execute()
        {
            TaskRan = true;
            return;
        }

    }
}
