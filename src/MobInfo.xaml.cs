using EQTool.Factories;
using EQTool.Models;
using EQTool.Services;
using EQTool.ViewModels;
using EQToolShared.Enums;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
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
		private readonly IAppDispatcher _appDispatcher;
		private readonly LogParser _logParser;
		private ViewModels.MobInfoViewModel _mobInfoViewModel;
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
			this._appDispatcher = appDispatcher;
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
			_zealMessageService.OnCharacterUpdated += ZealMessageService_OnCharacterUpdated;

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
		}

		protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
		{
			e.Handled = true;
		}

		private void ZealMessageService_OnCharacterUpdated(object sender, ZealCharacter.ZealCharacterUpdatedEventArgs e)
		{
			if (_settings.ZealEnabled && _settings.ZealMobInfo_AutoUpdate && e.Character.Detail.LabelData != null && e.Character.Detail.LabelData.Count > 0)
			{
				if(_activePlayer.Player.LastTarget != e.Character.Detail.LabelData[(int)ZealPipes.Common.LabelType.TargetName - 1]?.Value)
				{
					var mobName = e.Character.Detail.LabelData[(int)ZealPipes.Common.LabelType.TargetName - 1]?.Value;
					_appDispatcher.DispatchUI(() =>
					{
						_mobInfoViewModel.NewResults = _quarmService.GetData(mobName);
						FactionHitsStack.Visibility = _mobInfoViewModel.HasFactionHits;
						QuestsStack.Visibility = _mobInfoViewModel.HasQuests;
						KnownLootStack.Visibility = _mobInfoViewModel.HasKnownLoot;
						MerchandiseStack.Visibility = _mobInfoViewModel.HasMerchandise;
						SpecialAbilitiesStack.Visibility = _mobInfoViewModel.HasSpecials;

						Sees_Invis_Stack.Visibility = _mobInfoViewModel.See_Invis == 1 ? Visibility.Visible : Visibility.Collapsed;
						Might_See_Invis_Stack.Visibility = _mobInfoViewModel.See_Invis > 1 ? Visibility.Visible : Visibility.Collapsed;
						Sees_IVU_Stack.Visibility = _mobInfoViewModel.See_Invis_Undead == 1 ? Visibility.Visible : Visibility.Collapsed;
						Might_See_IVU_Stack.Visibility = _mobInfoViewModel.See_Invis_Undead > 1 ? Visibility.Visible : Visibility.Collapsed;
						Sees_Sneak_Stack.Visibility = _mobInfoViewModel.See_Sneak == 1 ? Visibility.Visible : Visibility.Collapsed;
						Might_See_Sneak_Stack.Visibility = _mobInfoViewModel.See_Sneak > 1 ? Visibility.Visible : Visibility.Collapsed;
						Sees_ImpHide_Stack.Visibility = _mobInfoViewModel.See_Imp_Hide == 1 ? Visibility.Visible : Visibility.Collapsed;
						Might_See_ImpHide_Stack.Visibility = _mobInfoViewModel.See_Imp_Hide > 1 ? Visibility.Visible : Visibility.Collapsed;

						_activePlayer.Player.LastTarget = mobName;
					});
				}
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

					Sees_Invis_Stack.Visibility = _mobInfoViewModel.See_Invis == 1 ? Visibility.Visible : Visibility.Collapsed;
					Might_See_Invis_Stack.Visibility = _mobInfoViewModel.See_Invis > 1 ? Visibility.Visible : Visibility.Collapsed;
					Sees_IVU_Stack.Visibility = _mobInfoViewModel.See_Invis_Undead == 1 ? Visibility.Visible : Visibility.Collapsed;
					Might_See_IVU_Stack.Visibility = _mobInfoViewModel.See_Invis_Undead > 1 ? Visibility.Visible : Visibility.Collapsed;
					Sees_Sneak_Stack.Visibility = _mobInfoViewModel.See_Sneak == 1 ? Visibility.Visible : Visibility.Collapsed;
					Might_See_Sneak_Stack.Visibility = _mobInfoViewModel.See_Sneak > 1 ? Visibility.Visible : Visibility.Collapsed;
					Sees_ImpHide_Stack.Visibility = _mobInfoViewModel.See_Imp_Hide == 1 ? Visibility.Visible : Visibility.Collapsed;
					Might_See_ImpHide_Stack.Visibility = _mobInfoViewModel.See_Imp_Hide > 1 ? Visibility.Visible : Visibility.Collapsed;
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

			_zealMessageService.OnCharacterUpdated -= ZealMessageService_OnCharacterUpdated;
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
			if(e.Source.GetType() != typeof(Button) || (e.Source as Button).Name != "TimerMenuBtn")
			{
				e.Handled = true;
			}
			else
			{
				FrameworkElement fe = e.Source as FrameworkElement;
				fe.ContextMenu = _timerWindowFactory.CreateTimerMenu(_settings.TimerWindows);

				fe.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
				fe.ContextMenu.PlacementTarget = sender as UIElement;
				fe.ContextMenu.IsOpen = true;
			}
		}
	}
}
