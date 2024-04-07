using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;
using DiscordBot.Engines;
using DiscordBot.UserProfile;

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
        [Command("time")]
        public async Task time(CommandContext ctx, params string[] names)
        {
            DiscordUserEngine discordUserEngine = new DiscordUserEngine();
            if (!discordUserEngine.UserExists(ctx.Message.Author.Id))
            {
                discordUserEngine.CreateUser(ctx.Message.Author);
            }

            DUser user = discordUserEngine.GetUser(ctx.Message.Author);

            DateTime botTime = DateTime.Now;
            DateTime utc = botTime.ToUniversalTime();
            DateTime localTime = user.LocalTime(botTime);

            string response = $"UTC Time: {utc}\n" +
            $"Bot Time: {botTime}\n" +
                $"Your Time: {localTime}\n\n" +
                $"You can change your timezone using /setmytimezone";

            await ctx.Message.RespondAsync(response);

        }

        /// <summary>
        /// Returns a random duck picture
        /// </summary>
        /// <param name="ctx">command context</param>
        /// <returns></returns>
        [Command("duck")]
        public async Task duck(CommandContext ctx)
        {
            DuckEngine duck = new DuckEngine();
            string url = duck.GetRandomDuck();


            await ctx.Message.RespondAsync(url);
        }

        /// <summary>
        /// Gets tje astronomy picture of the day
        /// </summary>
        /// <param name="ctx">command context</param>
        /// <returns></returns>
        [Command("astronomy")]
        public async Task astronomy(CommandContext ctx)
        {
            NASAEngine engine = new NASAEngine();

            await ctx.Message.RespondAsync(engine.GetAPOD().ToString());
        }
    }
}
