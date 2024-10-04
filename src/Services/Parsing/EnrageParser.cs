using EQTool.ViewModels;

namespace EQTool.Services
{
    public class EnrageParser
    {
        public class EnrageEvent
        {
            public string NpcName { get; set; }
        }

        private readonly ActivePlayer activePlayer;

        public EnrageParser(ActivePlayer activePlayer)
        {
            this.activePlayer = activePlayer;
        }

        public EnrageEvent EnrageCheck(string line)
        {
            if (line.EndsWith(" has become ENRAGED.", System.StringComparison.OrdinalIgnoreCase))
            {
                var npcname = line.Replace(" has become ENRAGED.", string.Empty).Trim();
                return new EnrageEvent { NpcName = npcname };
            }

            return null;
		}

		public bool EnrageCheck(string line, out EnrageEvent data)
		{
			data = null;

			if (line.EndsWith(" has become ENRAGED.", System.StringComparison.OrdinalIgnoreCase))
			{
				var npcname = line.Replace(" has become ENRAGED.", string.Empty).Trim();
				data = new EnrageEvent { NpcName = npcname };
			}

			return data == null;
		}
	}
}
