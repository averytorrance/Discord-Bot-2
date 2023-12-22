using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DiscordBot.Classes;
using DSharpPlus.Interactivity.EventHandling;

namespace DiscordBot.Commands.SlashCommands
{
    public class PollCommands : ApplicationCommandModule
    {
        [SlashCommand("poll", "Create a poll")]
        public async Task Poll(InteractionContext ctx, [Option("question", "The question for the poll")] string question,
                                                        [Option("option1", "1st poll option")] string option1,
                                                        [Option("option2", "2nd poll option")] string option2,
                                                        [Option("option3", "3rd poll option")] string option3 = "",
                                                        [Option("option4", "4th poll option")] string option4 = "",
                                                        [Option("option5", "5th poll option")] string option5 = "",
                                                        [Option("timelimit", "The time set on this poll in minutes")] long timeLimit = 5)
        {
            var interactvity = Program.Client.GetInteractivity(); //Getting the Interactivity Module
            TimeSpan timer = TimeSpan.FromMinutes(timeLimit); //Converting my time parameter to a timespan variable

            Dictionary<DiscordEmoji, PollOption> optionMap = new Dictionary<DiscordEmoji, PollOption>()
            {
                { DiscordEmoji.FromName(Program.Client, ":one:", false), new PollOption(option1)},
                { DiscordEmoji.FromName(Program.Client, ":two:", false), new PollOption(option2)},
                { DiscordEmoji.FromName(Program.Client, ":three:", false), new PollOption(option3)},
                { DiscordEmoji.FromName(Program.Client, ":four:", false), new PollOption(option4)},
                { DiscordEmoji.FromName(Program.Client, ":five:", false), new PollOption(option5)}
            };

            Poll poll = new Poll(question, optionMap, timeLimit);

            DiscordInteractionResponseBuilder initialPollMessageContent = new DiscordInteractionResponseBuilder(poll.PollOptionsMessage());
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, initialPollMessageContent);

            DiscordMessage initialMessage = await ctx.GetOriginalResponseAsync(); //Storing the await command in a variable

            foreach (DiscordEmoji emoji in poll.OptionMap.Keys)
            {
                await initialMessage.CreateReactionAsync(emoji);
            }

            var result = await interactvity.CollectReactionsAsync(initialMessage, timer); //Collects all the emoji's and how many peopele reacted to those emojis

            foreach (Reaction reaction in result) //Foreach loop to go through all the emojis in the message and filtering out the 4 emojis we need
            {
                poll.IncrementOption(reaction);
            }

            await initialMessage.RespondAsync(poll.GeneratePollResults()); 
        }
    }
}
