using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using DiscordBot.Config;
using System.Threading;
using DiscordBot.WatchRatings;
using DiscordBot.Classes;
using DiscordBot.Engines.Tasks;

namespace DiscordBot.Engines
{
    public class WatchRatingsEngine : ServerEngine
    {
        public static WatchRatingsEngine CurrentEngine;

        public static bool TaskRunning;

        public override Type EngineStateType { get; } = typeof(WatchRatingsEngineState);

        public static readonly Dictionary<DiscordEmoji, int> ScoreMap =
            new Dictionary <DiscordEmoji, int>()
            {
                { DiscordEmoji.FromName(Program.Client, ":one:", false), 1},
                { DiscordEmoji.FromName(Program.Client, ":two:", false), 2},
                { DiscordEmoji.FromName(Program.Client, ":three:", false), 3},
                { DiscordEmoji.FromName(Program.Client, ":four:", false), 4},
                { DiscordEmoji.FromName(Program.Client, ":five:", false), 5},
                { DiscordEmoji.FromName(Program.Client, ":six:", false), 6},
                { DiscordEmoji.FromName(Program.Client, ":seven:", false), 7},
                { DiscordEmoji.FromName(Program.Client, ":eight:", false), 8},
                { DiscordEmoji.FromName(Program.Client, ":nine:", false), 9},
                { DiscordEmoji.FromName(Program.Client, ":keycap_ten:", false), 10}
            };

        public static readonly DiscordEmoji HalfScore = DiscordEmoji.FromName(Program.Client, ":up:", false);

        /// <summary>
        /// Constructor
        /// </summary>
        public WatchRatingsEngine() : base()
        {
            CurrentEngine = this;
        }

        #region Start Up
        /// <summary>
        /// Startup Methods
        /// </summary>
        /// <param name="serverID"></param>
        public async void StartUp(ulong serverID)
        {
            Load(serverID);
            await AddMissingMessages(serverID);
            TaskEngine.CurrentEngine.AddTask(new ArchiveWatchRatingsTask(serverID));
        }

        /// <summary>
        /// Loads an enginestate for a specific discord server
        /// </summary>
        /// <param name="serverID">discord serverID</param>
        public override void Load(ulong serverID)
        {
            WatchRatingsEngineState state;
            if (TryGetValue(serverID, out state))
            {
                serverStates.Remove(serverID);
            }
            state = ServerEngineState.Load<WatchRatingsEngineState>(new WatchRatingsEngineState(serverID));
            serverStates.Add(serverID, state);
        }

        /// <summary>
        /// Generates a new Watch Ratings state using all of the messages before the 
        /// message with latestMessageID. 
        /// </summary>
        /// <param name="serverID"></param>
        /// <param name="latestMessageID"></param>
        public async void GenerateNewState(ulong serverID, ulong latestMessageID)
        {
            List<WatchEntry> archivedEntries = _getArchivedEntries(serverID);
            if (serverStates.ContainsKey(serverID))
            {
                serverStates.Remove(serverID);
            }
            WatchRatingsEngineState newState = new WatchRatingsEngineState(serverID);
            newState.ArchivedEntries = archivedEntries;
            serverStates.Add(serverID, newState);

            ulong WatchChannelID = _getWatchChannelID(serverID);
            DiscordChannel watchChannel = await Program.Client.GetChannelAsync(WatchChannelID);

            List<DiscordMessage> messages = await _getAllMessagesBefore(watchChannel, latestMessageID);

            await UpdateWatchEntries(messages.Select(x => x.Id).ToList(), serverID); 
        }

