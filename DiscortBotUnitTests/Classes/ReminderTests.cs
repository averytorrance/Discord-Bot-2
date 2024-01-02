using Microsoft.VisualStudio.TestTools.UnitTesting;
using DiscordBot.Classes;
using System;

namespace DiscortBotUnitTests.Classes
{
    [TestClass]
    public class ReminderTests
    {
        #region Constructor
        
        [TestMethod]
        [ExpectedException(typeof(Exception), "Attempted to create a reminder with a non UTC DateKind")]
        public void Constructor_NonUTCInput()
        {
            Reminder reminder = new Reminder(1, "Test", 1, DateTime.Now);
        }

        [TestMethod]
        public void Constructor_UTCInput()
        {
            Reminder reminder = new Reminder(1, "Test", 1, DateTime.UtcNow);
            // No Exception means this passed
        }

        #endregion

        #region IsStale and IsReady

        [TestMethod]
        public void IsStale_False()
        {
            Reminder reminder = new Reminder(1, "Test", 1, DateTime.UtcNow);

            Assert.IsFalse(reminder.IsStale());
        }

        [TestMethod]
        public void IsStale_True()
        {
            Reminder reminder = new Reminder(1, "Test", 1, DateTime.UtcNow.AddMinutes(-3));

            Assert.IsTrue(reminder.IsStale());
        }

        [TestMethod]
        public void IsReady_False()
        {
            Reminder reminder = new Reminder(1, "Test", 1, DateTime.UtcNow.AddMinutes(1));

            Assert.IsFalse(reminder.ReadyToSend());
        }

        [TestMethod]
        public void IsReady_True()
        {
            Reminder reminder = new Reminder(1, "Test", 1, DateTime.UtcNow.AddMinutes(-1));

            Assert.IsTrue(reminder.ReadyToSend());
        }

        [TestMethod]
        public void IsReady_TrueEqual()
        {
            Reminder reminder = new Reminder(1, "Test", 1, DateTime.UtcNow);

            Assert.IsTrue(reminder.ReadyToSend());
        }

        #endregion

    }
}
