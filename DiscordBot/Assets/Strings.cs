namespace DiscordBot.Assets
{
    public static class Strings
    {
        /// <summary>
        /// Generates the string for a discord message
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static string Mention(ulong userID)
        {
            return $"<@{userID}>";
        }
    }
}
