using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Models
{
	public class SubscriptionsDim
	{
		public string SubscriptionID { get; set; }
		public string UserID { get; set; }
		public int ReminderID { get; set; }

		public SubscriptionsDim(string SubscriptionID_, string UserID_, int ReminderID_)
		{
			this.SubscriptionID = SubscriptionID_;
			this.UserID = UserID_;
			this.ReminderID = ReminderID_;
		}
	}
}
