﻿using DSharpPlus;
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
using System.Linq;

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
                    ulong ID = ReminderEngine.CurrentEngine.CreateReminder(ctx.Guild.Id, message, ctx.Member.Id, sendTimeUTC, frequency, factor);
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
            reminders = reminders.Select(x => x).OrderBy(x => x.SendTime).ToList();

            string result = "";

            foreach(Reminder reminder in reminders)
            {
                result = $"{result}\n{reminder.ToString(ctx.User.Id)}";
            }

            if (result.Length == 0)
            {
                result = "No responses.";
            }

            DiscordServerEngine engine = new DiscordServerEngine(ctx.Guild);
            engine.SendInteractionResponse(ctx, result);
        }

        /// <summary>
        /// Searches the watch list for entries with specific criteria 
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [SlashCommand("DeleteReminder", "Deletes a reminder if you are the owner of it.")]
        public async Task DeleteReminder(InteractionContext ctx, [Option("ReminderID", "ID of reminder to delete.")] long _id)
        {
            string message = "";
            ulong id;
            try
            {
                try
                {
                    id = (ulong)_id;
                }
                catch
                {
                    throw new ReminderNotFoundException();
                }

                if (ReminderEngine.CurrentEngine.DeleteReminder((ulong)ctx.Channel.GuildId, ctx.Member.Id, id))
                {
                    message = $"Reminder {_id} successfully deleted.";
                }
                else
                {
                    message = "You must be the owner of a reminder to delete it.";
                }
            }
            catch (ReminderNotFoundException)
            {
                message = $"Unable to find reminder {_id}. You can use /ReminderSearch to search for your reminders.";
            }
            finally
            {
                DiscordServerEngine engine = new DiscordServerEngine(ctx.Guild);
                engine.SendInteractionResponse(ctx, message);
            }
        }

        /// <summary>
        /// Searches the watch list for entries with specific criteria 
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [SlashCommand("SubscribeReminder", "Subscribes you to a reminder.")]
        public async Task SubscribeReminder(InteractionContext ctx, [Option("ReminderID", "ID of reminder to subscribe to.")] long _id)
        {
            string message = "";
            ulong id;
            try
            {
                try
                {
                    id = (ulong)_id;
                }
                catch
                {
                    throw new ReminderNotFoundException();
                }

                if (ReminderEngine.CurrentEngine.SubscribeReminder((ulong)ctx.Channel.GuildId, ctx.Member.Id, id))
                {
                    message = $"Successfully subscribed to reminder {_id}.";
                }
                else
                {
                    message = "You can not subscribed to a reminder that you are already subscribed to. You can not subscribe to reminders that you own.";
                }
            }
            catch (ReminderNotFoundException)
            {
                message = $"Unable to find reminder {_id}. You can use /ReminderSearch to search for your reminders.";
            }
            finally
            {
                DiscordServerEngine engine = new DiscordServerEngine(ctx.Guild);
                engine.SendInteractionResponse(ctx, message);
            }
        }

        /// <summary>
        /// Searches the watch list for entries with specific criteria 
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [SlashCommand("UnsubscribeReminder", "Unsubscribes you from a reminder.")]
        public async Task UnsubscribeReminder(InteractionContext ctx, [Option("ReminderID", "ID of reminder to uunsubscribe from.")] long _id)
        {
            string message = "";
            ulong id;
            try
            {
                try 
                {
                    id = (ulong)_id;
                }
                catch
                {
                    throw new ReminderNotFoundException();
                }


                if (ReminderEngine.CurrentEngine.UnsubscribeReminder((ulong)ctx.Channel.GuildId, ctx.Member.Id, id))
                {
                    message = $"Successfully unsubscribed from reminder {_id}.";
                }
                else
                {
                    message = "You must subscribed to a reminder to unsubscribe from it.";
                }
            }
            catch (ReminderNotFoundException)
            {
                message = $"Unable to find reminder {_id}. You can use /ReminderSearch to search for your reminders.";
            }
            finally
            {
                DiscordServerEngine engine = new DiscordServerEngine(ctx.Guild);
                engine.SendInteractionResponse(ctx, message);
            }
        }

    }
}
