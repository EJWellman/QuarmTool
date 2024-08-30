using EQTool.Services;
using EQToolShared.ExtendedClasses;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Windows;
using ZealPipes.Services;
using System.Collections.ObjectModel;
using System.Windows.Media.Media3D;
using System.Linq;

namespace EQTool.Models
{
	public class WindowState
	{
		public Rect? WindowRect { get; set; }
		public System.Windows.WindowState State { get; set; }
		public bool Closed { get; set; }
		public bool AlwaysOnTop { get; set; }

		private double _Opacity = 1.0;
		public double? Opacity
		{
			get
			{
				return _Opacity;
			}
			set
			{
				_Opacity = value ?? 1.0;
			}
		}
	}

	public class EQToolSettings : INotifyPropertyChanged
	{
		public string DefaultEqDirectory { get; set; }
		public string EqLogDirectory { get; set; }
		public string SelectedVoice { get; set; }
		private int _FontSize = 12;
		public int? FontSize
		{
			get
			{
				return _FontSize;
			}
			set
			{
				_FontSize = value ?? 12;
				_FontSize = _FontSize < 12 ? 12 : _FontSize;
			}
		}

		private int? _OverlayFontSize = 24;
		public int? OverlayFontSize
		{
			get
			{
				return _OverlayFontSize;
			}
			set
			{
				_OverlayFontSize = value ?? 24;
				_OverlayFontSize = _OverlayFontSize < 12 ? 12 : _OverlayFontSize;
			}
		}

		private Color _EnrageOverlayColor = Color.FromRgb(255, 255, 0); //Red
		private Color _LevFadingOverlayColor = Color.FromRgb(255, 255, 0); //Red
		private Color _InvisFadingOverlayColor = Color.FromRgb(255, 255, 0); //Red
		private Color _FTEOverlayColor = Color.FromRgb(255, 255, 0); //Red
		private Color _CharmBreakOverlayColor = Color.FromRgb(255, 255, 0); //Red
		private Color _FailedFeignOverlayColor = Color.FromRgb(255, 255, 0); //Red
		private Color _GroupInviteOverlayColor = Color.FromRgb(255, 255, 0); //Red
		private Color _DragonRoarOverlayColor = Color.FromRgb(255, 255, 0); //Red
		private Color _RootWarningOverlayColor = Color.FromRgb(255, 255, 0); //Red
		private Color _ResistWarningOverlayColor = Color.FromRgb(255, 255, 0); //Red

		private double _atkIndicator_Top = 400;
		public double AtkIndicator_Top
		{
			get
			{
				return _atkIndicator_Top;
			}
			set
			{
				_atkIndicator_Top = value;
				OnPropertyChanged();
			}
		}
		private double _atkIndicator_Left = 400;
		public double AtkIndicator_Left
		{
			get
			{
				return _atkIndicator_Left;
			}
			set
			{
				_atkIndicator_Left = value;
				OnPropertyChanged();
			}
		}


		public System.Windows.Media.Color ResistWarningOverlayColor
		{ 
			get => _ResistWarningOverlayColor; 
			set => _ResistWarningOverlayColor = value; 
		}
		public System.Windows.Media.Color RootWarningOverlayColor
		{ 
			get => _RootWarningOverlayColor; 
			set => _RootWarningOverlayColor = value; 
		}
		public System.Windows.Media.Color DragonRoarOverlayColor 
		{ 
			get => _DragonRoarOverlayColor; 
			set => _DragonRoarOverlayColor = value; 
		}
		public System.Windows.Media.Color GroupInviteOverlayColor
		{ 
			get => _GroupInviteOverlayColor; 
			set => _GroupInviteOverlayColor = value; 
		}
		public System.Windows.Media.Color FailedFeignOverlayColor
		{ 
			get => _FailedFeignOverlayColor; 
			set => _FailedFeignOverlayColor = value; 
		}
		public System.Windows.Media.Color CharmBreakOverlayColor
		{ 
			get => _CharmBreakOverlayColor; 
			set => _CharmBreakOverlayColor = value; 
		}
		public System.Windows.Media.Color FTEOverlayColor
		{ 
			get => _FTEOverlayColor; 
			set => _FTEOverlayColor = value;
		}
		public System.Windows.Media.Color InvisFadingOverlayColor
		{ 
			get => _InvisFadingOverlayColor; 
			set => _InvisFadingOverlayColor = value; 
		}
		public System.Windows.Media.Color LevFadingOverlayColor
		{ 
			get => _LevFadingOverlayColor; 
			set => _LevFadingOverlayColor = value; 
		}
		public System.Windows.Media.Color EnrageOverlayColor
		{ 
			get => _EnrageOverlayColor; 
			set => _EnrageOverlayColor = value; 
		}

