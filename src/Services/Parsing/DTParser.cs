﻿using EQToolShared;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace EQTool.Services
{
    public class DTParser
	{
		public class DT_Event
        {
            public string NpcName { get; set; }
            public string DTReceiver { get; set; }
			public DateTime ExecutionTime { get; set; }
        }

        public DT_Event DtCheck(string line)
        {
			string mobShoutRegex = "^(\\w.+) shouts '([A-z]+)!'$";

			if(Regex.IsMatch(line, mobShoutRegex))
			{
				var match = Regex.Match(line, mobShoutRegex);
				if(match.Success)
				{
					string mobName = match.Groups[1].Value.Trim();
					string dtReceiver = match.Groups[2].Value;

					return new DT_Event { NpcName = mobName, DTReceiver = dtReceiver };
				}
			}
            return null;
        }

		public bool DtCheck(string line, out DT_Event event_data)
		{
			string mobShoutRegex = "^(\\w.+) shouts '([A-z]+)!'$";
			event_data = null;

			if (Regex.IsMatch(line, mobShoutRegex))
			{
				var match = Regex.Match(line, mobShoutRegex);
				if (match.Success)
				{
					string mobName = match.Groups[1].Value.Trim();
					string dtReceiver = match.Groups[2].Value;

					event_data = new DT_Event { NpcName = mobName, DTReceiver = dtReceiver };
				}
			}
			return event_data == null;
		}
	}
}