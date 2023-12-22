using DiscordBot.Commands;
using DiscordBot.Config;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using System;
using System.Threading.Tasks;
using DiscordBot.Engines;
using DSharpPlus.Entities;
using DiscordBot.UserProfile;
using DiscordBot.Commands.AdminCommands;
using DiscordBot.Commands.SlashCommands;
using DSharpPlus.SlashCommands;
using DiscordBot.Assets;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.SlashCommands.EventArgs;

namespace DiscordBot
{
    public sealed class Program
    {
        public static DiscordClient Client { get; private set; }
        public static CommandsNextExtension Commands { get; private set; }

        public static BlackListEngine BlackList { get; private set; }

        public static ReminderEngine Reminders { get; private set; }

        static async Task Main(string[] args)
        {
            //1. Get the details of your config.json file by deserialising it
            BotConfig configJsonFile = new BotConfig();
            await configJsonFile.GenerateConfig();
            BlackList = new BlackListEngine();


            //2. Setting up the Bot Configuration
            var discordConfig = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = configJsonFile.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true
            };

            //3. Apply this config to our DiscordClient
            Client = new DiscordClient(discordConfig);

            //4. Set the default timeout for Commands that use interactivity
            Client.UseInteractivity(new InteractivityConfiguration()
            {
                Timeout = TimeSpan.FromMinutes(2)
            });

            //5. Set up the Task Handler Ready event
            Client.Ready += OnClientReady;
            Client.MessageCreated += MessageSentHandler;
            Client.GuildMemberAdded += UserJoinedHandler;

            //6. Set up the Commands Configuration
            CommandsNextConfiguration commandsConfig = new CommandsNextConfiguration()
            {
                StringPrefixes = new string[] { configJsonFile.Prefix },
                EnableMentionPrefix = true,
                EnableDms = true,
                EnableDefaultHelp = false,
            };

            Commands = Client.UseCommandsNext(commandsConfig);
            SlashCommandsExtension slashCommands = Client.UseSlashCommands();

            //7. Register your commands
            Commands.RegisterCommands<BasicCommands>();
            Commands.RegisterCommands<BasicGameCommands>();
            Commands.RegisterCommands<AdminCommands>();
            Commands.RegisterCommands<MiscAdminCommands>();
            Commands.RegisterCommands<OwnerCommands>();

            slashCommands.RegisterCommands<PollCommands>();
            slashCommands.RegisterCommands<ReminderCommands>(427296058310393856);
            slashCommands.RegisterCommands<ReminderCommands>(436420385857077288);
            slashCommands.RegisterCommands<ProfileCommands>();

            //7.1 Command Error Handler
            Commands.CommandErrored += CommandErrorHandler;
            slashCommands.SlashCommandErrored += SlashCommandErrorHandler;

            _InitalizeReminders();

            //8. Connect to get the Bot online
            await Client.ConnectAsync();
            await Task.Delay(-1);
        }

        private static Task OnClientReady(DiscordClient sender, ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Initalizes the reminder engine
        /// </summary>
        private static void _InitalizeReminders()
        {
            foreach(ulong guildID in Program.Client.Guilds.Keys)
            {
                ReminderEngine.Load(guildID);
            }
            ReminderEngine.SendStaleReminders();


        }

        /// <summary>
        /// Handler for when a message has been sent to the chat
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private static async Task MessageSentHandler(DiscordClient sender, MessageCreateEventArgs e)
        {
            DiscordMember duser;
            e.Guild.Members.TryGetValue(e.Author.Id, out duser);
            if(duser == null)
            {
                return;
            }

            LocalUserEngine userEngine = new LocalUserEngine();
            
            LocalUser user = new LocalUser(duser);
            if (!userEngine.UserExists(user))
            {
                userEngine.CreateUser(user);
            }

            if (BlackListEngine.IsBlackListed(e.Message.Content))
            {
                DiscordEmoji dollar = DiscordEmoji.FromUnicode("💸");
                await e.Message.CreateReactionAsync(dollar);

                user = userEngine.GetUser(user.UserID, user.ServerID);
                user.AddToDebt();
                userEngine.UpdateUser(user);
            }

        }

        /// <summary>
        /// Handler for when a user joins the server.
        /// 1. Creates a user profile or updates the user avatar if their profile already exists.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private static async Task UserJoinedHandler(DiscordClient sender, GuildMemberAddEventArgs e)
        {
            LocalUser newUser = new LocalUser(e.Member);
            LocalUserEngine userEngine = new LocalUserEngine();
            
            if (!userEngine.UserExists(newUser))
            {
                userEngine.CreateUser(newUser);
            }
            else
            {
                userEngine.UpdateUserAvatar(newUser);
            }
        }

        /// <summary>
        /// Hanlder for when an error occurs during a command call
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        private static async Task CommandErrorHandler(CommandsNextExtension sender, CommandErrorEventArgs error)
        {
            if(error.Exception is CommandNotFoundException)
            {
                return;
            }

            await error.Context.Channel.SendMessageAsync(DiscordMessageAssets.GenerateErrorMessage(error.Exception));

            Console.WriteLine(error.Exception);

        }

        /// <summary>
        /// Handler for when errors occur during a slash command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        private static async Task SlashCommandErrorHandler(SlashCommandsExtension sender, SlashCommandErrorEventArgs error)
        {

            await error.Context.Channel.SendMessageAsync(DiscordMessageAssets.GenerateErrorMessage(error.Exception));

            Console.WriteLine(error.Exception);
        }

    }
}
