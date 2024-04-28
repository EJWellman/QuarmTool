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
	public class QuarmDataService
	{
		private readonly ActivePlayer _activePlayer;
		private static List<QuarmMonster> monsterTable;
		private static List<QuarmMonsterFaction> factionTable;
		private static List<QuarmMonsterDrops> dropsTable;
		private static List<QuarmMerchantItems> merchantTable;
		private static DataFileInfo FileLocations;
		public QuarmDataService(ActivePlayer activePlayer)
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

				var mobsTemp = cnn.Query<QuarmMonster>("SELECT * " +
					"FROM NPC"
					+ " WHERE Zone_Code = @Zone_Code", new { Zone_Code = zoneCode });

				var factionsTemp = cnn.Query<QuarmMonsterFaction>("SELECT * " +
					"FROM NPC_Factions"
					+ " WHERE Zone_Code = @Zone_Code", new { Zone_Code = zoneCode });

				var dropsTemp = cnn.Query<QuarmMonsterDrops>("SELECT * " +
					"FROM NPC_Drops"
					+ " WHERE Loottable_ID IN (SELECT Loottable_ID FROM NPC WHERE Zone_Code = @Zone_Code)", new { Zone_Code = zoneCode });

				var merchantItemsTemp = cnn.Query<QuarmMerchantItems>("SELECT * " +
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

		public QuarmMonster GetData(string name)
		{
			var currentzone = _activePlayer?.Player?.Zone;

			try
			{
				QuarmMonster matchedMonster = monsterTable.FirstOrDefault(r =>
					r.Name == name
					|| r.Name == name.Replace(' ', '_')
				);

				if (matchedMonster != null)
				{
					QuarmMonster match = matchedMonster;
					match.Name = match.Name.Trim().Replace('_', ' ');

					List<QuarmMonsterFaction> matchedFactions = factionTable.Where(f =>
						f.NPC_ID == match.ID).ToList();
					if (matchedFactions != null && matchedFactions.Count > 0)
					{
						match.Factions = matchedFactions;
					}
					List<QuarmMonsterDrops> matchedDrops = dropsTable.Where(d =>
						d.Loottable_ID == match.Loottable_ID).ToList();
					if (matchedDrops != null && matchedDrops.Count > 0)
					{
						match.Drops = matchedDrops;
					}
					if(merchantTable.Count > 0)
					{
						List<QuarmMerchantItems> matchedMerchantItems = merchantTable.Where(m =>
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
