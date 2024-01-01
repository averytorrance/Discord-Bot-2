using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordBot.Config;
using Newtonsoft.Json;

namespace DiscordBot.Engines
{
    class YoutubeAPIEngine : APIEngine
    {
        private static Dictionary<ulong, YoutubeEngineState> serverStates = new Dictionary<ulong, YoutubeEngineState>();

        /// <summary>
        /// Base URL for the API
        /// </summary>
        public override string BaseURL
        {
            get{ return "https://www.googleapis.com/youtube/v3/"; }
        }

        /// <summary>
        /// The URL to watch a youtube video
        /// </summary>
        public static string YoutubeVideoURL = "https://www.youtube.com/watch?v=";

        /// <summary>
        /// Gets the most recent video for a specific channel
        /// </summary>
        /// <param name="channelID"></param>
        /// <returns></returns>
        public string GetMostRecentVideo(string channelID)
        {
            YoutubeResponse response = GenerateObject(_SearchYoutubeChannelVideos(channelID));
            return $"{YoutubeVideoURL}{response.items.First().ID()}";
        }

        /// <summary>
        /// Searches for youtube videos for a specific channel, ordered by date.
        /// </summary>
        /// <param name="channelID">Youtube channel ID</param>
        /// <returns></returns>
        private string _SearchYoutubeChannelVideos(string channelID)
        {
            return $"search?part=snippet&channelId={channelID}&maxResults=4&order=date&type=video&key={GetAPIKey()}";
        }

        /// <summary>
        /// Gets the API Key
        /// </summary>
        /// <returns></returns>
        public override string GetAPIKey()
        {
            BotConfig configJsonFile = BotConfig.GetConfig();
            return configJsonFile.YoutubeAPIKey;
        }

        /// <summary>
        /// Gets a Youtube response given an API url
        /// </summary>
        /// <param name="requestURL">API request URL</param>
        /// <returns></returns>
        public YoutubeResponse GenerateObject(string requestURL)
        {
            return base.GenerateObject<YoutubeResponse>(requestURL);
        }
    }


    public class YoutubeEngineState : ServerEngineState
    {
        /// <summary>
        /// List of video IDs that have been sent
        /// </summary>
        public List<string> VideoIDs;


        [JsonIgnore]
        public override string StateFile_ { get; } = "YoutubeState.JSON";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serverID">Discord Server ID for this state</param>
        public YoutubeEngineState(ulong serverID) : base(serverID) 
        {
            VideoIDs = new List<string>();
        }

        /// <summary>
        /// Checks if the engine state has an ID for the video ID
        /// </summary>
        /// <param name="video"></param>
        /// <returns></returns>
        public bool HasYouTubeVideo(YTVideo video)
        {
            return VideoIDs.Contains(video.ID());
        }

        /// <summary>
        /// Adds a youtube video to the video list
        /// </summary>
        /// <param name="video"></param>
        public void AddYoutubeVideo(YTVideo video)
        {
            if (!HasYouTubeVideo(video))
            {
                VideoIDs.Add(video.ID());
            }
        }
    }

    #region Response Classes
    public abstract class Response
    {
        public string kind;
        public string etag;
    }

    public class YoutubeResponse : Response
    {
        public string nextPageToken;
        public string regionCode;

        public List<YTVideo> items;

    }

    public class YTVideo : Response
    {
        public ID id { private get; set; }
        public Snippet snippet;

        public string ID()
        {
            return id.videoID;
        }
    }

    public class ID
    {
        public string kind;
        public string videoID;
    }

    public class Snippet
    {
        public DateTime publishedAt;
        public string channelId;
        public string title;
        public string description;
        public string channelTitle;
        public DateTime publishTime;
    }
    #endregion
}