        /// <summary>
        /// Gets  a list of all of the messages before a specific message
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="latestMessageID"></param>
        /// <returns></returns>
        private async Task<List<DiscordMessage>> _getAllMessagesBefore(DiscordChannel channel, ulong latestMessageID)
        {
            List<DiscordMessage> messages = new List<DiscordMessage>();
            if (latestMessageID == null)
            {
                return messages;
            }

            IReadOnlyList<DiscordMessage> watchMessages = await channel.GetMessagesBeforeAsync(latestMessageID);

            if(watchMessages == null || watchMessages.Count == 0)
            {
                return messages;
            }

            messages.AddRange(watchMessages);

            ulong lastMessageID = messages.Last().Id;
            if(lastMessageID == null || watchMessages.Count < 100)
            {
                return messages;
            }

            List<DiscordMessage> addThese = await _getAllMessagesBefore(channel, lastMessageID);
            messages.AddRange(addThese);
            return messages;
        }

        /// <summary>
        /// Adds missing messages in the watch channel to the server state. This can happen if the bot goes offline
        /// </summary>
        /// <param name="serverID"></param>
        public async Task<bool> AddMissingMessages(ulong serverID)
        {
            ulong WatchChannelID = _getWatchChannelID(serverID);
            DiscordChannel watchChannel = await Program.Client.GetChannelAsync(WatchChannelID);

            WatchRatingsEngineState state;
            if(!TryGetValue(serverID, out state))
            {
                return false;
            }

            //Handling for if the newest entry message was deleted
            ulong? messageID = state.GetNewestEntry();

            if(messageID == null)
            {
                return true;
            }

            while(DiscordHandler.GetMessage(watchChannel, (ulong)messageID) == null)
            {
                state.ArchiveEntry((ulong)messageID);
                messageID = state.GetNewestEntry();
            }
            

            List<DiscordMessage> messages = await _getAllMessagesAfter(watchChannel, (ulong) messageID );
            messages = messages.Where(x => !state.HasEntry(x.Id)).ToList(); // List of missing messages
            await UpdateWatchEntries(messages.Select(x => x.Id).ToList(), serverID);
            return true;
        }

        /// <summary>
        /// Gets  a list of all of the messages before a specific message
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="latestMessageID"></param>
        /// <returns></returns>
        private async Task<List<DiscordMessage>> _getAllMessagesAfter(DiscordChannel channel, ulong afterMessageID)
        {
            List<DiscordMessage> messages = new List<DiscordMessage>();
            if (afterMessageID == null)
            {
                return messages;
            }

            IReadOnlyList<DiscordMessage> watchMessages = await channel.GetMessagesAfterAsync(afterMessageID);

            if (watchMessages == null || watchMessages.Count == 0)
            {
                return messages;
            }

            messages.AddRange(watchMessages);

            ulong lastMessageID = messages.OrderBy(x => x.Id).Last().Id;
            if (lastMessageID == null || watchMessages.Count < 100)
            {
                return messages;
            }

            List<DiscordMessage> addThese = await _getAllMessagesAfter(channel, lastMessageID);
            messages.AddRange(addThese);
            return messages;
        }

        /// <summary>
        /// Checks all messages in the watch state and archives any missing messages
        /// </summary>
        /// <param name="serverID"></param>
        public async Task<bool> ArchiveMissingMessages(ulong serverID)
        {
            ulong WatchChannelID = _getWatchChannelID(serverID);
            DiscordChannel watchChannel = await Program.Client.GetChannelAsync(WatchChannelID);

            WatchRatingsEngineState state;
            if (!TryGetValue(serverID, out state))
            {
                return false;
            }

            List<WatchEntry> entries = state.GetEntries();
            DiscordMessage message;

            foreach (WatchEntry entry in entries)
            {
                message = await DiscordHandler.GetMessage(watchChannel, entry.MessageID);
                if(message == null)
                {
                    state.ArchiveEntry(entry.MessageID);
                }
            }

            return true;
        }
        #endregion

        #region Data Load

        private List<WatchEntry> _getArchivedEntries(ulong serverID)
        {
            WatchRatingsEngineState state;
            if(TryGetValue(serverID, out state))
            {
                return state.ArchivedEntries;
            }
            return new List<WatchEntry>();
        }

