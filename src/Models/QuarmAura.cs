using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.Core.Converters;

namespace EQTool.Models
{
	[Table("Auras")]
	public class QuarmAura : INotifyPropertyChanged
	{
		private int _id;
		public int ID
		{
			get
			{
				return _id;
			}
			set
			{
				_id = value;
				OnPropertyChanged();
			}
		}

		private ZealPipes.Common.LabelType? _zealLabel;
		public ZealPipes.Common.LabelType? ZealLabel
		{
			get
			{
				return _zealLabel;
			}
			set
			{
				if (value != null & _zealGauge != null)
				{
					_zealLabel = null;
				}
				_zealLabel = value;
				OnPropertyChanged();
			}
		}
		private ZealPipes.Common.GaugeType? _zealGauge;
		public ZealPipes.Common.GaugeType? ZealGauge
		{
			get
			{
				return _zealGauge;
			}
			set
			{
				if (value != null & _zealLabel != null)
				{
					_zealLabel = null;
				}
				_zealGauge = value;
				OnPropertyChanged();
			}
		}

		private bool _auraEnabled;
		public bool AuraEnabled
		{
			get
			{
				return _auraEnabled;
			}
			set
			{
				_auraEnabled = value;
				OnPropertyChanged();
			}
		}

		private string _name;
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
				OnPropertyChanged();
			}
		}

		private string _auraDimensions; //Width, Height
		public string AuraDimensions
		{
			get
			{
				return _auraDimensions;
			}
			set
			{
				_auraDimensions = value;
				OnPropertyChanged();
			}
		}
		private string _auraPosition; //XPos, YPos
		public string AuraPosition
		{
			get
			{
				return _auraPosition;
			}
			set
			{
				_auraPosition = value;
				OnPropertyChanged();
			}
		}

		private bool _showEdgeAuras;
		public bool ShowEdgeAuras
		{
			get
			{
				return _showEdgeAuras;
			}
			set
			{
				_showEdgeAuras = value;
				OnPropertyChanged();
			}
		}
		private double _edgeAuraSize;
		public double EdgeAuraSize
		{
			get
			{
				return _edgeAuraSize;
			}
			set
			{
				_edgeAuraSize = value;
				OnPropertyChanged();
			}
		}
		private Color _edgeAuraColor; //A, R, G, B
		public Color EdgeAuraColor
		{
			get
			{
				return _edgeAuraColor;
			}
			set
			{
				_edgeAuraColor = value;
				OnPropertyChanged();
			}
		}
		private bool _showTopEdgeAura;
		public bool ShowTopEdgeAura
		{
			get
			{
				return _showTopEdgeAura;
			}
			set
			{
				_showTopEdgeAura = value;
				OnPropertyChanged();
			}
		}
		private bool _showBottomEdgeAura;
		public bool ShowBottomEdgeAura
		{
			get
			{
				return _showBottomEdgeAura;
			}
			set
			{
				_showBottomEdgeAura = value;
				OnPropertyChanged();
			}
		}
		private bool _showLeftEdgeAura;
		public bool ShowLeftEdgeAura
		{
			get
			{
				return _showLeftEdgeAura;
			}
			set
			{
				_showLeftEdgeAura = value;
				OnPropertyChanged();
			}
		}
		private bool _showRightEdgeAura;
		public bool ShowRightEdgeAura
		{
			get
			{
				return _showRightEdgeAura;
			}
			set
			{
				_showRightEdgeAura = value;
				OnPropertyChanged();
			}
		}

		private bool _pulseEnabled;
		public bool PulseEnabled
		{
			get
			{
				return _pulseEnabled;
			}
			set
			{
				_pulseEnabled = value;
				OnPropertyChanged();
			}
		}

		private double _pulseSpeed;
		public double PulseSpeed
		{
			get
			{
				return _pulseSpeed;
			}
			set
			{
				_pulseSpeed = value;
				OnPropertyChanged();
			}
		}
		private double _pulseSize;
		public double PulseSize
		{
			get
			{
				return _pulseSize;
			}
			set
			{
				_pulseSize = value;
				OnPropertyChanged();
			}
		}
		private bool _fadeEnabled;
		public bool FadeEnabled
		{
			get
			{
				return _fadeEnabled;
			}
			set
			{
				_fadeEnabled = value;
				OnPropertyChanged();
			}
		}
		private double _fadeSpeed;
		public double FadeSpeed
		{
			get
			{
				return _fadeSpeed;
			}
			set
			{
				_fadeSpeed = value;
				OnPropertyChanged();
			}
		}
		public double _fadedOpacity;
		public double FadedOpacity
		{
			get
			{
				return _fadedOpacity;
			}
			set
			{
				_fadedOpacity = value;
				OnPropertyChanged();
			}
		}
		private bool _showText;
		public bool ShowText
		{
			get
			{
				return _showText;
			}
			set
			{
				_showText = value;
				OnPropertyChanged();
			}
		}
		private string _showntext;
		public string ShownText
		{
			get
			{
				return _showntext;
			}
			set
			{
				_showntext = value;
				OnPropertyChanged();
			}
		}
		private int _textSize;
		public int TextSize
		{
			get
			{
				return _textSize;
			}
			set
			{
				_textSize = value;
				OnPropertyChanged();
			}
		}
		private string _textColor; //R, G, B, A
		public string TextColor
		{
			get
			{
				return _textColor;
			}
			set
			{
				_textColor = value;
				OnPropertyChanged();
			}
		}
		private string _textPosition; //XPos, YPos
		public string TextPosition
		{
			get
			{
				return _textPosition;
			}
			set
			{
				_textPosition = value;
				OnPropertyChanged();
			}
		}
		private bool _hasTextTrigger;
		public bool HasTextTrigger
		{
			get
			{
				return _hasTextTrigger;
			}
			set
			{
				_hasTextTrigger = value;
				OnPropertyChanged();
			}
		}
		private string _textTrigger;
		public string TextTrigger
		{
			get
			{
				return _textTrigger;
			}
			set
			{
				_textTrigger = value;
				OnPropertyChanged();
			}
		}
		private bool _imageEnabled;
		public bool ImageEnabled
		{
			get
			{
				return _imageEnabled;
			}
			set
			{
				_imageEnabled = value;
				OnPropertyChanged();
			}
		}
		private string _imagePath;
		public string ImagePath
		{
			get
			{
				return _imagePath;
			}
			set
			{
				_imagePath = value;
				OnPropertyChanged();
			}
		}
		private double opacity;
		public double Opacity
		{
			get
			{
				return opacity;
			}
			set
			{
				opacity = value;
				OnPropertyChanged();
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}
