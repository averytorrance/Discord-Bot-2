using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DiscordBot.Classes;
using DiscordBot.UserProfile;
using DiscordBot.Engines;
using DiscordBot.Assets;
using DiscordBot.Config;

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
        public async Task SetReminder(InteractionContext ctx, [Option("Reminder", "The Reminder message")] string message,
                                                                [Option("Date", "The Reminder Date.")] string date,
                                                                [Option("Time", "The Reminder Time.")] string time = "12AM")
        {
            ServerConfig config = ServerConfig.GetServerConfig(ctx.Guild.Id);
            if(config.ReminderChannelID == null && config.BotChannelID == null)
            {
                throw new Exception("No reminder channel or bot channel configured.");
            }

            DateTime sendTime;
            DiscordMessageBuilder response;
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
            string description; 

            if (DateTime.TryParse($"{date} {time}", out sendTime))
            {
                if (sendTime < DateTime.Now)
                {

                    description = "Input date/time is in the past. Reminders must be configured with a future time.";
                }
                else
                {

                    ReminderEngine.CreateReminder(ctx.Guild.Id, message, ctx.Member.Id, sendTime);
                    response = new DiscordMessageBuilder()
                                .AddEmbed(new DiscordEmbedBuilder()
                                {
                                    Title = "Successfully created reminder",
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


    }
}
