using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQTool.Constants
{
	public static class QuarmRules
	{
		private const int RespawnReductionDungeonLowerBoundMin = 360001; /// <summary> 5 Minutes </summary> 
		private const int RespawnReductionDungeonLowerBoundMax = 899000; /// <summary> 14:59 minutes </summary> 
		private const int RespawnReductionDungeonLowerBound = 360000; /// <summary> 6 minutes </summary>
		private const int RespawnReductionDungeonHigherBoundMin = 900000; /// <summary> 10 minutes </summary> 
		private const int RespawnReductionDungeonHigherBoundMax = 2400000; /// <summary> 40 minutes </summary> 
		private const int RespawnReductionDungeonHigherBound = 720000; /// <summary> 12 minutes </summary> 

		private const int RespawnReductionLowerBoundMin = 12001; /// <summary> 12 seconds </summary>
		private const int RespawnReductionLowerBoundMax = 60000; /// <summary> 1 minute </summary>
		private const int RespawnReductionLowerBound = 12000; /// <summary> 11.999 seconds </summary>
		private const int RespawnReductionHigherBoundMin = 60001; /// <summary> 1 minute 1 second </summary>
		private const int RespawnReductionHigherBoundMax = 300000; /// <summary> 5 minutes </summary>
		private const int RespawnReductionHigherBound = 60000; /// <summary> 1 minute </summary>

		public static double GetRespawnReductionDungeonLowerBoundMin() => RespawnReductionDungeonLowerBoundMin / 1000;
		public static double GetRespawnReductionDungeonLowerBoundMax() => RespawnReductionDungeonLowerBoundMax / 1000;
		public static double GetRespawnReductionDungeonLowerBound() => RespawnReductionDungeonLowerBound / 1000;
		public static double GetRespawnReductionDungeonHigherBoundMin() => RespawnReductionDungeonHigherBoundMin / 1000;
		public static double GetRespawnReductionDungeonHigherBoundMax() => RespawnReductionDungeonHigherBoundMax / 1000;
		public static double GetRespawnReductionDungeonHigherBound() => RespawnReductionDungeonHigherBound / 1000;
		public static double GetRespawnReductionLowerBoundMin() => RespawnReductionLowerBoundMin / 1000;
		public static double GetRespawnReductionLowerBoundMax() => RespawnReductionLowerBoundMax / 1000;
		public static double GetRespawnReductionLowerBound() => RespawnReductionLowerBound / 1000;
		public static double GetRespawnReductionHigherBoundMin() => RespawnReductionHigherBoundMin / 1000;
		public static double GetRespawnReductionHigherBoundMax() => RespawnReductionHigherBoundMax / 1000;
		public static double GetRespawnReductionHigherBound() => RespawnReductionHigherBound / 1000;
	}
}
