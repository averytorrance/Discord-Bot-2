using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Linq.Expressions;
using System.Reflection;
using DiscordBot.Config;
using DSharpPlus.CommandsNext;
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
        /// Filename to save the configuration to
        /// </summary>
        [JsonIgnore]
        public string FileName { get { return "config.json"; } }
        
        /// <summary>
        /// Generates the Config object from the config file
        /// </summary>
        /// <returns></returns>
        public async Task GenerateConfig() 
        {
            using (StreamReader sr = new StreamReader(FileName, new UTF8Encoding(false)))
            {
                string json = await sr.ReadToEndAsync(); //Reading whole file
                BotConfig obj = JsonConvert.DeserializeObject<BotConfig>(json); //Deserialising file

                this.Token = obj.Token; //Setting our token & prefix that we extracted from our file
                this.Prefix = obj.Prefix;
            }
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
        /// Gets the Server config for a specific discord server, or generates one if it does not exist
        /// </summary>
        /// <param name="serverID">Discord Server ID</param>
        /// <returns></returns>
        public static ServerConfig GetServerConfig(ulong serverID)
        {
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
        /// Gets the filename for a config for a specifc discord server
        /// </summary>
        /// <param name="serverID">server ID</param>
        /// <returns></returns>
        public static string FileName(ulong serverID)
        {
            return $"{serverID}{_fileName}";
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
