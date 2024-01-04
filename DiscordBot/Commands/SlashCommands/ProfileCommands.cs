using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System.Threading.Tasks;
using DiscordBot.UserProfile;
using DiscordBot.Engines;
using DiscordBot.Classes;

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
            DiscordUserEngine discordUserEngine = new DiscordUserEngine();
            DiscordServerEngine serverEngine = new DiscordServerEngine(ctx.Channel.Guild);

            if(member == null)
            {
                member = ctx.Member;
            }

            if(!discordUserEngine.UserExists(member.Id))
            {
                discordUserEngine.CreateUser(member);
            }

            ServerUser user = serverEngine.GetServerUser(member.Id);
            if(user == null)
            {
                serverEngine.CreateUser(member.Id);
                user = serverEngine.GetServerUser(member.Id);
            }

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(user.GenerateProfileMessage()));
        }

        /// <summary>
        /// Displays the profile for the user. Creates a profile for a user if it does not exist. 
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [SlashCommand("SetMyTimeZone", "Sets timezone on your user profile")]
        public async Task SetMyTimeZone(InteractionContext ctx, [Option("Timezone", "The user to view")] TimeZones timezone)
        {
            DiscordUserEngine engine = new DiscordUserEngine();
            DiscordUser member = ctx.Member;

            DUser user = engine.GetUser(member);

            user._timezone = timezone;

            engine.UpdateUser(user);

            DiscordServerEngine serverEngine = new DiscordServerEngine(ctx.Guild);
            ServerUser serverUser = serverEngine.GetServerUser(member.Id);

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(serverUser.GenerateProfileMessage()));
        }
    }
}
