using EQTool.Factories;
using EQTool.Models;
using EQTool.Services;
using EQTool.ViewModels;
using EQToolShared.Enums;
using EQToolShared.ExtendedClasses;
using EQToolShared.Map;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using ZealPipes.Services;
using static EQTool.Services.PipeParser;

namespace EQTool
{
	public partial class MappingWindow : BaseSaveStateWindow
	{
		private readonly LogParser logParser;
		private readonly PipeParser _pipeParser;
		private readonly MapViewModel mapViewModel;
		private readonly ActivePlayer _activePlayer;
		private readonly PlayerTrackerService playerTrackerService;
		private readonly IAppDispatcher appDispatcher;
		private readonly ISignalrPlayerHub signalrPlayerHub;
		private readonly System.Timers.Timer UITimer;
		private readonly EQToolSettings _settings;
		private QuarmDataService _quarmDataService;
		private TimerWindowFactory _timerWindowFactory;
		private ZealMessageService _zealMessageService;
		private LoggingService _logging;
		private DateTime LastYouActivity { get; set; }


		private DateTime _lastZealLocationUpdate = DateTime.Now;
		private float _lastZealLocationX = 0;
		private float _lastZealLocationY = 0;

		public MappingWindow(
			ISignalrPlayerHub signalrPlayerHub,
			MapViewModel mapViewModel,
			ActivePlayer activePlayer,
			LogParser logParser,
			PipeParser pipeParser,
			EQToolSettings settings,
			PlayerTrackerService playerTrackerService,
			EQToolSettingsLoad toolSettingsLoad,
			IAppDispatcher appDispatcher,
			LoggingService loggingService,
			QuarmDataService quarmDataService,
			TimerWindowFactory timerWindowFactory,
			ZealMessageService zealMessageService) : base(settings.MapWindowState, toolSettingsLoad, settings)
		{
			loggingService.Log(string.Empty, EventType.OpenMap, activePlayer?.Player?.Server);
			this._activePlayer = activePlayer;
			this.signalrPlayerHub = signalrPlayerHub;
			this.playerTrackerService = playerTrackerService;
			this.appDispatcher = appDispatcher;
			this.logParser = logParser;
			_pipeParser = pipeParser;
			_logging = loggingService;
			_quarmDataService = quarmDataService;
			_timerWindowFactory = timerWindowFactory;
			_settings = settings;
			_zealMessageService = zealMessageService;
			DataContext = this.mapViewModel = mapViewModel;
			InitializeComponent();
			base.Init();
			_ = mapViewModel.LoadDefaultMap(Map);
			Map.ZoneName = mapViewModel.ZoneName;
			Map.Height = Math.Abs(mapViewModel.AABB.MaxHeight);
			Map.Width = Math.Abs(mapViewModel.AABB.MaxWidth);
			this.logParser.PlayerLocationEvent += LogParser_PlayerLocationEvent;
			this.logParser.PlayerZonedEvent += LogParser_PlayerZonedEvent;
			this.logParser.EnteredWorldEvent += LogParser_EnteredWorldEvent;
			this.logParser.DeadEvent += LogParser_DeadEvent;
			this.logParser.StartTimerEvent += LogParser_StartTimerEvent;
			this.logParser.CancelTimerEvent += LogParser_CancelTimerEvent;
			KeyDown += PanAndZoomCanvas_KeyDown;
			Map.StartTimerEvent += Map_StartTimerEvent;
			Map.CancelTimerEvent += Map_CancelTimerEvent;
			ContextMenuOpening += Map_TimerMenu_OpenedEvent;
			ContextMenuClosing += Map_TimerMenu_ClosedEvent;
			Map.ContextMenuOpening += Map_TimerMenu_OpenedEvent;
			Map.ContextMenuClosing += Map_TimerMenu_ClosedEvent;
			Map.TimerMenu_ClosedEvent += Map_TimerMenu_ClosedEvent;
			Map.TimerMenu_OpenedEvent += Map_TimerMenu_OpenedEvent;
			_pipeParser.ZealLocationEvent += ZealMessageService_ZealLocationEvent;
			_pipeParser.ZealZoneChangeEvent += ZealMessageService_ZoneChangeEvent;
			this.signalrPlayerHub.PlayerLocationEvent += SignalrPlayerHub_PlayerLocationEvent;
			this.signalrPlayerHub.PlayerDisconnected += SignalrPlayerHub_PlayerDisconnected;
			UITimer = new System.Timers.Timer(1000);
			UITimer.Elapsed += UITimer_Elapsed;
			UITimer.Enabled = true;
			this.MouseEnter += ToggleMouseLocation_Event;
			this.MouseLeave += ToggleMouseLocation_Event;

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

		private void ZealMessageService_ZealLocationEvent(object sender, ZealMessageService.PlayerMessageReceivedEventArgs e)
		{
			if (_settings.UseZealForThis(e.ProcessId, _settings.ZealMap_AutoUpdate))
			{
				var loc = new Point3D(e.Message.Data.Position.X, e.Message.Data.Position.Y, e.Message.Data.Position.Z);
				appDispatcher.DispatchUI(() => mapViewModel.UpdateLocation(loc, e.Message.Data.heading));

				if (e.Message.Data != null && e.Message.Data.Position != null)
				{
					if (e.Message.Data.Position.X != _lastZealLocationX || e.Message.Data.Position.Y != _lastZealLocationY)
					{
						_lastZealLocationUpdate = DateTime.Now;
						_lastZealLocationX = e.Message.Data.Position.X;
						_lastZealLocationY = e.Message.Data.Position.Y;
						logParser.PingSignalRLocationEvent(loc);
					}
					else if (DateTime.Now.Subtract(_lastZealLocationUpdate).TotalMinutes >= 1.5)
					{
						var player = new SignalrPlayer
						{
							Zone = this._activePlayer.Player.Zone,
							GuildName = this._activePlayer.Player.GuildName,
							PlayerClass = this._activePlayer.Player.PlayerClass,
							Server = this._activePlayer.Player.Server.Value,
							MapLocationSharing = this._activePlayer.Player.MapLocationSharing,
							Name = this._activePlayer.Player.Name,
							TrackingDistance = this._activePlayer.Player.TrackingDistance
						};
						signalrPlayerHub.PushPlayerDisconnected(player);
					}
				}
			}
		}

		private void ToggleMouseLocation_Event(object sender, MouseEventArgs e)
		{
			mapViewModel.ToggleMouseLocation_Show(e.RoutedEvent.Equals(Mouse.MouseEnterEvent));
		}

		private void SignalrPlayerHub_PlayerDisconnected(object sender, SignalrPlayer e)
		{
			mapViewModel.PlayerDisconnected(e);
		}

		private void SignalrPlayerHub_PlayerLocationEvent(object sender, SignalrPlayer e)
		{
			mapViewModel.PlayerLocationEvent(e);
		}

		private void Map_PanAndZoomCanvas_MouseDownEvent(object sender, MouseButtonEventArgs e)
		{
			var mousePostion = e.GetPosition(Map);
			mapViewModel.PanAndZoomCanvas_MouseDown(mousePostion, e);
		}

		private void Map_TimerMenu_OpenedEvent(object sender, RoutedEventArgs e)
		{
			if (e.Source.GetType() == typeof(Button) && (e.Source as Button).Name == "TimerMenuBtn")
			{
				FrameworkElement fe = e.Source as FrameworkElement;
				fe.ContextMenu = _timerWindowFactory.CreateTimerMenu(_settings.TimerWindows);

				mapViewModel.TimerMenu_Opened();
			}
			else
			{
				//e.Handled = true;
			}
		}

		private void Map_TimerMenu_ClosedEvent(object sender, RoutedEventArgs e)
		{
			mapViewModel.TimerMenu_Closed();
		}

		private void LogParser_StartTimerEvent(object sender, LogParser.StartTimerEventArgs e)
		{
			var mw = mapViewModel.AddTimer(TimeSpan.FromSeconds(e.CustomTimer.DurationInSeconds), e.CustomTimer.Name, false);
			mapViewModel.MoveToPlayerLocation(mw);
		}

		private void LogParser_CancelTimerEvent(object sender, LogParser.CancelTimerEventArgs e)
		{
			mapViewModel.DeleteSelectedTimerByName(e.Name);
		}

		private void Map_StartTimerEvent(object sender, LogParser.StartTimerEventArgs e)
		{
			mapViewModel.TimerMenu_Closed();
			var mw = mapViewModel.AddTimer(TimeSpan.FromSeconds(e.CustomTimer.DurationInSeconds), e.CustomTimer.Name, true);
		}

		private void Map_CancelTimerEvent(object sender, EventArgs e)
		{
			mapViewModel.DeleteSelectedTimer();
		}

		private void Map_TimerMenu_Open(object sender, EventArgs e)
		{
			var cm = _timerWindowFactory.CreateTimerMenu(_settings.TimerWindows);
			if (cm == null)
			{
				return;
			}
			cm.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
			cm.PlacementTarget = sender as UIElement;
			cm.IsOpen = true;
		}

		private void LogParser_DeadEvent(object sender, LogParser.DeadEventArgs e)
		{
			try
			{
				string name = e.Name.CleanUpZealName(true);
				if (playerTrackerService.IsPlayer(name))
				{
					return;
				}

				if (_activePlayer.Player?.MapKillTimers == true)
				{
					var deathTimer = _quarmDataService.GetMonsterTimer(name);
					if (deathTimer != null)
					{
						var mw = mapViewModel.AddTimer(TimeSpan.FromSeconds(deathTimer.RespawnTimer), name, false);
						mapViewModel.MoveToPlayerLocation(mw);
					}
				}
			}
			catch (Exception ex)
			{
				_logging.Log(ex.Message, EventType.Error, _activePlayer?.Player?.Server);
			}
		}

		private void UITimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			appDispatcher.DispatchUI(() => mapViewModel.UpdateTimerWidgest());
		}

