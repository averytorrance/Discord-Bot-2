using DSharpPlus.SlashCommands;
using System;

namespace DiscordBot.Classes
{
    public class Time
    {

        /// <summary>
        /// Converts a datetime to a specific timezone
        /// </summary>
        /// <param name="time"></param>
        /// <param name="zone"></param>
        /// <returns></returns>
        public static DateTime ConvertTime(DateTime time, TimeZones zone)
        {
            return TimeZoneInfo.ConvertTime(time, TimeZone(zone));
        }

        /// <summary>
        /// Gets a timezone info object based on the TimeZones enum
        /// </summary>
        /// <param name="zone"></param>
        /// <returns></returns>
        public static TimeZoneInfo TimeZone(TimeZones zone)
        {
            return TimeZoneInfo.FindSystemTimeZoneById(zone.GetName());
        }

        /// <summary>
        /// Converts a datetime to another timezone
        /// </summary>
        /// <param name="time">datetime to convert</param>
        /// <param name="source">current timezone for the datetime object</param>
        /// <param name="destination">timezone to convert to</param>
        /// <returns></returns>
        public static DateTime ConvertTime(DateTime time, TimeZones source, TimeZones destination)
        {
            TimeZoneInfo sourceZone = TimeZone(source);
            TimeZoneInfo destinationZone = TimeZone(destination);

            return TimeZoneInfo.ConvertTime(time, sourceZone, destinationZone);

        }


    }

    public enum TimeZones
    {
        [ChoiceName("Central Standard Time")]
        CST,
        [ChoiceName("Eastern Standard Time")]
        EST,
        [ChoiceName("Mountain Standard Time")]
        MST,
        [ChoiceName("Pacific Standard Time")]
        PST,
        [ChoiceName("UTC")]
        UTC,
    }
}
