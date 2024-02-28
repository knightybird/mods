using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace BattleStats
{
	// Token: 0x02000003 RID: 3
	[XmlRoot("BattleStats")]
	public class BattleStatsXml
	{
		// Token: 0x04000003 RID: 3
		[XmlArrayItem("Hero")]
		public List<HeroRecords> Clan;

		// Token: 0x04000004 RID: 4
		[XmlArrayItem("Formation")]
		public List<ArmyRecords> Army;
	}
}
