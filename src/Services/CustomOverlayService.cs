using Dapper;
using Dapper.Contrib.Extensions;
using EQTool.Models;
using EQTool.ViewModels;
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
		private static DataService _dataService;

		public CustomOverlayService(DataService dataService)
		{
			_dataService = dataService;
		}


		public static bool AddNewCustomOverlay(CustomOverlay overlay)
		{
			long ret = _dataService.Insert(overlay);
			return ret > 0;
		}

		public static bool UpdateCustomOverlay(CustomOverlay overlay)
		{
			return _dataService.UpdateCustomOverlay(overlay);
		}

		public static bool DeleteCustomOverlay(CustomOverlay overlay)
		{
			return _dataService.Delete(overlay);
		}

		public static List<CustomOverlay> LoadCustomOverlayMessages()
		{
			return _dataService.GetData<CustomOverlay>("SELECT * FROM CustomOverlays").ToList();
		}
	}
}
