using Microsoft.VisualStudio.TestTools.UnitTesting;
using DiscordBot.Engines;
using DiscordBot.Engines.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;

namespace DiscordBotUnitTests
{
    [TestClass]
    public class TaskEngineTests
    {

        #region GetRunnableTasks

        [TestMethod]
        public void RunnableTaskPriority_DifferentPriorities()
        {
            var engine = new TaskEngine();

            DateTime taskTime = DateTime.UtcNow.AddMinutes(-1);

            TestTask task1 = new TestTask(taskTime);
            TestTask task2 = new TestTask(taskTime);
            task2.Priority = TaskPriority.Crucial;

            engine.AddTask(task1);
            engine.AddTask(task2);

            List<ITask> expected = new List<ITask>() { task2, task1 };

            List<ITask> result = engine.getRunnableTasks();

            Assert.IsTrue(expected.SequenceEqual(result));
            engine.Destroy();
        }

        [TestMethod]
        public void RunnableTaskPriority_AllTasksInFuture()
        {
            var engine = new TaskEngine();

            DateTime taskTime = DateTime.UtcNow.AddMinutes(1);

            TestTask task1 = new TestTask(taskTime);

            engine.AddTask(task1);

            List<ITask> result = engine.getRunnableTasks();

            Assert.IsTrue(result.Count == 0);
            engine.Destroy();
        }

        [TestMethod]
        public void RunnableTaskPriority_AllTasksInPast()
        {
            var engine = new TaskEngine();

            DateTime taskTime = DateTime.UtcNow.AddMinutes(-1);

            TestTask task1 = new TestTask(taskTime);

            engine.AddTask(task1);

            List<ITask> expected = new List<ITask>() { task1 };
            List<ITask> result = engine.getRunnableTasks();

            Assert.IsTrue(result.All(expected.Contains));
            engine.Destroy();
        }

        [TestMethod]
        public void RunnableTaskPriority_MixOfTasks()
        {
            var engine = new TaskEngine();

            TestTask task1 = new TestTask(DateTime.UtcNow);
            TestTask task2 = new TestTask(DateTime.UtcNow.AddMinutes(1));
            TestTask task3 = new TestTask(DateTime.UtcNow.AddMinutes(-1));

            engine.AddTask(task1);
            engine.AddTask(task2);
            engine.AddTask(task3);

            List<ITask> expected = new List<ITask>() { task1, task3 };
            List<ITask> result = engine.getRunnableTasks();

            Assert.IsTrue(result.All(expected.Contains));
            engine.Destroy();
        }

        #endregion

        #region Add/Remove Tasks
        [TestMethod]
        public void AddTask()
        {
            var engine = new TaskEngine();
            TestTask task1 = new TestTask();

            engine.AddTask(task1);

            Assert.IsTrue(engine.HasTask(task1.TaskID));
            engine.Destroy();
        }

        [TestMethod]
        public void RemoveTask_Task()
        {
            var engine = new TaskEngine();
            TestTask task1 = new TestTask();

            engine.AddTask(task1);

            engine.RemoveTask(task1);

            Assert.IsTrue(!engine.HasTask(task1.TaskID));
            engine.Destroy();
        }

        [TestMethod]
        public void RemoveTask_ID()
        {
            var engine = new TaskEngine();
            TestTask task1 = new TestTask();

            engine.AddTask(task1);

            engine.RemoveTask(task1.TaskID);

            Assert.IsTrue(!engine.HasTask(task1.TaskID));
            engine.Destroy();
        }

        #endregion

    }
}
