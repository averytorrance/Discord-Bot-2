using Microsoft.VisualStudio.TestTools.UnitTesting;
using DiscordBot.Classes;
using System;
using DiscordBot.WatchRatings;
using System.Collections.Generic;

namespace DiscortBotUnitTests.Classes
{

    [TestClass]
    public class WatchEntryTests
    {
        #region Equals
        [TestMethod]
        public void Equals_True()
        {
            WatchEntry entry1 = new WatchEntry()
            {
                MessageID = 1,
                ServerID = 1,
            };

            WatchEntry entry2 = new WatchEntry()
            {
                MessageID = 1,
                ServerID = 1,
            };

            Assert.IsTrue(entry1.Equals(entry2));
        }

        [TestMethod]
        public void Equals_TrueSelf()
        {
            WatchEntry entry1 = new WatchEntry()
            {
                MessageID = 1,
                ServerID = 1,
            };

            Assert.IsTrue(entry1.Equals(entry1));
        }

        [TestMethod]
        public void Equals_False()
        {
            WatchEntry entry1 = new WatchEntry()
            {
                MessageID = 1,
                ServerID = 1,
            };

            WatchEntry entry2 = new WatchEntry()
            {
                MessageID = 2,
                ServerID = 1,
            };

            Assert.IsFalse(entry1.Equals(entry2));
        }

        [TestMethod]
        public void Equals_FalseDiffServer()
        {
            WatchEntry entry1 = new WatchEntry()
            {
                MessageID = 1,
                ServerID = 1,
            };

            WatchEntry entry2 = new WatchEntry()
            {
                MessageID = 2,
                ServerID = 2,
            };

            Assert.IsFalse(entry1.Equals(entry2));
        }

        [TestMethod]
        public void Equals_FalseSameName()
        {
            WatchEntry entry1 = new WatchEntry()
            {
                MessageID = 1,
                ServerID = 1,
                Name = "Terminator",
                Year=1998
            };

            WatchEntry entry2 = new WatchEntry()
            {
                MessageID = 2,
                ServerID = 1,
                Name = "Terminator",
                Year = 1998
            };

            Assert.IsFalse(entry1.Equals(entry2));
        }

        #endregion

        #region TitleString

        #region Not Archived
        [TestMethod]
        public void TitleString_MovieNotArchived()
        {
            WatchEntry entry = new WatchEntry()
            {
                MessageID = 1,
                ServerID = 1,
                Name = "Face/Off",
                Year = 1998,
                Keys = new List<string>(),
            };

            Assert.AreEqual(entry.TitleString(), "Face/Off (1998)");
        }

        [TestMethod]
        public void TitleString_Movie2NotArchived()
        {
            WatchEntry entry = new WatchEntry()
            {
                MessageID = 1,
                ServerID = 1,
                Name = "Face/Off",
                Year = 1998,
                IsTV=false,
                Keys = new List<string>(),
            };

            Assert.AreEqual(entry.TitleString(), "Face/Off (1998)");
        }

        [TestMethod]
        public void TitleString_MovieNotArchivedNoYear()
        {
            WatchEntry entry = new WatchEntry()
            {
                MessageID = 1,
                ServerID = 1,
                Name = "Face/Off",
                IsTV = false,
                Keys = new List<string>(),
            };

            Assert.AreEqual(entry.TitleString(), "Face/Off");
        }

        [TestMethod]
        public void TitleString_TVNotArchived()
        {
            WatchEntry entry = new WatchEntry()
            {
                MessageID = 1,
                ServerID = 1,
                Name = "The Walking Dead",
                Year = 2010,
                IsTV = true,
                Keys = new List<string>(),
            };

            Assert.AreEqual(entry.TitleString(), "The Walking Dead (TV 2010)");
        }

        [TestMethod]
        public void TitleString_TVNotArchivedNoYear()
        {
            WatchEntry entry = new WatchEntry()
            {
                MessageID = 1,
                ServerID = 1,
                Name = "The Walking Dead",
                IsTV = true,
                Keys = new List<string>(),
            };

            Assert.AreEqual(entry.TitleString(), "The Walking Dead (TV)");
        }
        #endregion

