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

            // Wait until the next minute to start timer
            try
            {
                int millisecondsUntilNextMinute = _pollerMillisecondWait - DateTime.UtcNow.Millisecond;
                System.Threading.Thread.Sleep(millisecondsUntilNextMinute);
            }
            catch (Exception ex)
            {
                Log.WriteToFile(Log.LogLevel.DiscordBot, ex, "Error encountored while waiting for next minute to start poller.");
            }
            finally
            {
                _timer.Start();
            }
            
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
                List<ITask> runnableTasks = getRunnableTasks();

                foreach (ITask task in runnableTasks)
                {
                    if (IsRunning())
                    {
                        ExecuteTask(task);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteToFile(Log.LogLevel.DiscordBot, ex);
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
        /// Executes an ITask object
        /// </summary>
        /// <param name="task"></param>
        private void ExecuteTask(ITask task)
        {
            try
            {
                _tasks.Remove(task);
                task.Execute();
            }
            catch (Exception ex)
            {
                Log.WriteToFile(Log.LogLevel.DiscordBot, ex, $"Error executing task: {task}");
            }
        }

        /// <summary>
        /// Executes a task given its ID
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void ExecuteTask(ulong id)
        {
            if (!HasTask(id))
            {
                throw new InvalidOperationException($"Task with id {id} does not exist");
            }

            ITask task = GetTask(id);
            ExecuteTask(task);
        }

        /// <summary>
        /// Gets a list of runnable tasks ordered by priority 
        /// </summary>
        /// <returns></returns>
        public List<ITask> getRunnableTasks()
        {
            return _tasks.Where(x => x.ExecutionTime <= DateTime.UtcNow).OrderBy(x => x.Priority).ToList();
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
        /// Gets a task given a specific ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private ITask GetTask(ulong id)
        {
            return _tasks.Where(x => x.TaskID == id).FirstOrDefault();
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

        /// <summary>
        /// Destroys the engine.
        /// </summary>
        public void Destroy()
        {
            _state = PollerState.Stopped;
            _destroyTimer();
            CurrentEngine = null;
        }
    }

    /// <summary>
    /// Enum representing different states for the Task Engine poller
    /// </summary>
    public enum PollerState
    {
        Stopped,
        Running,
        Paused
    }
}
