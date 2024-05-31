using EQTool.Factories;
using EQTool.Models;
using EQTool.Services;
using EQTool.ViewModels;
using EQToolShared.Enums;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using ZealPipes.Common.Models;
using ZealPipes.Services;
using static System.Net.Mime.MediaTypeNames;

namespace EQTool
{
    /// <summary>
    /// Interaction logic for MobInfo.xaml
    /// </summary>
    public partial class MobInfo : BaseSaveStateWindow
	{
		private readonly IAppDispatcher appDispatcher;
		private readonly LogParser logParser;
        private ViewModels.MobInfoViewModel mobInfoViewModel;
        private readonly QuarmDataService _quarmService;
		private readonly PlayerTrackerService _playerTrackerService;
        private readonly ActivePlayer _activePlayer;
		private readonly TimerWindowFactory _timerWindowFactory;
        private ZealMessageService _zealMessageService;
        private readonly EQToolSettings _settings;
		public MobInfo(ActivePlayer activePlayer, 
			QuarmDataService quarmService, 
			PlayerTrackerService playerTrackerService,
			LogParser logParser, 
			EQToolSettings settings, 
			EQToolSettingsLoad toolSettingsLoad,
			IAppDispatcher appDispatcher,
			ZealMessageService zealMessageService,
            TimerWindowFactory timerWindowFactory,
            LoggingService loggingService) : base(settings.MobWindowState, toolSettingsLoad, settings)
        {
			this.appDispatcher = appDispatcher;
            loggingService.Log(string.Empty, EventType.OpenMobInfo, activePlayer?.Player?.Server);
            _activePlayer = activePlayer;
			_playerTrackerService = playerTrackerService;
            _quarmService = quarmService;
            _zealMessageService = zealMessageService;
            _logParser = logParser;
			_timerWindowFactory = timerWindowFactory;
			_settings = settings;
            DataContext = _mobInfoViewModel = new ViewModels.MobInfoViewModel();
            InitializeComponent();
            base.Init();
            this._logParser.ConEvent += LogParser_ConEvent;
			ContextMenuOpening += MobInfo_TimerMenu_OpenedEvent;

			foreach (var timer in settings.TimerWindows)
			{
				var item = new System.Windows.Controls.MenuItem()
				{
					Header = timer.Title,
					DataContext = timer.ID,
				};
				item.Click += (App.Current as App).OpenTimerWindow;

				TimerWindowsMenu.Items.Add(item);
			}
			_zealMessageService.OnCharacterUpdated += ZealMessageService_OnCharacterUpdated;
            this.logParser.ConEvent += LogParser_ConEvent;
        }

		private void ZealMessageService_OnCharacterUpdated(object sender, ZealCharacter.ZealCharacterUpdatedEventArgs e)
		{
			string mobName = e.Character.Detail.LabelData[(int)ZealPipes.Common.LabelType.TargetName - 1].Value;
			if (!string.IsNullOrWhiteSpace(mobName) && mobName != mobInfoViewModel.Name)
			{

				appDispatcher.DispatchUI(() =>
				{
					mobInfoViewModel.NewResults = _quarmService.GetData(mobName);
					FactionHitsStack.Visibility = mobInfoViewModel.HasFactionHits;
					QuestsStack.Visibility = mobInfoViewModel.HasQuests;
					KnownLootStack.Visibility = mobInfoViewModel.HasKnownLoot;
					MerchandiseStack.Visibility = mobInfoViewModel.HasMerchandise;
					SpecialAbilitiesStack.Visibility = mobInfoViewModel.HasSpecials;

					invis_rad.IsChecked = mobInfoViewModel.See_Invis;
					ivu_rad.IsChecked = mobInfoViewModel.See_Invis_Undead;
					sneak_rad.IsChecked = mobInfoViewModel.See_Sneak;
					ihide_rad.IsChecked = mobInfoViewModel.See_Imp_Hide;
				});

			}

			string blah = "";
		}

		private void LogParser_ConEvent(object sender, LogParser.ConEventArgs e)
        {
            try
            {
                if (e.Name != _mobInfoViewModel.Name)
                {
					_mobInfoViewModel.NewResults = _quarmService.GetData(e.Name);
					FactionHitsStack.Visibility = _mobInfoViewModel.HasFactionHits;
					QuestsStack.Visibility = _mobInfoViewModel.HasQuests;
					KnownLootStack.Visibility = _mobInfoViewModel.HasKnownLoot;
					MerchandiseStack.Visibility = _mobInfoViewModel.HasMerchandise;
					SpecialAbilitiesStack.Visibility = _mobInfoViewModel.HasSpecials;

					invis_rad.IsChecked = _mobInfoViewModel.See_Invis;
					ivu_rad.IsChecked = _mobInfoViewModel.See_Invis_Undead;
					sneak_rad.IsChecked = _mobInfoViewModel.See_Sneak;
					ihide_rad.IsChecked = _mobInfoViewModel.See_Imp_Hide;

				}
            }
            catch (Exception ex)
            {
                _mobInfoViewModel.ErrorResults = ex.Message;
                if (!_mobInfoViewModel.ErrorResults.Contains("The underlying connection was closed:"))
                {
                    _mobInfoViewModel.ErrorResults = "The server is down. Try again";
                    App.LogUnhandledException(ex, $"LogParser_ConEvent {e.Name}", _activePlayer?.Player?.Server);
                }
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (_logParser != null)
            {
                _logParser.ConEvent -= LogParser_ConEvent;
            }
            base.OnClosing(e);
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            _ = Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void Hyperlink_RequestNavigatebutton(object sender, RoutedEventArgs args)
        {
            _ = Process.Start(new ProcessStartInfo(_mobInfoViewModel.Url));
		}

		private void MobInfo_TimerMenu_OpenedEvent(object sender, RoutedEventArgs e)
		{
			FrameworkElement fe = e.Source as FrameworkElement;
			fe.ContextMenu = _timerWindowFactory.CreateTimerMenu(_settings.TimerWindows);

			fe.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
			fe.ContextMenu.PlacementTarget = sender as UIElement;
			fe.ContextMenu.IsOpen = true;
		}
	}
}
