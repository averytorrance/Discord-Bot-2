using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;
using DiscordBot.Engines;


namespace DiscordBot.Commands.AdminCommands
{
    class MiscAdminCommands : BaseCommandModule
    {
        /// <summary>
        /// Selects a random item from a list of input items
        /// </summary>
        /// <param name="ctx">command context</param>
        /// <returns></returns>
        [Command("backupemojis")]
        [RequireUserPermissions(DSharpPlus.Permissions.Administrator)]
        public async Task BackupEmojis(CommandContext ctx)
        {
            DiscordServerEngine engine = new DiscordServerEngine(ctx.Guild);
            await engine.BackupEmojis();
        }
    }
}
