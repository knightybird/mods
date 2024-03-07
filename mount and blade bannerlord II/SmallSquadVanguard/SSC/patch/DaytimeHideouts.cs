using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem.CampaignBehaviors;

namespace SSC.patch
{
	// Token: 0x02000038 RID: 56
	[HarmonyPatch(typeof(HideoutCampaignBehavior), "IsHideoutAttackableNow")]
	internal class DaytimeHideouts
	{
		// Token: 0x060001E9 RID: 489 RVA: 0x0000AA59 File Offset: 0x00008C59
		public static void Postfix(HideoutCampaignBehavior __instance, ref bool __result)
		{
			__result = true;
		}
	}
}
