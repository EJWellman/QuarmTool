using Autofac;
using EQTool.Factories;
using EQTool.Models;
using EQTool.Services;
using EQTool.ViewModels;
using EQToolShared.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using ZealPipes.Services;

namespace EQTool
{
    public partial class App : System.Windows.Application
    {
        public static HttpClient httpclient = new HttpClient();

        public static IContainer container;
        private System.Windows.Forms.NotifyIcon SystemTrayIcon;

        private System.Windows.Forms.MenuItem MapMenuItem;
		private System.Windows.Forms.MenuItem DpsMeterMenuItem;
        private System.Windows.Forms.MenuItem OverlayMenuItem;
		private System.Windows.Forms.MenuItem ImageOverlayMenuItem;
		private System.Windows.Forms.MenuItem SettingsMenuItem;
        private System.Windows.Forms.MenuItem GroupSuggestionsMenuItem;
        private System.Windows.Forms.MenuItem MobInfoMenuItem;

        private LogParser logParser => container.Resolve<LogParser>();
		private PipeParser _pipeParser => container.Resolve<PipeParser>();
		private ZealMessageService _zealMessageService => container.Resolve<ZealMessageService>();
        private System.Timers.Timer UITimer;
        private PlayerTrackerService PlayerTrackerService;
        private ZoneActivityTrackingService ZoneActivityTrackingService;
        private ISignalrPlayerHub signalrPlayerHub;
        private AudioService audioService;

		private TimerWindowFactory _timerWindowFactory;

        private EQToolSettings _EQToolSettings;

        private EQToolSettings _settings
        {
            get
            {
                if (_EQToolSettings == null)
                {
                    _EQToolSettings = container.Resolve<EQToolSettings>();
                }
                return _EQToolSettings;
            }
        }

        public static List<BaseSaveStateWindow> WindowList = new List<BaseSaveStateWindow>();
#if QUARM
        private const string programName = "quarmtool";
#else
        private const string programName = "eqtool";
#endif
        private bool WaitForEQToolToStop()
        {
#if DEBUG
            return true;
#endif
            var counter = 0;
            int count;
            do
            {
                count = Process.GetProcessesByName(programName).Count();
                if (counter++ > 6)
                {
                    return false;
                }
                Debug.WriteLine($"Waiting for {programName} {count} on counter {counter}");
                Thread.Sleep(3000);
            }
            while (count != 1);
            return true;
        }

        //public void CheckForUpdates(object sender, EventArgs e)
        //{
        //    new UpdateService().CheckForUpdates(Version);
        //}

        public class ExceptionRequest
        {
            public string Version { get; set; }
            public string Message { get; set; }
            public EventType EventType { get; set; }
            public BuildType BuildType { get; set; }
            public Servers? Server { get; set; }
        }

        public static void LogUnhandledException(Exception exception, string source, Servers? server)
        {
            var build = BuildType.Release;
#if TEST
            build =  BuildType.Test;
#elif DEBUG
            build = BuildType.Debug;
#elif BETA
            build = BuildType.Beta;
#elif QUARM
            build = BuildType.Quarm;
#endif
            try
            {
                var msg = new ExceptionRequest
                {
                    Version = Version,
                    Message = $"Unhandled exception ({source}) {exception}",
                    EventType = EventType.Error,
                    BuildType = build,
                    Server = server
                };
                if (msg.Message.Contains("Server timeout (30000.00ms) elapsed without receiving a message from the server.") ||
						msg.Message.Contains("The 'InvokeCoreAsync' method cannot be called") ||
						msg.Message.Contains("The remote party closed the WebSocket connection") ||
						msg.Message.Contains("An internal WebSocket error occurred.")
                    )
                {
                    return;
                }
                //var msagasjson = Newtonsoft.Json.JsonConvert.SerializeObject(msg);
                //var content = new StringContent(msagasjson, Encoding.UTF8, "application/json");
                //var result = httpclient.PostAsync("https://pigparse.azurewebsites.net/api/eqtool/exception", content).Result;
            }
            catch { }
        }

