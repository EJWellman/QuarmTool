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
using static EQTool.Services.POFDTParser;
using static EQTool.Services.RandomParser;
using static EQTool.Services.ResistSpellParser;

namespace EQTool.Services
{
    public class LogParser : IDisposable
    {
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
        private readonly POFDTParser _pOFDTParser;
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
		private readonly CustomOverlayParser _customOverlayParser;

        private bool StartingWhoOfZone = false;
        private bool Processing = false;
        private bool StillCamping = false;
        private bool HasUsedStartupEnterWorld = false;

        public LogParser(
            RandomParser randomParser,
            ResistSpellParser resistSpellParser,
            GroupInviteParser groupInviteParser,
            CharmBreakParser charmBreakParser,
            FTEParser fTEParser,
            ChParser chParser,
            QuakeParser quakeParser,
            EnterWorldParser enterWorldParser,
            SpellWornOffLogParse spellWornOffLogParse,
            SpellLogParse spellLogParse,
            LogCustomTimer logCustomTimer,
            ConLogParse conLogParse,
            LogDeathParse logDeathParse,
            DPSLogParse dPSLogParse,
            LocationParser locationParser,
            EQToolSettingsLoad toolSettingsLoad,
            ActivePlayer activePlayer,
            IAppDispatcher appDispatcher,
            EQToolSettings settings,
            POFDTParser pOFDTParser,
            EnrageParser enrageParser,
            LevelLogParse levelLogParse,
            PlayerWhoLogParse playerWhoLogParse,
            InvisParser invisParser,
            LevParser levParser,
			ModRodParser modRodParser,
            FailedFeignParser failedFeignParser,
			CustomOverlayParser customOverlayParser
            )
        {
            this._randomParser = randomParser;
            this._resistSpellParser = resistSpellParser;
            this._groupInviteParser = groupInviteParser;
            this._failedFeignParser = failedFeignParser;
            this._charmBreakParser = charmBreakParser;
            this._fTEParser = fTEParser;
            this._invisParser = invisParser;
            this._levParser = levParser;
            this._chParser = chParser;
            this._enrageParser = enrageParser;
            this._pOFDTParser = pOFDTParser;
            this._quakeParser = quakeParser;
            this._enterWorldParser = enterWorldParser;
            this._spellWornOffLogParse = spellWornOffLogParse;
            this._spellLogParse = spellLogParse;
            this._logCustomTimer = logCustomTimer;
            this._conLogParse = conLogParse;
            this._logDeathParse = logDeathParse;
            this._dpsLogParse = dPSLogParse;
            this._locationParser = locationParser;
            this._toolSettingsLoad = toolSettingsLoad;
            this._activePlayer = activePlayer;
            this._appDispatcher = appDispatcher;
            this._levelLogParse = levelLogParse;
			this._modRodParser = modRodParser;
            this._settings = settings;
            this._playerWhoLogParse = playerWhoLogParse;
			this._customOverlayParser = customOverlayParser;
            _uiTimer = new System.Timers.Timer(100);
            _uiTimer.Elapsed += Poll;
            _uiTimer.Enabled = true;
            LastYouActivity = DateTime.UtcNow.AddMonths(-1);
        }

        public DateTime LastYouActivity { get; private set; }

        private long? LastLogReadOffset { get; set; } = null;

        public class PlayerZonedEventArgs : EventArgs
        {
            public string Zone { get; set; }
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

        public event EventHandler<RandomRollEventArgs> RandomRollEvent;
        public event EventHandler<WhoEventArgs> WhoEvent;
        public event EventHandler<WhoPlayerEventArgs> WhoPlayerEvent;
        public event EventHandler<SpellWornOffSelfEventArgs> SpellWornOffSelfEvent;
        public event EventHandler<QuakeArgs> QuakeEvent;
        public event EventHandler<POF_DT_Event> POFDTEvent;
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

        public void Push(string log)
        {
            _appDispatcher.DispatchUI(() =>
            {
                MainRun(log);
            });
        }

