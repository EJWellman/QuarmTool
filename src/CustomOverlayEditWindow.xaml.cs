using EQTool.EventArgModels;
using EQTool.Models;
using EQTool.Services;
using EQTool.ViewModels;
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
	/// Interaction logic for CustomOverlayEditWindow.xaml
	/// </summary>
	public partial class CustomOverlayEditWindow : Window
	{
		private int ID { get; set; }

		public event EventHandler<CustomOverlayEditEventArgs> CustomOverlayEdited;

		public CustomOverlayEditWindow(int id)
		{
			InitializeComponent();
			ID = id;
			DataContext = CustomOverlayService.LoadCustomOverlayMessages().FirstOrDefault(x => x.ID == id);
		}

		private void SaveExistingCustomOverlay(object sender, RoutedEventArgs e)
		{
			var overlay = (CustomOverlay)DataContext;

			if (CustomOverlayService.UpdateCustomOverlay(overlay))
			{
				this.CustomOverlayEdited(this, new CustomOverlayEditEventArgs { Success = true, UpdatedOverlay = overlay });
				Close();
			}
			this.CustomOverlayEdited(this, new CustomOverlayEditEventArgs { Success = false });
        }
    }
}
