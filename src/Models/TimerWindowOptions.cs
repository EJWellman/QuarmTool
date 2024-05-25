using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQTool.Models
{
	[Table("TimerWindows")]
	public class TimerWindowOptions
	{
		public int ID { get; set; }
		public string Title { get; set; }
		public bool BestGuessSpells { get; set; }
		public bool YouOnlySpells { get; set; }
		public bool ShowRandomRolls { get; set; }
		public bool ShowTimers { get; set; }
		public bool ShowModRodTimers { get; set; }
		public bool ShowSpells { get; set; }

		public string WindowRect { get; set; } //-1092,727,310,413
		public int State { get; set; }
		public bool Closed { get; set; }
		public bool AlwaysOnTop { get; set;}
		public double Opacity { get; set; }
	}
}
