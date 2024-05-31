using Dapper;
using Dapper.Contrib.Extensions;
using EQTool.Models;
using EQToolShared.Enums;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EQTool.Services.FindEq;

namespace EQTool.Services
{
	public static class DataService
	{
		private const string _gameDataFileName = "Quarmtool_Data.db";
		private const string _userDataFileName = "Quarmtool_User.db";
		private static bool versionChecked = false;
		private static DataFileInfo _fileLocations;


		public static IEnumerable<T> GetData<T>(string query, object parameters = null)
		{
			QueryType _queryType;
			StringBuilder fileToUse = new StringBuilder();
			if(typeof(T) == typeof(QuarmMonster) 
				|| typeof(T) == typeof(QuarmMonsterFaction)
				|| typeof(T) == typeof(QuarmMonsterDrops)
				|| typeof(T) == typeof(QuarmMerchantItems)
				|| typeof(T) == typeof(QuarmMonsterTimer)
				|| typeof(T) == typeof(QuarmZone))
			{
				_queryType = QueryType.Data;
			}
			else
			{
				_queryType = QueryType.User;
			}

			if (_fileLocations == null && !DatabaseExists())
			{
				if(_fileLocations == null || string.IsNullOrWhiteSpace(_fileLocations?.User_File))
				{
					CreateDatabase(_userDataFileName);
				}
			}
			else if(!versionChecked && _queryType == QueryType.User)
			{
				CheckDatabaseVersion(_queryType);
			}

			if (_fileLocations != null)
			{
				string sqliteConnString = GetConnectionString(_queryType);
				using (SQLiteConnection cnn = new SQLiteConnection(sqliteConnString))
				{
					cnn.Open();
					if(parameters != null)
					{
						return cnn.Query<T>(query, parameters);
					}
					else
					{
						return cnn.Query<T>(query);
					}
				}
			}
			else
			{
				return null;
			}			
		}

		public static long Insert<T>(T obj)
			where T : class
		{
			QueryType _queryType;
			StringBuilder fileToUse = new StringBuilder();
			if (typeof(T) == typeof(QuarmMonster)
				|| typeof(T) == typeof(QuarmMonsterFaction)
				|| typeof(T) == typeof(QuarmMonsterDrops)
				|| typeof(T) == typeof(QuarmMerchantItems)
				|| typeof(T) == typeof(QuarmMonsterTimer)
				|| typeof(T) == typeof(QuarmZone))
			{
				_queryType = QueryType.Data;
			}
			else
			{
				_queryType = QueryType.User;
			}

			if (_fileLocations == null && !DatabaseExists())
			{
				if (_fileLocations != null && string.IsNullOrWhiteSpace(_fileLocations.User_File))
				{
					CreateDatabase(_userDataFileName);
				}
			}
			if (_fileLocations != null)
			{
				string sqliteConnString = GetConnectionString(_queryType);
				using (SQLiteConnection cnn = new SQLiteConnection(sqliteConnString))
				{
					cnn.Open();
					if (typeof(T) == typeof(CustomOverlay)
				|| typeof(T) == typeof(TimerWindowOptions))
					{
						return cnn.Insert(obj);
					}
				}
			}

			return 0;
		}

		public static bool Delete<T>(T obj)
			where T : class
		{
			QueryType _queryType;
			StringBuilder fileToUse = new StringBuilder();
			if (typeof(T) == typeof(QuarmMonster)
				|| typeof(T) == typeof(QuarmMonsterFaction)
				|| typeof(T) == typeof(QuarmMonsterDrops)
				|| typeof(T) == typeof(QuarmMerchantItems)
				|| typeof(T) == typeof(QuarmMonsterTimer)
				|| typeof(T) == typeof(QuarmZone))
			{
				_queryType = QueryType.Data;
			}
			else
			{
				_queryType = QueryType.User;
			}

			if (_fileLocations == null && !DatabaseExists())
			{
				if (_fileLocations != null && string.IsNullOrWhiteSpace(_fileLocations.User_File))
				{
					CreateDatabase(_userDataFileName);
				}
			}
			if (_fileLocations != null)
			{
				string sqliteConnString = GetConnectionString(_queryType);
				using (SQLiteConnection cnn = new SQLiteConnection(sqliteConnString))
				{
					cnn.Open();
					if (typeof(T) == typeof(CustomOverlay)
						|| typeof(T) == typeof(TimerWindowOptions))
					{
						return cnn.Delete(obj);
					}
				}
			}

			return false;
		}

