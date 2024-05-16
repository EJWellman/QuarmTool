using Dapper;
using Dapper.Contrib.Extensions;
using EQTool.Models;
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
				fileToUse.Append(_gameDataFileName);
			}
			else if(typeof(T) == typeof(CustomOverlay))
			{
				fileToUse.Append(_userDataFileName);
			}

			if (_fileLocations == null && !DatabaseExists())
			{
				if(_fileLocations != null && string.IsNullOrWhiteSpace(_fileLocations.User_File))
				{
					CreateDatabase(_userDataFileName);
				}
			}
			else if(!versionChecked)
			{
				CheckDatabaseVersion();
			}
			if (_fileLocations != null)
			{
				string sqliteConnString = $"Data Source={Path.Combine(_fileLocations.Location, fileToUse.ToString())};";
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
				fileToUse.Append(_gameDataFileName);
			}
			else if (typeof(T) == typeof(CustomOverlay))
			{
				fileToUse.Append(_userDataFileName);
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
				string sqliteConnString = $"Data Source={fileToUse};";
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
			string sqliteConnString = $"Data Source={Path.Combine(_fileLocations.Location, _userDataFileName)};";
			using (SQLiteConnection cnn = new SQLiteConnection(sqliteConnString))
			{
				cnn.Open();
				long version = cnn.QuerySingle<long>("PRAGMA user_version");
				string blah = "";

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

		public bool Update(CustomOverlay obj)
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
				string sqliteConnString = $"Data Source={Path.Combine(_fileLocations.Location, _userDataFileName)};";
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

			string sqliteConnString = $"Data Source={_fileLocations.User_File};";
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
			return true;
		}
	}
}