		private void PanAndZoomCanvas_KeyDown(object sender, KeyEventArgs e)
		{
			var scale = (int)MathHelper.ChangeRange(Math.Max(mapViewModel.AABB.MaxWidth, mapViewModel.AABB.MaxHeight), 500, 35000, 60, 300);
			switch (e.Key)
			{
				case Key.Left:
				case Key.A:
					mapViewModel.MoveMap(scale, 0);
					break;
				case Key.Right:
				case Key.D:
					mapViewModel.MoveMap(-scale, 0);
					break;
				case Key.Up:
				case Key.W:
					mapViewModel.MoveMap(0, scale);
					break;
				case Key.Down:
				case Key.S:
					mapViewModel.MoveMap(0, -scale);
					break;
				default:
					return;

			}
		}

		private void LogParser_EnteredWorldEvent(object sender, LogParser.EnteredWorldArgs e)
		{
			if (mapViewModel.LoadDefaultMap(Map))
			{
				Map.ZoneName = mapViewModel.ZoneName;
				Map.Height = Math.Abs(mapViewModel.AABB.MaxHeight);
				Map.Width = Math.Abs(mapViewModel.AABB.MaxWidth);
			}
			if (mapViewModel.ZoneName != null)
			{
				_quarmDataService.LoadMobDataForZone(mapViewModel.ZoneName);
			}
		}