        /// <summary>
        /// Creates a dictionary maping user IDs to ratings
        /// </summary>
        /// <param name="message">Discord movie/tv message object</param>
        /// <returns></returns>
        private async Task<Dictionary<ulong, double>> _createRatingsMap(DiscordMessage message)
        {
            IReadOnlyList<DiscordUser> reactions;

            Dictionary<ulong, double> ratings = new Dictionary<ulong, double>();
            foreach (DiscordReaction reaction in message.Reactions)
            {
                DiscordEmoji key = reaction.Emoji;
                if (!ScoreMap.ContainsKey(key))
                {
                    continue; //If the reaction is not a valid rating reaction, continue
                }

                int rating;
                ScoreMap.TryGetValue(key, out rating);

                reactions = await message.GetReactionsAsync(key);
                foreach (DiscordUser user in reactions)
                {
                    ratings.Add(user.Id, rating);
                }
                Thread.Sleep(1000);
            }

            reactions = await message.GetReactionsAsync(HalfScore);
            foreach (DiscordUser user in reactions)
            {
                double rating;
                if (ratings.TryGetValue(user.Id, out rating))
                {
                    if(rating == 10)
                    {
                        continue;
                    }

                    ratings.Remove(user.Id);
                    ratings.Add(user.Id, rating + 0.5);
                }
                else
                {
                    ratings.Add(user.Id, 0.5);
                }
            }

            return ratings;
        }

        /// <summary>
        /// Validates a watch rating message
        /// </summary>
        /// <param name="message"></param>
        public static bool IsWatchRatingsChannelMessage(DiscordMessage message)
        {
            try
            {
                return _getWatchChannelID(message.Channel.Guild.Id) == (ulong)message.Channel.Id;
            }
            catch
            {
                return false;
            }
            
        }

        /// <summary>
        /// Gets the watch channel ID for a specific server
        /// </summary>
        /// <param name="serverID"></param>
        /// <returns></returns>
        private static ulong _getWatchChannelID(ulong serverID)
        {
            ServerConfig config = ServerConfig.GetServerConfig(serverID);
            return config.WatchRatingsChannelID;
        }

        /// <summary>
        /// Generates an watch entry object based on a discord message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<WatchEntry> GenerateEntry(DiscordMessage message)
        {
            Dictionary<ulong, double> ratings = await _createRatingsMap(message);
            List<string> keys;
            string name;
            int? year;
            bool isTV;
            GetInformation(message.Content, out keys, out name, out year, out isTV);

            WatchEntry entry = new WatchEntry()
            {
                MessageID = message.Id,
                ServerID = (ulong)message.Channel.GuildId,
                EntryTime = message.Timestamp,
                Ratings = ratings,
                Keys = keys,
                Name = name,
                Year = year,
                IsTV = isTV,
            };

            return entry;
        }

        /// <summary>
        /// Creates a new watch entry or updates an existing one given a discord message object. 
        /// The server and channel IDs are pulled from the object
        /// </summary>
        /// <param name="message"></param>
        public async Task<bool> UpdateWatchEntry(DiscordMessage message)
        {
            ValidateServerMessage(message); // Throws exception is message doesn't have a server ID
            if (!IsWatchRatingsChannelMessage(message))
            {
                return false;
            }

            bool result = await UpdateWatchEntry((ulong)message.Channel.GuildId, message.Id);
            Console.WriteLine($"Movie Entry {message.Id} updated.");
            return result;
        }

