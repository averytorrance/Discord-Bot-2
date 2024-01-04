using System.Collections.Generic;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System;
using DiscordBot.Engines;
using DiscordBot.Classes;

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
        /// ID
        /// </summary>
        public ulong ID { get; set; }

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

        /// <summary>
        /// User's TimeZone
        /// </summary>
        public TimeZones _timezone { get; set; } = TimeZones.CST;

        /// <summary>
        /// Creates a new DUser obejct from a Discord User
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static DUser CreateDUser(DiscordUser user) 
        {
            return new DUser()
            {
                UserName = user.Username,
                ID = user.Id,
                AvatarURL = user.AvatarUrl,
            };
        }

        /// <summary>
        /// Checks if a Discord User is equal to another Discord User
        /// </summary>
        /// <param name="user">a Discord user object</param>
        /// <returns>true if the obejcts are the same person on the same server, false otherwise</returns>
        public bool Equals(ServerUser user)
        {
            return user.ID == ID;
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
        /// Gets the TimeZone for a user
        /// </summary>
        /// <returns></returns>
        public TimeZoneInfo TimeZone()
        {
            return Time.TimeZone(_timezone);
        }

        /// <summary>
        /// Gets the User's local time
        /// </summary>
        /// <returns></returns>
        public DateTime LocalTime()
        {
            return LocalTime(DateTime.UtcNow);
        }

        /// <summary>
        /// Gets the User's local time
        /// </summary>
        /// <returns></returns>
        public DateTime LocalTime(DateTime time)
        {
            return Time.ConvertTime(time, _timezone);
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
    public class ServerUser
    {

        public ulong ID { get; set; }

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
        /// <param name="user">Discord Member Object</param>
        public static ServerUser CreateServerUser(DiscordMember user)
        {
            return new ServerUser()
            {
                ID = user.Id,
                ServerID = user.Guild.Id
            };        
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="user">Discord Member Object</param>
        public static ServerUser CreateServerUser(ulong userID, ulong serverID)
        {
            return new ServerUser()
            {
                ID = userID,
                ServerID = serverID
            };
        }

        /// <summary>
        /// Checks if a Discord User is equal to another Discord User
        /// </summary>
        /// <param name="user">a Discord user object</param>
        /// <returns>true if the obejcts are the same person on the same server, false otherwise</returns>
        public new bool Equals(ServerUser user)
        {
            return user.ID == ID && user.ServerID == ServerID;
        }

        /// <summary>
        /// Description text for a user profile
        /// </summary>
        /// <returns></returns>
        public string Description()
        {
            DUser user = GetUser();

            string description = $"TimeZone: {user._timezone.GetName()}\n" +
                $"Server XP: {XP}\n" +
                $"Debt: {Debt}\n";
            return description;
        }

        /// <summary>
        /// Gets the corresponding D User object for this user
        /// </summary>
        /// <returns></returns>
        private DUser GetUser()
        {
            DiscordUserEngine engine = new DiscordUserEngine();
            return engine.GetUser(ID);
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
        /// Adds an amount to a chat members bill.
        /// </summary>
        /// <param name="amount">the amount to add to a chat member's bill</param>
        public void AddXP(double amount = 1)
        {
            this.XP = this.XP+ amount;
        }

        /// <summary>
        /// Generates the Embed for the user profile
        /// </summary>
        /// <returns>DisordEmbedBuilder object for the profile</returns>
        public DiscordEmbedBuilder GenerateEmbedProfile()
        {
            DUser user = GetUser();

            DiscordEmbedBuilder profile = new DiscordEmbedBuilder()
                .WithColor(DiscordColor.Purple)
                .WithTitle($"{user.UserName}'s Profile")
                .WithThumbnail(user.AvatarURL)
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
