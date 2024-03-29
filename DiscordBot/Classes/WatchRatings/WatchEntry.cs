﻿using System;
using System.Collections.Generic;
using System.Linq;
using DiscordBot.UserProfile;
using DiscordBot.Classes;
using DiscordBot.Engines;

namespace DiscordBot.WatchRatings
{
    public class WatchEntry
    {
        public ulong MessageID;
        public ulong ServerID;
        public DateTimeOffset EntryTime;
        public string Name;
        public int? Year;
        public Dictionary<ulong, double> Ratings = new Dictionary<ulong, double>();
        public List<string> Keys = new List<string>();
        public bool IsTV = false;
        public bool HasValidationWarning = false;
        public bool IsMerged = false;
        public List<ulong> MergedIDs = new List<ulong>();
        public bool HasManualChanges = false;

        /// <summary>
        /// Checks if a watchentry is the same entry as another entry.
        /// Checks if IDs are the same
        /// </summary>
        /// <param name="otherEntry"></param>
        /// <returns></returns>
        public bool Equals(WatchEntry otherEntry)
        {
            return MessageID == otherEntry.MessageID && ServerID == otherEntry.ServerID;
        }

        /// <summary>
        /// Returns the name of the entry including the year if it exists
        /// </summary>
        /// <returns></returns>
        public string TitleString()
        {
            string archived = "";
            if (IsArchived())
            {
                archived = "[Archived] ";
            }
            if (Year != null)
            {
                if (IsTV)
                {
                    return $"{archived}{Name} (TV {Year})";
                }
                return $"{archived}{Name} ({Year})";
            }
            if (IsTV)
            {
                return $"{archived}{Name} (TV)";
            }
            return $"{archived}{Name}";
        }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string title = TitleString();

            Statistics statistics = GetStats();

            DiscordUserEngine userEngine = new DiscordUserEngine();
            string scores = "";

            if(Ratings.Count == 0)
            {
                return $"{title}\nNo Ratings\n";
            }

            foreach (ulong userID in Ratings.Keys)
            {
                DUser user = userEngine.GetUser(userID);
                double rating = GetUserRating(userID);
                if (user == null)
                {
                    scores = $"{scores}Username: N/A   {rating}\n";
                }
                else
                {
                    scores = $"{scores}Username: {user.UserName}   {rating}\n";
                }
            }

            return $"{title}\n{scores}Average: {statistics.Mean}\n";
        }

        /// <summary>
        /// Gets a user rating for the ratings dictionary, given their ID
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public double GetUserRating(ulong userID)
        {
            double rating;
            if (!Ratings.TryGetValue(userID, out rating))
            {
                throw new Exception("No user in Ratings Dictionary");
            }
            return rating;
        }

        /// <summary>
        /// Checks if this watch entry has ratings
        /// </summary>
        /// <returns></returns>
        public bool HasRatings()
        {
            return Ratings.Count > 0;
        }

        /// <summary>
        /// Checks if the archived key is in the list of keys
        /// </summary>
        /// <returns></returns>
        public bool IsArchived()
        {
            return Keys.Contains("Archived");
        }

        #region Merge
        /// <summary>
        /// Merges a 2 watch entries
        /// </summary>
        /// <param name="entries"></param>
        /// <returns></returns>
        public static WatchEntry Merge(WatchEntry originalEntry, WatchEntry newEntry)
        {
            if (originalEntry.Equals(newEntry))
            {
                throw new Exception("Unable to merge equal Watch Entries");
            }
            List<WatchEntry> entries = new List<WatchEntry>();

            entries.Add(originalEntry);
            entries.Add(newEntry);

            return Merge(entries);
        }

        /// <summary>
        /// Merges a list of Watch Entries
        /// </summary>
        /// <param name="entries"></param>
        /// <returns></returns>
        public static WatchEntry Merge(List<WatchEntry> entries)
        {

            if (entries == null || entries.Count == 0)
            {
                return null;
            }
            if (entries.Count == 1)
            {
                return entries.First();
            }
            if (entries.Count != entries.Distinct().ToList().Count)
            {
                throw new Exception("Duplicate entries in merge list");
            }

            entries = entries.OrderBy(x => x.EntryTime).ToList();
            WatchEntry baseEntry = entries.First();

            //TODO; Merge Keys
            WatchEntry mergedEntry = new WatchEntry()
            {
                ServerID = baseEntry.ServerID,
                MessageID = baseEntry.MessageID,
                EntryTime = baseEntry.EntryTime,
                Name = baseEntry.Name,
                Year = baseEntry.Year,
                IsTV = baseEntry.IsTV,
                IsMerged = true,
                Ratings = baseEntry.Ratings
            };

            //Merge ratings and fill in null year if it is filled in during a later entry
            foreach (WatchEntry entry in entries)
            {
                //The merge entry starts off with information from the base entry.
                //No need to merge this. 
                if (baseEntry.Equals(entry))
                {
                    continue;
                }

                if(!WatchEntry.ShouldMerge(mergedEntry, entry))
                {
                    throw new Exception("Attempted to merge non-mergable entries");
                }

                if (mergedEntry.Year == null && entry.Year != null)
                {
                    mergedEntry.Year = entry.Year;
                }

                mergedEntry.MergedIDs.Add(entry.MessageID);

                //Merge Ratings
                foreach (ulong userID in entry.Ratings.Keys)
                {
                    //Only add  the oldest rating. 
                    //The list has already been ordered, 
                    //so we can assume the first instance is the oldest.
                    //Once added, we don't need to mess with it.
                    if (!mergedEntry.Ratings.ContainsKey(userID))
                    {
                        double rating;
                        entry.Ratings.TryGetValue(userID, out rating);
                        mergedEntry.Ratings.Add(userID, rating);
                    }
                }
            }

            return mergedEntry;

        }

        /// <summary>
        /// Checks if two entries should be merges
        /// </summary>
        /// <param name="originalEntry"></param>
        /// <param name="newEntry"></param>
        /// <returns></returns>
        public static bool ShouldMerge(WatchEntry originalEntry, WatchEntry newEntry)
        {
            bool DifferentIDs = originalEntry.MessageID != newEntry.MessageID;
            bool SameServerIDs = originalEntry.ServerID == newEntry.ServerID;
            bool SameTitle = StringUtils.StringSimiliar(originalEntry.Name, newEntry.Name);
            bool SameYear = originalEntry.Year == newEntry.Year;
            bool HasNullYear = (originalEntry.Year == null) || (newEntry.Year == null);
            bool SameType = originalEntry.IsTV == newEntry.IsTV;

            return SameServerIDs && DifferentIDs && SameTitle 
                && SameType && (HasNullYear || SameYear);
        }
        #endregion

        #region Statistic Methods

        /// <summary>
        /// Gets the average rating
        /// </summary>
        /// <returns></returns>
        public double GetAverage()
        {
            return GetStats().Mean;
        }

        public Statistics GetStats()
        {
            return new Statistics(GetRatings());
        }

        /// <summary>
        /// Gets the list of ratings
        /// </summary>
        /// <returns></returns>
        public List<double> GetRatings()
        {
            return Ratings.Values.OrderBy(x => x).ToList();
        }
        #endregion
    }
}
