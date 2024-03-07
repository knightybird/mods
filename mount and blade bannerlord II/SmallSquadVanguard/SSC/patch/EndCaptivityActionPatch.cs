using System;
using HarmonyLib;
using SSC.party;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;

namespace SSC.patch
{
	// Token: 0x02000036 RID: 54
	[HarmonyPatch(typeof(EndCaptivityAction))]
	public static class EndCaptivityActionPatch
	{
		// Token: 0x060001E6 RID: 486 RVA: 0x0000AA20 File Offset: 0x00008C20
		[HarmonyPrefix]
		[HarmonyPatch("ApplyInternal")]
		public static bool ApplyInternal(Hero prisoner, EndCaptivityDetail detail, Hero facilitatior = null)
		{
			if (Companion.isCustomCompanion(prisoner))
			{
				PartyBase partyBelongedToAsPrisoner = prisoner.PartyBelongedToAsPrisoner;
				if (partyBelongedToAsPrisoner != null)
				{
					World.CaptorsHack[prisoner] = partyBelongedToAsPrisoner;
				}
			}
			return true;
		}
	}
}