        private void MainRun(string line1)
        {
            if (line1 == null || line1.Length < 27)
            {
                return;
            }
            try
            {
                var date = line1.Substring(1, 24);
                var message = line1.Substring(27).Trim();
                if (string.IsNullOrWhiteSpace(message))
                {
                    return;
                }

                if (message.StartsWith("You"))
                {
                    LastYouActivity = DateTime.UtcNow;
                }

                var timestamp = LogFileDateTimeParse.ParseDateTime(date);

				try
				{
					var customOverlay = CustomOverlayParser.Parse(message);
					if (customOverlay != null)
					{
						_appDispatcher.DispatchUI(() =>
						{
							CustomOverlayEvent?.Invoke(this, new CustomOverlayEventArgs { CustomOverlay = customOverlay });
						});
					}
				}
				catch (Exception e)
				{
					Debug.WriteLine($"LogParser CustomOverlayEvent {message}", this._activePlayer?.Player?.Server);
				}

				var pos = _locationParser.Match(message);
                if (pos.HasValue)
                {
                    PlayerLocationEvent?.Invoke(this, new PlayerLocationEventArgs { Location = pos.Value, PlayerInfo = _activePlayer.Player });
                    return;
                }

                if (message == "It will take about 5 more seconds to prepare your camp.")
                {
                    StillCamping = true;
                    _ = System.Threading.Tasks.Task.Factory.StartNew(() =>
                    {
                        System.Threading.Thread.Sleep(1000 * 6);
                        if (StillCamping)
                        {
                            _appDispatcher.DispatchUI(() =>
                            {
                                Debug.WriteLine("CampEvent");
                                CampEvent?.Invoke(this, new CampEventArgs());
                            });
                        }
                    });
                    return;
                }
                else if (message == "You abandon your preparations to camp.")
                {
                    StillCamping = false;
                    return;
                }
                else if (message == "Welcome to EverQuest!")
                {
                    HasUsedStartupEnterWorld = true;
                    Debug.WriteLine("EnteredWorldEvent In Game");
                    EnteredWorldEvent?.Invoke(this, new EnteredWorldArgs());
					//Add log lock
                    return;
                }

                var playerwho = _playerWhoLogParse.ParsePlayerInfo(message);
                if (playerwho != null && StartingWhoOfZone)
                {
                    WhoPlayerEvent?.Invoke(this, new WhoPlayerEventArgs { PlayerInfo = playerwho });
                    return;
                }

                if (_playerWhoLogParse.IsZoneWhoLine(message))
                {
                    StartingWhoOfZone = true;
                    WhoEvent?.Invoke(this, new WhoEventArgs());
                    return;
                }
                else
                {
                    StartingWhoOfZone = message == "---------------------------" && StartingWhoOfZone;
                }

				var petOwner = _playerWhoLogParse.ParsePetOwner(message, _activePlayer.Player);
				if(!string.IsNullOrWhiteSpace(petOwner) && _activePlayer.Player != null)
				{
					_activePlayer.Player.PetName = petOwner;
					return;
				}

                var matched = _dpsLogParse.Match(message, timestamp);
                if (matched != null)
                {
                    FightHitEvent?.Invoke(this, new FightHitEventArgs { HitInformation = matched });
                    return;
                }

                var name = _logDeathParse.GetDeadTarget(message);
                if (!string.IsNullOrWhiteSpace(name))
                {
                    DeadEvent?.Invoke(this, new DeadEventArgs { Name = name, ExecutionTime = timestamp });
                    return;
                }

                name = _conLogParse.ConMatch(message);
                if (!string.IsNullOrWhiteSpace(name))
                {
                    ConEvent?.Invoke(this, new ConEventArgs { Name = name });
                    return;
                }

                var customtimer = _logCustomTimer.GetStartTimer(message);
                if (customtimer != null)
                {
					customtimer.ExecutionTime = timestamp;
                    StartTimerEvent?.Invoke(this, new StartTimerEventArgs { CustomTimer = customtimer });
                    return;
                }

                name = _logCustomTimer.GetCancelTimer(message);
                if (!string.IsNullOrWhiteSpace(name))
                {
                    CancelTimerEvent?.Invoke(this, new CancelTimerEventArgs { Name = name });
                    return;
                }

                var didcharmbreak = this._charmBreakParser.DidCharmBreak(message);
                if (didcharmbreak)
                {
                    CharmBreakEvent?.Invoke(this, new CharmBreakArgs());
                    return;
                }

                if (message == "The screams fade away.")
                {
                    SpellWornOtherOffEvent?.Invoke(this, new SpellWornOffOtherEventArgs { SpellName = "Soul Consumption" });
                    return;
                }

                var matchedspell = _spellLogParse.MatchSpell(message);
                if (matchedspell != null && matchedspell.Spell.name != "Modulation")
                {
                    StartCastingEvent?.Invoke(this, new SpellEventArgs { Spell = matchedspell });
                    return;
                }

                var resistspell = this._resistSpellParser.ParseNPCSpell(message);
                if (resistspell != null)
                {
                    ResistSpellEvent?.Invoke(this, resistspell);
                    return;
                }

                name = _spellWornOffLogParse.MatchWornOffOtherSpell(message);
                if (!string.IsNullOrWhiteSpace(name))
                {
                    SpellWornOtherOffEvent?.Invoke(this, new SpellWornOffOtherEventArgs { SpellName = name });
                    return;
                }

                var spells = _spellWornOffLogParse.MatchWornOffSelfSpell(message);
                if (spells.Any())
                {
                    SpellWornOffSelfEvent?.Invoke(this, new SpellWornOffSelfEventArgs { SpellNames = spells });
                    return;
                }

                var quaked = _quakeParser.IsQuake(message);
                if (quaked)
                {
                    QuakeEvent?.Invoke(this, new QuakeArgs());
                    return;
                }

                var randomdata = _randomParser.Parse1(message);
                if (randomdata != null)
                {
                    RandomRollEvent?.Invoke(this, new RandomRollEventArgs { RandomRollData = randomdata, ExecutionTime = timestamp });
                    return;
                }

                var dt = this._pOFDTParser.DtCheck(message);
                if (dt != null)
                {
					dt.ExecutionTime = timestamp;
                    POFDTEvent?.Invoke(this, dt);
                    return;
                }

                var enragecheck = this._enrageParser.EnrageCheck(message);
                if (enragecheck != null)
                {
                    EnrageEvent?.Invoke(this, enragecheck);
                    return;
                }

                var chdata = this._chParser.ChCheck(message);
                if (chdata != null)
                {
                    CHEvent?.Invoke(this, chdata);
                    return;
                }

                var lev = this._levParser.Parse(message);
                if (lev.HasValue)
                {
                    LevEvent?.Invoke(this, lev.Value);
                    return;
                }

                var invi = this._invisParser.Parse(message);
                if (invi.HasValue)
                {
                    InvisEvent?.Invoke(this, invi.Value);
                    return;
                }

                var fte = this._fTEParser.Parse(message);
                if (fte != null)
                {
                    FTEEvent?.Invoke(this, fte);
                    return;
                }

				var modRod = _modRodParser.Parse(message, timestamp);
				if (modRod != null)
				{
					ModRodUsedEvent?.Invoke(this, modRod);
					return;
				}

				var stringmsg = this._failedFeignParser.FailedFaignCheck(message);
                if (!string.IsNullOrWhiteSpace(stringmsg))
                {
                    FailedFeignEvent?.Invoke(this, stringmsg);
                    return;
                }

                stringmsg = this._groupInviteParser.Parse(message);
                if (!string.IsNullOrWhiteSpace(stringmsg))
                {
                    GroupInviteEvent?.Invoke(this, stringmsg);
                    return;
                }

                _levelLogParse.MatchLevel(message);
                var matchedzone = ZoneParser.Match(message);
                if (!string.IsNullOrWhiteSpace(matchedzone))
                {
                    var b4matchedzone = matchedzone;

					if (!string.IsNullOrWhiteSpace(_activePlayer.Player?.LastZoneEntered) 
						&& ZoneParser.CheckWhoAgainstPreviousZone(message, matchedzone, _activePlayer.Player?.LastZoneEntered))
					{
						matchedzone = _activePlayer.Player?.LastZoneEntered;
					}
					else
					{
						if(_activePlayer.Player != null)
						{
							_activePlayer.Player.LastZoneEntered = b4matchedzone;
						}
					}

					matchedzone = ZoneParser.TranslateToMapName(matchedzone);
                    Debug.WriteLine($"Zone Change Detected {matchedzone}--{b4matchedzone}");
                    var p = _activePlayer.Player;
                    if (p != null)
                    {
                        p.Zone = matchedzone;
                        _toolSettingsLoad.Save(_settings);
                    }
                    PlayerZonedEvent?.Invoke(this, new PlayerZonedEventArgs { Zone = matchedzone });
                    return;
                }
            }
            catch (Exception e)
            {
                App.LogUnhandledException(e, $"LogParser Filename: '{_activePlayer.LogFileName}' '{line1}'", this._activePlayer?.Player?.Server);
            }
        }

