using EQTool.Models;
using EQTool.Services;
using EQTool.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace EQTool
{
    public partial class ImageOverlay : BaseSaveStateWindow
    {
		private readonly PipeParser _pipeParser;
        private readonly EQToolSettings settings;
        private readonly ActivePlayer activePlayer;
        private readonly PigParseApi pigParseApi;
        private readonly IAppDispatcher appDispatcher;
		private readonly Models.WindowState _windowState;
		private BrushConverter converter = new BrushConverter();


		public ImageOverlay(PipeParser pipeParser, EQToolSettings settings, PigParseApi pigParseApi, EQToolSettingsLoad toolSettingsLoad, ActivePlayer activePlayer, IAppDispatcher appDispatcher)
            : base(settings.ImageOverlayWindowState, toolSettingsLoad, settings)
        {
			_windowState = settings.ImageOverlayWindowState;
			_pipeParser = pipeParser;
            this.pigParseApi = pigParseApi;
            this.appDispatcher = appDispatcher;
            this.activePlayer = activePlayer;
            this.settings = settings;

			InitializeComponent();
            base.Init();

			ManaGradient.Visibility = Visibility.Hidden;
			HealthGradient.Visibility = Visibility.Hidden;
			this.Topmost = true;
			_windowState.AlwaysOnTop = true;
			
			Mover.Visibility = settings.Lock_ImageOverlay_Position ? Visibility.Hidden : Visibility.Visible;

			this.IsHitTestVisible = !settings.Lock_ImageOverlay_Position;

			_pipeParser.HealthThresholdEvent += _pipeParser_HealthThresholdEvent;
			_pipeParser.ManaThresholdEvent += _pipeParser_ManaThresholdEvent;

			SetLockStatus(settings.Lock_ImageOverlay_Position);
        }

		public void SetLockStatus(bool isLocked)
		{
			Mover.Visibility = isLocked ? Visibility.Hidden : Visibility.Visible;
			if (isLocked)
			{
				this.WindowState = System.Windows.WindowState.Maximized;
				var hwnd = new WindowInteropHelper(this).Handle;
				WindowServices.SetWindowExTransparent(hwnd);
			}
			else
			{
				this.WindowState = System.Windows.WindowState.Normal;
				var hwnd = new WindowInteropHelper(this).Handle;
				WindowServices.SetWindowExNotTransparent(hwnd);
			}
			this.IsHitTestVisible = !isLocked;
		}

		private void _pipeParser_ManaThresholdEvent(object sender, PipeParser.ManaThresholdEventArgs e)
		{
			this.appDispatcher.DispatchUI(() =>
			{
				if(e.IsLow)
				{
					ManaGradient.Visibility = Visibility.Visible;
				}
				else
				{
					ManaGradient.Visibility = Visibility.Hidden;
				}
			});
		}

		private void _pipeParser_HealthThresholdEvent(object sender, PipeParser.HealthThresholdEventArgs e)
		{
			this.appDispatcher.DispatchUI(() =>
			{
				if (e.IsLow)
				{
					HealthGradient.Visibility = Visibility.Visible;
				}
				else
				{
					HealthGradient.Visibility = Visibility.Hidden;
				}
			});
		}

		protected override void OnClosing(CancelEventArgs e)
        {
            if (_pipeParser != null)
            {
				_pipeParser.HealthThresholdEvent -= _pipeParser_HealthThresholdEvent;
                _pipeParser.ManaThresholdEvent -= _pipeParser_ManaThresholdEvent;
            }

            base.OnClosing(e);
        }
    }
}
