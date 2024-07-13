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
using static ZealPipes.Services.ZealMessageService;

namespace EQTool.Services
{
	public class PipeParser : IDisposable
	{
		private readonly EQSpells _spells;

		private System.Timers.Timer _uiTimer;
		private readonly ActivePlayer _activePlayer;
		private readonly IAppDispatcher _appDispatcher;
		private string _lastLogFilename = string.Empty;
		private readonly EQToolSettings _settings;
		private readonly LevelLogParse _levelLogParse;
		private readonly EQToolSettingsLoad _toolSettingsLoad;
		private readonly ISignalrPlayerHub _signalrPlayerHub;

		private bool StartingWhoOfZone = false;
		private bool Processing = false;
		private bool StillCamping = false;
		private bool HasUsedStartupEnterWorld = false;
		public bool JustZoned = false;


		ZealMessageService _zealMessageService;


		public PipeParser(
			ResistSpellParser resistSpellParser,
			LogCustomTimer logCustomTimer,
			SpellWornOffLogParse spellWornOffLogParser,
			SpellLogParse spellLogParser,
			EQToolSettingsLoad toolSettingsLoad,
			ActivePlayer activePlayer,
			IAppDispatcher appDispatcher,
			EQToolSettings settings,
			EQSpells spells,
			ZealMessageService zealMessageService,
			ISignalrPlayerHub signalrPlayerHub
			)
		{
			_toolSettingsLoad = toolSettingsLoad;
			_activePlayer = activePlayer;
			_appDispatcher = appDispatcher;
			_levelLogParse = new LevelLogParse(activePlayer);
			_settings = settings;
		
			_spells = spells;
			_zealMessageService = zealMessageService;
			_signalrPlayerHub = signalrPlayerHub;

			_zealMessageService.OnLabelMessageReceived += _zealMessageService_OnLabelMessageReceived;
			_zealMessageService.OnLogMessageReceived += _zealMessageService_OnLogMessageReceived;
			_zealMessageService.OnPlayerMessageReceived += _zealMessageService_OnPlayerMessageReceived;
			_zealMessageService.OnConnectionTerminated += _zealMessageService_onConnectionTerminated;
			
		}

		private void _zealMessageService_onConnectionTerminated(object sender, ConnectionTerminatedEventArgs e)
		{
			_settings.SelectedCharacter = null;
			_settings.ZealProcessID = 0;
			_activePlayer.Player.ZoneId = 0;
			SignalRPushDisconnect();
		}

		private void _zealMessageService_OnLabelMessageReceived(object sender, ZealMessageService.LabelMessageReceivedEventArgs e)
		{
			if (e.Message != null && e.Message.Character == _settings.SelectedCharacter && e.ProcessId == _settings.ZealProcessID)
			{
				if (e.Message.Type == ZealPipes.Common.PipeMessageType.Label &&
					e.Message.Data != null && e.Message.Data.Length > 0)
				{
					var spellLabel = e.Message.Data.FirstOrDefault(x => x.Type == ZealPipes.Common.LabelType.CastingName);
					if (!string.IsNullOrWhiteSpace(spellLabel.Value))
					{
						var target = e.Message.Data.FirstOrDefault(x => x.Type == ZealPipes.Common.LabelType.TargetName)?.Value;
						if (string.Compare(target, e.Message.Character) == 0)
						{
							target = "You";
						}
						var spell = _spells.AllSpells.FirstOrDefault(a => a.name == spellLabel.Value);
						if (spell != null)
						{
							var spellparse = new SpellParsingMatch
							{
								Spell = _spells.AllSpells.FirstOrDefault(a => a.name == spell.name),
								TargetName = target,
								MultipleMatchesFound = false,
								//IsYourCast = true
							};
							StartCastingEvent?.Invoke(this, new SpellEventArgs() { Spell = spellparse });
						}
					}
				}
			}
		}