        private void SetupExceptionHandling()
        {

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
				var server = container?.Resolve<ActivePlayer>()?.Player?.Server;
                LogUnhandledException((Exception)e.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException", server);
            };

            DispatcherUnhandledException += (s, e) =>
            {
                var server = container?.Resolve<ActivePlayer>()?.Player?.Server;
                LogUnhandledException(e.Exception, "Application.Current.DispatcherUnhandledException", server);
            };

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                var server = container?.Resolve<ActivePlayer>()?.Player?.Server;
                LogUnhandledException(e.Exception, "TaskScheduler.UnobservedTaskException", server);
            };
        }

        private bool ShouldShutDownDueToNoWriteAccess()
        {
            try
            {
                File.Delete("test.json");
            }
            catch { }
            try
            {
                File.WriteAllText("test.json", "test");
            }
            catch (UnauthorizedAccessException)
            {
                _ = System.Windows.MessageBox.Show("EQTool is running from a directory where it does not have permission to save settings. Please, move it to a folder where it can write!", "EQTool Permissions!", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return true;
            }
            try
            {
                File.Delete("test.json");
            }
            catch { }
            return false;
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender1, cert, chain, sslPolicyErrors) => true;
            if (ShouldShutDownDueToNoWriteAccess())
            {
                App.Current.Shutdown();
                return;
            }
            ShutdownMode = ShutdownMode.OnExplicitShutdown;
            SetupExceptionHandling();
            if (!WaitForEQToolToStop())
            {
				System.Windows.MessageBox.Show("Another QuarmTool is currently running. You must shut that one down first!", "Multiple QuarmTools running!", MessageBoxButton.OK, MessageBoxImage.Error);
                App.Current.Shutdown();
                return;
            }
            try
            {
                var curr = Directory.GetCurrentDirectory();
                var path = Path.Combine(curr, "eqgame.exe");
                if (File.Exists(path))
                {
					System.Windows.MessageBox.Show("QuarmTool does not support running from in the EQ directory. Please move QuarmTool and try again", "QuarmTool Invalid Folder!", MessageBoxButton.OK, MessageBoxImage.Error);
                    App.Current.Shutdown();
                    return;
                }
            }
            catch { }
            httpclient.DefaultRequestHeaders.Add("User-Agent", "request");
//            var updateservice = new UpdateService();
//            var did_update = updateservice.ApplyUpdate(e.Args.FirstOrDefault());
//            if (did_update == UpdateService.UpdateStatus.UpdatesApplied)
//            {
//                return;
//            }
//            else if (did_update == UpdateService.UpdateStatus.NoUpdateApplied)
//            {
//#if !DEBUG
//                updateservice.CheckForUpdates(Version);
//#endif
//            }

            try
            {
                InitStuff();
            }
            catch (Exception ex)
            {
                LogUnhandledException(ex, "InitStuff", null);
                Thread.Sleep(1000 * 20);/// Sleep for 20 seconds here this will hopfully allow the update to occur and fix any problems
            }
        }

