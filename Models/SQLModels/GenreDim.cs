using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Models
{
	public class GenreDim
	{
		public int ID { get; set; }
		public string Genre { get; set; }

		public GenreDim(int ID_, string Genre_)
		{
			this.ID = ID_;
			this.Genre = Genre_;
		}
	}
}
