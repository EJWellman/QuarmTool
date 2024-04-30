using Dapper;
using Dapper.Contrib.Extensions;
using EQTool.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using static EQTool.Services.FindEq;

namespace EQTool.Services
{
	public class CustomOverlayService
	{
		private const string _fileName = "Quarmtool_User.db";
		private static DataFileInfo _fileLocations;

		public static bool AddNewCustomOverlay(CustomOverlay overlay)
		{
			_fileLocations = GetDataLocation("Data", _fileName);
			if (_fileLocations == null || !_fileLocations.Found)
			{
				CreateDatabase(_fileName);
			}

			string sqliteConnString = $"Data Source={_fileLocations.Data_File};";
			using (SQLiteConnection cnn = new SQLiteConnection(sqliteConnString))
			{
				try
				{
					cnn.Open();
					cnn.Insert(overlay);
				}
				catch (Exception ex)
				{
					return false;
				}
			}
			return true;
		}

		public static bool UpdateCustomOverlay(CustomOverlay overlay)
		{
			if(_fileLocations == null)
			{
				_fileLocations = GetDataLocation("Data", _fileName);
				if(_fileLocations == null)
				{
					return false;
				}
			}

			string sqliteConnString = $"Data Source={_fileLocations.Data_File};";
			using (SQLiteConnection cnn = new SQLiteConnection(sqliteConnString))
			{
				try
				{
					cnn.Open();
					cnn.Update<CustomOverlay>(overlay);
				}
				catch (Exception ex)
				{
					return false;
				}
			}
			return true;
		}

		public static List<CustomOverlay> LoadCustomOverlayMessages()
		{
			_fileLocations = GetDataLocation("Data", _fileName);
			if (_fileLocations == null || !_fileLocations.Found)
			{
				CreateDatabase(_fileName);
			}

			string sqliteConnString = $"Data Source={_fileLocations.Data_File};";
			using (SQLiteConnection cnn = new SQLiteConnection(sqliteConnString))
			{
				cnn.Open();
				List<CustomOverlay> ret = cnn.Query<CustomOverlay>("SELECT * FROM CustomOverlays").ToList();
				if(ret.Count > 0)
				{
					return ret;
				}
			}
			return null;
		}

		private static bool CreateDatabase(string fileName)
		{
			SQLiteConnection.CreateFile(Path.Combine(AppContext.BaseDirectory, "Data", fileName));
			_fileLocations = GetDataLocation("Data", fileName);
			if (_fileLocations == null || !_fileLocations.Found)
			{
				return false;
			}

			string sqliteConnString = $"Data Source={_fileLocations.Data_File};";
			using (SQLiteConnection cnn = new SQLiteConnection(sqliteConnString))
			{
				cnn.Open();
				string query = "CREATE TABLE IF NOT EXISTS CustomOverlays (ID INTEGER PRIMARY KEY, Trigger TEXT, Alternate_Trigger TEXT, Name TEXT, Message TEXT, DisplayColor TEXT, IsEnabled BOOL)";
				using (SQLiteCommand cmd = new SQLiteCommand(query, cnn))
				{
					cmd.ExecuteNonQuery();
				}
			}
			return true;
		}
	}
}
