using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
