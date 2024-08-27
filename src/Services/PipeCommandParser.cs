using EQTool.EventArgModels;
using EQTool.Models;
using EQTool.Services.Map;
using EQTool.Services.Parsing;
using EQTool.Services.Spells.Log;
using EQTool.Utilities;
using EQTool.ViewModels;
using EQToolShared.HubModels;
using EQToolShared.Map;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Media.Media3D;
using static EQTool.Services.ChParser;
using static EQTool.Services.EnrageParser;
using static EQTool.Services.FindEq;
using static EQTool.Services.FTEParser;
using static EQTool.Services.InvisParser;
using static EQTool.Services.LevParser;
using static EQTool.Services.DTParser;
using static EQTool.Services.RandomParser;
using static EQTool.Services.ResistSpellParser;
using ZealPipes.Services;

namespace EQTool.Services
{
    public class PipeCommandParser : IDisposable
    {
        private System.Timers.Timer _uiTimer;
        private readonly ActivePlayer _activePlayer;
        private readonly IAppDispatcher _appDispatcher;
        private readonly EQToolSettings _settings;
        private readonly EQToolSettingsLoad _toolSettingsLoad;

		ZealMessageService _zealMessageService;


		public PipeCommandParser(
            EQToolSettingsLoad toolSettingsLoad,
            ActivePlayer activePlayer,
            IAppDispatcher appDispatcher,
            EQToolSettings settings,
			ZealMessageService zealMessageService
			)
        {
            _toolSettingsLoad = toolSettingsLoad;
            _activePlayer = activePlayer;
            _appDispatcher = appDispatcher;
            _settings = settings;
			_zealMessageService = zealMessageService;

			_zealMessageService.OnPipeCmdMessageReceived += _zealMessageService_OnPipeCmdMessageReceived;
		}

		private void _zealMessageService_OnPipeCmdMessageReceived(object sender, ZealMessageService.PipeCmdMessageReceivedEventArgs e)
		{
			if (e.Message != null && !string.IsNullOrWhiteSpace(e.Message.Data.Text))
			{
				if (LockCurrentCharacterCheck(e))
				{
					return;
				}
				else if (ToggleMap(e))
				{
					return;
				}
				else if (ToggleMobInfo(e))
				{
					return;
				}
				else if (ToggleOverlay(e))
				{
					return;
				}
				else if (ToggleAuraOverlay(e))
				{
					return;
				}
				else if(ToggleDPS(e))
				{
					return;
				}

				if(CreatePointOfInterest(e))
				{
					return;
				}

				//if(string.Compare(e.Message.Data.Text, "quto lock mobinfo", true) == 0)
				//{
				//	App.Current.Dispatcher.Invoke((Action)delegate
				//	{
				//		(App.Current as App).ToggleMobInfoWindowBorders();
				//	});
				//}
			}
		}

		private bool CreatePointOfInterest(ZealMessageService.PipeCmdMessageReceivedEventArgs e)
		{
			if(e.Message.Data.Text.StartsWith("quto poi add", StringComparison.OrdinalIgnoreCase))
			{
				string stripped = e.Message.Data.Text.Substring(12).Trim();
				string[] locParts = stripped.Split(',');
				Point3D loc;
				string label = locParts[0];
				if (stripped.Length == 0)
				{
					return false;
				}
				else
				{
					if (locParts.Length != 4)
					{
						return false;
					}
					double.TryParse(locParts[2], out double x);
					double.TryParse(locParts[1], out double y);
					double.TryParse(locParts[3], out double z);

					loc = new Point3D(x, y, z);
				}

				if (loc != null && label != string.Empty)
				{
					AddPointOfInterestEvent?.Invoke(this, new PointOfInterestEventArgs { Location = loc, Label = label });
					return true;
				}
			}
			return false;
		}

		private bool ToggleMap(ZealMessageService.PipeCmdMessageReceivedEventArgs e)
		{
			if (string.Compare(e.Message.Data.Text, "quto toggle map", true) == 0)
			{
				App.Current.Dispatcher.Invoke((Action)delegate
				{
					(App.Current as App).HardToggleMapWindow();
				});
				return true;
			}
			return false;
		}
		private bool ToggleMobInfo(ZealMessageService.PipeCmdMessageReceivedEventArgs e)
		{
			if (string.Compare(e.Message.Data.Text, "quto toggle mobinfo", true) == 0)
			{
				App.Current.Dispatcher.Invoke((Action)delegate
				{
					(App.Current as App).HardToggleMobInfoWindow();
				});
				return true;
			}
			return false;
		}
		private bool ToggleOverlay(ZealMessageService.PipeCmdMessageReceivedEventArgs e)
		{
			if (string.Compare(e.Message.Data.Text, "quto toggle overlay", true) == 0)
			{
				App.Current.Dispatcher.Invoke((Action)delegate
				{
					(App.Current as App).HardToggleOverlayWindow();
				});
				return true;
			}
			return false;
		}
		private bool ToggleAuraOverlay(ZealMessageService.PipeCmdMessageReceivedEventArgs e)
		{
			if (string.Compare(e.Message.Data.Text, "quto toggle auras", true) == 0)
			{
				App.Current.Dispatcher.Invoke((Action)delegate
				{
					(App.Current as App).HardToggleImageOverlayWindow();
				});
				return true;
			}
			return false;
		}
		private bool ToggleDPS(ZealMessageService.PipeCmdMessageReceivedEventArgs e)
		{
			if (string.Compare(e.Message.Data.Text, "quto toggle dps", true) == 0)
			{
				App.Current.Dispatcher.Invoke((Action)delegate
				{
					(App.Current as App).HardToggleDPSWindow();
				});
				return true;
			}
			return false;
		}
		private bool LockCurrentCharacterCheck(ZealMessageService.PipeCmdMessageReceivedEventArgs e)
		{
			if (e.Message.Data.Text.StartsWith("quto lock character", StringComparison.OrdinalIgnoreCase))
			{
				_settings.SelectedCharacter = e.Message.Character;
				_settings.ZealProcessID = e.ProcessId;
				return true;
			}
			return false;
		}

		public event EventHandler<PointOfInterestEventArgs> AddPointOfInterestEvent;
		public class PointOfInterestEventArgs : EventArgs
		{
			public Point3D Location { get; set; }
			public string Label { get; set; }
		}

		public void Dispose()
        {
			_zealMessageService.StopProcessing();
        }

		internal void Start()
		{
			_zealMessageService.StartProcessing();
		}
	}
}
