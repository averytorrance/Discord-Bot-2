using DiscordBot.Config;
using DiscordBot.Engines;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.IO;
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
    }
}
