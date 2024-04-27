using EQTool.Models;
using EQTool.Services.Parsing;
using EQToolShared.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows;
using System.Xml.Linq;

namespace EQTool.ViewModels
{
	public class TestUriViewModel : INotifyPropertyChanged
	{
		private string _Name = string.Empty;

		public string Name
		{
			get => _Name;
			set
			{
				_Name = !string.IsNullOrWhiteSpace(value) ? Regex.Replace(value, " {2,}", " ").Trim() : value;
				OnPropertyChanged();
			}
		}

		private int value;
		public int Value
		{
			get => value;
			set
			{
				this.value = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(HasUrl));
				OnPropertyChanged(nameof(HasNoUrl));

			}
		}

		public string DisplayText
		{
			get => Value > 0 ? $"+{Value} - {Name}" : $"{Value} - {Name}";
		}

		private string _Url = string.Empty;

		public string Url
		{
			get => _Url;
			set
			{
				_Url = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(HasUrl));
				OnPropertyChanged(nameof(HasNoUrl));
			}
		}

		public Visibility HasNoUrl => string.IsNullOrWhiteSpace(Url) ? Visibility.Visible : Visibility.Collapsed;

		public Visibility HasUrl => string.IsNullOrWhiteSpace(Url) ? Visibility.Collapsed : Visibility.Visible;

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
	public class FactionHitViewModel : INotifyPropertyChanged
	{
		private string _Name = string.Empty;

		public string Name
		{
			get => _Name;
			set
			{
				_Name = !string.IsNullOrWhiteSpace(value) ? Regex.Replace(value, " {2,}", " ").Trim() : value;
				OnPropertyChanged();
			}
		}

		private int value;
		public int Value
		{
			get => value;
			set
			{
				this.value = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(HasUrl));
				OnPropertyChanged(nameof(HasNoUrl));

			}
		}

		public string DisplayText
		{
			get => Value > 0 ? $"+{Value} - {Name}" : $"{Value} - {Name}";
		}

		private string _Url = string.Empty;

		public string Url
		{
			get => _Url;
			set
			{
				_Url = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(HasUrl));
				OnPropertyChanged(nameof(HasNoUrl));
			}
		}

		public Visibility HasNoUrl => string.IsNullOrWhiteSpace(Url) ? Visibility.Visible : Visibility.Collapsed;

		public Visibility HasUrl => string.IsNullOrWhiteSpace(Url) ? Visibility.Collapsed : Visibility.Visible;

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
	public class MobDropViewModel : INotifyPropertyChanged
	{
		private string _Name = string.Empty;

		public string Name
		{
			get => _Name;
			set
			{
				_Name = !string.IsNullOrWhiteSpace(value) ? Regex.Replace(value, " {2,}", " ").Trim() : value;
				OnPropertyChanged();
			}
		}

		private float value;
		public float Value
		{
			get => value;
			set
			{
				this.value = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(HasUrl));
				OnPropertyChanged(nameof(HasNoUrl));

			}
		}


		private string _Url = string.Empty;

		public string Url
		{
			get => _Url;
			set
			{
				_Url = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(HasUrl));
				OnPropertyChanged(nameof(HasNoUrl));
			}
		}

		public Visibility HasNoUrl => string.IsNullOrWhiteSpace(Url) ? Visibility.Visible : Visibility.Collapsed;

		public Visibility HasUrl => string.IsNullOrWhiteSpace(Url) ? Visibility.Collapsed : Visibility.Visible;

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}

	public class PricingUriViewModel : TestUriViewModel
	{
		private string _Price = string.Empty;

		public string Price
		{
			get => _Price;
			set
			{
				_Price = value;
				OnPropertyChanged();
			}
		}

		private string _PriceUrl = string.Empty;

		public string PriceUrl
		{
			get => _PriceUrl;
			set
			{
				_PriceUrl = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(HasePriceUrl));
				OnPropertyChanged(nameof(HasePriceUrlNoUrl));
			}
		}

		public Visibility HasePriceUrlNoUrl => string.IsNullOrWhiteSpace(PriceUrl) ? Visibility.Visible : Visibility.Collapsed;

		public Visibility HasePriceUrl => string.IsNullOrWhiteSpace(PriceUrl) ? Visibility.Collapsed : Visibility.Visible;
	}

	public class MobInfoViewModel : INotifyPropertyChanged
	{
		public string Title { get; set; } = "Mob Info v" + App.Version;
		private string _Results = string.Empty;

		public string Results
		{
			get => _Results;
			set
			{
				_Results = value;
				_ErrorResults = string.Empty;
				//Parse();
				OnPropertyChanged();
				OnPropertyChanged(nameof(HasErrors));
				OnPropertyChanged(nameof(HasNoErrors));
			}
		}

		private JsonMonster newResults;
		public JsonMonster NewResults
		{
			get => newResults;
			set
			{
				newResults = value;
				_ErrorResults = string.Empty;
				ConvertToViewModel(value);
				OnPropertyChanged();
				OnPropertyChanged(nameof(HasErrors));
				OnPropertyChanged(nameof(HasNoErrors));
			}
		}

		private string _ErrorResults = string.Empty;

		public string ErrorResults
		{
			get => _ErrorResults;
			set
			{
				_ErrorResults = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(HasErrors));
				OnPropertyChanged(nameof(HasNoErrors));
			}
		}

		public Visibility HasNoErrors => !string.IsNullOrWhiteSpace(_ErrorResults) ? Visibility.Collapsed : Visibility.Visible;
		public Visibility HasErrors => !string.IsNullOrWhiteSpace(_ErrorResults) ? Visibility.Visible : Visibility.Collapsed;

		private string _Url = string.Empty;

		public string Url
		{
			get => _Url;
			set
			{
				_Url = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(HasUrl));
			}
		}

		public Visibility HasUrl => string.IsNullOrWhiteSpace(Url) ? Visibility.Collapsed : Visibility.Visible;

		private string _ImageUrl = string.Empty;

		public string ImageUrl
		{
			get => _ImageUrl;
			set
			{
				_ImageUrl = value;
				OnPropertyChanged();
			}
		}

		private string _Name = string.Empty;

		public string Name
		{
			get => _Name;
			set
			{
				_Name = value?.Trim();
				OnPropertyChanged();
			}
		}

		private string _Race = string.Empty;

		public string Race
		{
			get => _Race;
			set
			{
				_Race = value;
				OnPropertyChanged();
			}
		}

		private string _Class = string.Empty;

		public string Class
		{
			get => _Class;
			set
			{
				_Class = value;
				OnPropertyChanged();
			}
		}

		private string _Level = string.Empty;

		public string Level
		{
			get => _Level;
			set
			{
				_Level = value;
				OnPropertyChanged();
			}
		}

		private string _AggroRadius = string.Empty;

		public string AggroRadius
		{
			get => _AggroRadius;
			set
			{
				_AggroRadius = value;
				OnPropertyChanged();
			}
		}

		private string _RunSpeed = string.Empty;

		public string RunSpeed
		{
			get => _RunSpeed;
			set
			{
				_RunSpeed = value;
				OnPropertyChanged();
			}
		}
		private string _AC = string.Empty;

		public string AC
		{
			get => _AC;
			set
			{
				_AC = value;
				OnPropertyChanged();
			}
		}
		private string _HP = string.Empty;

		public string HP
		{
			get => _HP;
			set
			{
				_HP = value;
				OnPropertyChanged();
			}
		}
		private string _HPRegen = string.Empty;

		public string HPRegen
		{
			get => _HPRegen;
			set
			{
				_HPRegen = value;
				OnPropertyChanged();
			}
		}

		private string _ManaRegen = string.Empty;

		public string ManaRegen
		{
			get => _ManaRegen;
			set
			{
				_ManaRegen = value;
				OnPropertyChanged();
			}
		}
		private string _AttacksPerRound = string.Empty;

		public string AttacksPerRound
		{
			get => _AttacksPerRound;
			set
			{
				_AttacksPerRound = value;
				OnPropertyChanged();
			}
		}
		private string _AttackSpeed = string.Empty;

		public string AttackSpeed
		{
			get => _AttackSpeed;
			set
			{
				_AttackSpeed = value;
				OnPropertyChanged();
			}
		}
		private string _DamagePerHit = string.Empty;

		public string DamagePerHit
		{
			get => _DamagePerHit;
			set
			{
				_DamagePerHit = value;
				OnPropertyChanged();
			}
		}

		private string primaryFaction = string.Empty;

		public string PrimaryFaction
		{
			get => primaryFaction;
			set
			{
				primaryFaction = value;
				OnPropertyChanged();
			}
		}

		private string resist_Poison = string.Empty;

		public string Resist_Poison
		{
			get => resist_Poison;
			set
			{
				resist_Poison = value;
				OnPropertyChanged();
			}
		}

		private string resist_Disease = string.Empty;

		public string Resist_Disease
		{
			get => resist_Disease;
			set
			{
				resist_Disease = value;
				OnPropertyChanged();
			}
		}

		private string resist_Magic = string.Empty;

		public string Resist_Magic
		{
			get => resist_Magic;
			set
			{
				resist_Magic = value;
				OnPropertyChanged();
			}
		}

		private string resist_Fire = string.Empty;

		public string Resist_Fire
		{
			get => resist_Fire;
			set
			{
				resist_Fire = value;
				OnPropertyChanged();
			}
		}

		private string resist_Cold = string.Empty;

		public string Resist_Cold
		{
			get => resist_Cold;
			set
			{
				resist_Cold = value;
				OnPropertyChanged();
			}
		}

		private bool see_Invis;
		public bool See_Invis
		{
			get => see_Invis;
			set
			{
				see_Invis = value;
				OnPropertyChanged();
			}
		}

		private bool see_Invis_Undead;
		public bool See_Invis_Undead
		{
			get => see_Invis_Undead;
			set
			{
				see_Invis_Undead = value;
				OnPropertyChanged();
			}
		}

		private bool see_Sneak;
		public bool See_Sneak
		{
			get => see_Sneak;
			set
			{
				see_Sneak = value;
				OnPropertyChanged();
			}
		}

		private bool see_Imp_Hide;
		public bool See_Imp_Hide
		{
			get => see_Imp_Hide;
			set
			{
				see_Imp_Hide = value;
				OnPropertyChanged();
			}
		}

		public Visibility HasSpecials
		{
			get => _Specials.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
		}

		private ObservableCollection<TestUriViewModel> _Specials = new ObservableCollection<TestUriViewModel>();

		public ObservableCollection<TestUriViewModel> Specials
		{
			get => _Specials;
			set
			{
				_Specials = value;
				OnPropertyChanged();
			}
		}

		public Visibility HasKnownLoot
		{
			get => _KnownLoot.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
		}

		private ObservableCollection<MobDropViewModel> _KnownLoot = new ObservableCollection<MobDropViewModel>();

		public ObservableCollection<MobDropViewModel> KnownLoot
		{
			get => _KnownLoot;
			set
			{
				_KnownLoot = value;
				OnPropertyChanged();
			}
		}

		public Visibility HasMerchandise {
			get => _MerchantItems.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
		}

		private ObservableCollection<MobDropViewModel> _MerchantItems = new ObservableCollection<MobDropViewModel>();

		public ObservableCollection<MobDropViewModel> MerchantItems
		{
			get => _MerchantItems;
			set
			{
				_MerchantItems = value;
				OnPropertyChanged();
			}
		}

		public Visibility HasFactionHits
		{
			get => _Factions.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
		}

		private ObservableCollection<FactionHitViewModel> _Factions = new ObservableCollection<FactionHitViewModel>();

		public ObservableCollection<FactionHitViewModel> Factions
		{
			get => _Factions;
			set
			{
				_Factions = value;
				OnPropertyChanged();
			}
		}

		public Visibility HasQuests
		{
			get => _RelatedQuests.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
		}

		private ObservableCollection<TestUriViewModel> _RelatedQuests = new ObservableCollection<TestUriViewModel>();

		public ObservableCollection<TestUriViewModel> RelatedQuests
		{
			get => _RelatedQuests;
			set
			{
				_RelatedQuests = value;
				OnPropertyChanged();
			}
		}

		public static string StripHTML(string input)
		{
			return Regex.Replace(input, "<.*?>", string.Empty);
		}

		/*
        private void Parse()
        {
            if (string.IsNullOrWhiteSpace(Results))
            {
                return;
            }
            var spec = Specials.ToList();
            foreach (var item in spec)
            {
                _ = Specials.Remove(item);
            }
            var knownLoot = KnownLoot.ToList();
            foreach (var item in knownLoot)
            {
                _ = KnownLoot.Remove(item);
            }
            spec = Factions.ToList();
            foreach (var item in spec)
            {
                _ = Factions.Remove(item);
            }
            spec = OpposingFactions.ToList();
            foreach (var item in spec)
            {
                _ = OpposingFactions.Remove(item);
            }
            spec = _RelatedQuests.ToList();
            foreach (var item in spec)
            {
                _ = _RelatedQuests.Remove(item);
            }
            var cleanresults = Results;
            var lastindexof = cleanresults.LastIndexOf("}}");
            if (lastindexof == -1)
            {
                return;
            }

            cleanresults = cleanresults.Substring(0, lastindexof);
            cleanresults = cleanresults.Replace("\r\n", "\n").Replace("|imagefilename", "^imagefilename").Replace("| ", "^");
            var splits = cleanresults.Split('^').Where(a => !string.IsNullOrWhiteSpace(a)).Select(a => a.Trim().TrimStart('\n')).ToList();
            Name = GetValue("name", splits);
            Race = GetValue("race", splits);
            Class = GetValue("class", splits)?.Replace("[[", string.Empty).Replace("]]", string.Empty);
            var lvl = GetValue("level", splits)?.Where(a => char.IsDigit(a) || a == ' ' || a == '-')?.ToArray();
            if (lvl != null)
            {
                Level = new string(lvl);
            }

            AggroRadius = GetValue("agro_radius", splits);
            RunSpeed = GetValue("run_speed", splits);
            AC = GetValue("AC", splits);
            HP = GetValue("HP", splits);
            HPRegen = GetValue("HP_regen", splits);
            ManaRegen = GetValue("mana_regen", splits);
            AttacksPerRound = GetValue("attacks_per_round", splits);
            AttackSpeed = GetValue("attack_speed", splits);
            DamagePerHit = GetValue("damage_per_hit", splits);
            if (DamagePerHit?.Contains('\n') == true)
            {
                DamagePerHit = DamagePerHit.Split('\n')[0];
            }

            var infos = MobInfoParsing.ParseSpecials(splits);
            foreach (var item in infos)
            {
                Specials.Add(item);
            }

            var knownloot = MobInfoParsing.ParseKnownLoot(splits);
            foreach (var item in knownLoot)
            {
                KnownLoot.Add(item);
            }

            infos = MobInfoParsing.ParseFactions(splits);
            foreach (var item in infos)
            {
                Factions.Add(item);
            }

            infos = MobInfoParsing.ParseOpposingFactions(splits);
            foreach (var item in infos)
            {
                OpposingFactions.Add(item);
            }

            infos = MobInfoParsing.ParseRelatedQuests(splits);
            foreach (var item in infos)
            {
                RelatedQuests.Add(item);
            }

            if (!string.IsNullOrWhiteSpace(Name))
            {
                var name = HttpUtility.UrlEncode(Name.Replace(' ', '_'));
                Url = $"https://wiki.project1999.com/{name}";
            }

            var imageurl = GetValue("imagefilename", splits);
            if (!string.IsNullOrWhiteSpace(imageurl) && imageurl.Length > 2)
            {
                var indexof = imageurl.IndexOf(".png");
                if (indexof != -1)
                {
                    imageurl = imageurl.Substring(0, indexof + 4);
                }

                indexof = imageurl.IndexOf(".jpg");
                if (indexof != -1)
                {
                    imageurl = imageurl.Substring(0, indexof + 4);
                }

                imageurl = char.ToUpper(imageurl[0]) + imageurl.Substring(1, imageurl.Length - 1);
                ImageUrl = $"https://wiki.project1999.com/images/{imageurl}";
            }
        }
		*/

		public void ConvertToViewModel(JsonMonster monster)
		{
			if (monster != null)
			{
				string pqdi_url = @"https://www.pqdi.cc/";

				Name = monster.Name;
				Race = monster.Race;
				Class = Enum.GetName(typeof(Classes), monster.NPC_Class_ID);
				Level = (monster.MaxLevel != 0 ? $"{monster.Level}-{monster.MaxLevel}" : $"{monster.Level}");
				RunSpeed = monster.RunSpeed.ToString();
				AC = monster.AC.ToString();
				HP = monster.HP.ToString();
				HPRegen = monster.Combat_HP_Regen.ToString();
				ManaRegen = monster.Combat_Mana_Regen.ToString();
				AttacksPerRound = monster.Attack_Count.ToString();
				AttackSpeed = monster.Attack_Delay.ToString();
				DamagePerHit = $"{monster.MinDmg}-{monster.MaxDmg}";
				PrimaryFaction = monster.Primary_Faction;
				Resist_Cold = monster.CR.ToString();
				Resist_Fire = monster.FR.ToString();
				Resist_Magic = monster.MR.ToString();
				Resist_Poison = monster.PR.ToString();
				Resist_Disease = monster.DR.ToString();
				See_Invis = monster.See_Invis == 1 ? true : false;
				See_Invis_Undead = monster.See_Invis_Undead == 1 ? true : false;
				See_Sneak = monster.See_Sneak == 1 ? true : false;
				See_Imp_Hide = monster.See_Improved_Hidee == 1 ? true : false;

				#region Clear existing Data
				if (Factions.Count > 0)
				{
					Factions.Clear();
				}
				if (KnownLoot.Count > 0)
				{
					KnownLoot.Clear();
				}
				if (Specials.Count > 0)
				{
					Specials.Clear();
				}
				if(MerchantItems.Count > 0)
				{
					MerchantItems.Clear();
				}
				#endregion

				foreach (JsonMonsterFaction faction in monster.Factions)
				{
					Factions.Add(new FactionHitViewModel
					{
						Name = faction.Faction_Name,
						Value = faction.Faction_Hit,
						Url = $"{pqdi_url}faction/{faction.Faction_ID}"
					});
				}
				foreach (JsonMonsterDrops drop in monster.Drops)
				{
					KnownLoot.Add(new MobDropViewModel
					{
						Name = drop.Item_Name,
						Value = (float)System.Math.Round(drop.Drop_Chance, 2),
						Url = $"{pqdi_url}item/{drop.Item_ID}"
					});
				}
				List<string> specials = monster.GetSpecialAttacks().Split(',').ToList();
				foreach (string special in specials)
				{
					Specials.Add(new TestUriViewModel
					{
						Name = special
					});
				}
				foreach (JsonMerchantItems item in monster.MerchantItems)
				{
					MerchantItems.Add(new MobDropViewModel
					{
						Name = item.Item_Name,
						Value = item.Slot,
						Url = $"{pqdi_url}item/{item.Item_ID}"
					});
				}

				Url = $"{pqdi_url}npc/{monster.ID}";

			}
		}

		private string GetValue(string propname, List<string> lines)
		{
			var ret = lines.FirstOrDefault(a => a.StartsWith(propname));
			if (string.IsNullOrWhiteSpace(ret))
			{
				return string.Empty;
			}
			var index = ret.IndexOf('=');
			return index != -1 ? ret.Substring(index + 1).Trim() : string.Empty;
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
		public void PokeMe()
		{
			OnPropertyChanged();
		}
	}
}
