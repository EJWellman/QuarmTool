using EQTool.EventArgModels;
using EQTool.Models;
using EQTool.Services;
using EQTool.ViewModels;
using EQToolShared;
using EQToolShared.Enums;
using EQToolShared.HubModels;
using EQToolShared.Map;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace EQTool
{
    public partial class TimerWindow : BaseSaveStateWindow
    {
        private readonly System.Timers.Timer UITimer;
        private readonly TimerWindowViewModel timerWindowViewModel;
        private readonly LogParser logParser;
        private readonly ActivePlayer activePlayer;
        private readonly TimersService timersService;
        private readonly PlayerTrackerService playerTrackerService;
		private readonly QuarmDataService _quarmDataService;

        public TimerWindow(
            PlayerTrackerService playerTrackerService,
            TimersService timersService,
            EQToolSettings settings,
            TimerWindowViewModel TimerWindowViewModel,
            LogParser logParser,
            EQToolSettingsLoad toolSettingsLoad,
            ActivePlayer activePlayer,
			QuarmDataService quarmDataService,
            LoggingService loggingService) : base(settings.TimerWindowState, toolSettingsLoad, settings)
        {
            loggingService.Log(string.Empty, EventType.OpenMap, activePlayer?.Player?.Server);
            this.playerTrackerService = playerTrackerService;
            this.timersService = timersService;
            this.logParser = logParser;
            this.activePlayer = activePlayer;
			_quarmDataService = quarmDataService;
            TimerWindowViewModel.SpellList = new System.Collections.ObjectModel.ObservableCollection<UISpell>();
            DataContext = this.timerWindowViewModel = TimerWindowViewModel;

            InitializeComponent();
            base.Init();
            this.logParser.SpellWornOtherOffEvent += LogParser_SpellWornOtherOffEvent;
            this.logParser.CampEvent += LogParser_CampEvent;
            this.logParser.EnteredWorldEvent += LogParser_EnteredWorldEvent;
            this.logParser.DeadEvent += LogParser_DeadEvent;
            this.logParser.StartTimerEvent += LogParser_StartTimerEvent;
            this.logParser.CancelTimerEvent += LogParser_CancelTimerEvent;
            this.logParser.POFDTEvent += LogParser_POFDTEvent;
            this.logParser.RandomRollEvent += LogParser_RandomRollEvent;
			this.logParser.ModRodUsedEvent += LogParser_ModRodUsedEvent;
            UITimer = new System.Timers.Timer(1000);
            UITimer.Elapsed += PollUI;
            UITimer.Enabled = true;
            var view = (ListCollectionView)CollectionViewSource.GetDefaultView(spelllistview.ItemsSource);
            view.GroupDescriptions.Add(new PropertyGroupDescription(nameof(UISpell.TargetName)));
            view.LiveGroupingProperties.Add(nameof(UISpell.TargetName));
            view.IsLiveGrouping = true;
            view.SortDescriptions.Add(new SortDescription(nameof(UISpell.Sorting), ListSortDirection.Ascending));
            view.SortDescriptions.Add(new SortDescription(nameof(UISpell.Roll), ListSortDirection.Descending));
            view.SortDescriptions.Add(new SortDescription(nameof(UISpell.SecondsLeftOnSpell), ListSortDirection.Ascending));
            view.IsLiveSorting = true;
            view.LiveSortingProperties.Add(nameof(UISpell.SecondsLeftOnSpell));
        }

        private void LogParser_RandomRollEvent(object sender, LogParser.RandomRollEventArgs e)
        {
            this.timerWindowViewModel.TryAddCustom(new CustomTimer
            {
                TargetName = $"Random -- {e.RandomRollData.MinRoll}-{e.RandomRollData.MaxRoll}",
                Name = e.RandomRollData.PlayerName,
                SpellNameIcon = "Invisibility",
                SpellType = SpellTypes.RandomRoll,
                Roll = e.RandomRollData.Roll,
                DurationInSeconds = 60 * 3,
				ExecutionTime = e.ExecutionTime
			});
        }

        private void LogParser_POFDTEvent(object sender, DTParser.DT_Event e)
        {
            this.timerWindowViewModel.TryAddCustom(new CustomTimer
            {
                DurationInSeconds = 45,
                Name = $"--DT-- '{e.DTReceiver}'",
                SpellNameIcon = "Disease Cloud",
                SpellType = SpellTypes.BadGuyCoolDown,
				ExecutionTime = e.ExecutionTime
            });
        }

        private void LogParser_CampEvent(object sender, LogParser.CampEventArgs e)
        {
            base.SaveState();
            timerWindowViewModel.ClearYouSpells();
        }

        private void LogParser_EnteredWorldEvent(object sender, LogParser.EnteredWorldArgs e)
        {
            timerWindowViewModel.ClearYouSpells();
        }

        private void LogParser_SpellWornOtherOffEvent(object sender, LogParser.SpellWornOffOtherEventArgs e)
        {
            timerWindowViewModel.TryRemoveUnambiguousSpellOther(e.SpellName);
        }

        private int deathcounter = 1;
        private void LogParser_DeadEvent(object sender, LogParser.DeadEventArgs e)
        {
            timerWindowViewModel.TryRemoveTarget(e.Name);
            if (playerTrackerService.IsPlayer(e.Name) || !MasterNPCList.NPCs.Contains(e.Name))
            {
                return;
			}
			var deathTimer = _quarmDataService.GetMonsterTimer(e.Name);
			if (deathTimer != null)
			{
				var add = new CustomTimer
				{
					Name = "--Dead-- " + e.Name,
					DurationInSeconds = (int)(deathTimer.RespawnTimer != 0 ? deathTimer.RespawnTimer : deathTimer.Min_RespawnTimer),
					NegativeDurationToShow = (int)(deathTimer.Max_RespawnTimer),
					SpellNameIcon = "Disease Cloud",
					SpellType = SpellTypes.RespawnTimer,
					TargetName = "Death Timers",
					ExecutionTime = e.ExecutionTime
				};

				var exisitngdeathentry = timerWindowViewModel.SpellList.FirstOrDefault(a => a.SpellName == add.Name && a.TargetName == "Death Timers");
				if (exisitngdeathentry != null)
				{
					deathcounter = ++deathcounter > 999 ? 1 : deathcounter;
					add.Name += "_" + deathcounter;
				}

				timerWindowViewModel.TryAddCustom(add);
			}
		}

		private void LogParser_ModRodUsedEvent(object sender, ModRodUsageArgs e)
		{
			this.timerWindowViewModel.TryAddCustom(new CustomTimer
			{
				DurationInSeconds = 300,
				Name = $"{e.Name}",
				SpellNameIcon = "Modulation",
				SpellType = SpellTypes.ModRod,
				TargetName = "Mod Rod Consumed",
				ExecutionTime = e.ExecutionTime
			});
		}

		private void LogParser_CancelTimerEvent(object sender, LogParser.CancelTimerEventArgs e)
        {
            timerWindowViewModel.TryRemoveCustom(e.Name);
        }

        private void LogParser_StartTimerEvent(object sender, LogParser.StartTimerEventArgs e)
        {
            timerWindowViewModel.TryAddCustom(e.CustomTimer);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            UITimer?.Stop();
            UITimer?.Dispose();
            if (logParser != null)
            {
                logParser.SpellWornOtherOffEvent -= LogParser_SpellWornOtherOffEvent;
                logParser.CampEvent -= LogParser_CampEvent;
                logParser.EnteredWorldEvent -= LogParser_EnteredWorldEvent;
                logParser.DeadEvent -= LogParser_DeadEvent;
                logParser.StartTimerEvent -= LogParser_StartTimerEvent;
                logParser.CancelTimerEvent -= LogParser_CancelTimerEvent;
                logParser.POFDTEvent -= LogParser_POFDTEvent;
                logParser.RandomRollEvent -= LogParser_RandomRollEvent;
				logParser.ModRodUsedEvent -= LogParser_ModRodUsedEvent;
            }
            if (timerWindowViewModel != null)
            {
                timerWindowViewModel.SpellList = new System.Collections.ObjectModel.ObservableCollection<UISpell>();
			}
			base.SaveState();
			base.OnClosing(e);
        }

        private void PollUI(object sender, EventArgs e)
        {
            timerWindowViewModel.UpdateSpells();
        }

        private void RemoveSingleItem(object sender, RoutedEventArgs e)
        {
            var name = (sender as Button).DataContext;
            timerWindowViewModel.SpellList.Remove(name as UISpell);
		}

		private void RemoveFromSpells(object sender, RoutedEventArgs e)
		{
			var name = ((sender as Button).DataContext as dynamic)?.Name as string;
			var items = timerWindowViewModel.SpellList.Where(a => a.TargetName == name).ToList();
			foreach (var item in items)
			{
				_ = timerWindowViewModel.SpellList.Remove(item);
			}
		}
	}
}
