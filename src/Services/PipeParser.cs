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
        private readonly LocationParser _locationParser;
        private readonly DPSLogParse _dpsLogParse;
        private readonly LogDeathParse _logDeathParse;
        private readonly ConLogParse _conLogParse;
        private readonly LogCustomTimer _logCustomTimer;
        private readonly SpellLogParse _spellLogParse;
        private readonly SpellWornOffLogParse _spellWornOffLogParse;
        private readonly PlayerWhoLogParse _playerWhoLogParse;
        private readonly EnterWorldParser _enterWorldParser;
        private readonly QuakeParser _quakeParser;
        private readonly DTParser _pOFDTParser;
        private readonly EnrageParser _enrageParser;
        private readonly ChParser _chParser;
        private readonly InvisParser _invisParser;
        private readonly LevParser _levParser;
		private readonly ModRodParser _modRodParser;
        private readonly FTEParser _fTEParser;
        private readonly CharmBreakParser _charmBreakParser;
        private readonly FailedFeignParser _failedFeignParser;
        private readonly GroupInviteParser _groupInviteParser;
        private readonly ResistSpellParser _resistSpellParser;
        private readonly RandomParser _randomParser;

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
			ZealMessageService zealMessageService
			)
        {
            _randomParser = new RandomParser();
            _resistSpellParser = resistSpellParser;
            _groupInviteParser = new GroupInviteParser();
            _failedFeignParser = new FailedFeignParser(activePlayer);
            _charmBreakParser = new CharmBreakParser();
            _fTEParser = new FTEParser();
            _invisParser = new InvisParser();
            _levParser = new LevParser();
            _chParser = new ChParser(activePlayer);
            _enrageParser = new EnrageParser(activePlayer);
            _pOFDTParser = new DTParser();
            _quakeParser = new QuakeParser();
            _enterWorldParser = new EnterWorldParser();
            _spellWornOffLogParse = spellWornOffLogParser;
            _spellLogParse = spellLogParser;
            _logCustomTimer = logCustomTimer;
            _conLogParse = new ConLogParse();
            _logDeathParse = new LogDeathParse();
            _dpsLogParse = new DPSLogParse(activePlayer);
            _locationParser = new LocationParser();
            _toolSettingsLoad = toolSettingsLoad;
            _activePlayer = activePlayer;
            _appDispatcher = appDispatcher;
            _levelLogParse = new LevelLogParse(activePlayer);
			_modRodParser = new ModRodParser();
            _settings = settings;
            _playerWhoLogParse = new PlayerWhoLogParse();

			_spells = spells;
			_zealMessageService = zealMessageService;

			_zealMessageService.OnLabelMessageReceived += _zealMessageService_OnLabelMessageReceived;
			_zealMessageService.OnLogMessageReceived += _zealMessageService_OnLogMessageReceived;
		}

		private void _zealMessageService_OnLabelMessageReceived(object sender, ZealMessageService.LabelMessageReceivedEventArgs e)
		{
			if (e.Message != null && e.Message.Character == _settings.SelectedCharacter && e.ProcessId == _settings.ZealProcessID)
			{
				if(e.Message.Type == ZealPipes.Common.PipeMessageType.Label && 
					e.Message.Data != null && e.Message.Data.Length > 0)
				{
					var spellLabel = e.Message.Data.FirstOrDefault(x => x.Type == ZealPipes.Common.LabelType.CastingName);
					if(!string.IsNullOrWhiteSpace(spellLabel.Value))
					{
						var target = e.Message.Data.FirstOrDefault(x => x.Type == ZealPipes.Common.LabelType.TargetName)?.Value;
						if(string.Compare(target, e.Message.Character) == 0)
						{
							target = "You";
						}
						var spell = _spells.AllSpells.FirstOrDefault(a => a.name == spellLabel.Value);
						if(spell != null)
						{
							var spellparse = new SpellParsingMatch
							{
								Spell = _spells.AllSpells.FirstOrDefault(a => a.name == spell.name),
								TargetName = target,
								MultipleMatchesFound = false,
								IsYourCast = true
							};
							StartCastingEvent?.Invoke(this, new SpellEventArgs() { Spell = spellparse });
						}
					}
				}
			}
		}

		private void _zealMessageService_OnLogMessageReceived(object sender, ZealMessageService.LogMessageReceivedEventArgs e)
		{
			if(e.Message != null && e.Message.Character == _settings.SelectedCharacter && e.ProcessId == _settings.ZealProcessID)
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

		public DateTime LastYouActivity { get; private set; }

        private long? LastLogReadOffset { get; set; } = null;

        public class PlayerZonedEventArgs : EventArgs
        {
			public PlayerZonedInfo ZoneInfo { get; set;}
        }
        public class PlayerLocationEventArgs : EventArgs
        {
            public Point3D Location { get; set; }
            public PlayerInfo PlayerInfo { get; set; }
        }
        public class FightHitEventArgs : EventArgs
        {
            public DPSParseMatch HitInformation { get; set; }
        }
        public class DeadEventArgs : EventArgs
        {
            public string Name { get; set; }
			public DateTime ExecutionTime { get; set; }
        }

        public class ConEventArgs : EventArgs
        {
            public string Name { get; set; }
        }

        public class StartTimerEventArgs : EventArgs
        {
            public CustomTimer CustomTimer { get; set; }
        }

        public class CancelTimerEventArgs : EventArgs
        {
            public string Name { get; set; }
        }

        public class SpellEventArgs : EventArgs
        {
            public SpellParsingMatch Spell { get; set; }
        }

        public class SpellWornOffOtherEventArgs : EventArgs
        {
            public string SpellName { get; set; }
        }

        public class SpellWornOffSelfEventArgs : EventArgs
        {
            public List<string> SpellNames { get; set; }
        }

        public class WhoPlayerEventArgs : EventArgs
        {
            public EQToolShared.APIModels.PlayerControllerModels.Player PlayerInfo { get; set; }
        }
        public class RandomRollEventArgs : EventArgs
        {
            public RandomRollData RandomRollData { get; set; }
			public DateTime ExecutionTime { get; set; }
		}

		public class WhoEventArgs : EventArgs { }
        public class CampEventArgs : EventArgs { }
        public class EnteredWorldArgs : EventArgs { }
        public class QuakeArgs : EventArgs { }
        public class CharmBreakArgs : EventArgs { }

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

		public event EventHandler<RandomRollEventArgs> RandomRollEvent;
        public event EventHandler<WhoEventArgs> WhoEvent;
        public event EventHandler<WhoPlayerEventArgs> WhoPlayerEvent;
        public event EventHandler<SpellWornOffSelfEventArgs> SpellWornOffSelfEvent;
        public event EventHandler<QuakeArgs> QuakeEvent;
        public event EventHandler<DT_Event> DTEvent;
        public event EventHandler<EnrageEvent> EnrageEvent;
        public event EventHandler<ChParseData> CHEvent;
        public event EventHandler<LevStatus> LevEvent;
        public event EventHandler<InvisStatus> InvisEvent;
        public event EventHandler<FTEParserData> FTEEvent;
        public event EventHandler<CharmBreakArgs> CharmBreakEvent;
        public event EventHandler<string> FailedFeignEvent;
        public event EventHandler<string> GroupInviteEvent;
        public event EventHandler<SpellWornOffOtherEventArgs> SpellWornOtherOffEvent;
        public event EventHandler<ResistSpellData> ResistSpellEvent;
        public event EventHandler<SpellEventArgs> StartCastingEvent;
        public event EventHandler<CancelTimerEventArgs> CancelTimerEvent;
        public event EventHandler<StartTimerEventArgs> StartTimerEvent;
        public event EventHandler<ConEventArgs> ConEvent;
        public event EventHandler<DeadEventArgs> DeadEvent;
        public event EventHandler<FightHitEventArgs> FightHitEvent;
        public event EventHandler<PlayerZonedEventArgs> PlayerZonedEvent;
        public event EventHandler<PlayerLocationEventArgs> PlayerLocationEvent;
        public event EventHandler<CampEventArgs> CampEvent;
        public event EventHandler<EnteredWorldArgs> EnteredWorldEvent;
		public event EventHandler<ModRodUsageArgs> ModRodUsedEvent;
		public event EventHandler<CustomOverlayEventArgs> CustomOverlayEvent;

		public event EventHandler<FizzleEventArgs> FizzleCastingEvent;
		public event EventHandler<InterruptEventArgs> InterruptCastingEvent;



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
