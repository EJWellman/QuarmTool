using EQTool.Models;
using EQToolShared.Enums;
using EQToolShared.Map;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Speech.Synthesis;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace EQTool.ViewModels
{
    public class SettingsWindowViewModel : INotifyPropertyChanged
    {
        private readonly EQToolSettings toolSettings;

        public SettingsWindowViewModel(ActivePlayer activePlayer, EQToolSettings toolSettings)
        {
            this.toolSettings = toolSettings;
            ActivePlayer = activePlayer;
            for (var i = 12; i < 72; i++)
            {
                FontSizes.Add(i);
            }
            for (var i = 1; i < 61; i++)
            {
                Levels.Add(i);
            }

            for (var i = 1; i < 201; i++)
            {
                TrackSkills.Add(i);
            }

            foreach (var item in EQToolShared.Map.ZoneParser.Zones.OrderBy(a => a))
            {
                Zones.Add(item);
            }

            foreach (var item in Enum.GetValues(typeof(PlayerClasses)).Cast<PlayerClasses>())
            {
                var listitem = new BoolStringClass { TheText = item.ToString(), TheValue = item, IsChecked = false };
                this.SelectedPlayerClasses.Add(listitem);
            }

            this.InstalledVoices = new ObservableCollection<string>(new SpeechSynthesizer().GetInstalledVoices().Select(a => a.VoiceInfo.Name).ToList());
            this.SelectedVoice = this.toolSettings.SelectedVoice;
            if (string.IsNullOrWhiteSpace(this.SelectedVoice))
            {
                this.SelectedVoice = this.InstalledVoices.FirstOrDefault();
            }
        }

        public int GlobalFontSize
        {
            get
            {
                return this.toolSettings.FontSize.Value;
            }
            set
            {
                this.toolSettings.FontSize = value;
                App.Current.Resources["GlobalFontSize"] = (double)value;
                ((App)System.Windows.Application.Current).UpdateBackgroundOpacity("MyWindowStyleDPS", this.toolSettings.DpsWindowState.Opacity.Value);
                ((App)System.Windows.Application.Current).UpdateBackgroundOpacity("MyWindowStyleMap", this.toolSettings.MapWindowState.Opacity.Value);
                ((App)System.Windows.Application.Current).UpdateBackgroundOpacity("MyWindowStyleTrigger", this.toolSettings.SpellWindowState.Opacity.Value);
                OnPropertyChanged();
            }
        }

        public bool DpsAlwaysOnTop
        {
            get
            {
                return this.toolSettings.DpsWindowState.AlwaysOnTop;
            }
            set
            {
                this.toolSettings.DpsWindowState.AlwaysOnTop = value;
                OnPropertyChanged();
            }
		}

		public int OverlayFontSize
		{
			get
			{
				return this.toolSettings.OverlayFontSize.Value;
			}
			set
			{
				this.toolSettings.OverlayFontSize = value;
				OnPropertyChanged();
			}
		}

		public int AudioTriggerVolume
		{
			get
			{
				return this.toolSettings.AudioTriggerVolume;
			}
			set
			{
				this.toolSettings.AudioTriggerVolume = value;
				OnPropertyChanged();
			}
		}

		public Color EnrageOverlayColor 
		{
			get
			{
				return this.toolSettings.EnrageOverlayColor;
			}
			set
			{
				this.toolSettings.EnrageOverlayColor = value;
				OnPropertyChanged();
			}
		}
		public Color LevFadingOverlayColor 
		{
			get
			{
				return this.toolSettings.LevFadingOverlayColor;
			}
			set
			{
				this.toolSettings.LevFadingOverlayColor = value;
				OnPropertyChanged();
			}
		}
		public Color InvisFadingOverlayColor 
		{ 
			get
			{
				return this.toolSettings.InvisFadingOverlayColor;
			}
			set
			{
				this.toolSettings.InvisFadingOverlayColor = value;
				OnPropertyChanged();
			}
		}
		public Color FTEOverlayColor 
		{ 
			get 
			{ 
				return this.toolSettings.FTEOverlayColor;
			}
			set
			{
				this.toolSettings.FTEOverlayColor = value;
				OnPropertyChanged();
			}
		}
		public Color CharmBreakOverlayColor 
		{ 
			get 
			{ 
				return this.toolSettings.CharmBreakOverlayColor;
			}
			set
			{
				this.toolSettings.CharmBreakOverlayColor = value;
				OnPropertyChanged();
			}
		}
		public Color FailedFeignOverlayColor 
		{ 
			get 
			{
				return this.toolSettings.FailedFeignOverlayColor;
			}
			set
			{
				this.toolSettings.FailedFeignOverlayColor = value;
				OnPropertyChanged();
			}
		}
		public Color GroupInviteOverlayColor 
		{ 
			get 
			{
				return this.toolSettings.GroupInviteOverlayColor;
			}
			set
			{
				this.toolSettings.GroupInviteOverlayColor = value;
				OnPropertyChanged();
			}
		}
		public Color DragonRoarOverlayColor 
		{ 
			get 
			{
				return this.toolSettings.DragonRoarOverlayColor;
			}
			set
			{
				this.toolSettings.DragonRoarOverlayColor = value;
				OnPropertyChanged();
			}
		}
		public Color RootWarningOverlayColor 
		{ 
			get 
			{
				return this.toolSettings.RootWarningOverlayColor;
			}
			set
			{
				this.toolSettings.RootWarningOverlayColor = value;
				OnPropertyChanged();
			}
		}
		public Color ResistWarningOverlayColor 
		{ 
			get 
			{
				return this.toolSettings.ResistWarningOverlayColor;
			}
			set
			{
				this.toolSettings.ResistWarningOverlayColor = value;
				OnPropertyChanged();
			}
		}

		public double DPSWindowOpacity
        {
            get
            {
                return this.toolSettings.DpsWindowState.Opacity ?? 1.0;
            }
            set
            {
                this.toolSettings.DpsWindowState.Opacity = value;
                ((App)System.Windows.Application.Current).UpdateBackgroundOpacity("MyWindowStyleDPS", value);
                OnPropertyChanged();
            }
        }

        public bool MapAlwaysOnTop
        {
            get
            {
                return this.toolSettings.MapWindowState.AlwaysOnTop;
            }
            set
            {
                this.toolSettings.MapWindowState.AlwaysOnTop = value;
                OnPropertyChanged();
            }
        }

        public double MapWindowOpacity
        {
            get
            {
                return this.toolSettings.MapWindowState.Opacity ?? 1.0;
            }

            set
            {
                this.toolSettings.MapWindowState.Opacity = value;
                ((App)System.Windows.Application.Current).UpdateBackgroundOpacity("MyWindowStyleMap", value);
                OnPropertyChanged();
            }
        }
        public bool MobAlwaysOnTop
        {
            get
            {
                return this.toolSettings.MobWindowState.AlwaysOnTop;
            }
            set
            {
                this.toolSettings.MobWindowState.AlwaysOnTop = value;
                OnPropertyChanged();
            }
        }

        public bool ComboAlwaysOnTop
        {
            get
            {
                return this.toolSettings.ComboTimerWindowState.AlwaysOnTop;
            }
            set
            {
                this.toolSettings.ComboTimerWindowState.AlwaysOnTop = value;
                OnPropertyChanged();
            }
		}

		public bool SpellAlwaysOnTop
		{
			get
			{
				return this.toolSettings.SpellWindowState.AlwaysOnTop;
			}
			set
			{
				this.toolSettings.SpellWindowState.AlwaysOnTop = value;
				OnPropertyChanged();
			}
		}

		public bool TimerAlwaysOnTop
		{
			get
			{
				return this.toolSettings.TimerWindowState.AlwaysOnTop;
			}
			set
			{
				this.toolSettings.TimerWindowState.AlwaysOnTop = value;
				OnPropertyChanged();
			}
		}

		public bool OverlayAlwaysOnTop
		{
			get
			{
				return this.toolSettings.OverlayWindowState.AlwaysOnTop;
			}
			set
			{
				this.toolSettings.OverlayWindowState.AlwaysOnTop = value;
				OnPropertyChanged();
			}
		}

		public bool ComboShowRandomRolls
		{
			get
			{
				return this.toolSettings.ComboShowRandomRolls;
			}
			set
			{
				this.toolSettings.ComboShowRandomRolls = value;
				OnPropertyChanged();
			}
		}
		public bool ComboShowSpells
		{
			get
			{
				return this.toolSettings.ComboShowSpells;
			}
			set
			{
				this.toolSettings.ComboShowSpells = value;
				OnPropertyChanged();
			}
		}
		public bool ComboShowTimers
		{
			get
			{
				return this.toolSettings.ComboShowTimers;
			}
			set
			{
				this.toolSettings.ComboShowTimers = value;
				OnPropertyChanged();
			}
		}
		public bool ComboShowModRodTimers
		{
			get
			{
				return this.toolSettings.ComboShowModRodTimers;
			}
			set
			{
				this.toolSettings.ComboShowModRodTimers = value;
				OnPropertyChanged();
			}
		}

		public ObservableCollection<CustomOverlay> CustomOverlays
		{
			get
			{
				return this.toolSettings.CustomOverlays;
			}
		}

		public bool ShowRandomRolls
        {
            get
            {
                return this.toolSettings.ShowRandomRolls;
            }
            set
            {
                this.toolSettings.ShowRandomRolls = value;
                OnPropertyChanged();
            }
        }

        public double TriggerWindowOpacity
        {
            get
            {
                return this.toolSettings.SpellWindowState.Opacity ?? 1.0;
            }
            set
            {
                this.toolSettings.SpellWindowState.Opacity = value;
                ((App)System.Windows.Application.Current).UpdateBackgroundOpacity("MyWindowStyleTrigger", value);
                OnPropertyChanged();
            }
        }

        private ActivePlayer _ActivePlayer;
        public ActivePlayer ActivePlayer
        {
            get => _ActivePlayer;
            set
            {
                _ActivePlayer = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasCharName));
                OnPropertyChanged(nameof(HasNoCharName));
            }
        }

        private string _SelectedVoice = string.Empty;
        public string SelectedVoice
        {
            get => _SelectedVoice;
            set
            {
                _SelectedVoice = value;
                this.toolSettings.SelectedVoice = value;
                OnPropertyChanged();
            }
        }

        private string _EqLogPath = string.Empty;
        public string EqLogPath
        {
            get => _EqLogPath;
            set
            {
                _EqLogPath = value;
                OnPropertyChanged();
            }
        }

        private string _EqPath = string.Empty;
        public string EqPath
        {
            get => _EqPath;
            set
            {
                _EqPath = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DoesNotHaveEqPath));
                OnPropertyChanged(nameof(HasEqPath));
                OnPropertyChanged(nameof(MissingConfiguration));
                OnPropertyChanged(nameof(NotMissingConfiguration));
            }
        }

        public bool DoesNotHaveEqPath => string.IsNullOrWhiteSpace(_EqPath);
        public bool HasEqPath => !string.IsNullOrWhiteSpace(_EqPath);

        private bool _IsLoggingEnabled = false;

        public bool IsLoggingEnabled
        {
            get => _IsLoggingEnabled;
            set
            {
                _IsLoggingEnabled = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsLoggingDisabled));
                OnPropertyChanged(nameof(MissingConfiguration));
                OnPropertyChanged(nameof(NotMissingConfiguration));
            }
        }

        public bool IsLoggingDisabled => !_IsLoggingEnabled;

        public bool MissingConfiguration => DoesNotHaveEqPath || IsLoggingDisabled;
        public bool NotMissingConfiguration => HasEqPath && IsLoggingEnabled;
        public bool HasCharName => !string.IsNullOrWhiteSpace(ActivePlayer?.Player?.Name);
        public Visibility HasNoCharName => string.IsNullOrWhiteSpace(ActivePlayer?.Player?.Name) ? Visibility.Visible : Visibility.Collapsed;
        public ObservableCollection<string> InstalledVoices { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<BoolStringClass> SelectedPlayerClasses { get; set; } = new ObservableCollection<BoolStringClass>();
        public List<MapLocationSharing> LocationShareOptions => Enum.GetValues(typeof(MapLocationSharing)).Cast<MapLocationSharing>().ToList();
        public List<PlayerClasses> PlayerClasses => Enum.GetValues(typeof(PlayerClasses)).Cast<PlayerClasses>().ToList();

        public bool EqRunning
        {
            get => IsEqNotRunning;
            set
            {
                IsEqNotRunning = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsEqNotRunning));
            }
        }

        public bool TestingMode
        {
            get
            {
                var ret = true;
#if DEBUG
                ret = true;
#else

                ret = false;
#endif
                return ret;
            }
        }
        public bool IsEqRunning => !IsEqNotRunning;
        public bool IsEqNotRunning { get; private set; } = false;

        public ObservableCollection<string> Zones { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<int> FontSizes { get; set; } = new ObservableCollection<int>();
        public ObservableCollection<int> Levels { get; set; } = new ObservableCollection<int>();
        public ObservableCollection<int?> TrackSkills { get; set; } = new ObservableCollection<int?>();

		#region Timer Colors
		public Color SpellTimerNameColor
		{
			get
			{
				return this.toolSettings.SpellTimerNameColor;
			}
			set
			{
				this.toolSettings.SpellTimerNameColor = value;
				OnPropertyChanged();
			}
		}
		public Color BeneficialSpellTimerColor
		{ 
			get 
			{
				return this.toolSettings.BeneficialSpellTimerColor;
			}
			set
			{
				this.toolSettings.BeneficialSpellTimerColor = value;
				OnPropertyChanged();
			}
		}
		public Color DetrimentalSpellTimerColor
		{ 
			get 
			{
				return this.toolSettings.DetrimentalSpellTimerColor;
			}
			set
			{
				this.toolSettings.DetrimentalSpellTimerColor = value;
				OnPropertyChanged();
			}
		}
		public Color RespawnTimerColor
		{ 
			get 
			{
				return this.toolSettings.RespawnTimerColor;
			}
			set
			{
				this.toolSettings.RespawnTimerColor = value;
				OnPropertyChanged();
			}
		}
		public Color DisciplineTimerColor
		{ 
			get 
			{
				return this.toolSettings.DisciplineTimerColor;
			}
			set
			{
				this.toolSettings.DisciplineTimerColor = value;
				OnPropertyChanged();
			}
		}
		public Color ModRodTimerColor
		{ 
			get 
			{
				return this.toolSettings.ModRodTimerColor;
			}
			set
			{
				this.toolSettings.ModRodTimerColor = value;
				OnPropertyChanged();
			}
		}
		public Color OtherTimerColor
		{ 
			get 
			{
				return this.toolSettings.OtherTimerColor;
			}
			set
			{
				this.toolSettings.OtherTimerColor = value;
				OnPropertyChanged();
			}
		}
		#endregion

		public void Update()
        {
            _ = ActivePlayer.Update();
            OnPropertyChanged(nameof(ActivePlayer));
            OnPropertyChanged(nameof(HasCharName));
            OnPropertyChanged(nameof(HasNoCharName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
