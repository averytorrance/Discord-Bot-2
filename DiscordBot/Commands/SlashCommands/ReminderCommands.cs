using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System;
using System.Threading.Tasks;
using DiscordBot.Engines;
using DiscordBot.Assets;
using DiscordBot.Config;
using DiscordBot.Classes;
using System.Collections.Generic;
using System.IO;
using DiscordBot.UserProfile;

namespace DiscordBot.Commands.SlashCommands
{
    class ReminderCommands : ApplicationCommandModule
    {
        /// <summary>
        /// Displays the profile for the user. Creates a profile for a user if it does not exist. 
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [SlashCommand("SetReminder", "Sets a reminder")]
        public async Task SetReminder(InteractionContext ctx, [Option("Reminder", "The reminder message.")] string message,
                                                                [Option("Date", "The reminder date.")] string date,
                                                                [Option("Time", "The reminder time.")] string time = "12AM",
                                                                [Option("Frequency", "The frequency of the reminder.")] Freq frequency = Freq.None,
                                                                [Option("FrequencyFactor", "The amount of the frequency to add after a reminder is sent.")] long frequencyFactor = 1)
        {
            ServerConfig config = ServerConfig.GetServerConfig(ctx.Guild.Id);
            if(config.ReminderChannelID == null && config.BotChannelID == null)
            {
                throw new Exception("No reminder channel or bot channel configured.");
            }

            DiscordUserEngine discordUserEngine = new DiscordUserEngine();
            if (!discordUserEngine.UserExists(ctx.Member.Id))
            {
                discordUserEngine.CreateUser(ctx.Member);
            }

            DateTime sendTime;
            DiscordMessageBuilder response = new DiscordMessageBuilder();
            //Handling for t+x inputs
            if (date.ToLower()[0] == 't')
            {
                if(date.Length == 1)
                {
                    date = DateTime.Today.ToString("MM/dd/yyyy");
                }
                else if(date[1] == '+')
                {
                    string offset = date.Split('+')[1];
                    int days;
                    if(Int32.TryParse(offset, out days))
                    {
                        date = DateTime.Today.AddDays(days).ToString("MM/dd/yyyy");
                    }
                }
            }
            string error = "Unable to create reminder.";
            string description = "";
            int factor = 1; 

            if(frequencyFactor > Int32.MaxValue || frequencyFactor < 1)
            {
                description = "Invalid frequency value.";

            }
            else
            {
                factor = (int)frequencyFactor;
            }

            if (string.IsNullOrEmpty(description) && DateTime.TryParse($"{date} {time}", out sendTime))
            {
                sendTime = DateTime.SpecifyKind(sendTime, DateTimeKind.Unspecified);

                //Convert send time to UTC. Original timezone is pulled from User Profile, which defaults to CST.
                TimeZoneInfo localTimeZone = discordUserEngine.GetUser(ctx.Member.Id).TimeZone();
                TimeZoneInfo timeZoneUTC = TimeZoneInfo.FindSystemTimeZoneById(TimeZones.UTC.GetName());
                DateTime sendTimeUTC = TimeZoneInfo.ConvertTime(sendTime, localTimeZone, timeZoneUTC);

                if (sendTimeUTC < DateTime.UtcNow)
                {

                    description = "Input date/time is in the past. Reminders must be configured with a future time.";
                }
                else
                {
                    int ID = ReminderEngine.CurrentEngine.CreateReminder(ctx.Guild.Id, message, ctx.Member.Id, sendTimeUTC, frequency, factor);
                    response.AddEmbed(new DiscordEmbedBuilder()
                    {
                        Title = $"Successfully created reminder {ID}.",
                        Description = $"{sendTime} : {message}",
                        Color = DiscordColor.Green
                    });
                }
            }
            else
            {
                error = "Invalid invalid formatting";
                if (!DateTime.TryParse($"{date}", out sendTime))
                {
                    description = "The date format is invalid.";
                }
                else
                {
                    description = "The time format is invalid.";
                }
            }

            if (!string.IsNullOrEmpty(description))
            {
                response = DiscordMessageAssets.GenerateErrorMessage(error, description);
            }

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(response));
        }

        /// <summary>
        /// Searches the watch list for entries with specific criteria 
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [SlashCommand("ReminderSearch", "Search reminders with specific criteria. Invalid search entries are ignored.")]
        public async Task ReminderSearch(InteractionContext ctx, [Option("Search", "Search terms for the search")] string searchTerms = "",
                                                         [Option("SearchSubscriptions", "True to only include subscriptions, false to only show owned reminders, null to include all.")] bool? includeSubs = null,
                                                         [Option("SearchRecurring", "True to only include reoccuring, false to only show single time reminders, null to include all.")] bool? isRecurring = null)


        {
            ReminderSearch search = new ReminderSearch(ctx.User.Id)
            {
                SearchSubscriptions = includeSubs,
                SearchTerm = searchTerms,
                SearchRecurring = isRecurring
                
            };

            List<Reminder> reminders = ReminderEngine.CurrentEngine.Search(search, (ulong)ctx.Channel.GuildId);

            string result = "";

            foreach(Reminder reminder in reminders)
            {
                result = $"{result}\n{reminder.ToString(ctx.User.Id)}";
            }

            DiscordInteractionResponseBuilder response = new DiscordInteractionResponseBuilder();

            if (result.Length > 2000)
            {
                string filePath = $"{ServerConfig.ServerDirectory(ctx.Guild.Id)}{DateTime.Now.Ticks}.txt";
                File.WriteAllText(filePath, result);
                FileStream file = new FileStream(filePath, FileMode.Open);
                response.AddFile(file);
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, response);
                file.Close();
                File.Delete(filePath);
                return;
            }
            if(result.Length == 0)
            {
                result = "No Results.";
            }

            

            response.WithContent(result);

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, response);
        }
    }
}
