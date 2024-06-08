using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQToolShared.ExtendedClasses
{
	public static class StringExtensions
	{
		public static string CleanUpZealName(this string name, bool stripNumbers = false)
		{
			if (name.Length < 3 || !int.TryParse(name.Substring(name.Length - 3, 3), out _))
			{
				return name;
			}
			string mobNum = name.Substring(name.Length - 3, 3).Replace("_", " ");
			if (stripNumbers)
			{
				return name.Substring(0, name.Length - 3).Replace("_", " ");
			}
			else
			{
				return name.Substring(0, name.Length - 3).Replace("_", " ") + " " + mobNum;
			}
		}
	}
}
