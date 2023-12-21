using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace DiscordBot.Classes
{
    /// <summary>
    /// Class to represent a poll
    /// </summary>
    class Poll
    {
        public string Question;

        public Dictionary<DiscordEmoji, PollOption> OptionMap;

        public long TimeLimit;

        public Poll(string question, Dictionary<DiscordEmoji, PollOption> optionMap, long timeLimit)
        {
            Question = question;
            TimeLimit = timeLimit;

            // Remove Null Poll options
            PollOption option;

            List<DiscordEmoji> removeList = new List<DiscordEmoji>();

            foreach (DiscordEmoji emoji in optionMap.Keys)
            {
                optionMap.TryGetValue(emoji, out option);
                if(option.Option == "")
                {
                    removeList.Add(emoji);
                }
            }
            foreach(DiscordEmoji emoji in removeList)
            {
                optionMap.Remove(emoji);
            }

            OptionMap = optionMap;


        }

        /// <summary>
        /// Increments an emoji option.
        /// </summary>
        /// <param name="emoji">emoji to increment</param>
        /// <returns>true if an option was incremeneted, false otherwise</returns>
        public bool IncrementOption(DiscordEmoji emoji)
        {
            PollOption option;
            if (OptionMap.ContainsKey(emoji))
            {
                OptionMap.TryGetValue(emoji, out option);
                option.Increment();
                OptionMap.Remove(emoji);
                OptionMap.Add(emoji, option);
                return true;
            }
            return false;

        }

        /// <summary>
        /// Construts a discord message given a dictionary of poll results
        /// </summary>
        /// <param name="results">dictionary of discord emojis and their counts</param>
        /// <returns></returns>
        public DiscordMessageBuilder GeneratePollResults()
        {
            string resultsString = "";
            int total = 0;
            int count;
            PollOption option;

            foreach (DiscordEmoji emoji in OptionMap.Keys)
            {
                OptionMap.TryGetValue(emoji, out option);
                resultsString = $"{resultsString} {option.Option} : {option.Count}\n";
                total = total + option.Count;
            }

            resultsString = $"{resultsString}\n\n Total Votes: {total}";

            return new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.Green)
                    .WithTitle("Poll Results")
                    .WithDescription(resultsString));
        }

        /// <summary>
        /// Creates a discord message containing the poll options
        /// </summary>
        /// <returns></returns>
        public DiscordMessageBuilder PollOptionsMessage()
        {
            return new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.Azure)
                    .WithTitle(Question)
                    .WithDescription(PollOptions()));
        }

        /// <summary>
        /// Generates a string representing an emoji and its corresponding option
        /// </summary>
        /// <returns></returns>
        public string PollOptions()
        {
            PollOption option;
            string options = $"The poll will end in";
            if(TimeLimit != 1)
            {
                options = $"{options} {TimeLimit} minutes.\n";
            }
            else
            {
                options = $"{options} {TimeLimit} minute.\n";
            }
            foreach (DiscordEmoji emoji in OptionMap.Keys)
            {
                OptionMap.TryGetValue(emoji, out option);
                options = $"{options} {emoji} : {option.Option}\n";
            }

            return options;
        }

    }

    /// <summary>
    /// Class to represent a poll option
    /// </summary>
    class PollOption
    {
        //public DiscordEmoji Emoji { get; private set; }

        public string Option { get; private set; }

        public int Count { get; private set; }

        public PollOption(string option)
        {
            //this.Emoji = emoji;
            this.Option = option;
            this.Count = 0;
        }

        /// <summary>
        /// Increases the count of the poll option
        /// </summary>
        /// <returns>the poll count after increasing by 1.</returns>
        public int Increment()
        {
            Count++;
            return Count;
        }

        /// <summary>
        /// Decreases the poll count by 1. Throws an exception if count goes below 0.
        /// </summary>
        /// <returns>the poll count after decreasing by 1.</returns>
        public int Decrement()
        {
            Count--;
            if(Count < 0)
            {
                throw new Exception("Attempted to make poll count negative.");
            }
            return Count;
        }

    }


}
