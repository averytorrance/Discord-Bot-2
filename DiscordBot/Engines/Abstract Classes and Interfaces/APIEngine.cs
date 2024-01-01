using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordBot.Config;
using Newtonsoft.Json;
using RestSharp;

namespace DiscordBot.Engines
{
    public abstract class APIEngine
    {
        /// <summary>
        /// Base URL for the API
        /// </summary>
        public abstract string BaseURL { get; }

        /// <summary>
        /// Method to get the API Key from the configuration file.
        /// </summary>
        /// <returns></returns>
        public abstract string GetAPIKey();

        /// <summary>
        /// Returns the base URL with an appended endpoint
        /// </summary>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        public string EndPointURL(string endpoint)
        {
            return $"{BaseURL}/{endpoint}";
        }

        /// <summary>
        /// Generates a request object. Classes implementing this abstract class
        /// should create a wrapper for this method that returns the proper object class.
        /// </summary>
        /// <typeparam name="T">Response Object Type</typeparam>
        /// <param name="requestURL">URL for the request</param>
        /// <returns></returns>
        public T GenerateObject<T>(string requestURL)
        {
            RestClient restClient = new RestClient(BaseURL);

            var request = new RestRequest(requestURL, Method.Get);

            request.OnBeforeDeserialization = response => { response.ContentType = "application/json"; };
            var result = restClient.Execute(request);

            return JsonConvert.DeserializeObject<T>(result.Content);
        }

    }

}
