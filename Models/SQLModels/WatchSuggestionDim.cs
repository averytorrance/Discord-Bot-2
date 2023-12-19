using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Models
{
	public class WatchSuggestionDim
	{
		public string ID { get; set; }
		public string Title { get; set; }
		public string UserID { get; set; }

		public WatchSuggestionDim(string ID_, string Title_, string UserID_)
		{
			this.ID = ID_;
			this.Title = Title_;
			this.UserID = UserID_;
		}
	}
}
