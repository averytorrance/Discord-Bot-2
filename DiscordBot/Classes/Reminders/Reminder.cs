using DiscordBot.Assets;
using DiscordBot.Engines;
using DiscordBot.UserProfile;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;

namespace DiscordBot.Classes
{
	public class Reminder
	{
		public ulong ID { get; set; }
		public string Message { get; set; }
		public ulong OwnerId { get; set; }
		public bool Sent { get; set; } = false;
		public DateTime SendTime { get; set; }
		public Freq Frequency { get; set; } = Freq.None;
		public int FrequencyFactor { get; set; } = 1;
		public List<ulong> Subscribers { get; set; } = new List<ulong>();
		public DateTime? LastSent { get; set; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="id"></param>
		/// <param name="message"></param>
		/// <param name="ownerId"></param>
		/// <param name="sendTime">send time in UTC</param>
		/// <param name="frequency"></param>
		/// <param name="frequencyFactor"></param>
		public Reminder(ulong id, string message, ulong ownerId, DateTime sendTime, Freq frequency = Freq.None, int frequencyFactor = 1)
		{
			if (sendTime.Kind != DateTimeKind.Utc)
			{
				throw new Exception("Attempted to create a reminder with a non UTC DateKind");
			}

			this.ID = id;
			this.Message = message;
			this.OwnerId = ownerId;
			this.Frequency = frequency;
			this.FrequencyFactor = frequencyFactor;
			this.SendTime = sendTime;
		}

		/// <summary>
		/// ToString representation. Uses the UTC time. 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
        {
			string basic = $"ID: {ID}\n" +
				$"Scheduled: {SendTime}\n";
            if (IsRecurring())
            {
				string freq = Frequency.GetName();
				if (FrequencyFactor != 1)
                {
					freq = freq + "s";
				}

				basic = $"{basic}Occurs every {FrequencyFactor} {freq}\n";
            }

			return $"{basic}" +
				$"Message: {Message}\n";
		}

		/// <summary>
		/// Metadata string for the reminder
		/// </summary>
		/// <returns></returns>
		public string ReminderInformation()
        {
			return $"ID: {ID}\n";
        }

		/// <summary>
		/// ToString representation, with added information on if the user is subscribed to the reminder
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public string ToString(ulong id)
        {
			string basic = $"ID: {ID}\n" +
							$"Scheduled: {GetSendTimeUserTimeZone(id)}\n" +
                            $"Message: {Message}\n";

			if (IsRecurring())
			{
				string freq = Frequency.GetName();
				if (FrequencyFactor != 1)
				{
					freq = freq + "s";
				}

				basic = $"{basic}Occurs every {FrequencyFactor} {freq}\n";
			}

			string ownerInfo = "";
			if(IsUserSubscribed(id) && OwnerId != id)
            {
				ownerInfo = "\nYou are subscribed to this reminder";
            }

			return $"{basic}{ownerInfo}";
        }

		/// <summary>
		/// Gets the send time of this reminder in the input user's timezone
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public DateTime GetSendTimeUserTimeZone(ulong id)
        {
			DiscordUserEngine discordUserEngine = new DiscordUserEngine();
			TimeZoneInfo localTimeZone = discordUserEngine.GetUser(id).TimeZone();
			TimeZoneInfo timeZoneUTC = TimeZoneInfo.FindSystemTimeZoneById(TimeZones.UTC.GetName());

			return TimeZoneInfo.ConvertTime(SendTime, timeZoneUTC, localTimeZone);
		}

		/// <summary>
		/// Checks if a reminder should occur before or after this one.
		/// </summary>
		/// <param name="reminder"></param>
		/// <returns></returns>
		public int Compare(Reminder reminder)
		{
			// Scenarios where the send instant are different
			if (SendTime < reminder.SendTime)
			{
				return 1;
			}
			else if (reminder.SendTime > SendTime)
			{
				return -1;
			}

			// Resort to sorting with IDs if the sent instants are the same
			if (Equals(reminder))
			{
				return 0;
			}
			else if (reminder.ID < ID)
			{
				return 1;
			}
			return -1;

		}

		/// <summary>
		/// Checks if Reminders are equivalent
		/// </summary>
		/// <param name="reminder"></param>
		/// <returns></returns>
		public bool Equals(Reminder reminder)
		{
			return ID == reminder.ID;
		}

		/// <summary>
		/// Checks if a reminder is recurring
		/// </summary>
		/// <returns></returns>
		public bool IsRecurring()
        {
			return Frequency != Freq.None;
        }

		/// <summary>
		/// Updates a send time for recurring reminders
		/// </summary>
		public void UpdateSendTime()
        {
            switch (Frequency)
            {
				case Freq.None: return;
				case Freq.Hour: SendTime = SendTime.AddHours(FrequencyFactor); break;
				case Freq.Day: SendTime = SendTime.AddDays(FrequencyFactor); break;
				case Freq.Week: SendTime = SendTime.AddDays(7* FrequencyFactor); break;
				case Freq.Month: SendTime = SendTime.AddMonths(FrequencyFactor); break;
				case Freq.Year: SendTime = SendTime.AddYears(FrequencyFactor); break;
			}

			//If a reminder is stale, update it until it is no longer stale
            if (IsStale())
            {
				UpdateSendTime();
            }

        }

		/// <summary>
		/// Checks if a reminder is stale. This means that the current time is past the 
		/// scheduled time plus 1 minute for the reminder.
		/// </summary>
		/// <returns></returns>
		public bool IsStale()
        {
			return SendTime.AddMinutes(3) < DateTime.UtcNow;
        }

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public bool ReadyToSend()
        {
			return SendTime <= DateTime.UtcNow;
        }

		/// <summary>
		/// Subscribes a user to a reminder
		/// </summary>
		/// <param name="userID"></param>
		/// <returns></returns>
		public bool Subscribe(ulong userID)
        {
            if (IsUserSubscribed(userID))
            {
				return false;
            }

			Subscribers.Add(userID);
			return true;
        }

		/// <summary>
		/// Subscribes a user to a reminder
		/// </summary>
		/// <param name="userID"></param>
		/// <returns></returns>
		public bool UnSubscribe(ulong userID)
        {
			if (IsUserSubscribed(userID))
			{
				Subscribers.Remove(userID);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Checks if a user is subscribed
		/// </summary>
		/// <param name="userID"></param>
		/// <returns></returns>
		public bool IsUserSubscribed(ulong userID)
        {
			return Subscribers.Contains(userID);
        }

		/// <summary>
		/// Generates a Discord Message Builder object for the reminder
		/// </summary>
		/// <returns></returns>
		public DiscordMessageBuilder ReminderMessage()
        {
			List<IMention> users = new List<IMention>() { new UserMention(OwnerId) };
			
			string subscriberMentions = "";
			
			if(Subscribers.Count > 0)
            {
				subscriberMentions = "\n\nYou are being pinged since you subscribed to this reminder:\n";
				foreach(ulong userID in Subscribers)
                {
					subscriberMentions += Strings.Mention(userID); ;
					users.Add(new UserMention(userID));
                }
			}

			string content = $"{Strings.Mention(OwnerId)}: {Message}{subscriberMentions}";
            if (IsStale())
            {
				content = $"Stale Reminder. This should've been sent at {SendTime} UTC.\n\n{content}";
            }

			DiscordMessageBuilder builder = new DiscordMessageBuilder()
				.WithAllowedMentions((IEnumerable<IMention>)users)
				.WithContent(content);

			return builder;
        }
	}

	/// <summary>
	/// Enum for frequencies
	/// </summary>
	public enum Freq
	{
		[ChoiceName("None")]
		None,
		[ChoiceName("Hour")]
		Hour,
		[ChoiceName("Day")]
		Day,
		[ChoiceName("Week")]
		Week,
		[ChoiceName("Month")]
		Month,
		[ChoiceName("Year")]
		Year
	}
}
