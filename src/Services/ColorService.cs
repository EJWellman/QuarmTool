using EQTool.Models;
using EQToolShared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace EQTool.Services
{
	public class ColorService
	{
		EQToolSettings _settings;

		public ColorService(EQToolSettings settings)
		{
			_settings = settings;
		}

		public SolidColorBrush GetColorFromSpellType(SpellTypes spellType)
		{
			if (spellType == SpellTypes.Beneficial)
			{
				return new SolidColorBrush(_settings.BeneficialSpellTimerColor);
			}
			else if (spellType == SpellTypes.Detrimental
				|| spellType == SpellTypes.BadGuyCoolDown)
			{
				return new SolidColorBrush(_settings.DetrimentalSpellTimerColor);
			}
			else if (spellType == SpellTypes.RespawnTimer)
			{
				return new SolidColorBrush(_settings.RespawnTimerColor);
			}
			else if (spellType == SpellTypes.DisciplineCoolDown)
			{
				return new SolidColorBrush(_settings.DisciplineTimerColor);
			}
			else if (spellType == SpellTypes.ModRod)
			{
				return new SolidColorBrush(_settings.ModRodTimerColor);
			}
			else
			{
				return new SolidColorBrush(_settings.OtherTimerColor);
			}
		}
	}
}