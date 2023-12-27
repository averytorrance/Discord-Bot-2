using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DiscordBot.Classes;

namespace DiscordBot.WatchRatings
{
    public class WatchSearch
    {
        public string SearchTerm;
        public int? WatchYear;
        public int? ReleaseYear;
        public List<ulong> UserIDs;
        public bool? IsTV;
        public bool? HasValidationWarning;
        public double? UserScore;
        public Func<double, bool> Operator;

        /// <summary>
        /// Generates the search function associated with the values in this object
        /// </summary>
        /// <returns></returns>
        public Func<WatchEntry, bool> SearchFunction()
        {
            Func<WatchEntry, bool> func = (x) => _stringSearch(x) && _watchYear(x) && _releaseYear(x) && _hasUsers(x) && _entryType(x) && _hasWarning(x) && _userScore(x);
            return func;
        }

        /// <summary>
        /// Adds a user to the UserIDs list
        /// </summary>
        /// <param name="userID"></param>
        public void AddUser(ulong userID)
        {
            if(UserIDs == null)
            {
                UserIDs = new List<ulong>();
            }
            UserIDs.Add(userID);
        }

        /// <summary>
        /// Checks if a search is a null search
        /// </summary>
        /// <returns></returns>
        public bool IsNullSearch()
        {
            return this == new WatchSearch();
        }

        /// <summary>
        /// returns delegate to check if a watchentry has a specifc entry year
        /// </summary>
        /// <returns></returns>
        private bool _stringSearch(WatchEntry x)
        {
            if (string.IsNullOrWhiteSpace(SearchTerm))
            {
                return true;
            }
            string search = StringUtils.ReplaceLoopHoles(SearchTerm.ToLower());

            string entryName = StringUtils.ReplaceLoopHoles(x.Name.ToLower());
            ///Use starts with if search is 3 characters or less
            if (search.Length > 3)
            {
                return Regex.IsMatch(entryName, search);
            }
            return entryName.StartsWith(search);
        }

        /// <summary>
        /// returns delegate to check if a watchentry has a specifc entry year
        /// </summary>
        /// <returns></returns>
        private bool _watchYear(WatchEntry x)
        {
            if (WatchYear == null)
            {
                return true;
            }
            return x.EntryTime.Year == (int)WatchYear;
        }

        /// <summary>
        /// returns delegate to check if a watchentry has a specific year
        /// </summary>
        /// <returns></returns>
        private bool _releaseYear(WatchEntry x)
        {
            if (ReleaseYear == null)
            {
                return true;
            }
            return x.Year == (int)ReleaseYear;
        }

        /// <summary>
        /// returns delegate to check if a watchentry was rated by a list of users
        /// </summary>
        /// <returns></returns>
        private bool _hasUsers(WatchEntry x)
        {
            if (UserIDs == null || UserIDs.Count == 0)
            {
                return true;
            }
            foreach (ulong userID in UserIDs)
            {
                if (!x.Ratings.ContainsKey(userID))
                {
                    return false;
                }
            };
            return true;
        }

        /// <summary>
        /// returns delegate to check if IsTV
        /// </summary>
        /// <returns></returns>
        private bool _entryType(WatchEntry x)
        {
            if (IsTV == null)
            {
                return true;
            }
            return x.IsTV == IsTV;
        }

        /// <summary>
        /// returns delegate to check for validation warnings
        /// </summary>
        /// <returns></returns>
        private bool _hasWarning(WatchEntry x)
        {
            if (HasValidationWarning == null)
            {
                return true;
            }
            return x.HasValidationWarning == HasValidationWarning;
        }

        /// <summary>
        /// returns delegate to check for a specific average
        /// </summary>
        /// <returns></returns>
        private bool _userScore(WatchEntry x)
        {
            if (UserIDs == null || UserIDs.Count == 0 || UserScore == null)
            {
                return true;
            }

            if (Operator == null)
            {
                Operator = EqualsFunction();
            }

            foreach (ulong userID in UserIDs)
            {
                double score;
                x.Ratings.TryGetValue(userID, out score);
                if (!Operator(score))
                {
                    return false;
                }
            };
            return true;
        }

        /// <summary>
        /// Equals function. Value for Opteration field
        /// </summary>
        /// <returns></returns>
        public Func<double, bool> EqualsFunction()
        {
            return delegate (double val) { return val == UserScore; };
        }

        /// <summary>
        /// Not Equals function. Value for Opteration field
        /// </summary>
        /// <returns></returns>
        public Func<double, bool> NEQFunction()
        {
            return delegate (double val) { return val != UserScore; };
        }

        /// <summary>
        /// Greater than function. Value for Opteration field
        /// </summary>
        /// <returns></returns>
        public Func<double, bool> GreaterThanFunction()
        {
            return delegate (double val) { return val > UserScore; };
        }

        /// <summary>
        /// Greater than or equal to function. Value for Opteration field
        /// </summary>
        /// <returns></returns>
        public Func<double, bool> GreaterThanEqFunction()
        {
            return delegate (double val) { return val >= UserScore; };
        }

        /// <summary>
        /// Less than function. Value for Opteration field
        /// </summary>
        /// <returns></returns>
        public Func<double, bool> LessThanFunction()
        {
            return delegate (double val) { return val < UserScore; };
        }

        /// <summary>
        /// Less than or equal to function. Value for Opteration field
        /// </summary>
        /// <returns></returns>
        public Func<double, bool> LessThanEqFunction()
        {
            return delegate (double val) { return val <= UserScore; };
        }

    }
}
