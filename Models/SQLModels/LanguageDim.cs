using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Models
{
	public class LanguageDim
	{
		public int ID { get; set; }
		public string Language { get; set; }

		public LanguageDim(int ID_, string Language_)
		{
			this.ID = ID_;
			this.Language = Language_;
		}
	}
}