        #region Archived
        [TestMethod]
        public void TitleString_MovieArchived()
        {
            WatchEntry entry = new WatchEntry()
            {
                MessageID = 1,
                ServerID = 1,
                Name = "Face/Off",
                Year = 1998,
                Keys = new List<string>() { "Archived" },
            };

            Assert.AreEqual(entry.TitleString(), "[Archived] Face/Off (1998)");
        }

        [TestMethod]
        public void TitleString_Movie2Archived()
        {
            WatchEntry entry = new WatchEntry()
            {
                MessageID = 1,
                ServerID = 1,
                Name = "Face/Off",
                Year = 1998,
                IsTV = false,
                Keys = new List<string>() { "Archived" },
            };

            Assert.AreEqual(entry.TitleString(), "[Archived] Face/Off (1998)");
        }

        [TestMethod]
        public void TitleString_MovieArchivedNoYear()
        {
            WatchEntry entry = new WatchEntry()
            {
                MessageID = 1,
                ServerID = 1,
                Name = "Face/Off",
                IsTV = false,
                Keys = new List<string>() { "Archived" },
            };

            Assert.AreEqual(entry.TitleString(), "[Archived] Face/Off");
        }

        [TestMethod]
        public void TitleString_TVArchived()
        {
            WatchEntry entry = new WatchEntry()
            {
                MessageID = 1,
                ServerID = 1,
                Name = "The Walking Dead",
                Year = 2010,
                IsTV = true,
                Keys = new List<string>() { "Archived" },
            };

            Assert.AreEqual(entry.TitleString(), "[Archived] The Walking Dead (TV 2010)");
        }

        [TestMethod]
        public void TitleString_TVArchivedNoYear()
        {
            WatchEntry entry = new WatchEntry()
            {
                MessageID = 1,
                ServerID = 1,
                Name = "The Walking Dead",
                IsTV = true,
                Keys = new List<string>() { "Archived" },
            };

            Assert.AreEqual(entry.TitleString(), "[Archived] The Walking Dead (TV)");
        }

        #endregion

        #endregion

        #region ToString
        [TestMethod]
        public void ToString_NoRatings()
        {
            WatchEntry entry = new WatchEntry()
            {
                MessageID = 1,
                ServerID = 1,
                Name = "Face/Off",
                Year = 1998,
                Keys = new List<string>(),
            };

            Assert.AreEqual(entry.ToString(), "Face/Off (1998)\nNo Ratings\n");
        }

        [TestMethod]
        public void ToString_1Rating()
        {
            WatchEntry entry = new WatchEntry()
            {
                MessageID = 1,
                ServerID = 1,
                Name = "Face/Off",
                Year = 1998,
                Ratings = new Dictionary<ulong, double>() 
                {
                    {1, 1}
                },
                Keys = new List<string>(),
            };

            Assert.AreEqual(entry.ToString(), "Face/Off (1998)\nUsername: N/A   1\nAverage: 1\n");
        }

        [TestMethod]
        public void ToString_2Rating()
        {
            WatchEntry entry = new WatchEntry()
            {
                MessageID = 1,
                ServerID = 1,
                Name = "Face/Off",
                Year = 1998,
                Ratings = new Dictionary<ulong, double>()
                {
                    {1, 5},
                    {2, 10}
                },
                Keys = new List<string>(),
            };

            Assert.AreEqual(entry.ToString(), "Face/Off (1998)\n" +
                "Username: N/A   5\n" +
                "Username: N/A   10\n" +
                "Average: 7.5\n");
        }
        #endregion

