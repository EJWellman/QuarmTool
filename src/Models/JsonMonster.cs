using EQTool.Services.Parsing;
using EQTool.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Markup;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace EQTool.Models
{
	public class JsonMonster
	{
		#region Properties
		public int AC { get; set; }
		public int CR { get; set; }
		public int DR { get; set; }
		public int FR { get; set; }
		public int MR { get; set; }
		public int PR { get; set; }
		public int HP { get; set; }
		public int ID { get; set; }
		public int Mana { get; set; }
		public string Name { get; set; }
		public string Race { get; set; }
		public int NPC_Class_ID { get; set; }
		public int Greed { get; set; }
		public int Level { get; set; }
		public int MaxDmg { get; set; }
		public int MinDmg { get; set; }
		public int IsQuestNPC { get; set; }
		public int MaxLevel { get; set; }
		public float RunSpeed { get; set; }
		public string Zone_Code { get; set; }
		public string Zone_Name { get; set; }
		public int Merchant_ID { get; set; }
		public int Attack_Count { get; set; }
		public int Attack_Delay { get; set; }
		public int Loottable_ID { get; set; }
		public int NPC_Spells_ID { get; set; }
		public int Mitigates_Slow { get; set; }
		public int NPC_Faction_ID { get; set; }
		public int Combat_HP_Regen { get; set; }
		public int Combat_Mana_Regen { get; set; }
		public string Primary_Faction { get; set; }
		public int Slow_Mitigation { get; set; }
		public int See_Invis { get; set; }
		public int See_Invis_Undead { get; set; }
		public int See_Sneak { get; set; }
		public int See_Improved_Hidee { get; set; }
		public string Special_Abilities { get; set; }
		public int Unique_Spawn_By_Name { get; set; }
		public string Zone_Name_Guess { get; set; }
		public string Zone_Code_Guess { get; set; }

		public List<JsonMonsterFaction> Factions { get; set; } = new List<JsonMonsterFaction>();
		public List<JsonMonsterDrops> Drops { get; set; } = new List<JsonMonsterDrops>();
		public List<JsonMerchantItems> MerchantItems { get; set; } = new List<JsonMerchantItems>();





		#endregion

		#region Parsers
		public string GetSpecialAttacks()
		{
			StringBuilder data = new StringBuilder();

			if (!string.IsNullOrWhiteSpace(this.Special_Abilities))
			{
				string[] attacks = Special_Abilities.Split('^');
				string seperator = "";
				string status = "0";
				foreach (var attack in attacks)
				{
					string[] values = attack.Split(',');
					string atk = values[0];
					if (values.Length > 1)
					{
						status = values[1];
					}
					else
					{
						status = "0";
					}
					if (status != "0")
					{
						switch (atk)
						{
							case "19":
								data.Append(seperator + "Immune to melee");
								seperator = ",";
								break;
							case "20":
								data.Append(seperator + "Immune to magic");
								seperator = ",";
								break;
							case "14":
								data.Append(seperator + "Uncharmable");
								seperator = ",";
								break;
							case "17":
								data.Append(seperator + "Unfearable");
								seperator = ",";
								break;
							case "2":
								data.Append(seperator + "Enrage");
								seperator = ",";
								break;
							case "5":
								data.Append(seperator + "Flurry");
								seperator = ",";
								break;
							case "21":
								data.Append(seperator + "Immune to fleeing");
								seperator = ",";
								break;
							case "16":
								data.Append(seperator + "Unsnarable");
								seperator = ",";
								break;
							case "13":
								data.Append(seperator + "Unmezzable");
								seperator = ",";
								break;
							case "15":
								data.Append(seperator + "Unstunnable");
								seperator = ",";
								break;
							case "22":
								data.Append(seperator + "Immune to melee except bane");
								seperator = ",";
								break;
							case "7":
								data.Append(seperator + "Dual Wield");
								seperator = ",";
								break;
							case "3":
								data.Append(seperator + "Rampage");
								seperator = ",";
								break;
							case "4":
								data.Append(seperator + "Area Rampage");
								seperator = ",";
								break;
							case "1":
								data.Append(seperator + "Summon");
								seperator = ",";
								break;
							case "6":
								data.Append(seperator + "Triple Attack");
								seperator = ",";
								break;
							case "12":
								data.Append(seperator + "Unslowable");
								seperator = ",";
								break;
							case "23":
								data.Append(seperator + "Immune to melee except magical");
								seperator = ",";
								break;
							case "8":
								data.Append(seperator + "Do Not Equip");
								seperator = ",";
								break;
							case "9":
								data.Append(seperator + "Bane Attack");
								seperator = ",";
								break;
							case "10":
								data.Append(seperator + "Magical Attack");
								seperator = ",";
								break;
							case "11":
								data.Append(seperator + "Ranged Attack");
								seperator = ",";
								break;
							case "18":
								data.Append(seperator + "Immune to Dispell");
								seperator = ",";
								break;
							case "24":
								data.Append(seperator + "Will not Aggro");
								seperator = ",";
								break;
							case "25":
								data.Append(seperator + "Immune to Aggro");
								seperator = ",";
								break;
							case "26":
								data.Append(seperator + "Resistant to Ranged Spells");
								seperator = ",";
								break;
							case "27":
								data.Append(seperator + "Sees through Feign Death");
								seperator = ",";
								break;
							case "28":
								data.Append(seperator + "Immune to Taunt");
								seperator = ",";
								break;
							case "29":
								data.Append(seperator + "Tunnel Vision");
								seperator = ",";
								break;
							case "30":
								data.Append(seperator + "Does NOT buff/heal friends");
								seperator = ",";
								break;
							case "31":
								data.Append(seperator + "Immune to lull effects");
								seperator = ",";
								break;
							case "32":
								data.Append(seperator + "Leashed");
								seperator = ",";
								break;
							case "33":
								data.Append(seperator + "Tethered");
								seperator = ",";
								break;
							case "34":
								data.Append(seperator + "Permaroot Flee");
								seperator = ",";
								break;
							case "35":
								data.Append(seperator + "No Harm from Players");
								seperator = ",";
								break;
							case "36":
								data.Append(seperator + "Always flees");
								seperator = ",";
								break;
							case "37":
								data.Append(seperator + "Flee percentage");
								seperator = ",";
								break;
							case "38":
								data.Append(seperator + "Allow beneficial spells from players");
								seperator = ",";
								break;
							case "39":
								data.Append(seperator + "Unable to Melee");
								seperator = ",";
								break;
							case "40":
								data.Append(seperator + "Chase Distance");
								seperator = ",";
								break;
							case "41":
								data.Append(seperator + "Allow Tank");
								seperator = ",";
								break;
							case "42":
								data.Append(seperator + "Proximity Aggro");
								seperator = ",";
								break;
							case "43":
								data.Append(seperator + "Always Calls for Help");
								seperator = ",";
								break;
							case "44":
								data.Append(seperator + "Use Warrior Skills");
								seperator = ",";
								break;
							case "45":
								data.Append(seperator + "Always Flee Low Con");
								seperator = ",";
								break;
							case "46":
								data.Append(seperator + "NO LOITERING");
								seperator = ",";
								break;
							case "47":
								data.Append(seperator + "Bad Faction Block Handin");
								seperator = ",";
								break;
							case "48":
								data.Append(seperator + "PC Deathblow Corpse");
								seperator = ",";
								break;
							case "49":
								data.Append(seperator + "Corpse Camper");
								seperator = ",";
								break;
							case "50":
								data.Append(seperator + "Reverse Slow");
								seperator = ",";
								break;
							case "51":
								data.Append(seperator + "No Haste");
								seperator = ",";
								break;
							case "52":
								data.Append(seperator + "Immune to Disarm");
								seperator = ",";
								break;
							case "53":
								data.Append(seperator + "Immune to Riposte");
								seperator = ",";
								break;
							case "54":
								data.Append(seperator + "Proximity Aggro");
								seperator = ",";
								break;
							case "55":
								data.Append(seperator + "Max Special Attack");
								seperator = ",";
								break;
							default:
								data.Append("None");
								break;
						}
					}
					else
					{
						data.Append("None");
					}
				};
			}
			else
			{
				data.Append("None");
			}

			return data.ToString();
		}
		#endregion
	}
}
