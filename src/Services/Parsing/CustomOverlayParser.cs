using Dapper;
using Dapper.Contrib.Extensions;
using EQTool.Models;
using EQTool.Properties;
using EQToolShared.HubModels;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static EQTool.Services.FindEq;

namespace EQTool.Services.Parsing
{
	public class CustomOverlayParser
	{
		private readonly CustomOverlayService _customOverlayService;
		private static EQToolSettings _settings;

		public CustomOverlayParser(CustomOverlayService customOverlayService,
			EQToolSettings settings)
		{
			_customOverlayService = customOverlayService;
			_settings = settings;
		}

		public static CustomOverlay Parse(string message)
		{
			if(_settings.CustomOverlays == null)
			{
				_settings.CustomOverlays = new EQToolShared.ExtendedClasses.ObservableCollectionRange<CustomOverlay>();
				_settings.CustomOverlays.AddRange(CustomOverlayService.LoadCustomOverlayMessages());
			}
			if(_settings.CustomOverlays.Count > 0)
			{
				foreach(var overlay in _settings.CustomOverlays)
				{
					if(overlay.IsEnabled && !string.IsNullOrWhiteSpace(overlay.Trigger) 
						&& !string.IsNullOrWhiteSpace(overlay.Message) && Regex.Matches(message, overlay.Trigger, RegexOptions.IgnoreCase).Count > 0)
					{
						return overlay;
					}
					if (overlay.IsEnabled && !string.IsNullOrWhiteSpace(overlay.Alternate_Trigger)
						&& !string.IsNullOrWhiteSpace(overlay.Message) && Regex.Matches(message, overlay.Alternate_Trigger, RegexOptions.IgnoreCase).Count > 0)
					{
						return overlay;
					}
				}
			}
			return null;
		}
	}
}
