using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DiscordBot.Engines;
using DiscordBot.Assets;
using DiscordBot.WatchRatings;

namespace DiscordBot.Commands.SlashCommands
{
    public class WatchSlashCommands: ApplicationCommandModule
    {
        /// <summary>
        /// Searches the watch list for entries with specific criteria 
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [SlashCommand("Search", "Search watch entries with specific criteria. Invalid search entries are ignored.")]
        public async Task Search(InteractionContext ctx, [Option("Search", "Term to search.")] string searchTerm = "",
                                                        [Option("WatchYear", "The first year when the movie was watched in the chat.")] long watchYear = -1,
                                                        [Option("ReleaseYear", "The year the movie was released")] long releaseYear = -1,
                                                        [Option("User", "Movies that are rated by this user.")] DiscordUser user = null,
                                                        [Option("IsTV", "Restrict search results to movies or TV instances.")] NullableBool isTV = NullableBool.Null,
                                                        [Option("UserScore", "User score search")] double userScore = -1,
                                                        [Option("Operator", "enum option")] Operator operation = Operator.Equals)
        {
            WatchSearch search = new WatchSearch(){  };

            if(searchTerm != "")
            {
                search.SearchTerm = searchTerm;
            }

            string error = "Unable to search with current parameters.";
            string description = "";
            if(watchYear > 0)
            {
                try
                {
                    search.WatchYear = Convert.ToInt32(watchYear);
                }
                catch
                {
                    description = $"{description}Invalid Watch Year\n";
                    
                }
            }
            if(releaseYear > 0)
            {
                try 
                {
                    search.ReleaseYear = Convert.ToInt32(releaseYear);
                }
                catch
                {
                    description = $"{description}Invalid Release Year\n";
                }
                
            }

            if(description != "")
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(DiscordMessageAssets.GenerateErrorMessage(error, description)));
                return;
            }
            

            if (user != null)
            {
                List<ulong> userIDs = new List<ulong>()
                {
                    user.Id
                };
                search.UserIDs = userIDs;
                if(userScore > 0)
                {
                    search.UserScore = userScore;
                }

            }

            switch (operation)
            {
                case Operator.Equals: 
                    search.Operator = search.EqualsFunction();
                    break;
                case Operator.Nequals:
                    search.Operator = search.NEQFunction();
                    break;
                case Operator.Greater:
                    search.Operator = search.GreaterThanFunction();
                    break;
                case Operator.Greatereqa:
                    search.Operator = search.GreaterThanEqFunction();
                    break;
                case Operator.Less:
                    search.Operator = search.LessThanFunction();
                    break;
                case Operator.Lesseq:
                    search.Operator = search.LessThanEqFunction();
                    break;
            }

            bool? restrictTV = null;
            switch (isTV)
            {
                case (NullableBool.True): restrictTV = true; break;
                case (NullableBool.False): restrictTV = false; break;
            }
            search.IsTV = restrictTV;

            string result = "";
            if (!search.IsNullSearch())
            {
                result = WatchRatingsEngine.CurrentEngine.Search(search, (ulong)ctx.Channel.GuildId);
            }


            

            DiscordInteractionResponseBuilder response = new DiscordInteractionResponseBuilder();

            if(result.Length > 2000)
            {
                result = "Too many results. This will be handled in the future.";
            }

            response.WithContent(result);

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, response);
        }

        public enum Operator
        {
            [ChoiceName("Equals")]
            Equals,
            [ChoiceName("Not Equals")]
            Nequals,
            [ChoiceName("Greater Than")]
            Greater,
            [ChoiceName("Greater Than or Equal To")]
            Greatereqa,
            [ChoiceName("Less Than")]
            Less,
            [ChoiceName("Less Than or Equal To")]
            Lesseq,
        }

        public enum NullableBool
        {
            [ChoiceName("True")]
            True,
            [ChoiceName("False")]
            False,
            [ChoiceName("Null")]
            Null
        }

    }
}
