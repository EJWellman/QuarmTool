using EQTool.Models;
using EQTool.Services;
using EQTool.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

namespace EQTool
{
    public partial class ImageOverlay : BaseSaveStateWindow
    {
		private readonly PipeParser _pipeParser;
        private readonly EQToolSettings _settings;
        private readonly ActivePlayer activePlayer;
        private readonly PigParseApi pigParseApi;
        private readonly IAppDispatcher appDispatcher;
		private readonly Models.WindowState _windowState;
		private BrushConverter converter = new BrushConverter();

		private bool _healthLow = false;
		private bool _manaLow = false;

		private LinearGradientBrush _healthGradient = new LinearGradientBrush();
		private LinearGradientBrush _manaGradient = new LinearGradientBrush();
		private LinearGradientBrush _bothGradient = new LinearGradientBrush();


		public ImageOverlay(PipeParser pipeParser, EQToolSettings settings, PigParseApi pigParseApi, EQToolSettingsLoad toolSettingsLoad, ActivePlayer activePlayer, IAppDispatcher appDispatcher)
            : base(settings.ImageOverlayWindowState, toolSettingsLoad, settings)
		{
			_windowState = settings.ImageOverlayWindowState;
			_pipeParser = pipeParser;
			this.pigParseApi = pigParseApi;
			this.appDispatcher = appDispatcher;
			this.activePlayer = activePlayer;
			this._settings = settings;

			InitializeComponent();
			base.Init();

			Gradient_Top.Visibility = Visibility.Hidden;
			Gradient_Left.Visibility = Visibility.Hidden;
			Gradient_Right.Visibility = Visibility.Hidden;
			Gradient_Bottom.Visibility = Visibility.Hidden;
			Gradient_Top.IsHitTestVisible = false;
			Gradient_Left.IsHitTestVisible = false;
			Gradient_Right.IsHitTestVisible = false;
			Gradient_Bottom.IsHitTestVisible = false;
			this.Topmost = true;
			_windowState.AlwaysOnTop = true;

			Mover.Visibility = settings.Lock_ImageOverlay_Position ? Visibility.Hidden : Visibility.Visible;

			this.IsHitTestVisible = !settings.Lock_ImageOverlay_Position;

			_pipeParser.HealthThresholdEvent += _pipeParser_HealthThresholdEvent;
			_pipeParser.ManaThresholdEvent += _pipeParser_ManaThresholdEvent;

			UpdateGradientColors();
		}

