﻿using System;
using System.IO;
using Newtonsoft.Json;

namespace DiscordBot.Engines
{
    public interface IEngineState
    {
        /// <summary>
        /// Directory that stores the Engine State file
        /// </summary>
        /// <returns></returns>
        string FileDirectory();

        /// <summary>
        /// State File with the directory appended to it.
        /// This should be called when interacting with the enginestate file.
        /// </summary>
        /// <returns></returns>
        string StateFile();

        /// <summary>
        /// Saves the ServerEngineState
        /// </summary>
        /// <returns></returns>
        bool SaveState();
    }

    public abstract class EngineState : IEngineState
    {
        /// <summary>
        /// The name of the file storing the state
        /// </summary>
        [JsonIgnore]
        public abstract string StateFile_ { get; }

        /// <summary>
        /// Filepath to the Reminders JSON.
        /// </summary>
        /// <param name="serverID"></param>
        /// <returns></returns>
        public string FileDirectory()
        {
            return "";
        }

        /// <summary>
        /// File to store this state
        /// </summary>
        /// <param name="serverID"></param>
        /// <returns></returns>
        public string StateFile()
        {
            if(StateFile_ == null)
            {
                throw new Exception("Attempted to pull state file, when the file is undefined.");
            }
            return $"{FileDirectory()}{StateFile_}";
        }

        /// <summary>
        /// Creates an engine state from the JSON file
        /// </summary>
        /// <returns></returns>
        public static T Load<T>(IEngineState engineState)
        {
            JSONEngine engine = new JSONEngine();

            if (engineState.FileDirectory() != "" && !Directory.Exists(engineState.FileDirectory()))
            {
                Directory.CreateDirectory(engineState.FileDirectory());
            }

            string file = engineState.StateFile();

            if (!File.Exists(file))
            {
                engine.OverwriteObjectFile(engineState, file);
            }
            return engine.GenerateObject<T>(file);
        }

        /// <summary>
        /// Saves the engine state
        /// </summary>
        /// <returns></returns>
        public bool SaveState()
        {
            try
            {
                JSONEngine engine = new JSONEngine();
                return engine.OverwriteObjectFile(this, StateFile());
            }
            catch(Exception e)
            {
                Console.WriteLine("Failed to save state");
                return false;
            }
        }
    }
}
