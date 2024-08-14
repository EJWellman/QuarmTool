using EQTool.Models;
using EQTool.Services;
using EQTool.ViewModels;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace EQTool
{
	public partial class ImageOverlay : BaseSaveStateWindow
	{
		private readonly PipeParser _pipeParser;
		private readonly EQToolSettings _settings;
		private readonly IAppDispatcher _appDispatcher;
		private readonly Models.WindowState _windowState;

		private bool _healthLow = false;
		private bool _manaLow = false;

		private LinearGradientBrush _healthGradient = new LinearGradientBrush();
		private LinearGradientBrush _manaGradient = new LinearGradientBrush();
		private LinearGradientBrush _bothGradient = new LinearGradientBrush();


		public ImageOverlay(PipeParser pipeParser, EQToolSettings settings, EQToolSettingsLoad toolSettingsLoad, IAppDispatcher appDispatcher)
			: base(settings.ImageOverlayWindowState, toolSettingsLoad, settings)
		{
			_windowState = settings.ImageOverlayWindowState;
			_pipeParser = pipeParser;
			this._appDispatcher = appDispatcher;
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
			_pipeParser.AutoAttackStatusChangedEvent += _pipeParser_AutoAttackStatusChangedEvent;

			UpdateGradientColors();

			var atkIndicatorAura = new QuarmAura
			{
				ID = 1,
				TextColor = "200,255,0,0",
				AuraDimensions = "100, 100",
				AuraPosition = "500, 500",
				Opacity = 1.0,
				AuraEnabled = true,
				Name = "Auto_Attack",
				FadeEnabled = true,
				FadedOpacity = 0.4,
				FadeSpeed = 2.5,
				HasTextTrigger = false,
				ImageEnabled = true,
				ImagePath = "pack://application:,,,/dps.png",
				PulseEnabled = true,
				PulseSize = 2.0,
				PulseSpeed = 2.5,
				ShowText = true,
				ShownText = "Auto Attack On",
				TextSize = 18,
				TextPosition = "50, -50",
			};


			CreateAuraElement(atkIndicatorAura);
		}

		private void _pipeParser_AutoAttackStatusChangedEvent(object sender, PipeParser.AutoAttackStatusChangedEventArgs e)
		{
			this._appDispatcher.DispatchUI(() =>
			{
				if (e.IsAutoAttacking)
				{
					System.Windows.Controls.Image atkIndicator = AuraContainer.Children.OfType<Image>().FirstOrDefault(i => i.Name == "Auto_Attack");
					if (atkIndicator != null)
					{
						((System.Windows.Controls.Image)atkIndicator)
							.Visibility = Visibility.Visible;
					}
				}
				else
				{
					System.Windows.Controls.Image atkIndicator = AuraContainer.Children.OfType<Image>().FirstOrDefault(i => i.Name == "Auto_Attack");
					if (atkIndicator != null)
					{
						((System.Windows.Controls.Image)atkIndicator)
							.Visibility = Visibility.Hidden;
					}
				}
			});
		}

		private void CreateAuraElement(QuarmAura aura)
		{
			if(aura != null & aura.AuraEnabled)
			{
				bool isNewElementAssignable = false;
				FrameworkElement newElement = new FrameworkElement();
				Storyboard elementStoryboard = new Storyboard();
				if (aura.ImageEnabled && aura.ShowText)
				{
					double[] imgSize = aura.AuraDimensions.Split(',').Select(double.Parse).ToArray();
					double[] imgPos = aura.AuraPosition.Split(',').Select(double.Parse).ToArray();
					byte[] colorBytes = aura.TextColor.Split(',').Select(byte.Parse).ToArray();
					double[] textPos = aura.TextPosition.Split(',').Select(double.Parse).ToArray();
					var auraImage = new BitmapImage(new System.Uri(aura.ImagePath));
					var visual = new DrawingVisual();
					using (DrawingContext drawingContext = visual.RenderOpen())
					{
						drawingContext.DrawImage(auraImage, new Rect(0, 0, imgSize[0], imgSize[1]));
						drawingContext.DrawText(
							new FormattedText(aura.ShownText, CultureInfo.InvariantCulture, FlowDirection.LeftToRight,
								new Typeface("Segoe UI"), aura.TextSize, new SolidColorBrush(Color.FromArgb(colorBytes[0], colorBytes[1], colorBytes[2], colorBytes[3])), null, 1)
									, new Point(textPos[0], textPos[1]));
					}
					double width = imgSize[0];
					double height = imgSize[1];
					if (textPos[0] < 0)
					{
						width = imgSize[0] + System.Math.Abs(textPos[0]);
					}
					else if (textPos[0] > imgSize[0])
					{
						width = imgSize[0] + textPos[0];
					}
					if (textPos[1] < 0)
					{
						height = imgSize[1] + System.Math.Abs(textPos[1]);
					}
					else if (textPos[1] > imgSize[1])
					{
						height = imgSize[1] + textPos[1];
					}


					newElement = new System.Windows.Controls.Image()
					{
						Source = new DrawingImage(visual.Drawing),
						Width = width,
						Height = height,
						Name = aura.Name
					};

					Canvas.SetTop(newElement, imgPos[0]);
					Canvas.SetLeft(newElement, imgPos[1]);

					isNewElementAssignable = true;
				}
				else if (aura.ImageEnabled)
				{

				}
				else if (aura.ShowText)
				{

				}

				if(aura.PulseEnabled)
				{
					var heightAnimation = new System.Windows.Media.Animation.DoubleAnimation
					{
						From = 1,
						To = aura.PulseSize,
						Duration = new Duration(System.TimeSpan.FromSeconds(aura.PulseSpeed)),
						AutoReverse = true,
						RepeatBehavior = RepeatBehavior.Forever
					};
					var widthAnimation = new System.Windows.Media.Animation.DoubleAnimation
					{
						From = 1,
						To = aura.PulseSize,
						Duration = new Duration(System.TimeSpan.FromSeconds(aura.PulseSpeed)),
						AutoReverse = true,
						RepeatBehavior = RepeatBehavior.Forever
					};

					elementStoryboard.Children.Add(heightAnimation);
					elementStoryboard.Children.Add(widthAnimation);

					newElement.RenderTransform = new ScaleTransform(1.0, 1.0, newElement.Width / 2, newElement.Height / 2);

					Storyboard.SetTarget(heightAnimation, newElement);
					Storyboard.SetTargetProperty(heightAnimation, new PropertyPath("RenderTransform.ScaleY"));
					Storyboard.SetTarget(widthAnimation, newElement);
					Storyboard.SetTargetProperty(widthAnimation, new PropertyPath("RenderTransform.ScaleX"));
				}
				if (aura.FadeEnabled)
				{
					var opacityAnimation = new System.Windows.Media.Animation.DoubleAnimation
					{
						From = aura.Opacity,
						To = aura.FadedOpacity,
						Duration = new Duration(System.TimeSpan.FromSeconds(aura.FadeSpeed)),
						FillBehavior = FillBehavior.Stop,
						AutoReverse = true,
						RepeatBehavior = RepeatBehavior.Forever
					};

					elementStoryboard.Children.Add(opacityAnimation);

					Storyboard.SetTarget(opacityAnimation, newElement);
					Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath("Opacity"));
				}


				if (isNewElementAssignable)
				{
					AuraContainer.Children.Add(newElement);
					if (elementStoryboard.Children.Count > 0)
					{
						elementStoryboard.Begin();
					}
				}
			}
			//return;

			//System.Windows.Controls.Image atkImage = new System.Windows.Controls.Image
			//{
			//	Width = 100,
			//	Height = 100,
			//	Name = "AttackIndicator",
			//	Source = new BitmapImage(new System.Uri("pack://application:,,,/dps.png"))
			//};

			//Canvas.SetTop(atkImage, _settings.AtkIndicator_Top);
			//Canvas.SetLeft(atkImage, _settings.AtkIndicator_Left);

			//AuraContainer.Children.Add(atkImage);
		}

		public void UpdateGradientColors()
		{
			if (_healthGradient.GradientStops.Count > 0)
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
			SetWindowLockStatus(_settings.Lock_ImageOverlay_Position);
		}

		public void SetWindowLockStatus(bool isLocked)
		{
			Mover.Visibility = isLocked ? Visibility.Hidden : Visibility.Visible;
			if (isLocked)
			{
				this.WindowState = System.Windows.WindowState.Maximized;
				var hwnd = new WindowInteropHelper(this).Handle;
				WindowServices.SetWindowExTransparent(hwnd);
				AuraContainer.AllowDragging = false;


				System.Windows.Controls.Image atkIndicator = AuraContainer.Children.OfType<Image>().FirstOrDefault(i => i.Name == "AttackIndicator");
				if (atkIndicator != null)
				{
					var top = (double)((System.Windows.Controls.Image)atkIndicator).GetValue(Canvas.TopProperty);
					var left = (double)((System.Windows.Controls.Image)atkIndicator).GetValue(Canvas.LeftProperty);

					_settings.AtkIndicator_Top = top;
					_settings.AtkIndicator_Left = left;
				}
			}
			else
			{
				this.WindowState = System.Windows.WindowState.Normal;
				var hwnd = new WindowInteropHelper(this).Handle;
				WindowServices.SetWindowExNotTransparent(hwnd);
				AuraContainer.AllowDragging = true;
			}
			this.IsHitTestVisible = !isLocked;
		}

		private void _pipeParser_ManaThresholdEvent(object sender, PipeParser.ManaThresholdEventArgs e)
		{
			this._appDispatcher.DispatchUI(() =>
			{
				if (e.IsLow)
				{
					_manaLow = true;

					if (_manaLow && _healthLow && _settings.Mana_ShowTop && _settings.Health_ShowTop)
					{
						var gradient = _bothGradient.Clone();
						gradient.StartPoint = new Point(0, 0);
						gradient.EndPoint = new Point(0, 1);

						Gradient_Top.Height = (double)_settings.StaticOverlay_SizeTop;
						Gradient_Top.Fill = gradient;
						Gradient_Top.Visibility = Visibility.Visible;
					}
					else if (_settings.Mana_ShowTop)
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
					if (_manaLow && _healthLow && _settings.Mana_ShowLeft && _settings.Health_ShowLeft)
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
					if (_manaLow && _healthLow && _settings.Mana_ShowRight && _settings.Health_ShowRight)
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
					if (_manaLow && _healthLow && _settings.Mana_ShowBottom && _settings.Health_ShowBottom)
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

					if (!_healthLow && !_settings.Health_ShowTop)
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
			this._appDispatcher.DispatchUI(() =>
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

					if (!_manaLow && !_settings.Mana_ShowTop)
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
	}
}
