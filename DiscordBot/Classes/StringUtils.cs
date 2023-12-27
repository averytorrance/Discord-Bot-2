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

            #region Numbers
            loopholes.Add("0", "O");
            loopholes.Add("1", "I");
            loopholes.Add("3", "E");
            loopholes.Add("4", "A");
            loopholes.Add("5", "S");
            loopholes.Add("8", "B");
            #endregion


            loopholes.Add("|", "i;");
            loopholes.Add("@", "a");
            loopholes.Add("ø", "o");
            loopholes.Add("Ø", "O");
            loopholes.Add("¡", "i");
            loopholes.Add("$", "S");

            #region Accent characters
            loopholes.Add("à", "a");
            loopholes.Add("á", "a");
            loopholes.Add("â", "a");
            loopholes.Add("ã", "a");
            loopholes.Add("ä", "a");
            loopholes.Add("å", "a");
            loopholes.Add("À", "A");
            loopholes.Add("Á", "A");
            loopholes.Add("Â", "A");
            loopholes.Add("Ã", "A");
            loopholes.Add("Ä", "A");
            loopholes.Add("Å", "A");

            loopholes.Add("è", "e");
            loopholes.Add("é", "e");
            loopholes.Add("ê", "e");
            loopholes.Add("ë", "e");
            loopholes.Add("È", "E");
            loopholes.Add("É", "E");
            loopholes.Add("Ê", "E");
            loopholes.Add("Ë", "E");


            loopholes.Add("ì", "i");
            loopholes.Add("í", "i");
            loopholes.Add("î", "i");
            loopholes.Add("ï", "i");
            loopholes.Add("Ì", "I");
            loopholes.Add("Í", "I");
            loopholes.Add("Î", "I");
            loopholes.Add("Ï", "I");

            loopholes.Add("ñ", "n");
            loopholes.Add("Ñ", "N");

            loopholes.Add("ò", "o");
            loopholes.Add("ó", "o");
            loopholes.Add("ô", "o");
            loopholes.Add("õ", "o");
            loopholes.Add("ö", "o");
            loopholes.Add("Ò", "O");
            loopholes.Add("Ó", "O");
            loopholes.Add("Ô", "O");
            loopholes.Add("Õ", "O");
            loopholes.Add("Ö", "O");


            loopholes.Add("ù", "u");
            loopholes.Add("ú", "u");
            loopholes.Add("û", "u");
            loopholes.Add("ü", "u");
            loopholes.Add("Ù", "U");
            loopholes.Add("Ú", "U");
            loopholes.Add("Û", "U");
            loopholes.Add("Ü", "U");

            loopholes.Add("ý", "y");
            loopholes.Add("ÿ", "y");
            loopholes.Add("Ý", "Y");
            loopholes.Add("Ÿ", "Y");
            #endregion

            string value;
            foreach (string x in loopholes.Keys)
            {
                loopholes.TryGetValue(x, out value);
                content = content.Replace(x, value);
            }

            return content;
        }

        /// <summary>
        /// Replaces whitespace with null string
        /// </summary>
        /// <param name="content">content to replace loophole characters</param>
        /// <returns></returns>
        public static string StripWhitespace(string content)
        {
            return content.Replace(" ", "");
        }

        /// <summary>
        /// Replaces special characters with null strings
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string StripSpecialCharacters(string content)
        {
            content = content.Replace(":", "");
            content = content.Replace("[", "");
            content = content.Replace("]", "");

            return content;
        }

    }
}
