using EQTool.Models;
using EQTool.Services;
using EQTool.ViewModels;
using EQToolShared.Enums;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Navigation;
using ZealPipes.Common.Models;
using ZealPipes.Services;
using static System.Net.Mime.MediaTypeNames;

namespace EQTool
{
    /// <summary>
    /// Interaction logic for MobInfo.xaml
    /// </summary>
    public partial class MobInfo : BaseSaveStateWindow
	{
		private readonly IAppDispatcher appDispatcher;
		private readonly LogParser logParser;
        private ViewModels.MobInfoViewModel mobInfoViewModel;
        private readonly QuarmDataService _quarmService;
		private readonly PlayerTrackerService playerTrackerService;
        private readonly ActivePlayer activePlayer;
		private ZealMessageService _zealMessageService;
		public MobInfo(ActivePlayer activePlayer, 
			QuarmDataService quarmService, 
			PlayerTrackerService playerTrackerService,
			LogParser logParser, 
			EQToolSettings settings, 
			EQToolSettingsLoad toolSettingsLoad,
			IAppDispatcher appDispatcher,
			ZealMessageService zealMessageService, 
			LoggingService loggingService) : base(settings.MobWindowState, toolSettingsLoad, settings)
        {
			this.appDispatcher = appDispatcher;
            loggingService.Log(string.Empty, EventType.OpenMobInfo, activePlayer?.Player?.Server);
            this.activePlayer = activePlayer;
			this.playerTrackerService = playerTrackerService;
            _quarmService = quarmService;
			_zealMessageService = zealMessageService;
            this.logParser = logParser;
            DataContext = mobInfoViewModel = new ViewModels.MobInfoViewModel();
            InitializeComponent();
            base.Init();
			_zealMessageService.OnCharacterUpdated += ZealMessageService_OnCharacterUpdated;
            this.logParser.ConEvent += LogParser_ConEvent;
        }

		private void ZealMessageService_OnCharacterUpdated(object sender, ZealCharacter.ZealCharacterUpdatedEventArgs e)
		{
			string mobName = e.Character.Detail.LabelData[(int)ZealPipes.Common.LabelType.TargetName - 1].Value;
			if (!string.IsNullOrWhiteSpace(mobName) && mobName != mobInfoViewModel.Name)
			{

				appDispatcher.DispatchUI(() =>
				{
					mobInfoViewModel.NewResults = _quarmService.GetData(mobName);
					FactionHitsStack.Visibility = mobInfoViewModel.HasFactionHits;
					QuestsStack.Visibility = mobInfoViewModel.HasQuests;
					KnownLootStack.Visibility = mobInfoViewModel.HasKnownLoot;
					MerchandiseStack.Visibility = mobInfoViewModel.HasMerchandise;
					SpecialAbilitiesStack.Visibility = mobInfoViewModel.HasSpecials;

					invis_rad.IsChecked = mobInfoViewModel.See_Invis;
					ivu_rad.IsChecked = mobInfoViewModel.See_Invis_Undead;
					sneak_rad.IsChecked = mobInfoViewModel.See_Sneak;
					ihide_rad.IsChecked = mobInfoViewModel.See_Imp_Hide;
				});

			}

			string blah = "";
		}

		private void LogParser_ConEvent(object sender, LogParser.ConEventArgs e)
        {
            try
            {
                if (e.Name != mobInfoViewModel.Name)
                {
					mobInfoViewModel.NewResults = _quarmService.GetData(e.Name);
					FactionHitsStack.Visibility = mobInfoViewModel.HasFactionHits;
					QuestsStack.Visibility = mobInfoViewModel.HasQuests;
					KnownLootStack.Visibility = mobInfoViewModel.HasKnownLoot;
					MerchandiseStack.Visibility = mobInfoViewModel.HasMerchandise;
					SpecialAbilitiesStack.Visibility = mobInfoViewModel.HasSpecials;

					invis_rad.IsChecked = mobInfoViewModel.See_Invis;
					ivu_rad.IsChecked = mobInfoViewModel.See_Invis_Undead;
					sneak_rad.IsChecked = mobInfoViewModel.See_Sneak;
					ihide_rad.IsChecked = mobInfoViewModel.See_Imp_Hide;

				}
            }
            catch (Exception ex)
            {
                mobInfoViewModel.ErrorResults = ex.Message;
                if (!mobInfoViewModel.ErrorResults.Contains("The underlying connection was closed:"))
                {
                    mobInfoViewModel.ErrorResults = "The server is down. Try again";
                    App.LogUnhandledException(ex, $"LogParser_ConEvent {e.Name}", activePlayer?.Player?.Server);
                }
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (logParser != null)
            {
                logParser.ConEvent -= LogParser_ConEvent;
            }
            base.OnClosing(e);
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            _ = Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void Hyperlink_RequestNavigatebutton(object sender, RoutedEventArgs args)
        {
            _ = Process.Start(new ProcessStartInfo(mobInfoViewModel.Url));
        }
    }
}