        private void InitStuff()
        {
            container = DI.Init();

            UITimer = new System.Timers.Timer(1000 * 60);
#if !DEBUG
            UITimer.Elapsed += UITimer_Elapsed;
            UITimer.Enabled = true;
#endif
			container.Resolve<LoggingService>().Log(string.Empty, EventType.StartUp, null);
			SettingsMenuItem = new System.Windows.Forms.MenuItem("Settings", ToggleSettingsWindow);
			var standardgroup = new System.Windows.Forms.MenuItem("Standard Groups", CreateStandardGroup);
			var hotclericsamegroup = new System.Windows.Forms.MenuItem("HOT Clerics Same Group", CreateHOTClericsSameGroup);
			var hotclericsparsegroup = new System.Windows.Forms.MenuItem("HOT Clerics Sparse Group", CreateHOTClericsSparseGroup);
			GroupSuggestionsMenuItem = new System.Windows.Forms.MenuItem("Group Suggestions", new System.Windows.Forms.MenuItem[] { standardgroup, hotclericsamegroup, hotclericsparsegroup });
			MapMenuItem = new System.Windows.Forms.MenuItem("Map", ToggleMapWindow);
			DpsMeterMenuItem = new System.Windows.Forms.MenuItem("Dps", ToggleDPSWindow);
			OverlayMenuItem = new System.Windows.Forms.MenuItem("Overlay", ToggleOverlayWindow);
			ImageOverlayMenuItem = new System.Windows.Forms.MenuItem("Static Overlay", ToggleImageOverlayWindow);
			MobInfoMenuItem = new System.Windows.Forms.MenuItem("Mob Info", ToggleMobInfoWindow);
			var gitHubMenuItem = new System.Windows.Forms.MenuItem("Suggestions", Suggestions);
			//var whythepig = new System.Windows.Forms.MenuItem("Pigparse Discord", Discord);
			//var updates = new System.Windows.Forms.MenuItem("Check for Update", CheckForUpdates);
			var versionstring = Assembly.GetExecutingAssembly().GetName().Version.ToString();
			var beta = false;

#if BETA || DEBUG
			beta = true;
#endif

			var logo = EQTool.Properties.Resources.pig;
#if QUARM
			logo = EQTool.Properties.Resources.Quarm;
#endif
			if (beta)
			{
				versionstring = "Beta-" + versionstring;
				logo = EQTool.Properties.Resources.sickpic;
			}

			var version = new System.Windows.Forms.MenuItem(versionstring)
			{
				Enabled = false
			};
			ToggleMenuButtons(false);

			MenuItem timersMenu = new MenuItem("Timers");
			GenerateTimerMenu(timersMenu);
			timersMenu.Popup += (s, e) =>
			{
				timersMenu.MenuItems.Clear();
				GenerateTimerMenu(timersMenu);
			};
			SystemTrayIcon = new System.Windows.Forms.NotifyIcon
			{
				Icon = logo,
				Visible = true,
				ContextMenu = new System.Windows.Forms.ContextMenu(new System.Windows.Forms.MenuItem[]
				{
                    OverlayMenuItem,
					ImageOverlayMenuItem,
					DpsMeterMenuItem,
					MapMenuItem,
					timersMenu,
                    MobInfoMenuItem,
					SettingsMenuItem,
					gitHubMenuItem,
                    //updates,
                    version,
					new System.Windows.Forms.MenuItem("Exit", OnExit)
				}),
			};
			var hasvalideqdir = FindEq.IsValidEqFolder(_settings.DefaultEqDirectory);
			if (!hasvalideqdir || FindEq.TryCheckLoggingEnabled(_settings.DefaultEqDirectory) == false)
			{
				if (!hasvalideqdir)
				{
					_settings.DefaultEqDirectory = string.Empty;
				}
				OpenSettingsWindow();
			}
			else
			{
				ToggleMenuButtons(true);
				if (!_settings.DpsWindowState.Closed)
				{
					OpenDPSWindow();
				}
				if (!_settings.MapWindowState.Closed)
				{
					OpenMapWindow();
				}
				if (!_settings.MobWindowState.Closed)
				{
					OpenMobInfoWindow();
				}
				if (!_settings.OverlayWindowState.Closed)
				{
					OpenOverLayWindow();
				}
				if(!_settings.ImageOverlayWindowState.Closed)
				{
					OpenImageOverLayWindow();
				}
				if (_settings.TimerWindows.Any(tw => !tw.Closed))
				{
					_timerWindowFactory = container.Resolve<TimerWindowFactory>();

					var windows = _settings.TimerWindows.Where(t => !t.Closed);
					foreach (var timer in windows)
					{
						var timerWindow = _timerWindowFactory.CreateTimerWindow(timer);
						WindowList.Add(timerWindow);
						timerWindow.Closed += (se, ee) =>
						{
							_ = WindowList.Remove(timerWindow);
						};
						timerWindow.Show();
					}
				}
			}
			signalrPlayerHub = container.Resolve<ISignalrPlayerHub>();

            PlayerTrackerService = container.Resolve<PlayerTrackerService>();
            ZoneActivityTrackingService = container.Resolve<ZoneActivityTrackingService>();
            audioService = container.Resolve<AudioService>();
            logParser.QuakeEvent += LogParser_QuakeEvent;
            App.Current.Resources["GlobalFontSize"] = (double)(_settings?.FontSize ?? 12);
            ((App)System.Windows.Application.Current).UpdateBackgroundOpacity("MyWindowStyleDPS", _settings.DpsWindowState.Opacity.Value);
            ((App)System.Windows.Application.Current).UpdateBackgroundOpacity("MyWindowStyleMap", _settings.MapWindowState.Opacity.Value);
			((App)System.Windows.Application.Current).UpdateBackgroundOpacity("MyWindowStyleTrigger", _settings.TimerWindowOpacity);
			((App)System.Windows.Application.Current).UpdateBackgroundOpacity("MyMobWindowSyle", _settings.MobWindowState.Opacity.Value);

			_zealMessageService.StartProcessing();
			_pipeParser.Start();
        }
        private void GenerateTimerMenu(MenuItem timersMenu)
        {
            foreach (var timer in _settings.TimerWindows)
            {
                var item = new MenuItem(timer.Title, OpenTimerWindow);
                item.Tag = timer.ID;

                timersMenu.MenuItems.Add(item);
            }
            if (timersMenu.MenuItems.Count == 0)
            {
                timersMenu.MenuItems.Add(new MenuItem("No Timers Available"));
            }
        }
        public void UpdateBackgroundOpacity(string name, double opacity)
        {
            var newcolor = (SolidColorBrush)new BrushConverter().ConvertFrom("#1a1919");
            newcolor.Opacity = opacity;
            var style = new System.Windows.Style { TargetType = typeof(Window) };
            style.Setters.Add(new Setter(Window.BackgroundProperty, newcolor));
            style.Setters.Add(new Setter(Window.FontSizeProperty, (double)this._settings.FontSize.Value));
            App.Current.Resources[name] = style;
        }

