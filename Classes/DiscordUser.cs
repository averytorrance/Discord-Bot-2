using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus;

namespace DiscordBot.UserProfile
{
    /// <summary>
    /// Discord User
    /// Used for private messaging
    /// </summary>
    public class DUser
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
        /// URL for the User's Avatar
        /// </summary>
        public string AvatarURL { get; set; }

        /// <summary>
        /// Address for the User. Must be configured manually
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Email for the user. 
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Alternative users that this user uses
        /// </summary>
        public List<ulong> AltUsers { get; set; }

        /// <summary>
        /// True if the user is an Alt  User
        /// </summary>
        public bool IsAltUser { get; set; }

        public DUser() { }

        /// <summary>
        /// Checks if a Discord User is equal to another Discord User
        /// </summary>
        /// <param name="user">a Discord user object</param>
        /// <returns>true if the obejcts are the same person on the same server, false otherwise</returns>
        public bool Equals(LocalUser user)
        {
            return user.UserID == UserID;
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
                .WithThumbnail(AvatarURL)
                .WithDescription(GenerateDescription());
            return profile;
        }

        public string GenerateDescription()
        {
            return $"Address: {Address}";
        }

        public bool SetAddress(string address)
        {

            Address = address;
            return true;
        }

        public bool ValidateAddress()
        {
            return true;
        }

        public bool SetEmail(string email)
        {
            Email = email;
            return true;
        }

        public bool ValidateEmail()
        {
            return true;
        }



    }


    /// <summary>
    /// Discord User specific to a server
    /// Used for interactions in a specific discord server
    /// </summary>
    public class LocalUser : DUser
    {
        /// <summary>
        /// Server ID for the User Profile
        /// </summary>
        public ulong ServerID { get; set; }

        /// <summary>
        /// Experience Points for the User
        /// </summary>
        public double XP { get; set; } = 0;

        /// <summary>
        /// The amount of money in USD that a chat member owes to the chat fund.
        /// </summary>
        public double Debt { get; set; } = 0;

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
        public new bool Equals(LocalUser user)
        {
            return user.UserID == UserID && user.ServerID == ServerID;
        }

        public string Description()
        {
            string description = $"Server XP: {XP}\n" +
                $"Debt: {Debt}\n";
            return description;
        }

        /// <summary>
        /// Adds an amount to a chat members bill.
        /// </summary>
        /// <param name="amount">the amount to add to a chat member's bill</param>
        public void AddToDebt(double amount = 1)
        {
            this.Debt = this.Debt + amount;
        }

        /// <summary>
        /// Generates the Embed for the user profile
        /// </summary>
        /// <returns>DisordEmbedBuilder object for the profile</returns>
        public new DiscordEmbedBuilder GenerateEmbedProfile()
        {
            DiscordEmbedBuilder profile = new DiscordEmbedBuilder()
                .WithColor(DiscordColor.Purple)
                .WithTitle($"{UserName}'s Profile")
                .WithThumbnail(AvatarURL)
                .WithDescription(Description());
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
