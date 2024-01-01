using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Classes
{
    public class DiscordHandler
    {
        private ulong _serverID;
        private ulong _channelID;

        public DiscordGuild Server { get; private set; }

        public DiscordChannel Channel { get; private set; }

        public bool? ServerIsDeleted { get; private set; }

        public bool? ChannelIsDeleted { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serverID"></param>
        /// <param name="channelID"></param>
        public DiscordHandler(ulong serverID, ulong channelID)
        {
            _serverID = serverID;
            _channelID = channelID;
        }

        /// <summary>
        /// Populates Server and Channel properties
        /// </summary>
        /// <returns></returns>
        public async Task Setup()
        {
            try
            {
                Server = await Program.Client.GetGuildAsync(_serverID);
                ServerIsDeleted = false;
            }
            catch (NotFoundException e)
            {
                ServerIsDeleted = true;
            }

            try 
            {
                Channel = await Program.Client.GetChannelAsync(_channelID);
                ChannelIsDeleted = false;
            }
            catch (NotFoundException)
            {
                ChannelIsDeleted = true;
            }
        }

        /// <summary>
        /// Gets a discord message. Has handling for if the message can't be found
        /// </summary>
        /// <param name="messageID"></param>
        /// <returns></returns>
        public async Task<DiscordMessage> GetMessage(ulong messageID)
        {
            return await GetMessage(Channel, messageID);
        }

        /// <summary>
        /// Gets a discord message. Has handling for if the message can't be found
        /// </summary>
        /// <param name="messageID"></param>
        /// <returns></returns>
        public static async Task<DiscordMessage> GetMessage(DiscordChannel channel, ulong messageID)
        {
            try
            {
                return await channel.GetMessageAsync(messageID);
                
            }
            catch (NotFoundException e)
            {
                return null;
            }
        }
    }
}