        /// <summary>
        /// Creates a new watch entry or updates an existing one given a discord message object. 
        /// The server and channel IDs are pulled from the object
        /// </summary>
        /// <param name="serverID"></param>
        /// <param name="messageID"></param>
        public async Task<bool> UpdateWatchEntry(ulong serverID, ulong messageID)
        {
            ulong watchChannelID = _getWatchChannelID(serverID);

            WatchRatingsEngineState state;
            TryGetValue(serverID, out state);

            // Pull discord message so we don't use cached data
            DiscordChannel channel = await Program.Client.GetChannelAsync(watchChannelID);
            DiscordMessage message = await DiscordHandler.GetMessage(channel, messageID);

            if(message == null)
            {
                Console.WriteLine($"Archiving [{messageID}]");
                state.ArchiveEntry(messageID);
                return true;
            }
            
            Console.WriteLine($"Updating {message.Content} [{message.Id}]");

            WatchEntry updatedEntry = await GenerateEntry(message);

            state.UpdateEntry(updatedEntry);
            return true;
        }

        /// <summary>
        /// Updates Watch Entries based on a list of message IDs
        /// </summary>
        /// <param name="messages"></param>
        /// <param name="serverID"></param>
        public async Task<bool> UpdateWatchEntries(List<ulong> messages, ulong serverID)
        {
            foreach(ulong messageID in messages)
            {
                await UpdateWatchEntry(serverID, messageID);
                Thread.Sleep(1000);
            }
            return true;
        }

        /// <summary>
        /// Checks if a discord message is a valid watch ratings message
        /// </summary>
        /// <returns></returns>
        public bool IsValidWatchRatingsMessage(DiscordMessage message)
        {
            string content = message.Content;
            return IsValidMovieName(content) || IsValidTVName(content);
        }

        /// <summary>
        /// Checks if a message content is valid
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsValidMovieName(string name)
        {
            return GetMovieYear(name) != null;
        }

        /// <summary>
        /// Checks if a name is a valid tv name based on the ending
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsValidTVName(string name)
        {
            return GetTVYear(name) != null || name.EndsWith("(TV)");
        }

        /// <summary>
        /// With a title of the form "name (year)", gets the year
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int? GetTVYear(string name)
        {
            if (name == "")
            {
                return null;
            }

            string[] tvSplit = name.Split(new string[] { "(TV " }, StringSplitOptions.None);
            if (tvSplit.Length == 1)
            {
                return null;
            }

            string _year = tvSplit[tvSplit.Length - 1];
            if (_year[_year.Length - 1] == ')' && _year.Length == 5)
            {
                return Int32.Parse(_year.Substring(0, _year.Length - 1));
            }
            return null;
        }

        /// <summary>
        /// Gets the year from a string of the form "moviename (xxxx)"
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int? GetMovieYear(string name)
        {
            if (name == "")
            {
                return null;
            }
            if (name.Split('(').Length == 0)
            {
                return null;
            }

            string _year = name.Split('(')[name.Split('(').Length - 1];
            if (_year[_year.Length - 1] == ')' && _year.Length == 5)
            {
                return Int32.Parse(_year.Substring(_year.Length - 5, _year.Length - 1));
            }
            return null;
        }

        /// <summary>
        /// Gets the keys associated with a movie/tv string
        /// </summary>
        /// <param name="messageContent">movie/tv string</param>
        /// <returns></returns>
        public List<string> GetKeys(string messageContent)
        {
            if (string.IsNullOrEmpty(messageContent))
            {
                return null;
            }

            List<string> keys = new List<string>();

            if (messageContent[0] == '[')
            {
                string x = messageContent.Split(']')[0];
                string[] split = x.Split('[');
                if (x.Length < 2)
                {
                    return keys;
                }
                x = split[1];

                keys = x.Split(',').ToList();

            }
            return keys;

        }

        /// <summary>
        /// Gets the movie/tv name from a movie string
        /// </summary>
        /// <param name="messageContent"></param>
        /// <returns></returns>
        public string GetName(string messageContent)
        {
            string name = messageContent;
            if (messageContent.Contains(']'))
            {
                name = messageContent.Split(']')[1];
            }
            if(GetTVYear(messageContent) != null || GetMovieYear(messageContent) != null)
            {
                name = name.Split('(')[name.Split('(').Length - 2];
            }

            return name.Trim();
        }

