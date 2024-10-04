using EQTool.Models;
using EQTool.ViewModels;
using System.Linq;

namespace EQTool.Services.Spells.Log
{
    public class SpellLogParse
    {
        private readonly ActivePlayer _activePlayer;
        private readonly ParseHandleYouCasting _parseHandleYouCasting;
        private readonly ParseSpellGuess _parseSpellGuess;
        private readonly EQToolSettings _settings;
        private readonly Spell _healSpell;
		private readonly QuarmDataService _quarmDataService;

		public SpellLogParse(ParseSpellGuess parseSpellGuess, ParseHandleYouCasting parseHandleYouCasting, ActivePlayer activePlayer, 
			EQToolSettings settings, EQSpells spells, QuarmDataService quarmDataService)
        {
            _parseSpellGuess = parseSpellGuess;
            _settings = settings;
            _parseHandleYouCasting = parseHandleYouCasting;
            _activePlayer = activePlayer;
			_quarmDataService = quarmDataService;

            _healSpell = spells.AllSpells.FirstOrDefault(a => a.name == "Chloroplast") ?? spells.AllSpells.FirstOrDefault(a => a.name == "Regeneration");
        }

        public SpellParsingMatch MatchSpell(string message)
        {
            if (message == "You mend your wounds and heal some damage." || message == "You have failed to mend your wounds.")
            {
                var spellParsingMatch = new SpellParsingMatch
                {
                    MultipleMatchesFound = false,
                    Spell = new Spell
                    {
                        buffduration = _healSpell.buffduration,
                        buffdurationformula = _healSpell.buffduration,
                        casttime = _healSpell.casttime,
                        cast_on_other = _healSpell.cast_on_other,
                        cast_on_you = _healSpell.cast_on_you,
                        Classes = new System.Collections.Generic.Dictionary<EQToolShared.Enums.PlayerClasses, int>() { { EQToolShared.Enums.PlayerClasses.Monk, 1 } },
                        DescrNumber = _healSpell.DescrNumber,
                        id = _healSpell.id,
                        name = "Mend",
                        pvp_buffdurationformula = _healSpell.pvp_buffdurationformula,
                        Rect = _healSpell.Rect,
                        ResistCheck = _healSpell.ResistCheck,
                        resisttype = _healSpell.resisttype,
                        SpellIcon = _healSpell.SpellIcon,
                        SpellType = _healSpell.SpellType,
                        spell_fades = _healSpell.spell_fades,
                        spell_icon = _healSpell.spell_icon,
                        type = _healSpell.type
                    },
                    TargetName = EQSpells.SpaceYou,
                    TotalSecondsOverride = 6 * 60
                };
                return spellParsingMatch;
            }

            if (message.StartsWith(EQSpells.YouSpellisInterupted))
            {
                _activePlayer.UserCastingSpell = null;
                return null;
            }
            if (message.StartsWith(EQSpells.YouBeginCasting))
            {
                _parseHandleYouCasting.HandleYouBeginCastingSpellStart(message);
                return null;
            }
            else if (message.StartsWith(EQSpells.You))
            {
                if (_activePlayer?.UserCastingSpell != null)
                {
                    if (message == _activePlayer.UserCastingSpell.cast_on_you)
                    {
                        var spellParsingMatch = _parseHandleYouCasting.HandleYouBeginCastingSpellEnd(message);
						if(spellParsingMatch != null)
						{
							spellParsingMatch.IsTargetNPC = _quarmDataService.DoesMonsterExistInZone(spellParsingMatch.TargetName);
						}
                        return spellParsingMatch;
                    }
                    else if (!string.IsNullOrWhiteSpace(_activePlayer.UserCastingSpell.cast_on_other) && message.EndsWith(_activePlayer.UserCastingSpell.cast_on_other))
                    {
                        var spellParsingMatch = _parseHandleYouCasting.HandleYouBeginCastingSpellOtherEnd(message);
						if (spellParsingMatch != null)
						{
							spellParsingMatch.IsTargetNPC = _quarmDataService.DoesMonsterExistInZone(spellParsingMatch.TargetName);
						}
						return spellParsingMatch;
                    }
                }

                return _parseHandleYouCasting.HandleYouSpell(message);
            }

            if (message.StartsWith(EQSpells.Your))
            {
                if (_activePlayer?.UserCastingSpell != null)
                {
                    if (message == _activePlayer.UserCastingSpell.cast_on_you)
                    {
                        var spellParsingMatch = _parseHandleYouCasting.HandleYouBeginCastingSpellEnd(message);
						if (spellParsingMatch != null)
						{
							spellParsingMatch.IsTargetNPC = _quarmDataService.DoesMonsterExistInZone(spellParsingMatch.TargetName);
						}
						return spellParsingMatch;
                    }
                    else if (!string.IsNullOrWhiteSpace(_activePlayer.UserCastingSpell.cast_on_other) && message.EndsWith(_activePlayer.UserCastingSpell.cast_on_other))
                    {
                        var spellParsingMatch = _parseHandleYouCasting.HandleYouBeginCastingSpellOtherEnd(message);
						if (spellParsingMatch != null)
						{
							spellParsingMatch.IsTargetNPC = _quarmDataService.DoesMonsterExistInZone(spellParsingMatch.TargetName);
						}
						return spellParsingMatch;
                    }
                }

                var spell = _parseHandleYouCasting.HandleYourSpell(message);
                if (spell != null)
                {
                    return spell;
                }

                return _parseSpellGuess.HandleBestGuessSpell(message);
            }

            if (_activePlayer?.UserCastingSpell != null)
            {
                if (message == _activePlayer.UserCastingSpell.cast_on_you)
                {
                    var spellParsingMatch = _parseHandleYouCasting.HandleYouBeginCastingSpellEnd(message);
					if (spellParsingMatch != null)
					{
						spellParsingMatch.IsTargetNPC = _quarmDataService.DoesMonsterExistInZone(spellParsingMatch.TargetName);
					}
					return spellParsingMatch;
                }
                else if (!string.IsNullOrWhiteSpace(_activePlayer.UserCastingSpell.cast_on_other) && message.EndsWith(_activePlayer.UserCastingSpell.cast_on_other))
                {
                    var spellParsingMatch = _parseHandleYouCasting.HandleYouBeginCastingSpellOtherEnd(message);
					if (spellParsingMatch != null)
					{
						spellParsingMatch.IsTargetNPC = _quarmDataService.DoesMonsterExistInZone(spellParsingMatch.TargetName);
					}
					return spellParsingMatch;
                }
            }

            return _parseSpellGuess.HandleBestGuessSpell(message);
        }


