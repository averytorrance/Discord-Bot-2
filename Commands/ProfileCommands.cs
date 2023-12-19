using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;
using DiscordBot.UserProfile;
using DiscordBot.Assets;

namespace DiscordBot.Commands
{
    class ProfileCommands : BaseCommandModule
    {
        /// <summary>
        /// Displays the profile for the user. Creates a profile for a user if it does not exist. =
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [Command("profile")]
        public async Task Profile(CommandContext ctx)
        {
            try
            {
                LocalUserEngine userEngine = new LocalUserEngine();
                ulong userID = ctx.User.Id;
                ulong serverID = ctx.Guild.Id;

                LocalUser user = new LocalUser(ctx.Member);

                user = userEngine.GetCreateUser(user);

                await ctx.Channel.SendMessageAsync(user.GenerateProfileMessage());
            }
            catch
            {
                await ctx.Channel.SendMessageAsync(DiscordMessageAssets.GenerateErrorMessage());
            }
        }


    }
}