        /// <summary>
        /// Populates values given a movie/tv string
        /// </summary>
        /// <param name="messageContent">movie/tv string</param>
        /// <param name="Keys">keys associated with the string</param>
        /// <param name="name">name of the movie or tv show</param>
        /// <param name="year">year of the movie or tv show</param>
        /// <param name="IsTV">true if a tv show, false otherwise</param>
        public void GetInformation(string messageContent, out List<string> Keys, out string name, out int? year, out bool IsTV)
        {
            Keys = GetKeys(messageContent);
            IsTV = false;
            if (IsValidTVName(messageContent))
            {
                IsTV = true;
                year = GetTVYear(messageContent);
            }
            else
            {
                year = GetMovieYear(messageContent);
            }
            name = GetName(messageContent);
        }
        #endregion

        #region Reports
        /// <summary>
        /// Searches for a 
        /// </summary>
        /// <param name="search"></param>
        /// <param name="serverID"></param>
        /// <returns></returns>
        public string Search(WatchSearch search, ulong serverID)
        {
            WatchRatingsEngineState state;
            TryGetValue(serverID, out state);

            List<WatchEntry> results = state.Search(search);

            string res = "";
            foreach(WatchEntry entry in results)
            {
                res = $"{res}{entry}\n";
            }

            if(res == "")
            {
                res = "No results";
            }

            return res;
        }

        /// <summary>
        /// Gets overall statistics in the server
        /// </summary>
        /// <param name="serverID">server ID</param>
        /// <param name="userID">Optional paramter for user id. Pull statistics for user if non-null. Otherwise, pulls stats for the entire server</param>
        /// <returns></returns>
        public string ServerStatistics(ulong serverID, ulong? userID = null, bool? isTV = null)
        {
            WatchRatingsEngineState state;
            TryGetValue(serverID, out state);
            List<int> allYears = state.GetAllYears();
            List<double> allScores = new List<double>();
            Statistics stats = new Statistics(allScores);
            double min = 0;
            double max = 0;
           
            WatchSearch search = new WatchSearch()
            {
                IsTV = isTV
            };
            if(userID != null)
            {
                search.AddUser((ulong)userID);
            }


            List<WatchEntry> entries = state.Search(search);

            string results = "";

            /// Function on how to pull values from a WatchEntry list. Used in the select statement when converting a list 
            /// of watch entries to ratings (double)
            Func<WatchEntry, double> dataFromWatchEntry;
            if (userID != null)
            {
                dataFromWatchEntry = x => { return x.GetUserRating((ulong)userID); };
            }
            else
            {
                dataFromWatchEntry = x => { return x.GetAverage(); };
            }

            /// Function to get statistics
            Func<List<WatchEntry>, Statistics> getStats = entryInput =>
            {
                Statistics retStats;
                if (userID != null)
                {
                    allScores = entryInput.Select(x => dataFromWatchEntry(x)).ToList();
                    retStats = new Statistics(allScores);
                }
                else
                {
                    retStats = new Statistics(WatchRatingsEngineState.GetAllScores(entryInput));
                    List<WatchEntry> scoredEntries = entryInput.Where(x => x.HasRatings()).ToList();
                    retStats.SetMin(scoredEntries.Select(x => dataFromWatchEntry(x)).Min());
                    retStats.SetMax(scoredEntries.Select(x => dataFromWatchEntry(x)).Max());
                }

                return retStats;
            };

            stats = getStats(entries);

            results = $"**All Time Results:**\n" +
                $"Total Watched: {entries.Count}\n{stats.BasicString()}" +
                $"{_generateMinMaxSectionStats(entries, dataFromWatchEntry, stats)}\n";

            List<int> years = state.GetAllYears().OrderByDescending(x => x).ToList();
            foreach (int year in years)
            {
                search.WatchYear = year;
                List<WatchEntry> filteredEntries = state.Search(search);
                if(filteredEntries.Count == 0)
                {
                    continue;
                }

                stats = getStats(filteredEntries);

                results = $"{results}**{year} Results:**\n" +
                    $"Total New Watched: {filteredEntries.Count}\n{stats.BasicString()}\n" +
                    $"{_generateMinMaxSectionStats(filteredEntries, dataFromWatchEntry, stats)}\n";   
            }

            return results;
        }

