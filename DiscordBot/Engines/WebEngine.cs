using System.Threading.Tasks;
using System.Net.Http;
using System.IO;

namespace DiscordBot.Engines
{
    class WebEngine
    {

        /// <summary>
        /// Http Client. 
        /// </summary>
        private static HttpClient _client;

        /// <summary>
        /// Constructor
        /// </summary>
        public WebEngine()
        {
            Start();
        }

        /// <summary>
        /// Create a static WebClient
        /// </summary>
        public static void Start()
        {
            if (_client == null)
            {
                _client = new HttpClient();
            }
        }

        /// <summary>
        /// Closes the static webclient
        /// </summary>
        public static void Stop()
        {
            if(_client != null)
            {
                _client.Dispose();
            }
            _client = null;
        }

        /// <summary>
        /// Download an Image
        /// </summary>
        /// <param name="imageURL">url for the image</param>
        /// <param name="outputPath">output filepath</param>
        /// <returns></returns>
        public static async Task DownloadImage(string imageURL, string outputDirectory, string fileName)
        {
            string file = $"{outputDirectory}\\{fileName}";
            byte[] fileBytes = await _client.GetByteArrayAsync(imageURL);
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }
            if (File.Exists(file))
            {
                File.Delete(file);
            }
            File.WriteAllBytes(file, fileBytes);
        }

    }
}
