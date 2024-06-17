﻿namespace EQTool.Models
{
    public class SpellParsingMatch
    {
        public string TargetName { get; set; }

        public bool IsYou { get; set; }

        public Spell Spell { get; set; }

        public bool MultipleMatchesFound { get; set; }

        public int? TotalSecondsOverride { get; set; }

		public bool IsGuess { get; set; }
    }

}
