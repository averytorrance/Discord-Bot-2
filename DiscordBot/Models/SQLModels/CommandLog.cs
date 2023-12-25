using System;

namespace DiscordBot.Models
{
	public class CommandLog
	{
		public DateTime Instant { get; set; }
		public int Module { get; set; }
		public string UserID { get; set; }
		public int ErrorID { get; set; }
		public string ServerID { get; set; }
		public string Command { get; set; }

		public CommandLog(DateTime Instant_, int Module_, string UserID_, int ErrorID_, string ServerID_, string Command_)
		{
			this.Instant = Instant_;
			this.Module = Module_;
			this.UserID = UserID_;
			this.ErrorID = ErrorID_;
			this.ServerID = ServerID_;
			this.Command = Command_;
		}
	}
}
