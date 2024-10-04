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
using static EQTool.Services.FindEq;

namespace EQTool.ViewModels
{
    public class SettingsWindowViewModel : INotifyPropertyChanged
    {
        private readonly EQToolSettings _settings;

        public SettingsWindowViewModel(ActivePlayer activePlayer, EQToolSettings toolSettings)
        {
            this._settings = toolSettings;
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

            foreach (var item in EQToolShared.Map.ZoneParser.Zones.Distinct().OrderBy(a => a))
            {
                Zones.Add(item);
            }

            foreach (var item in Enum.GetValues(typeof(PlayerClasses)).Cast<PlayerClasses>())
            {
                var listitem = new BoolStringClass { TheText = item.ToString(), TheValue = item, IsChecked = false };
                this.SelectedPlayerClasses.Add(listitem);
            }

            this.InstalledVoices = new ObservableCollection<string>(new SpeechSynthesizer().GetInstalledVoices().Select(a => a.VoiceInfo.Name).ToList());
            this.SelectedVoice = this._settings.SelectedVoice;
            if (string.IsNullOrWhiteSpace(this.SelectedVoice))
            {
                this.SelectedVoice = this.InstalledVoices.FirstOrDefault();
            }
        }

        public int GlobalFontSize
        {
            get
            {
                return this._settings.FontSize.Value;
            }
            set
            {
                this._settings.FontSize = value;
                App.Current.Resources["GlobalFontSize"] = (double)value;
                ((App)System.Windows.Application.Current).UpdateBackgroundOpacity("MyWindowStyleDPS", this._settings.DpsWindowState.Opacity.Value);
                ((App)System.Windows.Application.Current).UpdateBackgroundOpacity("MyWindowStyleMap", this._settings.MapWindowState.Opacity.Value);
				((App)System.Windows.Application.Current).UpdateBackgroundOpacity("MyMobWindowSyle", _settings.MobWindowState.Opacity.Value);
				OnPropertyChanged();
            }
        }

        public bool DpsAlwaysOnTop
        {
            get
            {
                return this._settings.DpsWindowState.AlwaysOnTop;
            }
            set
            {
                this._settings.DpsWindowState.AlwaysOnTop = value;
                OnPropertyChanged();
            }
		}

		public int OverlayFontSize
		{
			get
			{
				return this._settings.OverlayFontSize.Value;
			}
			set
			{
				this._settings.OverlayFontSize = value;
				OnPropertyChanged();
			}
		}

		public int AudioTriggerVolume
		{
			get
			{
				return this._settings.AudioTriggerVolume;
			}
			set
			{
				this._settings.AudioTriggerVolume = value;
				OnPropertyChanged();
			}
		}

		public double MapLabelMultiplier
		{
			get
			{
				return _settings.MapLabelMultiplier;
			}
			set
			{
				_settings.MapLabelMultiplier = value;
				OnPropertyChanged();
			}
		}

		public double BeneficialSpellDurationMultiplier
		{
			get
			{
				return _settings.BeneficialSpellDurationMultiplier;
			}
			set
			{
				_settings.BeneficialSpellDurationMultiplier = value;
				OnPropertyChanged();
			}
		}
		public double DetrimentalSpellDurationMultiplier
		{
			get
			{
				return _settings.DetrimentalSpellDurationMultiplier;
			}
			set
			{
				_settings.DetrimentalSpellDurationMultiplier = value;
				OnPropertyChanged();
			}
		}

		public double SpawnRateMultiplier
		{
			get
			{
				return _settings.SpawnRateMultiplier;
			}
			set
			{
				_settings.SpawnRateMultiplier = value;
				OnPropertyChanged();
			}
		}


		#region Zeal
		public bool ZealMap_AutoUpdate
		{
			get
			{
				return _settings.ZealMap_AutoUpdate;
			}
			set
			{
				_settings.ZealMap_AutoUpdate = value;
				OnPropertyChanged();
			}
		}
		public bool ZealZone_AutoUpdate
		{
			get
			{
				return _settings.ZealZone_AutoUpdate;
			}
			set
			{
				_settings.ZealZone_AutoUpdate = value;
				OnPropertyChanged();
			}
		}
		public bool ZealMobInfo_AutoUpdate
		{
			get 
			{
				return _settings.ZealMobInfo_AutoUpdate;
			}
			set
			{
				_settings.ZealMobInfo_AutoUpdate = value;
				OnPropertyChanged();
			}
		}
		public bool Zeal_CastingEnabled
		{
			get
			{
				return _settings.Zeal_CastingEnabled;
			}
			set
			{
				_settings.Zeal_CastingEnabled = value;
				OnPropertyChanged();
			}
		}

