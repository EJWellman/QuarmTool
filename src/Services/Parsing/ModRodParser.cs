using EQTool.EventArgModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQTool.Services.Parsing
{

	public class ModRodParser
	{

		public ModRodUsageArgs Parse(string line)
		{
			var modulates = " modulates.";
			var youModulate = "You modulate.";

			if (!line.Contains(modulates) && !line.Contains(youModulate))
			{
				return null;
			}

			string[] modRodParts = line.Split(' ');
			return new ModRodUsageArgs
			{
				Name = modRodParts[0],
				Action = modRodParts[1]
			};
		}
	}
}
