namespace DiscordBot.Assets
{
    public static class Strings
    {
        /// <summary>
        /// Generates the string for a discord mention
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static string Mention(ulong userID)
        {
            return $"<@{userID}>";
        }
    }
}