		private WindowState _OverlayWindowState;
		public WindowState OverlayWindowState
		{
			get
			{
				if (_OverlayWindowState == null)
				{
					_OverlayWindowState = new WindowState();
				}
				return _OverlayWindowState;
			}
			set => _OverlayWindowState = value ?? new WindowState();
		}

		private WindowState _imageOverlayWindowState;
		public WindowState ImageOverlayWindowState
		{
			get
			{
				if (_imageOverlayWindowState == null)
				{
					_imageOverlayWindowState = new WindowState();
				}
				return _imageOverlayWindowState;
			}
			set => _imageOverlayWindowState = value ?? new WindowState();
		}

		private WindowState _DpsWindowState;
		public WindowState DpsWindowState
		{
			get
			{
				if (_DpsWindowState == null)
				{
					_DpsWindowState = new WindowState();
				}
				return _DpsWindowState;
			}
			set => _DpsWindowState = value ?? new WindowState();
		}

		private int _dpsRemovalTimerThreshold = 45;
		public int DpsRemovalTimerThreshold
		{
			get
			{
				return _dpsRemovalTimerThreshold;
			}
			set
			{
				_dpsRemovalTimerThreshold = value;
				OnPropertyChanged();
			}
		}

		private WindowState _MapWindowState;
		public WindowState MapWindowState
		{
			get
			{
				if (_MapWindowState == null)
				{
					_MapWindowState = new WindowState();
				}
				return _MapWindowState;
			}
			set => _MapWindowState = value ?? new WindowState();
		}

		private double _mapLabelMultiplier = 1.0;
		public double MapLabelMultiplier
		{
			get
			{
				return _mapLabelMultiplier;
			}
			set
			{
				_mapLabelMultiplier = value;
				OnPropertyChanged();
			}
		}

		private bool _trackingVisibility = true;
		public bool TrackingVisibility
		{
			get
			{
				return _trackingVisibility;
			}
			set
			{
				_trackingVisibility = value;
				OnPropertyChanged();
			}
		}

		private bool _hideSmallLabels = false;
		public bool HideSmallLabels
		{
			get
			{
				return _hideSmallLabels;
			}
			set
			{
				_hideSmallLabels = value;
				OnPropertyChanged();
			}
		}

		private bool _mobInfo_ManualSizing = false;
		public bool MobInfo_ManualSizing
		{
			get
			{
				return _mobInfo_ManualSizing;
			}
			set
			{
				_mobInfo_ManualSizing = value;
				OnPropertyChanged();
			}
		}

		private WindowState _MobWindowState;
		public WindowState MobWindowState
		{
			get
			{
				if (_MobWindowState == null)
				{
					_MobWindowState = new WindowState();
				}
				return _MobWindowState;
			}
			set => _MobWindowState = value ?? new WindowState();
		}

		private WindowState _SettingsWindowState;
		public WindowState SettingsWindowState
		{
			get
			{
				if (_SettingsWindowState == null)
				{
					_SettingsWindowState = new WindowState();
				}
				return _SettingsWindowState;
			}
			set => _SettingsWindowState = value ?? new WindowState();
		}

		public List<PlayerInfo> Players { get; set; } = new List<PlayerInfo>();

		public bool ShowTimerDropShadows { get; set; } = false;
		public Color SpellTimerNameColor { get; set; } = Color.FromRgb(255, 255, 255);
		public Color BeneficialSpellTimerColor { get; set; } = Color.FromRgb(102, 205, 170);
		public Color DetrimentalSpellTimerColor { get; set; } = Color.FromRgb(255, 69, 0);
		public Color RespawnTimerColor { get; set; } = Color.FromRgb(255, 160, 122);
		public Color DisciplineTimerColor { get; set; } = Color.FromRgb(255, 215, 0);
		public Color ModRodTimerColor { get; set; } = Color.FromRgb(255, 215, 0);
		public Color OtherTimerColor { get; set; } = Color.FromRgb(143, 188, 143);

		private ObservableCollectionRange<CustomOverlay> _customOverlays = new ObservableCollectionRange<CustomOverlay>();
		[JsonIgnore]
		public ObservableCollectionRange<CustomOverlay> CustomOverlays
		{
			get
			{
				if (_customOverlays.Count == 0)
				{
					var tmp = CustomOverlayService.LoadCustomOverlayMessages();
					if(tmp != null)
					{
						_customOverlays.AddRange(tmp);
					}
				}
				return _customOverlays;
			}
			set
			{
				_customOverlays = value;
				OnPropertyChanged();
			}
		}
		[JsonIgnore]
		public ZealMessageService ZealMessageService { get; set; }

