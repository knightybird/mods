using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace SSC.world.escort
{
	// Token: 0x02000026 RID: 38
	internal class SettlementToPartyDistanceComparer : IComparer<Settlement>
	{
		// Token: 0x060001A3 RID: 419 RVA: 0x00009445 File Offset: 0x00007645
		public SettlementToPartyDistanceComparer(MobileParty party)
		{
			this.party = party;
		}

		// Token: 0x060001A4 RID: 420 RVA: 0x00009454 File Offset: 0x00007654
		public int Compare(Settlement s1, Settlement s2)
		{
			float distance = Campaign.Current.Models.MapDistanceModel.GetDistance(this.party, s1);
			float distance2 = Campaign.Current.Models.MapDistanceModel.GetDistance(this.party, s2);
			return distance.CompareTo(distance2);
		}

		// Token: 0x0400008C RID: 140
		private MobileParty party;
	}
}
