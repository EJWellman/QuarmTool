using Dapper;
using EQTool.Constants;
using EQTool.Models;
using EQTool.Utilities;
using EQTool.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using static EQTool.Services.FindEq;

namespace EQTool.Services
{
	public class QuarmDataService
	{
		private readonly ActivePlayer _activePlayer;
		private static List<QuarmMonster> _monsters;
		private static List<QuarmMonsterFaction> _factions;
		private static List<QuarmMonsterDrops> _drops;
		private static List<QuarmMerchantItems> _merchantWares;
		private static List<QuarmMonsterTimer> _monsterTimers;
		private static QuarmZone _currentZone;
		private static DataFileInfo _fileLocations;
		public QuarmDataService(ActivePlayer activePlayer)
		{
			_activePlayer = activePlayer;
		}

		public bool LoadMobDataForZone(string zoneCode)
		{
			string likeZoneCode1 = "%" + zoneCode + "^%";
			string likeZoneCode2 = "%^" + zoneCode + "%";

			var mobsTemp = DataService.GetData<QuarmMonster>("SELECT * " +
				"FROM NPC " +
				"WHERE (Zone_Code LIKE @Zone_CodeLike1" +
				"	OR Zone_Code LIKE @Zone_CodeLike2" +
				"	OR Zone_Code = @Zone_Code" +
				"	OR Zone_Code_Guess = @Zone_Code)", new { Zone_CodeLike1 = likeZoneCode1, Zone_CodeLike2 = likeZoneCode2, Zone_Code = zoneCode });

			var factionsTemp = DataService.GetData<QuarmMonsterFaction>("SELECT * " +
				"FROM NPC_Factions"
				+ " WHERE NPC_ID IN (" + string.Join(",", mobsTemp.Select(m => m.ID)) + ");");

			var dropsTemp = DataService.GetData<QuarmMonsterDrops>("SELECT * " +
				"FROM NPC_Drops"
				+ " WHERE Loottable_ID IN (" + string.Join(",", mobsTemp.Select(m => m.Loottable_ID)) + ");");

			var merchantItemsTemp = DataService.GetData<QuarmMerchantItems>("SELECT * " +
				"FROM NPC_Wares"
				+ " WHERE MerchantID IN (" + string.Join(",", mobsTemp.Select(m => m.Merchant_ID)) + ");");

			var timersTemp = DataService.GetData<QuarmMonsterTimer>("SELECT * " +
				"FROM NPC_RespawnTimers"
				+ " WHERE Zone_Code LIKE @Zone_CodeLike1" +
				"	OR Zone_Code LIKE @Zone_CodeLike2" +
				"	OR Zone_Code = @Zone_Code", new { Zone_CodeLike1 = likeZoneCode1, Zone_CodeLike2 = likeZoneCode2, Zone_Code = zoneCode });

			var tempZone = DataService.GetData<QuarmZone>("SELECT * " +
				"FROM Zones"
				+ " WHERE Code = @Zone_Code", new { Zone_Code = zoneCode }).FirstOrDefault();

			_monsters = mobsTemp.ToList();
			_factions = factionsTemp.ToList();
			_drops = dropsTemp.ToList();
			_merchantWares = merchantItemsTemp.ToList();
			_monsterTimers = timersTemp.ToList();
			_currentZone = tempZone;

			if(_monsters.Count > 0)
			{
				return true;
			}

			return false;
		}