        public bool MatchSpell(string message, out SpellParsingMatch match)
        {
            if (message == "You mend your wounds and heal some damage." || message == "You have failed to mend your wounds.")
            {
                match = new SpellParsingMatch
                {
                    MultipleMatchesFound = false,
                    Spell = new Spell
                    {
                        buffduration = _healSpell.buffduration,
                        buffdurationformula = _healSpell.buffduration,
                        casttime = _healSpell.casttime,
                        cast_on_other = _healSpell.cast_on_other,
                        cast_on_you = _healSpell.cast_on_you,
                        Classes = new System.Collections.Generic.Dictionary<EQToolShared.Enums.PlayerClasses, int>() { { EQToolShared.Enums.PlayerClasses.Monk, 1 } },
                        DescrNumber = _healSpell.DescrNumber,
                        id = _healSpell.id,
                        name = "Mend",
                        pvp_buffdurationformula = _healSpell.pvp_buffdurationformula,
                        Rect = _healSpell.Rect,
                        ResistCheck = _healSpell.ResistCheck,
                        resisttype = _healSpell.resisttype,
                        SpellIcon = _healSpell.SpellIcon,
                        SpellType = _healSpell.SpellType,
                        spell_fades = _healSpell.spell_fades,
                        spell_icon = _healSpell.spell_icon,
                        type = _healSpell.type
                    },
                    TargetName = EQSpells.SpaceYou,
                    TotalSecondsOverride = 6 * 60
                };
                return true;
            }

            if (message.StartsWith(EQSpells.YouSpellisInterupted))
            {
                _activePlayer.UserCastingSpell = null;
                match = null;
                return false;
            }
            if (message.StartsWith(EQSpells.YouBeginCasting))
            {
                _parseHandleYouCasting.HandleYouBeginCastingSpellStart(message);
                match = null;
                return false;
            }
            else if (message.StartsWith(EQSpells.You))
            {
                if (_activePlayer?.UserCastingSpell != null)
                {
                    if (message == _activePlayer.UserCastingSpell.cast_on_you)
                    {
                        match = _parseHandleYouCasting.HandleYouBeginCastingSpellEnd(message);
						if (match != null)
						{
							match.IsTargetNPC = _quarmDataService.DoesMonsterExistInZone(match.TargetName);
						}
						return true;
                    }
                    else if (!string.IsNullOrWhiteSpace(_activePlayer.UserCastingSpell.cast_on_other) && message.EndsWith(_activePlayer.UserCastingSpell.cast_on_other))
                    {
                        match = _parseHandleYouCasting.HandleYouBeginCastingSpellOtherEnd(message);
						if (match != null)
						{
							match.IsTargetNPC = _quarmDataService.DoesMonsterExistInZone(match.TargetName);
						}
						return true;
                    }
                }

                match = _parseHandleYouCasting.HandleYouSpell(message);
				if (match != null)
				{
					match.IsTargetNPC = _quarmDataService.DoesMonsterExistInZone(match.TargetName);
				}
				return match != null;
            }

            if (message.StartsWith(EQSpells.Your))
            {
                if (_activePlayer?.UserCastingSpell != null)
                {
                    if (message == _activePlayer.UserCastingSpell.cast_on_you)
                    {
                        match = _parseHandleYouCasting.HandleYouBeginCastingSpellEnd(message);
						if (match != null)
						{
							match.IsTargetNPC = _quarmDataService.DoesMonsterExistInZone(match.TargetName);
						}
						return true;
                    }
                    else if (!string.IsNullOrWhiteSpace(_activePlayer.UserCastingSpell.cast_on_other) && message.EndsWith(_activePlayer.UserCastingSpell.cast_on_other))
                    {
                        match = _parseHandleYouCasting.HandleYouBeginCastingSpellOtherEnd(message);
						if (match != null)
						{
							match.IsTargetNPC = _quarmDataService.DoesMonsterExistInZone(match.TargetName);
						}
						return true;
                    }
                }

                match = _parseHandleYouCasting.HandleYourSpell(message);
				if (match != null)
                {
					match.IsTargetNPC = _quarmDataService.DoesMonsterExistInZone(match.TargetName);
                    return true;
                }

                match = _parseSpellGuess.HandleBestGuessSpell(message);
				if (match != null)
				{
					match.IsTargetNPC = _quarmDataService.DoesMonsterExistInZone(match.TargetName);
				}
				return match != null;
            }

            if (_activePlayer?.UserCastingSpell != null)
            {
                if (message == _activePlayer.UserCastingSpell.cast_on_you)
                {
                    match = _parseHandleYouCasting.HandleYouBeginCastingSpellEnd(message);
					if (match != null)
					{
						match.IsTargetNPC = _quarmDataService.DoesMonsterExistInZone(match.TargetName);
					}
					return true;
                }
                else if (!string.IsNullOrWhiteSpace(_activePlayer.UserCastingSpell.cast_on_other) && message.EndsWith(_activePlayer.UserCastingSpell.cast_on_other))
                {
                    match = _parseHandleYouCasting.HandleYouBeginCastingSpellOtherEnd(message);
					if (match != null)
					{
						match.IsTargetNPC = _quarmDataService.DoesMonsterExistInZone(match.TargetName);
					}
					return true;
                }
            }

            match = _parseSpellGuess.HandleBestGuessSpell(message);
			if (match != null)
			{
				match.IsTargetNPC = _quarmDataService.DoesMonsterExistInZone(match.TargetName);
			}
			return match != null;
        }
    }
}
