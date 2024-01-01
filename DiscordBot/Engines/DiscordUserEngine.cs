using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using DiscordBot.Config;
using DSharpPlus.Entities;
using DSharpPlus;
using DiscordBot.Engines;
using System.Linq;
using System;
using DiscordBot.UserProfile;

namespace DiscordBot.Engines
{
    public class DiscordUserEngine : Engine
    {
        public override Type EngineStateType => typeof(UserEngineState);

        public DiscordUserEngine()
        {
            Load();
        }

        /// <summary>
        /// Creates a user if the user does not exist in the file
        /// </summary>
        /// <param name="user">user to create</param>
        /// <returns>true if the user was created, false otherwise</returns>
        public bool CreateUser(DiscordUser user)
        {
            if (UserExists(user.Id))
            {
                return false;
            }

            GetState().Users.Add(DUser.CreateDUser(user));
            Save();
            return true;
        }

        /// <summary>
        /// Checks if a user profile exists for a user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool UserExists(ulong id)
        {
            return GetState().Users.Where(x => x.ID == id).Any();
        }

        /// <summary>
        /// Gets a Discord user given a Discord User object
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public DUser GetUser(DiscordUser user)
        {
            return GetUser(user.Id);    
        }

        /// <summary>
        /// Gets a Discord User from the State object
        /// </summary>
        /// <param name="id">user ID of the user</param>
        /// <returns>null if the user doesn't exist, user if it exists</returns>
        public DUser GetUser(ulong id)
        {
            if (!UserExists(id))
            {
                return null;
            }

            return GetState().Users.Where(x => x.ID == id).FirstOrDefault();
        }

        /// <summary>
        /// Updates a user profile in the State. Creates a new user with default values if it does not.
        /// </summary>
        /// <param name="user">discord user object</param>
        /// <returns>true if the user was updated, false otherwise</returns>
        public bool UpdateUser(DUser user)
        {
            if (UserExists(user.ID))
            {
                GetState().Users.Remove(user);
            }
            GetState().Users.Add(user);
            GetState().SaveState();
            return true;
        }


        /// <summary>
        /// Updates the user's avatar url in the JSON
        /// </summary>
        /// <param name="user">discord user object</param>
        /// <returns>true if the update was made, false otherwise</returns>
        public bool UpdateUserAvatar(DiscordUser user)
        {
            return UpdateUserAvatar(user.Id, user.AvatarUrl);
        }

        /// <summary>
        /// Updates the user's avatar url in the JSON
        /// </summary>
        /// <param name="id">id of the user to update</param>
        /// <returns>true if the update was made, false otherwise</returns>
        public bool UpdateUserAvatar(ulong id, string avatarURL)
        {
            if (!UserExists(id))
            {
                return false;
            }
            GetUser(id).AvatarURL = avatarURL;
            State.SaveState();
            return true;
        }

        /// <summary>
        /// Gets the state as a UserEngineState object
        /// </summary>
        /// <returns></returns>
        public UserEngineState GetState()
        {
            return (UserEngineState)State;
        }

        /// <summary>
        /// Loads the EngineState
        /// </summary>
        public override void Load()
        {
            State = EngineState.Load<UserEngineState>(new UserEngineState());
        }

    }

    public class UserEngineState : EngineState
    {
        /// <summary>
        /// List of all the Discord Users
        /// </summary>
        public List<DUser> Users = new List<DUser>();

        [JsonIgnore]
        public override string StateFile_ { get; } = "Users.JSON";

    }

}
