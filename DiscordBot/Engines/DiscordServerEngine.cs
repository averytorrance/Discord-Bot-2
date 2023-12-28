using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DiscordBot.Config;

namespace DiscordBot.Engines
{
    class DiscordServerEngine
    {
        /// <summary>
        /// Discord Server Object
        /// </summary>
        public DiscordGuild Server { get; private set; }

        /// <summary>
        /// Discord Server ID
        /// </summary>
        public ulong ID { get; private set; }

        public DiscordServerEngine(DiscordGuild server)
        {
            Server = server;
            ID = server.Id;
        }

        /// <summary>
        /// Backups 
        /// </summary>
        /// <returns></returns>
        public async Task BackupEmojis()
        {
            WebEngine.Start();
            foreach (DiscordEmoji emoji in Server.Emojis.Values)
            {
                if (emoji.IsAnimated)
                {

                }
                await WebEngine.DownloadImage(emoji.Url, BackupDirectory("Emojis"), EmojiFileName(emoji));
            }
        }

        /// <summary>
        /// Gets the filename for a specific emoji
        /// </summary>
        /// <param name="emoji">Discord Emoji object</param>
        /// <returns></returns>
        public string EmojiFileName(DiscordEmoji emoji)
        {
            string filename = $"{emoji.Name}";
            string extension = "png";

            if (emoji.IsAnimated)
            {
                extension = "gif";
            }

            return $"{filename}.{extension}";
        }

        /// <summary>
        /// Backup directory
        /// </summary>
        /// <param name="backupType">subdirectory folder name in the backup folder</param>
        /// <returns></returns>
        public string BackupDirectory(string backupType)
        {
            return $"{ServerConfig.BackupDirectoryToday(ID)}{backupType}";
        }

    }
}
