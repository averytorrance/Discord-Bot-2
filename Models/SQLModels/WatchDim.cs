using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Models
{
	public class WatchDim
	{
		public string ID { get; set; }
		public string Title { get; set; }
		public int Year { get; set; }
		public string Type { get; set; }
		public bool isRewatch { get; set; }
		public DateTime Instant { get; set; }
		public int GenreID { get; set; }
		public int CountryID { get; set; }
		public int LanguageID { get; set; }

		public WatchDim(string ID_, string Title_, int Year_, string Type_, bool isRewatch_, DateTime Instant_, int GenreID_, int CountryID_, int LanguageID_)
		{
			this.ID = ID_;
			this.Title = Title_;
			this.Year = Year_;
			this.Type = Type_;
			this.isRewatch = isRewatch_;
			this.Instant = Instant_;
			this.GenreID = GenreID_;
			this.CountryID = CountryID_;
			this.LanguageID = LanguageID_;
		}
	}
}
