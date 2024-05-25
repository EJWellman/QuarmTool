using Autofac;
using System.Collections.Generic;
using System.Linq;

namespace EQTool.Services
{
	public class TimerWindowService
	{
		private static DataService _dataService;

		public TimerWindowService(DataService dataService)
		{
			_dataService = dataService;
		}


		public static bool AddNewTimerWindow(Models.TimerWindowOptions timer)
		{
			long ret = _dataService.Insert(timer);
			return ret > 0;
		}

		public static bool UpdateTimerWindow(Models.TimerWindowOptions timer)
		{
			return _dataService.UpdateTimerWindow(timer);
		}

		public static bool DeleteTimerWindow(Models.TimerWindowOptions timer)
		{
			return _dataService.Delete(timer);
		}

		public static List<Models.TimerWindowOptions> LoadTimerWindows()
		{
			if(_dataService == null)
			{
				_dataService = App.Container.Resolve<DataService>();
			}
			var ret = _dataService.GetData<Models.TimerWindowOptions>("SELECT * FROM TimerWindows").ToList();
			return ret;
		}
	}
}
