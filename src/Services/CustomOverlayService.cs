using EQTool.Models;
using System.Collections.Generic;
using System.Linq;

namespace EQTool.Services
{
	public class CustomOverlayService
	{
		public CustomOverlayService()
		{
		}


		public static bool AddNewCustomOverlay(CustomOverlay overlay)
		{
			long ret = DataService.Insert(overlay);
			return ret > 0;
		}

		public static bool UpdateCustomOverlay(CustomOverlay overlay)
		{
			return DataService.UpdateCustomOverlay(overlay);
		}

		public static bool DeleteCustomOverlay(CustomOverlay overlay)
		{
			return DataService.Delete(overlay);
		}

		public static List<CustomOverlay> LoadCustomOverlayMessages()
		{
			return DataService.GetData<CustomOverlay>("SELECT * FROM CustomOverlays").ToList();
		}
	}
}
