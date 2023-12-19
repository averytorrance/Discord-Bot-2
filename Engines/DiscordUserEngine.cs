using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using DiscordBot.Config;
using DSharpPlus.Entities;
using DSharpPlus;
using DiscordBot.Engines;
using System.Linq;

namespace DiscordBot.UserProfile
{
    public class LocalUserEngine
    {
        private const string FilePath = "UserInfo.json";

        /// <summary>
        /// Gets the user or creates the user profile if it doesn't exist
        /// </summary>
        /// <param name="user">Local user object to search for</param>
        /// <returns>a local user object</returns>
        public LocalUser GetCreateUser(LocalUser user)
        {
            if (!UserExists(user))
            {
                CreateUser(user);
            }
            UpdateUserAvatar(user);

            return GetUser(user.UserID, user.ServerID);
        }

        /// <summary>
        /// Creates a user if the user does not exist in the file
        /// </summary>
        /// <param name="user">user to create</param>
        /// <returns>true if the user was created, false otherwise</returns>
        public bool CreateUser(LocalUser user)
        {
            if (UserExists(user))
            {
                return false;
            }

            JSONEngine jsonEngine = new JSONEngine();
            List<LocalUser> users = jsonEngine.GenerateListObjects<LocalUser>(FilePath);
            if(users == null)
            {
                users = new List<LocalUser>();
            }

            users.Add(user);
            return jsonEngine.OverwriteObjectFile(users, FilePath);
        }

        /// <summary>
        /// Checks if a user profile exists for a user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool UserExists(LocalUser user)
        {
            return UserExists(user.UserID, user.ServerID);
        }

        /// <summary>
        /// Checks if a user profile exists for a user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool UserExists(ulong userID, ulong serverID)
        {
            LocalUser user = GetUser(userID, serverID);
            return user != null;
        }

        /// <summary>
        /// Gets a Discord User from the JSON file
        /// </summary>
        /// <param name="userID">user ID of the user</param>
        /// <param name="serverID">server ID for the user profile</param>
        /// <returns>a discord user object or null if it is not found in the JSON</returns>
        public LocalUser GetUser(ulong userID, ulong serverID)
        {
            JSONEngine jsonEngine = new JSONEngine();

            List<LocalUser> usersToGet = jsonEngine.GenerateListObjects<LocalUser>(FilePath);
            if(usersToGet == null) { return null; }

            foreach (LocalUser user in usersToGet)
            {
                if (user.UserID == userID && user.ServerID == serverID)
                {
                    return user;
                }
            }

            return null;
        }

        /// <summary>
        /// Updates a user profile in the JSON. Creates the user if it does not exist. 
        /// </summary>
        /// <param name="user">discord user object</param>
        /// <returns>true if the user was updated, false otherwise</returns>
        public bool UpdateUser(LocalUser user)
        {
            JSONEngine jsonEngine = new JSONEngine();
            if (!UserExists(user))
            {
                return CreateUser(user);
            }

            List<LocalUser> allUsers = jsonEngine.GenerateListObjects<LocalUser>(FilePath);

            // We already check if the user exists, so we can assume that the user will be removed.
            foreach (LocalUser currentUser in allUsers)
            {
                if (currentUser.Equals(user))
                {
                    allUsers.Remove(currentUser);
                    break;
                }
            }

            allUsers.Add(user);
            return jsonEngine.OverwriteObjectFile(allUsers, FilePath);
        }

        /// <summary>
        /// Updates the user's avatar url in the JSON
        /// </summary>
        /// <param name="user">the discord user object</param>
        /// <returns>true if the update was made, false otherwise</returns>
        public bool UpdateUserAvatar(LocalUser user)
        {
            LocalUser localInstance = GetUser(user.UserID, user.ServerID);
            localInstance.AvatarURL = user.AvatarURL;
            return UpdateUser(localInstance);
        }
    }

    public class DiscordUserEngine
    {
        /// <summary>
        /// Checks if a user has Administrator Permissions
        /// </summary>
        /// <param name="user">Discord Member object. Pulls from a discord server.</param>
        /// <returns></returns>
        public static bool IsAdmin(DiscordMember user)
        {
            return user.Permissions.HasFlag(Permissions.Administrator);
        }
    }
}
