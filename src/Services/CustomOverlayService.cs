using EQTool.Models;
using System.Collections.Generic;
using System.Linq;

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
