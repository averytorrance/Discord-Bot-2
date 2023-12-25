using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DiscordBot.Engines
{
    public interface IServerEngine
    {
        /// <summary>
        /// Dictionary mapping server states to a state object
        /// </summary>
        Dictionary<ulong, IEngineState> serverStates { get; set; }

        /// <summary>
        /// Loads a specific server's engine state. 
        /// Creates a new engine state if the file does not exist
        /// </summary>
        /// <param name="serverID"></param>
        void Load(ulong serverID);

        /// <summary>
        /// Saves the state of all states in serverStates
        /// </summary>
        void Save();
    }

    public abstract class ServerEngine : IServerEngine
    {
        /// <summary>
        /// Dictionary to store enginestates for specific servers
        /// Keys are the discord server IDs
        /// </summary>
        public Dictionary<ulong, IEngineState> serverStates { get; set; }

        /// <summary>
        /// Type of EngineStates that this engine should contain
        /// </summary>
        public abstract Type EngineStateType { get; }

        public ServerEngine()
        {
            serverStates = new Dictionary<ulong, IEngineState>();
        }

        /// <summary>
        /// Loads a specific server into the serverStates dictionary
        /// </summary>
        /// <param name="serverID"></param>
        public abstract void Load(ulong serverID);

        public bool TryGetValue<T>(ulong serverID, out T state)
        {
            state = default(T);
            IEngineState foundState;
            if(serverStates.TryGetValue(serverID, out foundState))
            {
                if (foundState is T)
                {
                    state = (T)foundState;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Saves the state of all states
        /// </summary>
        public void Save()
        {
            Validate();
            foreach (var state in serverStates.Values)
            {
                state.SaveState();
            }
        }

        /// <summary>
        /// Validates the dicitonary states
        /// </summary>
        /// <returns></returns>
        public bool Validate()
        {
            foreach(var state in serverStates.Values)
            {
                if(state.GetType() != EngineStateType.GetType())
                {
                    throw new Exception("Different state type found in ServerStates dictionary");
                } 
            }
            return true;
        }

        /// <summary>
        /// Validates that a discord message has a server ID. 
        /// Throws an exception is GuildID is null.
        /// </summary>
        /// <param name="message"></param>
        public void ValidateServerMessage(DiscordMessage message)
        {
            if (message.Channel.GuildId == null)
            {
                throw new Exception("Non discord server message.");
            }
        }
    }
}
