using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQTool.Utilities
{
	public static class ZoneSwapper
	{
		public static string GetSwappedInstanceZoneCode(string zoneCode, bool checkValue = false)
		{
			Dictionary<string, string> zonesToSwapForMaps = new Dictionary<string, string>()
			{
				{ "air_instanced", "airplane" },
				{ "charasis_instanced", "charasis" },
				{ "chardok_instanced", "chardok" },
				{ "citymist_instanced", "citymist" },
				{ "cshome2", "cshome" },
				{ "dreadlands_instanced", "dreadlands" },
				{ "emeraldjungle_instanced", "emeraldjungle" },
				{ "fear_instanced", "fearplane" },
				{ "fireice", "arena" },
				{ "hate_instanced", "hateplane" },
				{ "hole_instanced", "hole" },
				{ "karnor_instanced", "karnor" },
				{ "kedge_tryout", "kedge" },
				{ "kithicor_alt", "kithicor" },
				{ "kithicor_instanced", "kithicor" },
				{ "mischiefhouse", "mischiefplane" },
				{ "myriah", "halas" },
				{ "perma_tryout", "permafrost" },
				{ "plaguecrypt", "dalnir" },
				{ "rivervale_alt", "rivervale" },
				{ "sebilis_instanced", "sebilis" },
				{ "skyfire_instanced", "skyfire" },
				{ "sodecay", "codecay" },
				{ "soldungb_tryout", "soldungb" },
				{ "timorous_instanced", "timorous" },
				{ "towerbone", "fieldofbone" },
				{ "towerfrost", "kurn" },
				{ "veeshan_instanced", "veeshan" }
			};

			if (checkValue)
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
		public static string GetSwappedZoneCode(string zoneCode, bool checkValue = false)
		{
			Dictionary<string, string> zonesToSwapForMaps = new Dictionary<string, string>()
			{
				{ "air_instanced", "airplane" },
				{ "charasis_instanced", "charasis" },
				{ "chardok_instanced", "chardok" },
				{ "citymist_instanced", "citymist" },
				{ "cshome2", "cshome" },
				{ "dreadlands_instanced", "dreadlands" },
				{ "emeraldjungle_instanced", "emeraldjungle" },
				{ "fear_instanced", "fearplane" },
				{ "fireice", "arena" },
				{ "hate_instanced", "hateplane" },
				{ "hole_instanced", "hole" },
				{ "karnor_instanced", "karnor" },
				{ "kedge_tryout", "kedge" },
				{ "kithicor_alt", "kithicor" },
				{ "kithicor_instanced", "kithicor" },
				{ "mischiefhouse", "mischiefplane" },
				{ "myriah", "halas" },
				{ "perma_tryout", "permafrost" },
				{ "plaguecrypt", "dalnir" },
				{ "rivervale_alt", "rivervale" },
				{ "sebilis_instanced", "sebilis" },
				{ "skyfire_instanced", "skyfire" },
				{ "sodecay", "codecay" },
				{ "soldungb_tryout", "soldungb" },
				{ "timorous_instanced", "timorous" },
				{ "towerbone", "fieldofbone" },
				{ "towerfrost", "kurn" },
				{ "veeshan_instanced", "veeshan" }
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
				{ "field of bone (alt)", "fieldofbone" },
				{ "shard of decay", "codecay" },
				{ "nagafen's lair (instanced)", "soldungb" },
				{ "permafrost caverns (instanced)", "permafrost" },
				{ "kedge keep (instanced)", "kedge" },
				{ "domain of frost", "halas" },
				{ "trial of fire and ice (instanced)", "arena" },
				{ "plane of hate (instanced)", "hateplane" },
				{ "plane of fear (instanced)", "fearplane" },
				{ "plane of sky (instanced)", "airplane" },
				{ "kurn's tower (alternate)", "kurn" },
				{ "the hole (instanced)", "hole" },
				{ "sunset home", "cshome" },
				{ "house of mischief", "mischiefplane" },
				{ "bloodied kithicor", "kithicor" },
				{ "bloodied rivervale (instanced)", "rivervale" },
				{ "howling stones (instanced)", "charasis" },
				{ "chardok (instanced)", "chardok" },
				{ "dreadlands (instanced)", "dreadlands" },
				{ "emerald jungle (instanced)", "emeraldjungle" },
				{ "timorous deep (instanced)", "timorous" },
				{ "skyfire mountains (instanced)", "skyfire" },
				{ "ruins of sebilis (instanced)", "sebilis" },
				{ "karnor's castle (instanced)", "karnor" },
				{ "veeshan's peak (instanced)", "veeshan" },
				{ "oops, all goos!", "dalnir" },
				{ "kithicor forest (instanced)", "kithicor" },
				{ "the city of mist (instanced)", "citymist" }

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
