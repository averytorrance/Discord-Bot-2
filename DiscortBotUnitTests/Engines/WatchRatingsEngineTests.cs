using Microsoft.VisualStudio.TestTools.UnitTesting;
using DiscordBot.Engines;
using System.Collections.Generic;
using System.Linq;
using DiscordBot.WatchRatings;

namespace DiscordBotUnitTests
{
    [TestClass]
    public class WatchRatingsEngineTests
    {

        #region WatchRatingsEngine Search

        #region Name Search
        [TestMethod]
        public void Search_Name_ExactName()
        {
            string search = "Get In";

            WatchRatingsEngineState data = LoadTestData();
            List < WatchEntry > watchEntries= data.MergedEntries.Values.ToList().OrderBy(x => x.MessageID).ToList();
            watchEntries = watchEntries.Where(x => x.Name == search).ToList();

            WatchSearch watchSearch = new WatchSearch()
            {
                SearchTerm = search
            };

            List<WatchEntry> results = data.Search(watchSearch);

            Assert.IsTrue(results.SequenceEqual(watchEntries));
        }

        [TestMethod]
        public void Search_Name_EndString()
        {
            string search = "inator";

            WatchRatingsEngineState data = LoadTestData();
            List<WatchEntry> watchEntries = data.MergedEntries.Values.ToList().OrderBy(x => x.MessageID).ToList();
            watchEntries = watchEntries.Where(x => x.Name.EndsWith(search)).ToList();

            WatchSearch watchSearch = new WatchSearch()
            {
                SearchTerm = search
            };

            List<WatchEntry> results = data.Search(watchSearch);

            Assert.IsTrue(results.SequenceEqual(watchEntries));
        }


        [TestMethod]
        public void Search_Name_StartString()
        {
            string search = "Get";

            WatchRatingsEngineState data = LoadTestData();
            List<WatchEntry> watchEntries = data.MergedEntries.Values.ToList().OrderBy(x => x.MessageID).ToList();
            watchEntries = watchEntries.Where(x => x.Name.StartsWith(search)).ToList();

            WatchSearch watchSearch = new WatchSearch()
            {
                SearchTerm = search
            };

            List<WatchEntry> results = data.Search(watchSearch);

            Assert.IsTrue(results.SequenceEqual(watchEntries));
        }

        [TestMethod]
        public void Search_Name_MiddleString()
        {
            string search = "Term";

            WatchRatingsEngineState data = LoadTestData();
            List<WatchEntry> watchEntries = data.MergedEntries.Values.ToList().OrderBy(x => x.MessageID).ToList();
            watchEntries = watchEntries.Where(x => x.Name == "The Terminator").ToList();

            WatchSearch watchSearch = new WatchSearch()
            {
                SearchTerm = search
            };

            List<WatchEntry> results = data.Search(watchSearch);

            Assert.IsTrue(results.SequenceEqual(watchEntries));
        }

        [TestMethod]
        public void Search_Name_AccentCharacterInSearch()
        {
            string search = "Tèrm";

            WatchRatingsEngineState data = LoadTestData();
            List<WatchEntry> watchEntries = data.MergedEntries.Values.ToList().OrderBy(x => x.MessageID).ToList();
            watchEntries = watchEntries.Where(x => x.Name == "The Terminator").ToList();

            WatchSearch watchSearch = new WatchSearch()
            {
                SearchTerm = search
            };

            List<WatchEntry> results = data.Search(watchSearch);

            Assert.IsTrue(results.SequenceEqual(watchEntries));
        }

        [TestMethod]
        public void Search_Name_AccentCharacterInName()
        {
            string search = "Léon";

            WatchRatingsEngineState data = LoadTestData();
            List<WatchEntry> watchEntries = data.MergedEntries.Values.ToList().OrderBy(x => x.MessageID).ToList();
            watchEntries = watchEntries.Where(x => x.Name == "Léon: The Professional").ToList();

            WatchSearch watchSearch = new WatchSearch()
            {
                SearchTerm = search
            };

            List<WatchEntry> results = data.Search(watchSearch);

            Assert.IsTrue(results.SequenceEqual(watchEntries));
        }

        [TestMethod]
        public void Search_Name_SpecialCharacter()
        {
            string search = "Leon: The";

            WatchRatingsEngineState data = LoadTestData();
            List<WatchEntry> watchEntries = data.MergedEntries.Values.ToList().OrderBy(x => x.MessageID).ToList();
            watchEntries = watchEntries.Where(x => x.Name == "Léon: The Professional").ToList();

            WatchSearch watchSearch = new WatchSearch()
            {
                SearchTerm = search
            };

            List<WatchEntry> results = data.Search(watchSearch);

            Assert.IsTrue(results.SequenceEqual(watchEntries));
        }
        #endregion

