﻿using EQTool.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace EQTool.Services.Spells.Log
{
    public class DPSLogParse
    {
        private readonly string PointsOfDamage = " points of damage.";
        private readonly string PointOfDamage = " point of damage.";
        private readonly string WasHitByNonMelee = "was hit by non-melee";

        private readonly List<string> HitTypes = new List<string>()
        {
            " slice ",
            " slices ",
            " crush ",
            " crushes ",
            " bash ",
            " bashes ",
            " kick ",
            " kicks ",
            " hit ",
            " hits ",
            " punch ",
            " punches ",
            " pierce ",
            " pierces ",
            " backstab ",
            " backstabs ",
            " claw ",
            " claws "
        };

        public DPSLogParse()
        {
        }

        public DPSParseMatch Match(string linelog)
        {
            /// [Mon Nov 14 20:11:25 2022]
            var date = linelog.Substring(1, 24);
            var format = "ddd MMM dd HH:mm:ss yyyy";
            var timestamp = DateTime.Now;
            try
            {
                timestamp = DateTime.ParseExact(date, format, CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
            }

            var message = linelog.Substring(27);
            Debug.WriteLine($"DPSParse: " + message);
            if (message.Contains(WasHitByNonMelee))
            {
                return null;
            }

            if (message.EndsWith(PointsOfDamage) || message.EndsWith(PointOfDamage))
            {
                message = message.Replace(PointsOfDamage, string.Empty);
                message = message.Replace(PointOfDamage, string.Empty);
                var splits = message.Split(' ');
                var damagedone = splits[splits.Length - 1];
                var nameofattacker = string.Empty;
                foreach (var item in HitTypes)
                {
                    var found = message.IndexOf(item);
                    if (found != -1)
                    {
                        nameofattacker = message.Substring(0, found + 1).Trim();
                        break;
                    }
                }

                if (!string.IsNullOrWhiteSpace(nameofattacker))
                {
                    return new DPSParseMatch
                    {
                        SourceName = nameofattacker,
                        DamageDone = int.Parse(damagedone),
                        TimeStamp = timestamp,
                        TargetName = nameofattacker
                    };
                }
            }

            return null;
        }
    }
}