﻿using EQTool.EventArgModels;
using EQTool.Factories;
using EQTool.Models;
using EQTool.Services;
using EQToolShared.ExtendedClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace EQTool
{
	/// <summary>
	/// Interaction logic for CreateTimerWindow.xaml
	/// </summary>
	public partial class EditTimerWindow : Window
	{
		private EQToolSettings _settings;
		private TimerWindowFactory _timerWindowFactory;
		public event EventHandler<TimerWindowEditEventArgs> TimerWindowEdited;

		public EditTimerWindow(EQToolSettings settings, TimerWindowFactory timerWindowFactory, int? id = null)
		{
			_settings = settings;
			_timerWindowFactory = timerWindowFactory;

			TimerWindowOptions timer = null;

			if(id != null)
			{
				timer = TimerWindowService.LoadTimerWindows().FirstOrDefault(t => t.ID == (int)id);
				Title = "Edit Timer Window";
				DataContext = timer;
			}
			else
			{
				Title = "Create Timer Window";
				DataContext = new TimerWindowOptions();
			}

			InitializeComponent();

			if(timer != null)
			{
				WindowTitle.Text = timer.Title;
				ShowBestGuess.IsChecked = timer.BestGuessSpells;
				ShowModRods.IsChecked = timer.ShowModRodTimers;
				ShowDeathTouches.IsChecked = timer.ShowDeathTouches;
				ShowSpells.IsChecked = timer.ShowSpells;
				ShowDeaths.IsChecked = timer.ShowTimers;
				ShowRolls.IsChecked = timer.ShowRandomRolls;
				ShowYouOnly.IsChecked = timer.YouOnlySpells;
				ShowSimpleTimers.IsChecked = timer.ShowSimpleTimers;
				ShowNPCs.IsChecked = timer.ShowNPCs;
				ShowPCs.IsChecked = timer.ShowPCs;
			}
		}

		private void NewOverlay_Save(object sender, RoutedEventArgs e)
		{
			var temp = (sender as System.Windows.Controls.Button).DataContext as TimerWindowOptions;

			if(temp.ID != 0)
			{
				temp.Title = WindowTitle.Text;
				temp.BestGuessSpells = ShowBestGuess.IsChecked ?? false;
				temp.ShowModRodTimers = ShowModRods.IsChecked ?? false;
				temp.ShowDeathTouches = ShowDeathTouches.IsChecked ?? false;
				temp.ShowSpells = ShowSpells.IsChecked ?? false;
				temp.ShowTimers = ShowDeaths.IsChecked ?? false;
				temp.ShowRandomRolls = ShowRolls.IsChecked ?? false;
				temp.YouOnlySpells = ShowYouOnly.IsChecked ?? false;
				temp.ShowSimpleTimers = ShowSimpleTimers.IsChecked ?? false;
				temp.ShowNPCs = ShowNPCs.IsChecked ?? false;
				temp.ShowPCs = ShowPCs.IsChecked ?? false;

				UpdateTimerWindow(temp);
				return;
			}


			TimerWindowOptions windowOptions = new TimerWindowOptions
			{
				Title = WindowTitle.Text,
				BestGuessSpells = ShowBestGuess.IsChecked ?? false,
				ShowModRodTimers = ShowModRods.IsChecked ?? false,
				ShowDeathTouches = ShowDeathTouches.IsChecked ?? false,
				ShowSpells = ShowSpells.IsChecked ?? false,
				ShowTimers = ShowDeaths.IsChecked ?? false,
				ShowRandomRolls = ShowRolls.IsChecked ?? false,
				YouOnlySpells = ShowYouOnly.IsChecked ?? false,
				ShowSimpleTimers = ShowSimpleTimers.IsChecked ?? false,
				ShowNPCs = ShowNPCs.IsChecked ?? false,
				ShowPCs = ShowPCs.IsChecked ?? false,
				WindowRect = "0,0,250,400",
				Closed = false
			};

			if (TimerWindowService.AddNewTimerWindow(windowOptions))
			{
				if (_settings.TimerWindows == null)
				{
					_settings.TimerWindows = new ObservableCollectionRange<TimerWindowOptions>();
				}
				List<TimerWindowOptions> timerWindows = TimerWindowService.LoadTimerWindows();
				foreach (var window in timerWindows)
				{
					if (!_settings.TimerWindows.Any(co => co.ID == window.ID))
					{
						_settings.TimerWindows.Add(window);
					}
				}
				var newWindow = _timerWindowFactory.CreateTimerWindow(windowOptions);
				(App.Current as App).OpenSpawnableWindow<BaseTimerWindow>(newWindow);
			}

			//unset
			WindowTitle.Text = string.Empty;
			ShowBestGuess.IsChecked = false;
			ShowModRods.IsChecked = false;
			ShowDeathTouches.IsChecked = false;
			ShowSpells.IsChecked = false;
			ShowDeaths.IsChecked = false;
			ShowRolls.IsChecked = false;
			ShowYouOnly.IsChecked = false;
			ShowSimpleTimers.IsChecked = false;
			ShowNPCs.IsChecked = false;
			ShowPCs.IsChecked = false;

			//close popup
			this.Close();
		}

		private void UpdateTimerWindow(TimerWindowOptions timer)
		{
			var w = (Application.Current as App).GetSpawnableTimerWindowBase(timer);
			if(w != null)
			{
				timer.WindowRect = w.Top + "," + w.Left + "," + w.Width + "," + w.Height;
			}
			

			if (TimerWindowService.UpdateTimerWindow(timer))
			{
				this.TimerWindowEdited(this, new TimerWindowEditEventArgs { Success = true, UpdatedWindow = timer });
				Close();
				(Application.Current as App).UpdateSpawnableTimerWindowContext(timer);
			}
			this.TimerWindowEdited(this, new TimerWindowEditEventArgs { Success = false });
        }
    }
}
