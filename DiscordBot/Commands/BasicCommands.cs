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
            DateTime localTime = user.LocalTime(botTime);

            string response = $"Bot Time: {botTime}\n" +
                $"Your Time: {localTime}\n\n" +
                $"You can change your timezone using /setmytimezone";

            await ctx.Message.RespondAsync(response);

        }

        /// <summary>
        /// Selects a random item from a list of input items
        /// </summary>
        /// <param name="ctx">command context</param>
        /// <param name="names">input parameters. handling added to use ',' as the delimeter for each item.</param>
        /// <returns></returns>
        [Command("test")]
        public async Task test(CommandContext ctx, params string[] names)
        {
            ulong serverID = ctx.Guild.Id;

            ReminderEngine.CurrentEngine.SendReminder(serverID, Int32.Parse(names[0]));

        }




    }
}
