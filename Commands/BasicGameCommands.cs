using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;
using DiscordBot.Engines;
using System;

namespace DiscordBot.Commands
{
    class BasicGameCommands : BaseCommandModule
    {
        /// <summary>
        /// Displays the profile for the user. Creates a profile for a user if it does not exist. =
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [Command("roulette")]
        public async Task Roulette(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync(RouletteEngine.Play(ctx.Guild.Id));
        }

        /// <summary>
        /// Flips a coin
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [Command("coin")]
        public async Task Coin(CommandContext ctx)
        {
            string coinSide = "Heads!";
            int x = new Random().Next(0, 2); //chooses a random int between 0 and 1
            if(x < 1)
            {
                coinSide = "Tails";
            }

            await ctx.Channel.SendMessageAsync(coinSide);
        }

    }
}
