using Autofac;
using System.Collections.Generic;
using System.Linq;

namespace EQTool.Services
{
	public class TimerWindowService
	{
		public TimerWindowService()
		{
		}


		public static bool AddNewTimerWindow(Models.TimerWindowOptions timer)
		{
			long ret = DataService.Insert(timer);
			return ret > 0;
		}

		public static bool UpdateTimerWindow(Models.TimerWindowOptions timer)
		{
			return DataService.UpdateTimerWindow(timer);
		}

		public static bool DeleteTimerWindow(Models.TimerWindowOptions timer)
		{
			return DataService.Delete(timer);
		}

		public static List<Models.TimerWindowOptions> LoadTimerWindows()
		{
			var ret = DataService.GetData<Models.TimerWindowOptions>("SELECT * FROM TimerWindows").ToList();
			return ret;
		}

		public static Models.TimerWindowOptions LoadTimerWindow(int id_in)
		{
			var ret = DataService.GetData<Models.TimerWindowOptions>("SELECT * FROM TimerWindows WHERE ID = @id", new { id = id_in }).FirstOrDefault();
			return ret;
		}
	}
}