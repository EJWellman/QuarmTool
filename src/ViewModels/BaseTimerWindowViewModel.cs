using EQTool.Models;
using EQTool.Services;
using EQTool.Services.Spells;
using EQToolShared;
using EQToolShared.Enums;
using EQToolShared.HubModels;
using EQToolShared.ExtendedClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Text.RegularExpressions;

namespace EQTool.ViewModels
{
    public class BaseTimerWindowViewModel : INotifyPropertyChanged
    {
        private readonly ActivePlayer _activePlayer;
        private readonly IAppDispatcher _appDispatcher;
        private readonly EQToolSettings _settings;
        private readonly EQSpells _spells;
		private readonly QuarmDataService _quarmService;
		private ColorService _colorService;
		public TimerWindowOptions _windowOptions;

		public BaseTimerWindowViewModel(ActivePlayer activePlayer, IAppDispatcher appDispatcher, EQToolSettings settings, EQSpells spells, ColorService colorService, TimerWindowOptions windowOptions, QuarmDataService quarmDataService)
        {
			_activePlayer = activePlayer;
			_appDispatcher = appDispatcher;
            _settings = settings;
            _spells = spells;
			_quarmService = quarmDataService;
			_colorService = colorService;
			_windowOptions = windowOptions;

		}
		public int ID
		{
			get
			{
				return _windowOptions.ID;
			}
			set
			{
				_windowOptions.ID = value;
				OnPropertyChanged();
			}
		}
		public bool BestGuessSpells
		{
			get
			{
				return _windowOptions.BestGuessSpells;
			}
			set
			{
				_windowOptions.BestGuessSpells = value;
				OnPropertyChanged();
			}
		}
		public bool YouOnlySpells
		{
			get
			{
				return _windowOptions.YouOnlySpells;
			}
			set
			{
				_windowOptions.YouOnlySpells = value;
				OnPropertyChanged();
			}
		}
		public bool ShowRandomRolls
		{
			get
			{
				return _windowOptions.ShowRandomRolls;
			}
			set
			{
				_windowOptions.ShowRandomRolls = value;
				OnPropertyChanged();
			}
		}
		public bool ShowTimers
		{
			get
			{
				return _windowOptions.ShowTimers;
			}
			set
			{
				_windowOptions.ShowTimers = value;
				OnPropertyChanged();
			}
		}
		public bool ShowModRodTimers
		{
			get
			{
				return _windowOptions.ShowModRodTimers;
			}
			set
			{
				_windowOptions.ShowModRodTimers = value;
				OnPropertyChanged();
			}
		}
		public bool ShowDeathTouches
		{
			get
			{
				return _windowOptions.ShowDeathTouches;
			}
			set
			{
				_windowOptions.ShowDeathTouches = value;
				OnPropertyChanged();
			}
		}
		public bool ShowSpells
		{
			get
			{
				return _windowOptions.ShowSpells;
			}
			set
			{
				_windowOptions.ShowSpells = value;
				OnPropertyChanged();
			}
		}
		public bool ShowNPCs
		{
			get
			{
				return _windowOptions.ShowNPCs;
			}
			set
			{
				_windowOptions.ShowNPCs = value;
				OnPropertyChanged();
			}
		}
		public bool ShowPCs
		{
			get
			{
				return _windowOptions.ShowPCs;
			}
			set
			{
				_windowOptions.ShowPCs = value;
				OnPropertyChanged();
			}
		}
		public bool ShowSimpleTimers
		{
			get
			{
				return _windowOptions.ShowSimpleTimers;
			}
			set
			{
				_windowOptions.ShowSimpleTimers = value;
				OnPropertyChanged();
			}
		}
		public string WindowTitle
		{
			get
			{
				return _windowOptions.Title;
			}
			set
			{
				_windowOptions.Title = value;
				OnPropertyChanged();
			}
		}
		public bool AlwaysOnTop
		{
			get
			{
				return _windowOptions.AlwaysOnTop;
			}
			set
			{
				_windowOptions.AlwaysOnTop = value;
				OnPropertyChanged();
			}
		}
		private Models.WindowState _windowState;
		public Models.WindowState WindowState
		{
			get
			{
				if(_windowState == null)
				{
					_windowState = new Models.WindowState();
				}
				return _windowState;
			}
			set
			{
				_windowState = value ?? new Models.WindowState();
				OnPropertyChanged();
			}
		}

