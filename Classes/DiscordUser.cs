using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus;

namespace DiscordBot.UserProfile
{
    public class LocalUser
    {
        /// <summary>
        /// UserName
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// UserID
        /// </summary>
        public ulong UserID { get; set; }

        /// <summary>
        /// Server ID for the User Profile
        /// </summary>
        public ulong ServerID { get; set; }

        /// <summary>
        /// URL for the User's Avatar
        /// </summary>
        public string AvatarURL { get; set; }

        /// <summary>
        /// Address for the User. Must be configured manually
        /// </summary>
        public string Address { get; set;  }

        /// <summary>
        /// Experience Points for the User
        /// </summary>
        public double XP { get; set; } = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        public LocalUser() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="user">Discord Member Object</param>
        public LocalUser(DiscordMember user)
        {
            this.UserID = user.Id;
            this.UserName = user.Username;
            this.ServerID = user.Guild.Id;
            this.AvatarURL = user.AvatarUrl;
        }

        /// <summary>
        /// Checks if a Discord User is equal to another Discord User
        /// </summary>
        /// <param name="user">a Discord user object</param>
        /// <returns>true if the obejcts are the same person on the same server, false otherwise</returns>
        public bool Equals(LocalUser user)
        {
            return user.UserID == UserID && user.ServerID == ServerID;
        }

        /// <summary>
        /// Generates the Embed for the user profile
        /// </summary>
        /// <returns>DisordEmbedBuilder object for the profile</returns>
        public DiscordEmbedBuilder GenerateEmbedProfile()
        {
            DiscordEmbedBuilder profile = new DiscordEmbedBuilder()
                .WithColor(DiscordColor.Purple)
                .WithTitle($"{UserName}'s Profile")
                .WithThumbnail(AvatarURL);
            return profile;
        }

        /// <summary>
        /// Generates a discord message object for this user's profile
        /// </summary>
        /// <returns>DiscordMessageBuilder object for the profile</returns>
        public DiscordMessageBuilder GenerateProfileMessage()
        {
            return new DiscordMessageBuilder().AddEmbed(GenerateEmbedProfile());
        }

    }
}