		private void LogParser_PlayerZonedEvent(object sender, LogParser.PlayerZonedEventArgs e)
		{
			if (mapViewModel.LoadMap(e.ZoneInfo, Map))
			{
				Map.ZoneName = mapViewModel.ZoneName;
				Map.Height = Math.Abs(mapViewModel.AABB.MaxHeight);
				Map.Width = Math.Abs(mapViewModel.AABB.MaxWidth);
			}
		}

		private void ZealMessageService_ZoneChangeEvent(object sender, ZealLocationEventArgs e)
		{
			if (_settings.UseZealForThis(e.ProcessId, _settings.ZealZone_AutoUpdate))
			{
				var zoneInfo = _quarmDataService.GetZoneByID(e.ZoneId);
				PlayerZonedInfo zonedInfo = new PlayerZonedInfo()
				{
					ZoneName = zoneInfo.Code,
					IsInstance = e.ZoneId > 1000
				};


				appDispatcher.DispatchUI(() =>
				{
					if (mapViewModel.LoadMap(zonedInfo, Map))
					{
						Map.ZoneName = mapViewModel.ZoneName;
						Map.Height = Math.Abs(mapViewModel.AABB.MaxHeight);
						Map.Width = Math.Abs(mapViewModel.AABB.MaxWidth);
					};
				}
				);

			}
		}


