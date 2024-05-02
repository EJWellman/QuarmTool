﻿using EQTool.Models;
using EQTool.Services;
using EQTool.Services.Spells;
using EQToolShared;
using EQToolShared.Enums;
using EQToolShared.HubModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace EQTool.ViewModels
{
    public class ComboTimerWindowViewModel : INotifyPropertyChanged
    {
        private readonly ActivePlayer activePlayer;
        private readonly IAppDispatcher appDispatcher;
        private readonly EQToolSettings settings;
        private readonly EQSpells spells;

        public ComboTimerWindowViewModel(ActivePlayer activePlayer, IAppDispatcher appDispatcher, EQToolSettings settings, EQSpells spells)
        {

            this.activePlayer = activePlayer;
            this.appDispatcher = appDispatcher;
            this.settings = settings;
            Title = "Combined Timers v" + App.Version;
            this.spells = spells;
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

        private string _Title = null;
        public string Title
        {
            get => _Title;
            set
            {
                _Title = value;
                OnPropertyChanged();
            }
        }

        public void UpdateStuff()
        {
            OnPropertyChanged(nameof(SpellList));
        }

        private long? _LastReadOffset = null;
        public long? LastReadOffset
        {
            get => _LastReadOffset;
            set
            {
                _LastReadOffset = value;
                OnPropertyChanged();
            }
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
            appDispatcher.DispatchUI(() =>
            {
                var player = activePlayer.Player;
                var itemsToRemove = new List<UISpell>();

                var d = DateTime.Now;
                foreach (var item in SpellList)
                {
                    item.UpdateTimeLeft();
					if((item.SpellType != SpellTypes.RandomRoll	&& item.SpellType != SpellTypes.RespawnTimer)
						&& settings.YouOnlySpells && item.TargetName != EQSpells.SpaceYou)
					{
						itemsToRemove.Add(item);
					}
					if (item.NegativeDurationToShow.TotalSeconds <= 0 && !item.PersistentSpell)
					{
                        itemsToRemove.Add(item);
                    }
                    else if (item.PersistentSpell && (d - item.UpdatedDateTime).TotalMinutes > 30)
                    {
                        itemsToRemove.Add(item);
                    }
                    item.HideGuesses = !settings.BestGuessSpells;
                    item.ShowOnlyYou = settings.YouOnlySpells;
                    item.HideClasses = player != null && SpellUIExtensions.HideSpell(player.ShowSpellsForClasses, item.Classes) && item.TargetName != EQSpells.SpaceYou;
                    if (item.SpellType == SpellTypes.RandomRoll)
                    {
                        item.HideClasses = !this.settings.ComboShowRandomRolls;
					}
					if (item.SpellType == SpellTypes.RespawnTimer
						&& !this.settings.ComboShowTimers)
					{
						itemsToRemove.Add(item);
					}
					else if (item.SpellType == SpellTypes.RandomRoll
					&& !this.settings.ComboShowRandomRolls)
					{
						itemsToRemove.Add(item);
					}
					else if (!this.settings.ComboShowSpells
						&& (item.SpellType != SpellTypes.RandomRoll
							&& item.SpellType != SpellTypes.RespawnTimer))
					{
						itemsToRemove.Add(item);
					}
				}

                var groupedspells = SpellList.GroupBy(a => a.TargetName).ToList();
                foreach (var spells in groupedspells)
                {
                    var allspellshidden = true;
                    foreach (var spell in spells)
                    {
                        if (spell.ColumnVisibility != System.Windows.Visibility.Collapsed)
                        {
                            allspellshidden = false;
                        }
                    }

                    if (allspellshidden)
                    {
                        foreach (var spell in spells)
                        {
                            spell.HeaderVisibility = System.Windows.Visibility.Collapsed;
                        }
                    }
                    else
                    {
                        foreach (var spell in spells)
                        {
                            spell.HeaderVisibility = System.Windows.Visibility.Visible;
                        }
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
            appDispatcher.DispatchUI(() =>
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

        public void TryAdd(SpellParsingMatch match, bool resisted)
        {
            if (match?.Spell == null)
            {
                return;
            }

            appDispatcher.DispatchUI(() =>
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
                    var playerlevel = this.activePlayer.Player.Level;
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
                var needscount = SpellsThatNeedCounts.Contains(spellname);
                var spellduration = TimeSpan.FromSeconds(SpellDurations.GetDuration_inSeconds(match.Spell, activePlayer.Player));
                var duration = needscount ? 0 : match.TotalSecondsOverride ?? spellduration.TotalSeconds;
                var isnpc = MasterNPCList.NPCs.Contains(match.TargetName);
                var uispell = new UISpell(DateTime.Now.AddSeconds((int)duration), DateTime.Now.AddSeconds((int)duration), isnpc)
                {
                    UpdatedDateTime = DateTime.Now,
                    PercentLeftOnSpell = 100,
                    SpellType = match.Spell.type,
                    TargetName = match.TargetName,
                    SpellName = spellname,
                    Rect = match.Spell.Rect,
                    PersistentSpell = needscount,
                    Counter = needscount ? 1 : (int?)null,
                    SpellIcon = match.Spell.SpellIcon,
                    Classes = match.Spell.Classes,
                    GuessedSpell = match.MultipleMatchesFound
                };
                var s = SpellList.FirstOrDefault(a => a.SpellName == spellname && match.TargetName == a.TargetName);
                if (s != null)
                {
                    if (needscount)
                    {
                        s.Counter += 1;
                        s.UpdatedDateTime = DateTime.Now;
                    }
                    else
                    {
                        _ = SpellList.Remove(s);
                        SpellList.Add(uispell);
                    }
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

            appDispatcher.DispatchUI(() =>
            {
                var s = SpellList.FirstOrDefault(a => a.SpellName == match.Name && match.TargetName == a.TargetName);
                if (s != null)
                {
                    if (match.SpellType != SpellTypes.RandomRoll)
                    {
                        _ = SpellList.Remove(s);
                    }
                }

                var spellduration = match.DurationInSeconds;
				var negativeDuration = match.NegativeDurationToShow;
				var spellicon = spells.AllSpells.FirstOrDefault(a => a.name == match.SpellNameIcon);
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

				SpellList.Add(new UISpell(DateTime.Now.AddSeconds(spellduration), DateTime.Now.AddSeconds(negativeDuration > spellduration ? negativeDuration : spellduration), false)
				{
					UpdatedDateTime = DateTime.Now,
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
                    RollOrder = rollorder + 1
                });
				string blah = "";
            });
        }

        public void AddSavedYouSpells(List<YouSpells> youspells)
        {
            if (youspells == null || !youspells.Any())
            {
                return;
            }

            appDispatcher.DispatchUI(() =>
            {
                foreach (var item in youspells)
                {
                    var match = spells.AllSpells.FirstOrDefault(a => a.name == item.Name);
                    if (match != null)
                    {
                        var spellduration = TimeSpan.FromSeconds(SpellDurations.GetDuration_inSeconds(match, activePlayer.Player));
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
                            GuessedSpell = false
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

            appDispatcher.DispatchUI(() =>
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

            appDispatcher.DispatchUI(() =>
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

            appDispatcher.DispatchUI(() =>
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

            appDispatcher.DispatchUI(() =>
            {
                var spellstormove = SpellList.Where(a => a.TargetName.ToLower() == target.ToLower()).ToList();
                foreach (var item in spellstormove)
                {
                    Debug.WriteLine($"Removing {item.SpellName}");
                    _ = SpellList.Remove(item);
                }
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