		public ObservableCollection<UISpell> _SpellList = new ObservableCollection<UISpell>();
		public ObservableCollection<UISpell> SpellList
		{
			get => _SpellList;
			set
			{
				_SpellList = value;
				OnPropertyChanged();
			}
		}

		public void UpdateStuff()
		{
			OnPropertyChanged(nameof(SpellList));
		}

		public void ClearYouSpells()
		{
			var itemstoremove = SpellList.Where(a => a.TargetName == EQSpells.SpaceYou).ToList();
			foreach (var item in itemstoremove)
			{
				_ = SpellList.Remove(item);
			}
		}

		public void UpdateSpells()
		{
			_appDispatcher.DispatchUI(() =>
			{
				var d = DateTime.Now;
				var player = _activePlayer.Player;
				var itemsToRemove = new List<UISpell>();

				foreach (var item in SpellList)
				{
					item.UpdateTimeLeft();
					if (!ShouldProduceTimer(item))
					{
						itemsToRemove.Add(item);
					}
				}

				var groupedspells = SpellList.GroupBy(a => a.TargetName).ToList();
				foreach (var spells in groupedspells)
				{
					foreach (var spell in spells)
					{
						spell.HeaderVisibility = System.Windows.Visibility.Visible;
					}
				}

				foreach (var item in itemsToRemove)
				{
					_ = SpellList.Remove(item);
				}
			});
		}

		public void ClearAllSpells()
		{
			_appDispatcher.DispatchUI(() =>
			{
				while (SpellList.Count > 0)
				{
					SpellList.RemoveAt(SpellList.Count - 1);
				}
			});
		}

		private readonly List<string> SpellsThatNeedCounts = new List<string>()
		{
			"Mana Sieve",
			"LowerElement",
			"Concussion",
			"Flame Lick",
			"Jolt",
			"Cinder Jolt",
		};

		private readonly List<string> SpellsThatDragonsDo = new List<string>()
		{
			"Dragon Roar",
			"Silver Breath",
			"Ice breath",
			"Mind Cloud",
			"Rotting Flesh",
			"Putrefy Flesh",
			"Stun Breath",
			"Immolating Breath",
			"Rain of Molten Lava",
			"Frost Breath",
			"Lava Breath",
			"Cloud of Fear",
			"Diseased Cloud",
			"Tsunami",
			"Ancient Breath"
		};

		public void UpdateSpellVisuals(TimerWindowOptions options)
		{
			foreach (var item in SpellList)
			{
				item.ShowSimpleTimers = options.ShowSimpleTimers;
				item.SpellNameColor = new System.Windows.Media.SolidColorBrush(_settings.SpellTimerNameColor);
				item.ProgressBarColor = _colorService.GetColorFromSpellType(item.SpellType);
				item.DropShadowVisibility = _settings.ShowTimerDropShadows ? Visibility.Visible : Visibility.Collapsed;
				item.UpdateTimeLeft();
			}
		}

