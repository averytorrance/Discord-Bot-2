namespace DiscordBot.Engines
{
    public class DuckEngine : APIEngine
    {
        /// <summary>
        /// Base URL
        /// </summary>
        public override string BaseURL => "https://random-d.uk/api";

        /// <summary>
        /// No API key needed for this API
        /// </summary>
        /// <returns></returns>
        public override string GetAPIKey()
        {
            return "";
        }

        /// <summary>
        /// The URL to grab a random ducks
        /// </summary>
        /// <returns></returns>
        private string RandomDuckURL()
        {
            return EndPointURL("random");
        }

        /// <summary>
        /// Generates a RandomDuck object
        /// </summary>
        /// <returns></returns>
        public RandomDuck GenerateObject()
        {
            return base.GenerateObject<RandomDuck>(RandomDuckURL());
        }

        /// <summary>
        /// Returns the URL for a random duck image. 
        /// </summary>
        /// <returns></returns>
        public string GetRandomDuck()
        {
            return GenerateObject().url;
        }
    }

    #region Response Classes

    public class RandomDuck
    {
        public string url;
        public string message;
    }

    #endregion
}
