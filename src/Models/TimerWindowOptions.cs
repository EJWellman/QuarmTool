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
	[Table("TimerWindows")]
	public class TimerWindowOptions : INotifyPropertyChanged
	{
		private int id;

		public int ID
		{
			get
			{
				return id;
			}
			set
			{
				id = value;
				OnPropertyChanged();
			}
		}
		private string title;
		public string Title
		{
			get
			{
				return title;
			}
			set
			{
				title = value;
				OnPropertyChanged();
			}
		}
		private bool bestGuessSpells;
		public bool BestGuessSpells
		{
			get
			{
				return bestGuessSpells;
			}
			set
			{
				bestGuessSpells = value;
				OnPropertyChanged();
			}
		}
		private bool youOnlySpells;
		public bool YouOnlySpells
		{
			get
			{
				return youOnlySpells;
			}
			set
			{
				youOnlySpells = value;
				OnPropertyChanged();
			}
		}
		private bool showRandomRolls;
		public bool ShowRandomRolls
		{
			get
			{
				return showRandomRolls;
			}
			set
			{
				showRandomRolls = value;
				OnPropertyChanged();
			}
		}
		private bool showTimers;
		public bool ShowTimers
		{
			get
			{
				return showTimers;
			}
			set
			{
				showTimers = value;
				OnPropertyChanged();
			}
		}
		private bool showModRodTimers;
		public bool ShowModRodTimers
		{
			get
			{
				return showModRodTimers;
			}
			set
			{
				showModRodTimers = value;
				OnPropertyChanged();
			}
		}
		private bool showDeathTouches;
		public bool ShowDeathTouches
		{
			get
			{
				return showDeathTouches;
			}
			set
			{
				showDeathTouches = value;
				OnPropertyChanged();
			}
		}
		private bool showSpells;
		public bool ShowSpells
		{
			get
			{
				return showSpells;
			}
			set
			{
				showSpells = value;
				OnPropertyChanged();
			}
		}

		private bool showSimpleTimers;
		public bool ShowSimpleTimers
		{
			get
			{
				return showSimpleTimers;
			}
			set
			{
				showSimpleTimers = value;
				OnPropertyChanged();
			}
		}

		private bool showNPCs;
		public bool ShowNPCs
		{
			get
			{
				return showNPCs;
			}
			set
			{
				showNPCs = value;
				OnPropertyChanged();
			}
		}

		private bool showPCs;
		public bool ShowPCs
		{
			get
			{
				return showPCs;
			}
			set
			{
				showPCs = value;
				OnPropertyChanged();
			}
		}

		public string WindowRect { get; set; } //-1092,727,310,413 - XPos, YPos, Width, Height
		public int State { get; set; }
		public bool Closed { get; set; }


		private bool alwaysOnTop;
		public bool AlwaysOnTop
		{
			get
			{
				return alwaysOnTop;
			}
			set
			{
				alwaysOnTop = value;
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