		public void TryAdd(SpellParsingMatch match, bool resisted)
		{
			if (match?.Spell == null)
			{
				return;
			}

			match.TargetName = match.TargetName.CleanUpZealName();
			string cleanTargetName = match.TargetName.CleanUpZealName(true);

			_appDispatcher.DispatchUI(() =>
			{
				var spellname = match.Spell.name;
				if (string.Equals(match.Spell.name, "Harvest", StringComparison.OrdinalIgnoreCase) && match.TargetName == EQSpells.SpaceYou)
				{
					TryAddCustom(new CustomTimer
					{
						DurationInSeconds = 600,
						Name = "--CoolDown-- " + spellname,
						SpellNameIcon = spellname,
						SpellType = SpellTypes.HarvestCooldown
					});
					return;
				}
				if (SpellsThatDragonsDo.Contains(match.Spell.name))
				{
					TryAddCustom(new CustomTimer
					{
						DurationInSeconds = (int)(match.Spell.recastTime / 1000.0),
						Name = "--CoolDown-- " + spellname,
						SpellNameIcon = spellname,
						SpellType = SpellTypes.BadGuyCoolDown
					});
					return;
				}
				if (match.Spell.name.EndsWith("Discipline"))
				{
					var basetime = (int)(match.Spell.recastTime / 1000.0);
					var playerlevel = this._activePlayer.Player.Level;
					if (match.Spell.name == "Evasive Discipline")
					{
						float baseseconds = 15 * 60;
						float levelrange = 60 - 51;
						float secondsrange = (15 - 7) * 60;
						float secondsperlevelrange = (secondsrange / levelrange);
						float playerleveltick = playerlevel - 52;
						basetime = (int)(baseseconds - (playerleveltick * secondsperlevelrange));
					}
					else if (match.Spell.name == "Defensive Discipline")
					{
						float baseseconds = 15 * 60;
						float levelrange = 60 - 54;
						float secondsrange = (15 - 10) * 60;
						float secondsperlevelrange = secondsrange / levelrange;
						float playerleveltick = playerlevel - 55;
						basetime = (int)(baseseconds - (playerleveltick * secondsperlevelrange));
					}
					else if (match.Spell.name == "Precision Discipline")
					{
						float baseseconds = 30 * 60;
						float levelrange = 60 - 56;
						float secondsrange = (30 - 27) * 60;
						float secondsperlevelrange = secondsrange / levelrange;
						float playerleveltick = playerlevel - 57;
						basetime = (int)(baseseconds - (playerleveltick * secondsperlevelrange));
					}
					TryAddCustom(new CustomTimer
					{
						DurationInSeconds = basetime,
						Name = "--Discipline-- " + spellname,
						SpellNameIcon = "Strengthen",
						SpellType = SpellTypes.DisciplineCoolDown,
						TargetName = match.TargetName,
						Classes = match.Spell.Classes
					});
				}
				if (resisted)
				{
					return;
				}
				//var needscount = SpellsThatNeedCounts.Contains(spellname);

				TimeSpan spellDuration;

				if(match.Spell.type == SpellTypes.Beneficial)
				{
					spellDuration = TimeSpan.FromSeconds(SpellDurations.GetDuration_inSeconds(match.Spell, _activePlayer.Player, _settings.BeneficialSpellDurationMultiplier != 1.0d));
				}
				else if(match.Spell.type == SpellTypes.Detrimental)
				{
					spellDuration = TimeSpan.FromSeconds(SpellDurations.GetDuration_inSeconds(match.Spell, _activePlayer.Player, _settings.DetrimentalSpellDurationMultiplier != 1.0d));
				}
				else
				{
					spellDuration = TimeSpan.FromSeconds(SpellDurations.GetDuration_inSeconds(match.Spell, _activePlayer.Player));
				}

				//var spellduration = TimeSpan.FromSeconds(SpellDurations.GetDuration_inSeconds(match.Spell, _activePlayer.Player, _settings.BeneficialSpellDurationMultiplier != 1.0d));
				var duration = match.TotalSecondsOverride ?? spellDuration.TotalSeconds;
				//var isNPCCast = match.
				var isTargetNPC = _quarmService.DoesMonsterExistInZone(cleanTargetName);
				if (_settings.BeneficialSpellDurationMultiplier != 1.0 && match.Spell.type == SpellTypes.Beneficial)
				{
					duration = Convert.ToInt32(duration * _settings.BeneficialSpellDurationMultiplier);
				}
				else if(_settings.BeneficialSpellDurationMultiplier != 1.0 && match.Spell.type == SpellTypes.Beneficial)
				{
					duration = Convert.ToInt32(duration * _settings.DetrimentalSpellDurationMultiplier);
				}

				var uispell = new UISpell(DateTime.Now.AddSeconds((int)duration), DateTime.Now.AddSeconds((int)duration), isTargetNPC, _windowOptions.ShowSimpleTimers)
				{
					UpdatedDateTime = DateTime.Now,
					PercentLeftOnSpell = 100,
					SpellType = match.Spell.type,
					TargetName = match.TargetName,
					SpellName = spellname,
					Rect = match.Spell.Rect,
					//PersistentSpell = needscount,
					//Counter = needscount ? 1 : (int?)null,
					SpellIcon = match.Spell.SpellIcon,
					Classes = match.Spell.Classes,
					GuessedSpell = match.IsGuess,
					SpellNameColor = new System.Windows.Media.SolidColorBrush(_settings.SpellTimerNameColor),
					ProgressBarColor = _colorService.GetColorFromSpellType(match.Spell.type),
					DropShadowVisibility = _settings.ShowTimerDropShadows ? Visibility.Visible : Visibility.Collapsed,
					IsNPC = isTargetNPC,
					IsYourCast = match.IsYourCast,
					ExecutionTime = DateTime.Now
				};
				if (!ShouldProduceTimer(uispell))
				{
					return;
				}
				var s = SpellList.FirstOrDefault(a => a.SpellName == spellname && match.TargetName == a.TargetName);
				if (s != null)
				{
					//if (needscount)
					//{
					//	s.Counter += 1;
					//	s.UpdatedDateTime = DateTime.Now;
					//}
					//else
					//{
						_ = SpellList.Remove(s);
						SpellList.Add(uispell);
					//}
				}
				else
				{
					SpellList.Add(uispell);
				}
			});
		}

