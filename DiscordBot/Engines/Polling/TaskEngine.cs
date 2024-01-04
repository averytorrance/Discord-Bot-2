using DiscordBot.Classes;
using DiscordBot.Engines.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace DiscordBot.Engines
{
    public class TaskEngine
    {

        public static TaskEngine CurrentEngine { get; private set; }

        private List<ITask> _tasks = new List<ITask>();

        private PollerState _state { get; set; }

        private int _pollerMillisecondWait = 60000;

        private Timer _timer { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <exception cref="Exception"></exception>
        public TaskEngine()
        {
            if(CurrentEngine != null)
            {
                throw new Exception("Instance of Task Engine already exists");
            }
            CurrentEngine = this;
        }

        /// <summary>
        /// Starts the TaskEngine
        /// </summary>
        public async void Start()
        {
            if (IsRunning()) return;

            _state = PollerState.Running;
            _timer = new Timer(_pollerMillisecondWait);
            _timer.Elapsed += new ElapsedEventHandler(_runTasks);
            _timer.Start();
        }

        /// <summary>
        /// Checks if the TaskEngine is running
        /// </summary>
        /// <returns></returns>
        public bool IsRunning()
        {
            return _state == PollerState.Running;
        }

        /// <summary>
        /// Runs the tasks in the task list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void _runTasks(object sender, ElapsedEventArgs args)
        {
            try
            {
                if (IsRunning())
                {
                    List<ITask> runnableTasks = _tasks.Where(x => x.ExecutionTime <= DateTime.UtcNow).ToList();

                    foreach (ITask task in runnableTasks)
                    {
                        try
                        {
                            _tasks.Remove(task);
                            task.Execute();
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                if (IsRunning())
                {
                    _timer.Start();
                }
            }

        }

        /// <summary>
        /// Adds a task object to the task queue
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public bool AddTask(ITask task)
        {
            if(task.ExecutionTime.Kind != DateTimeKind.Utc)
            {
                throw new Exception("Attempted to queue a task using a non-UTC DateTimeKind.");
            }

            if (!HasTask(task.TaskID))
            {
                _tasks.Add(task);
                _tasks = _tasks.OrderBy(t => t.TaskID).ToList();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes a Task from the queue
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public bool RemoveTask(ITask task)
        {
            return _tasks.Remove(task);
        }

        public void RemoveTask(ulong id)
        {
            _tasks.RemoveAll(x => x.TaskID == id);
        }

        /// <summary>
        /// Checks if a Task with a specific ID is queued
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool HasTask(ulong id)
        {
            return _tasks.Where(x => x.TaskID == id).Count() > 0;
        }

        /// <summary>
        /// Tops the Task Engine
        /// </summary>
        public void Stop()
        {
            if (!IsRunning())
            {
                return;
            }
            _state = PollerState.Stopped;
            _destroyTimer();
        }

        /// <summary>
        /// Destroys the timer
        /// </summary>
        private void _destroyTimer()
        {
            if(_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
                _timer = null;
            }
            
        }

        /// <summary>
        /// Generates a random unique task ID
        /// </summary>
        public ulong GenerateRandomTaskID() 
        {
            ulong rand = _getRandomUlong();

            while (HasTask(rand))
            {
                rand = _getRandomUlong();
            }

            return rand;
        }

        /// <summary>
        /// Generates a random ulong
        /// </summary>
        /// <returns></returns>
        private ulong _getRandomUlong()
        {
            ulong min = (ulong)int.MaxValue;
            Random rand = new Random();
            return min + (ulong)rand.Next(0, int.MaxValue);
        }

        /// <summary>
        /// Generates a string containing all of the task queue information
        /// </summary>
        /// <returns></returns>
        public string GetTaskList(ulong? serverID = null)
        {
            string results = "";
            foreach(ITask task in _tasks)
            {
                string result = $"Type: {task.GetType()}\n" +
                    $"ID: {task.TaskID}\n" +
                    $"Execution Time UTC: {task.ExecutionTime}\n" +
                    $"Execution Time Local: {TimeZoneInfo.ConvertTime(task.ExecutionTime, TimeZoneInfo.Local)}";
                    
                if (serverID != null)
                {
                    if (task is IServerTask)
                    {
                        IServerTask serverTask = (IServerTask)task;
                        if(serverTask.ServerID != serverID)
                        {
                            continue;
                        }
                        result = $"ServerID: {serverTask.ServerID}\n" +
                            $"{result}";
                    }
                }

                results += $"{result}\n\n";
            }

            return results;
        }

    }

    /// <summary>
    /// Enum representing different states for the Task Engine poller
    /// </summary>
    public enum PollerState
    {
        Stopped,
        StopRequested,
        Running,
        Paused
    }
}
