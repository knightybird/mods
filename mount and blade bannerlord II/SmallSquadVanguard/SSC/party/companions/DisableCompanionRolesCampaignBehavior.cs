using System;
using HarmonyLib;
using SandBox.CampaignBehaviors;

namespace SSC.party.companions
{
	// Token: 0x02000069 RID: 105
	[HarmonyPatch(typeof(CompanionRolesCampaignBehavior))]
	public static class DisableCompanionRolesCampaignBehavior
	{
		// Token: 0x06000333 RID: 819 RVA: 0x0001254F File Offset: 0x0001074F
		[HarmonyPrefix]
		[HarmonyPatch(typeof(CompanionRolesCampaignBehavior), "RegisterEvents")]
		public static bool Prefix()
		{
			return false;
		}

		// Token: 0x06000334 RID: 820 RVA: 0x00012552 File Offset: 0x00010752
		[HarmonyPrefix]
		[HarmonyPatch(typeof(CompanionRolesCampaignBehavior), "AddDialogs")]
		public static bool AddDialogsPrefix()
		{
			return false;
		}

		// Token: 0x06000335 RID: 821 RVA: 0x00012555 File Offset: 0x00010755
		[HarmonyPrefix]
		[HarmonyPatch(typeof(CompanionRolesCampaignBehavior), "SyncData")]
		public static bool SyncDataPrefix()
		{
			return false;
		}
	}
}
