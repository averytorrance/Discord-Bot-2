using System;

namespace DiscordBot.Models
{
	public class Reminder
	{
		public int ID { get; set; }
		public string Message { get; set; }
		public ulong OwnerId { get; set; }
		public bool Sent { get; set; } = false;
		public DateTime SendTime { get; set; }
		public Freq Frequency { get; set; } = Freq.None;
		public string DayOfWeek { get; set; }
		public bool SoftDeleted { get; set; }

		public Reminder(int id, string message, ulong ownerId, DateTime sendTime, Freq frequency = Freq.None, string dayOfWeek = null, bool softDeleted = false)
		{
			this.ID = id;
			this.Message = message;
			this.OwnerId = ownerId;
			this.SendTime = sendTime;
			this.Frequency = frequency;
			this.DayOfWeek = dayOfWeek;
			this.SoftDeleted = softDeleted;
		}

		public bool IsRecurring()
        {
			return Frequency != Freq.None;
        }

		public bool IsStale()
        {
			return false ;
        }

		public bool CanSend()
        {
			return true;
        }

		public bool ReadyToSend()
        {
			return true;
        }

		public string ReminderMessage()
        {
			return "";
        }

		public bool Send()
        {
			this.SendTime = DateTime.Now;
			this.Sent = true;
			return true;
        }

		public int Compare(Reminder reminder)
        {
			// Scenarios where the send instant are different
			if(SendTime < reminder.SendTime)
            {
				return 1;
            }
			else if( reminder.SendTime > SendTime)
            {
				return -1;
            }

			// Resort to sorting with IDs if the sent instants are the same
            if (Equals(reminder))
            {
				return 0;
            }
			else if( reminder.ID < ID)
            {
				return 1;
            }
			return -1;

        }

		public bool Equals(Reminder reminder)
        {
			return ID == reminder.ID;
        }


		public enum Freq
        {
			None,
        }

	}
}
