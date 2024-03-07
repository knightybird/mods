using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Siege;

namespace SSC.patch
{
	// Token: 0x02000037 RID: 55
	[HarmonyPatch(typeof(DefaultTroopSacrificeModel), "GetLostTroopCountForBreakingInBesiegedSettlement")]
	internal class BreakInBesiegedSettlement
	{
		// Token: 0x060001E7 RID: 487 RVA: 0x0000AA4C File Offset: 0x00008C4C
		public static void Postfix(MobileParty party, SiegeEvent siegeEvent, ref int __result)
		{
			__result = 0;
		}
	}
}