		public void UpdateGradientColors()
		{
			if(_healthGradient.GradientStops.Count > 0)
			{
				_healthGradient.GradientStops.Clear();
			}
			_healthGradient.GradientStops.Add(new GradientStop(Color.FromArgb(90, _settings.Health_Color.R, _settings.Health_Color.G, _settings.Health_Color.B), 0));
			_healthGradient.GradientStops.Add(new GradientStop(Color.FromArgb(70, _settings.Health_Color.R, _settings.Health_Color.G, _settings.Health_Color.B), 0.05));
			_healthGradient.GradientStops.Add(new GradientStop(Color.FromArgb(60, _settings.Health_Color.R, _settings.Health_Color.G, _settings.Health_Color.B), 0.1));
			_healthGradient.GradientStops.Add(new GradientStop(Color.FromArgb(35, _settings.Health_Color.R, _settings.Health_Color.G, _settings.Health_Color.B), 0.25));
			_healthGradient.GradientStops.Add(new GradientStop(Color.FromArgb(00, _settings.Health_Color.R, _settings.Health_Color.G, _settings.Health_Color.B), 1.0));

			if (_manaGradient.GradientStops.Count > 0)
			{
				_manaGradient.GradientStops.Clear();
			}
			_manaGradient.GradientStops.Add(new GradientStop(Color.FromArgb(90, _settings.Mana_Color.R, _settings.Mana_Color.G, _settings.Mana_Color.B), 0));
			_manaGradient.GradientStops.Add(new GradientStop(Color.FromArgb(70, _settings.Mana_Color.R, _settings.Mana_Color.G, _settings.Mana_Color.B), 0.05));
			_manaGradient.GradientStops.Add(new GradientStop(Color.FromArgb(60, _settings.Mana_Color.R, _settings.Mana_Color.G, _settings.Mana_Color.B), 0.1));
			_manaGradient.GradientStops.Add(new GradientStop(Color.FromArgb(35, _settings.Mana_Color.R, _settings.Mana_Color.G, _settings.Mana_Color.B), 0.25));
			_manaGradient.GradientStops.Add(new GradientStop(Color.FromArgb(00, _settings.Mana_Color.R, _settings.Mana_Color.G, _settings.Mana_Color.B), 1.0));

			if (_bothGradient.GradientStops.Count > 0)
			{
				_bothGradient.GradientStops.Clear();
			}
			_bothGradient.GradientStops.Add(new GradientStop(Color.FromArgb(90, _settings.Health_Mana_Color.R, _settings.Health_Mana_Color.G, _settings.Health_Mana_Color.B), 0));
			_bothGradient.GradientStops.Add(new GradientStop(Color.FromArgb(70, _settings.Health_Mana_Color.R, _settings.Health_Mana_Color.G, _settings.Health_Mana_Color.B), 0.05));
			_bothGradient.GradientStops.Add(new GradientStop(Color.FromArgb(60, _settings.Health_Mana_Color.R, _settings.Health_Mana_Color.G, _settings.Health_Mana_Color.B), 0.1));
			_bothGradient.GradientStops.Add(new GradientStop(Color.FromArgb(35, _settings.Health_Mana_Color.R, _settings.Health_Mana_Color.G, _settings.Health_Mana_Color.B), 0.25));
			_bothGradient.GradientStops.Add(new GradientStop(Color.FromArgb(00, _settings.Health_Mana_Color.R, _settings.Health_Mana_Color.G, _settings.Health_Mana_Color.B), 1.0));
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			SetLockStatus(_settings.Lock_ImageOverlay_Position);
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
					_manaLow = true;

					if(_manaLow && _healthLow && _settings.Mana_ShowTop && _settings.Health_ShowTop)
					{
						var gradient = _bothGradient.Clone();
						gradient.StartPoint = new Point(0, 0);
						gradient.EndPoint = new Point(0, 1);

						Gradient_Top.Height = (double)_settings.StaticOverlay_SizeTop;
						Gradient_Top.Fill = gradient;
						Gradient_Top.Visibility = Visibility.Visible;
					}
					else if(_settings.Mana_ShowTop)
					{
						var gradient = _manaGradient.Clone();
						gradient.StartPoint = new Point(0, 0);
						gradient.EndPoint = new Point(0, 1);

						Gradient_Top.Height = (double)_settings.StaticOverlay_SizeTop;
						Gradient_Top.Fill = gradient;
						Gradient_Top.Visibility = Visibility.Visible;
					}
					else if (!_settings.Mana_ShowTop && !(_settings.Health_ShowTop && _healthLow))
					{
						Gradient_Top.Visibility = Visibility.Hidden;
					}
					if(_manaLow && _healthLow && _settings.Mana_ShowLeft && _settings.Health_ShowLeft)
					{
						var gradient = _bothGradient.Clone();
						gradient.StartPoint = new Point(0, 0);
						gradient.EndPoint = new Point(1, 0);

						Gradient_Left.Width = (double)_settings.StaticOverlay_SizeLeft;
						Gradient_Left.Fill = gradient;
						Gradient_Left.Visibility = Visibility.Visible;
					}
					else if (_settings.Mana_ShowLeft)
					{
						var gradient = _manaGradient.Clone();
						gradient.StartPoint = new Point(0, 0);
						gradient.EndPoint = new Point(1, 0);

						Gradient_Left.Width = (double)_settings.StaticOverlay_SizeLeft;
						Gradient_Left.Fill = gradient;
						Gradient_Left.Visibility = Visibility.Visible;
					}
					else if (!_settings.Mana_ShowLeft && !(_settings.Health_ShowLeft && _healthLow))
					{
						Gradient_Left.Visibility = Visibility.Hidden;
					}
					if(_manaLow && _healthLow && _settings.Mana_ShowRight && _settings.Health_ShowRight)
					{
						var gradient = _bothGradient.Clone();
						gradient.StartPoint = new Point(1, 0);
						gradient.EndPoint = new Point(0, 0);

						Gradient_Right.Width = (double)_settings.StaticOverlay_SizeRight;
						Gradient_Right.Fill = gradient;
						Gradient_Right.Visibility = Visibility.Visible;
					}
					else if (_settings.Mana_ShowRight)
					{
						var gradient = _manaGradient.Clone();
						gradient.StartPoint = new Point(1, 0);
						gradient.EndPoint = new Point(0, 0);

						Gradient_Right.Width = (double)_settings.StaticOverlay_SizeRight;
						Gradient_Right.Fill = gradient;
						Gradient_Right.Visibility = Visibility.Visible;
					}
					else if (!_settings.Mana_ShowRight && !(_settings.Health_ShowRight && _healthLow))
					{
						Gradient_Right.Visibility = Visibility.Hidden;
					}
					if(_manaLow && _healthLow && _settings.Mana_ShowBottom && _settings.Health_ShowBottom)
					{
						var gradient = _bothGradient.Clone();
						gradient.StartPoint = new Point(0, 1);
						gradient.EndPoint = new Point(0, 0);

						Gradient_Bottom.Height = (double)_settings.StaticOverlay_SizeBottom;
						Gradient_Bottom.Fill = gradient;
						Gradient_Bottom.Visibility = Visibility.Visible;
					}
					else if (_settings.Mana_ShowBottom)
					{
						var gradient = _manaGradient.Clone();
						gradient.StartPoint = new Point(0, 1);
						gradient.EndPoint = new Point(0, 0);

						Gradient_Bottom.Height = (double)_settings.StaticOverlay_SizeBottom;
						Gradient_Bottom.Fill = gradient;
						Gradient_Bottom.Visibility = Visibility.Visible;
					}
					else if (!_settings.Mana_ShowBottom && !(_settings.Health_ShowBottom && _healthLow))
					{
						Gradient_Bottom.Visibility = Visibility.Hidden;
					}
				}
				else
				{
					_manaLow = false;

					if(!_healthLow && !_settings.Health_ShowTop)
					{
						Gradient_Top.Visibility = Visibility.Hidden;
					}
					if (!_healthLow && !_settings.Health_ShowLeft)
					{
						Gradient_Left.Visibility = Visibility.Hidden;
					}
					if (!_healthLow && !_settings.Health_ShowRight)
					{
						Gradient_Right.Visibility = Visibility.Hidden;
					}
					if (!_healthLow && !_settings.Health_ShowBottom)
					{
						Gradient_Bottom.Visibility = Visibility.Hidden;
					}
				}
			});
		}

		private void _pipeParser_HealthThresholdEvent(object sender, PipeParser.HealthThresholdEventArgs e)
		{
			this.appDispatcher.DispatchUI(() =>
			{
				if (e.IsLow)
				{
					_healthLow = true;

					if (_manaLow && _healthLow && _settings.Mana_ShowTop && _settings.Health_ShowTop)
					{
						var gradient = _bothGradient.Clone();
						gradient.StartPoint = new Point(0, 0);
						gradient.EndPoint = new Point(0, 1);

						Gradient_Top.Height = (double)_settings.StaticOverlay_SizeTop;
						Gradient_Top.Fill = gradient;
						Gradient_Top.Visibility = Visibility.Visible;
					}
					else if (_settings.Health_ShowTop)
					{
						var gradient = _healthGradient.Clone();
						gradient.StartPoint = new Point(0, 0);
						gradient.EndPoint = new Point(0, 1);

						Gradient_Top.Height = (double)_settings.StaticOverlay_SizeTop;
						Gradient_Top.Fill = gradient;
						Gradient_Top.Visibility = Visibility.Visible;
					}
					else if (!_settings.Health_ShowTop && !(_settings.Mana_ShowTop && _manaLow))
					{
						Gradient_Top.Visibility = Visibility.Hidden;
					}
					if (_manaLow && _healthLow && _settings.Mana_ShowLeft && _settings.Health_ShowLeft)
					{
						var gradient = _bothGradient.Clone();
						gradient.StartPoint = new Point(0, 0);
						gradient.EndPoint = new Point(1, 0);

						Gradient_Left.Width = (double)_settings.StaticOverlay_SizeLeft;
						Gradient_Left.Fill = gradient;
						Gradient_Left.Visibility = Visibility.Visible;
					}
					else if (_settings.Health_ShowLeft)
					{
						var gradient = _healthGradient.Clone();
						gradient.StartPoint = new Point(0, 0);
						gradient.EndPoint = new Point(1, 0);

						Gradient_Left.Width = (double)_settings.StaticOverlay_SizeLeft;
						Gradient_Left.Fill = gradient;
						Gradient_Left.Visibility = Visibility.Visible;
					}
					else if (!_settings.Health_ShowLeft && !(_settings.Mana_ShowLeft && _manaLow))
					{
						Gradient_Left.Visibility = Visibility.Hidden;
					}
					if (_manaLow && _healthLow && _settings.Mana_ShowRight && _settings.Health_ShowRight)
					{
						var gradient = _bothGradient.Clone();
						gradient.StartPoint = new Point(1, 0);
						gradient.EndPoint = new Point(0, 0);

						Gradient_Right.Width = (double)_settings.StaticOverlay_SizeRight;
						Gradient_Right.Fill = gradient;
						Gradient_Right.Visibility = Visibility.Visible;
					}
					else if (_settings.Health_ShowRight)
					{
						var gradient = _healthGradient.Clone();
						gradient.StartPoint = new Point(1, 0);
						gradient.EndPoint = new Point(0, 0);

						Gradient_Right.Width = (double)_settings.StaticOverlay_SizeRight;
						Gradient_Right.Fill = gradient;
						Gradient_Right.Visibility = Visibility.Visible;
					}
					else if (!_settings.Health_ShowRight && !(_settings.Mana_ShowRight && _manaLow))
					{
						Gradient_Right.Visibility = Visibility.Hidden;
					}
					if (_manaLow && _healthLow && _settings.Mana_ShowBottom && _settings.Health_ShowBottom)
					{
						var gradient = _bothGradient.Clone();
						gradient.StartPoint = new Point(0, 1);
						gradient.EndPoint = new Point(0, 0);

						Gradient_Bottom.Height = (double)_settings.StaticOverlay_SizeBottom;
						Gradient_Bottom.Fill = gradient;
						Gradient_Bottom.Visibility = Visibility.Visible;
					}
					else if (_settings.Health_ShowBottom)
					{
						var gradient = _healthGradient.Clone();
						gradient.StartPoint = new Point(0, 1);
						gradient.EndPoint = new Point(0, 0);

						Gradient_Bottom.Height = (double)_settings.StaticOverlay_SizeBottom;
						Gradient_Bottom.Fill = gradient;
						Gradient_Bottom.Visibility = Visibility.Visible;
					}
					else if (!_settings.Health_ShowBottom && !(_settings.Mana_ShowBottom && _manaLow))
					{
						Gradient_Bottom.Visibility = Visibility.Hidden;
					}
				}
				else
				{
					_healthLow = false;

					if(!_manaLow && !_settings.Mana_ShowTop)
					{
						Gradient_Top.Visibility = Visibility.Hidden;
					}
					if (!_manaLow && !_settings.Mana_ShowLeft)
					{
						Gradient_Left.Visibility = Visibility.Hidden;
					}
					if (!_manaLow && !_settings.Mana_ShowRight)
					{
						Gradient_Right.Visibility = Visibility.Hidden;
					}
					if (!_manaLow && !_settings.Mana_ShowBottom)
					{
						Gradient_Bottom.Visibility = Visibility.Hidden;
					}
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

		private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
		{
			double yAdjust = OverlayText.Height + e.VerticalChange;
			double xAdjust = OverlayText.Width + e.HorizontalChange;
			if((xAdjust >= 0) && (yAdjust >= 0))
			{
				OverlayText.Width = xAdjust;
				OverlayText.Height = yAdjust;
			}

		}

		private void Thumb_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
		{
			OverlayText.Background = new SolidColorBrush(Color.FromArgb(100, 40, 40, 40));
		}

		private void Thumb_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
		{
			OverlayText.Background = new SolidColorBrush(Color.FromArgb(180, 20, 20, 20));
		}
	}
}
