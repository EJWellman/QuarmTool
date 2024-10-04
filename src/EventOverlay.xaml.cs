﻿using EQTool.EventArgModels;
using EQTool.Models;
using EQTool.Services;
using EQTool.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace EQTool
{
    public class ChainOverlayData : ChainData
    {
        public Canvas Canvas { get; set; }
        public List<FrameworkElement> ChildrenInRow { get; set; }
        public string TargetName { get; set; }
        public int ActiveAnimations { get; set; } = 0;
        public RowDefinition RowDefinition { get; set; }
        public System.Timers.Timer CHTimer { get; set; }
    }

    public partial class EventOverlay : BaseSaveStateWindow
    {
        private readonly LogParser logParser;
        private readonly EQToolSettings settings;
        private readonly ActivePlayer activePlayer;
        private readonly List<ChainOverlayData> chainDatas = new List<ChainOverlayData>();
        private readonly PigParseApi pigParseApi;
        private readonly IAppDispatcher appDispatcher;
		private BrushConverter converter = new BrushConverter();


		public EventOverlay(LogParser logParser, EQToolSettings settings, PigParseApi pigParseApi, EQToolSettingsLoad toolSettingsLoad, ActivePlayer activePlayer, IAppDispatcher appDispatcher)
            : base(settings.OverlayWindowState, toolSettingsLoad, settings)
        {
            this.pigParseApi = pigParseApi;
            this.appDispatcher = appDispatcher;
            this.activePlayer = activePlayer;
            this.settings = settings;
            this.logParser = logParser;
            InitializeComponent();
            base.Init();
            logParser.EnrageEvent += LogParser_EnrageEvent;
            logParser.CHEvent += LogParser_CHEvent;
            logParser.LevEvent += LogParser_LevEvent;
            logParser.InvisEvent += LogParser_InvisEvent;
            logParser.FTEEvent += LogParser_FTEEvent;
            logParser.CharmBreakEvent += LogParser_CharmBreakEvent;
            logParser.FailedFeignEvent += LogParser_FailedFeignEvent;
            logParser.GroupInviteEvent += LogParser_GroupInviteEvent;
            logParser.StartCastingEvent += LogParser_StartCastingEvent;
            logParser.SpellWornOtherOffEvent += LogParser_SpellWornOtherOffEvent;
            logParser.ResistSpellEvent += LogParser_ResistSpellEvent;
			logParser.CustomOverlayEvent += LogParser_CustomOverlayEvent;
        }


        private List<string> RootSpells = new List<string>()
        {
            "Root",
            "Fetter",
            "Enstill",
            "Immobalize",
            "Paralyzing Earth",
            "Grasping Roots",
            "Ensnaring Roots",
            "Enveloping Roots",
            "Engulfing Roots",
            "Engorging Roots",
            "Entrapping Roots"
        };

        private void LogParser_SpellWornOtherOffEvent(object sender, LogParser.SpellWornOffOtherEventArgs e)
        {
            var overlay = this.activePlayer?.Player?.RootWarningOverlay ?? false;
            if (!overlay)
            {
                return;
            }

            if (RootSpells.Any(a => string.Equals(a, e.SpellName, StringComparison.OrdinalIgnoreCase)))
            {
                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    this.appDispatcher.DispatchUI(() =>
					{
						CenterText.FontSize = settings.OverlayFontSize.Value;
						CenterText.Text = $"{e.SpellName} has worn off!";
                        CenterText.Foreground = (Brush)converter.ConvertFromString(settings.RootWarningOverlayColor.ToString());
					});
                    System.Threading.Thread.Sleep(3000);
                    this.appDispatcher.DispatchUI(() =>
                    {
                        CenterText.Text = string.Empty;
                        CenterText.Foreground = Brushes.Red;
                    });
                });
            }
        }

		private void LogParser_ResistSpellEvent(object sender, ResistSpellParser.ResistSpellData e)
		{
			var overlay = this.activePlayer?.Player?.ResistWarningOverlay ?? false;
			if (!overlay)
			{
				return;
			}

			System.Threading.Tasks.Task.Factory.StartNew(() =>
			{
				var target = e.isYou ? "You " : "Your target ";
				this.appDispatcher.DispatchUI(() =>
				{
					CenterText.FontSize = settings.OverlayFontSize.Value;
					CenterText.Text = $"{target} resisted the {e.Spell.name} spell";
					CenterText.Foreground = (Brush)converter.ConvertFromString(settings.ResistWarningOverlayColor.ToString());
				});
				System.Threading.Thread.Sleep(3000);
				this.appDispatcher.DispatchUI(() =>
				{
					CenterText.Text = string.Empty;
					CenterText.Foreground = Brushes.Red;
				});
			});
		}

		private void LogParser_GroupInviteEvent(object sender, string e)
        {
            var overlay = this.activePlayer?.Player?.GroupInviteOverlay ?? false;
            if (!overlay)
            {
                return;
            }

            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                this.appDispatcher.DispatchUI(() =>
				{
					CenterText.FontSize = settings.OverlayFontSize.Value;
					CenterText.Text = e;
                    CenterText.Foreground = (Brush)converter.ConvertFromString(settings.GroupInviteOverlayColor.ToString());
				});
                System.Threading.Thread.Sleep(1000 * 5);
                this.appDispatcher.DispatchUI(() =>
				{
					CenterText.Text = string.Empty;
                    CenterText.Foreground = Brushes.Red;
                });
            });
        }

        private void LogParser_FailedFeignEvent(object sender, string e)
        {
            var overlay = this.activePlayer?.Player?.FailedFeignOverlay ?? false;
            if (!overlay)
            {
                return;
            }

            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                this.appDispatcher.DispatchUI(() =>
				{
					CenterText.FontSize = settings.OverlayFontSize.Value;
					CenterText.Text = "Feign Death Failed!";
                    CenterText.Foreground = (Brush)converter.ConvertFromString(settings.FailedFeignOverlayColor.ToString());
				});
                System.Threading.Thread.Sleep(1000 * 5);
                this.appDispatcher.DispatchUI(() =>
                {
                    CenterText.Text = string.Empty;
                    CenterText.Foreground = Brushes.Red;
                });
            });
        }

        private void LogParser_CharmBreakEvent(object sender, LogParser.CharmBreakArgs e)
        {
            var overlay = this.activePlayer?.Player?.CharmBreakOverlay ?? false;
            if (!overlay)
            {
                return;
            }

            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                this.appDispatcher.DispatchUI(() =>
				{
					CenterText.FontSize = settings.OverlayFontSize.Value;
					CenterText.Text = "Charm Break";
                    CenterText.Foreground = (Brush)converter.ConvertFromString(settings.CharmBreakOverlayColor.ToString());
				});
                System.Threading.Thread.Sleep(1000 * 5);
                this.appDispatcher.DispatchUI(() =>
                {
                    CenterText.Text = string.Empty;
                    CenterText.Foreground = Brushes.Red;
                });
            });
        }

        private void LogParser_FTEEvent(object sender, FTEParser.FTEParserData e)
        {
            var overlay = this.activePlayer?.Player?.FTEOverlay ?? false;
            if (!overlay)
            {
                return;
            }

            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                var fteperson = this.pigParseApi.GetPlayerData(e.FTEPerson, this.activePlayer.Player.Server.Value);
                this.appDispatcher.DispatchUI(() =>
                {
                    if (fteperson == null)
					{
						CenterText.FontSize = settings.OverlayFontSize.Value;
						CenterText.Text = $"{e.FTEPerson} FTE {e.NPCName}";
                        CenterText.Foreground = (Brush)converter.ConvertFromString(settings.FTEOverlayColor.ToString());
                    }
                    else
					{
						CenterText.FontSize = settings.OverlayFontSize.Value;
						CenterText.Text = $"{fteperson.Name} <{fteperson.GuildName}> FTE {e.NPCName}";
						CenterText.Foreground = (Brush)converter.ConvertFromString(settings.FTEOverlayColor.ToString());
					}
                });
                System.Threading.Thread.Sleep(1000 * 5);
                this.appDispatcher.DispatchUI(() =>
                {
                    CenterText.Text = string.Empty;
                    CenterText.Foreground = Brushes.Yellow;
                });
            });
        }

        private void LogParser_InvisEvent(object sender, InvisParser.InvisStatus e)
        {
            var overlay = this.activePlayer?.Player?.InvisFadingOverlay ?? false;
            if (!overlay)
            {
                return;
            }

            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                this.appDispatcher.DispatchUI(() =>
				{
					CenterText.FontSize = settings.OverlayFontSize.Value;
					CenterText.Text = "Invis Fading";
                    CenterText.Foreground = (Brush)converter.ConvertFromString(settings.InvisFadingOverlayColor.ToString());
				});
                System.Threading.Thread.Sleep(1000 * 5);
                this.appDispatcher.DispatchUI(() =>
                {
                    CenterText.Text = string.Empty;
                    CenterText.Foreground = Brushes.Red;
                });
            });
        }

        private void LogParser_LevEvent(object sender, LevParser.LevStatus e)
        {
            var overlay = this.activePlayer?.Player?.LevFadingOverlay ?? false;
            if (!overlay)
            {
                return;
            }

            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                this.appDispatcher.DispatchUI(() =>
				{
					CenterText.FontSize = settings.OverlayFontSize.Value;
					CenterText.Text = "Levitate Fading";
                    CenterText.Foreground = (Brush)converter.ConvertFromString(settings.LevFadingOverlayColor.ToString());
				});
                System.Threading.Thread.Sleep(1000 * 5);
                this.appDispatcher.DispatchUI(() =>
                {
                    CenterText.Text = string.Empty;
                    CenterText.Foreground = Brushes.Red;
                });
            });
		}

		private void LogParser_EnrageEvent(object sender, EnrageParser.EnrageEvent e)
		{
			var overlay = this.activePlayer?.Player?.EnrageOverlay ?? false;
			if (!overlay)
			{
				return;
			}
			appDispatcher.DispatchUI(() =>
			{
				CenterText.FontSize = settings.OverlayFontSize.Value;
				CenterText.Text = e.NpcName + " ENRAGED";
				CenterText.Foreground = Brushes.Red;
			});

			System.Threading.Tasks.Task.Factory.StartNew(() =>
			{
				System.Threading.Thread.Sleep(1000 * 12);
				this.appDispatcher.DispatchUI(() =>
				{
					CenterText.FontSize = settings.OverlayFontSize.Value;
					CenterText.Text = "ENRAGE OFF";
					CenterText.Foreground = Brushes.Red;
				});
				System.Threading.Thread.Sleep(1000 * 3);
				this.appDispatcher.DispatchUI(() =>
				{
					CenterText.Text = string.Empty;
					CenterText.Foreground = Brushes.Red;
				});
			});
		}



		private void LogParser_StartCastingEvent(object sender, LogParser.SpellEventArgs e)
		{
			var overlay = this.activePlayer?.Player?.DragonRoarOverlay ?? false;
			if (!overlay || e.Spell.Spell.name != "Dragon Roar")
			{
				return;
			}

			System.Threading.Tasks.Task.Factory.StartNew(() =>
			{
				System.Threading.Thread.Sleep(1000 * 30);
				this.appDispatcher.DispatchUI(() =>
				{
					CenterText.FontSize = settings.OverlayFontSize.Value;
					CenterText.Text = "Dragon Roar in 6 Seconds!";
					CenterText.Foreground = (Brush)converter.ConvertFromString(settings.DragonRoarOverlayColor.ToString());
				});
				System.Threading.Thread.Sleep(1000);
				this.appDispatcher.DispatchUI(() =>
				{
					CenterText.FontSize = settings.OverlayFontSize.Value;
					CenterText.Text = "Dragon Roar in 5 Seconds!";
					CenterText.Foreground = (Brush)converter.ConvertFromString(settings.DragonRoarOverlayColor.ToString());
				});
				System.Threading.Thread.Sleep(1000);
				this.appDispatcher.DispatchUI(() =>
				{
					CenterText.FontSize = settings.OverlayFontSize.Value;
					CenterText.Text = "Dragon Roar in 4 Seconds!";
					CenterText.Foreground = (Brush)converter.ConvertFromString(settings.DragonRoarOverlayColor.ToString());
				});
				System.Threading.Thread.Sleep(1000);
				this.appDispatcher.DispatchUI(() =>
				{
					CenterText.FontSize = settings.OverlayFontSize.Value;
					CenterText.Text = "Dragon Roar in 3 Seconds!";
					CenterText.Foreground = (Brush)converter.ConvertFromString(settings.DragonRoarOverlayColor.ToString());
				});
				System.Threading.Thread.Sleep(1000);
				this.appDispatcher.DispatchUI(() =>
				{
					CenterText.FontSize = settings.OverlayFontSize.Value;
					CenterText.Text = "Dragon Roar in 2 Seconds!";
					CenterText.Foreground = (Brush)converter.ConvertFromString(settings.DragonRoarOverlayColor.ToString());
				});
				System.Threading.Thread.Sleep(1000);
				this.appDispatcher.DispatchUI(() =>
				{
					CenterText.FontSize = settings.OverlayFontSize.Value;
					CenterText.Text = "Dragon Roar in 1 Seconds!";
					CenterText.Foreground = (Brush)converter.ConvertFromString(settings.DragonRoarOverlayColor.ToString());
				});
				System.Threading.Thread.Sleep(1000);
				this.appDispatcher.DispatchUI(() =>
				{
					CenterText.Text = string.Empty;
					CenterText.Foreground = Brushes.Red;
				});
			});
		}

		private void LogParser_CHEvent(object sender, ChParser.ChParseData e)
        {
            var overlay = this.activePlayer?.Player?.ChChainOverlay ?? false;
            var warningoverlay = this.activePlayer?.Player?.ChChainWarningOverlay ?? false;
            if (!overlay && !warningoverlay)
            {
                return;
            }

            appDispatcher.DispatchUI(() =>
            {
                var chaindata = this.GetOrCreateChain(e.Recipient);
                if (warningoverlay)
                {
                    var shouldwarn = CHService.ShouldWarnOfChain(chaindata, e);
                    if (shouldwarn)
                    {
                        System.Threading.Tasks.Task.Factory.StartNew(() =>
                        {
                            this.appDispatcher.DispatchUI(() =>
							{
								CenterText.FontSize = settings.OverlayFontSize.Value;
								CenterText.Text = "CH Chain Warning";
                                CenterText.Foreground = Brushes.Red;
                            });
                            System.Threading.Thread.Sleep(1000 * 2);
                            this.appDispatcher.DispatchUI(() =>
                            {
                                CenterText.Text = string.Empty;
                                CenterText.Foreground = Brushes.Red;
                            });
                        });
                    }
                }

                var random = new Random(DateTime.Now.Millisecond);
                var color = System.Windows.Media.Color.FromRgb(
                    (byte)random.Next(0, 40),
                    (byte)random.Next(0, 40),
                    (byte)random.Next(0, 40));
                var colorA = System.Windows.Media.Color.FromArgb(140, color.R, color.G, color.B);
                var targetwidth = chaindata.Canvas.ActualWidth / 10.0;
                var target = new TextBlock
                {
                    FontSize = settings.FontSize.Value * 2,
                    Text = e.Position.ToString(),
                    Foreground = Brushes.White,
                    TextAlignment = TextAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };
                var textborder = new Border { Height = 28, Width = targetwidth, Background = Brushes.ForestGreen, BorderBrush = Brushes.Black, BorderThickness = new Thickness(1) };
                textborder.Child = target;
                DoubleAnimation animation = new DoubleAnimation();
                animation.From = -targetwidth;
                animation.To = chaindata.Canvas.ActualWidth;
                animation.Duration = TimeSpan.FromSeconds(11);

                Storyboard.SetTarget(animation, textborder);
                Storyboard.SetTargetProperty(animation, new PropertyPath(Canvas.RightProperty));

                Storyboard storyboard = new Storyboard();
                storyboard.Children.Add(animation);
                chaindata.Canvas.Children.Add(textborder);

                storyboard.Completed += (s, ev) =>
                {
                    chaindata.ActiveAnimations--;
                    appDispatcher.DispatchUI(() =>
                    {
                        chaindata.Canvas.Children.Remove(textborder);
                    });
                    if (chaindata.ActiveAnimations <= 0)
                    {
                        System.Threading.Tasks.Task.Factory.StartNew(() =>
                        {
                            System.Threading.Thread.Sleep(5000);
                            appDispatcher.DispatchUI(() =>
                            {
                                if (chaindata.ActiveAnimations <= 0)
                                {
                                    var rowremoved = Grid.GetRow(chaindata.ChildrenInRow.FirstOrDefault());
                                    Debug.WriteLine($"Removing Row {rowremoved}");
                                    this.chainDatas.Remove(chaindata);
                                    foreach (var item in chaindata.ChildrenInRow)
                                    {
                                        this.ChainStackPanel.Children.Remove(item);
                                    }
                                    this.ChainStackPanel.RowDefinitions.Remove(chaindata.RowDefinition);
                                    foreach (var item in this.chainDatas)
                                    {
                                        foreach (var cell in item.ChildrenInRow)
                                        {
                                            var itemrow = Grid.GetRow(cell);
                                            if (itemrow > rowremoved)
                                            {
                                                Debug.WriteLine($"Updating Row {itemrow} to {itemrow - 1}");
                                                Grid.SetRow(cell, itemrow - 1);
                                            }
                                        }
                                    }
                                }
                            });
                        });
                    }
                };
                storyboard.Begin();
            });
        }

        private ChainOverlayData GetOrCreateChain(string targetname)
        {
            var chaindata = this.chainDatas.FirstOrDefault(a => a.TargetName == targetname);
            if (chaindata != null)
            {
                chaindata.ActiveAnimations += 1;
                return chaindata;
            }
            chaindata = new ChainOverlayData
            {
                Canvas = new Canvas(),
                ChildrenInRow = new List<FrameworkElement>(),
                TargetName = targetname,
                ActiveAnimations = 1,
                RowDefinition = new RowDefinition { MaxHeight = 30 }
                //CHTimer = new System.Timers.Timer(1000)
            };

            //chaindata.CHTimer.Elapsed += (a, b) =>
            //{

            //};
            //chaindata.CHTimer.Enabled = true;
            chaindata.Canvas.IsHitTestVisible = false;
            chaindata.Canvas.Background = Brushes.Transparent;
            var target = new TextBlock
            {
                Height = 30,
                FontSize = settings.FontSize.Value * 1.5,
                Text = targetname,
                Padding = new Thickness(4),
                Foreground = Brushes.Black,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            var textborder = new Border { CornerRadius = new CornerRadius(3), Background = Brushes.Chocolate, BorderThickness = new Thickness(2), BorderBrush = Brushes.Black };
            textborder.Child = target;
            var getrow = this.ChainStackPanel.RowDefinitions.Count;
            Debug.WriteLine($"Adding row {getrow}");
            Grid.SetRow(textborder, getrow);
            Grid.SetZIndex(textborder, 1);
            Grid.SetColumn(textborder, 0);
            Grid.SetRow(chaindata.Canvas, getrow);
            Grid.SetColumn(chaindata.Canvas, 1);
            Grid.SetZIndex(chaindata.Canvas, 0);
            var stackpanel = new StackPanel { Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(200, 10, 10, 10)), Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Stretch, Height = 30 };
            Grid.SetRow(stackpanel, getrow);
            Grid.SetColumn(stackpanel, 1);
            this.ChainStackPanel.RowDefinitions.Add(chaindata.RowDefinition);
            this.ChainStackPanel.Children.Add(textborder);
            this.ChainStackPanel.Children.Add(stackpanel);
            this.ChainStackPanel.Children.Add(chaindata.Canvas);
            chaindata.ChildrenInRow.Add(textborder);
            chaindata.ChildrenInRow.Add(stackpanel);
            chaindata.ChildrenInRow.Add(chaindata.Canvas);
            this.chainDatas.Add(chaindata);
            chaindata.Canvas.UpdateLayout();
            for (var i = 0; i < 10; i++)
            {
                stackpanel.Children.Add(new Border
                {
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Height = 30,
                    Width = chaindata.Canvas.ActualWidth / 10,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    BorderBrush = Brushes.WhiteSmoke,
                    BorderThickness = new Thickness(1, 0, 1, 2),
                    Child = new TextBlock
                    {
                        Text = (i + 1).ToString(),
                        Foreground = Brushes.Red,
                        VerticalAlignment = VerticalAlignment.Center,
                        TextAlignment = TextAlignment.Center,
                        Width = chaindata.Canvas.ActualWidth / 10,
                        HorizontalAlignment = HorizontalAlignment.Stretch
                    }
                });

            }
            return chaindata;
        }

		private void LogParser_CustomOverlayEvent(object sender, CustomOverlayEventArgs e)
		{
			var overlay = e.CustomOverlay;
			if (!overlay.IsEnabled)
			{
				return;
			}
			appDispatcher.DispatchUI(() =>
			{
				CenterText.FontSize = settings.OverlayFontSize.Value;
				CenterText.Text = overlay.Message;
				CenterText.Foreground = (Brush)converter.ConvertFromString(overlay.DisplayColor);
			});

			System.Threading.Tasks.Task.Factory.StartNew(() =>
			{
				System.Threading.Thread.Sleep(1000 * 3);
				this.appDispatcher.DispatchUI(() =>
				{
					CenterText.Text = string.Empty;
					CenterText.Foreground = Brushes.Red;
				});
			});
		}

		protected override void OnClosing(CancelEventArgs e)
        {
            if (logParser != null)
            {
                logParser.EnrageEvent -= LogParser_EnrageEvent;
                logParser.CHEvent -= LogParser_CHEvent;
                logParser.LevEvent -= LogParser_LevEvent;
                logParser.InvisEvent -= LogParser_InvisEvent;
                logParser.FTEEvent -= LogParser_FTEEvent;
                logParser.CharmBreakEvent -= LogParser_CharmBreakEvent;
                logParser.FailedFeignEvent -= LogParser_FailedFeignEvent;
                logParser.GroupInviteEvent -= LogParser_GroupInviteEvent;
                logParser.StartCastingEvent -= LogParser_StartCastingEvent;
                logParser.SpellWornOtherOffEvent -= LogParser_SpellWornOtherOffEvent;
                logParser.ResistSpellEvent -= LogParser_ResistSpellEvent;
				logParser.CustomOverlayEvent -= LogParser_CustomOverlayEvent;
            }

            base.OnClosing(e);
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            WindowResizeChrome.ResizeBorderThickness = new Thickness(8);
            WindowBorder.BorderThickness = new Thickness(1, 1, 1, 1);
            LastWindowInteraction = DateTime.UtcNow;
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                while (Math.Abs((DateTime.UtcNow - LastWindowInteraction).TotalSeconds) < 10)
                {
                    System.Threading.Thread.Sleep(1000 * 1);
                }
                this.appDispatcher.DispatchUI(() =>
                {
                    WindowResizeChrome.ResizeBorderThickness = new Thickness(0);
                    WindowBorder.BorderThickness = new Thickness(0, 0, 0, 0);
                });
            });
        }
    }
}
