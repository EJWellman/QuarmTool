using EQTool.Models;
using EQTool.Tools;
using EQTool.ViewModels;
using EQToolShared.Map;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using static EQTool.Services.FindEq;

namespace EQTool.Services
{
	public class JsonDataService
	{
		private readonly ActivePlayer _activePlayer;
		private static DataTable monsterTable;
		private static DataTable factionTable;
		private static DataTable dropsTable;
		private static DataTable merchantTable;
		private static DataFileInfo FileLocations;
		public JsonDataService(ActivePlayer activePlayer)
		{
			this._activePlayer = activePlayer;
		}

		public bool LoadMonsterDataTable(string zoneCode/*, string lastZoneEntered*/)
		{
			FileLocations = GetDataLocation();
			if(FileLocations == null)
			{
				return false;
			}
			FileStream fs = new FileStream(FileLocations.NPC_File, FileMode.Open, FileAccess.Read);

			string jsonContents;
			using (StreamReader sr = new StreamReader(fs))
			{
				jsonContents = sr.ReadToEnd();
			}

			List<JsonMonster> monsterList = JsonConvert.DeserializeObject<List<JsonMonster>>(jsonContents);
			DataTable tempMonsterTable = monsterList.ToDataTable();
			string columnFilter = "zone_code";
			string backupColumnFilter = "zone_code_guess";

			monsterTable = tempMonsterTable.AsEnumerable().Where(r =>
				(r.Field<string>(columnFilter) != null 
					&& r.Field<string>(columnFilter).Contains(zoneCode))
				|| 
				(r.Field<string>(columnFilter) == null
					&& r.Field<string>(backupColumnFilter).Contains(zoneCode))
			).CopyToDataTable();

			if (monsterTable.Rows.Count > 0)
			{
				//If monsters were found, load additional data
				bool factionsLoaded = LoadFactionDataTable(zoneCode);
				List<int> lootTableIds = monsterTable.AsEnumerable().Select(r => r.Field<int>("loottable_id")).ToList();
				bool dropsLoaded = LoadDropsDataTable(lootTableIds);
				List<int> merchantIds = monsterTable.AsEnumerable().Select(r => r.Field<int>("merchant_id")).Where(r => r != 0).ToList();
				bool merchantInfoLoaded = LoadMerchantsDataTable(merchantIds);

				return true;
			}

			return false;
		}

		private bool LoadFactionDataTable(string zoneCode)
		{
			FileStream fs = new FileStream(FileLocations.Faction_File, FileMode.Open, FileAccess.Read);

			string jsonContents;
			using (StreamReader sr = new StreamReader(fs))
			{
				jsonContents = sr.ReadToEnd();
			}

			List<JsonMonsterFaction> factionList = JsonConvert.DeserializeObject<List<JsonMonsterFaction>>(jsonContents);
			DataTable tempFactionTable = factionList.ToDataTable();
			string columnFilter = "zone_code";

			var tempResults  = tempFactionTable.AsEnumerable().Where(r =>
				r.Field<string>(columnFilter) == zoneCode
			);

			if (tempResults.Count() != 0)
			{
				factionTable = tempResults.CopyToDataTable();
			}
			else
			{
				factionTable = new DataTable();
			}

			if (factionTable.Rows.Count > 0)
			{
				return true;
			}

			return false;
		}

		private bool LoadDropsDataTable(List<int> loottableIds)
		{
			FileStream fs = new FileStream(FileLocations.Loot_File, FileMode.Open, FileAccess.Read);

			string jsonContents;
			using (StreamReader sr = new StreamReader(fs))
			{
				jsonContents = sr.ReadToEnd();
			}

			List<JsonMonsterDrops> dropsList = JsonConvert.DeserializeObject<List<JsonMonsterDrops>>(jsonContents);
			DataTable tempDropsTable = dropsList.ToDataTable();
			string columnFilter = "loottable_id";

			var tempResults = tempDropsTable.AsEnumerable().Where(r =>
				loottableIds.Contains(r.Field<int>(columnFilter))
			);

			if (tempResults.Count() != 0)
			{
				dropsTable = tempResults.CopyToDataTable();
			}
			else
			{
				dropsTable = new DataTable();
			}

			if (dropsTable.Rows.Count > 0)
			{
				return true;
			}

			return false;
		}