        private void Poll(object sender, EventArgs e)
        {
            if (Processing)
            {
                return;
            }
            Processing = true;
            LogFileInfo logfounddata = null;
            try
            {
                logfounddata = FindEq.GetLogFileLocation(new FindEq.FindEQData { EqBaseLocation = _settings.DefaultEqDirectory, EQlogLocation = _settings.EqLogDirectory });
            }
            catch { }
            if (logfounddata == null || !logfounddata.Found)
            {
                Processing = false;
                return;
            }
            _settings.EqLogDirectory = logfounddata.Location;
            _appDispatcher.DispatchUI(() =>
            {
                try
                {
                    var playerchanged = _activePlayer.Update();
                    var filepath = _activePlayer.LogFileName;
                    if (playerchanged || filepath != _lastLogFilename)
                    {
                        LastLogReadOffset = null;
                        _lastLogFilename = filepath;
                    }

                    if (string.IsNullOrWhiteSpace(filepath))
                    {
                        Debug.WriteLine($"No playerfile found!");
                        return;
                    }

                    var fileinfo = new FileInfo(filepath);
                    var newplayerdetected = false;
                    if (!LastLogReadOffset.HasValue || (LastLogReadOffset > fileinfo.Length && fileinfo.Length > 0))
                    {
                        Debug.WriteLine($"Player Switched or new Player detected {filepath} {fileinfo.Length}");
                        LastLogReadOffset = fileinfo.Length;
                        StillCamping = false;
                        newplayerdetected = true;
                    }
                    var linelist = new List<string>();
                    using (var stream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (var reader = new StreamReader(stream))
                    {
                        var lookbacksize = 4000;
                        if (newplayerdetected && LastLogReadOffset.Value > lookbacksize)
                        {
                            _ = stream.Seek(LastLogReadOffset.Value - lookbacksize, SeekOrigin.Begin);
                            var buffer = new byte[lookbacksize];
                            _ = stream.Read(buffer, 0, lookbacksize);
                            using (var ms = new MemoryStream(buffer))
                            using (var innerstream = new StreamReader(ms))
                            {
                                while (!innerstream.EndOfStream)
                                {
                                    if (_enterWorldParser.HasEnteredWorld(innerstream.ReadLine()))
                                    {
                                        HasUsedStartupEnterWorld = true;
                                        Debug.WriteLine("EnteredWorldEvent Player Changed");
                                        EnteredWorldEvent?.Invoke(this, new EnteredWorldArgs());
                                        break;
                                    }
                                }
                            }
                        }
                        _ = stream.Seek(LastLogReadOffset.Value, SeekOrigin.Begin);
                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            linelist.Add(line);
                            LastLogReadOffset = stream.Position;
                        }
                    }

                    if (!HasUsedStartupEnterWorld && linelist.Any())
                    {
                        HasUsedStartupEnterWorld = true;
                        Debug.WriteLine("EnteredWorldEvent First Time");
                        EnteredWorldEvent?.Invoke(this, new EnteredWorldArgs());
                    }
                    foreach (var line in linelist)
                    {
                        MainRun(line);
                    }
                }
                catch (Exception ex) when (!(ex is System.IO.IOException) && !(ex is UnauthorizedAccessException))
                {
                    App.LogUnhandledException(ex, "LogParser DispatchUI", _activePlayer?.Player?.Server);
                }
                finally
                {
                    Processing = false;
                }
            });
        }

        public void Dispose()
        {
            _uiTimer.Stop();
            _uiTimer.Dispose();
            _uiTimer = null;
        }
    }
}