		public QuarmMonster GetData(string name)
		{
			var currentzone = _activePlayer?.Player?.Zone;

			try
			{
				QuarmMonster matchedMonster = _monsters.FirstOrDefault(r =>
					r.Name == name.Trim()
					|| r.Name == name.Trim().Replace(' ', '_')
					|| r.Name == name.Trim().Replace('_', ' ')
				);

				if (matchedMonster != null)
				{
					QuarmMonster match = matchedMonster;
					match.Name = match.Name.Trim().Replace('_', ' ');

					List<QuarmMonsterFaction> matchedFactions = _factions.Where(f =>
						f.NPC_ID == match.ID).ToList();
					if (matchedFactions != null && matchedFactions.Count > 0)
					{
						match.Factions = matchedFactions;
					}
					List<QuarmMonsterDrops> matchedDrops = _drops.Where(d =>
						d.Loottable_ID == match.Loottable_ID).ToList();
					if (matchedDrops != null && matchedDrops.Count > 0)
					{
						match.Drops = matchedDrops;
					}
					if(_merchantWares.Count > 0)
					{
						List<QuarmMerchantItems> matchedMerchantItems = _merchantWares.Where(m =>
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

		public QuarmMonster GetSlimData(string name)
		{
			var currentzone = _activePlayer?.Player?.Zone;

			try
			{
				QuarmMonster matchedMonster = _monsters.FirstOrDefault(r =>
					r.Name == name.Trim()
					|| r.Name == name.Trim().Replace(' ', '_')
					|| r.Name == name.Trim().Replace('_', ' ')
				);

				if (matchedMonster != null)
				{
					return matchedMonster;
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

		public QuarmMonsterTimer GetMonsterTimer(string name)
		{
			QuarmMonsterTimer timer = _monsterTimers.FirstOrDefault(t => t.Mob_Name == name || t.Mob_Name == name.Replace(" ", "_"))?.ShallowClone();
			if (timer == null)
			{
				timer = new QuarmMonsterTimer()
				{
					Mob_Name = name,
					Min_RespawnTimer =  QuarmRules.GetRespawnReductionDungeonHigherBoundMin(),
					Max_RespawnTimer = QuarmRules.GetRespawnReductionDungeonHigherBoundMin()
				};
			}

			if (timer.Min_RespawnTimer == timer.Max_RespawnTimer)
			{
				timer.RespawnTimer = timer.Min_RespawnTimer; 
			}

			if (_currentZone.HasReducedSpawnTimers)
			{
				timer.RespawnTimer = timer.Min_RespawnTimer;

				var spawnTimer = timer.Min_RespawnTimer;
				if (_currentZone.IsDungeon)
				{
					if (timer.RespawnTimer >= QuarmRules.GetRespawnReductionDungeonHigherBoundMin() && timer.RespawnTimer <= QuarmRules.GetRespawnReductionDungeonHigherBoundMax())
					{
						timer.RespawnTimer = QuarmRules.GetRespawnReductionDungeonHigherBound();
						timer.Min_RespawnTimer = QuarmRules.GetRespawnReductionDungeonHigherBound();
						timer.Max_RespawnTimer = QuarmRules.GetRespawnReductionDungeonHigherBound();
					}
					else if (timer.RespawnTimer >= QuarmRules.GetRespawnReductionDungeonLowerBoundMin() && timer.RespawnTimer <= QuarmRules.GetRespawnReductionDungeonLowerBoundMax())
					{
						timer.RespawnTimer = QuarmRules.GetRespawnReductionDungeonLowerBound();
						timer.Min_RespawnTimer = QuarmRules.GetRespawnReductionDungeonLowerBound();
						timer.Max_RespawnTimer = QuarmRules.GetRespawnReductionDungeonLowerBound();
					}
				}
				else
				{
					var mob = GetSlimData(name.Replace(" ", "_"));
					if (mob.Level > 0 && mob.MaxLevel < 15)
					{
						if (timer.RespawnTimer >= QuarmRules.GetRespawnReductionHigherBoundMin() && timer.RespawnTimer <= QuarmRules.GetRespawnReductionHigherBoundMax())
						{
							timer.RespawnTimer = QuarmRules.GetRespawnReductionHigherBound();
							timer.Min_RespawnTimer = QuarmRules.GetRespawnReductionHigherBound();
							timer.Max_RespawnTimer = QuarmRules.GetRespawnReductionHigherBound();
						}
						else if (timer.RespawnTimer >= QuarmRules.GetRespawnReductionLowerBoundMin() && timer.RespawnTimer <= QuarmRules.GetRespawnReductionLowerBoundMax())
						{
							timer.RespawnTimer = QuarmRules.GetRespawnReductionLowerBound();
							timer.Min_RespawnTimer = QuarmRules.GetRespawnReductionLowerBound();
							timer.Max_RespawnTimer = QuarmRules.GetRespawnReductionLowerBound();
						}
					}
				}
			}

			if (timer != null)
			{
				return timer;
			}
			else
			{
				return null;
			}
		}

		public bool DoesMonsterExistInZone(string mobName)
		{
			if(_monsters != null && _monsters.Count > 0)
			{
				return _monsters.Any(m => m.Name.Replace("_"," ") == mobName);
			}
			else
			{
				return false;
			}
		}

		public QuarmZone GetZoneByID(int zoneId)
		{
			var zone = DataService.GetData<QuarmZone>("SELECT * " +
				"FROM Zones"
				+ " WHERE Zone_ID = @Zone_ID", new { Zone_ID = zoneId}).FirstOrDefault();

			return zone;
		}
	}
}
