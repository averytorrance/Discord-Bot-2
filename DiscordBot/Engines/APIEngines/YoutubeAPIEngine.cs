using System;
using System.Collections.Generic;
using System.Linq;
using DiscordBot.Config;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using DiscordBot.Classes;

namespace DiscordBot.Engines
{
    /// <summary>
    /// https://developers.google.com/youtube/v3/docs/videos
    /// </summary>
    public class YoutubeAPIEngine : APIEngine
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
        /// Max Search Results per search
        /// </summary>
        private int _maxSearch = 3;

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
        /// Adds a discord server to the state dictionary
        /// Creates a new state if one doesn't exist
        /// </summary>
        /// <param name="serverID"></param>
        public void InitalizeState(ulong serverID)
        {
            YoutubeEngineState state;
            if (!serverStates.TryGetValue(serverID, out state))
            {
                state = new YoutubeEngineState(serverID);
                state.SaveState();
            }
        }

        /// <summary>
        /// Subscribes a server to a YT Channel
        /// </summary>
        /// <param name="serverID"></param>
        /// <param name="channel"></param>
        public void SubscribeToChannel(ulong serverID, string channel)
        {
            YoutubeEngineState state;
            if (!serverStates.TryGetValue(serverID, out state))
            {
                state = new YoutubeEngineState(serverID);
            }
            state.SubscribedChannels.Add(channel);          
            serverStates.Add(serverID, state);
            state.SaveState();
        }

        /// <summary>
        /// Unsubscribes a server from a YT Channel
        /// </summary>
        /// <param name="serverID"></param>
        /// <param name="channel"></param>
        public void UnSubscribeFromChannel(ulong serverID, string channel)
        {
            YoutubeEngineState state;
            if (!serverStates.TryGetValue(serverID, out state))
            {
                return;
            }
            state.SubscribedChannels.Remove(channel);
            serverStates.Add(serverID, state);
            state.SaveState();
        }

        /// <summary>
        /// Sends usent youtube videos to the configured discord channel for a discord
        /// </summary>
        /// <param name="serverID"></param>
        public async void SendVideos(ulong serverID)
        {
            Load(serverID);
            YoutubeEngineState state; 
            if(!serverStates.TryGetValue(serverID, out state))
            {
                state = new YoutubeEngineState(serverID);
                state.SaveState();
                serverStates.Add(serverID, state);
                return;
            }

            if(state.SubscribedChannels.Count == 0)
            {
                return;
            }

            DiscordChannel channel;
            ulong channelID = ServerConfig.GetServerConfig(serverID).YTPlanToWatchChannelID;
            if(channelID == null)
            {
                return;
            }

            DiscordServerEngine engine = await DiscordServerEngine.GetDiscordServerEngine(serverID);
            YoutubeError error;
            foreach (string ytchannel in state.SubscribedChannels)
            {
                List<YTVideo> videoList = GetMostRecentVideos(ytchannel, out error);

                if(error != YoutubeError.None)
                {
                    Log.WriteToFile(Log.LogLevel.DiscordBot, "");
                    return;
                }

                foreach(YTVideo video in videoList)
                {
                    if (state.ShouldAddYTVideo(video))
                    {
                        ///Send the video link to the youtube discord channel
                        engine.SendYTMessage(video.URL());
                        state.AddYoutubeVideo(video);
                    }
                }
            }
        }

        /// <summary>
        /// Loads an enginestate for a specific discord server
        /// </summary>
        /// <param name="serverID">discord serverID</param>
        public void Load(ulong serverID)
        {
            YoutubeEngineState state;
            if (serverStates.TryGetValue(serverID, out state))
            {
                serverStates.Remove(serverID);
            }
            state = ServerEngineState.Load<YoutubeEngineState>(new YoutubeEngineState(serverID));
            serverStates.Add(serverID, state);
        }

        /// <summary>
        /// Gets the most recent video for a specific channel
        /// </summary>
        /// <param name="channelID"></param>
        /// <returns></returns>
        public List<YTVideo> GetMostRecentVideos(string channelID, out YoutubeError error)
        {
            error = YoutubeError.None;
            YoutubeResponse response = GenerateObject(_SearchYoutubeChannelVideos(channelID));
            if (response.HasError())
            {
                response.GetErrorType();
            }

            return (response.items ?? new List<YTVideo>()).ToList();
        }

