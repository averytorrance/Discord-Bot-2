﻿using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;
using DiscordBot.Engines;


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
    }
}