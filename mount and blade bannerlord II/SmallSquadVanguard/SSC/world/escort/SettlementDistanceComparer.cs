using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;

namespace SSC.world.escort
{
	// Token: 0x02000025 RID: 37
	internal class SettlementDistanceComparer : IComparer<Settlement>
	{
		// Token: 0x060001A1 RID: 417 RVA: 0x000093E7 File Offset: 0x000075E7
		public SettlementDistanceComparer(Settlement settlement)
		{
			this.settlement = settlement;
		}

		// Token: 0x060001A2 RID: 418 RVA: 0x000093F8 File Offset: 0x000075F8
		public int Compare(Settlement s1, Settlement s2)
		{
			float distance = Campaign.Current.Models.MapDistanceModel.GetDistance(this.settlement, s1);
			float distance2 = Campaign.Current.Models.MapDistanceModel.GetDistance(this.settlement, s2);
			return distance.CompareTo(distance2);
		}

		// Token: 0x0400008B RID: 139
		private Settlement settlement;
	}
}