		private void LogParser_PlayerLocationEvent(object sender, LogParser.PlayerLocationEventArgs e)
		{
			mapViewModel.UpdateLocation(e.Location);
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			UITimer?.Stop();
			UITimer?.Dispose();
			if (logParser != null)
			{
				logParser.PlayerLocationEvent -= LogParser_PlayerLocationEvent;
				logParser.PlayerZonedEvent -= LogParser_PlayerZonedEvent;
				logParser.EnteredWorldEvent -= LogParser_EnteredWorldEvent;
				logParser.DeadEvent -= LogParser_DeadEvent;
				logParser.StartTimerEvent -= LogParser_StartTimerEvent;
				logParser.CancelTimerEvent -= LogParser_CancelTimerEvent;
			}

			KeyDown -= PanAndZoomCanvas_KeyDown;
			if (Map != null)
			{
				Map.StartTimerEvent -= Map_StartTimerEvent;
				Map.CancelTimerEvent -= Map_CancelTimerEvent;
				Map.TimerMenu_ClosedEvent -= Map_TimerMenu_ClosedEvent;
				Map.TimerMenu_OpenedEvent -= Map_TimerMenu_OpenedEvent;
			}
			if (signalrPlayerHub != null)
			{
				signalrPlayerHub.PlayerLocationEvent -= SignalrPlayerHub_PlayerLocationEvent;
				signalrPlayerHub.PlayerDisconnected -= SignalrPlayerHub_PlayerDisconnected;
			}

			this.MouseEnter -= ToggleMouseLocation_Event;
			this.MouseLeave -= ToggleMouseLocation_Event;
			_zealMessageService.OnPlayerMessageReceived -= ZealMessageService_ZealLocationEvent;
			_pipeParser.ZealZoneChangeEvent -= ZealMessageService_ZoneChangeEvent;

			base.OnClosing(e);
		}

		private void SetMapCenter()
		{
			return;
			var loc = new Point(MapWrapper.ActualWidth / 2, MapWrapper.ActualHeight / 2);
			loc = this.MapWrapper.PointToScreen(loc);
			loc = this.Map.PointFromScreen(loc);
			this.mapViewModel.CenterRelativeToCanvas = loc;
		}


		private void PanAndZoomCanvas_MouseUp(object sender, MouseButtonEventArgs e)
		{
			this.mapViewModel.PanAndZoomCanvas_MouseUp(e.GetPosition(Map));
		}

		private void PanAndZoomCanvas_MouseMove(object sender, MouseEventArgs e)
		{
			this.SetMapCenter();
			this.mapViewModel.PanAndZoomCanvas_MouseMove(e.GetPosition(Map), e.LeftButton);
		}

		private void PanAndZoomCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			this.SetMapCenter();
			this.mapViewModel.PanAndZoomCanvas_MouseWheel(e.GetPosition(Map), e.Delta);
		}

		protected void toggleCenterOnyou(object sender, RoutedEventArgs e)
		{
			this.mapViewModel.ToggleCenter();
		}
	}
}
