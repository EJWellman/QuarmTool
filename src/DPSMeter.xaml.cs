﻿using EQTool.Models;
using EQTool.Services;
using EQTool.ViewModels;
using EQToolShared.Enums;
using EQToolShared.Map;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace EQTool
{
    public partial class DPSMeter : BaseSaveStateWindow
    {
        private readonly System.Timers.Timer UITimer;
        private readonly LogParser logParser;
        private readonly DPSWindowViewModel dPSWindowViewModel;
        private readonly ActivePlayer activePlayer;

        public DPSMeter(LogParser logParser, 
			DPSWindowViewModel dPSWindowViewModel, 
			EQToolSettings settings, 
			EQToolSettingsLoad toolSettingsLoad, 
			LoggingService loggingService, 
			ActivePlayer activePlayer)
             : base(settings.DpsWindowState, toolSettingsLoad, settings)
        {
            this.activePlayer = activePlayer;
            loggingService.Log(string.Empty, EventType.OpenDPS, activePlayer?.Player?.Server);
            this.logParser = logParser;
            this.dPSWindowViewModel = dPSWindowViewModel;
            this.dPSWindowViewModel.EntityList = new System.Collections.ObjectModel.ObservableCollection<EntityDPS>();
            DataContext = dPSWindowViewModel;
            InitializeComponent();
            base.Init();
            this.logParser.FightHitEvent += LogParser_FightHitEvent;
            this.logParser.DeadEvent += LogParser_DeadEvent;
            UITimer = new System.Timers.Timer(1000);
            UITimer.Elapsed += PollUI;
            UITimer.Enabled = true;
            DpsList.ItemsSource = dPSWindowViewModel.EntityList;
            var view = (ListCollectionView)CollectionViewSource.GetDefaultView(dPSWindowViewModel.EntityList);
            view.GroupDescriptions.Add(new PropertyGroupDescription(nameof(EntityDPS.TargetName)));
            view.LiveGroupingProperties.Add(nameof(EntityDPS.TargetName));
            view.IsLiveGrouping = true;
            view.SortDescriptions.Add(new SortDescription(nameof(EntityDPS.TargetName), ListSortDirection.Ascending));
            view.SortDescriptions.Add(new SortDescription(nameof(EntityDPS.TotalDamage), ListSortDirection.Descending));
            view.IsLiveSorting = true;
            view.LiveSortingProperties.Add(nameof(EntityDPS.TotalDamage));
        }

        private void LogParser_FightHitEvent(object sender, LogParser.FightHitEventArgs e)
        {
            dPSWindowViewModel.TryAdd(e.HitInformation);
        }

        private void LogParser_DeadEvent(object sender, LogParser.DeadEventArgs e)
        {
            var zone = this.activePlayer?.Player?.Zone;
			//TODO: Revisit this later
            //if (!string.IsNullOrWhiteSpace(zone) && ZoneParser.ZoneInfoMap.TryGetValue(zone, out var fzone))
            //{
            //    if (fzone.NotableNPCs.Any(a => a == e.Name))
            //    {
            //        this.copytoclipboard(e.Name);
            //    }
            //}

            dPSWindowViewModel.TargetDied(e.Name);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            UITimer?.Stop();
            UITimer?.Dispose();
            if (this.logParser != null)
            {
                logParser.DeadEvent -= LogParser_DeadEvent;
                logParser.FightHitEvent -= LogParser_FightHitEvent;
            }
            base.OnClosing(e);
        }

        private void PollUI(object sender, EventArgs e)
        {
            dPSWindowViewModel.UpdateDPS();
        }

        private void copytoclipboard(object sender, RoutedEventArgs e)
        {
            var name = ((sender as Button).DataContext as dynamic)?.Name as string;
            copytoclipboard(name);
        }

        private void copytoclipboard(string name)
        {
            var items = dPSWindowViewModel.EntityList.Where(a => a.TargetName == name);
            var fights = new List<string>();
            foreach (var item in items.OrderByDescending(a => a.TotalDamage))
            {
                fights.Add($"{item.SourceName} {item.PercentOfTotalDamage}% DPS:{item.TotalDPS} DMG:{item.TotalDamage}");
            }
            var fightdetails = "Fight Details: " + name + " Dmg: " + (items.FirstOrDefault()?.TargetTotalDamage ?? 0) + "    " + string.Join(" / ", fights);
            for (var i = 0; i < 2; i++)
            {
                try
                {
                    System.Windows.Forms.Clipboard.SetText(fightdetails, System.Windows.Forms.TextDataFormat.Text);
                    (App.Current as App).ShowBalloonTip(4000, "DPS Copied", $"DPS for the fight with {name} has been copied to clipboard", System.Windows.Forms.ToolTipIcon.Info);
                    Thread.Sleep(10);
                }
                catch (Exception e)
                {
                    if (i == 1)
                    {
                        throw;
                    }
                }
            }
        }

        private void MoveCurrentToLastSession(object sender, RoutedEventArgs e)
        {
            dPSWindowViewModel.SessionPlayerDamage.LastSessionPlayerDamage = dPSWindowViewModel.SessionPlayerDamage.CurrentSessionPlayerDamage;
            dPSWindowViewModel.SessionPlayerDamage.CurrentSessionPlayerDamage = new PlayerDamage();
        }

        private void RemoveLastSession(object sender, RoutedEventArgs e)
        {
            dPSWindowViewModel.SessionPlayerDamage.LastSessionPlayerDamage = null;
        }
    }
}
