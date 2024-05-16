using Dapper.Contrib.Extensions;
using System.Windows.Media;

namespace EQTool.Models
{
	[Table("CustomOverlays")]
	public class CustomOverlay
	{
		public int ID { get; set; }
		public string Trigger { get; set; }
		public string Alternate_Trigger { get; set; }
		public string Name { get; set; }
		public string Message { get; set; }
		[Write(false)]
		public System.Windows.Media.Color Color { get; set; }
		public string DisplayColor
		{
			get
			{
				return Color.ToString();
			}
			set
			{
				this.Color = (System.Windows.Media.Color)ColorConverter.ConvertFromString(value);
			}
		}
		public bool IsEnabled { get; set; }
		public bool IsAudioEnabled { get; set; }
		public string AudioMessage { get; set; }

		public CustomOverlay ShallowClone()
		{
			return (CustomOverlay)this.MemberwiseClone();
		}
	}
}
