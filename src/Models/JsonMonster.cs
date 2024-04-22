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
		public int hp { get; set; }
		public int id { get; set; }
		public int mana { get; set; }
		public string name { get; set; }
		public string race { get; set; }
		public int npc_class_id { get; set; }
		public int greed { get; set; }
		public int level { get; set; }
		public int maxdmg { get; set; }
		public int mindmg { get; set; }
		public int isquest { get; set; }
		public int maxlevel { get; set; }
		public float runspeed { get; set; }
		public int see_invis { get; set; }
		public int see_sneak { get; set; }
		public string zone_code { get; set; }
		public string zone_name { get; set; }
		public int merchant_id { get; set; }
		public int attack_count { get; set; }
		public int attack_delay { get; set; }
		public int loottable_id { get; set; }
		public int npc_spells_id { get; set; }
		public int mitigates_slow { get; set; }
		public int npc_faction_id { get; set; }
		public int combat_hp_regen { get; set; }
		public string primary_faction { get; set; }
		public int slow_mitigation { get; set; }
		public int see_invis_undead { get; set; }
		public int combat_mana_regen { get; set; }
		public int see_improved_hide { get; set; }
		public string special_abilities { get; set; }
		public int unique_spawn_by_name { get; set; }
		public string zone_name_guess { get; set; }
		public string zone_code_guess { get; set; }

		public List<JsonMonsterFaction> Factions { get; set; } = new List<JsonMonsterFaction>();
		public List<JsonMonsterDrops> Drops { get; set; } = new List<JsonMonsterDrops>();
		public List<JsonMerchantItems> MerchantItems { get; set; } = new List<JsonMerchantItems>();





		#endregion

		#region Parsers
		public string GetSpecialAttacks()
		{
			StringBuilder data = new StringBuilder();

			if (!string.IsNullOrWhiteSpace(this.special_abilities))
			{
				string[] attacks = special_abilities.Split('^');
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