        /// <summary>
        /// Searches for youtube videos for a specific channel, ordered by date.
        /// </summary>
        /// <param name="channelID">Youtube channel ID</param>
        /// <returns></returns>
        private string _SearchYoutubeChannelVideos(string channelID)
        {
            return $"search?part=snippet&channelId={channelID}&maxResults={_maxSearch}&order=date&type=video&key={GetAPIKey()}";
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

    public enum YoutubeError
    {
        None,
        QuotaExceeded,
        Unknown
    }


    public class YoutubeEngineState : ServerEngineState
    {
        /// <summary>
        /// The time when to stop pulling videos.
        /// </summary>
        public DateTime StartTime;

        /// <summary>
        /// List of video IDs that have been sent
        /// </summary>
        public List<YTVideo> Videos = new List<YTVideo>();

        /// <summary>
        /// List of subscribed channels to pull videos from
        /// </summary>
        public List<string> SubscribedChannels = new List<string>();

        [JsonIgnore]
        public override string StateFile_ { get; } = "YoutubeState.JSON";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serverID">Discord Server ID for this state</param>
        public YoutubeEngineState(ulong serverID) : base(serverID) 
        {
            StartTime = DateTime.UtcNow;
            Videos = new List<YTVideo>();
        }

        /// <summary>
        /// True if the video should be added, false if not
        /// </summary>
        /// <returns></returns>
        public bool ShouldAddYTVideo(YTVideo video)
        {
            return !HasYouTubeVideo(video) && video.PublishTime() >= StartTime;
        }

        /// <summary>
        /// Checks if the engine state has an ID for the video ID
        /// </summary>
        /// <param name="video"></param>
        /// <returns></returns>
        public bool HasYouTubeVideo(YTVideo video)
        {
            return Videos.Where(x => x.ID() == video.ID()).Any();
        }

        /// <summary>
        /// Adds a youtube video to the video list
        /// </summary>
        /// <param name="video"></param>
        public void AddYoutubeVideo(YTVideo video)
        {
            if (!HasYouTubeVideo(video))
            {
                Videos.Add(video);
            }
            SaveState();
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

        public error _error;

        /// <summary>
        /// True if the Quota has been exceeded
        /// </summary>
        /// <returns></returns>
        public bool QuotaExceeded()
        {

            if(HasError() && _error.code == 403 && _error.errors.Count > 0)
            {
                return _error.errors.Where(e => e.reason == "quoteExceeded").Any();
            }
            return false;
        }

        /// <summary>
        /// Gets the first error reason
        /// </summary>
        /// <returns></returns>
        public string GetErrorReason()
        {

            if (HasError() && _error.code == 403 && _error.errors.Count > 0)
            {
                return _error.errors.FirstOrDefault().reason;
            }
            return "";
        }

        /// <summary>
        /// Returns true if an error was returned as a response
        /// </summary>
        /// <returns></returns>
        public bool HasError()
        {
            return _error != null;
        }

        /// <summary>
        /// Gets the error type.
        /// </summary>
        /// <returns></returns>
        public YoutubeError GetErrorType()
        {
            if (QuotaExceeded())
            {
                return YoutubeError.QuotaExceeded;
            }

            Console.WriteLine($"Error encountered when calling youtube API: {GetErrorReason()}");
            return YoutubeError.Unknown;
        }

    }

    public class YTVideo : Response, IEquatable<YTVideo>
    {
        public ID id { get; set; }
        public Snippet snippet;

        /// <summary>
        /// The URL to watch a youtube video
        /// </summary>
        [JsonIgnore]
        public static string YoutubeVideoURL = "https://www.youtube.com/watch?v=";

        /// <summary>
        /// Equals method
        /// </summary>
        /// <param name="video"></param>
        /// <returns></returns>
        public bool Equals(YTVideo video)
        {
            return ID() == video.ID();
        }

        /// <summary>
        /// Gets the ID of the youtube video
        /// </summary>
        /// <returns></returns>
        public string ID()
        {
            return id.videoID;
        }

        /// <summary>
        /// Gets the video URL
        /// </summary>
        /// <returns></returns>
        public string URL()
        {
            return $"{YoutubeVideoURL}{ID()}";
        }

        /// <summary>
        /// Gets the time the video was published
        /// </summary>
        /// <returns></returns>
        public DateTime PublishTime()
        {
            return snippet.publishTime;
        }

        /// <summary>
        /// Channel that owns the video
        /// </summary>
        /// <returns></returns>
        public string ChannelID()
        {
            return snippet.channelId;
        }

        /// <summary>
        /// Title of the video
        /// </summary>
        /// <returns></returns>
        public string Title()
        {
            return snippet.title;
        }

        /// <summary>
        /// Channel name of the video owner
        /// </summary>
        /// <returns></returns>
        public string ChannelName()
        {
            return snippet.channelTitle;
        }

        /// <summary>
        /// Video Description
        /// </summary>
        /// <returns></returns>
        public string Description()
        {
            return snippet.description;
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

    public class error
    {
        public int code;
        public List<eitem> errors = new List<eitem>();
    }

    public class eitem
    {
        public string message;
        public string domain;
        public string reason = "";
    }

    #endregion
}
