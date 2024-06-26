﻿using EQTool.Factories;
using EQTool.Models;
using EQTool.Services;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace EQTool
{
    public partial class BaseSaveStateWindow : Window
    {
        private readonly DispatcherTimer timer = new DispatcherTimer
        {
            Interval = new TimeSpan(0, 0, 0, 0, 500),
            IsEnabled = false
        };

        private readonly EQToolSettingsLoad toolSettingsLoad;
        private readonly EQToolSettings settings;
        private readonly Models.WindowState windowState;
        private bool InitCalled = false;
        protected DateTime LastWindowInteraction = DateTime.UtcNow;
        public BaseSaveStateWindow(Models.WindowState windowState, EQToolSettingsLoad toolSettingsLoad, EQToolSettings settings)
        {
            this.windowState = windowState;
            this.toolSettingsLoad = toolSettingsLoad;
            this.settings = settings;
            windowState.Closed = false;
        }

        protected void Init()
        {
            if (InitCalled) return;
            InitCalled = true;
            AdjustWindow();
            timer.Tick += timer_Tick;
            SizeChanged += Window_SizeChanged;
            StateChanged += Window_StateChanged;
            LocationChanged += Window_LocationChanged;
            windowState.Closed = false;
            this.SaveState();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            timer.IsEnabled = false;
            SaveState();
        }

        private void DebounceSave()
        {
            timer.IsEnabled = true;
            timer.Stop();
            timer.Start();
        }

        public virtual void SaveState()
        {
            SaveWindowState(windowState);
            toolSettingsLoad.Save(settings);
        }

		public virtual bool GetClosedState()
		{
			return windowState.Closed;
		}

        public void CloseWindow()
        {
            windowState.Closed = true;
            SaveState();
            Close();
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            LastWindowInteraction = DateTime.UtcNow;
            DebounceSave();
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
			if(((Window)sender).Top <= SystemParameters.VirtualScreenTop)
			{
				((Window)sender).Top = SystemParameters.VirtualScreenTop + 1;
			}
			if(((Window)sender).Left < SystemParameters.VirtualScreenLeft)
			{
				((Window)sender).Left = SystemParameters.VirtualScreenLeft + 1;
			}
			if(((Window)sender).Top + ((Window)sender).Height > SystemParameters.VirtualScreenHeight)
			{
				((Window)sender).Top = SystemParameters.VirtualScreenHeight - ((Window)sender).Height - 1;
			}
			if(((Window)sender).Left + ((Window)sender).Width > SystemParameters.VirtualScreenLeft + SystemParameters.VirtualScreenWidth)
			{
				((Window)sender).Left = SystemParameters.VirtualScreenWidth + SystemParameters.VirtualScreenLeft - ((Window)sender).Width - 1;
			}

            LastWindowInteraction = DateTime.UtcNow;
            DebounceSave();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            LastWindowInteraction = DateTime.UtcNow;
            DebounceSave();
        }

        protected void DragWindow(object sender, MouseButtonEventArgs args)
        {
            LastWindowInteraction = DateTime.UtcNow;
            DragMove();
        }

        private void AdjustWindow()
        {
            this.Topmost = windowState.AlwaysOnTop;
            if (WindowBounds.isPointVisibleOnAScreen(windowState.WindowRect))
            {
                this.Left = windowState.WindowRect.Value.Left;
                this.Top = windowState.WindowRect.Value.Top;
                this.Height = windowState.WindowRect.Value.Height;
                this.Width = windowState.WindowRect.Value.Width;
                this.WindowState = windowState.State;
            }
        }

        private void SaveWindowState(EQTool.Models.WindowState windowState)
        {
            windowState.WindowRect = new Rect
            {
                X = this.Left,
                Y = this.Top,
                Height = this.Height,
                Width = this.Width
            };
            windowState.State = this.WindowState;
            windowState.AlwaysOnTop = this.Topmost;
			windowState.Closed = this.windowState.Closed;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (timer != null)
            {
                timer.Tick -= timer_Tick;
            }
            timer?.Stop();
            SizeChanged -= Window_SizeChanged;
            StateChanged -= Window_StateChanged;
            LocationChanged -= Window_LocationChanged;
            base.OnClosing(e);
        }
        protected void openmobinfo(object sender, RoutedEventArgs e)
        {
            (App.Current as App).OpenMobInfoWindow();
        }

        protected void opendps(object sender, RoutedEventArgs e)
        {
            (App.Current as App).OpenDPSWindow();
        }

        protected void opensettings(object sender, RoutedEventArgs e)
        {
            (App.Current as App).OpenSettingsWindow();
        }
        protected void openmap(object sender, RoutedEventArgs e)
        {
            (App.Current as App).OpenMapWindow();
        }

		protected void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.CloseWindow();
        }

        protected void MinimizeWindow(object sender, RoutedEventArgs e)
        {
            WindowState = System.Windows.WindowState.Minimized;
        }

        protected void MaximizeWindow(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == System.Windows.WindowState.Maximized ? System.Windows.WindowState.Normal : System.Windows.WindowState.Maximized;
        }
    }
}
