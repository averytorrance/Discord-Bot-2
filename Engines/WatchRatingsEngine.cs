using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DiscordBot.UserProfile;

namespace DiscordBot.Engines
{
    class WatchRatingsEngine
    {
        public WatchEntry CreateWatchEntry(DiscordMessage message)
        {
            List<string> keys;
            string name;
            int? year;
            bool isTV;
            GetInformation(message.Content, out keys, out name, out year, out isTV);

            WatchEntry entry = new WatchEntry()
            {
                MessageID = message.Id,
                EntryTime = message.Timestamp,
                Keys = keys,
                Name = name,
                Year = year,
                IsTV = isTV,
            };

            return entry;

        }

        public bool IsValidTVName(string name)
        {
            return GetTVYear(name) != null || name.Substring(name.Length - 4, name.Length) == "(TV)";
        }


        /// <summary>
        /// With a title of the form "name (year)", gets the year
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int? GetTVYear(string name)
        {
            if (name == "")
            {
                return null;
            }
            if (name.Split('(').Length == 0)
            {
                return null;
            }

            string _year = name.Split('(')[name.Split('(').Length - 1];
            if (_year[_year.Length - 1] == ')' && _year.Length == 8)
            {
                return Int32.Parse(_year.Substring(_year.Length - 5, _year.Length - 2));
            }
            return null;
        }

        public int? GetMovieYear(string name)
        {
            if (name == "")
            {
                return null;
            }
            if (name.Split('(').Length == 0)
            {
                return null;
            }

            string _year = name.Split('(')[name.Split('(').Length - 1];
            if (_year[_year.Length - 1] == ')' && _year.Length == 5)
            {
                return Int32.Parse(_year.Substring(_year.Length - 5, _year.Length - 2));
            }
            return null;
        }

        public List<string> GetKeys(string messageContent)
        {
            if (messageContent == "")
            {
                return null;
            }

            List<string> keys = new List<string>();

            if (messageContent[0] == '[')
            {
                string x = messageContent.Split(']')[0];
                x = x.Split('[')[0];

                keys = x.Split(',').ToList();

            }
            return keys;

        }

        public string GetName(string messageContent)
        {
            string name = messageContent;
            if (messageContent.Contains(']'))
            {
                name = messageContent.Split(']')[1];
            }
            if(GetTVYear(messageContent) != null || GetMovieYear(messageContent) != null)
            {
                name = name.Split('(')[name.Split('(').Length - 2];
            }

            return name;
        }

        public void GetInformation(string messageContent, out List<string> Keys, out string name, out int? year, out bool IsTV)
        {
            Keys = GetKeys(messageContent);
            IsTV = false;
            if (IsValidTVName(messageContent))
            {
                IsTV = true;
                year = GetTVYear(messageContent);
            }
            else
            {
                year = GetMovieYear(messageContent);
            }
            name = GetName(messageContent);
        }

    }

    public class WatchEntry
    {
        public ulong MessageID;
        public DateTimeOffset EntryTime;
        public string Name;
        public int? Year;
        public List<WatchRating> Ratings;
        public List<string> Keys;
        public bool IsTV = false;

    }

    public class WatchRating
    {
        public DUser User;
        public float Rating;
    }
}
