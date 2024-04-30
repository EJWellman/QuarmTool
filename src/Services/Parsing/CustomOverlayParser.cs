using Dapper;
using Dapper.Contrib.Extensions;
using EQTool.Models;
using EQToolShared.HubModels;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using static EQTool.Services.FindEq;

namespace EQTool.Services.Parsing
{
	public class CustomOverlayParser
	{
		private readonly CustomOverlayService _customOverlayService;
		private static List<CustomOverlay> _customOverlays;

		public CustomOverlayParser(CustomOverlayService customOverlayService)
		{
			_customOverlayService = customOverlayService;
		}

		public static CustomOverlay Parse(string message)
		{
			if(_customOverlays == null)
			{
				_customOverlays = CustomOverlayService.LoadCustomOverlayMessages();
			}
			if(_customOverlays.Count > 0)
			{
				foreach(var overlay in _customOverlays)
				{
					if(overlay.Trigger.Contains(message)
						|| overlay.Alternate_Trigger.Contains(message))
					{
						return overlay;
					}
				}
			}
			return null;
		}
	}
}