		public void TryAddCustom(CustomTimer match)
		{
			if (match?.Name == null)
			{
				return;
			}

			match.Name = match.Name.CleanUpZealName();
			match.TargetName = match.TargetName.CleanUpZealName();

			_appDispatcher.DispatchUI(() =>
			{
				//var s = SpellList.FirstOrDefault(a => a.SpellName == match.Name && match.TargetName == a.TargetName);
				//if (s != null)
				//{
				//	if (match.SpellType != SpellTypes.RandomRoll)
				//	{
				//		_ = SpellList.Remove(s);
				//	}
				//}

				var spellduration = match.DurationInSeconds;
				var negativeDuration = match.NegativeDurationToShow;
				var spellicon = _spells.AllSpells.FirstOrDefault(a => a.name == match.SpellNameIcon);
				var rollorder = 0;
				if (match.SpellType == SpellTypes.RandomRoll)
				{
					var rollsingroup = SpellList.Where(a => a.TargetName == match.TargetName).OrderByDescending(a => a.Roll).ToList();
					rollorder = rollsingroup.Where(a => a.SpellName == match.Name).Select(a => (int?)a.RollOrder).Max() ?? 0;
					//var maxrolls = 10;
					//if (rollsingroup.Count >= maxrolls)
					//{
					//    _ = SpellList.Remove(rollsingroup.LastOrDefault());
					//    rollsingroup = SpellList.Where(a => a.TargetName == match.TargetName).ToList();
					//}
					foreach (var item in rollsingroup)
					{
						item.TimerEndDateTime = DateTime.Now.AddSeconds(spellduration);
					}
				}
				DateTime endTime;
				DateTime negativeEndTime;
				if (match.ExecutionTime != null)
				{
					endTime = match.ExecutionTime.AddSeconds(spellduration);
					negativeEndTime = match.ExecutionTime.AddSeconds(negativeDuration > spellduration ? negativeDuration : spellduration);
				}
				else
				{
					endTime = DateTime.Now.AddSeconds(spellduration);
					negativeEndTime = DateTime.Now.AddSeconds(negativeDuration > spellduration ? negativeDuration : spellduration);
				}

				SpellList.Add(new UISpell(endTime, negativeEndTime, false, _windowOptions.ShowSimpleTimers)
				{
					UpdatedDateTime = match.ExecutionTime,
					PercentLeftOnSpell = 100,
					SpellType = match.SpellType,
					TargetName = match.TargetName,
					SpellName = match.Name,
					Rect = spellicon.Rect,
					SpellIcon = spellicon.SpellIcon,
					Classes = match.Classes,
					GuessedSpell = false,
					PersistentSpell = false,
					Roll = match.Roll,
					RollOrder = rollorder + 1,
					ExecutionTime = match.ExecutionTime,
					SpellNameColor = new System.Windows.Media.SolidColorBrush(_settings.SpellTimerNameColor),
					ProgressBarColor = _colorService.GetColorFromSpellType(match.SpellType),
					DropShadowVisibility = _settings.ShowTimerDropShadows ? Visibility.Visible : Visibility.Collapsed,
					IsNPC = match.IsNPC
				});
			});
		}

