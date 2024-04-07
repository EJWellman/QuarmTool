using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQTool.Models
{
	public class JsonMonsterDrops
	{
		public int item_id { get; set; }
		public string item_name { get; set; }
		public float drop_chance { get; set; }
		public int loottable_id { get; set; }
	}
}