		public string Zeal_HealthThreshold
		{
			get
			{
				return _settings.Zeal_HealthThreshold.ToString();
			}
			set
			{
				_settings.Zeal_HealthThreshold = decimal.TryParse(value, out decimal result) ? result : 0;
				OnPropertyChanged();
			}
		}

		public string Zeal_ManaThreshold
		{
			get
			{
				return _settings.Zeal_ManaThreshold.ToString();
			}
			set
			{
				_settings.Zeal_ManaThreshold = decimal.TryParse(value, out decimal result) ? result : 0;
				OnPropertyChanged();
			}
		}

		public bool Zeal_HealthThresholdEnabled
		{
			get
			{
				return _settings.Zeal_HealthThresholdEnabled;
			}
			set
			{
				_settings.Zeal_HealthThresholdEnabled = value;
				if (!value)
				{

				}
				OnPropertyChanged();
			}
		}

		public bool Zeal_ManaThresholdEnabled
		{
			get
			{
				return _settings.Zeal_ManaThresholdEnabled;
			}
			set
			{
				_settings.Zeal_ManaThresholdEnabled = value;
				OnPropertyChanged();
			}
		}

		public bool Mana_ShowTop
		{
			get
			{
				return _settings.Mana_ShowTop;
			}
			set
			{
				_settings.Mana_ShowTop = value;
				OnPropertyChanged();
			}
		}
		public bool Mana_ShowLeft
		{
			get
			{
				return _settings.Mana_ShowLeft;
			}
			set
			{
				_settings.Mana_ShowLeft = value;
				OnPropertyChanged();
			}
		}
		public bool Mana_ShowRight
		{
			get
			{
				return _settings.Mana_ShowRight;
			}
			set
			{
				_settings.Mana_ShowRight = value;
				OnPropertyChanged();
			}
		}
		public bool Mana_ShowBottom
		{
			get
			{
				return _settings.Mana_ShowBottom;
			}
			set
			{
				_settings.Mana_ShowBottom = value;
				OnPropertyChanged();
			}
		}
		public bool Health_ShowTop
		{
			get
			{
				return _settings.Health_ShowTop;
			}
			set
			{
				_settings.Health_ShowTop = value;
				OnPropertyChanged();
			}
		}
		public bool Health_ShowLeft
		{
			get
			{
				return _settings.Health_ShowLeft;
			}
			set
			{
				_settings.Health_ShowLeft = value;
				OnPropertyChanged();
			}
		}
		public bool Health_ShowRight
		{
			get
			{
				return _settings.Health_ShowRight;
			}
			set
			{
				_settings.Health_ShowRight = value;
				OnPropertyChanged();
			}
		}
		public bool Health_ShowBottom
		{
			get
			{
				return _settings.Health_ShowBottom;
			}
			set
			{
				_settings.Health_ShowBottom = value;
				OnPropertyChanged();
			}
		}

		public Color Health_Color
		{
			get
			{
				return _settings.Health_Color;
			}
			set
			{
				_settings.Health_Color = value;
				OnPropertyChanged();
			}
		}

		public Color Mana_Color
		{
			get
			{
				return _settings.Mana_Color;
			}
			set
			{
				_settings.Mana_Color = value;
				OnPropertyChanged();
			}
		}

		public Color Health_Mana_Color
		{
			get
			{
				return _settings.Health_Mana_Color;
			}
			set
			{
				_settings.Health_Mana_Color = value;
				OnPropertyChanged();
			}
		}

