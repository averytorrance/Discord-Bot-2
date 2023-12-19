using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Models
{
	public class CountryDim
	{
		public int ID { get; set; }
		public string Country { get; set; }

		public CountryDim(int ID_, string Country_)
		{
			this.ID = ID_;
			this.Country = Country_;
		}
	}
}
