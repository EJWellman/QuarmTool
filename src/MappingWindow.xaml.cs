﻿using EQTool.Models;
using EQTool.Services;
using EQTool.Utilities;
using EQTool.ViewModels;
using EQToolShared.Enums;
using EQToolShared.Map;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace EQTool
{
    public partial class MappingWindow : BaseSaveStateWindow
    {
        private readonly LogParser logParser;
        private readonly MapViewModel mapViewModel;
        private readonly ActivePlayer activePlayer;
        private readonly PlayerTrackerService playerTrackerService;
        private readonly IAppDispatcher appDispatcher;
        private readonly ISignalrPlayerHub signalrPlayerHub;
        private readonly System.Timers.Timer UITimer;
		private QuarmDataService _quarmDataService;

		public MappingWindow(
            ISignalrPlayerHub signalrPlayerHub,
            MapViewModel mapViewModel,
            ActivePlayer activePlayer,
            LogParser logParser,
            EQToolSettings settings,
            PlayerTrackerService playerTrackerService,
            EQToolSettingsLoad toolSettingsLoad,
            IAppDispatcher appDispatcher,
            LoggingService loggingService,
			QuarmDataService quarmDataService) : base(settings.MapWindowState, toolSettingsLoad, settings)
        {
            loggingService.Log(string.Empty, EventType.OpenMap, activePlayer?.Player?.Server);
            this.activePlayer = activePlayer;
            this.signalrPlayerHub = signalrPlayerHub;
            this.playerTrackerService = playerTrackerService;
            this.appDispatcher = appDispatcher;
            this.logParser = logParser;
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
            Map.TimerMenu_ClosedEvent += Map_TimerMenu_ClosedEvent;
            Map.TimerMenu_OpenedEvent += Map_TimerMenu_OpenedEvent;
            this.signalrPlayerHub.PlayerLocationEvent += SignalrPlayerHub_PlayerLocationEvent;
            this.signalrPlayerHub.PlayerDisconnected += SignalrPlayerHub_PlayerDisconnected;
            UITimer = new System.Timers.Timer(1000);
            UITimer.Elapsed += UITimer_Elapsed;
            UITimer.Enabled = true;
            //   this.SetCenerMap();

			_quarmDataService = quarmDataService;
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
            mapViewModel.TimerMenu_Opened();
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

        private void LogParser_DeadEvent(object sender, LogParser.DeadEventArgs e)
        {
            if (playerTrackerService.IsPlayer(e.Name))
            {
                return;
            }

            if (activePlayer.Player?.MapKillTimers == true)
            {
                var zonetimer = ZoneSpawnTimes.GetSpawnTime(e.Name, mapViewModel.ZoneName);
                var mw = mapViewModel.AddTimer(zonetimer, e.Name, true);
                mapViewModel.MoveToPlayerLocation(mw);
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
			if(mapViewModel.ZoneName != null)
			{
				_quarmDataService.LoadMobDataForZone(mapViewModel.ZoneName);
			}
		}

        private void LogParser_PlayerZonedEvent(object sender, LogParser.PlayerZonedEventArgs e)
        {
            if (mapViewModel.LoadMap(e.Zone, Map))
            {
                Map.ZoneName = mapViewModel.ZoneName;
                Map.Height = Math.Abs(mapViewModel.AABB.MaxHeight);
                Map.Width = Math.Abs(mapViewModel.AABB.MaxWidth);
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
            base.OnClosing(e);
        }

        private void SetCenerMap()
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
            this.SetCenerMap();
            this.mapViewModel.PanAndZoomCanvas_MouseMove(e.GetPosition(Map), e.LeftButton);
        }

        private void PanAndZoomCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            this.SetCenerMap();
            this.mapViewModel.PanAndZoomCanvas_MouseWheel(e.GetPosition(Map), e.Delta);
        }

        protected void toggleCenterOnyou(object sender, RoutedEventArgs e)
        {
            this.mapViewModel.ToggleCenter();
        }
    }
}
