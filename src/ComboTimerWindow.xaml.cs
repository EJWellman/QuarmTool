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
    public partial class ComboTimerWindow : BaseSaveStateWindow
    {
        private readonly System.Timers.Timer UITimer;
        private readonly ComboTimerWindowViewModel comboTimerWindowViewModel;
        private readonly LogParser logParser;
        private readonly ActivePlayer activePlayer;
        private readonly TimersService timersService;
        private readonly PlayerTrackerService playerTrackerService;
		private readonly QuarmDataService _quarmDataService;

        public ComboTimerWindow(
            PlayerTrackerService playerTrackerService,
            TimersService timersService,
            EQToolSettings settings,
            ComboTimerWindowViewModel comboTimerWindowViewModel,
            LogParser logParser,
            EQToolSettingsLoad toolSettingsLoad,
            ActivePlayer activePlayer,
			QuarmDataService quarmDataService,
            LoggingService loggingService) : base(settings.SpellWindowState, toolSettingsLoad, settings)
        {
            loggingService.Log(string.Empty, EventType.OpenMap, activePlayer?.Player?.Server);
            this.playerTrackerService = playerTrackerService;
            this.timersService = timersService;
            this.logParser = logParser;
            this.activePlayer = activePlayer;
			_quarmDataService = quarmDataService;
            comboTimerWindowViewModel.SpellList = new System.Collections.ObjectModel.ObservableCollection<UISpell>();
            DataContext = this.comboTimerWindowViewModel = comboTimerWindowViewModel;
            if (this.activePlayer.Player != null)
            {
                comboTimerWindowViewModel.AddSavedYouSpells(this.activePlayer.Player.YouSpells);
            }

            InitializeComponent();
			base.Init();
			this.logParser.SpellWornOtherOffEvent += LogParser_SpellWornOtherOffEvent;
			this.logParser.CampEvent += LogParser_CampEvent;
			this.logParser.EnteredWorldEvent += LogParser_EnteredWorldEvent;
			this.logParser.SpellWornOffSelfEvent += LogParser_SpellWornOffSelfEvent;
			this.logParser.StartCastingEvent += LogParser_StartCastingEvent;
			this.logParser.DeadEvent += LogParser_DeadEvent;
			this.logParser.StartTimerEvent += LogParser_StartTimerEvent;
			this.logParser.CancelTimerEvent += LogParser_CancelTimerEvent;
			this.logParser.POFDTEvent += LogParser_POFDTEvent;
			this.logParser.ResistSpellEvent += LogParser_ResistSpellEvent;
			this.logParser.RandomRollEvent += LogParser_RandomRollEvent;
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
			this.comboTimerWindowViewModel.TryAddCustom(new CustomTimer
			{
				TargetName = $"Random -- {e.RandomRollData.MaxRoll}",
				Name = e.RandomRollData.PlayerName,
				SpellNameIcon = "Invisibility",
				SpellType = EQToolShared.Enums.SpellTypes.RandomRoll,
				Roll = e.RandomRollData.Roll,
				DurationInSeconds = 60 * 3
			});
		}

		private void LogParser_ResistSpellEvent(object sender, ResistSpellParser.ResistSpellData e)
		{
			if (e.isYou)
			{
				comboTimerWindowViewModel.TryAdd(new SpellParsingMatch
				{
					IsYou = e.isYou,
					Spell = e.Spell,
					MultipleMatchesFound = false,
					TargetName = EQSpells.SpaceYou,
				}, true);
			}
		}

		private void LogParser_POFDTEvent(object sender, POFDTParser.POF_DT_Event e)
		{
			this.comboTimerWindowViewModel.TryAddCustom(new CustomTimer
			{
				DurationInSeconds = 45,
				Name = $"--DT-- '{e.DTReceiver}'",
				SpellNameIcon = "Disease Cloud",
				SpellType = EQToolShared.Enums.SpellTypes.BadGuyCoolDown
			});
		}

		private void LogParser_CampEvent(object sender, LogParser.CampEventArgs e)
		{
			TrySaveYouSpellData();
			base.SaveState();
			comboTimerWindowViewModel.ClearYouSpells();
		}

		private void LogParser_EnteredWorldEvent(object sender, LogParser.EnteredWorldArgs e)
		{
			comboTimerWindowViewModel.ClearYouSpells();
			if (activePlayer.Player != null)
			{
				comboTimerWindowViewModel.AddSavedYouSpells(activePlayer.Player.YouSpells);
			}
		}

		private void LogParser_SpellWornOffSelfEvent(object sender, LogParser.SpellWornOffSelfEventArgs e)
		{
			comboTimerWindowViewModel.TryRemoveUnambiguousSpellSelf(e.SpellNames);
		}

		private void LogParser_SpellWornOtherOffEvent(object sender, LogParser.SpellWornOffOtherEventArgs e)
		{
			comboTimerWindowViewModel.TryRemoveUnambiguousSpellOther(e.SpellName);
		}

		private void LogParser_StartCastingEvent(object sender, LogParser.SpellEventArgs e)
		{
			comboTimerWindowViewModel.TryAdd(e.Spell, false);
		}

		private int deathcounter = 1;
		private void LogParser_DeadEvent(object sender, LogParser.DeadEventArgs e)
		{
			comboTimerWindowViewModel.TryRemoveTarget(e.Name);
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
					TargetName = "Death Timers"
			};

				var exisitngdeathentry = comboTimerWindowViewModel.SpellList.FirstOrDefault(a => a.SpellName == add.Name && CustomTimer.CustomerTime == a.TargetName);
				if (exisitngdeathentry != null)
				{
					deathcounter = ++deathcounter > 999 ? 1 : deathcounter;
					add.Name += "_" + deathcounter;
				}

				comboTimerWindowViewModel.TryAddCustom(add);
			}
		}

		private void LogParser_CancelTimerEvent(object sender, LogParser.CancelTimerEventArgs e)
		{
			comboTimerWindowViewModel.TryRemoveCustom(e.Name);
		}

		private void LogParser_StartTimerEvent(object sender, LogParser.StartTimerEventArgs e)
		{
			comboTimerWindowViewModel.TryAddCustom(e.CustomTimer);
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
				logParser.SpellWornOffSelfEvent -= LogParser_SpellWornOffSelfEvent;
				logParser.StartCastingEvent -= LogParser_StartCastingEvent;
				logParser.DeadEvent -= LogParser_DeadEvent;
				logParser.StartTimerEvent -= LogParser_StartTimerEvent;
				logParser.CancelTimerEvent -= LogParser_CancelTimerEvent;
				logParser.POFDTEvent -= LogParser_POFDTEvent;
				logParser.ResistSpellEvent -= LogParser_ResistSpellEvent;
				logParser.RandomRollEvent -= LogParser_RandomRollEvent;
			}
			if (comboTimerWindowViewModel != null)
			{
				comboTimerWindowViewModel.SpellList = new System.Collections.ObjectModel.ObservableCollection<UISpell>();
			}
			base.OnClosing(e);
		}

		private void TrySaveYouSpellData()
		{
			if (activePlayer?.Player != null)
			{
				var before = activePlayer.Player.YouSpells ?? new System.Collections.Generic.List<YouSpells>();
				activePlayer.Player.YouSpells = comboTimerWindowViewModel.SpellList.Where(a => a.TargetName == EQSpells.SpaceYou).Select(a => new YouSpells
				{
					Name = a.SpellName,
					TotalSecondsLeft = (int)a.SecondsLeftOnSpell.TotalSeconds,
				}).ToList();
			}
		}

		private void PollUI(object sender, EventArgs e)
		{
			comboTimerWindowViewModel.UpdateSpells();
		}

		private void RemoveSingleItem(object sender, RoutedEventArgs e)
		{
			var name = (sender as Button).DataContext;
			comboTimerWindowViewModel.SpellList.Remove(name as UISpell);
		}

		private void RemoveFromSpells(object sender, RoutedEventArgs e)
		{
			var name = ((sender as Button).DataContext as dynamic)?.Name as string;
			var items = comboTimerWindowViewModel.SpellList.Where(a => a.TargetName == name).ToList();
			foreach (var item in items)
			{
				_ = comboTimerWindowViewModel.SpellList.Remove(item);
			}
		}
	}
}