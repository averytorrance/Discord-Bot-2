using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Models
{
	public class Subscriptions
	{
		public string SubscriptionID { get; set; }
		public string UserID { get; set; }
		public int ReminderID { get; set; }

		public Subscriptions(string SubscriptionID_, string UserID_, int ReminderID_)
		{
			this.SubscriptionID = SubscriptionID_;
			this.UserID = UserID_;
			this.ReminderID = ReminderID_;
		}
	}
}