        /// <summary>
        /// Generate stats for a list of entries
        /// </summary>
        /// <param name="entries">List of entries</param>
        /// <param name="DataFromWatchEntry">function with WatchEntry input and double output. Returns the data
        ///                                  from WatchEntry that statistics should be pulled from</param>
        /// <returns></returns>
        private string _generateMinMaxSectionStats(List<WatchEntry> entries, Func<WatchEntry,double> DataFromWatchEntry, Statistics stats)
        {
            string results = "";
            if (entries.Count > 0)
            {
                Func<List<WatchEntry>, double, string> generateSection = (entry, searchVal) =>
                {
                    string list = "";
                    List<WatchEntry> filteredEntries = entries.Where(x => DataFromWatchEntry(x) == searchVal).ToList();
                    foreach (WatchEntry e in filteredEntries)
                    {
                        list = $"{list}{e.TitleString()}\n";
                    }
                    return list;
                };


                results = $"{results}\nMaximum Score: {stats.Maximum}\n{generateSection(entries, stats.Maximum)}";
                if(stats.Maximum == stats.Minimum)
                {
                    return results;
                }
                results = $"{results}\nMinimum Score: {stats.Minimum}\n{generateSection(entries, stats.Minimum)}";
            }
            return results;
        }
        #endregion
    }

    public class WatchRatingsEngineState : ServerEngineState
    {
        public Dictionary<ulong, WatchEntry> WatchEntries = new Dictionary<ulong, WatchEntry>();

        public List<WatchEntry> ArchivedEntries = new List<WatchEntry>();

        public Dictionary<ulong, WatchEntry> MergedEntries = new Dictionary<ulong, WatchEntry>();

        public bool HasValidationWarnings;

        [JsonIgnore]
        public override string StateFile_ { get; } = "WatchRatings.JSON";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serverID"></param>
        public WatchRatingsEngineState(ulong serverID) : base(serverID)
        {
            HasValidationWarnings = false;
        }

        /// <summary>
        /// Updates an entry in the watch entries state
        /// </summary>
        /// <param name="entry"></param>
        public void UpdateEntry(WatchEntry entry)
        {
            entry.ServerID = ServerID; // Ensure that the server ID is correct

            if (!WatchEntries.ContainsKey(entry.MessageID))
            {
                WatchEntries.Add(entry.MessageID, entry);
            }
            else
            {
                WatchEntry oldEntry;
                WatchEntries.TryGetValue(entry.MessageID, out oldEntry);
                WatchEntries.Remove(oldEntry.MessageID);
                WatchEntries.Add(entry.MessageID, entry);
            }

            if (MergedEntries.Count == 0 || WatchEntries.Values.Select(x => x.Name == entry.Name).Count() > 1)
            {
                GenerateMergedStates();
            }
            SaveState();
        }

        /// <summary>
        /// Archives a watch entry. Moves an entry in the WatchEntries to the ArcivedEntried. 
        /// </summary>
        /// <param name="id"></param>
        public void ArchiveEntry(ulong id)
        {
            if (!WatchEntries.ContainsKey(id))
            {
                throw new InvalidOperationException($"Attempted to archive a message with id {id}, but it does not exist.");
            }
            else
            {
                WatchEntry oldEntry;
                WatchEntries.TryGetValue(id, out oldEntry);
                WatchEntries.Remove(oldEntry.MessageID);

                // Only add ratings with scores added.
                if (!oldEntry.HasRatings())
                {
                    return;
                }
                oldEntry.Keys.Add("Archived");
                Console.WriteLine($"Archiving {oldEntry.Name} [{oldEntry.MessageID}]");
                ArchivedEntries.Add(oldEntry);
                SaveState();
            }
        }

