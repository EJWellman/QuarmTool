using EQTool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQTool.EventArgModels
{
	public class CustomOverlayEditEventArgs : EventArgs
	{
		public bool Success { get; set; }
		public CustomOverlay UpdatedOverlay { get; set; }
	}
}