        #region GetUserRating, HasRating, and IsArchived
        [TestMethod]
        [ExpectedException(typeof(Exception), "No user in Ratings Dictionary")]
        public void GetUserRating_NonExistentUser()
        {
            WatchEntry entry = new WatchEntry()
            {
                MessageID = 1,
                ServerID = 1,
                Name = "Face/Off",
                Year = 1998,
                Ratings = new Dictionary<ulong, double>()
                {
                    {1, 5},
                    {2, 10}
                },
                Keys = new List<string>(),
            };

            entry.GetUserRating(3);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "No user in Ratings Dictionary")]
        public void GetUserRating_NoRatings()
        {
            WatchEntry entry = new WatchEntry()
            {
                MessageID = 1,
                ServerID = 1,
                Name = "Face/Off",
                Year = 1998,
                Keys = new List<string>(),
            };

            entry.GetUserRating(1);
        }

        [TestMethod]
        public void GetUserRating_UserRating()
        {
            WatchEntry entry = new WatchEntry()
            {
                MessageID = 1,
                ServerID = 1,
                Name = "Face/Off",
                Year = 1998,
                Ratings = new Dictionary<ulong, double>()
                {
                    {1, 5},
                    {2, 10}
                },
                Keys = new List<string>(),
            };

            Assert.AreEqual(entry.GetUserRating(2), 10);
        }

        [TestMethod]
        public void HasRatings_True()
        {
            WatchEntry entry = new WatchEntry()
            {
                MessageID = 1,
                ServerID = 1,
                Name = "Face/Off",
                Year = 1998,
                Ratings = new Dictionary<ulong, double>()
                {
                    {1, 5},
                },
                Keys = new List<string>(),
            };

            Assert.IsTrue(entry.HasRatings());
        }

        [TestMethod]
        public void HasRatings_False()
        {
            WatchEntry entry = new WatchEntry()
            {
                MessageID = 1,
                ServerID = 1,
                Name = "Face/Off",
                Year = 1998,
                Keys = new List<string>(),
            };

            Assert.IsFalse(entry.HasRatings());
        }

        [TestMethod]
        public void IsArchived_FalseWithKey()
        {
            WatchEntry entry = new WatchEntry()
            {
                MessageID = 1,
                ServerID = 1,
                Name = "Face/Off",
                Year = 1998,
                Keys = new List<string>() { "Not Archived"},
            };

            Assert.IsFalse(entry.IsArchived());
        }

        [TestMethod]
        public void IsArchived_FalseNoKey()
        {
            WatchEntry entry = new WatchEntry()
            {
                MessageID = 1,
                ServerID = 1,
                Name = "Face/Off",
                Year = 1998,
            };

            Assert.IsFalse(entry.IsArchived());
        }

        [TestMethod]
        public void IsArchived_True()
        {
            WatchEntry entry = new WatchEntry()
            {
                MessageID = 1,
                ServerID = 1,
                Name = "Face/Off",
                Year = 1998,
                Keys = new List<string>() { "Archived" },
            };

            Assert.IsTrue(entry.IsArchived());
        }

        [TestMethod]
        public void IsArchived_TrueMultipleKeys()
        {
            WatchEntry entry = new WatchEntry()
            {
                MessageID = 1,
                ServerID = 1,
                Name = "Face/Off",
                Year = 1998,
                Keys = new List<string>() { "This is a key", "Archived" },
            };

            Assert.IsTrue(entry.IsArchived());
        }
        #endregion

        #region Should Merge

        #region True Cases
        [TestMethod]
        public void ShouldMerge_Yes_SameName()
        {
            string name = "Test Name";
            int year = 2007;
            WatchEntry originalEntry = new WatchEntry()
            {
                ServerID = 1,
                MessageID = 1,
                Name = name,
                Year = year,
                IsTV = false
            };
            WatchEntry newEntry = new WatchEntry()
            {
                ServerID = 1,
                MessageID = 2,
                Name = name,
                Year = year,
                IsTV = false
            };

            Assert.IsTrue(WatchEntry.ShouldMerge(originalEntry, newEntry));
        }

