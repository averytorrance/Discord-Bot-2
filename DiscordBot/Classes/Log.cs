using DSharpPlus.SlashCommands;
using System;
using System.Diagnostics;
using System.IO;

namespace DiscordBot.Classes
{
    public static class Log
    {
        /// <summary>
        /// Writes a line to the log file
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        public static void WriteToFile(LogLevel level, string message)
        {
            string path = $"{AppDomain.CurrentDomain.BaseDirectory}\\Logs";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            message = $"{DateTime.Now}: {message}\n___________________________________________________________________________" +
                $"_________________________________________________________________________________________________________\n";

            string logFile = $"{path}\\{level.GetName()}_{DateTime.Now.Date.ToString("yyyy-MM-dd")}.txt";
            if (!File.Exists(logFile))
            {
                using (StreamWriter sw = new StreamWriter(logFile))
                {
                    sw.WriteLine(message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(logFile))
                {
                    sw.WriteLine(message);
                }
            }
        }

        /// <summary>
        /// Writes exception information to a log file.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        public static void WriteToFile(LogLevel level, Exception error, string additionalInformation = "")
        {
            string errorLog = ExceptionString(error);
            if (!string.IsNullOrWhiteSpace(additionalInformation))
            {
                errorLog = $"{additionalInformation}\n {errorLog}";
            }
            WriteToFile(level, errorLog);
        }

        /// <summary>
        /// constructs a string for an exception
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        private static string ExceptionString(Exception error)
        {
            string ret = $"Error: {error.Message}" +
                $"StackTrace: {error.StackTrace}";
            if (error.InnerException != null)
            {
                return $"{ret}\n\n{ExceptionString(error.InnerException)}";
            }
            return ret;
        }

        /// <summary>
        /// Log Level
        /// </summary>
        public enum LogLevel
        {
            DiscordBot,
            BotService
        }

    }
}
 
