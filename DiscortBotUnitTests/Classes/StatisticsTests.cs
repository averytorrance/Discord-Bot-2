using Microsoft.VisualStudio.TestTools.UnitTesting;
using DiscordBot.Classes;
using System;
using System.Collections.Generic;

namespace DiscortBotUnitTests.Classes
{
    [TestClass]
    public class StatisticsTests
    {
        /// <summary>
        /// data set 1 for testing
        /// </summary>
        public List<double> DataSet1 = new List<double>()
        {
            1,2,3,4,5,6,7,8,9,10
        };

        /// <summary>
        /// data set 2 for testing
        /// </summary>
        public List<double> DataSet2 = new List<double>()
        {
            6.5,7,8,8
        };

        /// <summary>
        /// data set 2 for testing
        /// </summary>
        public List<double> DataSet3 = new List<double>()
        {
            4.5
        };

        /// <summary>
        /// The acceptable error in numerical checks
        /// </summary>
        public double Delta = .0001;

        [TestMethod]
        public void Statistics_NonNullData()
        {
            Statistics stats = new Statistics(new List<double>() { 1 });

            Assert.IsFalse(stats.NullData);
        }

        #region Null Set

        [TestMethod]
        public void Statistics_NullDataBoolCheck()
        {
            Statistics stats = new Statistics(new List<double>());

            Assert.IsTrue(stats.NullData);
        }

        [TestMethod]
        public void Statistics_NullSetString()
        {
            Statistics stats = new Statistics(new List<double>());

            Assert.AreEqual("Empty Data set. Unable to calculate statistics.", stats.ToString());
        }

        [TestMethod]
        public void Statistics_NullSetCount()
        {
            Statistics stats = new Statistics(new List<double>());

            Assert.AreEqual(0, stats.Count);
        }

        [TestMethod]
        public void Statistics_NullSetSum()
        {
            Statistics stats = new Statistics(new List<double>());

            Assert.AreEqual(0, stats.Sum);
        }

        [TestMethod]
        public void Statistics_NullSetMean()
        {
            Statistics stats = new Statistics(new List<double>());

            Assert.AreEqual(0, stats.Mean);
        }

        [TestMethod]
        public void Statistics_NullSetStandardDeviation()
        {
            Statistics stats = new Statistics(new List<double>());

            Assert.AreEqual(0, stats.StandardDeviation);
        }

        [TestMethod]
        public void Statistics_NullSetMaximum()
        {
            Statistics stats = new Statistics(new List<double>());

            Assert.AreEqual(0, stats.StandardDeviation);
        }

        [TestMethod]
        public void Statistics_NullSetMinimum()
        {
            Statistics stats = new Statistics(new List<double>());

            Assert.AreEqual(0, stats.StandardDeviation);
        }
        #endregion

        #region DataSet1

        [TestMethod]
        public void Statistics_DataSet1Sum()
        {
            
            Statistics stats = new Statistics(DataSet1);

            Assert.AreEqual(55, stats.Sum);
        }

        [TestMethod]
        public void Statistics_DataSet1Average()
        {

            Statistics stats = new Statistics(DataSet1);

            Assert.AreEqual(5.5, stats.Mean);
        }

        [TestMethod]
        public void Statistics_DataSet1StandardDeviation()
        {
            Statistics stats = new Statistics(DataSet1);

            Assert.AreEqual(2.8723, stats.StandardDeviation, Delta);
        }

        [TestMethod]
        public void Statistics_DataSet1Maximum()
        {
            Statistics stats = new Statistics(DataSet1);

            Assert.AreEqual(10, stats.Maximum);
        }

        [TestMethod]
        public void Statistics_DataSet1Minimum()
        {
            Statistics stats = new Statistics(DataSet1);

            Assert.AreEqual(1, stats.Minimum);
        }

        [TestMethod]
        public void Statistics_DataSet1Count()
        {
            Statistics stats = new Statistics(DataSet1);

            Assert.AreEqual(10, stats.Count);
        }

        #endregion

        #region DataSet2

        [TestMethod]
        public void Statistics_DataSet2Sum()
        {

            Statistics stats = new Statistics(DataSet2);

            Assert.AreEqual(29.5, stats.Sum);
        }

        [TestMethod]
        public void Statistics_DataSet2Average()
        {

            Statistics stats = new Statistics(DataSet2);

            Assert.AreEqual(7.375,stats.Mean);
        }

        [TestMethod]
        public void Statistics_DataSet2StandardDeviation()
        {
            Statistics stats = new Statistics(DataSet2);

            Assert.AreEqual(0.649519, stats.StandardDeviation, Delta);
        }

        [TestMethod]
        public void Statistics_DataSet2Maximum()
        {
            Statistics stats = new Statistics(DataSet2);

            Assert.AreEqual(8, stats.Maximum);
        }

        [TestMethod]
        public void Statistics_DataSet2Minimum()
        {
            Statistics stats = new Statistics(DataSet2);

            Assert.AreEqual(6.5, stats.Minimum);
        }

        [TestMethod]
        public void Statistics_DataSet2Count()
        {
            Statistics stats = new Statistics(DataSet2);

            Assert.AreEqual(4, stats.Count);
        }

        #endregion

        #region DataSet3

        [TestMethod]
        public void Statistics_DataSet3Sum()
        {

            Statistics stats = new Statistics(DataSet3);

            Assert.AreEqual(4.5, stats.Sum);
        }

        [TestMethod]
        public void Statistics_DataSet3Average()
        {

            Statistics stats = new Statistics(DataSet3);

            Assert.AreEqual(4.5, stats.Mean);
        }

        [TestMethod]
        public void Statistics_DataSet3StandardDeviation()
        {
            Statistics stats = new Statistics(DataSet3);

            Assert.AreEqual(0.0, stats.StandardDeviation);
        }

        [TestMethod]
        public void Statistics_DataSet3Maximum()
        {
            Statistics stats = new Statistics(DataSet3);

            Assert.AreEqual(4.5, stats.Maximum);
        }

        [TestMethod]
        public void Statistics_DataSet3Minimum()
        {
            Statistics stats = new Statistics(DataSet3);

            Assert.AreEqual(4.5, stats.Minimum);
        }

        [TestMethod]
        public void Statistics_DataSet3Count()
        {
            Statistics stats = new Statistics(DataSet3);

            Assert.AreEqual(1, stats.Count);
        }

        #endregion
    }
}
