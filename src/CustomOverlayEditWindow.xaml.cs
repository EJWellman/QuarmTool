using EQTool.EventArgModels;
using EQTool.Models;
using EQTool.Services;
using System;
using System.Linq;
using System.Windows;

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