        [TestMethod]
        public void ShouldMerge_Yes_SimiliarName()
        {
            int year = 2007;
            WatchEntry originalEntry = new WatchEntry()
            {
                ServerID = 1,
                MessageID = 1,
                Name = "Léon: The Professional",
                Year = year,
                IsTV = false
            };
            WatchEntry newEntry = new WatchEntry()
            {
                ServerID = 1,
                MessageID = 2,
                Name = "Leon The Professional",
                Year = year,
                IsTV = false
            };

            Assert.IsTrue(WatchEntry.ShouldMerge(originalEntry, newEntry));
        }

        [TestMethod]
        public void ShouldMerge_Yes_SameNameNullYear()
        {
            string name = "Test Name";
            int year = 2007;
            WatchEntry originalEntry = new WatchEntry()
            {
                ServerID = 1,
                MessageID = 1,
                Name = name,
                Year = year,
                IsTV = false
            };
            WatchEntry newEntry = new WatchEntry()
            {
                ServerID = 1,
                MessageID = 2,
                Name = name,
                IsTV = false
            };

            Assert.IsTrue(WatchEntry.ShouldMerge(originalEntry, newEntry));
        }

        [TestMethod]
        public void ShouldMerge_Yes_SimiliarNameNullYear()
        {
            int year = 2007;
            WatchEntry originalEntry = new WatchEntry()
            {
                ServerID = 1,
                MessageID = 1,
                Name = "Léon: The Professional",
                Year = year,
                IsTV = false
            };
            WatchEntry newEntry = new WatchEntry()
            {
                ServerID = 1,
                MessageID = 2,
                Name = "Leon The Professional",
                IsTV = false
            };

            Assert.IsTrue(WatchEntry.ShouldMerge(originalEntry, newEntry));
        }

        #endregion

        #region False Cases
        [TestMethod]
        public void ShouldMerge_No_DifferentName()
        {
            int year = 2007;
            WatchEntry originalEntry = new WatchEntry()
            {
                ServerID = 1,
                MessageID = 1,
                Name = "Léon: The Professional",
                Year = year,
                IsTV = false
            };
            WatchEntry newEntry = new WatchEntry()
            {
                ServerID = 1,
                MessageID = 2,
                Name = "The Terminator",
                Year = year,
                IsTV = false
            };

            Assert.IsTrue(!WatchEntry.ShouldMerge(originalEntry, newEntry));
        }

        [TestMethod]
        public void ShouldMerge_No_SameIDs()
        {
            string name = "The Terminator";
            int year = 2007;
            ulong id = 1;
            WatchEntry originalEntry = new WatchEntry()
            {
                ServerID = 1,
                MessageID = id,
                Name = name,
                Year = year,
                IsTV = false
            };
            WatchEntry newEntry = new WatchEntry()
            {
                ServerID = 1,
                MessageID = id,
                Name = name,
                Year = year,
                IsTV = false
            };

            Assert.IsTrue(!WatchEntry.ShouldMerge(originalEntry, newEntry));
        }

        [TestMethod]
        public void ShouldMerge_No_DifferentYears()
        {
            string name = "The Terminator";
            WatchEntry originalEntry = new WatchEntry()
            {
                ServerID = 1,
                MessageID = 1,
                Name = name,
                Year = 2009,
                IsTV = false
            };
            WatchEntry newEntry = new WatchEntry()
            {
                ServerID = 1,
                MessageID = 2,
                Name = name,
                Year = 2010,
                IsTV = false
            };

            Assert.IsTrue(!WatchEntry.ShouldMerge(originalEntry, newEntry));
        }

        [TestMethod]
        public void ShouldMerge_No_DifferentTypes()
        {
            string name = "Test Name";
            int year = 2007;
            WatchEntry originalEntry = new WatchEntry()
            {
                ServerID = 1,
                MessageID = 1,
                Name = name,
                Year = year,
                IsTV = true
            };
            WatchEntry newEntry = new WatchEntry()
            {
                ServerID = 1,
                MessageID = 2,
                Name = name,
                Year = year,
                IsTV = false
            };

            Assert.IsTrue(!WatchEntry.ShouldMerge(originalEntry, newEntry));
        }

