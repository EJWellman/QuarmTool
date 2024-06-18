﻿using EQToolShared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EQToolShared.HubModels
{
    public class CustomTimer
    {
        public const string CustomerTime = " Custom Timer";
        public string Name { get; set; }
        public int Roll { get; set; } = -1;
        public int DurationInSeconds { get; set; }
		public int NegativeDurationToShow { get; set; }
        public string SpellNameIcon { get; set; } = "Feign Death";
        public SpellTypes SpellType { get; set; } = SpellTypes.Beneficial;
        public string TargetName { get; set; } = CustomerTime;
        public Dictionary<PlayerClasses, int> Classes { get; set; } = Enum.GetValues(typeof(PlayerClasses)).Cast<PlayerClasses>().Select(a => new { key = a, level = 1 }).ToDictionary(a => a.key, a => a.level);
		public DateTime ExecutionTime { get; set; } = DateTime.Now;
		public bool IsNPC { get; set; } = false;
    }

    public class SignalrCustomTimer : CustomTimer
    {
        public Servers Server { get; set; }
    }
}
