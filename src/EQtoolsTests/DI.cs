﻿using Autofac;
using Autofac.Features.ResolveAnything;
using EQTool.Models;
using EQTool.Services;
using EQTool.ViewModels;
using EQToolShared.Discord;
using System.IO;

namespace EQToolTests
{
    public static class DI
    {
        public static IContainer Init()
        {
            var builder = new ContainerBuilder();
            _ = builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());
            _ = builder.Register(a =>
            {
                return new EQToolSettings
                {
                    DefaultEqDirectory = string.Empty,
                    EqLogDirectory = string.Empty,
                    Players = new System.Collections.Generic.List<PlayerInfo>(),
                    DpsWindowState = new WindowState
                    {
                        Closed = false,
                        State = System.Windows.WindowState.Normal
                    },
                    MapWindowState = new WindowState
                    {
                        Closed = false,
                        State = System.Windows.WindowState.Normal
                    },
                    MobWindowState = new WindowState
                    {
                        Closed = false,
                        State = System.Windows.WindowState.Normal
                    }
					//,
     //               SpellWindowState = new WindowState
     //               {
     //                   Closed = false,
     //                   State = System.Windows.WindowState.Normal
     //               }
                };
            }).AsSelf().SingleInstance();
            _ = builder.RegisterType<FakeAppDispatcher>().As<IAppDispatcher>().SingleInstance();
            _ = builder.RegisterType<SpellIcons>().AsSelf().SingleInstance();
            _ = builder.RegisterType<ParseSpells_spells_us>().AsSelf().SingleInstance();
            _ = builder.RegisterType<SettingsWindowViewModel>().AsSelf().SingleInstance();
            _ = builder.RegisterType<EQSpells>().AsSelf().SingleInstance();
            _ = builder.RegisterType<ActivePlayer>().AsSelf().SingleInstance();
            //_ = builder.RegisterType<SpellWindowViewModel>().AsSelf().SingleInstance();
            _ = builder.RegisterType<LogParser>().AsSelf().SingleInstance();
            _ = builder.RegisterType<DPSWindowViewModel>().AsSelf().SingleInstance();
            _ = builder.RegisterType<DiscordAuctionParse>().AsSelf().SingleInstance();

            var b = builder.Build();
            var settings = b.Resolve<EQToolSettings>();
            settings.DefaultEqDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            return b;
        }
    }
}