        private void LogParser_QuakeEvent(object sender, LogParser.QuakeArgs e)
        {
            container.Resolve<PigParseApi>().SendQuake();
        }

        [DllImport("user32.dll")]
        static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        [StructLayout(LayoutKind.Sequential)]
        struct LASTINPUTINFO
        {
            public uint cbSize;
            public int dwTime;
        }
        public static TimeSpan GetIdleTime()
        {
            LASTINPUTINFO lastInPut = new LASTINPUTINFO();
            lastInPut.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(lastInPut);
            if (GetLastInputInfo(ref lastInPut))
            {
                return TimeSpan.FromMilliseconds(Environment.TickCount - lastInPut.dwTime);
            }
            return TimeSpan.FromMinutes(20);
        }

        private bool updatecalled = false;
        private void UITimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var dispatcher = container.Resolve<IAppDispatcher>();
            dispatcher.DispatchUI(() =>
            {
                if (updatecalled)
                {
                    return;
                }
                updatecalled = true;
                try
                {
                    var idletime = GetIdleTime();
                    var spellstuff = container.Resolve<BaseTimerWindowViewModel>();
                    var logParser = container.Resolve<LogParser>();
                    //if (spellstuff != null)
                    //{
                    //    if (spellstuff.SpellList.Count() < 2 && (DateTime.UtcNow - logParser.LastYouActivity).TotalMinutes > 10 && idletime.TotalMinutes > 10)
                    //    {
                    //        new UpdateService().CheckForUpdates(Version);
                    //    }
                    //}
                    //else if ((DateTime.UtcNow - logParser.LastYouActivity).TotalMinutes > 10 && idletime.TotalMinutes > 10)
                    //{
                    //    new UpdateService().CheckForUpdates(Version);
                    //}
                }
                finally
                {
                    updatecalled = false;
                }
            });
        }

        public class GithubAsset
        {
            public string browser_download_url { get; set; }
        }

        public class GithubVersionInfo
        {
            public List<GithubAsset> assets { get; set; }
            public string name { get; set; }
            public string tag_name { get; set; }
            public bool prerelease { get; set; }
            public DateTime created_at { get; set; }
            public DateTime published_at { get; set; }
        }

        private static string _Version = string.Empty;
        public static string Version
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_Version))
                {
                    return _Version;
                }
                var v = Assembly.GetExecutingAssembly().GetName().Version.ToString();
