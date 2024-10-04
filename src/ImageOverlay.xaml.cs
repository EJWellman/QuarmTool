using EQTool.Models;
using EQTool.Services;
using EQTool.Utilities;
using EQTool.ViewModels;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using System.Windows.Threading;
using System;
using ZealPipes.Services;
using System.Windows.Documents;
using EQTool.Controls;
using System.Windows.Controls.Primitives;
using EQTool.EventArgModels;
using static System.Resources.ResXFileRef;


namespace EQTool
{
	public partial class ImageOverlay : BaseSaveStateWindow
	{
		private readonly PipeParser _pipeParser;
		private readonly EQToolSettings _settings;
		private readonly IAppDispatcher _appDispatcher;
		private readonly EQToolSettingsLoad _settingsLoad;
		private readonly Models.WindowState _windowState;
		private MeshElement3D _POIArrow = null;
		private Matrix3D? _POIArrowMatrix = null;
		private Point3D _POIArrowCenter;

		private Point3D _PlayerLocation;

		private bool _healthLow = false;
		private bool _manaLow = false;

		private LinearGradientBrush _healthGradient = new LinearGradientBrush();
		private LinearGradientBrush _manaGradient = new LinearGradientBrush();
		private LinearGradientBrush _bothGradient = new LinearGradientBrush();

		private BrushConverter converter = new BrushConverter();

		public ImageOverlay(PipeParser pipeParser, EQToolSettings settings, EQToolSettingsLoad toolSettingsLoad, IAppDispatcher appDispatcher)
			: base(settings.ImageOverlayWindowState, toolSettingsLoad, settings)
		{
			_settingsLoad = toolSettingsLoad;
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
			//_pipeParser.AutoAttackStatusChangedEvent += _pipeParser_AutoAttackStatusChangedEvent;
			//_pipeParser.ZealLocationEvent += _pipeParser_ZealLocationEvent;

			_pipeParser.ResistSpellEvent += PipeParser_ResistSpellEvent;
			_pipeParser.GroupInviteEvent += PipeParser_GroupInviteEvent;
			_pipeParser.FailedFeignEvent += PipeParser_FailedFeignEvent;
			_pipeParser.CharmBreakEvent += PipeParser_CharmBreakEvent;
			_pipeParser.InvisEvent += PipeParser_InvisEvent;
			_pipeParser.LevEvent += PipeParser_LevEvent;
			_pipeParser.EnrageEvent += PipeParser_EnrageEvent;



			_pipeParser.CustomOverlayEvent += _pipeParser_CustomOverlayEvent;

			UpdateGradientColors();

			OverlayText_Canvas.Background = new SolidColorBrush(Color.FromArgb(122, 35, 35, 35));

			var atkIndicatorAura = new QuarmAura
			{
				ID = 1,
				TextColor = "200,255,0,0",
				AuraDimensions = "100, 100",
				AuraPosition = "1230, 125",
				Opacity = 1.0,
				AuraDuration = 3,
				AuraEnabled = false,
				ShowEdgeAuras = false,
				ShowTopEdgeAura = true,
				ShowLeftEdgeAura = true,
				ShowRightEdgeAura = true,
				ShowBottomEdgeAura = true,
				EdgeAuraColor = Color.FromRgb(255, 0, 0),
				EdgeAuraSize = 200,
				Name = "Auto_Attack",
				FadeEnabled = true,
				FadedOpacity = 0.4,
				FadeSpeed = 3.5,
				HasTextTrigger = false,
				ImageEnabled = true,
				ImagePath = "pack://application:,,,/dps.png",
				PulseEnabled = true,
				PulseSize = 1.5,
				PulseSpeed = 3.5,
				ShowText = true,
				ShownText = "Auto Attack On",
				TextSize = 18,
				TextPosition = "-10, -50",
			};

			ArrowContainer.Visibility = Visibility.Hidden;
			//CreateAuraElement(atkIndicatorAura);
			//Add3DArrowToScene();

			OverlayText_Canvas.SetValue(Canvas.LeftProperty, _settings.ImageOverlay_TriggersRect.X);
			OverlayText_Canvas.SetValue(Canvas.TopProperty, _settings.ImageOverlay_TriggersRect.Y);
			OverlayText_Canvas.Width = _settings.ImageOverlay_TriggersRect.Width;
			OverlayText_Canvas.Height = _settings.ImageOverlay_TriggersRect.Height;
		}

