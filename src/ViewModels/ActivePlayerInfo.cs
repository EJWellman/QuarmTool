using EQTool.Models;
using EQToolShared.Enums;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace EQTool.ViewModels
{
    public class ActivePlayer : INotifyPropertyChanged
    {
        private readonly EQToolSettings settings;
        public ActivePlayer(EQToolSettings settings)
        {
            this.settings = settings;
        }

        public static PlayerInfo GetInfoFromString(string logFileName)
        {
            var charName_WithExt = logFileName.Replace("eqlog_", string.Empty);
            var indexPart = charName_WithExt.IndexOf("_");
            var charName = charName_WithExt.Substring(0, indexPart);

            var p = new PlayerInfo
            {
                Level = 1,
                Name = charName,
                PlayerClass = null,
                Zone = "freportw",
                ShowSpellsForClasses = Enum.GetValues(typeof(PlayerClasses)).Cast<PlayerClasses>().ToList()
            };

            indexPart = charName_WithExt.LastIndexOf("_");
            if (indexPart != -1)
            {
                var server = charName_WithExt.Substring(indexPart + 1).Replace(".txt", string.Empty);
                if (server == "P1999PVP")
                {
                    p.Server = Servers.Red;
                }
                else if (server == "P1999Green")
                {
                    p.Server = Servers.Green;
                }
                else
                {
                    p.Server = Servers.Blue;
                }
            }

            if (logFileName.IndexOf("pq.proj.txt") != -1)
            {
                p.Server = Servers.Quarm;
            }

            return p;
        }

		public string GetCharNameFromLogName(string logFileName)
		{
			var charName_WithExt = logFileName.Replace("eqlog_", string.Empty);
			var indexPart = charName_WithExt.IndexOf("_");
			var charName = charName_WithExt.Substring(0, indexPart);

			return charName;
		}

		private bool CheckAvailableCharactersForValueAndAdd(string newCharacter)
		{
			if (settings.AvailableCharacters.Contains(newCharacter))
			{
				return false;
			}
			else
			{
				settings.AvailableCharacters.Add(newCharacter);
				return true;
			}
		}

		public bool Update()
        {
            var playerChanged = false;
            try
            {
                var players = settings.Players ?? new System.Collections.Generic.List<PlayerInfo>();
                var directory = new DirectoryInfo(settings.EqLogDirectory);
                var loggedInCharLogFile = directory.GetFiles("eqlog*.txt", SearchOption.TopDirectoryOnly)
                    .OrderByDescending(a => a.LastWriteTime)
                    .FirstOrDefault();

				var files = directory.GetFiles("eqlog*.txt", SearchOption.TopDirectoryOnly)
					.OrderByDescending(a => a.LastWriteTimeUtc)
					.Select(f => f.Name).ToList();

				if(files.Count != settings.AvailableCharacters.Count)
				{
					files.ForEach(f => CheckAvailableCharactersForValueAndAdd(GetCharNameFromLogName(f)));
				}


				if (loggedInCharLogFile != null)
                {
                    var parseInfo = GetInfoFromString(loggedInCharLogFile.Name);
                    var tempPlayer = players.FirstOrDefault(a => a.Name == parseInfo.Name);

                    if (tempPlayer == null)
                    {
                        players.Add(parseInfo);
                    }
                    else
                    {
                        tempPlayer.Server = parseInfo.Server;
                    }

					if ((!string.IsNullOrWhiteSpace(settings.SelectedCharacter) && tempPlayer.Name == settings.SelectedCharacter)
						|| string.IsNullOrWhiteSpace(settings.SelectedCharacter))
					{
						Player = tempPlayer;
						LogFileName = loggedInCharLogFile.FullName;

						playerChanged = tempPlayer != Player;
					}
                }
                else
                {
                    Player = null;
                }
            }
            catch
            {

            }

            return playerChanged;
        }

        private Spell _UserCastingSpell;

        public Spell UserCastingSpell
        {
            get => _UserCastingSpell;
            set
            {
                _UserCastingSpell = value;
                OnPropertyChanged();
            }
        }

        public int UserCastingSpellCounter { get; set; }

        public string LogFileName;

        private PlayerInfo _Player;

        public PlayerInfo Player
        {
            get => _Player;
            set
            {
                _Player = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