#if BETA
                v = "Beta-" + v;
#endif
                _Version = v;
                return _Version;
            }
        }

        public void ToggleMenuButtons(bool value)
        {
            MapMenuItem.Enabled = value;
			OverlayMenuItem.Enabled = value;
			ImageOverlayMenuItem.Enabled = value;
            DpsMeterMenuItem.Enabled = value;
            MobInfoMenuItem.Enabled = value;
            GroupSuggestionsMenuItem.Enabled = value;
        }

        private void CreateStandardGroup(object sender, EventArgs e)
        {
            CreateGroup(GroupOptimization.Standard);
        }

        private void CreateHOTClericsSparseGroup(object sender, EventArgs e)
        {
            CreateGroup(GroupOptimization.HOT_Cleric_SparseGroup);
        }

        private void CreateHOTClericsSameGroup(object sender, EventArgs e)
        {
            CreateGroup(GroupOptimization.HOT_Cleric_SameGroup);
        }

        private void CreateGroup(GroupOptimization grp)
        {
            var grpstring = new List<string>();
            var groups = PlayerTrackerService.CreateGroups(grp);
            var groupindex = 1;
            foreach (var group in groups)
            {
                var str = $"/gu Group {groupindex++} ";
                foreach (var player in group.Players)
                {
                    str += player.Name + ",";
                }
                grpstring.Add(str);
            }
            if (grpstring.Any())
            {
                System.Windows.Forms.Clipboard.SetText(string.Join("\r\n", grpstring));
            }
            else
            {
                System.Windows.Forms.Clipboard.SetText("You must /who in the zone before group suggestions can be made!");
            }
        }

        private void Discord(object sender, EventArgs e)
        {
            _ = System.Diagnostics.Process.Start(new ProcessStartInfo
            {
                FileName = "https://discord.gg/rkU8ewzWWk",
                UseShellExecute = true
            });
        }

        private void Suggestions(object sender, EventArgs e)
        {
            _ = System.Diagnostics.Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/EJWellman/QuarmTool/issues",
                UseShellExecute = true
            });
        }

        private void ToggleWindow<T>(System.Windows.Forms.MenuItem m) where T : BaseSaveStateWindow
        {
            var w = WindowList.FirstOrDefault(a => a.GetType() == typeof(T));
            m.Checked = !m.Checked;
            if (m.Checked)
            {
                if (w != null)
                {
                    _ = w.Focus();
                }
                else
                {
                    w?.Close();
                    w = container.Resolve<T>();
                    WindowList.Add(w);
                    w.Closed += (se, ee) =>
                    {
                        m.Checked = false;
                        _ = WindowList.Remove(w);
                    };
                    w.Show();
                }
            }
            else
            {
                w?.CloseWindow();
                _ = WindowList.Remove(w);
            }
		}

		private void HardToggleWindow<T>() where T : BaseSaveStateWindow
		{
			var w = WindowList.FirstOrDefault(a => a.GetType() == typeof(T));
			if (w != null)
			{
				w?.CloseWindow();
				_ = WindowList.Remove(w);
			}
			else
			{
				w?.Close();
				w = container.Resolve<T>();
				WindowList.Add(w);
				w.Closed += (se, ee) =>
				{
					_ = WindowList.Remove(w);
				};
				w.Show();
			}
		}


		//private void ToggleWindowBorder<T>() where T : BaseSaveStateWindow
		//{
		//	var w = WindowList.FirstOrDefault(a => a.GetType() == typeof(T));
		//	if (w != null)
		//	{
		//		HardToggleWindow<T>();

		//		var w2 = container.Resolve<T>();
		//		w2.Closed += (se, ee) =>
		//		{
		//			_ = WindowList.Remove(w2);
		//		};
		//		w2.IsHitTestVisible = false;
		//		w2.AllowsTransparency = true;
		//		w2.Show();
		//		w2.Opacity = 25;
		//	}
		//}

		private void OpenWindow<T>(System.Windows.Forms.MenuItem m) where T : BaseSaveStateWindow
        {
            var w = WindowList.FirstOrDefault(a => a.GetType() == typeof(T));
            if (w != null)
            {
                _ = w.Focus();
            }
            else
            {
                m.Checked = true;
                w?.Close();
                w = container.Resolve<T>();
                WindowList.Add(w);
                w.Closed += (se, ee) =>
                {
                    m.Checked = false;
                    _ = WindowList.Remove(w);
                };
                w.Show();
            }
		}

		public void OpenSpawnableWindow<T>(T w) where T : BaseSaveStateWindow
		{
			if(typeof(T) == typeof(BaseTimerWindow))
			{
				if(WindowList.Any(a => a.GetType() == typeof(BaseTimerWindow)
					&& ((BaseTimerWindowViewModel)a.DataContext).ID == ((BaseTimerWindowViewModel)w.DataContext).ID))
				{
					return;
				}

				//w?.Close();
				WindowList.Add(w);
				w.Closed += (se, ee) =>
				{
					_ = WindowList.Remove(w);
				};
				w.Show();
			}
		}

		public void UpdateSpawnableTimerWindowContext(TimerWindowOptions options)
		{
			var w = WindowList.FirstOrDefault(a => a.GetType() == typeof(BaseTimerWindow)
				&& ((BaseTimerWindowViewModel)a.DataContext).ID == options.ID);
			if (w != null)
			{
				var vm = (BaseTimerWindowViewModel)w.DataContext;

				w.Title = options.Title;
				vm.WindowTitle = options.Title;
				vm.BestGuessSpells = options.BestGuessSpells;
				vm.ShowModRodTimers = options.ShowModRodTimers;
				vm.ShowDeathTouches = options.ShowDeathTouches;
				vm.ShowSpells = options.ShowSpells;
				vm.ShowTimers = options.ShowTimers;
				vm.ShowRandomRolls = options.ShowRandomRolls;
				vm.YouOnlySpells = options.YouOnlySpells;

				vm.ID = options.ID;
				vm.WindowState.AlwaysOnTop = options.AlwaysOnTop;
				vm.AlwaysOnTop = options.AlwaysOnTop;
				w.Topmost = !options.AlwaysOnTop;
				w.Topmost = options.AlwaysOnTop;
				vm.WindowState.Opacity = _settings.TimerWindowOpacity;

				vm.ShowNPCs = options.ShowNPCs;
				vm.ShowPCs = options.ShowPCs;
				vm.ShowSimpleTimers = options.ShowSimpleTimers;

				vm.UpdateSpellVisuals(options);

				w.Activate();
			}
		}

		public BaseSaveStateWindow GetSpawnableTimerWindowBase(TimerWindowOptions options)
		{
			var w = WindowList.FirstOrDefault(a => a.GetType() == typeof(BaseTimerWindow)
				&& ((BaseTimerWindowViewModel)a.DataContext).ID == options.ID);
			if (w != null)
			{
				return w;
			}
			else 
			{ 
				return null; 
			}
		}

		public void ToggleMapWindow(object sender, EventArgs e)
        {
            var s = (System.Windows.Forms.MenuItem)sender;
            ToggleWindow<MappingWindow>(s);
        }

		public void HardToggleMapWindow()
		{
			HardToggleWindow<MappingWindow>();
		}

		public void ToggleDPSWindow(object sender, EventArgs e)
        {
            var s = (System.Windows.Forms.MenuItem)sender;
            ToggleWindow<DPSMeter>(s);
		}
		public void HardToggleDPSWindow()
		{
			HardToggleWindow<DPSMeter>();
		}

		public void ToggleOverlayWindow(object sender, EventArgs e)
        {
            var s = (System.Windows.Forms.MenuItem)sender;
            ToggleWindow<EventOverlay>(s);
		}
		public void HardToggleOverlayWindow()
		{
			HardToggleWindow<EventOverlay>();
		}

		public void ToggleImageOverlayWindow(object sender, EventArgs e)
		{
			var s = (System.Windows.Forms.MenuItem)sender;
			ToggleWindow<ImageOverlay>(s);
		}
		public void HardToggleImageOverlayWindow()
		{
			HardToggleWindow<ImageOverlay>();
		}

		public void ToggleMobInfoWindow(object sender, EventArgs e)
        {
            var s = (System.Windows.Forms.MenuItem)sender;
            ToggleWindow<MobInfo>(s);
		}
		public void HardToggleMobInfoWindow()
		{
			HardToggleWindow<MobInfo>();
		}
		public void ToggleMobInfoWindowBorders()
		{
			//ToggleWindowBorder<MobInfo>();
		}

		public void ToggleSettingsWindow(object sender, EventArgs e)
        {
            var s = (System.Windows.Forms.MenuItem)sender;
            ToggleWindow<Settings>(s);
        }

		public void OpenDPSWindow()
        {
            OpenWindow<DPSMeter>(DpsMeterMenuItem);
        }

        public void OpenMapWindow()
        {
            OpenWindow<MappingWindow>(MapMenuItem);
        }

        public void OpenMobInfoWindow()
        {
            OpenWindow<MobInfo>(MobInfoMenuItem);
        }

        public void OpenOverLayWindow()
        {
            OpenWindow<EventOverlay>(OverlayMenuItem);
		}

		public void OpenImageOverLayWindow()
		{
			OpenWindow<ImageOverlay>(ImageOverlayMenuItem);
		}

		public void OpenSettingsWindow()
        {
            OpenWindow<Settings>(SettingsMenuItem);
        }

        public void ShowBalloonTip(int timeout, string tipTitle, string tipText, System.Windows.Forms.ToolTipIcon tipIcon)
        {
            this.SystemTrayIcon.ShowBalloonTip(timeout, tipTitle, tipText, tipIcon);
        }

		private void OnExit(object sender, EventArgs e)
        {
			_zealMessageService.StopProcessing();
            Current.Shutdown();
        }

		public void OpenTimerWindow(object sender, EventArgs e)
		{
			if(_timerWindowFactory == null)
			{
				_timerWindowFactory = container.Resolve<TimerWindowFactory>();
			}

			if((sender as System.Windows.Controls.MenuItem)?.Tag != null)
			{
				var contextID = (sender as System.Windows.Controls.MenuItem).Tag as int?;
				if (contextID != null)
				{
					var w = _timerWindowFactory.CreateTimerWindow((int)contextID);
					(App.Current as App).OpenSpawnableWindow<BaseTimerWindow>(w);
				}
			}
			if ((sender as System.Windows.Forms.MenuItem)?.Tag != null)
			{
				var contextID = (sender as System.Windows.Forms.MenuItem).Tag as int?;
				if (contextID != null)
				{
					var w = _timerWindowFactory.CreateTimerWindow((int)contextID);
					(App.Current as App).OpenSpawnableWindow<BaseTimerWindow>(w);
				}
			}
			else if((sender as System.Windows.Controls.MenuItem)?.DataContext != null)
			{
				var contextID = (sender as System.Windows.Controls.MenuItem).DataContext as int?;
				if (contextID != null)
				{
					var w = _timerWindowFactory.CreateTimerWindow((int)contextID);
					(App.Current as App).OpenSpawnableWindow<BaseTimerWindow>(w);
				}
			}
			else
			{
				return;
			}
		}

		public void ApplyAlwaysOnTop()
        {
            foreach (var item in WindowList)
            {
                if (item is DPSMeter w)
                {
                    w.Topmost = _settings.DpsWindowState.AlwaysOnTop;
                }
                else if (item is MappingWindow w1)
                {
                    w1.Topmost = _settings.MapWindowState.AlwaysOnTop;
                }
                else if (item is MobInfo w2)
                {
                    w2.Topmost = _settings.MobWindowState.AlwaysOnTop;
                }
				else if (item is EventOverlay w6)
				{
					w6.Topmost = _settings.OverlayWindowState.AlwaysOnTop;
					w6.Activate();
				}
				else if (item is ImageOverlay w7)
				{
					w7.Topmost = _settings.ImageOverlayWindowState.AlwaysOnTop;
					w7.Activate();
				}
			}
        }

		public void UpdateStaticOverlayColors()
		{
			foreach (var item in WindowList)
			{
				if (item is ImageOverlay w)
				{
					w.UpdateGradientColors();
				}
			}
		}
    }
}
