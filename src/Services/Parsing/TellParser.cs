using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EQTool.Services
{
	public class TellMessage
	{
		public string Sender { get; set; }
		public string Message { get; set; }
	}
	public class TellParser
	{
		string tellPattern = @"((?:^|\]\s*)(\b\w+\b)\s+tells\s+you)";
		int timestampLength = 26; //[Fri Nov 01 22:14:54 2024] 

		public bool Parse(string message, out TellMessage match)
		{
			match = new TellMessage();
			// Match the tell pattern in the message
			var triggerMatches = Regex.Matches(message, tellPattern, RegexOptions.IgnoreCase);

			// If there are no matches, return null
			if (triggerMatches.Count == 0)
			{
				match = null;
				return false;
			}

			match.Sender = triggerMatches[0].Groups[1].Value.Replace("] ", "").Replace(" tells you", "");
			int splitIndex = message.IndexOf("tells you, '");
			match.Message = message.Substring(splitIndex + 12, message.Length - splitIndex - 13);
			return true;
		}
	}
}