		private void AddOverlayMessage(string message, SolidColorBrush overlayColor)
		{
			Label overlayText = new Label();
			overlayText.Content = message;
			overlayText.FontSize = _settings.OverlayFontSize ?? 30;
			overlayText.Foreground = overlayColor;
			overlayText.Height = OverlayText_Canvas.Height;
			overlayText.Width = OverlayText_Canvas.Width;
			overlayText.HorizontalContentAlignment = HorizontalAlignment.Center;
			overlayText.VerticalContentAlignment = VerticalAlignment.Center;

			bool slideUp = true;

			var slideTransform = new TranslateTransform(0, 0);
			Duration dur = new Duration(TimeSpan.FromSeconds(3));
			DoubleAnimation anim = new DoubleAnimation(slideUp ? -100 : 100, dur);
			anim.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseInOut };

			OverlayText_Grid.Children.Add(overlayText);
			overlayText.RenderTransform = slideTransform;
			slideTransform.BeginAnimation(TranslateTransform.YProperty, anim);
			var t = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(3) };
			t.Tick += (s, e2) =>
			{
				OverlayText_Grid.Children.Remove(overlayText);
				overlayText = null;
				t.Stop();
			};
			t.Start();
		}

		private void _pipeParser_CustomOverlayEvent(object sender, CustomOverlayEventArgs e)
		{
			_appDispatcher.DispatchUI(() =>
			{
				SolidColorBrush overlayColor = new SolidColorBrush(Color.FromRgb(e.CustomOverlay.Color.R, e.CustomOverlay.Color.G, e.CustomOverlay.Color.B));
				AddOverlayMessage(e.CustomOverlay.Message, overlayColor);
			});
		}

		private void _pipeParser_ZealLocationEvent(object sender, ZealMessageService.PlayerMessageReceivedEventArgs e)
		{
			if (_PlayerLocation == null)
			{
				_PlayerLocation = new Point3D(e.Message.Data.Position.X, e.Message.Data.Position.Y, e.Message.Data.Position.Z);
			}
			else
			{
				_PlayerLocation.X = e.Message.Data.Position.Y;
				_PlayerLocation.Y = e.Message.Data.Position.X;
				_PlayerLocation.Z = e.Message.Data.Position.Z;
			}

			PointArrowToLocation(new Point3D(-309, -1702, 4), e.Message.Data.heading);
		}

		private void Add3DArrowToScene()
		{
			ArrowContainer.Children.Add(new DefaultLights());
			ArrowContainer.ShowViewCube = false;
			ArrowContainer.Height = 200;
			ArrowContainer.Width = 200;
			var arrow = new ArrowVisual3D();
			_POIArrow = arrow;
			Vector3D axis = new Vector3D(1, 0, 1);

			var x_mid = _POIArrow.Content.Bounds.SizeX / 2;
			var y_mid = _POIArrow.Content.Bounds.SizeY / 2;
			var z_mid = _POIArrow.Content.Bounds.SizeZ / 2;
			_POIArrowCenter = new Point3D(x_mid, y_mid, z_mid);

			_POIArrowMatrix = _POIArrow.Content.Transform.Value;

			//ArrowContainer.Children.Add(_POIArrow);
			ArrowContainer.Children.Add(arrow);
		}


		private void _pipeParser_AutoAttackStatusChangedEvent(object sender, PipeParser.AutoAttackStatusChangedEventArgs e)
		{
			this._appDispatcher.DispatchUI(() =>
			{
				if (e.IsAutoAttacking)
				{
					var autoAtkIndicators = AuraContainer.Children.OfType<FrameworkElement>().Where(i => i.Name == "Auto_Attack");
					foreach (var autoAtkIndicator in autoAtkIndicators)
					{
						((FrameworkElement)autoAtkIndicator)
							.Visibility = Visibility.Visible;
					}
					var autoAtkEdges = AuraGrid.Children.OfType<FrameworkElement>().Where(i => i.Name == "Auto_Attack");
					foreach (var autoAtkEdge in autoAtkEdges)
					{
						((FrameworkElement)autoAtkEdge)
							.Visibility = Visibility.Visible;
					}
				}
				else
				{
					var autoAtkIndicators = AuraContainer.Children.OfType<FrameworkElement>().Where(i => i.Name == "Auto_Attack");
					foreach (var autoAtkIndicator in autoAtkIndicators)
					{
						((FrameworkElement)autoAtkIndicator)
							.Visibility = Visibility.Hidden;
					}
					var autoAtkEdges = AuraGrid.Children.OfType<FrameworkElement>().Where(i => i.Name == "Auto_Attack");
					foreach (var autoAtkEdge in autoAtkEdges)
					{
						((FrameworkElement)autoAtkEdge)
							.Visibility = Visibility.Hidden;
					}
				}
			});
		}

		private void CreateAuraElement(QuarmAura aura)
		{
			if (aura != null & aura.AuraEnabled)
			{
				bool isNewElementAssignable = false;
				bool isEdgeElementAssignable = false;
				FrameworkElement newElement = new FrameworkElement();
				List<FrameworkElement> edgeElements = new List<FrameworkElement>();
				Storyboard elementStoryboard = new Storyboard();
				if (aura.ImageEnabled && aura.ShowText)
				{
					double[] auraSize = aura.AuraDimensions.Split(',').Select(double.Parse).ToArray();
					double[] auraPos = aura.AuraPosition.Split(',').Select(double.Parse).ToArray();
					byte[] colorBytes = aura.TextColor.Split(',').Select(byte.Parse).ToArray();
					double[] textPos = aura.TextPosition.Split(',').Select(double.Parse).ToArray();
					var auraImage = new BitmapImage(new System.Uri(aura.ImagePath));
					var visual = new DrawingVisual();
					using (DrawingContext drawingContext = visual.RenderOpen())
					{
						drawingContext.DrawImage(auraImage, new Rect(0, 0, auraSize[0], auraSize[1]));
						drawingContext.DrawText(
							new FormattedText(aura.ShownText, CultureInfo.InvariantCulture, FlowDirection.LeftToRight,
								new Typeface("Segoe UI"), aura.TextSize, new SolidColorBrush(Color.FromArgb(colorBytes[0], colorBytes[1], colorBytes[2], colorBytes[3])), null, 1)
									, new Point(textPos[0], textPos[1]));
					}
					double width = auraSize[0];
					double height = auraSize[1];
					if (textPos[0] < 0)
					{
						width = auraSize[0] + System.Math.Abs(textPos[0]);
					}
					else if (textPos[0] > auraSize[0])
					{
						width = auraSize[0] + textPos[0];
					}
					if (textPos[1] < 0)
					{
						height = auraSize[1] + System.Math.Abs(textPos[1]);
					}
					else if (textPos[1] > auraSize[1])
					{
						height = auraSize[1] + textPos[1];
					}

					newElement = new System.Windows.Controls.Image()
					{
						Source = new DrawingImage(visual.Drawing),
						Width = width,
						Height = height,
						Name = aura.Name
					};

					Canvas.SetLeft(newElement, auraPos[0]);
					Canvas.SetTop(newElement, auraPos[1]);

					isNewElementAssignable = true;
				}
				else if (aura.ImageEnabled)
				{
					double[] imgSize = aura.AuraDimensions.Split(',').Select(double.Parse).ToArray();
					double[] imgPos = aura.AuraPosition.Split(',').Select(double.Parse).ToArray();
					byte[] colorBytes = aura.TextColor.Split(',').Select(byte.Parse).ToArray();
					var auraImage = new BitmapImage(new System.Uri(aura.ImagePath));
					var visual = new DrawingVisual();
					using (DrawingContext drawingContext = visual.RenderOpen())
					{
						drawingContext.DrawImage(auraImage, new Rect(0, 0, imgSize[0], imgSize[1]));
					}
					double width = imgSize[0];
					double height = imgSize[1];

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
				else if (aura.ShowText)
				{
					double[] auraPos = aura.AuraPosition.Split(',').Select(double.Parse).ToArray();
					byte[] colorBytes = aura.TextColor.Split(',').Select(byte.Parse).ToArray();
					double[] textPos = aura.TextPosition.Split(',').Select(double.Parse).ToArray();
					var visual = new DrawingVisual();
					using (DrawingContext drawingContext = visual.RenderOpen())
					{
						drawingContext.DrawText(
							new FormattedText(aura.ShownText, CultureInfo.InvariantCulture, FlowDirection.LeftToRight,
								new Typeface("Segoe UI"), aura.TextSize, new SolidColorBrush(Color.FromArgb(colorBytes[0], colorBytes[1], colorBytes[2], colorBytes[3])), null, 1)
									, new Point(textPos[0], textPos[1]));
					}

					DrawingImage drawingImage = new DrawingImage(visual.Drawing);

					newElement = new System.Windows.Controls.Image()
					{
						Source = drawingImage,
						Width = drawingImage.Width,
						Height = drawingImage.Height,
						Name = aura.Name
					};

					Canvas.SetTop(newElement, auraPos[0]);
					Canvas.SetLeft(newElement, auraPos[1]);

					isNewElementAssignable = true;
				}
				if (aura.ShowEdgeAuras)
				{
					if (aura.ShowTopEdgeAura)
					{
						LinearGradientBrush gradient = new LinearGradientBrush();
						gradient.GradientStops.Add(new GradientStop(Color.FromArgb(90, aura.EdgeAuraColor.R, aura.EdgeAuraColor.G, aura.EdgeAuraColor.B), 0));
						gradient.GradientStops.Add(new GradientStop(Color.FromArgb(70, aura.EdgeAuraColor.R, aura.EdgeAuraColor.G, aura.EdgeAuraColor.B), 0.05));
						gradient.GradientStops.Add(new GradientStop(Color.FromArgb(60, aura.EdgeAuraColor.R, aura.EdgeAuraColor.G, aura.EdgeAuraColor.B), 0.1));
						gradient.GradientStops.Add(new GradientStop(Color.FromArgb(35, aura.EdgeAuraColor.R, aura.EdgeAuraColor.G, aura.EdgeAuraColor.B), 0.25));
						gradient.GradientStops.Add(new GradientStop(Color.FromArgb(00, aura.EdgeAuraColor.R, aura.EdgeAuraColor.G, aura.EdgeAuraColor.B), 1.0));

						gradient.StartPoint = new Point(0, 0);
						gradient.EndPoint = new Point(0, 1);

						Gradient_Top.Height = aura.EdgeAuraSize;
						Gradient_Top.Fill = gradient;
						Gradient_Top.Visibility = Visibility.Visible;

						Rectangle topEdge = new Rectangle()
						{
							Width = this.Width,
							Height = aura.EdgeAuraSize,
							Fill = gradient,
							VerticalAlignment = VerticalAlignment.Top,
							Name = aura.Name
						};

						Grid.SetRow(topEdge, 0);
						Grid.SetColumnSpan(topEdge, 3);
						AuraGrid.Children.Add(topEdge);
						edgeElements.Add(topEdge);
						isEdgeElementAssignable = true;
					}
					if (aura.ShowLeftEdgeAura)
					{
						LinearGradientBrush gradient = new LinearGradientBrush();
						gradient.GradientStops.Add(new GradientStop(Color.FromArgb(90, aura.EdgeAuraColor.R, aura.EdgeAuraColor.G, aura.EdgeAuraColor.B), 0));
						gradient.GradientStops.Add(new GradientStop(Color.FromArgb(70, aura.EdgeAuraColor.R, aura.EdgeAuraColor.G, aura.EdgeAuraColor.B), 0.05));
						gradient.GradientStops.Add(new GradientStop(Color.FromArgb(60, aura.EdgeAuraColor.R, aura.EdgeAuraColor.G, aura.EdgeAuraColor.B), 0.1));
						gradient.GradientStops.Add(new GradientStop(Color.FromArgb(35, aura.EdgeAuraColor.R, aura.EdgeAuraColor.G, aura.EdgeAuraColor.B), 0.25));
						gradient.GradientStops.Add(new GradientStop(Color.FromArgb(00, aura.EdgeAuraColor.R, aura.EdgeAuraColor.G, aura.EdgeAuraColor.B), 1.0));

						gradient.StartPoint = new Point(0, 0);
						gradient.EndPoint = new Point(1, 0);

						Gradient_Left.Width = aura.EdgeAuraSize;
						Gradient_Left.Fill = gradient;
						Gradient_Left.Visibility = Visibility.Visible;

						Rectangle leftEdge = new Rectangle()
						{
							Width = aura.EdgeAuraSize,
							Height = this.Height,
							Fill = gradient,
							HorizontalAlignment = HorizontalAlignment.Left,
							Name = aura.Name
						};

						Grid.SetColumn(leftEdge, 0);
						Grid.SetRowSpan(leftEdge, 3);
						AuraGrid.Children.Add(leftEdge);

						edgeElements.Add(leftEdge);
						isEdgeElementAssignable = true;
					}
					if (aura.ShowRightEdgeAura)
					{
						LinearGradientBrush gradient = new LinearGradientBrush();
						gradient.GradientStops.Add(new GradientStop(Color.FromArgb(90, aura.EdgeAuraColor.R, aura.EdgeAuraColor.G, aura.EdgeAuraColor.B), 0));
						gradient.GradientStops.Add(new GradientStop(Color.FromArgb(70, aura.EdgeAuraColor.R, aura.EdgeAuraColor.G, aura.EdgeAuraColor.B), 0.05));
						gradient.GradientStops.Add(new GradientStop(Color.FromArgb(60, aura.EdgeAuraColor.R, aura.EdgeAuraColor.G, aura.EdgeAuraColor.B), 0.1));
						gradient.GradientStops.Add(new GradientStop(Color.FromArgb(35, aura.EdgeAuraColor.R, aura.EdgeAuraColor.G, aura.EdgeAuraColor.B), 0.25));
						gradient.GradientStops.Add(new GradientStop(Color.FromArgb(00, aura.EdgeAuraColor.R, aura.EdgeAuraColor.G, aura.EdgeAuraColor.B), 1.0));

						gradient.StartPoint = new Point(1, 0);
						gradient.EndPoint = new Point(0, 0);

						Gradient_Right.Width = aura.EdgeAuraSize;
						Gradient_Right.Fill = gradient;
						Gradient_Right.Visibility = Visibility.Visible;

						Rectangle rightEdge = new Rectangle()
						{
							Width = aura.EdgeAuraSize,
							Height = this.Height,
							Fill = gradient,
							HorizontalAlignment = HorizontalAlignment.Right,
							Name = aura.Name
						};

						Grid.SetColumn(rightEdge, 2);
						Grid.SetRowSpan(rightEdge, 3);
						AuraGrid.Children.Add(rightEdge);

						edgeElements.Add(rightEdge);
						isEdgeElementAssignable = true;
					}
					if (aura.ShowBottomEdgeAura)
					{
						LinearGradientBrush gradient = new LinearGradientBrush();
						gradient.GradientStops.Add(new GradientStop(Color.FromArgb(90, aura.EdgeAuraColor.R, aura.EdgeAuraColor.G, aura.EdgeAuraColor.B), 0));
						gradient.GradientStops.Add(new GradientStop(Color.FromArgb(70, aura.EdgeAuraColor.R, aura.EdgeAuraColor.G, aura.EdgeAuraColor.B), 0.05));
						gradient.GradientStops.Add(new GradientStop(Color.FromArgb(60, aura.EdgeAuraColor.R, aura.EdgeAuraColor.G, aura.EdgeAuraColor.B), 0.1));
						gradient.GradientStops.Add(new GradientStop(Color.FromArgb(35, aura.EdgeAuraColor.R, aura.EdgeAuraColor.G, aura.EdgeAuraColor.B), 0.25));
						gradient.GradientStops.Add(new GradientStop(Color.FromArgb(00, aura.EdgeAuraColor.R, aura.EdgeAuraColor.G, aura.EdgeAuraColor.B), 1.0));

						gradient.StartPoint = new Point(0, 1);
						gradient.EndPoint = new Point(0, 0);


						Gradient_Bottom.Height = aura.EdgeAuraSize;
						Gradient_Bottom.Fill = gradient;
						Gradient_Bottom.Visibility = Visibility.Visible;

						Rectangle bottomEdge = new Rectangle()
						{
							Width = this.Width,
							Height = aura.EdgeAuraSize,
							Fill = gradient,
							VerticalAlignment = VerticalAlignment.Bottom,
							Name = aura.Name
						};

						Grid.SetRow(bottomEdge, 2);
						Grid.SetColumnSpan(bottomEdge, 3);
						AuraGrid.Children.Add(bottomEdge);
						edgeElements.Add(bottomEdge);
						isEdgeElementAssignable = true;
					}
				}

				if (aura.PulseEnabled)
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

					if (isNewElementAssignable)
					{
						newElement.RenderTransform = new ScaleTransform(1.0, 1.0, newElement.Width / 2, newElement.Height / 2);

						Storyboard.SetTarget(heightAnimation, newElement);
						Storyboard.SetTargetProperty(heightAnimation, new PropertyPath("RenderTransform.ScaleY"));
						Storyboard.SetTarget(widthAnimation, newElement);
						Storyboard.SetTargetProperty(widthAnimation, new PropertyPath("RenderTransform.ScaleX"));
					}
					if (isEdgeElementAssignable)
					{
						var heightAnimation2 = new System.Windows.Media.Animation.DoubleAnimation
						{
							From = 1,
							To = aura.PulseSize,
							Duration = new Duration(System.TimeSpan.FromSeconds(aura.PulseSpeed)),
							AutoReverse = true,
							RepeatBehavior = RepeatBehavior.Forever
						};
						var widthAnimation2 = new System.Windows.Media.Animation.DoubleAnimation
						{
							From = 1,
							To = aura.PulseSize,
							Duration = new Duration(System.TimeSpan.FromSeconds(aura.PulseSpeed)),
							AutoReverse = true,
							RepeatBehavior = RepeatBehavior.Forever
						};

						foreach (var edgeElement in edgeElements)
						{
							int column = Grid.GetColumn(edgeElement);
							int row = Grid.GetRow(edgeElement);
							int colSpan = Grid.GetColumnSpan(edgeElement);
							int rowSpan = Grid.GetRowSpan(edgeElement);
							if (colSpan == 1)
							{
								Storyboard.SetTargetName(heightAnimation2, ((Grid)edgeElement.Parent).ColumnDefinitions[column].Name);
								Storyboard.SetTargetProperty(heightAnimation2, new PropertyPath("Height"));
							}
							else if (rowSpan == 1)
							{
								Storyboard.SetTargetName(widthAnimation2, ((Grid)edgeElement.Parent).RowDefinitions[row].Name);
								Storyboard.SetTargetProperty(widthAnimation2, new PropertyPath("Width"));
							}
						}
					}
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

					if (isNewElementAssignable)
					{
						Storyboard.SetTarget(opacityAnimation, newElement);
						Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath("Opacity"));
					}
					if (isEdgeElementAssignable)
					{
						foreach (var edgeElement in edgeElements)
						{
							Storyboard.SetTarget(opacityAnimation, edgeElement.Parent);
							Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath("Opacity"));
						}
					}
				}

				if (isNewElementAssignable)
				{
					AuraContainer.Children.Add(newElement);
				}

				if (elementStoryboard.Children.Count > 0)
				{
					elementStoryboard.Begin();
				}
			}
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
			var adorner = AdornerLayer.GetAdornerLayer(OverlayText_Canvas);
			adorner.Add(new TextBlockAdorner(OverlayText_Canvas));

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
				AdornerLayer.GetAdornerLayer(OverlayText_Canvas).GetAdorners(OverlayText_Canvas).ToList().ForEach(a => a.Visibility = Visibility.Hidden);
				OverlayText_Canvas.Background = new SolidColorBrush(Color.FromArgb(0, 35, 35, 35));
				Mover.IsHitTestVisible = false;

				_settings.ImageOverlay_TriggersRect = new Rect(new Point((double)OverlayText_Canvas.GetValue(Canvas.LeftProperty), (double)OverlayText_Canvas.GetValue(Canvas.TopProperty)), new Size(OverlayText_Canvas.Width, OverlayText_Canvas.Height));

				//System.Windows.Controls.Image atkIndicator = AuraContainer.Children.OfType<Image>().FirstOrDefault(i => i.Name == "AttackIndicator");
				//if (atkIndicator != null)
				//{
				//	var top = (double)((System.Windows.Controls.Image)atkIndicator).GetValue(Canvas.TopProperty);
				//	var left = (double)((System.Windows.Controls.Image)atkIndicator).GetValue(Canvas.LeftProperty);

				//	_settings.AtkIndicator_Top = top;
				//	_settings.AtkIndicator_Left = left;
				//}
			}
			else
			{
				this.WindowState = System.Windows.WindowState.Normal;
				var hwnd = new WindowInteropHelper(this).Handle;
				WindowServices.SetWindowExNotTransparent(hwnd);
				AuraContainer.AllowDragging = true;
				AdornerLayer.GetAdornerLayer(OverlayText_Canvas).GetAdorners(OverlayText_Canvas)?.ToList()?.ForEach(a => a.Visibility = Visibility.Visible);
				OverlayText_Canvas.Background = new SolidColorBrush(Color.FromArgb(188, 75, 0, 0));
				Mover.IsHitTestVisible = true;

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
		private void PointArrowToLocation(Point3D targetLocation, float heading)
		{
			if (_POIArrow != null && _POIArrowMatrix.HasValue)
			{
				this._appDispatcher.DispatchUI(() =>
				{
					Vector3D direction = targetLocation - _PlayerLocation;
					direction.Normalize();

					// Calculate the player's heading
					float playerHeading2 = (heading / -512) * 360 + 180;

					double angleInRadians = playerHeading2 * Math.PI / 180; // Convert the angle to radians
					double x = Math.Cos(angleInRadians);
					double y = Math.Sin(angleInRadians);

					Vector3D playerHeading = new Vector3D(y, x, _PlayerLocation.Z);
					playerHeading.Normalize();

					// Calculate the rotation axis
					Vector3D rotationAxis = Vector3D.CrossProduct(playerHeading, direction);
					rotationAxis.X *= -1;
					rotationAxis.Z = 0;
					//rotationAxis.Y *= -1;
					rotationAxis.Normalize();

					// Calculate the rotation angle
					double rotationAngle = Vector3D.AngleBetween(playerHeading, direction);

					// Create the rotation matrix
					Matrix3D rotationMatrix = new Matrix3D();
					rotationMatrix.Rotate(new Quaternion(rotationAxis, rotationAngle));

					// Apply the rotation to the transformation matrix
					Matrix3D transformationMatrix = _POIArrowMatrix.Value * rotationMatrix;

					_POIArrow.Content.Transform = new MatrixTransform3D(transformationMatrix);

					//Vector3D playerVector = new Vector3D(_PlayerLocation.X, _PlayerLocation.Y, _PlayerLocation.Z);
					//Vector3D targetVector = new Vector3D(targetLocation.X, targetLocation.Y, targetLocation.Z);

					//Vector3D deltaVector = targetVector - playerVector;

					//double theta = Math.Atan2(deltaVector.Y, deltaVector.X);
					//double thetaDegrees = theta * (Math.PI / 180);


					//Vector3D direction = targetLocation - _PlayerLocation;
					//Vector3D direction2 = targetLocation - _PlayerLocation;
					//direction.Normalize();

					//// Calculate the player's heading
					//float playerHeading2 = (heading / -512) * 360 + 180;

					//double angleInRadians = playerHeading2 * Math.PI / 180; // Convert the angle to radians
					//double x = Math.Cos(angleInRadians);
					//double y = Math.Sin(angleInRadians);

					////Vector3D playerHeading = new Vector3D(x, y, _PlayerLocation.Z);
					////playerHeading.Normalize();

					//direction2.X -= x;
					//direction2.Y -= y;

					//// Calculate the rotation axis
					//Vector3D rotationAxis = new Vector3D(1, 0, 1); //Vector3D.CrossProduct(direction, direction2);
					//rotationAxis.Normalize();

					//// Calculate the rotation angle
					//double rotationAngle = thetaDegrees; //Vector3D.AngleBetween(direction, direction2);

					//// Create the rotation matrix
					//Matrix3D rotationMatrix = new Matrix3D();
					//rotationMatrix.Rotate(new Quaternion(rotationAxis, rotationAngle));

					//// Apply the rotation to the transformation matrix
					//Matrix3D transformationMatrix = _POIArrowMatrix.Value * rotationMatrix;

					//_POIArrow.Content.Transform = new MatrixTransform3D(transformationMatrix);
				});
			}
		}

		#region Prebuilt Overlay Events

		private void PipeParser_ResistSpellEvent(object sender, ResistSpellParser.ResistSpellData e)
		{
			_appDispatcher.DispatchUI(() =>
			{
				var target = e.isYou ? "You " : "Your target ";
				var message = $"{target} resisted the {e.Spell.name} spell";
				SolidColorBrush overlayColor = new SolidColorBrush(_settings.ResistWarningOverlayColor);
				AddOverlayMessage(message, overlayColor);
			});
		}
		private void PipeParser_GroupInviteEvent(object sender, string e)
		{
			_appDispatcher.DispatchUI(() =>
			{
				SolidColorBrush overlayColor = new SolidColorBrush(_settings.GroupInviteOverlayColor);
				AddOverlayMessage(e, overlayColor);
			});
		}
		private void PipeParser_FailedFeignEvent(object sender, string e)
		{
			_appDispatcher.DispatchUI(() =>
			{
				SolidColorBrush overlayColor = new SolidColorBrush(_settings.FailedFeignOverlayColor);
				AddOverlayMessage("Feign Death Failed!", overlayColor);
			});
		}
		private void PipeParser_CharmBreakEvent(object sender, LogParser.CharmBreakArgs e)
		{
			_appDispatcher.DispatchUI(() =>
			{
				SolidColorBrush overlayColor = new SolidColorBrush(_settings.CharmBreakOverlayColor);
				AddOverlayMessage("Charm Break", overlayColor);
			});
		}
		private void PipeParser_InvisEvent(object sender, InvisParser.InvisStatus e)
		{
			_appDispatcher.DispatchUI(() =>
			{
				SolidColorBrush overlayColor = new SolidColorBrush(_settings.InvisFadingOverlayColor);
				AddOverlayMessage("Invis Fading", overlayColor);
			});
		}
		private void PipeParser_LevEvent(object sender, LevParser.LevStatus e)
		{
			_appDispatcher.DispatchUI(() =>
			{
				SolidColorBrush overlayColor = new SolidColorBrush(_settings.LevFadingOverlayColor);
				AddOverlayMessage("Levitate Fading", overlayColor);
			});
		}
		private void PipeParser_EnrageEvent(object sender, EnrageParser.EnrageEvent e)
		{
			_appDispatcher.DispatchUI(() =>
			{
				SolidColorBrush overlayColor = new SolidColorBrush(_settings.LevFadingOverlayColor);
				AddOverlayMessage("ENRAGED!", overlayColor);

				var t = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(12) };
				t.Tick += (s, e2) =>
				{
					AddOverlayMessage($"{e.NpcName} enrage faded", overlayColor);
					t.Stop();
				};
				t.Start();
			});
		}

		#endregion Prebuilt Overlay Events
	}
}