        [TestMethod]
        public void ShouldMerge_No_DifferentServers()
        {
            string name = "Test Name";
            int year = 2007;
            WatchEntry originalEntry = new WatchEntry()
            {
                ServerID  = 1,
                MessageID = 1,
                Name = name,
                Year = year,
                IsTV = true
            };
            WatchEntry newEntry = new WatchEntry()
            {
                ServerID = 2,
                MessageID = 2,
                Name = name,
                Year = year,
                IsTV = false
            };

            Assert.IsTrue(!WatchEntry.ShouldMerge(originalEntry, newEntry));
        }


        #endregion

        #endregion

        #region Merge
        [TestMethod]
        public void Merge_MessageID()
        {
            DateTimeOffset entryTime = DateTimeOffset.UtcNow;

            List<WatchEntry> entries = new List<WatchEntry>() {
                new WatchEntry()
                {
                    MessageID = 1,
                    EntryTime = entryTime,
                    Name = "Test Entry"
                },
                new WatchEntry()

                {
                    MessageID = 2,
                    EntryTime = DateTimeOffset.UtcNow.AddYears(1),
                    Name = "Test Entry"
                }
            };

            Assert.AreEqual(WatchEntry.Merge(entries).MessageID, (ulong)1);
        }

        [TestMethod]
        public void Merge_EntryTime()
        {
            DateTimeOffset entryTime = DateTimeOffset.UtcNow;

            List<WatchEntry> entries = new List<WatchEntry>() {
                new WatchEntry()
                {
                    MessageID = 1,
                    EntryTime = entryTime,
                    Name = "Test Entry"
                },
                new WatchEntry()

                {
                    MessageID = 2,
                    EntryTime = DateTimeOffset.UtcNow.AddYears(1),
                    Name = "Test Entry"
                }
            };

            Assert.AreEqual(WatchEntry.Merge(entries).EntryTime, entryTime);
        }

        [TestMethod]
        public void Merge_YearFirst()
        {
            DateTimeOffset entryTime = DateTimeOffset.UtcNow;

            List<WatchEntry> entries = new List<WatchEntry>() {
                new WatchEntry()
                {
                    MessageID = 1,
                    EntryTime = entryTime,
                    Year=2001,
                    Name = "Test Entry"
                },
                new WatchEntry()

                {
                    MessageID = 2,
                    EntryTime = DateTimeOffset.UtcNow.AddYears(1),
                    Name = "Test Entry"
                }
            };

            Assert.AreEqual(WatchEntry.Merge(entries).Year, 2001);
        }

        [TestMethod]
        public void Merge_YearSecond()
        {
            DateTimeOffset entryTime = DateTimeOffset.UtcNow;

            List<WatchEntry> entries = new List<WatchEntry>() {
                new WatchEntry()
                {
                    MessageID = 1,
                    EntryTime = entryTime,
                    Name = "Test Entry"

                },
                new WatchEntry()

                {
                    MessageID = 2,
                    EntryTime = DateTimeOffset.UtcNow.AddYears(1),
                    Year=2001,
                    Name = "Test Entry"
                }
            };

            Assert.AreEqual(WatchEntry.Merge(entries).Year, 2001);
        }

        [TestMethod]
        public void Merge_IsMerged()
        {
            DateTimeOffset entryTime = DateTimeOffset.UtcNow;

            List<WatchEntry> entries = new List<WatchEntry>() {
                new WatchEntry()
                {
                    MessageID = 1,
                    EntryTime = DateTimeOffset.UtcNow,
                    Name = "Test Entry"
                },
                new WatchEntry()
                {
                    MessageID = 2,
                    EntryTime = DateTimeOffset.UtcNow.AddYears(1),
                    Name = "Test Entry"
                }
            };

            Assert.IsTrue(WatchEntry.Merge(entries).IsMerged);
        }