        /// <summary>
        /// Checks if a message ID is in the WatchEntries
        /// </summary>
        /// <param name="id"></param>
        public bool HasEntry(ulong id)
        {
            return WatchEntries.ContainsKey(id);
        }

        /// <summary>
        /// Generates the Merged Entries Dictionary
        /// </summary>
        public void GenerateMergedStates()
        {
            MergedEntries = new Dictionary<ulong, WatchEntry>();
            List<WatchEntry> entries = WatchEntries.Values.ToList().OrderBy(x => x.Name, StringComparer.InvariantCultureIgnoreCase).ToList();
            entries.AddRange(ArchivedEntries); // Archived entries should be considered in the merged states

            for (int i = 0; i < entries.Count; i++)
            {
                WatchEntry baseEntry = entries[i];
                List<WatchEntry> mergeThese = new List<WatchEntry>();
                for (int j = i + 1; j < entries.Count; j++)
                {
                    WatchEntry checkMerge = entries[j];
                    if (!WatchEntry.ShouldMerge(baseEntry, checkMerge))
                    {
                        i = j - 1;
                        break;
                    }
                    mergeThese.Add(checkMerge);
                }

                if (mergeThese.Count == 0)
                {
                    MergedEntries.Add(baseEntry.MessageID, baseEntry);
                }
                else
                {
                    mergeThese.Add(baseEntry);
                    WatchEntry mergedEntry = WatchEntry.Merge(mergeThese);
                    MergedEntries.Add(mergedEntry.MessageID, mergedEntry);
                }
            }
        }

        /// <summary>
        /// Gets the oldest entry in the watch list
        /// </summary>
        /// <returns></returns>
        public ulong? GetOldestEntry()
        {
            var entries = WatchEntries.Values.OrderByDescending(x => x.EntryTime).ToList();
            if (entries.Count == 0)
            {
                return 0;
            }
            return entries.Last().MessageID;
        }

        /// <summary>
        /// Gets the newest entry in the watch list
        /// </summary>
        /// <returns></returns>
        public ulong? GetNewestEntry()
        {
            var entries = WatchEntries.Values.OrderBy(x => x.EntryTime).ToList();
            if(entries.Count == 0)
            {
                return 0;
            }
            return entries.Last().MessageID;
        }

        /// <summary>
        /// Gets a list of all entries
        /// </summary>
        /// <returns></returns>
        public List<WatchEntry> GetEntries()
        {
            return MergedEntries.Values.ToList();
        }

        /// <summary>
        /// Given a list of watch entries, generates a list of all the user scores
        /// </summary>
        /// <returns></returns>
        public static List<double> GetAllScores(List<WatchEntry> entries)
        {
            List<double> scores = new List<double>();

            foreach(WatchEntry entry in entries)
            {
                scores.AddRange(entry.GetRatings());
            }

            return scores;
        }

        /// <summary>
        /// Searches for watch entries using the criteria in a watch search object
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        public List<WatchEntry> Search(WatchSearch searchParams)
        {
            return Search(searchParams.SearchFunction());
        }

        /// <summary>
        /// Searches for watch entries where a specific criteria is true
        /// </summary>
        /// <param name="func">delegate function where the input parameter is a watch entry and the output is a boolean if the watch entry should be included</param>
        /// <returns></returns>
        public List<WatchEntry> Search(Func<WatchEntry, bool> func)
        {
            return GetEntries().Where(x => func(x)).ToList();
        }

        /// <summary>
        /// Gets a list of all the years where a movie was watched
        /// </summary>
        /// <returns></returns>
        public List<int> GetAllYears()
        {
            return MergedEntries.Values.ToList().Select(x => x.EntryTime.Year).Distinct().OrderBy(x => x).ToList();
        }

    }
}
