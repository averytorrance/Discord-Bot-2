using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Models
{
	public class RatingsFact
	{
		public string WatchID { get; set; }
		public string UserID { get; set; }
		public double Score { get; set; }

		public RatingsFact(string WatchID_, string UserID_, double Score_)
		{
			this.WatchID = WatchID_;
			this.UserID = UserID_;
			this.Score = Score_;
		}
	}
}
