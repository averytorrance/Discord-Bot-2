using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using System.Threading.Tasks;
using DiscordBot.UserProfile;

namespace DiscordBot.Commands.SlashCommands
{
    class ProfileCommands : ApplicationCommandModule
    {
        /// <summary>
        /// Displays the profile for the user. Creates a profile for a user if it does not exist. 
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [SlashCommand("Profile", "Views a user's profile")]
        public async Task Profile(InteractionContext ctx, [Option("User", "The user to view")] DiscordUser member = null )
        {
            LocalUserEngine userEngine = new LocalUserEngine();
            if(member == null)
            {
                member = ctx.Member;
            }
            
            LocalUser user = new LocalUser((DiscordMember)member);

            user = userEngine.GetCreateUser(user);

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(user.GenerateProfileMessage()));
        }


    }
}
