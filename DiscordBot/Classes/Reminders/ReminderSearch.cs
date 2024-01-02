using System;
using System.Text.RegularExpressions;

namespace DiscordBot.Classes
{
    public class ReminderSearch
    {
        public ulong UserID;

        public string SearchTerm;

        /// <summary>
        /// Null includes if user is owner or subscriber
        /// True includes subscribed to reminders
        /// False includes owned reminders
        /// </summary>
        public bool? SearchSubscriptions = null;

        /// <summary>
        /// Null includes if the remider is recurring
        /// True includes only recurring reminders
        /// False includes single time reminders
        /// </summary>
        public bool? SearchRecurring = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userID"></param>
        public ReminderSearch (ulong userID)
        {
            UserID = userID;
        }

        /// <summary>
        /// Generates the search function associated with the values in this object
        /// </summary>
        /// <returns></returns>
        public Func<Reminder, bool> SearchFunction()
        {
            Func<Reminder, bool> func = (x) => _stringSearch(x) &&
                                               _userSearch(x) &&
                                               _recurringSearch(x);
            return func;
        }

        /// <summary>
        /// returns delegate to check if a search term matches a reminder message
        /// </summary>
        /// <returns></returns>
        private bool _stringSearch(Reminder reminder)
        {
            if (string.IsNullOrWhiteSpace(SearchTerm))
            {
                return true;
            }
            string search = StringUtils.ReplaceLoopHoles(SearchTerm.ToLower());
            search = StringUtils.StripSpecialCharacters(search);


            string entryName = StringUtils.ReplaceLoopHoles(reminder.Message.ToLower());
            entryName = StringUtils.StripSpecialCharacters(entryName);

            ///Use starts with if search is 3 characters or less
            if (search.Length > 3)
            {
                return Regex.IsMatch(entryName, search);
            }
            return entryName.StartsWith(search);
        }

        /// <summary>
        /// returns delegate to check if user owns a reminder or is subscribed to it
        /// </summary>
        /// <returns></returns>
        private bool _userSearch(Reminder reminder)
        {
            bool isOwner = reminder.OwnerId == UserID; ;
            bool isSubbed = reminder.IsUserSubscribed(UserID);
            bool result = false;
            switch (SearchSubscriptions)
            {
                case null: result = isOwner || isSubbed; break;
                case true: result = isSubbed; break;
                case false: result = isOwner; break;
            }
            return result;
        }

        /// <summary>
        /// returns delegate to check if a reminder is recurring
        /// </summary>
        /// <returns></returns>
        private bool _recurringSearch(Reminder reminder)
        {
            if(SearchRecurring == null)
            {
                return true;
            }
            return SearchRecurring == reminder.IsRecurring();
        }

        
    }
}
