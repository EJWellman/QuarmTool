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

        public LogParser(
			ResistSpellParser resistSpellParser,
            LogCustomTimer logCustomTimer,
			SpellWornOffLogParse spellWornOffLogParser,
			SpellLogParse spellLogParser,
            EQToolSettingsLoad toolSettingsLoad,
            ActivePlayer activePlayer,
            IAppDispatcher appDispatcher,
            EQToolSettings settings
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
            _uiTimer = new System.Timers.Timer(100);
            _uiTimer.Elapsed += Poll;
            _uiTimer.Enabled = true;
            LastYouActivity = DateTime.UtcNow.AddMonths(-1);
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


		public event EventHandler<RandomRollEventArgs> RandomRollEvent;
        public event EventHandler<WhoEventArgs> WhoEvent;
        public event EventHandler<WhoPlayerEventArgs> WhoPlayerEvent;
        public event EventHandler<SpellWornOffSelfEventArgs> SpellWornOffSelfEvent;
        public event EventHandler<QuakeArgs> QuakeEvent;
        public event EventHandler<DT_Event> POFDTEvent;
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

		public event EventHandler<SignalRLocationEventArgs> Zeal_SignalRLocationEvent;

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
					var customOverlay = CustomOverlayParser.Parse(message, _settings.CustomOverlays);
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

								if(_settings.SelectedCharacter != null)
								{
									_settings.SelectedCharacter = null;
								}
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
					if(_settings.SelectedCharacter == null)
					{
						_settings.SelectedCharacter = _activePlayer.Player.Name;
					}
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
                if (matchedzone != null)
                {
                    string b4matchedzone = matchedzone.ZoneName;

					if (!string.IsNullOrWhiteSpace(_activePlayer.Player?.LastZoneEntered) 
						&& ZoneParser.CheckWhoAgainstPreviousZone(message, matchedzone.ZoneName, _activePlayer.Player?.LastZoneEntered))
					{
						matchedzone.ZoneName = _activePlayer.Player?.LastZoneEntered;
					}
					else
					{
						if(_activePlayer.Player != null)
						{
							_activePlayer.Player.LastZoneEntered = b4matchedzone;
						}
					}

					matchedzone.ZoneName = ZoneParser.TranslateToMapName(matchedzone.ZoneName);
                    Debug.WriteLine($"Zone Change Detected {matchedzone.ZoneName}--{b4matchedzone}");
                    var p = _activePlayer.Player;
                    if (p != null)
                    {
                        p.Zone = matchedzone.ZoneName;
                        _toolSettingsLoad.Save(_settings);
                    }
                    PlayerZonedEvent?.Invoke(this, new PlayerZonedEventArgs { ZoneInfo = matchedzone });
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
                logfounddata = FindEq.GetLogFileLocation(new FindEq.FindEQData { EqBaseLocation = _settings.DefaultEqDirectory, EQlogLocation = _settings.EqLogDirectory }, _settings.SelectedCharacter);
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
					if(_settings.SelectedCharacter != null)
					{
					}

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

		public void PingSignalRLocationEvent(Point3D loc)
		{
			_appDispatcher.DispatchUI(() =>
			{
				Zeal_SignalRLocationEvent?.Invoke(null, new SignalRLocationEventArgs() { Location = loc });
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
