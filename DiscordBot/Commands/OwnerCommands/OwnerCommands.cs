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
    class OwnerCommands : BaseCommandModule
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
        [Command]
        [RequireOwner]
        public async Task ViewTaskQueue(CommandContext ctx)
        {
            DiscordMessageBuilder response = new DiscordMessageBuilder();
            string result = TaskEngine.CurrentEngine.GetTaskList(ctx.Channel.GuildId);

            if (result.Length > 2000)
            {
                string filePath = $"{ServerConfig.ServerDirectory(ctx.Guild.Id)}{DateTime.Now.Ticks}.txt";
                File.WriteAllText(filePath, result);
                FileStream file = new FileStream(filePath, FileMode.Open);
                response.AddFile(file);
                await ctx.RespondAsync(response);
                file.Close();
                File.Delete(filePath);
                return;
            }

            response.WithContent(result);
            await ctx.RespondAsync(response);
        }
    }
}