		private ObservableCollectionRange<TimerWindowOptions> _timerWindows = new ObservableCollectionRange<TimerWindowOptions>();
		[JsonIgnore]
		public ObservableCollectionRange<TimerWindowOptions> TimerWindows
		{
			get
			{
				if (_timerWindows.Count == 0)
				{
					var tmp = TimerWindowService.LoadTimerWindows();
					if (tmp != null)
					{
						_timerWindows.AddRange(tmp);
					}
				}
				return _timerWindows;
			}
			set
			{
				_timerWindows = value;
				OnPropertyChanged();
			}
		}

		private double _timerWindowOpacity = 1.0;
		public double TimerWindowOpacity
		{
			get
			{
				return _timerWindowOpacity;
			}
			set
			{
				_timerWindowOpacity = value;
				OnPropertyChanged();
			}
		}

		private int _AudioTriggerVolume = 100;
		public int AudioTriggerVolume
		{
			get
			{
				return _AudioTriggerVolume;
			}
			set
			{
				_AudioTriggerVolume = value;
			}
		}

		#region Zeal
		//private bool _zealEnabled;
		//public bool ZealEnabled
		//{
		//	get
		//	{
		//		return _zealEnabled;
		//	}
		//	set
		//	{
		//		_zealEnabled = value;
		//		OnPropertyChanged();
		//	}
		//}
		private bool _zealMap_AutoUpdate;
		public bool ZealMap_AutoUpdate
		{
			get
			{
				return _zealMap_AutoUpdate;
			}
			set
			{
				_zealMap_AutoUpdate = value;
				OnPropertyChanged();
			}
		}
		public bool _zealZone_AutoUpdate;
		public bool ZealZone_AutoUpdate
		{
			get
			{
				return _zealZone_AutoUpdate;
			}
			set
			{
				_zealZone_AutoUpdate = value;
				OnPropertyChanged();
			}
		}

		private bool _zealMobInfo_AutoUpdate;
		public bool ZealMobInfo_AutoUpdate
		{
			get
			{
				return _zealMobInfo_AutoUpdate;
			}
			set
			{
				_zealMobInfo_AutoUpdate = value;
				OnPropertyChanged();
			}
		}

		private int _zealProcessID = 0;
		[JsonIgnore]
		public int ZealProcessID
		{
			get
			{
				return _zealProcessID;
			}
			set
			{
				_zealProcessID = value;
				OnPropertyChanged();
			}
		}

		private bool _zeal_HealthThresholdEnabled = false;
		public bool Zeal_HealthThresholdEnabled
		{
			get
			{
				return _zeal_HealthThresholdEnabled;
			}
			set
			{
				_zeal_HealthThresholdEnabled = value;
				OnPropertyChanged();
			}
		}

		private decimal _zeal_HealthThreshold = 0;
		public decimal Zeal_HealthThreshold
		{
			get
			{
				return _zeal_HealthThreshold;
			}
			set
			{
				_zeal_HealthThreshold = value;
				OnPropertyChanged();
			}
		}

		private bool _zealManaThresholdEnabled = false;
		public bool Zeal_ManaThresholdEnabled
		{
			get
			{
				return _zealManaThresholdEnabled;
			}
			set
			{
				_zealManaThresholdEnabled = value;
				OnPropertyChanged();
			}
		}

		private decimal _zeal_ManaThreshold = 0;
		public decimal Zeal_ManaThreshold
		{
			get
			{
				return _zeal_ManaThreshold;
			}
			set
			{
				_zeal_ManaThreshold = value;
				OnPropertyChanged();
			}
		}

		private Color _health_Color = Color.FromRgb(255, 0, 0); //Red

		public System.Windows.Media.Color Health_Color
		{
			get => _health_Color;
			set => _health_Color = value;
		}
		private System.Windows.Media.Color _mana_Color = Color.FromRgb(0, 0, 255); //Red
		public System.Windows.Media.Color Mana_Color
		{
			get => _mana_Color;
			set => _mana_Color = value;
		}
		private System.Windows.Media.Color _health_Mana_Color = Color.FromRgb(255, 0, 255); //Red
		public System.Windows.Media.Color Health_Mana_Color
		{
			get => _health_Mana_Color;
			set => _health_Mana_Color = value;
		}

