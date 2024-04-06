using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DiscordBot.Config;
using DiscordBot.UserProfile;
using Newtonsoft.Json;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.CommandsNext;
using System.IO;
using DiscordBot.Classes;

namespace DiscordBot.Engines
{
    public class DiscordServerEngine : ServerEngine
    {
        /// <summary>
        /// Discord Server Object
        /// </summary>
        public DiscordGuild Server { get; private set; }

        /// <summary>
        /// Discord Server ID
        /// </summary>
        public ulong ID { get; private set; }

        /// <summary>
        /// Bot Channel ID
        /// </summary>
        public ulong BotChannelID { get; private set; }

        /// <summary>
        /// YT Plan to Watch Channel ID
        /// </summary>
        public ulong YTPlanToWatchChannelID { get; private set; }

        public override Type EngineStateType { get; } = typeof(DiscordServerEngineState);

        public DiscordServerEngine(DiscordGuild server)
        {
            Server = server;
            ID = server.Id;
            Load(ID);

            ServerConfig config = ServerConfig.GetServerConfig(ID);
            BotChannelID = config.BotChannelID;
            YTPlanToWatchChannelID = config.YTPlanToWatchChannelID;
        }

        /// <summary>
        /// Gets a discord server engine using a discord server ID
        /// </summary>
        /// <param name="serverID"></param>
        /// <returns></returns>
        public static async Task<DiscordServerEngine> GetDiscordServerEngine(ulong serverID)
        {
            DiscordGuild server = await Program.Client.GetGuildAsync(serverID);
            return new DiscordServerEngine(server);
        }

        /// <summary>
        /// Load server state
        /// </summary>
        /// <param name="serverID"></param>
        public override void Load(ulong serverID)
        {
            DiscordServerEngineState state;
            if (TryGetValue(serverID, out state))
            {
                serverStates.Remove(serverID);
            }
            state = ServerEngineState.Load<DiscordServerEngineState>(new DiscordServerEngineState(serverID));
            serverStates.Add(serverID, state);
        }

        /// <summary>
        /// Gets the ServerEngineState for this Engine
        /// </summary>
        /// <returns></returns>
        public DiscordServerEngineState GetState()
        {
            DiscordServerEngineState state;
            TryGetValue(ID, out state);
            return state;
        }

        /// <summary>
        /// Determines if the state object has a user with a specific id
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool HasServerUser(ulong userID)
        {
            DiscordServerEngineState state = GetState();
            return state.Users.Where(x => x.ID == userID).Any();
        }

        /// <summary>
        /// Gets a discord server user
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public ServerUser GetServerUser(ulong userID)
        {
            DiscordServerEngineState state = GetState();
            return state.Users.Where(x => x.ID == userID).FirstOrDefault();
        }

        /// <summary>
        /// Gets a discord user given a server user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public DUser GetUser(ServerUser user)
        {
            return GetUser(user.ID);
        }

        /// <summary>
        /// Gets a discord server user given the ID
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public DUser GetUser(ulong userID)
        {
            DiscordUserEngine engine = new DiscordUserEngine();

            return engine.GetUser(userID);
        }

        /// <summary>
        /// Creates a user with a specific ID
        /// </summary>
        /// <param name="userID"></param>
        /// <returns>true if the User Engine has the user, false if it doesnt</returns>
        public bool CreateUser(ulong userID)
        {
            DiscordServerEngineState state = GetState();
            if (GetServerUser(userID) == null)
            {
                state.Users.Add(ServerUser.CreateServerUser(userID, ID));
                state.SaveState();
            }
            DiscordUserEngine userEngine = new DiscordUserEngine();
            return userEngine.UserExists(userID);
        }

        /// <summary>
        /// Adds XP to a user
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="XP"></param>
        /// <returns>the current xp of the user</returns>
        public double AddXP(ulong userID, double XP)
        {
            DiscordServerEngineState state = GetState();
            GetServerUser(userID).AddXP(XP);
            state.SaveState();
            return GetServerUser(userID).XP;
        }

        /// <summary>
        /// Adds debt to the user
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="amount"></param>
        /// <returns>the current debt amount</returns>
        public double AddDebt(ulong userID, double amount = 1.0)
        {
            DiscordServerEngineState state = GetState();
            GetServerUser(userID).AddToDebt(amount);
            state.SaveState();
            return GetServerUser(userID).Debt;
        }

        /// <summary>
        /// Backups 
        /// </summary>
        /// <returns></returns>
        public async Task BackupEmojis()
        {
            WebEngine.Start();
            foreach (DiscordEmoji emoji in Server.Emojis.Values)
            {
                if (emoji.IsAnimated)
                {

                }
                await WebEngine.DownloadImage(emoji.Url, BackupDirectory("Emojis"), EmojiFileName(emoji));
            }
        }

        /// <summary>
        /// Gets the filename for a specific emoji
        /// </summary>
        /// <param name="emoji">Discord Emoji object</param>
        /// <returns></returns>
        public string EmojiFileName(DiscordEmoji emoji)
        {
            string filename = $"{emoji.Name}";
            string extension = "png";

            if (emoji.IsAnimated)
            {
                extension = "gif";
            }

            return $"{filename}.{extension}";
        }

