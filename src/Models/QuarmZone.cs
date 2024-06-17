using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQTool.Models
{
	public class QuarmZone
	{
		public int ID { get; set; }
		public int Zone_ID { get; set; }
		public string Name { get; set; }
		public string Code { get; set; }
		public bool IsDungeon { get; set; }
		public bool IsOutdoor { get; set; }
		public bool HasReducedSpawnTimers { get; set; }
		public double ZEM { get; set; }

	}
}
