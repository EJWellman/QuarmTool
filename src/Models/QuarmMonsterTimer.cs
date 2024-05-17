namespace EQTool.Models
{
	public class QuarmMonsterTimer
	{
		public double Min_RespawnTimer { get; set; }
		public double Max_RespawnTimer { get; set; }
		public double RespawnTimer { get; set; }
		public string Mob_Name { get; set; }
		public string Zone_Code { get; set; }
		public string Zone_ID { get; set; }

		public QuarmMonsterTimer ShallowClone()
		{
			return (QuarmMonsterTimer)this.MemberwiseClone();
		}
	}

}
