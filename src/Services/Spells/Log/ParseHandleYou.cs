using EQTool.Models;
using EQTool.ViewModels;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace EQTool.Services.Spells.Log
{
    public class ParseHandleYouCasting
    {
        private readonly ActivePlayer _activePlayer;
        private readonly IAppDispatcher _appDispatcher;
        private readonly EQSpells _spells;

		public ParseHandleYouCasting(ActivePlayer activePlayer, IAppDispatcher appDispatcher, EQSpells spells)
        {
            this._activePlayer = activePlayer;
            this._appDispatcher = appDispatcher;
            this._spells = spells;
        }

        public void HandleYouBeginCastingSpellStart(string message)
        {
            var spellname = message.Substring(EQSpells.YouBeginCasting.Length - 1).Trim().TrimEnd('.');
            if (_spells.YouCastSpells.TryGetValue(spellname, out var foundspells))
            {
                var foundspell = SpellDurations.MatchClosestLevelToSpell(foundspells, _activePlayer.Player);
                Debug.WriteLine($"Self Casting Spell: {spellname} Delay: {foundspell.casttime}");
                _appDispatcher.DispatchUI(() =>
                {
                    if (_activePlayer.Player != null)
                    {
                        if (foundspell.Classes.Count == 1)
                        {
                            if (!_activePlayer.Player.PlayerClass.HasValue)
                            {
                                _activePlayer.Player.PlayerClass = foundspell.Classes.FirstOrDefault().Key;
                            }

                            if (_activePlayer.Player.Level < foundspell.Classes.FirstOrDefault().Value)
                            {
                                _activePlayer.Player.Level = foundspell.Classes.FirstOrDefault().Value;
                            }
                        }
                    }

                    _activePlayer.UserCastingSpell = foundspell;
                    if (_activePlayer.UserCastingSpell.casttime > 0)
                    {
                        _activePlayer.UserCastingSpellCounter++;
                        _ = Task.Delay(_activePlayer.UserCastingSpell.casttime * 4).ContinueWith(a =>
                        {
                            Debug.WriteLine($"Cleaning Spell");
                            _appDispatcher.DispatchUI(() =>
                            {
                                if (--_activePlayer.UserCastingSpellCounter <= 0)
                                {
                                    _activePlayer.UserCastingSpellCounter = 0;
                                    _activePlayer.UserCastingSpell = null;
                                }
                            });
                        });
                    }
                });
            }
        }

        public SpellParsingMatch HandleYouSpell(string message)
        {
            if (_spells.CastOnYouSpells.TryGetValue(message, out var foundspells))
            {
                var foundspell = SpellDurations.MatchClosestLevelToSpell(foundspells, _activePlayer.Player);
                Debug.WriteLine($"You Casting Spell: {message} Delay: {foundspell.casttime}");
                return new SpellParsingMatch
                {
                    Spell = foundspell,
                    TargetName = EQSpells.SpaceYou
                };
            }

            return null;
        }

        public SpellParsingMatch HandleYourSpell(string message)
        {
            if (_spells.CastOnYouSpells.TryGetValue(message, out var foundspells))
            {
                var foundspell = SpellDurations.MatchClosestLevelToSpell(foundspells, _activePlayer.Player);
                Debug.WriteLine($"Your Casting Spell: {message} Delay: {foundspell.casttime}");
                return new SpellParsingMatch
                {
                    Spell = foundspell,
                    TargetName = EQSpells.SpaceYou
                };
            }

            return null;
        }

        public SpellParsingMatch HandleYouBeginCastingSpellEnd(string message)
        {
            Debug.WriteLine($"Self Finished Spell: {message}");
            var spell = _activePlayer.UserCastingSpell;
            _appDispatcher.DispatchUI(() =>
            {
                _activePlayer.UserCastingSpell = null;
            });
            return new SpellParsingMatch
            {
                Spell = spell,
                TargetName = EQSpells.SpaceYou,
                IsYou = true
            };
        }

        public SpellParsingMatch HandleYouBeginCastingSpellOtherEnd(string message)
        {
            var targetname = message.Replace(_activePlayer.UserCastingSpell.cast_on_other, string.Empty).Trim();
            Debug.WriteLine($"Self Finished Spell: {message}");
            var spell = _activePlayer.UserCastingSpell;
            return new SpellParsingMatch
            {
                Spell = spell,
                TargetName = targetname,
                IsYou = true
            };
        }
    }
}
