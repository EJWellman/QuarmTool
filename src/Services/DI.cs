using Autofac;
using Autofac.Features.ResolveAnything;
using EQTool.Models;
using EQTool.ViewModels;
using ZealPipes.Common;
using ZealPipes.Services;
using ZealPipes.Services.Helpers;

namespace EQTool.Services
{
    public static class DI
    {
        public static IContainer Init()
        {
            var builder = new ContainerBuilder();
            builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());
            builder.RegisterType<EQToolSettingsLoad>().AsSelf().SingleInstance();
            builder.Register(a =>
            {
                return a.Resolve<EQToolSettingsLoad>().Load();
            }).AsSelf().SingleInstance();
            builder.RegisterType<AppDispatcher>().As<IAppDispatcher>().SingleInstance();
            builder.RegisterType<SpellIcons>().AsSelf().SingleInstance();
            builder.RegisterType<ParseSpells_spells_us>().AsSelf().SingleInstance();
            builder.RegisterType<SettingsWindowViewModel>().AsSelf().SingleInstance();
            builder.RegisterType<EQSpells>().AsSelf().SingleInstance();
            builder.RegisterType<ActivePlayer>().AsSelf().SingleInstance();
            builder.RegisterType<SpellWindowViewModel>().AsSelf().SingleInstance();
            builder.RegisterType<LogParser>().AsSelf().SingleInstance();
            builder.RegisterType<DPSWindowViewModel>().AsSelf().SingleInstance();
            builder.RegisterType<ZoneViewModel>().AsSelf().SingleInstance();
            builder.RegisterType<SessionPlayerDamage>().AsSelf().SingleInstance();
            builder.RegisterType<LoggingService>().AsSelf().SingleInstance();
            builder.RegisterType<PlayerTrackerService>().AsSelf().SingleInstance();
            builder.RegisterType<ZoneActivityTrackingService>().AsSelf().SingleInstance();
            builder.RegisterType<TimersService>().AsSelf().SingleInstance();
            builder.RegisterType<SignalrPlayerHub>().As<ISignalrPlayerHub>().SingleInstance();
            builder.RegisterType<AudioService>().AsSelf().SingleInstance();
			builder.RegisterType<QuarmDataService>().AsSelf().SingleInstance();
			builder.RegisterType<CustomOverlayService>().AsSelf().SingleInstance();
			//Zeal Pipes
			builder.RegisterType<ProcessMonitor>().AsSelf().SingleInstance();
			builder.RegisterType<ZealPipeReader>().AsSelf().SingleInstance();
			builder.RegisterType<ZealSettings>().AsSelf().SingleInstance();
			builder.Register(ctx =>
			{
				var settings = ctx.Resolve<ZealSettings>();
				var processMonitor = ctx.Resolve<ProcessMonitor>();
				ZealPipeReader pipeReader = ctx.Resolve<ZealPipeReader>();
				return new ZealMessageService(processMonitor, settings, pipeReader);
			}).As<ZealMessageService>().SingleInstance();
			builder.RegisterType<ZealMessageService>().AsSelf().SingleInstance();


            return builder.Build();
        }
    }
}
