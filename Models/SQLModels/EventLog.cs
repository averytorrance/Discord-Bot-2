using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Models
{
	public class EventLog
	{
		public DateTime Instant { get; set; }
		public int Module { get; set; }
		public string UserID { get; set; }
		public int ErrorID { get; set; }
		public string ServerID { get; set; }
		public string Command { get; set; }
		public int Runtime { get; set; }

		public EventLog(DateTime Instant_, int Module_, string UserID_, int ErrorID_, string ServerID_, string Command_, int Runtime_)
		{
			this.Instant = Instant_;
			this.Module = Module_;
			this.UserID = UserID_;
			this.ErrorID = ErrorID_;
			this.ServerID = ServerID_;
			this.Command = Command_;
			this.Runtime = Runtime_;
		}
	}
}
