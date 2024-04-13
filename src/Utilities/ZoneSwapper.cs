using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQTool.Utilities
{
	public static class ZoneSwapper
	{
		public static string GetSwappedZoneCode(string zoneCode, bool checkValue = false)
		{
			Dictionary<string, string> zonesToSwapForMaps = new Dictionary<string, string>()
			{
				{ "towerfrost", "kurn" },
				{ "myriah", "halas"},
				{ "sodecay", "codecay"}
			};

			if(checkValue)
			{
				if (zonesToSwapForMaps.ContainsValue(zoneCode))
				{
					return zonesToSwapForMaps.FirstOrDefault(x => x.Value == zoneCode).Key;
				}
			}

			if (zonesToSwapForMaps.ContainsKey(zoneCode))
			{
				return zonesToSwapForMaps[zoneCode];
			}

			return zoneCode;
		}

		public static string GetSwappedZoneName(string zoneName, bool checkValue = false)
		{
			Dictionary<string, string> zonesToSwapForMaps = new Dictionary<string, string>()
			{
				{ "oops, all icebones!", "kurn's tower" },
				{ "domain of frost", "halas"},
				{ "shard of decay", "the crypt of decay"}
			};

			if(checkValue)
			{
				if (zonesToSwapForMaps.ContainsValue(zoneName))
				{
					return zonesToSwapForMaps.FirstOrDefault(x => x.Value == zoneName).Key;
				}
			}

			if (zonesToSwapForMaps.ContainsKey(zoneName))
			{
				return zonesToSwapForMaps[zoneName];
			}

			return zoneName;

		}
	}
}