		public void AddSavedYouSpells(List<YouSpells> youspells)
		{
			if (youspells == null || !youspells.Any())
			{
				return;
			}

			_appDispatcher.DispatchUI(() =>
			{
				foreach (var item in youspells)
				{
					var match = _spells.AllSpells.FirstOrDefault(a => a.name == item.Name);
					if (match != null)
					{
						var spellduration = TimeSpan.FromSeconds(SpellDurations.GetDuration_inSeconds(match, _activePlayer.Player));
						var savedspellduration = item.TotalSecondsLeft;
						var uispell = new UISpell(DateTime.Now.AddSeconds(savedspellduration), false)
						{
							UpdatedDateTime = DateTime.Now,
							PercentLeftOnSpell = 100,
							SpellType = match.type,
							TargetName = EQSpells.SpaceYou,
							SpellName = match.name,
							Rect = match.Rect,
							PersistentSpell = false,
							Counter = null,
							SpellIcon = match.SpellIcon,
							Classes = match.Classes,
							GuessedSpell = false,
							SpellNameColor = new System.Windows.Media.SolidColorBrush(_settings.SpellTimerNameColor),
							ProgressBarColor = _colorService.GetColorFromSpellType(match.type),
							DropShadowVisibility = _settings.ShowTimerDropShadows ? Visibility.Visible : Visibility.Collapsed
						};
						SpellList.Add(uispell);
					}
				}
			});
		}

		public void TryRemoveUnambiguousSpellOther(string possiblespell)
		{
			if (string.IsNullOrWhiteSpace(possiblespell))
			{
				return;
			}

			_appDispatcher.DispatchUI(() =>
			{
				var s = SpellList.Where(a => a.SpellName == possiblespell && a.TargetName != EQSpells.SpaceYou).ToList();
				if (s.Count() == 1)
				{
					_ = SpellList.Remove(s.FirstOrDefault());
				}
			});
		}

		public void TryRemoveUnambiguousSpellSelf(List<string> possiblespellnames)
		{
			if (!possiblespellnames.Any())
			{
				return;
			}

			_appDispatcher.DispatchUI(() =>
			{
				var spells = SpellList.Where(a => possiblespellnames.Contains(a.SpellName) && a.TargetName == EQSpells.SpaceYou).ToList();
				if (spells.Count() == 1)
				{
					_ = SpellList.Remove(spells.FirstOrDefault());
				}
			});
		}

		public void TryRemoveCustom(string name)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				return;
			}
			
			name = name.CleanUpZealName();

