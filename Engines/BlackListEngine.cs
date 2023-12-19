using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace DiscordBot.Engines
{
    public static class BlackListEngine
    {
        private static readonly List<string> _blackListedTerms = new List<string>()
        {
            "test"
        };

        /// <summary>
        /// Checks if a string has Blacklisted content
        /// </summary>
        /// <param name="content">string to check if contains blacklisted content</param>
        /// <returns>true if the string contains blacklisted content</returns>
        public static bool IsBlackListed(string content)
        {
            content = content.ToLower();
            content = _replaceLoopHoles(content);

            List<string> regEx = new List<string>();

            //Create list of blacklisted regex patters
            regEx.Add("booo+k+((?!choy))"); //Hardcoded checks for recurring variations of bok choy
            regEx.Add("bok+((?!choy))");
            foreach(string blacklisted in _blackListedTerms)
            {
                regEx.Add(addPlus(blacklisted));
            }

            //Regex pattern check for all possible blacklisted regex patterns
            foreach(string pattern in regEx)
            {
                if(Regex.IsMatch(content, pattern))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Replaces loophole characters in the string with proper characters. 
        /// </summary>
        /// <param name="content">content to replace loophole characters</param>
        /// <returns></returns>
        private static string _replaceLoopHoles(string content)
        {
            Dictionary<string, string> loopholes = new Dictionary<string, string>();
            loopholes.Add("3", "e");
            loopholes.Add("1", "i");
            loopholes.Add("|", "i;");
            loopholes.Add("4", "a");
            loopholes.Add("0", "o");
            loopholes.Add("@", "a");
            loopholes.Add("ö", "o");
            loopholes.Add(" ", "");
            loopholes.Add("5", "g");

            string value;
            foreach(string x in loopholes.Keys)
            {
                loopholes.TryGetValue(x, out value);
                content = content.Replace(x, value);
            }

            return content;
        }

        /// <summary>
        /// Adds a plus sign between all characters in a string
        /// </summary>
        /// <param name="blacklist"></param>
        /// <returns></returns>
        private static string addPlus(string blacklist)
        {
            var plus_string = "";
            for (int i = 0; i < blacklist.Length; i++)
            {
                plus_string = plus_string + blacklist[i] + '+';
            }
            return plus_string;
        }
    }
}
