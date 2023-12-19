using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Models
{
	public class Errors
	{
		public int ID { get; set; }
		public DateTime Instant { get; set; }
		public string Error { get; set; }
		public string Command { get; set; }
		public string ServerID { get; set; }

		public Errors(int ID_, DateTime Instant_, string Error_, string Command_, string ServerID_)
		{
			this.ID = ID_;
			this.Instant = Instant_;
			this.Error = Error_;
			this.Command = Command_;
			this.ServerID = ServerID_;
		}
	}
}
