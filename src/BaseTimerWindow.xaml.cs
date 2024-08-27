using EQTool.EventArgModels;
using EQTool.Models;
using EQTool.Services;
using EQTool.ViewModels;
using EQToolShared;
using EQToolShared.Enums;
using EQToolShared.ExtendedClasses;
using EQToolShared.HubModels;
using EQToolShared.Map;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
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
		private readonly PipeParser _pipeParser;
        private readonly ActivePlayer _activePlayer;
        private readonly PlayerTrackerService _playerTrackerService;
		private readonly QuarmDataService _quarmDataService;
		private readonly EQToolSettings _settings;
		private readonly LoggingService _logging;

        public BaseTimerWindow(
            PlayerTrackerService playerTrackerService,
            EQToolSettings settings,
            BaseTimerWindowViewModel baseTimerWindowViewModel,
            LogParser logParser,
			PipeParser pipeParser,
            EQToolSettingsLoad toolSettingsLoad,
            ActivePlayer activePlayer,
			QuarmDataService quarmDataService,
            LoggingService loggingService) : base(baseTimerWindowViewModel.WindowState, toolSettingsLoad, settings)
        {
            loggingService.Log(string.Empty, EventType.OpenTriggers, activePlayer?.Player?.Server);
            _playerTrackerService = playerTrackerService;
            _logParser = logParser;
			_pipeParser = pipeParser;
            _activePlayer = activePlayer;
			_quarmDataService = quarmDataService;
			_settings = settings;
			_logging = loggingService;
            baseTimerWindowViewModel.SpellList = new System.Collections.ObjectModel.ObservableCollection<UISpell>();
            DataContext = _baseTimerWindowViewModel = baseTimerWindowViewModel;
            if (_activePlayer.Player != null)
            {
                baseTimerWindowViewModel.AddSavedYouSpells(_activePlayer.Player.YouSpells);
            }

            InitializeComponent();
			base.Init();
			_logParser.SpellWornOtherOffEvent += LogParser_SpellWornOtherOffEvent;
			_logParser.CampEvent += LogParser_CampEvent;
			_logParser.EnteredWorldEvent += LogParser_EnteredWorldEvent;
			_logParser.SpellWornOffSelfEvent += LogParser_SpellWornOffSelfEvent;
			_logParser.StartCastingEvent += LogParser_StartCastingEvent;
			
			_pipeParser.StartCastingEvent += _pipeParser_StartCastingEvent;
			_pipeParser.FizzleCastingEvent += _pipeParser_FizzleCastingEvent;
			_pipeParser.InterruptCastingEvent += _pipParser_InterruptCastingEvent;

			_logParser.DeadEvent += LogParser_DeadEvent;
			_logParser.StartTimerEvent += LogParser_StartTimerEvent;
			_logParser.CancelTimerEvent += LogParser_CancelTimerEvent;
			_logParser.DTEvent += LogParser_DTEvent;
			_logParser.ResistSpellEvent += LogParser_ResistSpellEvent;
			_logParser.RandomRollEvent += LogParser_RandomRollEvent;
			_logParser.ModRodUsedEvent += LogParser_ModRodUsedEvent;
			LocationChanged += Window_LocationChanged;
			SizeChanged += Window_SizeChanged;
			Loaded += BaseTimerWindow_Opened;
			Activated += OnActivated;
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

			Topmost = baseTimerWindowViewModel._windowOptions.AlwaysOnTop;
		}

		private void _pipeParser_FizzleCastingEvent(object sender, PipeParser.FizzleEventArgs e)
		{
			_baseTimerWindowViewModel.RemoveCastingSpell(e.ExecutionTime);
		}
		private void _pipParser_InterruptCastingEvent(object sender, PipeParser.InterruptEventArgs e)
		{
			_baseTimerWindowViewModel.RemoveCastingSpell(e.ExecutionTime);
		}

		private void _pipeParser_StartCastingEvent(object sender, PipeParser.SpellEventArgs e)
		{
			_baseTimerWindowViewModel.TryAdd(e.Spell, false);
		}

		private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			e.Handled = true;
			KeepWindowOnScreen(sender);
			BaseTimerWindowViewModel vm = ((BaseTimerWindowViewModel)((Window)sender).DataContext);
			var timerOptions = TimerWindowService.LoadTimerWindow(vm.ID);
			if (timerOptions != null)
			{
				timerOptions.WindowRect = ((Window)sender).Top + "," + ((Window)sender).Left + "," + ((Window)sender).Width + "," + ((Window)sender).Height;
				_settings.TimerWindows.FirstOrDefault(tw => tw.ID == timerOptions.ID).WindowRect = timerOptions.WindowRect;
			}

			TimerWindowService.UpdateTimerWindow(timerOptions);

			LastWindowInteraction = DateTime.UtcNow;
		}

		private void BaseTimerWindow_Opened(object sender, EventArgs e)
		{
			KeepWindowOnScreen(sender);
			BaseTimerWindowViewModel vm = ((BaseTimerWindowViewModel)((Window)sender).DataContext);
			var timerOptions = TimerWindowService.LoadTimerWindow(vm.ID);
			if (timerOptions != null)
			{
				timerOptions.Closed = false;
			}
			TimerWindowService.UpdateTimerWindow(timerOptions);
		}

		private void Window_LocationChanged(object sender, EventArgs e)
		{
			BaseTimerWindowViewModel vm = ((BaseTimerWindowViewModel)((Window)sender).DataContext);
			var timerOptions = TimerWindowService.LoadTimerWindow(vm.ID);
			if (timerOptions != null)
			{
				timerOptions.WindowRect = ((Window)sender).Top + "," + ((Window)sender).Left + "," + ((Window)sender).Width + "," + ((Window)sender).Height;
				_settings.TimerWindows.FirstOrDefault(tw => tw.ID == timerOptions.ID).WindowRect = timerOptions.WindowRect;
			}

			TimerWindowService.UpdateTimerWindow(timerOptions);

			LastWindowInteraction = DateTime.UtcNow;
		}

		private void KeepWindowOnScreen(object sender)
		{
			if (((Window)sender).Top <= SystemParameters.VirtualScreenTop)
			{
				((Window)sender).Top = SystemParameters.VirtualScreenTop + 1;
			}
			if (((Window)sender).Left < SystemParameters.VirtualScreenLeft)
			{
				((Window)sender).Left = SystemParameters.VirtualScreenLeft + 1;
			}
			if (((Window)sender).Top + ((Window)sender).Height > SystemParameters.VirtualScreenHeight)
			{
				((Window)sender).Top = SystemParameters.VirtualScreenHeight - ((Window)sender).Height - 1;
			}
			if (((Window)sender).Left + ((Window)sender).Width > SystemParameters.VirtualScreenLeft + SystemParameters.VirtualScreenWidth)
			{
				((Window)sender).Left = SystemParameters.VirtualScreenWidth + SystemParameters.VirtualScreenLeft - ((Window)sender).Width - 1;
			}

			if(WindowIsOffScreen((Window)sender))
			{
				((Window)sender).Top = SystemParameters.VirtualScreenTop + 1;
				((Window)sender).Left = SystemParameters.VirtualScreenLeft + 1;
			}
			if(WindowIsTooTall((Window)sender))
			{
				((Window)sender).Height = 350;
			}
			if (WindowIsTooWide((Window)sender))
			{
				((Window)sender).Width = 250;
			}
		}

		private bool WindowIsOffScreen(Window sender)
		{
			return sender.Top < SystemParameters.VirtualScreenTop 
				|| sender.Left < SystemParameters.VirtualScreenLeft 
				|| sender.Top + sender.Height > SystemParameters.VirtualScreenHeight 
				|| sender.Left + sender.Width > SystemParameters.VirtualScreenWidth + SystemParameters.VirtualScreenLeft;
		}
		private bool WindowIsTooTall(Window sender)
		{
			return sender.Height > SystemParameters.VirtualScreenHeight;
		}
		private bool WindowIsTooWide(Window sender)
		{
			return sender.Width > SystemParameters.VirtualScreenWidth;
		}

		private void LogParser_RandomRollEvent(object sender, LogParser.RandomRollEventArgs e)
		{
			_baseTimerWindowViewModel.TryAddCustom(new CustomTimer
			{
				TargetName = $"Random -- {e.RandomRollData.MinRoll}-{e.RandomRollData.MaxRoll}",
				Name = e.RandomRollData.PlayerName,
				SpellNameIcon = "Invisibility",
				SpellType = SpellTypes.RandomRoll,
				Roll = e.RandomRollData.Roll,
				DurationInSeconds = 60 * 3,
				ExecutionTime = e.ExecutionTime,
				IsNPC = false
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

		private void LogParser_DTEvent(object sender, DTParser.DT_Event e)
		{
			this._baseTimerWindowViewModel.TryAddCustom(new CustomTimer
			{
				TargetName = "Death Touches",
				DurationInSeconds = 45,
				Name = $"--DT-- {e.NpcName} > {e.DTReceiver}",
				SpellNameIcon = "Disease Cloud",
				SpellType = EQToolShared.Enums.SpellTypes.DeathTouch,
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
			try
			{
				_baseTimerWindowViewModel.TryRemoveTarget(e.Name);
				string name = e.Name.CleanUpZealName();
				string cleanName = e.Name.CleanUpZealName(true);
				if (_playerTrackerService.IsPlayer(cleanName) || !MasterNPCList.NPCs.Contains(cleanName))
				{
					return;
				}
				var deathTimer = _quarmDataService.GetMonsterTimer(cleanName);
				if (deathTimer != null)
				{
					var add = new CustomTimer
					{
						Name = "--Dead-- " + name,
						DurationInSeconds = (int)(deathTimer.RespawnTimer != 0 ? deathTimer.RespawnTimer : deathTimer.Min_RespawnTimer),
						NegativeDurationToShow = (int)(deathTimer.Max_RespawnTimer),
						SpellNameIcon = "Disease Cloud",
						SpellType = SpellTypes.RespawnTimer,
						TargetName = "Death Timers",
						ExecutionTime = e.ExecutionTime,
						IsNPC = true
				};

					var existingDeathEntry = _baseTimerWindowViewModel.SpellList.FirstOrDefault(a => a.SpellName == add.Name && a.TargetName == "Death Timers");
					if (existingDeathEntry != null)
					{
						deathcounter = ++deathcounter > 999 ? 1 : deathcounter;
						add.Name += "_" + deathcounter;
					}

					_baseTimerWindowViewModel.TryAddCustom(add);
				}
			}
			catch(Exception ex)
			{
				_logging.Log(ex.Message, EventType.Error, _activePlayer?.Player?.Server);
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
				ExecutionTime = e.ExecutionTime,
				IsNPC = false
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
				_logParser.DTEvent -= LogParser_DTEvent;
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

		private void OnActivated(object sender, EventArgs e)
		{
			Topmost = !_baseTimerWindowViewModel._windowOptions.AlwaysOnTop;
			Topmost = _baseTimerWindowViewModel._windowOptions.AlwaysOnTop;

		}
		public override void SaveState()
		{
			_baseTimerWindowViewModel._windowOptions.AlwaysOnTop = this.Topmost;
			_baseTimerWindowViewModel._windowOptions.Closed = this.GetClosedState();

			base.SaveState();
			TimerWindowService.UpdateTimerWindow(_baseTimerWindowViewModel._windowOptions);
		}
	}
}