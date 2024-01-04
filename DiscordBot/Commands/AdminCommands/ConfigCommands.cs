using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;
using DiscordBot.Config;
using DiscordBot.Assets;
using DiscordBot.Engines;

namespace DiscordBot.Commands.AdminCommands
{
    public class AdminCommands : BaseCommandModule
    {

        /// <summary>
        /// Selects a random item from a list of input items
        /// </summary>
        /// <param name="ctx">command context</param>
        /// <returns></returns>
        [Command]
        [RequireUserPermissions(DSharpPlus.Permissions.Administrator)]
        public async Task SetBotChannel(CommandContext ctx)
        {
            ulong channelID = ctx.Channel.Id;
            ServerConfig config = ServerConfig.GetServerConfig(ctx.Guild.Id);
            config.BotChannelID = channelID;
            await _saveConfig(ctx, config);
        }

        /// <summary>
        /// Selects a random item from a list of input items
        /// </summary>
        /// <param name="ctx">command context</param>
        /// <returns></returns>
        [Command]
        [RequireUserPermissions(DSharpPlus.Permissions.Administrator)]
        public async Task SetWatchRatingsChannel(CommandContext ctx)
        {
            ulong channelID = ctx.Channel.Id;
            ServerConfig config = ServerConfig.GetServerConfig(ctx.Guild.Id);
            config.WatchRatingsChannelID = channelID;
            await _saveConfig(ctx, config);
        }

        /// <summary>
        /// Selects a random item from a list of input items
        /// </summary>
        /// <param name="ctx">command context</param>
        /// <returns></returns>
        [Command]
        [RequireUserPermissions(DSharpPlus.Permissions.Administrator)]
        public async Task SetPlanToWatchChannel(CommandContext ctx)
        {
            ulong channelID = ctx.Channel.Id;
            ServerConfig config = ServerConfig.GetServerConfig(ctx.Guild.Id);
            config.PlanToWatchChannelID = channelID;
            await _saveConfig(ctx, config);
        }

        /// <summary>
        /// Selects a random item from a list of input items
        /// </summary>
        /// <param name="ctx">command context</param>
        /// <returns></returns>
        [Command]
        [RequireUserPermissions(DSharpPlus.Permissions.Administrator)]
        public async Task SetReminderChannel(CommandContext ctx)
        {
            ulong channelID = ctx.Channel.Id;
            ServerConfig config = ServerConfig.GetServerConfig(ctx.Guild.Id);
            config.ReminderChannelID = channelID;
            await _saveConfig(ctx, config);
        }


        /// <summary>
        /// Selects a random item from a list of input items
        /// </summary>
        /// <param name="ctx">command context</param>
        /// <returns></returns>
        [Command]
        [RequireUserPermissions(DSharpPlus.Permissions.Administrator)]
        public async Task ViewTaskQueue(CommandContext ctx)
        {
            await ctx.RespondAsync(TaskEngine.CurrentEngine.GetTaskList(ctx.Channel.GuildId));
        }

        /// <summary>
        /// Saves a server configuration object
        /// </summary>
        /// <param name="ctx">command context</param>
        /// <param name="config">config object to save</param>
        /// <returns></returns>
        private async Task _saveConfig(CommandContext ctx, ServerConfig config)
        {
            if (config.SaveConfig())
            {
                await ctx.Channel.SendMessageAsync("Channel configuration set.");
            }
            else
            {
                await ctx.Channel.SendMessageAsync(DiscordMessageAssets.GenerateErrorMessage());
            }
        }
    }
}
