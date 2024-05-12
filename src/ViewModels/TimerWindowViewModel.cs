using EQTool.Models;
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
    public class TimerWindowViewModel : INotifyPropertyChanged
    {
        private readonly ActivePlayer activePlayer;
        private readonly IAppDispatcher appDispatcher;
        private readonly EQToolSettings settings;
        private readonly EQSpells spells;

        public TimerWindowViewModel(ActivePlayer activePlayer, IAppDispatcher appDispatcher, EQToolSettings settings, EQSpells spells)
        {

            this.activePlayer = activePlayer;
            this.appDispatcher = appDispatcher;
            this.settings = settings;
            Title = "Timers v" + App.Version;
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
					if(item.MaxTimerEndDateTime != null && item.MaxTimerEndDateTime < d)
					{
						itemsToRemove.Add(item);
					}
                    else if (item.NegativeDurationToShow.TotalSeconds <= 0 && !item.PersistentSpell)
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
                        item.HideClasses = !this.settings.ShowRandomRolls;
					}

					if (!this.settings.ShowModRodTimers
						&& item.SpellType == SpellTypes.ModRod)
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

				SpellList.Add(new UISpell(endTime, negativeEndTime, false)
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
					ExecutionTime = match.ExecutionTime
                });
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
