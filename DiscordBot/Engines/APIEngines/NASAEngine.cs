using DiscordBot.Config;
using System;

namespace DiscordBot.Engines
{
    /// <summary>
    /// https://api.nasa.gov/
    /// </summary>
    public class NASAEngine : APIEngine
    {
        /// <summary>
        /// Base URL
        /// </summary>
        public override string BaseURL => "https://api.nasa.gov/planetary";

        /// <summary>
        /// No API key needed for this API
        /// </summary>
        /// <returns></returns>
        public override string GetAPIKey()
        {
            BotConfig configJsonFile = BotConfig.GetConfig();
            return configJsonFile.NasaAPIKey;
        }

        /// <summary>
        /// The URL to grab a random ducks
        /// </summary>
        /// <returns></returns>
        private string APODURL(DateTime date)
        {
            string parameterString = $"?api_key={GetAPIKey()}&thumbs=true&date={date.ToString("yyyy-MM-dd")}";

            return $"{EndPointURL("apod")}{parameterString}";
        }

        /// <summary>
        /// Returns the URL for a random duck image. 
        /// </summary>
        /// <returns></returns>
        public APOD GetAPOD(DateTime? date = null)
        {
            if(date == null)
            {
                date = DateTime.Now;
            }
            return GenerateObject<APOD>(APODURL((DateTime)date));
        }
    }

    #region Response Classes

    public class APOD
    {
        public DateTime date;
        public string title;
        public string explanation;
        public string hdurl;
        public string url;

        /// <summary>
        /// ToString representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{title}: {explanation}\n{hdurl}";
        }
    }

    #endregion
}
