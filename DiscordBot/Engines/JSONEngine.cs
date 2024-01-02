using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace DiscordBot.Engines
{
    public class JSONEngine
    {

        /// <summary>
        /// Generates an object using its file
        /// </summary>
        /// <typeparam name="T">type of the object</typeparam>
        /// <param name="filePath">filepath to the json file</param>
        /// <returns></returns>
        public T GenerateObject<T>(string filePath)
        {
            string json = File.ReadAllText(filePath);

            T obj = JsonConvert.DeserializeObject<T>(json);
            return obj;
        }

        /// <summary>
        /// Overwrites a json object file
        /// </summary>
        /// <typeparam name="T">type of the json object</typeparam>
        /// <param name="item">object to write to file</param>
        /// <param name="filePath">filepath to the json file</param>
        /// <returns></returns>
        public bool OverwriteObjectFile<T>(T item, string filePath)
        {
            try
            {
                string json = JsonConvert.SerializeObject(item);
                File.WriteAllText(filePath, json);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Generates a list of objects from a json file that consists of a list of objects
        /// </summary>
        /// <typeparam name="T">Type of the object</typeparam>
        /// <param name="filePath">filepath to the json file</param>
        /// <returns></returns>
        public List<T> GenerateListObjects<T>(string filePath)
        {
            string json = File.ReadAllText(filePath);
            if (json == null)
            {
                return null;
            }
            List<T> objectList = JsonConvert.DeserializeObject<List<T>>(json);
            return objectList;
        }

    }
}
