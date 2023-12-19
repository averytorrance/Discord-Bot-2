using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace DiscordBot.Engines
{
    class JSONEngine
    {
        public T GenerateObject<T>(string filePath)
        {
            string json = File.ReadAllText(filePath);

            T obj = JsonConvert.DeserializeObject<T>(json);
            return obj;
        }

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
