using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQTool.Models
{
	public class QuarmMonsterFaction
	{
		public int NPC_ID { get; set; }
		public string Zone_Code { get; set; }
		public string Zone_Name { get; set; }
		public int Faction_ID { get; set; }
		public int Sort_Order { get; set; }
		public int Faction_Hit { get; set; }
		public string Faction_Name { get; set; }
	}

}
