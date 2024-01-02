using Newtonsoft.Json;
using System.IO;
using System;
using DiscordBot.Engines;


namespace DiscordBot.Config
{
    /// <summary>
    /// Bot Configuration Class
    /// </summary>
    internal class BotConfig
    {
        /// <summary>
        /// Discord Bot Token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Prefix for commands
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// API Key for Youtube APIs
        /// </summary>
        public string YoutubeAPIKey { get; set; }

        /// <summary>
        /// Filename to save the configuration to
        /// </summary>
        [JsonIgnore]
        private static string FileName = "config.json";
        
        /// <summary>
        /// Generates the Config object from the config file
        /// </summary>
        /// <returns></returns>
        public static BotConfig GetConfig()
        {
            JSONEngine engine = new JSONEngine();
            return engine.GenerateObject<BotConfig>(FileName);
        }

    }

    /// <summary>
    /// Server Configuration Class
    /// </summary>
    internal class ServerConfig
    {
        /// <summary>
        /// Server ID for the server configuration
        /// </summary>
        public ulong ServerID { get; set; }

        /// <summary>
        /// ID for the Bot Channel
        /// </summary>
        public ulong BotChannelID { get; set; }

        /// <summary>
        /// ID for channel to send reminders to
        /// </summary>
        public ulong ReminderChannelID { get; set; }

        /// <summary>
        /// ID for the Watch Ratings channel 
        /// </summary>
        public ulong WatchRatingsChannelID { get; set; }

        /// <summary>
        /// ID for the Plan To Watch Channel
        /// </summary>
        public ulong PlanToWatchChannelID { get; set; }

        /// <summary>
        /// Filename to store server configurations
        /// </summary>
        [JsonIgnore]
        private static readonly string _fileName = "ServerConfig.json";

        /// <summary>
        /// File Directory to store server files
        /// </summary>
        [JsonIgnore]
        private static readonly string _directory = "ServerFiles\\";

        /// <summary>
        /// Gets the Server config for a specific discord server, or generates one if it does not exist
        /// </summary>
        /// <param name="serverID">Discord Server ID</param>
        /// <returns></returns>
        public static ServerConfig GetServerConfig(ulong serverID)
        {
            if (!Directory.Exists(ServerDirectory(serverID)))
            {
                Directory.CreateDirectory(ServerDirectory(serverID));
            }

            if (!File.Exists(FileName(serverID)))
            {
                ServerConfig config = new ServerConfig()
                {
                    ServerID = serverID
                };
                config.SaveConfig();
            }

            JSONEngine engine = new JSONEngine();
            return engine.GenerateObject<ServerConfig>(FileName(serverID));
        }

        /// <summary>
        /// Saves this configuration to the server configuration file
        /// </summary>
        /// <returns></returns>
        public bool SaveConfig()
        {
            JSONEngine jsonEngine = new JSONEngine();
            return jsonEngine.OverwriteObjectFile<ServerConfig>(this, FileName());
        }

        /// <summary>
        /// Directory for Server Backup files   
        /// </summary>
        /// <param name="serverID">server ID</param>
        /// <returns></returns>
        private static string ServerBackupDirectory(ulong serverID)
        {
            return $"{ServerDirectory(serverID)}Backups\\";
        }

        /// <summary>
        /// The backup directory for today
        /// </summary>
        /// <param name="serverID">server ID</param>
        /// <returns></returns>
        public static string BackupDirectoryToday(ulong serverID)
        {
            return $"{ServerBackupDirectory(serverID)}\\{DateTime.Now.ToString("yyyy MM dd")}\\";
        }

        /// <summary>
        /// Directory for Server files
        /// </summary>
        /// <returns></returns>
        public static string ServerDirectory(ulong serverID)
        {
            return $"{_directory}{serverID}\\";
        }

        /// <summary>
        /// Directory for Server files
        /// </summary>
        /// <returns></returns>
        public string ServerDirectory()
        {
            return ServerDirectory(ServerID);
        }

        /// <summary>
        /// Gets the filename for a config for a specifc discord server
        /// </summary>
        /// <param name="serverID">server ID</param>
        /// <returns></returns>
        public static string FileName(ulong serverID)
        {
            return $"{ServerDirectory(serverID)}{_fileName}";
        }

        /// <summary>
        /// Gets the filename for a specific server config
        /// </summary>
        /// <returns></returns>
        public string FileName()
        {
            return FileName(ServerID);
        }

    }

}
