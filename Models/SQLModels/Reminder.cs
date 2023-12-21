using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Models
{
	public class Reminder
	{
		public int ReminderID { get; set; }
		public string Message { get; set; }
		public string OwnerId { get; set; }
		public bool Sent { get; set; }
		public DateTime SendInstant { get; set; }
		public bool Recurring { get; set; }
		public string Frequency { get; set; }
		public string DayOfWeek { get; set; }
		public bool SoftDeleted { get; set; }

		public Reminder(int ReminderID_, string Message_, string OwnerId_, bool Sent_, DateTime SendInstant_, bool Recurring_, string Frequency_, string DayOfWeek_, bool SoftDeleted_)
		{
			this.ReminderID = ReminderID_;
			this.Message = Message_;
			this.OwnerId = OwnerId_;
			this.Sent = Sent_;
			this.SendInstant = SendInstant_;
			this.Recurring = Recurring_;
			this.Frequency = Frequency_;
			this.DayOfWeek = DayOfWeek_;
			this.SoftDeleted = SoftDeleted_;
		}
	}
}
