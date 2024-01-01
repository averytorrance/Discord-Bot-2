using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DiscordBot.Engines
{
    public interface IEngine
    {
        /// <summary>
        /// Engine State
        /// </summary>
        IEngineState State { get; set; }

        /// <summary>
        /// Loads the state of thje engine
        /// </summary>
        void Load();

        /// <summary>
        /// Saves the state of all states in serverStates
        /// </summary>
        void Save();
    }

    public abstract class Engine : IEngine
    {
        /// <summary>
        /// Engine State
        /// </summary>
        public IEngineState State { get; set; }

        /// <summary>
        /// Type of EngineStates that this engine should contain
        /// </summary>
        public abstract Type EngineStateType { get; }

        /// <summary>
        /// Loads a specific server into the serverStates dictionary
        /// </summary>
        /// <param name="serverID"></param>
        public abstract void Load();

        /// <summary>
        /// Saves the state of all states
        /// </summary>
        public void Save()
        {
            State.SaveState();
        }
    }
}
