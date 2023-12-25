using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using DiscordBot.Classes;

namespace DiscordBot.Engines
{
    public class BlackListEngine
    {
        /// <summary>
        /// List of blacklisted terms
        /// </summary>
        private static List<string> _blackListedTerms = new List<string>()
        {
            "test"
        };

        /// <summary>
        /// Blacklist file name
        /// </summary>
        private const string _blackListFile = "BlackList.JSON";

        /// <summary>
        /// Constructor and initializes the blacklist.
        /// </summary>
        public BlackListEngine()
        {
            RefreshList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="term"></param>
        public void AddBlackListTerm(string term)
        {
            _blackListedTerms.Add(term);
            _saveList();
            RefreshList();
        }

        /// <summary>
        /// Removes a blacklisted term
        /// </summary>
        /// <param name="term">term to remove from the blacklist</param>
        public bool RemoveBlackListTerm(string term)
        {
            bool result = _blackListedTerms.Remove(term);
            _saveList();
            RefreshList();
            return result;
        }

        /// <summary>
        /// Saves the blacklist to a JSON file
        /// </summary>
        private void _saveList()
        {
            JSONEngine engine = new JSONEngine();
            engine.OverwriteObjectFile<List<string>>(_blackListedTerms,_blackListFile);
        }

        /// <summary>
        /// Reloads the blacklist from the JSON file
        /// </summary>
        public void RefreshList()
        {
            JSONEngine engine = new JSONEngine();
            if (!File.Exists(_blackListFile))
            {
                engine.OverwriteObjectFile(new List<string>(), _blackListFile);
            }
            _blackListedTerms = engine.GenerateObject<List<string>>(_blackListFile);
        }

        /// <summary>
        /// Checks if a string has Blacklisted content
        /// </summary>
        /// <param name="content">string to check if contains blacklisted content</param>
        /// <returns>true if the string contains blacklisted content</returns>
        public static bool IsBlackListed(string content)
        {
            content = content.ToLower();
            content = StringUtils.ReplaceLoopHoles(content);

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
