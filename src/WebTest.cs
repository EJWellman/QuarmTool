using EQTool.Models;
using EQTool.Services;
using EQTool.ViewModels;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EQTool
{
	/// <summary>
	/// Interaction logic for webTest.xaml
	/// </summary>
	public partial class WebTest : BaseSaveStateWindow
	{
		private EQToolSettings _settings;
		public WebTest(
			EQToolSettings settings,
			EQToolSettingsLoad toolSettingsLoad) : base(settings.WebViewWindowState, toolSettingsLoad, settings)
		{
			_settings = settings;
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			string url = webViewer.ExecuteScriptAsync("document.querySelector(\"input[type='text'][readonly]\").value").Result;
			_settings.DiscordSettings.DiscordUrl = url.Substring(1, url.Length - 2);
		}
	}
}