		private bool _mana_ShowTop = false;
		public bool Mana_ShowTop
		{
			get
			{
				return _mana_ShowTop;
			}
			set
			{
				_mana_ShowTop = value;
				OnPropertyChanged();
			}
		}
		private bool _mana_ShowLeft = false;
		public bool Mana_ShowLeft
		{
			get
			{
				return _mana_ShowLeft;
			}
			set
			{
				_mana_ShowLeft = value;
				OnPropertyChanged();
			}
		}
		private bool _mana_ShowRight = false;
		public bool Mana_ShowRight
		{
			get
			{
				return _mana_ShowRight;
			}
			set
			{
				_mana_ShowRight = value;
				OnPropertyChanged();
			}
		}
		private bool _mana_ShowBottom = false;
		public bool Mana_ShowBottom
		{
			get
			{
				return _mana_ShowBottom;
			}
			set
			{
				_mana_ShowBottom = value;
				OnPropertyChanged();
			}
		}
		private bool _health_ShowTop = false;
		public bool Health_ShowTop
		{
			get
			{
				return _health_ShowTop;
			}
			set
			{
				_health_ShowTop = value;
				OnPropertyChanged();
			}
		}
		private bool _health_ShowLeft = false;
		public bool Health_ShowLeft
		{
			get
			{
				return _health_ShowLeft;
			}
			set
			{
				_health_ShowLeft = value;
				OnPropertyChanged();
			}
		}
		private bool _health_ShowRight = false;
		public bool Health_ShowRight
		{
			get
			{
				return _health_ShowRight;
			}
			set
			{
				_health_ShowRight = value;
				OnPropertyChanged();
			}
		}
		private bool _health_ShowBottom = false;
		public bool Health_ShowBottom
		{
			get
			{
				return _health_ShowBottom;
			}
			set
			{
				_health_ShowBottom = value;
				OnPropertyChanged();
			}
		}

		private decimal _staticOverlay_SizeTop = 150;
		public decimal StaticOverlay_SizeTop
		{
			get
			{
				return _staticOverlay_SizeTop;
			}
			set
			{
				_staticOverlay_SizeTop = value;
				OnPropertyChanged();
			}
		}
		private decimal _staticOverlay_SizeLeft = 150;
		public decimal StaticOverlay_SizeLeft
		{
			get
			{
				return _staticOverlay_SizeLeft;
			}
			set
			{
				_staticOverlay_SizeLeft = value;
				OnPropertyChanged();
			}
		}
		private decimal _staticOverlay_SizeRight = 150;
		public decimal StaticOverlay_SizeRight
		{
			get
			{
				return _staticOverlay_SizeRight;
			}
			set
			{
				_staticOverlay_SizeRight = value;
				OnPropertyChanged();
			}
		}
		private decimal _staticOverlay_SizeBottom = 150;
		public decimal StaticOverlay_SizeBottom
		{
			get
			{
				return _staticOverlay_SizeBottom;
			}
			set
			{
				_staticOverlay_SizeBottom = value;
				OnPropertyChanged();
			}
		}

		#endregion

		private bool _guildInstance_Force = false;
		public bool GuildInstance_Force
		{
			get
			{
				return _guildInstance_Force;
			}
			set
			{
				_guildInstance_Force = value;
				OnPropertyChanged();
			}
		}

		private string selectedCharacter;
		[JsonIgnore]
		public string SelectedCharacter
		{
			get
			{
				return selectedCharacter;
			}
			set
			{
				selectedCharacter = value;
				OnPropertyChanged();
			}
		}

		private ObservableCollectionRange<string> availableCharacters = new ObservableCollectionRange<string>();
		[JsonIgnore]
		public ObservableCollectionRange<string> AvailableCharacters
		{
			get
			{
				return availableCharacters;
			}
			set
			{
				availableCharacters = value;
				OnPropertyChanged();
			}
		}

		private bool lock_ImageOverlay_Position = false;
		public bool Lock_ImageOverlay_Position
		{
			get
			{
				return lock_ImageOverlay_Position;
			}
			set
			{
				lock_ImageOverlay_Position = value;
				(App.Current.Windows.Cast<Window>().FirstOrDefault(
					x => x is ImageOverlay) as ImageOverlay)?.SetWindowLockStatus(value);
				OnPropertyChanged();
			}
		}

		public bool UseZealForThis(int processId, bool specificProperty)
		{
			if (specificProperty)
			{
				if (ZealProcessID != 0 && ZealProcessID != processId)
				{
					return false;
				}
				return true;
			}
			return false;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}
