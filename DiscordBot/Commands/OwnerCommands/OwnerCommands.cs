using DiscordBot.Config;
using DiscordBot.Engines;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.Commands
{
    public class OwnerCommands : BaseCommandModule
    {
        /// <summary>
        /// Selects a random item from a list of input items
        /// </summary>
        /// <param name="ctx">command context</param>
        /// <returns></returns>
        [Command("addblacklist")]
        [RequireOwner]
        public async Task addblacklist(CommandContext ctx, params string[] names)
        {
            string term = string.Join(" ", names);
            Program.BlackList.AddBlackListTerm(term);
        }

        /// <summary>
        /// Selects a random item from a list of input items
        /// </summary>
        /// <param name="ctx">command context</param>
        /// <returns></returns>
        [Command("removeblacklist")]
        [RequireOwner]
        public async Task removeblacklist(CommandContext ctx, params string[] names)
        {
            string term = string.Join(" ", names);
            Program.BlackList.RemoveBlackListTerm(term);
        }

        /// <summary>
        /// Selects a random item from a list of input items
        /// </summary>
        /// <param name="ctx">command context</param>
        /// <returns></returns>
        [Command("viewtaskqueue")]
        [RequireOwner]
        public async Task ViewTaskQueue(CommandContext ctx)
        {
            string result = TaskEngine.CurrentEngine.GetTaskList(ctx.Channel.GuildId);
            DiscordServerEngine engine = new DiscordServerEngine(ctx.Guild);

            if(result == "")
            {
                result = "Queue is empty.";
            }

            engine.SendResponse(ctx, result, true);
        }

        /// <summary>
        /// Executes a task on the task list
        /// </summary>
        /// <param name="ctx">command context</param>
        /// <returns></returns>
        [Command("executetask")]
        [RequireOwner]
        public async Task ExecuteTask(CommandContext ctx, params string[] names)
        {
            ulong id = (ulong)(Int64.Parse(string.Join(" ", names)));
            TaskEngine.CurrentEngine.ExecuteTask(id);
        }

        /// <summary>
        /// Executes a task on the task list
        /// </summary>
        /// <param name="ctx">command context</param>
        /// <returns></returns>
        [Command("ispollerrunning")]
        [RequireOwner]
        public async Task IsPollerRunning(CommandContext ctx)
        {
            await ctx.RespondAsync(TaskEngine.CurrentEngine.IsRunning().ToString());
        }

        /// <summary>
        /// Executes a task on the task list
        /// </summary>
        /// <param name="ctx">command context</param>
        /// <returns></returns>
        [Command("getytsubscriptionlist")]
        public async Task GetYTSubscriptionList(CommandContext ctx)
        {
            YoutubeAPIEngine engine = new YoutubeAPIEngine();
            List<string> channels = engine.GetSubscribedChannelList(ctx.Guild.Id);
            channels = channels.Select(x => $"https://www.youtube.com/channel/{x}").ToList();
            DiscordServerEngine server = new DiscordServerEngine(ctx.Guild);

            string response = "No channels found.";
            if (channels.Count > 0)
            {
                response = string.Join("\n", channels);
            }
            server.SendResponse(ctx, response);
        }

        /// <summary>
        /// Executes a task on the task list
        /// </summary>
        /// <param name="ctx">command context</param>
        /// <returns></returns>
        [RequireOwner]
        [Command("ytsubscribe")]
        public async Task YTSubscribe(CommandContext ctx, params string[] names)
        {
            YoutubeAPIEngine engine = new YoutubeAPIEngine();
            foreach(string channel in names)
            {
                engine.SubscribeToChannel(ctx.Guild.Id, channel);
            }
            
            DiscordServerEngine server = new DiscordServerEngine(ctx.Guild);
            server.SendResponse(ctx, "Success");
        }

        /// <summary>
        /// Executes a task on the task list
        /// </summary>
        /// <param name="ctx">command context</param>
        /// <returns></returns>
        [RequireOwner]
        [Command("ytunsubscribe")]
        public async Task YTUnsubscribe(CommandContext ctx, params string[] names)
        {
            YoutubeAPIEngine engine = new YoutubeAPIEngine();
            foreach (string channel in names)
            {
                engine.UnSubscribeFromChannel(ctx.Guild.Id, channel);
            }

            DiscordServerEngine server = new DiscordServerEngine(ctx.Guild);
            server.SendResponse(ctx, "Success");
        }


    }
}
