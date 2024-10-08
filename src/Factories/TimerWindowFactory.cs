﻿using Autofac;
using Autofac.Core;
using EQTool.Models;
using EQTool.Services;
using EQTool.ViewModels;
using EQToolShared.ExtendedClasses;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EQTool.Factories
{
	public class TimerWindowFactory
	{
		private readonly EQToolSettings _settings;
		private readonly EQToolSettingsLoad _toolSettingsLoad;
		private readonly EQSpells _spells;
		private readonly IAppDispatcher _appDispatcher;
		private readonly ActivePlayer _activePlayer;
		private readonly ColorService _colorService;
		private readonly QuarmDataService _quarmService;

		public TimerWindowFactory(EQToolSettings settings, 
			EQToolSettingsLoad toolSettingsLoad, 
			EQSpells spells, 
			IAppDispatcher appDispatcher,
			ActivePlayer activePlayer, 
			ColorService colorService,
			QuarmDataService quarmDataService)
		{
			_settings = settings;
			_toolSettingsLoad = toolSettingsLoad;
			_spells = spells;
			_appDispatcher = appDispatcher;
			_activePlayer = activePlayer;
			_colorService = colorService;
			_quarmService = quarmDataService;
		}

		public BaseTimerWindow CreateTimerWindow(TimerWindowOptions timerWindow)
		{
			var newTimerWindowViewModel = new BaseTimerWindowViewModel(_activePlayer, _appDispatcher, _settings, _spells, _colorService, timerWindow, _quarmService);

			string[] rectParts = timerWindow.WindowRect?.Split(',');
			Rect rect = new Rect();
			if (rectParts != null)
			{
				var windowPoint = new Point(double.Parse(rectParts[1]), double.Parse(rectParts[0]));
				var windowSize = new Size(double.Parse(rectParts[2]), double.Parse(rectParts[3]));
				rect = new Rect(windowPoint, windowSize);
			}

			newTimerWindowViewModel.BestGuessSpells = timerWindow.BestGuessSpells;
			newTimerWindowViewModel.ShowModRodTimers = timerWindow.ShowModRodTimers;
			newTimerWindowViewModel.ShowSpells = timerWindow.ShowSpells;
			newTimerWindowViewModel.ShowTimers = timerWindow.ShowTimers;
			newTimerWindowViewModel.ShowRandomRolls = timerWindow.ShowRandomRolls;
			newTimerWindowViewModel.YouOnlySpells = timerWindow.YouOnlySpells;
			newTimerWindowViewModel.ID = timerWindow.ID;
			newTimerWindowViewModel.WindowState = new Models.WindowState()
			{
				State = (System.Windows.WindowState)timerWindow.State,
				Closed = timerWindow.Closed,
				AlwaysOnTop = timerWindow.AlwaysOnTop,
				Opacity = _settings.TimerWindowOpacity,
				WindowRect = rect,
			};

			var logParser = App.container.Resolve<LogParser>();
			var playerTrackerService = App.container.Resolve<PlayerTrackerService>();
			var quarmDataService = App.container.Resolve<QuarmDataService>();
			var loggingService = App.container.Resolve<LoggingService>();
			var pipeParser = App.container.Resolve<PipeParser>();

			var newTimerWindow = new BaseTimerWindow(playerTrackerService, _settings, newTimerWindowViewModel, logParser, pipeParser, _toolSettingsLoad, _activePlayer, quarmDataService, loggingService)
			{
				Title = timerWindow.Title,
				Topmost = timerWindow.AlwaysOnTop
			};

			newTimerWindow.DataContext = newTimerWindowViewModel;
			newTimerWindow.Top = rect.Top;
			newTimerWindow.Left = rect.Left;
			newTimerWindow.Width = rect.Width;
			newTimerWindow.Height = rect.Height;
			newTimerWindow.Topmost = timerWindow.AlwaysOnTop;
			return newTimerWindow;
		}
		public BaseTimerWindow CreateTimerWindow(int windowId)
		{
			var windowOptions = _settings.TimerWindows.FirstOrDefault(s => s.ID == windowId);
			return CreateTimerWindow(windowOptions);
		}

		public ContextMenu CreateTimerMenu(ObservableCollectionRange<TimerWindowOptions> timerWindows)
		{
			ContextMenu menu = new ContextMenu();
			foreach (var timerWindow in timerWindows)
			{
				MenuItem menuItem = new MenuItem();
				menuItem.Header = timerWindow.Title;
				menuItem.Tag = timerWindow.ID;
				menuItem.Click += (s, e) =>
				{
					(App.Current as App).OpenTimerWindow(menuItem, e);
				};
				menu.Items.Add(menuItem);
			}

			return menu;
		}
	}
}