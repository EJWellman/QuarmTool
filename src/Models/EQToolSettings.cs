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

		#endregion

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
