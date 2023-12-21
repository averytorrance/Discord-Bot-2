using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Models
{
	public class User
	{
		public string UserID { get; set; }
		public string Username { get; set; }
		public bool isDuplicateUser { get; set; }
		public string OriginalUserID { get; set; }
		public string Name { get; set; }
		public string Address { get; set; }
		public string Timezone { get; set; }

		public User(string UserID_, string Username_, bool isDuplicateUser_, string OriginalUserID_, string Name_, string Address_, string Timezone_)
		{
			this.UserID = UserID_;
			this.Username = Username_;
			this.isDuplicateUser = isDuplicateUser_;
			this.OriginalUserID = OriginalUserID_;
			this.Name = Name_;
			this.Address = Address_;
			this.Timezone = Timezone_;
		}
	}
}