		private bool LoadMerchantsDataTable(List<int> merchantIds)
		{
			FileStream fs = new FileStream(FileLocations.Merchant_File, FileMode.Open, FileAccess.Read);

			string jsonContents;
			using (StreamReader sr = new StreamReader(fs))
			{
				jsonContents = sr.ReadToEnd();
			}

			List<JsonMerchantItems> merchantList = JsonConvert.DeserializeObject<List<JsonMerchantItems>>(jsonContents);
			DataTable tempMerchantTable = merchantList.ToDataTable();
			string columnFilter = "merchantid";

			var tempResults = tempMerchantTable.AsEnumerable().Where(r =>
				merchantIds.Contains(r.Field<int>(columnFilter))
			);

			if (tempResults.Count() != 0)
			{
				merchantTable = tempResults.CopyToDataTable();
			}
			else
			{
				merchantTable = new DataTable();
			}


			if (merchantTable != null &&  merchantTable.Rows.Count > 0)
			{
				return true;
			}

			return false;
		}

		public JsonMonster GetData(string name)
		{
			var currentzone = _activePlayer?.Player?.Zone;

			try
			{
				//string encodedName = HttpUtility.UrlEncode(name.Trim().Replace(' ', '_'));
				name = name.Trim().Replace(' ', '_');

				DataRow matchedMonster = monsterTable.AsEnumerable().FirstOrDefault(r =>
					r.Field<string>("name") == name
				);

				if (matchedMonster != null)
				{
					JsonMonster match = DataTableConverter.CreateItemFromRow<JsonMonster>(matchedMonster);
					match.name = match.name.Trim().Replace('_', ' ');

					List<JsonMonsterFaction> matchedFactions = factionTable.AsEnumerable().Where(f =>
						f.Field<int>("npc_id") == match.id)
						.Select(row => new JsonMonsterFaction
						{
							npc_id = (int)row["npc_id"],
							zone_code = (string)row["zone_code"],
							faction_id = (int)row["faction_id"],
							faction_name = (string)row["faction_name"],
							faction_hit = (int)row["faction_hit"]
						}
					).ToList();
					if (matchedFactions != null && matchedFactions.Count > 0)
					{
						match.Factions = matchedFactions;
					}
					List<JsonMonsterDrops> matchedDrops = dropsTable.AsEnumerable().Where(d =>
						d.Field<int>("loottable_id") == match.loottable_id)
						.Select(row => new JsonMonsterDrops
						{
							loottable_id = (int)row["loottable_id"],
							drop_chance = (float)row["drop_chance"],
							item_id = (int)row["item_id"],
							item_name = (string)row["item_name"]
						}
					).ToList();
					if (matchedDrops != null && matchedDrops.Count > 0)
					{
						match.Drops = matchedDrops;
					}
					if(merchantTable is DataTable)
					{
						List<JsonMerchantItems> matchedMerchantItems = merchantTable.AsEnumerable().Where(m =>
							m.Field<int>("merchantid") == match.merchant_id)
							.Select(row => new JsonMerchantItems
							{
								merchantid = (int)row["merchantid"],
								item_id = (int)row["item_id"],
								item_name = (string)row["item_name"],
								slot = (int)row["slot"]
							}
						).ToList();
						if (matchedMerchantItems != null && matchedMerchantItems.Count > 0)
						{
							match.MerchantItems = matchedMerchantItems;
						}
					}

					return match;
				}

			}
			catch (System.AggregateException er)
			{
				if (er.InnerException != null && er.InnerException.GetType() == typeof(HttpRequestException))
				{
					var err = er.InnerException as HttpRequestException;
					if (err.InnerException?.GetType() == typeof(WebException))
					{
						var innererr = err.InnerException as WebException;
						throw new System.Exception(innererr.Message);
					}
					else
					{
						throw new System.Exception(err.Message);
					}
				}
			}
			catch (Exception e)
			{
				var msg = $"Zone: {name} ";
				throw new System.Exception(msg + e.Message);
			}

			return null;
		}
	}
}