        /// <summary>
        /// Backup directory
        /// </summary>
        /// <param name="backupType">subdirectory folder name in the backup folder</param>
        /// <returns></returns>
        public string BackupDirectory(string backupType)
        {
            return $"{ServerConfig.BackupDirectoryToday(ID)}{backupType}";
        }

        /// <summary>
        /// Checks if a user has Administrator Permissions
        /// </summary>
        /// <param name="user">Discord Member object. Pulls from a discord server.</param>
        /// <returns></returns>
        public static bool IsAdmin(DiscordMember user)
        {
            return user.Permissions.HasFlag(Permissions.Administrator);
        }

        /// <summary>
        /// Sends a message to the bot channel
        /// </summary>
        /// <param name="message"></param>
        public async void SendBotMessage(DiscordMessageBuilder message)
        {
            DiscordChannel channel = await GetBotChannel();
            await SentChannelMessage(channel, message);
        }

        /// <summary>
        /// Sends a message to the bot channel
        /// </summary>
        /// <param name="message"></param>
        public async Task<bool> SendBotMessage(string message)
        {
            if(BotChannelID == 0) { return false; }

            DiscordChannel channel = await GetBotChannel();
            await SentChannelMessage(channel, message);
            return true;
        }

        /// <summary>
        /// Sends a message to the YT Plan to Watch Channel
        /// </summary>
        /// <param name="message"></param>
        public async Task<bool> SendYTMessage(string message)
        {
            if (YTPlanToWatchChannelID == 0) { return false; }

            DiscordChannel channel = await GetYTPlanToWatchChannel();
            await SentChannelMessage(channel, message);
            return true;
        }

        /// <summary>
        /// Gets the Bot Channel
        /// </summary>
        /// <returns></returns>
        private async Task<DiscordChannel> GetBotChannel()
        {
            return await Program.Client.GetChannelAsync(BotChannelID);
        }

        /// <summary>
        /// Gets the YT Plan to Watch Channel
        /// </summary>
        /// <returns></returns>
        private async Task<DiscordChannel> GetYTPlanToWatchChannel()
        {
            return await Program.Client.GetChannelAsync(YTPlanToWatchChannelID);
        }

        /// <summary>
        /// Sends an interaction message
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="response"></param>
        public async void SendInteractionResponse(InteractionContext ctx, string response)
        {
            DiscordInteractionResponseBuilder message = new DiscordInteractionResponseBuilder();

            if (response.Length > 2000)
            {
                string filePath = $"{ServerConfig.ServerDirectory(ctx.Guild.Id)}{DateTime.Now.Ticks}.txt";
                File.WriteAllText(filePath, response);
                FileStream file = new FileStream(filePath, FileMode.Open);
                message.AddFile(file);
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, message);
                file.Close();
                File.Delete(filePath);
                return;
            }


            message.WithContent(response);

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, message);
        }

        /// <summary>
        /// Sends a response to a message
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="response"></param>
        public async void SendResponse(CommandContext ctx, string response, bool forceFile = false)
        {
            try
            {
                DiscordMessageBuilder message = new DiscordMessageBuilder();

                if (forceFile || response.Length > 2000)
                {
                    string filePath = $"{ServerConfig.ServerDirectory(ctx.Guild.Id)}{DateTime.Now.Ticks}.txt";
                    File.WriteAllText(filePath, response);
                    FileStream file = new FileStream(filePath, FileMode.Open);
                    message.AddFile(file);
                    await ctx.RespondAsync(message);
                    file.Close();
                    File.Delete(filePath);
                    return;
                }

                message.WithContent(response);
                await ctx.RespondAsync(message);
            }
            catch(Exception ex)
            {
                Log.WriteToFile(Log.LogLevel.DiscordBot, ex);
                throw ex;
            }

        }

        /// <summary>
        /// sends a chennel message
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task<bool> SentChannelMessage(DiscordChannel channel, string message, bool forceFile = false)
        {
            var messageBuilder = new DiscordMessageBuilder();

            if (forceFile || message.Length > 2000)
            {
                string filePath = $"{ServerConfig.ServerDirectory(channel.Guild.Id)}{DateTime.Now.Ticks}.txt";
                File.WriteAllText(filePath, message);
                FileStream file = new FileStream(filePath, FileMode.Open);
                messageBuilder.AddFile(file);
                messageBuilder.Content = "";
                await channel.SendMessageAsync(messageBuilder);
                file.Close();
                File.Delete(filePath);
                return true;
            }

            messageBuilder.Content = message;

            return await SentChannelMessage(channel, messageBuilder);
        }

        /// <summary>
        /// sends a chennel message
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task<bool> SentChannelMessage(DiscordChannel channel, DiscordMessageBuilder message, bool forceFile = false)
        {
            try
            {
                await channel.SendMessageAsync(message);
                return true;
            }
            catch(Exception ex)
            {
                Log.WriteToFile(Log.LogLevel.DiscordBot, ex);
                throw ex;
            }
        }

    }

    public class DiscordServerEngineState : ServerEngineState
    {
        /// <summary>
        /// List of Users
        /// </summary>
        public List<ServerUser> Users { get; set; } = new List<ServerUser>();

        [JsonIgnore]
        public override string StateFile_ { get; } = "DiscordServerState.JSON";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serverID"></param>
        public DiscordServerEngineState(ulong serverID) : base(serverID)
        {
            ServerID = serverID;
        }
    }
}
