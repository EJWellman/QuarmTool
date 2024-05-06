using EQTool.Models;
using System;


namespace EQTool.EventArgModels
{
	public class ModRodUsageArgs : EventArgs
	{
		public string Name { get; set; }
		public string Action { get; set; }
	}
}
