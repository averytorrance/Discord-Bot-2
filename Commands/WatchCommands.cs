using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DiscordBot.Engines;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using DiscordBot.Config;
using System.Threading.Tasks;
using System;
using DSharpPlus.Entities;

namespace DiscordBot.Commands
{
    public class WatchCommands : BaseCommandModule
    {

        private string getFile(ulong serverID)
        {
            string directory = $"{ServerConfig.ServerDirectory(serverID)}Test.txt";
            if (!File.Exists(directory))
            {
                File.Create(directory);
            }

            return directory;
        }

        /// <summary>
        /// Selects a random item from a list of input items
        /// </summary>
        /// <param name="ctx">command context</param>
        /// <param name="names">input parameters. handling added to use ',' as the delimeter for each item.</param>
        /// <returns></returns>
        [Command("LoadData")]
        public async Task LoadData(CommandContext ctx)
        {
            ulong serverID = ctx.Guild.Id;
            string test = File.ReadAllText(getFile(serverID));
            List<ulong> messages = test.Split('\n').ToList().Select(x => ulong.Parse(x)).ToList();

            await WatchRatingsEngine.CurrentEngine.UpdateWatchEntries(messages, serverID);

            Console.WriteLine("Data Load Complete");
        }

        /// <summary>
        /// Selects a random item from a list of input items
        /// </summary>
        /// <param name="ctx">command context</param>
        /// <param name="names">input parameters. handling added to use ',' as the delimeter for each item.</param>
        /// <returns></returns>
        [Command("GetIDS")]
        public async Task GetIDS(CommandContext ctx)
        {
            ulong serverID = ctx.Guild.Id;
            ServerConfig config = ServerConfig.GetServerConfig(serverID);

            DiscordChannel channel = await Program.Client.GetChannelAsync(config.WatchRatingsChannelID);

            try
            {
                var messages = (await channel.GetMessagesAsync()).ToList();

                if (serverID == 427296058310393856)
                {
                    messages.AddRange((await channel.GetMessagesAfterAsync(655584795199275027)).ToList());
                    messages.AddRange((await channel.GetMessagesAfterAsync(655584795199275027)).ToList());
                    messages.AddRange((await channel.GetMessagesAfterAsync(791162629346295848)).ToList());
                    messages.AddRange((await channel.GetMessagesAfterAsync(910010299665514507)).ToList());
                    messages.AddRange((await channel.GetMessagesAfterAsync(1061559232501133392)).ToList());
                }

                messages = messages.Distinct().OrderBy(x => x.Id).ToList();

                List<string> ids = new List<string>();
                foreach (DiscordMessage message in messages)
                {

                    ids.Add(message.Id.ToString());
                }

                string content = string.Join("\n", ids);

                File.WriteAllText(getFile(serverID), content);

                Console.WriteLine("Data Pull Complete");
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }

    }
}
