using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQTool.Models
{
	public class JsonMonsterDrops
	{
		public int Item_ID { get; set; }
		public string Item_Name { get; set; }
		public float Drop_Chance { get; set; }
		public int Loottable_ID { get; set; }
	}
}
