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

namespace DiscordBot
{
    public sealed class Program
    {
        public static DiscordClient Client { get; private set; }
        public static CommandsNextExtension Commands { get; private set; }
        static async Task Main(string[] args)
        {
            //1. Get the details of your config.json file by deserialising it
            BotConfig configJsonFile = new BotConfig();
            await configJsonFile.GenerateConfig();

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
            Commands.RegisterCommands<ProfileCommands>();
            Commands.RegisterCommands<AdminCommands>();
            Commands.RegisterCommands<MiscAdminCommands>();

            slashCommands.RegisterCommands<PollCommands>();

            //7.1 Command Error Handler
            Commands.CommandErrored += CommandErrorHandler;


            //8. Connect to get the Bot online
            await Client.ConnectAsync();
            await Task.Delay(-1);
        }

        private static Task OnClientReady(DiscordClient sender, ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Handler for when a message has been sent to the chat
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private static async Task MessageSentHandler(DiscordClient sender, MessageCreateEventArgs e)
        {
            DiscordMember test;
            e.Guild.Members.TryGetValue(e.Author.Id, out test);
            if (BlackListEngine.IsBlackListed(e.Message.Content))
            {
                DiscordEmoji dollar = DiscordEmoji.FromUnicode("💸");
                await e.Message.CreateReactionAsync(dollar);
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
        /// <param name=""></param>
        /// <returns></returns>
        private static async Task CommandErrorHandler(CommandsNextExtension sender, CommandErrorEventArgs e)
        {
            await e.Context.Channel.SendMessageAsync(DiscordMessageAssets.GenerateErrorMessage(e.Exception));
        }


    }
}
