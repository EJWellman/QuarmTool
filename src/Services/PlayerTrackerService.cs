﻿using EQTool.ViewModels;
using EQToolShared.Enums;
using EQToolShared.Map;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace EQTool.Services
{
    public class PlayerTrackerService
    {
        private readonly LogParser logParser;
        private readonly PigParseApi pigParseApi;
        internal readonly ActivePlayer activePlayer;
        private readonly LoggingService loggingService;
        private readonly PlayerGroupService playerGroupService;
		private QuarmDataService _quarmDataService;
        private readonly Dictionary<string, EQToolShared.APIModels.PlayerControllerModels.Player> Player = new Dictionary<string, EQToolShared.APIModels.PlayerControllerModels.Player>(StringComparer.InvariantCultureIgnoreCase);
        private readonly Dictionary<string, EQToolShared.APIModels.PlayerControllerModels.Player> PlayersInZones = new Dictionary<string, EQToolShared.APIModels.PlayerControllerModels.Player>(StringComparer.InvariantCultureIgnoreCase);
        private readonly Dictionary<string, EQToolShared.APIModels.PlayerControllerModels.Player> DirtyPlayers = new Dictionary<string, EQToolShared.APIModels.PlayerControllerModels.Player>(StringComparer.InvariantCultureIgnoreCase);
        private string CurrentZone;
        private readonly System.Timers.Timer UITimer;
        private readonly object ContainerLock = new object();

        public PlayerTrackerService(LogParser logParser, ActivePlayer activePlayer, PigParseApi pigParseApi, LoggingService loggingService, PlayerGroupService playerGroupService, QuarmDataService quarmDataService)
        {
            _ = activePlayer.Update();
            CurrentZone = activePlayer.Player?.Zone;
            this.logParser = logParser;
            this.playerGroupService = playerGroupService;
            this.logParser.PlayerZonedEvent += LogParser_PlayerZonedEvent;
            this.logParser.WhoEvent += LogParser_WhoEvent;
            this.logParser.WhoPlayerEvent += LogParser_WhoPlayerEvent;
            UITimer = new System.Timers.Timer(1000);
            UITimer.Elapsed += UITimer_Elapsed; ;
            UITimer.Enabled = true;
            this.pigParseApi = pigParseApi;
            this.activePlayer = activePlayer;
            this.loggingService = loggingService;
			_quarmDataService = quarmDataService;
        }

        public bool IsPlayer(string name)
        {
            if (name == "You")
            {
                return true;
            }

            lock (ContainerLock)
            {
                return Player.ContainsKey(name);
            }
        }

        private void UITimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var playerstosync = new List<EQToolShared.APIModels.PlayerControllerModels.Player>();
            if (activePlayer.Player?.Server == null)
            {
                return;
            }

            lock (ContainerLock)
            {
                playerstosync = DirtyPlayers.Values.ToList();
                DirtyPlayers.Clear();
            }
            try
            {
                pigParseApi.SendPlayerData(playerstosync, activePlayer.Player.Server.Value);
            }
            catch (Exception ex)
            {
                loggingService.Log(ex.ToString(), EventType.Error, activePlayer?.Player?.Server);
            }
        }

        private void LogParser_WhoEvent(object sender, LogParser.WhoEventArgs e)
        {
            lock (ContainerLock)
            {
                if (CurrentZone != activePlayer.Player?.Zone
					&& CurrentZone != ZoneParser.ZoneNameMapper[activePlayer.Player?.LastZoneEntered])
                {
                    CurrentZone = activePlayer.Player?.Zone;
					_quarmDataService.LoadMobDataForZone(CurrentZone);
					Debug.WriteLine("Clearing zone Players");
                    PlayersInZones.Clear();
                }
                else
				{
					Debug.WriteLine("NOT Clearing zone Players");
                }
            }
        }

        private void LogParser_WhoPlayerEvent(object sender, LogParser.WhoPlayerEventArgs e)
        {
			if(activePlayer.Player != null)
			{
				if(activePlayer.Player.Name == e.PlayerInfo.Name
					&& !string.IsNullOrWhiteSpace(e.PlayerInfo.GuildName)
					&& activePlayer.Player.GuildName != e.PlayerInfo.GuildName)
				{
					activePlayer.Player.GuildName = e.PlayerInfo.GuildName;
				}
				if(activePlayer.Player.Name == e.PlayerInfo.Name
					&& activePlayer.Player.PlayerClass != e.PlayerInfo.PlayerClass)
				{
					activePlayer.Player.PlayerClass = e.PlayerInfo.PlayerClass;
				}
				if(activePlayer.Player.Name == e.PlayerInfo.Name
					&& activePlayer.Player.Level != e.PlayerInfo.Level)
				{
					activePlayer.Player.Level = e.PlayerInfo.Level ?? 0;
				}
			}

            lock (ContainerLock)
            {
                if (Player.TryGetValue(e.PlayerInfo.Name, out var playerinfo))
                {
                    if (
                        (playerinfo.Level != e.PlayerInfo.Level && e.PlayerInfo.Level.HasValue) ||
                        (playerinfo.GuildName != e.PlayerInfo.GuildName && !string.IsNullOrWhiteSpace(e.PlayerInfo.GuildName)) ||
                        (playerinfo.PlayerClass != e.PlayerInfo.PlayerClass && e.PlayerInfo.PlayerClass.HasValue)
                        )
                    {
                        if (!DirtyPlayers.ContainsKey(e.PlayerInfo.Name))
                        {
                            Debug.WriteLine($"DirtyPlayer Add {e.PlayerInfo.Name}");
                            DirtyPlayers[e.PlayerInfo.Name] = e.PlayerInfo;
                        }
                    }
                    playerinfo.Level = playerinfo.Level ?? e.PlayerInfo.Level;
                    playerinfo.GuildName = playerinfo.GuildName ?? e.PlayerInfo.GuildName;
                    playerinfo.PlayerClass = playerinfo.PlayerClass ?? e.PlayerInfo.PlayerClass;
                    Debug.WriteLine($"Updating {playerinfo.Name} {playerinfo.Level} {playerinfo.GuildName} {playerinfo.PlayerClass}");
                }
                else
                {
                    Player.Add(e.PlayerInfo.Name, e.PlayerInfo);
                    if (!DirtyPlayers.ContainsKey(e.PlayerInfo.Name))
                    {
                        DirtyPlayers[e.PlayerInfo.Name] = e.PlayerInfo;
                        Debug.WriteLine($"DirtyPlayer Add {e.PlayerInfo.Name}");
                    }
                    Debug.WriteLine($"Adding {e.PlayerInfo.Name} {e.PlayerInfo.Level} {e.PlayerInfo.GuildName} {e.PlayerInfo.PlayerClass}");
                }

                if (!PlayersInZones.ContainsKey(e.PlayerInfo.Name))
                {
                    PlayersInZones[e.PlayerInfo.Name] = e.PlayerInfo;
                }
            }

        }

        private void LogParser_PlayerZonedEvent(object sender, LogParser.PlayerZonedEventArgs e)
        {
            lock (ContainerLock)
            {
				_quarmDataService.LoadMobDataForZone(activePlayer.Player?.Zone/*, activePlayer.Player?.LastZoneEntered*/);
				Debug.WriteLine("Clearing zone Players");
                PlayersInZones.Clear();
            }
        }

        public List<Group> CreateGroups(GroupOptimization groupOptimization)
        {
            if (string.IsNullOrWhiteSpace(activePlayer.Player?.GuildName) || activePlayer.Player.Server == null)
            {
                return new List<Group>();
            }

            var players = new List<EQToolShared.APIModels.PlayerControllerModels.Player>();
            lock (ContainerLock)
            {
                players = PlayersInZones.Values.ToList();
            }

            var uknownplayerdata = players.Where(a => !a.PlayerClass.HasValue || !a.Level.HasValue).Select(a => a.Name).ToList();
            var playerdatafromserver = pigParseApi.GetPlayerData(uknownplayerdata, activePlayer.Player.Server.Value);
            foreach (var item in playerdatafromserver)
            {
                var playerlocally = players.FirstOrDefault(a => a.Name == item.Name);
                if (playerlocally != null)
                {
                    playerlocally.Level = playerlocally.Level ?? item.Level;
                    playerlocally.PlayerClass = playerlocally.PlayerClass ?? item.PlayerClass;
                }
            }

            players = players.Where(a => a.GuildName == activePlayer.Player.GuildName).ToList();
            switch (groupOptimization)
            {
                case GroupOptimization.HOT_Cleric_SparseGroup:
                    return playerGroupService.CreateHOT_Clerics_SparseGroups(players);
                case GroupOptimization.HOT_Cleric_SameGroup:
                    return playerGroupService.CreateHOT_Clerics_SameGroups(players);
                case GroupOptimization.Standard:
                    return playerGroupService.CreateStandardGroups(players);
                default:
                    return new List<Group>();
            }
        }

    }
}
