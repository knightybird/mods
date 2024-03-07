using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;

namespace SSC.party
{
	// Token: 0x0200004F RID: 79
	public static class PlayerParty
	{
		// Token: 0x06000267 RID: 615 RVA: 0x0000DAF4 File Offset: 0x0000BCF4
		public static void forEachTroop(Action<TroopRosterElement> action)
		{
			foreach (TroopRosterElement obj in MobileParty.MainParty.MemberRoster.GetTroopRoster())
			{
				action(obj);
			}
		}
	}
}
