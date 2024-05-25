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
    public partial class BaseTimerWindow : BaseSaveStateWindow
    {
        private readonly System.Timers.Timer _uiTimer;
        private readonly BaseTimerWindowViewModel _baseTimerWindowViewModel;
        private readonly LogParser _logParser;
        private readonly ActivePlayer _activePlayer;
        private readonly PlayerTrackerService _playerTrackerService;
		private readonly QuarmDataService _quarmDataService;

		public BaseTimerWindow(BaseTimerWindowViewModel viewModel,
			EQToolSettingsLoad toolSettingsLoad,
			EQToolSettings settings) : base(viewModel.WindowState, toolSettingsLoad, settings)
		{
			DataContext = _baseTimerWindowViewModel = viewModel;
			_uiTimer = new System.Timers.Timer(1000);
			_uiTimer.Elapsed += PollUI;
			_uiTimer.Enabled = true;
		}

		public void Setup()
		{

			InitializeComponent();
			//this._playerTrackerService = playerTrackerService;
			//this._logParser = logParser;
			//this._activePlayer = activePlayer;
			//_quarmDataService = quarmDataService;
			_baseTimerWindowViewModel.SpellList = new System.Collections.ObjectModel.ObservableCollection<UISpell>();
			if (this._activePlayer.Player != null)
			{
				_baseTimerWindowViewModel.AddSavedYouSpells(this._activePlayer.Player.YouSpells);
			}

			InitializeComponent();
			base.Init();
			this._logParser.SpellWornOtherOffEvent += LogParser_SpellWornOtherOffEvent;
			this._logParser.CampEvent += LogParser_CampEvent;
			this._logParser.EnteredWorldEvent += LogParser_EnteredWorldEvent;
			this._logParser.SpellWornOffSelfEvent += LogParser_SpellWornOffSelfEvent;
			this._logParser.StartCastingEvent += LogParser_StartCastingEvent;
			this._logParser.DeadEvent += LogParser_DeadEvent;
			this._logParser.StartTimerEvent += LogParser_StartTimerEvent;
			this._logParser.CancelTimerEvent += LogParser_CancelTimerEvent;
			this._logParser.POFDTEvent += LogParser_POFDTEvent;
			this._logParser.ResistSpellEvent += LogParser_ResistSpellEvent;
			this._logParser.RandomRollEvent += LogParser_RandomRollEvent;
			this._logParser.ModRodUsedEvent += LogParser_ModRodUsedEvent;
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

        public BaseTimerWindow(
            PlayerTrackerService playerTrackerService,
            EQToolSettings settings,
            BaseTimerWindowViewModel BaseTimerWindowViewModel,
            LogParser logParser,
            EQToolSettingsLoad toolSettingsLoad,
            ActivePlayer activePlayer,
			QuarmDataService quarmDataService,
            LoggingService loggingService) : base(BaseTimerWindowViewModel.WindowState, toolSettingsLoad, settings)
        {
            loggingService.Log(string.Empty, EventType.OpenMap, activePlayer?.Player?.Server);
            this._playerTrackerService = playerTrackerService;
            this._logParser = logParser;
            this._activePlayer = activePlayer;
			_quarmDataService = quarmDataService;
            BaseTimerWindowViewModel.SpellList = new System.Collections.ObjectModel.ObservableCollection<UISpell>();
            DataContext = this._baseTimerWindowViewModel = BaseTimerWindowViewModel;
            if (this._activePlayer.Player != null)
            {
                BaseTimerWindowViewModel.AddSavedYouSpells(this._activePlayer.Player.YouSpells);
            }

            InitializeComponent();
			base.Init();
			this._logParser.SpellWornOtherOffEvent += LogParser_SpellWornOtherOffEvent;
			this._logParser.CampEvent += LogParser_CampEvent;
			this._logParser.EnteredWorldEvent += LogParser_EnteredWorldEvent;
			this._logParser.SpellWornOffSelfEvent += LogParser_SpellWornOffSelfEvent;
			this._logParser.StartCastingEvent += LogParser_StartCastingEvent;
			this._logParser.DeadEvent += LogParser_DeadEvent;
			this._logParser.StartTimerEvent += LogParser_StartTimerEvent;
			this._logParser.CancelTimerEvent += LogParser_CancelTimerEvent;
			this._logParser.POFDTEvent += LogParser_POFDTEvent;
			this._logParser.ResistSpellEvent += LogParser_ResistSpellEvent;
			this._logParser.RandomRollEvent += LogParser_RandomRollEvent;
			this._logParser.ModRodUsedEvent += LogParser_ModRodUsedEvent;
			_uiTimer = new System.Timers.Timer(1000);
			_uiTimer.Elapsed += PollUI;
			_uiTimer.Enabled = true;
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
			this._baseTimerWindowViewModel.TryAddCustom(new CustomTimer
			{
				TargetName = $"Random -- {e.RandomRollData.MaxRoll}",
				Name = e.RandomRollData.PlayerName,
				SpellNameIcon = "Invisibility",
				SpellType = EQToolShared.Enums.SpellTypes.RandomRoll,
				Roll = e.RandomRollData.Roll,
				DurationInSeconds = 60 * 3,
				ExecutionTime = e.ExecutionTime
			});
		}

		private void LogParser_ResistSpellEvent(object sender, ResistSpellParser.ResistSpellData e)
		{
			if (e.isYou)
			{
				_baseTimerWindowViewModel.TryAdd(new SpellParsingMatch
				{
					IsYou = e.isYou,
					Spell = e.Spell,
					MultipleMatchesFound = false,
					TargetName = EQSpells.SpaceYou
				}, true);
			}
		}

		private void LogParser_POFDTEvent(object sender, POFDTParser.POF_DT_Event e)
		{
			this._baseTimerWindowViewModel.TryAddCustom(new CustomTimer
			{
				DurationInSeconds = 45,
				Name = $"--DT-- '{e.DTReceiver}'",
				SpellNameIcon = "Disease Cloud",
				SpellType = EQToolShared.Enums.SpellTypes.BadGuyCoolDown,
				ExecutionTime = e.ExecutionTime
			});
		}

		private void LogParser_CampEvent(object sender, LogParser.CampEventArgs e)
		{
			TrySaveYouSpellData();
			base.SaveState();
			_baseTimerWindowViewModel.ClearYouSpells();
		}

		private void LogParser_EnteredWorldEvent(object sender, LogParser.EnteredWorldArgs e)
		{
			_baseTimerWindowViewModel.ClearYouSpells();
			if (_activePlayer.Player != null)
			{
				_baseTimerWindowViewModel.AddSavedYouSpells(_activePlayer.Player.YouSpells);
			}
		}

		private void LogParser_SpellWornOffSelfEvent(object sender, LogParser.SpellWornOffSelfEventArgs e)
		{
			_baseTimerWindowViewModel.TryRemoveUnambiguousSpellSelf(e.SpellNames);
		}

		private void LogParser_SpellWornOtherOffEvent(object sender, LogParser.SpellWornOffOtherEventArgs e)
		{
			_baseTimerWindowViewModel.TryRemoveUnambiguousSpellOther(e.SpellName);
		}

		private void LogParser_StartCastingEvent(object sender, LogParser.SpellEventArgs e)
		{
			_baseTimerWindowViewModel.TryAdd(e.Spell, false);
		}

		private int deathcounter = 1;
		private void LogParser_DeadEvent(object sender, LogParser.DeadEventArgs e)
		{
			_baseTimerWindowViewModel.TryRemoveTarget(e.Name);
			if (_playerTrackerService.IsPlayer(e.Name) || !MasterNPCList.NPCs.Contains(e.Name))
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

				var exisitngdeathentry = _baseTimerWindowViewModel.SpellList.FirstOrDefault(a => a.SpellName == add.Name && a.TargetName == "Death Timers");
				if (exisitngdeathentry != null)
				{
					deathcounter = ++deathcounter > 999 ? 1 : deathcounter;
					add.Name += "_" + deathcounter;
				}

				_baseTimerWindowViewModel.TryAddCustom(add);
			}
		}

		private void LogParser_ModRodUsedEvent(object sender, ModRodUsageArgs e)
		{
			this._baseTimerWindowViewModel.TryAddCustom(new CustomTimer
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
			_baseTimerWindowViewModel.TryRemoveCustom(e.Name);
		}

		private void LogParser_StartTimerEvent(object sender, LogParser.StartTimerEventArgs e)
		{
			_baseTimerWindowViewModel.TryAddCustom(e.CustomTimer);
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			_uiTimer?.Stop();
			_uiTimer?.Dispose();
			if (_logParser != null)
			{
				_logParser.SpellWornOtherOffEvent -= LogParser_SpellWornOtherOffEvent;
				_logParser.CampEvent -= LogParser_CampEvent;
				_logParser.EnteredWorldEvent -= LogParser_EnteredWorldEvent;
				_logParser.SpellWornOffSelfEvent -= LogParser_SpellWornOffSelfEvent;
				_logParser.StartCastingEvent -= LogParser_StartCastingEvent;
				_logParser.DeadEvent -= LogParser_DeadEvent;
				_logParser.StartTimerEvent -= LogParser_StartTimerEvent;
				_logParser.CancelTimerEvent -= LogParser_CancelTimerEvent;
				_logParser.POFDTEvent -= LogParser_POFDTEvent;
				_logParser.ResistSpellEvent -= LogParser_ResistSpellEvent;
				_logParser.RandomRollEvent -= LogParser_RandomRollEvent;
				_logParser.ModRodUsedEvent -= LogParser_ModRodUsedEvent;
			}
			if (_baseTimerWindowViewModel != null)
			{
				_baseTimerWindowViewModel.SpellList = new System.Collections.ObjectModel.ObservableCollection<UISpell>();
			}
			base.SaveState();
			base.OnClosing(e);
		}

		private void TrySaveYouSpellData()
		{
			if (_activePlayer?.Player != null)
			{
				var before = _activePlayer.Player.YouSpells ?? new System.Collections.Generic.List<YouSpells>();
				_activePlayer.Player.YouSpells = _baseTimerWindowViewModel.SpellList.Where(a => a.TargetName == EQSpells.SpaceYou).Select(a => new YouSpells
				{
					Name = a.SpellName,
					TotalSecondsLeft = (int)a.SecondsLeftOnSpell.TotalSeconds,
				}).ToList();
			}
		}

		private void PollUI(object sender, EventArgs e)
		{
			_baseTimerWindowViewModel.UpdateSpells();
		}

		private void RemoveSingleItem(object sender, RoutedEventArgs e)
		{
			var name = (sender as Button).DataContext;
			_baseTimerWindowViewModel.SpellList.Remove(name as UISpell);
		}

		private void RemoveFromSpells(object sender, RoutedEventArgs e)
		{
			var name = ((sender as Button).DataContext as dynamic)?.Name as string;
			var items = _baseTimerWindowViewModel.SpellList.Where(a => a.TargetName == name).ToList();
			foreach (var item in items)
			{
				_ = _baseTimerWindowViewModel.SpellList.Remove(item);
			}
		}
	}
}