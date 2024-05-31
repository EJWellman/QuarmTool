using EQTool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQTool.EventArgModels
{
	public class TimerWindowEditEventArgs : EventArgs
	{
		public bool Success { get; set; }
		public TimerWindowOptions UpdatedWindow { get; set; }
	}
}
