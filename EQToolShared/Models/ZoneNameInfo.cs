using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQToolShared.Models
{
	public class ZoneNameInfo
	{
		public string LongName { get; set; }
		public string ShortName { get; set; }
		public string MapName 
		{ 
			get
			{
				if (string.IsNullOrWhiteSpace(_mapName))
				{
					return ShortName;
				}
				else
				{
					return _mapName;
				}
			}
			set
			{
				_mapName = value;
			}
		}
		public string EnterName
		{
			get
			{
				if (string.IsNullOrWhiteSpace(_enterName))
				{
					return LongName;
				}
				else
				{
					return _enterName;
				}
			}
			set
			{
				_enterName = value;
			}
		}
		public string WhoName
		{
			get
			{
				if(string.IsNullOrWhiteSpace(_whoName))
				{
					return LongName;
				}
				else
				{
					return _whoName;
				}
			}
			set
			{
				_whoName = value;
			}
		}

		private string _mapName;
		private string _enterName;
		private string _whoName;

		public ZoneNameInfo(string longName, string shortName)
		{
			LongName = longName;
			ShortName = shortName;
		}
		public ZoneNameInfo(string longName, string shortName, string whoName)
		{
			LongName = longName;
			ShortName = shortName;
			WhoName = whoName;
		}
		public ZoneNameInfo(string longName, string shortName, string whoName, string enterName)
		{
			LongName = longName;
			ShortName = shortName;
			WhoName = whoName;
			EnterName = enterName;
		}
		public ZoneNameInfo(string longName, string shortName, string whoName, string enterName, string mapName)
		{
			LongName = longName;
			ShortName = shortName;
			WhoName = whoName;
			EnterName = enterName;
			MapName = mapName;
		}
	}
}
