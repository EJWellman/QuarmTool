using Dapper;
using EQTool.Models;
using EQTool.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Net;
using System.Net.Http;
using static EQTool.Services.FindEq;

namespace EQTool.Services
{
	public class JsonDataService
	{
		private readonly ActivePlayer _activePlayer;
		private static List<JsonMonster> monsterTable;
		private static List<JsonMonsterFaction> factionTable;
		private static List<JsonMonsterDrops> dropsTable;
		private static List<JsonMerchantItems> merchantTable;
		private static DataFileInfo FileLocations;
		public JsonDataService(ActivePlayer activePlayer)
		{
			this._activePlayer = activePlayer;
		}

		public bool LoadMobDataForZone(string zoneCode/*, string lastZoneEntered*/)
		{
			FileLocations = GetDataLocation();
			if(FileLocations == null || !FileLocations.Found)
			{
				return false;
			}

			string sqliteConnString = $"Data Source={FileLocations.Data_File};";
			using (SQLiteConnection cnn = new SQLiteConnection(sqliteConnString))
			{
				cnn.Open();

				var mobsTemp = cnn.Query<JsonMonster>("SELECT * " +
					"FROM NPC"
					+ " WHERE Zone_Code = @Zone_Code", new { Zone_Code = zoneCode });

				var factionsTemp = cnn.Query<JsonMonsterFaction>("SELECT * " +
					"FROM NPC_Factions"
					+ " WHERE Zone_Code = @Zone_Code", new { Zone_Code = zoneCode });

				var dropsTemp = cnn.Query<JsonMonsterDrops>("SELECT * " +
					"FROM NPC_Drops"
					+ " WHERE Loottable_ID IN (SELECT Loottable_ID FROM NPC WHERE Zone_Code = @Zone_Code)", new { Zone_Code = zoneCode });

				var merchantItemsTemp = cnn.Query<JsonMerchantItems>("SELECT * " +
					"FROM NPC_Wares"
					+ " WHERE MerchantID IN (SELECT Merchant_ID FROM NPC WHERE Zone_Code = @Zone_Code)", new { Zone_Code = zoneCode });

				monsterTable = mobsTemp.ToList();
				factionTable = factionsTemp.ToList();
				dropsTable = dropsTemp.ToList();
				merchantTable = merchantItemsTemp.ToList();

				if(monsterTable.Count > 0)
				{
					return true;
				}
			}

			return false;
		}

		public JsonMonster GetData(string name)
		{
			var currentzone = _activePlayer?.Player?.Zone;

			try
			{
				JsonMonster matchedMonster = monsterTable.FirstOrDefault(r =>
					r.Name == name
					|| r.Name == name.Replace(' ', '_')
				);

				if (matchedMonster != null)
				{
					JsonMonster match = matchedMonster;
					match.Name = match.Name.Trim().Replace('_', ' ');

					List<JsonMonsterFaction> matchedFactions = factionTable.Where(f =>
						f.NPC_ID == match.ID).ToList();
					if (matchedFactions != null && matchedFactions.Count > 0)
					{
						match.Factions = matchedFactions;
					}
					List<JsonMonsterDrops> matchedDrops = dropsTable.Where(d =>
						d.Loottable_ID == match.Loottable_ID).ToList();
					if (matchedDrops != null && matchedDrops.Count > 0)
					{
						match.Drops = matchedDrops;
					}
					if(merchantTable.Count > 0)
					{
						List<JsonMerchantItems> matchedMerchantItems = merchantTable.Where(m =>
							m.MerchantID == match.Merchant_ID).ToList();
						if (matchedMerchantItems != null && matchedMerchantItems.Count > 0)
						{
							match.MerchantItems = matchedMerchantItems;
						}
					}

					return match;
				}

			}
			catch (AggregateException er)
			{
				if (er.InnerException != null && er.InnerException.GetType() == typeof(HttpRequestException))
				{
					var err = er.InnerException as HttpRequestException;
					if (err.InnerException?.GetType() == typeof(WebException))
					{
						var innererr = err.InnerException as WebException;
						throw new Exception(innererr.Message);
					}
					else
					{
						throw new Exception(err.Message);
					}
				}
			}
			catch (Exception e)
			{
				var msg = $"Zone: {name} ";
				throw new Exception(msg + e.Message);
			}

			return null;
		}
	}
}
