using System;
using HarmonyLib;
using SSC.misc;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace SSC.patch
{
	// Token: 0x02000034 RID: 52
	[HarmonyPatch(typeof(PartiesBuyHorseCampaignBehavior), "OnSettlementEntered")]
	public static class BuyHorsesPatch
	{
		// Token: 0x060001E5 RID: 485 RVA: 0x0000A9FF File Offset: 0x00008BFF
		[HarmonyPrefix]
		public static bool SettlementEntered_Prefix(MobileParty mobileParty, Settlement settlement, Hero hero)
		{
			return mobileParty == null || mobileParty != World.ScoutMission_LedParty || !settlement.IsTown || !World.isFlagSet(Flags.ScoutMission_BuyHorsesMode);
		}
	}
}
