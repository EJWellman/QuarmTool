using EQToolShared.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EQToolShared.Map
{
	public class NpcSpawnTime
	{
		public string Name { get; set; }
		public TimeSpan RespawnTime { get; set; }
	}

	public class ZoneInfo
	{
		public bool ShowAllMapLevels { get; set; }
		public double ZoneLevelHeight { get; set; }
		public string Name { get; set; }
	}

	public class PlayerZonedInfo
	{
		public string ZoneName { get; set; }
		public bool IsInstance { get; set; } = false;
		public bool ForceUpdate { get; set; } = false;
	}

	public static class ZoneParser
	{
		private const string YouHaveEntered = "You have entered ";
		private const string ThereAreNoPlayers = "There are no players ";
		private const string ThereAre = "There are ";
		private const string ThereIs = "There is ";
		private const string YouHaveEnteredAreaPVP = "You have entered an Arena (PvP) area.";
		private const string spaceinspace = "in ";
		public static readonly List<string> KaelFactionMobs = new List<string>() {
			"Bygloirn Omorden",
			"Dagron Stonecutter",
			"Barlek Stonefist",
			"Gragek Mjlorkigar",
			"Kelenek Bluadfeth",
			"Veldern Blackhammer",
			"Kragek Thunderforge",
			"Stoem Lekbar",
			"Bjarorm Mjlorn",
			"Ulkar Jollkarek",
			"Vylleam Vyaeltor",
			"Jaglorm Ygorr",
			"Yeeldan Spiritcaller"
		};

		public static readonly List<ZoneNameInfo> ZoneNames = new List<ZoneNameInfo>();
		public static readonly Dictionary<string, ZoneInfo> ZoneInfoMap = new Dictionary<string, ZoneInfo>();

		static ZoneParser()
		{
			#region build ZoneInfoMap
			ZoneInfoMap.Add("airplane", new ZoneInfo
			{
				Name = "airplane",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("air_instanced", new ZoneInfo
			{
				Name = "air_instanced",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("akanon", new ZoneInfo
			{

				Name = "akanon",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("arena", new ZoneInfo
			{
				Name = "arena",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("befallen", new ZoneInfo
			{
				Name = "befallen",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("beholder", new ZoneInfo
			{
				Name = "beholder",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("blackburrow", new ZoneInfo
			{
				Name = "blackburrow",
				ShowAllMapLevels = false,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("burningwood", new ZoneInfo
			{
				Name = "burningwood",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("butcher", new ZoneInfo
			{
				Name = "butcher",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("cabeast", new ZoneInfo
			{
				Name = "cabeast",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10

			});
			ZoneInfoMap.Add("cabwest", new ZoneInfo
			{
				Name = "cabwest",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10

			});
			ZoneInfoMap.Add("cauldron", new ZoneInfo
			{
				Name = "cauldron",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10

			});
			ZoneInfoMap.Add("cazicthule", new ZoneInfo
			{
				Name = "cazicthule",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("charasis", new ZoneInfo
			{
				Name = "charasis",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("chardok", new ZoneInfo
			{
				Name = "chardok",
				ShowAllMapLevels = false,
				ZoneLevelHeight = 30,
			});
			ZoneInfoMap.Add("citymist", new ZoneInfo
			{
				Name = "citymist",
				ShowAllMapLevels = false,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("cobaltscar", new ZoneInfo
			{
				Name = "cobaltscar",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("commons", new ZoneInfo
			{
				Name = "commons",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10
				,
			});
			ZoneInfoMap.Add("crushbone", new ZoneInfo
			{
				Name = "crushbone",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("crystal", new ZoneInfo
			{
				Name = "crystal",
				ZoneLevelHeight = 20
			});
			ZoneInfoMap.Add("dalnir", new ZoneInfo
			{
				Name = "dalnir",
				ShowAllMapLevels = false,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("dreadlands", new ZoneInfo
			{
				Name = "dreadlands",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10
				,
			});
			ZoneInfoMap.Add("droga", new ZoneInfo
			{
				Name = "droga",
				ShowAllMapLevels = false,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("eastkarana", new ZoneInfo
			{
				Name = "eastkarana",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("eastwastes", new ZoneInfo
			{
				Name = "eastwastes",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10

			});
			ZoneInfoMap.Add("ecommons", new ZoneInfo
			{
				Name = "ecommons",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10

			});
			ZoneInfoMap.Add("emeraldjungle", new ZoneInfo
			{
				Name = "emeraldjungle",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10

			});
			ZoneInfoMap.Add("erudnext", new ZoneInfo
			{
				Name = "erudnext",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10

			});
			ZoneInfoMap.Add("erudnint", new ZoneInfo
			{
				Name = "erudnint",
				ShowAllMapLevels = false,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("erudsxing", new ZoneInfo
			{
				Name = "erudsxing",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10

			});
			ZoneInfoMap.Add("everfrost", new ZoneInfo
			{
				Name = "everfrost",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10

			});
			ZoneInfoMap.Add("fearplane", new ZoneInfo
			{
				Name = "fearplane",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("fear_instanced", new ZoneInfo
			{
				Name = "fear_instanced",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("feerrott", new ZoneInfo
			{
				Name = "feerrott",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10

			});
			ZoneInfoMap.Add("felwithea", new ZoneInfo
			{
				Name = "felwithea",
				ShowAllMapLevels = false,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("felwitheb", new ZoneInfo
			{
				Name = "felwitheb",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("fieldofbone", new ZoneInfo
			{
				Name = "fieldofbone",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10

			});
			ZoneInfoMap.Add("firiona", new ZoneInfo
			{
				Name = "firiona",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10

			});
			ZoneInfoMap.Add("freporte", new ZoneInfo
			{
				Name = "freporte",
				ShowAllMapLevels = false,
				ZoneLevelHeight = 15

			});
			ZoneInfoMap.Add("freportn", new ZoneInfo
			{
				Name = "freportn",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10

			});
			ZoneInfoMap.Add("freportw", new ZoneInfo
			{
				Name = "freportw",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("frontiermtns", new ZoneInfo
			{
				Name = "frontiermtns",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10

			});
			ZoneInfoMap.Add("frozenshadow", new ZoneInfo
			{
				Name = "frozenshadow",
				ShowAllMapLevels = false,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("gfaydark", new ZoneInfo
			{
				Name = "gfaydark",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("greatdivide", new ZoneInfo
			{
				Name = "greatdivide",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("grobb", new ZoneInfo
			{
				Name = "grobb",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("growthplane", new ZoneInfo
			{
				Name = "growthplane",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10,
			});
			ZoneInfoMap.Add("gukbottom", new ZoneInfo
			{
				Name = "gukbottom",
				ShowAllMapLevels = false,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("guktop", new ZoneInfo
			{
				Name = "guktop",
				ShowAllMapLevels = false,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("halas", new ZoneInfo
			{
				Name = "halas",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("hateplane", new ZoneInfo
			{
				Name = "hateplane",
				ShowAllMapLevels = false,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("hate_instanced", new ZoneInfo
			{
				Name = "hate_instanced",
				ShowAllMapLevels = false,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("highkeep", new ZoneInfo
			{
				Name = "highkeep",
				ShowAllMapLevels = false,
				ZoneLevelHeight = 10,
			});
			ZoneInfoMap.Add("highpass", new ZoneInfo
			{
				Name = "highpass",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("hole", new ZoneInfo
			{
				Name = "hole",
				ShowAllMapLevels = false,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("hole_instanced", new ZoneInfo
			{
				Name = "hole_instanced",
				ShowAllMapLevels = false,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("iceclad", new ZoneInfo
			{
				Name = "iceclad",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10

			});
			ZoneInfoMap.Add("innothule", new ZoneInfo
			{
				Name = "innothule",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10

			});
			ZoneInfoMap.Add("kael", new ZoneInfo
			{
				Name = "kael",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("kaesora", new ZoneInfo
			{
				Name = "kaesora",
				ShowAllMapLevels = false,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("kaladima", new ZoneInfo
			{
				Name = "kaladima",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10

			});
			ZoneInfoMap.Add("kaladimb", new ZoneInfo
			{
				Name = "kaladimb",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10

			});
			ZoneInfoMap.Add("karnor", new ZoneInfo
			{
				Name = "karnor",
				ShowAllMapLevels = false,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("kedge", new ZoneInfo
			{
				Name = "kedge",
				ShowAllMapLevels = false,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("kedge_tryout", new ZoneInfo
			{
				Name = "kedge_tryout",
				ShowAllMapLevels = false,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("kerraridge", new ZoneInfo
			{
				Name = "kerraridge",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("kithicor", new ZoneInfo
			{
				Name = "kithicor",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10

			});
			ZoneInfoMap.Add("kurn", new ZoneInfo
			{
				Name = "kurn",
				ShowAllMapLevels = false,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("lakeofillomen", new ZoneInfo
			{
				Name = "lakeofillomen",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10
				,
			});
			ZoneInfoMap.Add("lakerathe", new ZoneInfo
			{
				Name = "lakerathe",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("lavastorm", new ZoneInfo
			{
				Name = "lavastorm",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10

			});
			ZoneInfoMap.Add("lfaydark", new ZoneInfo
			{
				Name = "lfaydark",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10

			});
			ZoneInfoMap.Add("mischiefplane", new ZoneInfo
			{
				Name = "mischiefplane",
				ShowAllMapLevels = false,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("mistmoore", new ZoneInfo
			{
				Name = "mistmoore",
				ShowAllMapLevels = false,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("misty", new ZoneInfo
			{
				Name = "misty",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10

			});
			ZoneInfoMap.Add("najena", new ZoneInfo
			{
				Name = "najena",
				ShowAllMapLevels = false,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("necropolis", new ZoneInfo
			{
				Name = "necropolis",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("nektulos", new ZoneInfo
			{
				Name = "nektulos",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10

			});
			ZoneInfoMap.Add("neriaka", new ZoneInfo
			{
				Name = "neriaka",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10,
			});
			ZoneInfoMap.Add("neriakb", new ZoneInfo
			{
				Name = "neriakb",
				ShowAllMapLevels = false,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("neriakc", new ZoneInfo
			{
				Name = "neriakc",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("northkarana", new ZoneInfo
			{
				Name = "northkarana",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("nro", new ZoneInfo
			{
				Name = "nro",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10

			});
			ZoneInfoMap.Add("nurga", new ZoneInfo
			{
				Name = "nurga",
				ShowAllMapLevels = false,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("oasis", new ZoneInfo
			{
				Name = "oasis",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10
				,
			});
			ZoneInfoMap.Add("oggok", new ZoneInfo
			{
				Name = "oggok",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("oot", new ZoneInfo
			{
				Name = "oot",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("overthere", new ZoneInfo
			{
				Name = "overthere",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10

			});
			ZoneInfoMap.Add("paineel", new ZoneInfo
			{
				Name = "paineel",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10,
			});
			ZoneInfoMap.Add("paw", new ZoneInfo
			{
				Name = "paw",
				ShowAllMapLevels = false,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("permafrost", new ZoneInfo
			{
				Name = "permafrost",
				ShowAllMapLevels = false,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("perma_tryout", new ZoneInfo
			{
				Name = "perma_tryout",
				ShowAllMapLevels = false,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("qcat", new ZoneInfo
			{
				Name = "qcat",
				ShowAllMapLevels = false,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("qey2hh1", new ZoneInfo
			{
				Name = "qey2hh1",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10

			});
			ZoneInfoMap.Add("qeynos", new ZoneInfo
			{
				Name = "qeynos",
				ShowAllMapLevels = false,
				ZoneLevelHeight = 10

			});
			ZoneInfoMap.Add("qeynos2", new ZoneInfo
			{
				Name = "qeynos2",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10

			});
			ZoneInfoMap.Add("qeytoqrg", new ZoneInfo
			{
				Name = "qeytoqrg",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10

			});
			ZoneInfoMap.Add("qrg", new ZoneInfo
			{
				Name = "qrg",
				ShowAllMapLevels = false,
				ZoneLevelHeight = 10

			});
			ZoneInfoMap.Add("rathemtn", new ZoneInfo
			{
				Name = "rathemtn",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10

			});
			ZoneInfoMap.Add("rivervale", new ZoneInfo
			{
				Name = "rivervale",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("runnyeye", new ZoneInfo
			{
				Name = "runnyeye",
				ShowAllMapLevels = false,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("sebilis", new ZoneInfo
			{
				Name = "sebilis",
				ShowAllMapLevels = false,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("sirens", new ZoneInfo
			{
				Name = "sirens",
				ShowAllMapLevels = false,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("skyfire", new ZoneInfo
			{
				Name = "skyfire",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("skyshrine", new ZoneInfo
			{
				Name = "skyshrine",
				ShowAllMapLevels = false,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("sleeper", new ZoneInfo
			{
				Name = "sleeper",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("soldunga", new ZoneInfo
			{
				Name = "soldunga",
				ShowAllMapLevels = false,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("soldungb", new ZoneInfo
			{
				Name = "soldungb",
				ShowAllMapLevels = false,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("soltemple", new ZoneInfo
			{
				Name = "soltemple",
				ShowAllMapLevels = false,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("southkarana", new ZoneInfo
			{
				Name = "southkarana",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10,
			});
			ZoneInfoMap.Add("sro", new ZoneInfo
			{
				Name = "sro",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10

			});
			ZoneInfoMap.Add("steamfont", new ZoneInfo
			{
				Name = "steamfont",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10

			});
			ZoneInfoMap.Add("stonebrunt", new ZoneInfo
			{
				Name = "stonebrunt",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("swampofnohope", new ZoneInfo
			{
				Name = "swampofnohope",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10

			});
			ZoneInfoMap.Add("templeveeshan", new ZoneInfo
			{
				Name = "templeveeshan",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10,
			});
			ZoneInfoMap.Add("thurgadina", new ZoneInfo
			{
				Name = "thurgadina",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("thurgadinb", new ZoneInfo
			{
				Name = "thurgadinb",
				ShowAllMapLevels = false,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("timorous", new ZoneInfo
			{
				Name = "timorous",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("tox", new ZoneInfo
			{
				Name = "tox",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10

			});
			ZoneInfoMap.Add("trakanon", new ZoneInfo
			{
				Name = "trakanon",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10

			});
			ZoneInfoMap.Add("unrest", new ZoneInfo
			{
				Name = "unrest",
				ShowAllMapLevels = false,
				ZoneLevelHeight = 4
			});

			ZoneInfoMap.Add("veeshan", new ZoneInfo
			{
				Name = "veeshan",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("velketor", new ZoneInfo
			{
				Name = "velketor",
				ShowAllMapLevels = false,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("wakening", new ZoneInfo
			{
				Name = "wakening",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10
				,
			});
			ZoneInfoMap.Add("warrens", new ZoneInfo
			{
				Name = "warrens",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10
				,
			});
			ZoneInfoMap.Add("warslikswood", new ZoneInfo
			{
				Name = "warslikswood",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10

			});
			ZoneInfoMap.Add("westwastes", new ZoneInfo
			{
				Name = "westwastes",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10,

			});
			ZoneInfoMap.Add("towerfrost", new ZoneInfo
			{
				Name = "towerfrost",
				ShowAllMapLevels = false,
				ZoneLevelHeight = 10
			});
			ZoneInfoMap.Add("myriah", new ZoneInfo
			{
				Name = "myriah",
				ShowAllMapLevels = true,
				ZoneLevelHeight = 10,
			});
			ZoneInfoMap.Add("soldungb_tryout", new ZoneInfo
			{
				Name = "soldungb_tryout",
				ShowAllMapLevels = false,
				ZoneLevelHeight = 10
			});
			#endregion

			#region ZoneNames
			ZoneNames.Add(new ZoneNameInfo("acrylia caverns", "acrylia"));
			ZoneNames.Add(new ZoneNameInfo("plane of sky", "airplane"));
			ZoneNames.Add(new ZoneNameInfo("ak'anon", "akanon"));
			ZoneNames.Add(new ZoneNameInfo("akheva ruins", "akheva"));
			ZoneNames.Add(new ZoneNameInfo("the arena", "arena"));
			ZoneNames.Add(new ZoneNameInfo("the arena two", "arena2"));
			ZoneNames.Add(new ZoneNameInfo("aviak village", "aviak"));
			ZoneNames.Add(new ZoneNameInfo("befallen", "befallen"));
			ZoneNames.Add(new ZoneNameInfo("gorge of king xorbb", "beholder"));
			ZoneNames.Add(new ZoneNameInfo("blackburrow", "blackburrow"));
			ZoneNames.Add(new ZoneNameInfo("bastion of thunder", "bothunder"));
			ZoneNames.Add(new ZoneNameInfo("the burning wood", "burningwood", "burning woods"));
			ZoneNames.Add(new ZoneNameInfo("butcherblock mountains", "butcher"));
			ZoneNames.Add(new ZoneNameInfo("cabilis east", "cabeast", "east cabilis"));
			ZoneNames.Add(new ZoneNameInfo("cabilis west", "cabwest", "west cabilis"));
			ZoneNames.Add(new ZoneNameInfo("dagnor's cauldron", "cauldron"));
			ZoneNames.Add(new ZoneNameInfo("lost temple of cazicthule", "cazicthule", "cazic-thule"));
			ZoneNames.Add(new ZoneNameInfo("the howling stones", "charasis", "howling stones"));
			ZoneNames.Add(new ZoneNameInfo("chardok", "chardok"));
			ZoneNames.Add(new ZoneNameInfo("the city of mist", "citymist", "city of mist"));
			ZoneNames.Add(new ZoneNameInfo("loading", "clz"));
			ZoneNames.Add(new ZoneNameInfo("cobaltscar", "cobaltscar"));
			ZoneNames.Add(new ZoneNameInfo("the crypt of decay", "codecay"));
			ZoneNames.Add(new ZoneNameInfo("tower of bone (gm)", "towerbone", "fieldofbone"));
			ZoneNames.Add(new ZoneNameInfo("west commonlands", "commons"));
			ZoneNames.Add(new ZoneNameInfo("crushbone", "crushbone", "clan crushbone"));
			ZoneNames.Add(new ZoneNameInfo("crystal caverns", "crystal"));
			ZoneNames.Add(new ZoneNameInfo("sunset home", "cshome"));
			ZoneNames.Add(new ZoneNameInfo("the crypt of dalnir", "dalnir"));
			ZoneNames.Add(new ZoneNameInfo("the dawnshroud peaks", "dawnshroud"));
			ZoneNames.Add(new ZoneNameInfo("the dreadlands", "dreadlands"));
			ZoneNames.Add(new ZoneNameInfo("mines of droga", "droga", "temple of droga"));
			ZoneNames.Add(new ZoneNameInfo("eastern plains of karana", "eastkarana", "east karana"));
			ZoneNames.Add(new ZoneNameInfo("eastern wastes", "eastwastes"));
			ZoneNames.Add(new ZoneNameInfo("echo caverns", "echo"));
			ZoneNames.Add(new ZoneNameInfo("east commonlands", "ecommons"));
			ZoneNames.Add(new ZoneNameInfo("the emerald jungle", "emeraldjungle"));
			ZoneNames.Add(new ZoneNameInfo("erudin", "erudnext"));
			ZoneNames.Add(new ZoneNameInfo("erudin palace", "erudnint"));
			ZoneNames.Add(new ZoneNameInfo("erud's crossing", "erudsxing"));
			ZoneNames.Add(new ZoneNameInfo("marauders mire", "erudsxing2"));
			ZoneNames.Add(new ZoneNameInfo("everfrost peaks", "everfrost"));
			ZoneNames.Add(new ZoneNameInfo("plane of fear", "fearplane"));
			ZoneNames.Add(new ZoneNameInfo("the feerrott", "feerrott"));
			ZoneNames.Add(new ZoneNameInfo("northern felwithe", "felwithea", "felwithe"));
			ZoneNames.Add(new ZoneNameInfo("southern felwithe", "felwitheb", "felwithe"));
			ZoneNames.Add(new ZoneNameInfo("field of bone", "fieldofbone", "the field of bone"));
			ZoneNames.Add(new ZoneNameInfo("field of bone (alt)", "fieldofbone", "the field of bone"));
			ZoneNames.Add(new ZoneNameInfo("firiona vie", "firiona"));
			ZoneNames.Add(new ZoneNameInfo("east freeport", "freporte"));
			ZoneNames.Add(new ZoneNameInfo("north freeport", "freportn"));
			ZoneNames.Add(new ZoneNameInfo("west freeport", "freportw"));
			ZoneNames.Add(new ZoneNameInfo("frontier mountains", "frontiermtns"));
			ZoneNames.Add(new ZoneNameInfo("tower of frozen shadow", "frozenshadow"));
			ZoneNames.Add(new ZoneNameInfo("the fungus grove", "fungusgrove"));
			ZoneNames.Add(new ZoneNameInfo("greater faydark", "gfaydark"));
			ZoneNames.Add(new ZoneNameInfo("the great divide", "greatdivide"));
			ZoneNames.Add(new ZoneNameInfo("grieg's end", "griegsend"));
			ZoneNames.Add(new ZoneNameInfo("grimling forest", "grimling"));
			ZoneNames.Add(new ZoneNameInfo("grobb", "grobb"));
			ZoneNames.Add(new ZoneNameInfo("plane of growth", "growthplane"));
			ZoneNames.Add(new ZoneNameInfo("ruins of old guk", "gukbottom", "lower guk"));
			ZoneNames.Add(new ZoneNameInfo("guk", "guktop", "upper guk"));
			ZoneNames.Add(new ZoneNameInfo("halas", "halas"));
			ZoneNames.Add(new ZoneNameInfo("plane of hate", "hateplane", "the plane of hate"));
			ZoneNames.Add(new ZoneNameInfo("high keep", "highkeep", "highkeep"));
			ZoneNames.Add(new ZoneNameInfo("highpass hold", "highpass"));
			ZoneNames.Add(new ZoneNameInfo("halls of honor", "hohonora"));
			ZoneNames.Add(new ZoneNameInfo("temple of marr", "hohonorb"));
			ZoneNames.Add(new ZoneNameInfo("the hole", "hole"));
			ZoneNames.Add(new ZoneNameInfo("hollowshade moor", "hollowshade"));
			ZoneNames.Add(new ZoneNameInfo("iceclad ocean", "iceclad"));
			ZoneNames.Add(new ZoneNameInfo("innothule swamp", "innothule"));
			ZoneNames.Add(new ZoneNameInfo("jaggedpine forest", "jaggedpine"));
			ZoneNames.Add(new ZoneNameInfo("kael drakkel", "kael"));
			ZoneNames.Add(new ZoneNameInfo("kaesora", "kaesora"));
			ZoneNames.Add(new ZoneNameInfo("south kaladim", "kaladima", "kaladim"));
			ZoneNames.Add(new ZoneNameInfo("north kaladim", "kaladimb", "kaladim"));
			ZoneNames.Add(new ZoneNameInfo("karnor's castle", "karnor"));
			ZoneNames.Add(new ZoneNameInfo("katta castellum", "katta"));
			ZoneNames.Add(new ZoneNameInfo("kedge keep", "kedge"));
			ZoneNames.Add(new ZoneNameInfo("kerra isle", "kerraridge"));
			ZoneNames.Add(new ZoneNameInfo("kithicor forest", "kithicor"));
			ZoneNames.Add(new ZoneNameInfo("kurn's tower", "kurn"));
			ZoneNames.Add(new ZoneNameInfo("kurn's tower (alt)", "kurn"));
			ZoneNames.Add(new ZoneNameInfo("lake of ill omen", "lakeofillomen"));
			ZoneNames.Add(new ZoneNameInfo("lake rathetear", "lakerathe"));
			ZoneNames.Add(new ZoneNameInfo("lavastorm mountains", "lavastorm"));
			ZoneNames.Add(new ZoneNameInfo("mons letalis", "letalis"));
			ZoneNames.Add(new ZoneNameInfo("lesser faydark", "lfaydark"));
			ZoneNames.Add(new ZoneNameInfo("loading zone", "load"));
			ZoneNames.Add(new ZoneNameInfo("new loading zone", "load2"));
			ZoneNames.Add(new ZoneNameInfo("the maiden's eye", "maiden"));
			ZoneNames.Add(new ZoneNameInfo("plane of mischief", "mischiefplane"));
			ZoneNames.Add(new ZoneNameInfo("castle of mistmoore", "mistmoore", "castle mistmoore"));
			ZoneNames.Add(new ZoneNameInfo("misty thicket", "misty"));
			ZoneNames.Add(new ZoneNameInfo("marus seru", "mseru"));
			ZoneNames.Add(new ZoneNameInfo("najena", "najena"));
			ZoneNames.Add(new ZoneNameInfo("dragon necropolis", "necropolis"));
			ZoneNames.Add(new ZoneNameInfo("nektropos", "nektropos"));
			ZoneNames.Add(new ZoneNameInfo("neriak - foreign quarter", "neriaka", "neriak foreign quarter"));
			ZoneNames.Add(new ZoneNameInfo("neriak - commons", "neriakb", "neriak commons"));
			ZoneNames.Add(new ZoneNameInfo("neriak - 3rd gate", "neriakc", "neriak third gate"));
			ZoneNames.Add(new ZoneNameInfo("neriak palace", "neriakd"));
			ZoneNames.Add(new ZoneNameInfo("netherbian lair", "netherbian"));
			ZoneNames.Add(new ZoneNameInfo("nexus", "nexus", "the nexus"));
			ZoneNames.Add(new ZoneNameInfo("the lair of terris thule", "nightmareb"));
			ZoneNames.Add(new ZoneNameInfo("northern plains of karana", "northkarana", "north karana"));
			ZoneNames.Add(new ZoneNameInfo("northern desert of ro", "nro", "north ro"));
			ZoneNames.Add(new ZoneNameInfo("mines of nurga", "nurga"));
			ZoneNames.Add(new ZoneNameInfo("oasis of marr", "oasis"));
			ZoneNames.Add(new ZoneNameInfo("oggok", "oggok"));
			ZoneNames.Add(new ZoneNameInfo("ocean of tears", "oot"));
			ZoneNames.Add(new ZoneNameInfo("the overthere", "overthere"));
			ZoneNames.Add(new ZoneNameInfo("paineel", "paineel"));
			ZoneNames.Add(new ZoneNameInfo("the paludal caverns", "paludal"));
			ZoneNames.Add(new ZoneNameInfo("lair of the splitpaw", "paw", "infected paw"));
			ZoneNames.Add(new ZoneNameInfo("permafrost caverns", "permafrost", "permafrost keep"));
			ZoneNames.Add(new ZoneNameInfo("plane of air", "poair"));
			ZoneNames.Add(new ZoneNameInfo("plane of disease", "podisease"));
			ZoneNames.Add(new ZoneNameInfo("plane of earth", "poeartha"));
			ZoneNames.Add(new ZoneNameInfo("plane of earth", "poearthb"));
			ZoneNames.Add(new ZoneNameInfo("plane of fire", "pofire"));
			ZoneNames.Add(new ZoneNameInfo("plane of innovation", "poinnovation"));
			ZoneNames.Add(new ZoneNameInfo("plane of justice", "pojustice"));
			ZoneNames.Add(new ZoneNameInfo("plane of knowledge", "poknowledge"));
			ZoneNames.Add(new ZoneNameInfo("plane of nightmares", "ponightmare"));
			ZoneNames.Add(new ZoneNameInfo("plane of storms", "postorms"));
			ZoneNames.Add(new ZoneNameInfo("drunder, the fortress of zek", "potactics"));
			ZoneNames.Add(new ZoneNameInfo("plane of time", "potimea"));
			ZoneNames.Add(new ZoneNameInfo("plane of time", "potimeb"));
			ZoneNames.Add(new ZoneNameInfo("torment, the plane of pain", "potorment"));
			ZoneNames.Add(new ZoneNameInfo("plane of tranquility", "potranquility"));
			ZoneNames.Add(new ZoneNameInfo("plane of valor", "povalor"));
			ZoneNames.Add(new ZoneNameInfo("plane of war", "powar"));
			ZoneNames.Add(new ZoneNameInfo("plane of water", "powater"));
			ZoneNames.Add(new ZoneNameInfo("qeynos aqueduct system", "qcat", "qeynos catacombs"));
			ZoneNames.Add(new ZoneNameInfo("western plains of karana", "qey2hh1", "west karana"));
			ZoneNames.Add(new ZoneNameInfo("south qeynos", "qeynos"));
			ZoneNames.Add(new ZoneNameInfo("north qeynos", "qeynos2"));
			ZoneNames.Add(new ZoneNameInfo("qeynos hills", "qeytoqrg"));
			ZoneNames.Add(new ZoneNameInfo("surefall glade", "qrg"));
			ZoneNames.Add(new ZoneNameInfo("rathe mountains", "rathemtn", "mountains of rathe"));
			ZoneNames.Add(new ZoneNameInfo("rivervale", "rivervale"));
			ZoneNames.Add(new ZoneNameInfo("runnyeye", "runnyeye", "clan runnyeye"));
			ZoneNames.Add(new ZoneNameInfo("scarlet desert", "scarlet"));
			ZoneNames.Add(new ZoneNameInfo("ruins of sebilis", "sebilis", "old sebilis"));
			ZoneNames.Add(new ZoneNameInfo("shadeweaver's thicket", "shadeweaver"));
			ZoneNames.Add(new ZoneNameInfo("shadow haven", "shadowhaven"));
			ZoneNames.Add(new ZoneNameInfo("the city of shar vahl", "sharvahl"));
			ZoneNames.Add(new ZoneNameInfo("siren's grotto", "sirens"));
			ZoneNames.Add(new ZoneNameInfo("skyfire mountains", "skyfire"));
			ZoneNames.Add(new ZoneNameInfo("skyshrine", "skyshrine"));
			ZoneNames.Add(new ZoneNameInfo("sleeper's tomb", "sleeper"));
			ZoneNames.Add(new ZoneNameInfo("solusek's eye", "soldunga"));
			ZoneNames.Add(new ZoneNameInfo("nagafen's lair", "soldungb"));
			ZoneNames.Add(new ZoneNameInfo("tower of solusek ro", "solrotower"));
			ZoneNames.Add(new ZoneNameInfo("temple of solusek ro", "soltemple"));
			ZoneNames.Add(new ZoneNameInfo("southern plains of karana", "southkarana", "south karana"));
			ZoneNames.Add(new ZoneNameInfo("southern desert of ro", "sro", "south ro"));
			ZoneNames.Add(new ZoneNameInfo("sanctus seru", "sseru"));
			ZoneNames.Add(new ZoneNameInfo("ssraeshza temple", "ssratemple"));
			ZoneNames.Add(new ZoneNameInfo("steamfont mountains", "steamfont"));
			ZoneNames.Add(new ZoneNameInfo("stonebrunt mountains", "stonebrunt"));
			ZoneNames.Add(new ZoneNameInfo("swamp of no hope", "swampofnohope"));
			ZoneNames.Add(new ZoneNameInfo("temple of veeshan", "templeveeshan"));
			ZoneNames.Add(new ZoneNameInfo("the tenebrous mountains", "tenebrous"));
			ZoneNames.Add(new ZoneNameInfo("the deep", "thedeep"));
			ZoneNames.Add(new ZoneNameInfo("the grey", "thegrey"));
			ZoneNames.Add(new ZoneNameInfo("the city of thurgadin", "thurgadina"));
			ZoneNames.Add(new ZoneNameInfo("icewell keep", "thurgadinb"));
			ZoneNames.Add(new ZoneNameInfo("timorous deep", "timorous"));
			ZoneNames.Add(new ZoneNameInfo("toxxulia forest", "tox"));
			ZoneNames.Add(new ZoneNameInfo("trakanon's teeth", "trakanon"));
			ZoneNames.Add(new ZoneNameInfo("everquest tutorial", "tutorial"));
			ZoneNames.Add(new ZoneNameInfo("twilight", "twilight"));
			ZoneNames.Add(new ZoneNameInfo("the umbral plains", "umbral"));
			ZoneNames.Add(new ZoneNameInfo("the estate of unrest", "unrest", "estate of unrest"));
			ZoneNames.Add(new ZoneNameInfo("veeshan's peak", "veeshan"));
			ZoneNames.Add(new ZoneNameInfo("veksar", "veksar"));
			ZoneNames.Add(new ZoneNameInfo("velketor's labyrinth", "velketor"));
			ZoneNames.Add(new ZoneNameInfo("vex thal", "vexthal"));
			ZoneNames.Add(new ZoneNameInfo("the wakening land", "wakening"));
			ZoneNames.Add(new ZoneNameInfo("the warrens", "warrens"));
			ZoneNames.Add(new ZoneNameInfo("warsliks woods", "warslikswood", "warsliks wood"));
			ZoneNames.Add(new ZoneNameInfo("western wastes", "westwastes"));
			ZoneNames.Add(new ZoneNameInfo("nektulos forest", "nektulos"));
			ZoneNames.Add(new ZoneNameInfo("the bazaar", "bazaar"));
			ZoneNames.Add(new ZoneNameInfo("field of bone (alt)", "towerbone", null, null, "fieldofbone"));
			ZoneNames.Add(new ZoneNameInfo("shard of decay", "sodecay", null, null, "codecay"));
			ZoneNames.Add(new ZoneNameInfo("nagafen's lair (instanced)", "soldungb_tryout", null, null, "soldungb"));
			ZoneNames.Add(new ZoneNameInfo("permafrost caverns (instanced)", "perma_tryout", null, null, "permafrost"));
			ZoneNames.Add(new ZoneNameInfo("kedge keep (instanced)", "kedge_tryout", null, null, "kedge"));
			ZoneNames.Add(new ZoneNameInfo("domain of frost", "myriah", null, null, "halas"));
			ZoneNames.Add(new ZoneNameInfo("trial of fire and ice (instanced)", "fireice", null, null, "arena"));
			ZoneNames.Add(new ZoneNameInfo("plane of hate (instanced)", "hate_instanced", null, null, "hateplane"));
			ZoneNames.Add(new ZoneNameInfo("plane of fear (instanced)", "fear_instanced", null, null, "fearplane"));
			ZoneNames.Add(new ZoneNameInfo("plane of sky (instanced)", "air_instanced", null, null, "airplane"));
			ZoneNames.Add(new ZoneNameInfo("kurn's tower (alternate)", "towerfrost", null, null, "kurn"));
			ZoneNames.Add(new ZoneNameInfo("the hole (instanced)", "hole_instanced", null, null, "hole"));
			ZoneNames.Add(new ZoneNameInfo("sunset home", "cshome2", null, null, "cshome"));
			ZoneNames.Add(new ZoneNameInfo("house of mischief", "mischiefhouse", null, null, "mischiefplane"));
			ZoneNames.Add(new ZoneNameInfo("bloodied kithicor", "kithicor_alt", null, null, "kithicor"));
			ZoneNames.Add(new ZoneNameInfo("bloodied rivervale (instanced)", "rivervale_alt", null, null, "rivervale"));
			ZoneNames.Add(new ZoneNameInfo("howling stones (instanced)", "charasis_instanced", null, null, "charasis"));
			ZoneNames.Add(new ZoneNameInfo("chardok (instanced)", "chardok_instanced", null, null, "chardok"));
			ZoneNames.Add(new ZoneNameInfo("dreadlands (instanced)", "dreadlands_instanced", null, null, "dreadlands"));
			ZoneNames.Add(new ZoneNameInfo("emerald jungle (instanced)", "emeraldjungle_instanced", null, null, "emeraldjungle"));
			ZoneNames.Add(new ZoneNameInfo("timorous deep (instanced)", "timorous_instanced", null, null, "timorous"));
			ZoneNames.Add(new ZoneNameInfo("skyfire mountains (instanced)", "skyfire_instanced", null, null, "skyfire"));
			ZoneNames.Add(new ZoneNameInfo("ruins of sebilis (instanced)", "sebilis_instanced", null, null, "sebilis"));
			ZoneNames.Add(new ZoneNameInfo("karnor's castle (instanced)", "karnor_instanced", null, null, "karnor"));
			ZoneNames.Add(new ZoneNameInfo("veeshan's peak (instanced)", "veeshan_instanced", null, null, "veeshan"));
			ZoneNames.Add(new ZoneNameInfo("oops, all goos!", "plaguecrypt", null, null, "dalnir"));
			ZoneNames.Add(new ZoneNameInfo("kithicor forest (instanced)", "kithicor_instanced", null, null, "kithicor"));
			ZoneNames.Add(new ZoneNameInfo("the city of mist (instanced)", "citymist_instanced", null, null, "citymist"));

			#endregion

			Zones = ZoneNames.Select(a => a.MapName.ToLower()).ToList();
		}

		public static readonly List<string> Zones;
		public static string TranslateToMapName(string name)
		{
			name = name?.ToLower()?.Trim();
			if (string.IsNullOrWhiteSpace(name))
			{
				return string.Empty;
			}

			if (ZoneNames.Select(a => a.MapName).Contains(name))
			{
				return name;
			}

			if (ZoneNames.Select(a => a.WhoName).Contains(name))
			{
				//if(ZoneNames.Where(a => a.WhoName == name).Count() > 1)
				//{

				//}
				name = ZoneNames.First(a => a.WhoName == name).MapName;
			}
			else if (ZoneNames.Select(a => a.EnterName).Contains(name))
			{
				name = ZoneNames.First(a => a.EnterName == name).MapName;
			}

			return Zones.Any(a => a == name) ? name : string.Empty;
		}
		public static bool CheckWhoAgainstPreviousZone(string message, string name, string lastZone)
		{
			string messageZone = string.Empty;
			if (message.StartsWith(ThereAreNoPlayers) || message.StartsWith(YouHaveEnteredAreaPVP) || message.StartsWith(YouHaveEntered))
			{
				return false;
			}
			else if (message.StartsWith(ThereAre) || message.StartsWith(ThereIs))
			{
				message = message.Replace(ThereAre, string.Empty).Replace(ThereIs, string.Empty).Trim();
				var inindex = message.IndexOf(spaceinspace);
				if (inindex != -1)
				{
					message = message.Substring(inindex + spaceinspace.Length).Trim().TrimEnd('.').ToLower();
					if (message != "everquest")
					{
						messageZone = message;
					}
				}
			}
			if (string.IsNullOrWhiteSpace(messageZone))
			{
				return false;
			}
			else
			{
				if (ZoneNames.Where(a => a.WhoName == messageZone && (a.MapName == lastZone || a.EnterName == lastZone)).Count() > 1)
				{
					messageZone = ZoneNames.First(a => a.WhoName == messageZone && (a.MapName == lastZone || a.EnterName == lastZone)).MapName;
				}
				else
				{
					return false;
				}
			}

			return !string.IsNullOrWhiteSpace(messageZone);
		}

		public static PlayerZonedInfo Match(string message)
		{
			PlayerZonedInfo ret = new PlayerZonedInfo();
			if (string.IsNullOrWhiteSpace(message))
			{
				return null;
			}
			else if(message.Contains(" FORCECLEAR"))
			{
				ret.ForceUpdate = true;
				message = message.Replace(" FORCECLEAR", string.Empty);
			}
			//Alt/Instance Check
			var msgSplit = message.Split(' ');
			if(msgSplit.Length > 1 && message.StartsWith(YouHaveEntered) && (msgSplit.Last().Contains("(Inst") || msgSplit.Last().Contains("(Alt")))
			{
				ret.IsInstance = true;
			}

			if (message.StartsWith(ThereAreNoPlayers) || message.StartsWith(YouHaveEnteredAreaPVP))
			{
				return null;
			}
			else if (message.StartsWith(YouHaveEntered))
			{
				message = message.Replace(YouHaveEntered, string.Empty).Trim().TrimEnd('.').ToLower();
				ret.ZoneName = message;
				return ret;
			}
			else if (message.StartsWith(ThereAre))
			{
				message = message.Replace(ThereAre, string.Empty).Trim();
				var inindex = message.IndexOf(spaceinspace);
				if (inindex != -1)
				{
					message = message.Substring(inindex + spaceinspace.Length).Trim().TrimEnd('.').ToLower();
					if (message != "everquest")
					{
						ret.ZoneName = message;
						return ret;
					}
				}
			}
			else if (message.StartsWith(ThereIs))
			{
				message = message.Replace(ThereIs, string.Empty).Trim();
				var inindex = message.IndexOf(spaceinspace);
				if (inindex != -1)
				{
					message = message.Substring(inindex + spaceinspace.Length).Trim().TrimEnd('.').ToLower();
					if (message != "everquest")
					{
						ret.ZoneName = message;
						return ret;
					}
				}
			}

			return null;
		}
	}
}
