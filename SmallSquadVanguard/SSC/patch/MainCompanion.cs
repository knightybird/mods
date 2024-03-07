using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem;

namespace SSC.patch
{
	// Token: 0x02000039 RID: 57
	[HarmonyPatch(typeof(Clan), "CompanionLimit", MethodType.Getter)]
	internal class MainCompanion
	{
		// Token: 0x060001EB RID: 491 RVA: 0x0000AA66 File Offset: 0x00008C66
		public static void Postfix(Clan __instance, ref int __result)
		{
			if (__instance.Leader.IsHumanPlayerCharacter)
			{
				__result = World.CompanionLimit.Value;
			}
		}
	}
}
