using EQTool.Models;
using EQTool.Services;
using System;
using System.Web;

namespace EQTool
{
	/// <summary>
	/// Interaction logic for webTest.xaml
	/// </summary>
	public partial class WebTest2 : BaseSaveStateWindow
	{
		EQToolSettings _settings;
		public WebTest2(
			EQToolSettings settings,
			EQToolSettingsLoad toolSettingsLoad) : base(settings.WebViewWindowState, toolSettingsLoad, settings)
		{
			_settings = settings;
			InitializeComponent();
		}

		public override void EndInit()
		{
			_settings.DiscordUrl = _settings.DiscordUrl.Substring(1, _settings.DiscordUrl.Length - 2);

			Uri uri = new UriBuilder(_settings.DiscordUrl.Replace("\\\"", "").Replace("\"\\", "")).Uri;
			webViewer.Source = uri;
			base.EndInit();
		}

		public void SetUrl(string url)
		{
			webViewer.Source = new System.Uri(url);
		}
	}
}