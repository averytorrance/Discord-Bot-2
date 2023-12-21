using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;

namespace DiscordBot.Assets
{
    public static class DiscordMessageAssets
    {

        /// <summary>
        /// Generic error Message object
        /// </summary>
        /// <returns></returns>
        public static DiscordMessageBuilder GenerateErrorMessage()
        {
            return GenerateErrorMessage("An error has occured");
        }

        /// <summary>
        /// Generic error Message object
        /// </summary>
        /// <returns></returns>
        public static DiscordMessageBuilder GenerateErrorMessage(Exception ex)
        {
            return GenerateErrorMessage($"Error: {ex.Message}", ex.StackTrace);
        }

        /// <summary>
        /// Error Message object
        /// </summary>
        /// <param name="error">error message</param>
        /// <returns></returns>
        public static DiscordMessageBuilder GenerateErrorMessage(string error, string description="")
        {
            return new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                {
                    Title = error,
                    Description = description,
                    Color = DiscordColor.Red
                });
        }

        /// <summary>
        /// Error Message object
        /// </summary>
        /// <param name="error">error message</param>
        /// <returns></returns>
        public static DiscordMessageBuilder GenerateAdminOnlyErrorMessage()
        {
            return GenerateErrorMessage("Access Denied", "You must be an admin to run this command!");
        }

    }
}
