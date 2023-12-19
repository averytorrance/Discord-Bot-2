using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;

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
                await WebEngine.DownloadImage(emoji.Url, _backupDirectory("Emoji Backup"), EmojiFileName(emoji));
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
        /// Directory to backup files
        /// </summary>
        /// <param name="name">optional subdirectory</param>
        /// <returns></returns>
        private string _backupDirectory(string name = "")
        {
            string directory = $@"H:\Users\avery\Desktop\Discord Bot\discord-bot-2\{ID}\{DateTime.Now.ToString("yyyy MM dd")}";
            if (!string.IsNullOrEmpty(name))
            {
                directory = $@"{directory}\{name}";
            }
            return directory;
        }
    }
}
