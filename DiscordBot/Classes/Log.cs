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
        public static void WriteToFile(LogLevel level, Exception error)
        { 
            WriteToFile(level, $"______________\n" +
                $"Error: {error.Message} \n" +
                $"StackTrace: {error.StackTrace} \n_________");
            if(error.InnerException != null)
            {
                WriteToFile(level, error.InnerException);
            }
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
 
