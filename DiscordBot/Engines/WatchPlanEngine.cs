using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordBot.Config;
using DSharpPlus.Entities;
using Newtonsoft.Json;

namespace DiscordBot.Engines
{
    public class WatchPlanEngine : ServerEngine
    {

        public static WatchPlanEngine CurrentEngine;

        public override Type EngineStateType { get; } = typeof(WatchPlanEngineState);

        /// <summary>
        /// Constructor
        /// </summary>
        public WatchPlanEngine() : base()
        {
            CurrentEngine = this;
        }

        /// <summary>
        /// Loads an enginestate for a specific discord server
        /// </summary>
        /// <param name="serverID">discord serverID</param>
        public override void Load(ulong serverID)
        {
            WatchPlanEngineState state;
            if (TryGetValue(serverID, out state))
            {
                serverStates.Remove(serverID);
            }
            state = EngineState.Load<WatchPlanEngineState>(new WatchPlanEngineState(serverID));
            serverStates.Add(serverID, state);
        }

        /// <summary>
        /// Adds an entry to the engine state
        /// </summary>
        /// <param name="message"></param>
        public void AddWatchEntry(DiscordMessage message)
        {
            if (message == null)
            {
                return;
            }
            ValidateServerMessage(message); // Throws exception is message doesn't have a server ID
            if (!IsWatchPlanChannelMessage(message))
            {
                return;
            }

            ulong serverID = (ulong)message.Channel.GuildId;

            WatchPlanEngineState state;
            TryGetValue(serverID, out state);

            WatchSuggestion entry = new WatchSuggestion(message);

            state.AddEntry(entry);
        }

        /// <summary>
        /// Removes a watch entry from the engine state
        /// </summary>
        /// <param name="message"></param>
        public void DeleteWatchEntry(DiscordMessage message)
        {
            if (message == null)
            {
                return;
            }
            ValidateServerMessage(message); // Throws exception is message doesn't have a server ID
            if (!IsWatchPlanChannelMessage(message))
            {
                return;
            }

            ulong serverID = (ulong)message.Channel.GuildId;

            WatchPlanEngineState state;
            TryGetValue(serverID, out state);

            WatchSuggestion entry = new WatchSuggestion(message);

            state.RemoveEntry(entry);
        }

        /// <summary>
        /// Validates a message
        /// </summary>
        /// <param name="message"></param>
        public static bool IsWatchPlanChannelMessage(DiscordMessage message)
        {
            return _getWatchPlanChannelID(message.Channel.Guild.Id) == (ulong)message.Channel.Id;
        }

        /// <summary>
        /// Gets the watch plan channel ID for a specific server
        /// </summary>
        /// <param name="serverID"></param>
        /// <returns></returns>
        private static ulong _getWatchPlanChannelID(ulong serverID)
        {
            ServerConfig config = ServerConfig.GetServerConfig(serverID);
            return config.PlanToWatchChannelID;
        }

    }

    public class WatchPlanEngineState : EngineState
    {
        public List<WatchSuggestion> Suggestions = new List<WatchSuggestion>();

        [JsonIgnore]
        public override string StateFile_ { get; } = "WatchPlan.JSON";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serverID"></param>
        public WatchPlanEngineState(ulong serverID) : base(serverID)
        {

        }

        /// <summary>
        /// Updates an entry in the watch entries state
        /// </summary>
        /// <param name="entry"></param>
        public void AddEntry(WatchSuggestion entry)
        {
            if (Suggestions.Contains(entry))
            {
                return;
            }
            Suggestions.Add(entry);
            SaveState();
        }

        /// <summary>
        /// Updates an entry in the watch entries state
        /// </summary>
        /// <param name="entry"></param>
        public void RemoveEntry(WatchSuggestion entry)
        {
            if (!Suggestions.Contains(entry))
            {
                return;
            }
            Suggestions.Remove(entry);
            SaveState();
        }

    }

    public class WatchSuggestion : IEquatable<WatchSuggestion>
    {
        public ulong MessageID;

        public ulong SuggestedUserID; 

        public WatchSuggestion(DiscordMessage message)
        {
            MessageID = message.Id;
            SuggestedUserID = message.Author.Id;
        }

        /// <summary>
        /// Checks if a Watch Suggestion is equal to another
        /// </summary>
        /// <param name="suggestion"></param>
        /// <returns></returns>
        public bool Equals(WatchSuggestion suggestion)
        {
            return suggestion.MessageID == MessageID;
        }


    }
}
