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
using DiscordBot.Engines.Tasks;

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
            BotConfig configJsonFile = BotConfig.GetConfig();
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
            _InitalizeEngines();

            //5. Set up the Task Handler Ready event
            Client.Ready += OnClientReady;
            Client.MessageCreated += MessageSentHandler;
            Client.GuildMemberAdded += UserJoinedHandler;
            Client.GuildAvailable += GuildAvailableHandler;
            Client.MessageReactionAdded += MessageReactionAddedHandler;
            Client.MessageReactionRemoved += MessageReactionRemoveHandler;
            Client.MessageDeleted += MessageDeleteHandler;
            Client.MessageUpdated += MessageUpdatedHandler;

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
            Commands.RegisterCommands<WatchCommands>();

            slashCommands.RegisterCommands<BasicSlashCommands>();
            slashCommands.RegisterCommands<ReminderCommands>(427296058310393856);
            slashCommands.RegisterCommands<WatchSlashCommands>(427296058310393856);
            slashCommands.RegisterCommands<ProfileCommands>();

            //7.1 Command Error Handler
            Commands.CommandErrored += CommandErrorHandler;
            slashCommands.SlashCommandErrored += SlashCommandErrorHandler;

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
        private static void _InitalizeEngines()
        {
            new TaskEngine();
            TaskEngine.CurrentEngine.Start();

            new ReminderEngine();
            new WatchRatingsEngine();
            new WatchPlanEngine();
        }

        /// <summary>
        /// Handler for Populating objects once a discord server becomes availabe
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private static async Task GuildAvailableHandler(DiscordClient sender, GuildCreateEventArgs e)
        {
            ///Load Reminder Engine States
            ReminderEngine.CurrentEngine.Load(e.Guild.Id, true);
            ReminderEngine.CurrentEngine.SendStaleReminders(e.Guild.Id);
            WatchRatingsEngine.CurrentEngine.StartUp(e.Guild.Id);
            //WatchPlanEngine.CurrentEngine.Load(e.Guild.Id);

            TaskEngine.CurrentEngine.AddTask(new ChristmasReminderTask(e.Guild.Id));
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
            
            DiscordUserEngine userEngine = new DiscordUserEngine();
            if (!userEngine.UserExists(e.Author.Id))
            {
                userEngine.CreateUser(e.Author);
            }

            DiscordServerEngine serverEngine = new DiscordServerEngine(e.Guild);
            if (!serverEngine.HasServerUser(e.Author.Id))
            {
                serverEngine.CreateUser(e.Author.Id);
            }
            ServerUser user = serverEngine.GetServerUser(e.Author.Id);

            if (BlackListEngine.IsBlackListed(e.Message.Content))
            {
                DiscordEmoji dollar = DiscordEmoji.FromUnicode("💸");
                await e.Message.CreateReactionAsync(dollar);

                serverEngine.AddDebt(user.ID);
            }
            /*if (WatchPlanEngine.IsWatchPlanChannelMessage(e.Message))
            {
                WatchPlanEngine.CurrentEngine.AddWatchEntry(e.Message);
            }*/

        }

        /// <summary>
        /// Handler for when a message gets deleted
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private static async Task MessageDeleteHandler(DiscordClient sender, MessageDeleteEventArgs e)
        {
            if (WatchRatingsEngine.IsWatchRatingsChannelMessage(e.Message))
            {
                WatchRatingsEngine.CurrentEngine.UpdateWatchEntry(e.Message);
            }
            /*if (WatchPlanEngine.IsWatchPlanChannelMessage(e.Message))
            {
                WatchPlanEngine.CurrentEngine.DeleteWatchEntry(e.Message);
            }*/
        }

        /// <summary>
        /// Handler for when a reaction is added to a message
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public static async Task MessageReactionAddedHandler(DiscordClient sender, MessageReactionAddEventArgs e)
        {
            if(WatchRatingsEngine.IsWatchRatingsChannelMessage(e.Message))
            {
                WatchRatingsEngine.CurrentEngine.UpdateWatchEntry(e.Message);
            }
            
        }

        /// <summary>
        /// Handler for when a reaction is removed from a message
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public static async Task MessageReactionRemoveHandler(DiscordClient sender, MessageReactionRemoveEventArgs e)
        {
            if (WatchRatingsEngine.IsWatchRatingsChannelMessage(e.Message))
            {
                WatchRatingsEngine.CurrentEngine.UpdateWatchEntry(e.Message);
            }
        }

        /// <summary>
        /// Handler for when a message is updated
        /// </summary>
        /// <returns></returns>
        public static async Task MessageUpdatedHandler(DiscordClient sender, MessageUpdateEventArgs e)
        {
            if (WatchRatingsEngine.IsWatchRatingsChannelMessage(e.Message))
            {
                WatchRatingsEngine.CurrentEngine.UpdateWatchEntry(e.Message);
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
            DiscordUserEngine userEngine = new DiscordUserEngine();
            if (!userEngine.UserExists(e.Member.Id))
            {
                userEngine.CreateUser(e.Member);
            }
            else
            {
                userEngine.UpdateUserAvatar(e.Member);
            }

            DiscordServerEngine serverEngine = new DiscordServerEngine(e.Guild);
            ServerUser user = serverEngine.GetServerUser(e.Member.Id);
            if (user == null)
            {
                serverEngine.CreateUser(e.Member.Id);
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
            if(error.Exception is SlashExecutionChecksFailedException)
            {
                DiscordInteractionResponseBuilder message = new DiscordInteractionResponseBuilder(
                    DiscordMessageAssets.GenerateErrorMessage("You do not have the proper permissions to run this command."));
                await error.Context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, message);
                return;
            }

            await error.Context.Channel.SendMessageAsync(DiscordMessageAssets.GenerateErrorMessage(error.Exception));

            Console.WriteLine(error.Exception);
        }

    }
}
