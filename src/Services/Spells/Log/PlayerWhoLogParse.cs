﻿using EQTool.Models;
using EQToolShared.Enums;
using System;
using System.Collections.Generic;

namespace EQTool.Services.Spells.Log
{
    public class PlayerWhoLogParse
    {
        private readonly Dictionary<PlayerClasses, List<string>> ClassMapping;
        public PlayerWhoLogParse()
        {
            ClassMapping = new Dictionary<PlayerClasses, List<string>>()
            {
                { PlayerClasses.Bard, new List<string>{ "Bard", "Minstrel","Troubadour","Virtuoso" } },
                { PlayerClasses.Cleric, new List<string>{ "Cleric", "Vicar", "Templar", "High Priest" } },
                { PlayerClasses.Druid, new List<string>{ "Druid", "Wanderer", "Preserver", "Hierophant" } },
                { PlayerClasses.Enchanter, new List<string>{ "Enchanter", "Illusionist", "Beguiler", "Phantasmist" } },
                { PlayerClasses.Magician, new List<string>{ "Magician", "Elementalist", "Conjurer", "Arch Mage" } },
                { PlayerClasses.Monk, new List<string>{ "Monk", "Disciple", "Master", "Grandmaster" } },
                { PlayerClasses.Necromancer, new List<string>{ "Necromancer", "Heretic", "Defiler", "Warlock" } },
                { PlayerClasses.Paladin, new List<string>{ "Paladin", "Cavalier", "Knight", "Crusader" } },
                { PlayerClasses.Ranger, new List<string>{ "Ranger", "Pathfinder", "Outrider", "Warder" } },
                { PlayerClasses.Rogue, new List<string>{ "Rogue", "Rake", "Blackguard", "Assassin" } },
                { PlayerClasses.ShadowKnight, new List<string>{ "Shadow Knight", "Reaver", "Revenant", "Grave Lord" } },
                { PlayerClasses.Shaman, new List<string>{ "Shaman", "Mystic", "Luminary", "Oracle" } },
                { PlayerClasses.Warrior, new List<string>{ "Warrior", "Champion", "Myrmidon", "Warlord" } },
                { PlayerClasses.Wizard, new List<string>{ "Wizard", "Channeler", "Evoker", "Sorcerer" } },
            };
        }

        public bool IsZoneWhoLine(string message)
        {
            return message == "Players on EverQuest:";
        }

        public EQToolShared.APIModels.PlayerControllerModels.Player ParsePlayerInfo(string message)
        {
            if (!message.StartsWith("AFK") && !message.StartsWith("["))
            {
                return null;
            }

            var begindex = message.IndexOf('[');
            if (begindex == -1)
            {
                return null;
            }
            message = message.Substring(begindex);
            var endindex = message.IndexOf("]");
            if (endindex == -1)
            {
                return null;
            }

            var spaceindex = message.IndexOf(" ");
            if (spaceindex == -1)
            {
                return null;
            }
			//Handles /anon & /role players
			var anonIndex = message.IndexOf("[ANONYMOUS]");
			if (anonIndex != -1)
			{
				return null;
			}


            var guess = new EQToolShared.APIModels.PlayerControllerModels.Player();
            if (spaceindex < endindex)
            {
                var levelsring = message.Substring(1, spaceindex - 1).Trim();
                if (int.TryParse(levelsring, out var levelguess))
                {
                    guess.Level = levelguess;
                }

                var classguess = message.Substring(spaceindex, endindex - spaceindex).Trim();
                foreach (var item in ClassMapping)
                {
                    if (item.Value.Contains(classguess))
                    {
                        guess.PlayerClass = item.Key;
                        break;
                    }
                }
            }

            message = message.Substring(endindex + 1).Trim();
            spaceindex = message.IndexOf(" ");
            guess.Name = spaceindex != -1 ? message.Substring(0, spaceindex).Trim() : message;

            var carrotindex = message.IndexOf('<');
            if (carrotindex != -1)
            {
                endindex = message.IndexOf('>');
                guess.GuildName = message.Substring(carrotindex, endindex - carrotindex).Trim('<', '>', ' ');
            }

            return guess;
        }

		internal string ParsePetOwner(string message, PlayerInfo player)
		{
			string myLeaderIsPattern = " says 'My leader is ";
			var index = message.IndexOf(myLeaderIsPattern);
			var playerIndex = message.IndexOf(player.Name);
			if (index == -1 || playerIndex == -1)
			{
				return null;
			}
			string petName = message.Substring(0, index);

			return petName;
		}
	}
}
