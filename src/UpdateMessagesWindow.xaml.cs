﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;

namespace EQTool
{
    public class UpdateMessageData : INotifyPropertyChanged
    {
        public string Date { get { return DateTime.ToShortDateString(); } }
        public DateTime DateTime { get; set; }
        public string Message { get; set; }
        public string Image { get; set; }
        public Visibility ImageVisibility { get; set; } = Visibility.Collapsed;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }

    public partial class UpdateMessagesWindow : Window
    {
        public ObservableCollection<UpdateMessageData> UpdateMessages { get; set; } = new ObservableCollection<UpdateMessageData>();
        public UpdateMessagesWindow()
        {
            this.Topmost = true;
            this.DataContext = this;
            InitializeComponent();
            var view = (ListCollectionView)CollectionViewSource.GetDefaultView(Messages.ItemsSource);
            view.GroupDescriptions.Add(new PropertyGroupDescription(nameof(UpdateMessageData.Date)));
            view.LiveGroupingProperties.Add(nameof(UpdateMessageData.Message));
            view.IsLiveGrouping = true;
            view.SortDescriptions.Add(new SortDescription(nameof(UpdateMessageData.DateTime), ListSortDirection.Descending));
            view.IsLiveSorting = true;
            UpdateMessages.Add(new UpdateMessageData
            {
                DateTime = new DateTime(2024, 2, 26),
                Message =
                $"* Added Vulak AOE timers{Environment.NewLine}" +
                $"* Improved update performance."
            });
            UpdateMessages.Add(new UpdateMessageData
            {
                DateTime = new DateTime(2024, 2, 25),
                Message =
                    $"* Fixed slain targets not showing up in timers.{Environment.NewLine}" +
                    $"* Fixed Faction pull server notice.{Environment.NewLine}" +
                    $"* Added Random tracker which will show the TOP 5 rolls automatically. Check out the image below for an example.",
                Image = "pack://application:,,,/update1.png",
                ImageVisibility = Visibility.Visible
            });

            UpdateMessages.Add(new UpdateMessageData
            {
                DateTime = new DateTime(2024, 2, 24),
                Message =
                $"* Added support for 'King Tormax died.' to supported death triggers"
            });
        }
    }
}