		private static void CheckDatabaseVersion(QueryType queryType)
		{
			string sqliteConnString = GetConnectionString(queryType);
			using (SQLiteConnection cnn = new SQLiteConnection(sqliteConnString))
			{
				cnn.Open();
				long version = cnn.QuerySingle<long>("PRAGMA user_version");

				if (version < 1)
				{
					StringBuilder queryBuilder = new StringBuilder();
					//queryBuilder.AppendLine("CREATE TABLE IF NOT EXISTS CustomOverlays (ID INTEGER PRIMARY KEY, Trigger TEXT, Alternate_Trigger TEXT, Name TEXT, Message TEXT, DisplayColor TEXT, IsEnabled BOOL)");
					queryBuilder.AppendLine("PRAGMA user_version = 1;");
					queryBuilder.AppendLine("ALTER TABLE CustomOverlays ADD COLUMN AudioMessage TEXT;");
					queryBuilder.AppendLine("ALTER TABLE CustomOverlays ADD COLUMN IsAudioEnabled BOOL;");

					cnn.Execute(queryBuilder.ToString());
				}

				if (version == 1)
				{
					StringBuilder queryBuilder = new StringBuilder();
					queryBuilder.AppendLine("CREATE TABLE IF NOT EXISTS TimerWindows (ID INTEGER PRIMARY KEY, Title TEXT, BestGuessSpells BOOL, " +
							"ShowModRodTimers BOOL, ShowSpells BOOL, ShowTimers BOOL, ShowRandomRolls BOOL, YouOnlySpells BOOL, WindowRect TEXT, State INTEGER," +
							"Closed BOOL, AlwaysOnTop BOOL, Opacity DOUBLE);");

					cnn.Execute(queryBuilder.ToString());
				}


			}
			versionChecked = true;
		}

		public static bool UpdateCustomOverlay(CustomOverlay obj)
		{
			if (_fileLocations == null && !DatabaseExists())
			{
				if (_fileLocations != null && string.IsNullOrWhiteSpace(_fileLocations.User_File))
				{
					CreateDatabase(_userDataFileName);
				}
			}
			if (_fileLocations != null)
			{
				string sqliteConnString = GetConnectionString(QueryType.User);
				using (SQLiteConnection cnn = new SQLiteConnection(sqliteConnString))
				{
					cnn.Open();
					return cnn.Update<CustomOverlay>(obj);
				}
			}

			return false;
		}

		public static bool UpdateTimerWindow(TimerWindowOptions obj)
		{
			if (_fileLocations == null && !DatabaseExists())
			{
				if (_fileLocations != null && string.IsNullOrWhiteSpace(_fileLocations.User_File))
				{
					CreateDatabase(_userDataFileName);
				}
			}
			if (_fileLocations != null)
			{
				string sqliteConnString = GetConnectionString(QueryType.User);
				using (SQLiteConnection cnn = new SQLiteConnection(sqliteConnString))
				{
					cnn.Open();
					return cnn.Update<TimerWindowOptions>(obj);
				}
			}

			return false;
		}

		private static bool DatabaseExists()
		{
			_fileLocations = GetDataLocation("Data", _gameDataFileName, _userDataFileName);
			if (_fileLocations == null || !_fileLocations.Found || string.IsNullOrWhiteSpace(_fileLocations.User_File))
			{
				return false;
			}
			return true;
		}

		private static bool CreateDatabase(string fileName)
		{
			Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "Data"));
			SQLiteConnection.CreateFile(Path.Combine(AppContext.BaseDirectory, "Data", fileName));
			_fileLocations = GetDataLocation("Data", _gameDataFileName, _userDataFileName);
			if (_fileLocations == null || !_fileLocations.Found)
			{
				return false;
			}

			string sqliteConnString = GetConnectionString(QueryType.User);
			using (SQLiteConnection cnn = new SQLiteConnection(sqliteConnString))
			{
				cnn.Open();
				StringBuilder queryBuilder = new StringBuilder();
				if(fileName == _userDataFileName)
				{
					queryBuilder.AppendLine("CREATE TABLE IF NOT EXISTS CustomOverlays (ID INTEGER PRIMARY KEY, Trigger TEXT, Alternate_Trigger TEXT, " +
						"Name TEXT, Message TEXT, DisplayColor TEXT, IsEnabled BOOL, AudioMessage TEXT, IsAudioEnabled BOOL);");
					queryBuilder.AppendLine("CREATE TABLE IF NOT EXISTS TimerWindows (ID INTEGER PRIMARY KEY, Title TEXT, BestGuessSpells BOOL, " +
						"ShowModRodTimers BOOL, ShowSpells BOOL, ShowTimers BOOL, ShowRandomRolls BOOL, YouOnlySpells BOOL, WindowRect TEXT, State INTEGER," +
						"Closed BOOL, AlwaysOnTop BOOL, Opacity DOUBLE);");
					queryBuilder.AppendLine("PRAGMA user_version = 2;");
					cnn.Execute(queryBuilder.ToString());
				}
			}

			_fileLocations.User_File = Path.Combine(AppContext.BaseDirectory, "Data", fileName);
			versionChecked = true;
			return true;
		}

		private static string GetConnectionString(QueryType type)
		{
			if(type == QueryType.Data)
			{
				return $"Data Source={_fileLocations.Data_File};";
			}
			else if(type == QueryType.User)
			{
				return $"Data Source={_fileLocations.User_File};";
			}
			else
			{
				return string.Empty;
			}
		}
	}
}
