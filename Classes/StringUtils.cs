using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Classes
{
    public static class StringUtils
    {
        /// <summary>
        /// Replaces loophole characters in the string with proper characters. 
        /// </summary>
        /// <param name="content">content to replace loophole characters</param>
        /// <returns></returns>
        public static string ReplaceLoopHoles(string content)
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
            foreach (string x in loopholes.Keys)
            {
                loopholes.TryGetValue(x, out value);
                content = content.Replace(x, value);
            }

            return content;
        }

    }
}
