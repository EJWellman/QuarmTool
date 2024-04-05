﻿using EQTool.Models;
using EQTool.Tools;
using EQTool.ViewModels;
using EQToolShared.Map;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;

namespace EQTool.Services
{
    public class JsonDataService
    {
        private readonly ActivePlayer activePlayer;
		private static DataTable monsterTable;
        public JsonDataService(ActivePlayer activePlayer)
        {
            this.activePlayer = activePlayer;
        }

		public bool LoadMonsterDataTable(string zoneName)
		{
			//REPLACE THIS WITH CONFIG OPTION
			string filename = "B:\\Hobby Stuff\\QuarmTool Data\\json\\Initial NPC set.json";
			FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);

			string jsonContents;
			using (StreamReader sr = new StreamReader(fs))
			{
				jsonContents = sr.ReadToEnd();
			}

			List<JsonMonster> monsterList = JsonConvert.DeserializeObject<List<JsonMonster>>(jsonContents);
			DataTable tempMonsterTable = monsterList.ToDataTable();
			string columnFilter = "zone_code";

			monsterTable = tempMonsterTable.AsEnumerable().Where(r =>
				r.Field<string>(columnFilter) == zoneName
			).CopyToDataTable();

			if(monsterTable.Rows.Count > 0 )
			{
				return true;
			}

			return false;
		}

        public class DisambigData
        {
            public string Name { get; set; }
            public string ZonePart { get; set; }
        }

        public static string Disambig(string response, string zone)
        {
            var splits = response.Split('\n').Where(a => !string.IsNullOrWhiteSpace(a)).Select(a => a.Trim()).Where(a => !string.IsNullOrWhiteSpace(a)).Where(a => a.Contains("Disambig")).ToList();
            var data = new List<DisambigData>();
            foreach (var item in splits)
            {
                var r = new DisambigData();
                var indexof = item.IndexOf("[[");
                if (indexof == -1)
                {
                    continue;
                }
                var name = item.Substring(indexof + 2);
                indexof = name.IndexOf("]]");
                if (indexof == -1)
                {
                    continue;
                }
                name = name.Substring(0, indexof);
                r.Name = name?.Trim();
                r.ZonePart = r.Name;
                data.Add(r);
                indexof = name.IndexOf("(");
                if (indexof == -1)
                {
                    continue;
                }
                var zonefromdata = name.Substring(indexof + 1);
                indexof = zonefromdata.IndexOf(")");
                if (indexof == -1)
                {
                    continue;
                }
                name = zonefromdata.Substring(0, indexof);
                r.ZonePart = name;
            }
            zone = zone?.ToLower() ?? string.Empty;
            var zonnamemapper = EQToolShared.Map.ZoneParser.ZoneNameMapper.FirstOrDefault(a => a.Value == zone);

            foreach (var item in data)
            {
                if (item.ZonePart.Contains(zone))
                {
                    return item.Name;
                }
                else if (!string.IsNullOrWhiteSpace(zonnamemapper.Key) && item.ZonePart.ToLower().Contains(zonnamemapper.Key))
                {
                    return item.Name;
                }
            }

            data = data.Where(a => !ZoneParser.Zones.Any(b => a.ZonePart.ToLower().Contains(b))).ToList();
            data = data.Where(a => !ZoneParser.ZoneWhoMapper.Keys.Any(b => a.ZonePart.ToLower().Contains(b.ToLower()))).ToList();
            data = data.Where(a => !ZoneParser.ZoneNameMapper.Keys.Any(b => a.ZonePart.ToLower().Contains(b.ToLower()))).ToList();
            return data.FirstOrDefault()?.Name;
        }

        public JsonMonster GetData(string name)
        {
            var currentzone = activePlayer?.Player?.Zone;
            if (name == "Snitch" && currentzone == "mischiefplane")
            {
                name = "Snitch_(PoM)";
            }
            else if (name == "Cazic Thule")
            {
                name = "Cazic_Thule_(God)";
            }
            else if (name == "Innoruuk")
            {
                name = "Innoruuk_(God)";
            }
            else if (name == "Bristlebane")
            {
                name = "Bristlebane_(God)";
            }

            try
            {
                name = HttpUtility.UrlEncode(name.Trim().Replace(' ', '_'));

				DataRow matchedMonster = monsterTable.AsEnumerable().FirstOrDefault(r =>
					r.Field<string>("name") == name
				);

				if (matchedMonster != null)
				{
					JsonMonster match = DataTableConverter.CreateItemFromRow<JsonMonster>(matchedMonster);
					match.name = match.name.Trim().Replace('_', ' ');
					return match;
				}

            }
            catch (System.AggregateException er)
            {
                if (er.InnerException != null && er.InnerException.GetType() == typeof(HttpRequestException))
                {
                    var err = er.InnerException as HttpRequestException;
                    if (err.InnerException?.GetType() == typeof(WebException))
                    {
                        var innererr = err.InnerException as WebException;
                        throw new System.Exception(innererr.Message);
                    }
                    else
                    {
                        throw new System.Exception(err.Message);
                    }
                }
            }
            catch (Exception e)
            {
                var msg = $"Zone: {name} ";
                throw new System.Exception(msg + e.Message);
            }

            return null;
        }
    }
}
