using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQTool.Models
{
	public class JsonMonsterFaction
	{
		public int npc_id { get; set; }
		public string zone_code { get; set; }
		public string zone_name { get; set; }
		public int faction_id { get; set; }
		public int sort_order { get; set; }
		public int faction_hit { get; set; }
		public string faction_name { get; set; }
	}

}
