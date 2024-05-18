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
	public class DataService
	{
		private const string _gameDataFileName = "Quarmtool_Data.db";
		private const string _userDataFileName = "Quarmtool_User.db";
		private static bool versionChecked = false;
		private DataFileInfo _fileLocations;

		private QueryType _queryType;

		public IEnumerable<T> GetData<T>(string query, object parameters = null)
		{
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
			else if(typeof(T) == typeof(CustomOverlay))
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
				CheckDatabaseVersion();
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

		public long Insert<T>(T obj)
			where T : class
		{
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
			else if (typeof(T) == typeof(CustomOverlay))
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
					if (typeof(T) == typeof(CustomOverlay))
					{
						return cnn.Insert(obj);
					}
				}
			}

			return 0;
		}

		private void CheckDatabaseVersion()
		{
			string sqliteConnString = GetConnectionString(_queryType);
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
			}
			versionChecked = true;
		}

		public bool UpdateCustomOverlay(CustomOverlay obj)
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


		private bool DatabaseExists()
		{
			_fileLocations = GetDataLocation("Data", _gameDataFileName, _userDataFileName);
			if (_fileLocations == null || !_fileLocations.Found || string.IsNullOrWhiteSpace(_fileLocations.User_File))
			{
				return false;
			}
			return true;
		}

		private bool CreateDatabase(string fileName)
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
					queryBuilder.AppendLine("CREATE TABLE IF NOT EXISTS CustomOverlays (ID INTEGER PRIMARY KEY, Trigger TEXT, Alternate_Trigger TEXT, Name TEXT, Message TEXT, DisplayColor TEXT, IsEnabled BOOL, AudioMessage TEXT, IsAudioEnabled BOOL);");
					queryBuilder.AppendLine("PRAGMA user_version = 1;");
					cnn.Execute(queryBuilder.ToString());
				}
			}

			_fileLocations.User_File = Path.Combine(AppContext.BaseDirectory, "Data", fileName);
			versionChecked = true;
			return true;
		}

		private string GetConnectionString(QueryType type)
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