		public decimal StaticOverlay_SizeTop
		{
			get
			{
				return _settings.StaticOverlay_SizeTop;
			}
			set
			{
				_settings.StaticOverlay_SizeTop = value;
				OnPropertyChanged();
			}
		}
		public decimal StaticOverlay_SizeLeft
		{
			get
			{
				return _settings.StaticOverlay_SizeLeft;
			}
			set
			{
				_settings.StaticOverlay_SizeLeft = value;
				OnPropertyChanged();
			}
		}
		public decimal StaticOverlay_SizeRight
		{
			get
			{
				return _settings.StaticOverlay_SizeRight;
			}
			set
			{
				_settings.StaticOverlay_SizeRight = value;
				OnPropertyChanged();
			}
		}
		public decimal StaticOverlay_SizeBottom
		{
			get
			{
				return _settings.StaticOverlay_SizeBottom;
			}
			set
			{
				_settings.StaticOverlay_SizeBottom = value;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Overlay Colors
		public Color EnrageOverlayColor 
		{
			get
			{
				return this._settings.EnrageOverlayColor;
			}
			set
			{
				this._settings.EnrageOverlayColor = value;
				OnPropertyChanged();
			}
		}
		public Color LevFadingOverlayColor 
		{
			get
			{
				return this._settings.LevFadingOverlayColor;
			}
			set
			{
				this._settings.LevFadingOverlayColor = value;
				OnPropertyChanged();
			}
		}
		public Color InvisFadingOverlayColor 
		{ 
			get
			{
				return this._settings.InvisFadingOverlayColor;
			}
			set
			{
				this._settings.InvisFadingOverlayColor = value;
				OnPropertyChanged();
			}
		}
		public Color FTEOverlayColor 
		{ 
			get 
			{ 
				return this._settings.FTEOverlayColor;
			}
			set
			{
				this._settings.FTEOverlayColor = value;
				OnPropertyChanged();
			}
		}
		public Color CharmBreakOverlayColor 
		{ 
			get 
			{ 
				return this._settings.CharmBreakOverlayColor;
			}
			set
			{
				this._settings.CharmBreakOverlayColor = value;
				OnPropertyChanged();
			}
		}
		public Color FailedFeignOverlayColor 
		{ 
			get 
			{
				return this._settings.FailedFeignOverlayColor;
			}
			set
			{
				this._settings.FailedFeignOverlayColor = value;
				OnPropertyChanged();
			}
		}
		public Color GroupInviteOverlayColor 
		{ 
			get 
			{
				return this._settings.GroupInviteOverlayColor;
			}
			set
			{
				this._settings.GroupInviteOverlayColor = value;
				OnPropertyChanged();
			}
		}
		public Color DragonRoarOverlayColor 
		{ 
			get 
			{
				return this._settings.DragonRoarOverlayColor;
			}
			set
			{
				this._settings.DragonRoarOverlayColor = value;
				OnPropertyChanged();
			}
		}
		public Color RootWarningOverlayColor 
		{ 
			get 
			{
				return this._settings.RootWarningOverlayColor;
			}
			set
			{
				this._settings.RootWarningOverlayColor = value;
				OnPropertyChanged();
			}
		}
		public Color ResistWarningOverlayColor 
		{ 
			get 
			{
				return this._settings.ResistWarningOverlayColor;
			}
			set
			{
				this._settings.ResistWarningOverlayColor = value;
				OnPropertyChanged();
			}
		}
		#endregion Overlay Colors

		public double DPSWindowOpacity
        {
            get
            {
                return this._settings.DpsWindowState.Opacity ?? 1.0;
            }
            set
            {
                this._settings.DpsWindowState.Opacity = value;
                ((App)System.Windows.Application.Current).UpdateBackgroundOpacity("MyWindowStyleDPS", value);
                OnPropertyChanged();
            }
        }

		public int DpsRemovalTimerThreshold
		{
			get
			{
				return this._settings.DpsRemovalTimerThreshold;
			}
			set
			{
				this._settings.DpsRemovalTimerThreshold = value;
				OnPropertyChanged();
			}
		}


		public bool MapAlwaysOnTop
        {
            get
            {
                return this._settings.MapWindowState.AlwaysOnTop;
            }
            set
            {
                this._settings.MapWindowState.AlwaysOnTop = value;
                OnPropertyChanged();
            }
        }

        public double MapWindowOpacity
        {
            get
            {
                return this._settings.MapWindowState.Opacity ?? 1.0;
            }

            set
            {
                this._settings.MapWindowState.Opacity = value;
                ((App)System.Windows.Application.Current).UpdateBackgroundOpacity("MyWindowStyleMap", value);
                OnPropertyChanged();
            }
		}
		public double MobWindowOpacity
		{
			get
			{
				return this._settings.MobWindowState.Opacity ?? 1.0;
			}

			set
			{
				this._settings.MobWindowState.Opacity = value;
				((App)System.Windows.Application.Current).UpdateBackgroundOpacity("MyMobWindowSyle", value);
				((App)System.Windows.Application.Current).UpdateBackgroundOpacity("MyListViewStyle", value);
				
				OnPropertyChanged();
			}
		}

		public bool MobAlwaysOnTop
        {
            get
            {
                return this._settings.MobWindowState.AlwaysOnTop;
            }
            set
            {
                this._settings.MobWindowState.AlwaysOnTop = value;
                OnPropertyChanged();
            }
        }

		public bool MobInfo_ManualSizing
		{
			get
			{
				return _settings.MobInfo_ManualSizing;
			}
			set
			{
				_settings.MobInfo_ManualSizing = value;
				OnPropertyChanged();
			}
		}

		public bool MapTrackingVisiblity
		{
			get
			{
				return _settings.TrackingVisibility;
			}
			set
			{
				_settings.TrackingVisibility = value;
				OnPropertyChanged();
			}
		}

		public bool HideSmallLabels
		{
			get
			{
				return _settings.HideSmallLabels;
			}
			set
			{
				_settings.HideSmallLabels = value;
				OnPropertyChanged();
			}
		}

		public bool OverlayAlwaysOnTop
		{
			get
			{
				return this._settings.OverlayWindowState.AlwaysOnTop;
			}
			set
			{
				this._settings.OverlayWindowState.AlwaysOnTop = value;
				OnPropertyChanged();
			}
		}

		public bool GuildInstance_Force
		{
			get
			{
				return _settings.GuildInstance_Force;
			}
			set
			{
				_settings.GuildInstance_Force = value;
				OnPropertyChanged();
			}
		}

		public ObservableCollection<CustomOverlay> CustomOverlays
		{
			get
			{
				return this._settings.CustomOverlays;
			}
		}

		public ObservableCollection<TimerWindowOptions> TimerWindows
		{
			get
			{
				return this._settings.TimerWindows;
			}
		}

		public double TimerWindowOpacity
		{
			get
			{
				return _settings.TimerWindowOpacity;
			}
			set
			{
				_settings.TimerWindowOpacity = value;
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
                this._settings.SelectedVoice = value;
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

		public string SelectedCharacter
		{
			get
			{
				return _settings.SelectedCharacter;
			}
			set
			{
				_settings.SelectedCharacter = value;
				OnPropertyChanged();
				OnPropertyChanged(_settings.SelectedCharacter);
			}
		}
				
		public ObservableCollection<string> AvailableCharacters
		{
			get
			{
				return _settings.AvailableCharacters;
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
				return this._settings.SpellTimerNameColor;
			}
			set
			{
				this._settings.SpellTimerNameColor = value;
				OnPropertyChanged();
			}
		}
		public Color BeneficialSpellTimerColor
		{ 
			get 
			{
				return this._settings.BeneficialSpellTimerColor;
			}
			set
			{
				this._settings.BeneficialSpellTimerColor = value;
				OnPropertyChanged();
			}
		}
		public Color DetrimentalSpellTimerColor
		{ 
			get 
			{
				return this._settings.DetrimentalSpellTimerColor;
			}
			set
			{
				this._settings.DetrimentalSpellTimerColor = value;
				OnPropertyChanged();
			}
		}
		public Color RespawnTimerColor
		{ 
			get 
			{
				return this._settings.RespawnTimerColor;
			}
			set
			{
				this._settings.RespawnTimerColor = value;
				OnPropertyChanged();
			}
		}
		public Color DisciplineTimerColor
		{ 
			get 
			{
				return this._settings.DisciplineTimerColor;
			}
			set
			{
				this._settings.DisciplineTimerColor = value;
				OnPropertyChanged();
			}
		}
		public Color ModRodTimerColor
		{ 
			get 
			{
				return this._settings.ModRodTimerColor;
			}
			set
			{
				this._settings.ModRodTimerColor = value;
				OnPropertyChanged();
			}
		}
		public Color OtherTimerColor
		{ 
			get 
			{
				return this._settings.OtherTimerColor;
			}
			set
			{
				this._settings.OtherTimerColor = value;
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