		private void _zealMessageService_OnLogMessageReceived(object sender, ZealMessageService.LogMessageReceivedEventArgs e)
		{
			if (e.Message != null && e.Message.Character == _settings.SelectedCharacter && e.ProcessId == _settings.ZealProcessID)
			{
				if (e.Message.Type == ZealPipes.Common.PipeMessageType.LogText)
				{
					string yourFizzle = "Your spell fizzles!";
					string yourInterrupt = "Your spell is interrupted.";
					if (string.Compare(e.Message.Data.Text, yourFizzle) == 0)
					{
						FizzleCastingEvent?.Invoke(this, new FizzleEventArgs() { ExecutionTime = DateTime.Now });
					}
					else if (string.Compare(e.Message.Data.Text, yourInterrupt) == 0)
					{
						InterruptCastingEvent?.Invoke(this, new InterruptEventArgs() { ExecutionTime = DateTime.Now });
					}
				}
			}
		}

		private void _zealMessageService_OnPlayerMessageReceived(object sender, ZealMessageService.PlayerMessageReceivedEventArgs e)
		{
			if(e.Message != null)
			{
				if (_settings.ZealProcessID == 0)
				{
					if (string.IsNullOrWhiteSpace(_settings.SelectedCharacter))
					{
						_settings.ZealProcessID = e.ProcessId;
					}
				}
				else if (_settings.ZealProcessID != 0 && _settings.ZealProcessID != e.ProcessId)
				{
					return;
				}
				//adjust this logic
				if (!string.IsNullOrWhiteSpace(_settings.SelectedCharacter)
					&& string.Compare(_settings.SelectedCharacter, e.Message.Character, true) == 0)
				{
					_settings.ZealProcessID = e.ProcessId;
				}
				if(e.Message.Data != null && e.Message.Data.ZoneId > 0 && e.Message.Data.ZoneId != _activePlayer.Player.ZoneId)
				{
					_activePlayer.Player.LastZoneEntered = _activePlayer.Player.Zone;
					ZealZoneChangeEvent?.Invoke(this, new ZealLocationEventArgs() 
						{ 
							ZoneId = e.Message.Data.ZoneId, 
							Previous_ZoneId = _activePlayer.Player.ZoneId, 
							ProcessId = e.ProcessId 
						}
					);
					_activePlayer.Player.ZoneId = e.Message.Data.ZoneId;
				}
				//if(e.Message.Data.ZoneId > 1000)
				//{
				//	_activePlayer.IsInInstance = true;
				//}
				//else
				//{
				//	_activePlayer.IsInInstance = false;
				//}
				ZealLocationEvent?.Invoke(this, e);
			}
		}

		private void SignalRPushDisconnect()
		{
			_appDispatcher.DispatchUI(() =>
			{
				var player = new SignalrPlayer
				{
					Zone = this._activePlayer.Player.Zone,
					GuildName = this._activePlayer.Player.GuildName,
					PlayerClass = this._activePlayer.Player.PlayerClass,
					Server = this._activePlayer.Player.Server.Value,
					MapLocationSharing = this._activePlayer.Player.MapLocationSharing,
					Name = this._activePlayer.Player.Name,
					TrackingDistance = this._activePlayer.Player.TrackingDistance
				};
				_signalrPlayerHub.PushPlayerDisconnected(player);
			});
		}

		public class SpellEventArgs : EventArgs
		{
			public SpellParsingMatch Spell { get; set; }
		}

		public class SignalRLocationEventArgs : EventArgs
		{
			public Point3D Location { get; set; }
		}

		public class FizzleEventArgs : EventArgs
		{
			public DateTime ExecutionTime { get; set; }
		}

		public class InterruptEventArgs : EventArgs
		{
			public DateTime ExecutionTime { get; set; }
		}
		public class ZealLocationEventArgs : EventArgs
		{
			public int ZoneId { get; set; }
			public int Previous_ZoneId { get; set; }
			public int ProcessId { get; set; }
		}


		public event EventHandler<SpellEventArgs> StartCastingEvent;
		public event EventHandler<FizzleEventArgs> FizzleCastingEvent;
		public event EventHandler<InterruptEventArgs> InterruptCastingEvent;
		public event EventHandler<PlayerMessageReceivedEventArgs> ZealLocationEvent;
		public event EventHandler<ZealLocationEventArgs> ZealZoneChangeEvent;



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