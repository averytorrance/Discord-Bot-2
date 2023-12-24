using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;
using DiscordBot.Engines;

namespace DiscordBot.Commands
{
    public class BasicCommands : BaseCommandModule
    {
        /// <summary>
        /// Selects a random item from a list of input items
        /// </summary>
        /// <param name="ctx">command context</param>
        /// <param name="names">input parameters. handling added to use ',' as the delimeter for each item.</param>
        /// <returns></returns>
        [Command("random")]
        public async Task SelectRandom(CommandContext ctx, params string[] names)
        {
            string joinedValues = string.Join(" ", names);
            List<string> selections = joinedValues.Split(',').ToList();
            int index = (new Random()).Next(selections.Count);
            await ctx.Channel.SendMessageAsync(selections[index]);
        }

        /// <summary>
        /// Selects a random item from a list of input items
        /// </summary>
        /// <param name="ctx">command context</param>
        /// <param name="names">input parameters. handling added to use ',' as the delimeter for each item.</param>
        /// <returns></returns>
        [Command("test")]
        public async Task test(CommandContext ctx)
        {
            string channelId = "UCVm_MQHtSoafvlKbd8fqNCA"; // lopgger joshua
            YoutubeAPIEngine engine = new YoutubeAPIEngine();

            await ctx.Channel.SendMessageAsync(engine.GetMostRecentVideo(channelId));
        }
    }
}