			_appDispatcher.DispatchUI(() =>
			{
				var s = SpellList.FirstOrDefault(a => a.SpellName == name && CustomTimer.CustomerTime == a.TargetName);
				if (s != null)
				{
					_ = SpellList.Remove(s);
				}
			});
		}

		public void TryRemoveTarget(string target)
		{
			if (string.IsNullOrWhiteSpace(target))
			{
				return;
			}

			target = target.CleanUpZealName();

			_appDispatcher.DispatchUI(() =>
			{
				var spellstormove = SpellList.Where(a => a.TargetName.ToLower() == target.ToLower()).ToList();
				foreach (var item in spellstormove)
				{
					Debug.WriteLine($"Removing {item.SpellName}");
					_ = SpellList.Remove(item);
				}
			});
		}

		private bool ShouldProduceTimer(UISpell item)
		{
			var d = DateTime.Now;
			var player = _activePlayer.Player;

			if ((item.SpellType != SpellTypes.RandomRoll && item.SpellType != SpellTypes.RespawnTimer && item.SpellType != SpellTypes.DeathTouch)
				&& YouOnlySpells && item.TargetName != EQSpells.SpaceYou)
			{
				return false;
			}
			//else if (item.MaxTimerEndDateTime != null && item.MaxTimerEndDateTime < DateTime.Now)
			//{
			//	return false;
			//}
			else if (item.NegativeDurationToShow.TotalSeconds <= 0 && !item.PersistentSpell)
			{
				return false;
			}
			else if (item.PersistentSpell && (d - item.UpdatedDateTime).TotalMinutes > 30)
			{
				return false;
			}
			else if (item.IsNPC && !_windowOptions.ShowNPCs)
			{
				return false;
			}
			else if (!item.IsNPC && !_windowOptions.ShowPCs)
			{
				return false;
			}

			item.HideGuesses = !_windowOptions.BestGuessSpells;
			item.ShowOnlyYou = _windowOptions.YouOnlySpells;
			item.HideClasses = player != null 
				&& SpellUIExtensions.HideSpell(player.ShowSpellsForClasses, item.Classes) 
				&& item.TargetName != EQSpells.SpaceYou
				&& !item.IsYourCast;
			if (item.SpellType == SpellTypes.RandomRoll)
			{
				item.HideClasses = !ShowRandomRolls;
			}
			if (item.SpellType == SpellTypes.DeathTouch 
				&& !_windowOptions.ShowDeathTouches)
			{
				return false;
			}
			if (item.SpellType == SpellTypes.RespawnTimer
				&& !ShowTimers)
			{
				return false;
			}
			else if (item.SpellType == SpellTypes.RandomRoll
			&& !ShowRandomRolls)
			{
				return false;
			}
			else if (!ShowModRodTimers
				&& item.SpellType == SpellTypes.ModRod)
			{
				return false;
			}
			else if (!ShowSpells
				&& item.SpellType != SpellTypes.RandomRoll
				&& item.SpellType != SpellTypes.RespawnTimer
				&& item.SpellType != SpellTypes.ModRod 
				&& !_windowOptions.ShowDeathTouches)
			{
				return false;
			}
			else if (!ShowSpells
				&& (item.SpellType == SpellTypes.Other
				|| item.SpellType == SpellTypes.Beneficial
				|| item.SpellType == SpellTypes.Detrimental))
			{
				return false;
			}

			return true;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		internal void RemoveCastingSpell(DateTime executionTime)
		{
			_appDispatcher.DispatchUI(() =>
			{

				var yourMostRecentSpell = SpellList.Where(s => s.IsYourCast).OrderBy(t => Math.Abs((t.ExecutionTime - executionTime).Ticks))
							 .FirstOrDefault();
				var lastExeTime = SpellList.Where(s => s.IsYourCast).OrderBy(t => Math.Abs((t.ExecutionTime - executionTime).Ticks)).FirstOrDefault()?.ExecutionTime;
				var timeDelta = (DateTime.Now - lastExeTime).Value.TotalMilliseconds;

				if (yourMostRecentSpell != null)
				{
					var latestSpell = _spells.AllSpells.FirstOrDefault(a => a.name == yourMostRecentSpell.SpellName);
					if(timeDelta < latestSpell.casttime)
					{
						Debug.WriteLine($"Removing {yourMostRecentSpell.SpellName}");
						_ = SpellList.Remove(yourMostRecentSpell);
					}
				}
			});
		}
	}
}