        #region Search Year
        [TestMethod]
        public void Search_Year_DefinedInput()
        {
            int search = 2020;

            WatchRatingsEngineState data = LoadTestData();
            List<WatchEntry> watchEntries = data.MergedEntries.Values.ToList().OrderBy(x => x.MessageID).ToList();
            watchEntries = watchEntries.Where(x => x.EntryTime.Year == search).ToList();

            WatchSearch watchSearch = new WatchSearch()
            {
                WatchYear = search
            };

            List<WatchEntry> results = data.Search(watchSearch);

            Assert.IsTrue(results.SequenceEqual(watchEntries));
        }

        [TestMethod]
        public void Search_ReleaseYear_DefinedInput()
        {
            int search = 2018;

            WatchRatingsEngineState data = LoadTestData();
            List<WatchEntry> watchEntries = data.MergedEntries.Values.ToList().OrderBy(x => x.MessageID).ToList();
            watchEntries = watchEntries.Where(x => x.Year == search).ToList();

            WatchSearch watchSearch = new WatchSearch()
            {
                ReleaseYear = search
            };

            List<WatchEntry> results = data.Search(watchSearch);

            Assert.IsTrue(results.SequenceEqual(watchEntries));
        }

        #endregion

        #region User Search
        [TestMethod]
        public void Search_User_SingleUser()
        {
            List<ulong> search = new List<ulong>() { 1 };

            WatchRatingsEngineState data = LoadTestData();
            List<WatchEntry> watchEntries = data.MergedEntries.Values.ToList().OrderBy(x => x.MessageID).ToList();
            watchEntries = watchEntries.Where(x => x.MessageID == 1 || x.MessageID == 3).ToList();

            WatchSearch watchSearch = new WatchSearch()
            {
                UserIDs = search
            };

            List<WatchEntry> results = data.Search(watchSearch);

            Assert.IsTrue(results.SequenceEqual(watchEntries));
        }

        [TestMethod]
        public void Search_User_MultipleUsers()
        {
            List<ulong> search = new List<ulong>() { 2,3 };

            WatchRatingsEngineState data = LoadTestData();
            List<WatchEntry> watchEntries = data.MergedEntries.Values.ToList().OrderBy(x => x.MessageID).ToList();
            watchEntries = watchEntries.Where(x => x.MessageID == 2 ).ToList();

            WatchSearch watchSearch = new WatchSearch()
            {
                UserIDs = search
            };

            List<WatchEntry> results = data.Search(watchSearch);

            Assert.IsTrue(results.SequenceEqual(watchEntries));
        }

        #endregion

        #region Statistic Search
        [TestMethod]
        public void Search_UserScore_Equals()
        {
            double search = 5.0;
            List<ulong> users = new List<ulong>() { 1 };

            WatchRatingsEngineState data = LoadTestData();
            List<WatchEntry> watchEntries = data.MergedEntries.Values.ToList().OrderBy(x => x.MessageID).ToList();
            watchEntries = watchEntries.Where(x => x.MessageID == 1).ToList();

            WatchSearch watchSearch = new WatchSearch()
            {
                UserIDs = users,
                UserScore = search
            };

            List<WatchEntry> results = data.Search(watchSearch);

            Assert.IsTrue(results.SequenceEqual(watchEntries));
        }

        [TestMethod]
        public void Search_UserScore_LessThan()
        {
            double search = 7.0;
            List<ulong> users = new List<ulong>() { 2 };

            WatchRatingsEngineState data = LoadTestData();
            List<WatchEntry> watchEntries = data.MergedEntries.Values.ToList().OrderBy(x => x.MessageID).ToList();
            watchEntries = watchEntries.Where(x => x.MessageID == 2).ToList();

            WatchSearch watchSearch = new WatchSearch()
            {
                UserIDs = users,
                UserScore = search
            };
            watchSearch.Operator = watchSearch.LessThanEqFunction();

            List<WatchEntry> results = data.Search(watchSearch);

            Assert.IsTrue(results.SequenceEqual(watchEntries));
        }

        [TestMethod]
        public void Search_UserScore_NullSearch()
        {
            WatchRatingsEngineState data = LoadTestData();
            List<WatchEntry> watchEntries = data.MergedEntries.Values.ToList().OrderBy(x => x.MessageID).ToList();

            WatchSearch watchSearch = new WatchSearch();

            List<WatchEntry> results = data.Search(watchSearch);

            Assert.IsTrue(results.SequenceEqual(watchEntries));
        }

        #endregion

        #endregion
        /// <summary>
        /// Loads test data. 
        /// </summary>
        /// <returns></returns>
        public WatchRatingsEngineState LoadTestData()
        {
            JSONEngine engine = new JSONEngine();
            WatchRatingsEngineState data = engine.GenerateObject<WatchRatingsEngineState>("WatchRatingsEngineState.json");
            return data;
        }

    }
}