        [TestMethod]
        public void Merge_RatingsAllInFirst()
        {
            DateTimeOffset entryTime = DateTimeOffset.UtcNow;

            List<WatchEntry> entries = new List<WatchEntry>() {
                new WatchEntry()
                {
                    MessageID = 1,
                    Name = "Test Entry",
                    Ratings = new Dictionary<ulong, double>()
                    {
                        {1, 5},
                        {2, 10}
                    },
                },
                new WatchEntry()

                {
                    MessageID = 2,
                    Name = "Test Entry"
                }
            };

            Dictionary<ulong, double> expectedRatings = new Dictionary<ulong, double>()
            {
                {1, 5},
                {2, 10}
            };

            Assert.IsTrue(RatingsAreEqual(WatchEntry.Merge(entries), expectedRatings));
        }

        [TestMethod]
        public void Merge_RatingsAllInSecond()
        {
            DateTimeOffset entryTime = DateTimeOffset.UtcNow;

            List<WatchEntry> entries = new List<WatchEntry>() {
                new WatchEntry()
                {
                    MessageID = 1,
                    Name = "Test Entry",
                },
                new WatchEntry()

                {
                    MessageID = 2,
                    Name = "Test Entry",
                    Ratings = new Dictionary<ulong, double>()
                    {
                        {1, 5},
                        {2, 10}
                    },
                }
            };

            Dictionary<ulong, double> expectedRatings = new Dictionary<ulong, double>()
            {
                {1, 5},
                {2, 10}
            };

            Assert.IsTrue(RatingsAreEqual(WatchEntry.Merge(entries), expectedRatings));
        }

        [TestMethod]
        public void Merge_RatingsMixed()
        {
            DateTimeOffset entryTime = DateTimeOffset.UtcNow;

            List<WatchEntry> entries = new List<WatchEntry>() {
                new WatchEntry()
                {
                    MessageID = 1,
                    Name = "Test Entry",
                    Ratings = new Dictionary<ulong, double>()
                    {
                        {2, 10}
                    },
                },
                new WatchEntry()

                {
                    MessageID = 2,
                    Name = "Test Entry",
                    Ratings = new Dictionary<ulong, double>()
                    {
                        {1, 5},
                    },
                }
            };

            Dictionary<ulong, double> expectedRatings = new Dictionary<ulong, double>()
            {
                {1, 5},
                {2, 10}
            };

            Assert.IsTrue(RatingsAreEqual(WatchEntry.Merge(entries), expectedRatings));
        }

        [TestMethod]
        public void Merge_RatringsDupeUsers()
        {
            DateTimeOffset entryTime = DateTimeOffset.UtcNow;

            List<WatchEntry> entries = new List<WatchEntry>() {
                new WatchEntry()
                {
                    MessageID = 1,
                    Name = "Test Entry",
                    Ratings = new Dictionary<ulong, double>()
                    {
                        {1, 5},
                        {2, 10}
                    },
                },
                new WatchEntry()

                {
                    MessageID = 2,
                    Name = "Test Entry",
                    Ratings = new Dictionary<ulong, double>()
                    {
                        {1, 10},
                    },
                }
            };

            Dictionary<ulong, double> expectedRatings = new Dictionary<ulong, double>()
            {
                {1, 5},
                {2, 10}
            };

            Assert.IsTrue(RatingsAreEqual(WatchEntry.Merge(entries), expectedRatings));
        }

        /// <summary>
        /// Checks if the ratings are equal to the input dictionary
        /// </summary>
        /// <returns></returns>
        public bool RatingsAreEqual(WatchEntry entry, Dictionary<ulong, double> ratings)
        {
            if(entry.Ratings.Count != ratings.Count)
            {
                return false;
            }

            foreach(ulong id in ratings.Keys)
            {
                double ratingExpected;
                double ratingActual;
                if(!ratings.TryGetValue(id, out ratingExpected))
                {
                    return false;
                }
                if(!entry.Ratings.TryGetValue(id, out ratingActual))
                {
                    return false;
                }

                if(ratingExpected != ratingActual)
                {
                    return false;
                }
            }

            return true;
        }


        #endregion

    }
}
