using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using SSC.story;
using StoryMode.GameComponents.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Party;

namespace SSC.patch
{
	// Token: 0x0200003B RID: 59
	public static class HarmonyLibrary
	{
		// Token: 0x060001F1 RID: 497 RVA: 0x0000AAAE File Offset: 0x00008CAE
		public static void applyRegularPatches()
		{
			if (HarmonyLibrary.applied)
			{
				return;
			}
			new Harmony(HarmonyLibrary.harmony_id).PatchAll();
			HarmonyLibrary.applied = true;
		}

		// Token: 0x060001F2 RID: 498 RVA: 0x0000AAD0 File Offset: 0x00008CD0
		public static void patchInfo()
		{
			foreach (MethodBase methodBase in new Harmony(HarmonyLibrary.harmony_id).GetPatchedMethods())
			{
			}
		}

		// Token: 0x060001F3 RID: 499 RVA: 0x0000AB20 File Offset: 0x00008D20
		public static void printHarmonyInfo()
		{
			Version version;
			foreach (KeyValuePair<string, Version> keyValuePair in Harmony.VersionInfo(out version))
			{
				string key = keyValuePair.Key;
				Version value = keyValuePair.Value;
			}
		}

		// Token: 0x060001F4 RID: 500 RVA: 0x0000AB80 File Offset: 0x00008D80
		public static void applyOnSettlementLeftPatch()
		{
			Harmony harmony = new Harmony(HarmonyLibrary.harmony_id);
			MethodInfo method = typeof(FirstPhaseCampaignBehavior).GetMethod("OnSettlementLeft", BindingFlags.Instance | BindingFlags.NonPublic);
			MethodInfo method2 = typeof(StoryModePatches).GetMethod("OnSettlementLeft_Prefix");
			harmony.Patch(method, new HarmonyMethod(method2), null, null, null);
		}

		// Token: 0x060001F5 RID: 501 RVA: 0x0000ABD4 File Offset: 0x00008DD4
		public static void RestrictPartySize(bool enable)
		{
			if (HarmonyLibrary.partySizePatchApplied == enable)
			{
				return;
			}
			Harmony harmony = new Harmony(HarmonyLibrary.harmony_id);
			MethodInfo getMethod = typeof(PartyBase).GetProperty("PartySizeLimit", BindingFlags.Instance | BindingFlags.Public).GetGetMethod();
			MethodInfo method = typeof(PartyBase_PartySizeLimit).GetMethod("PartySizeLimit_Postfix");
			if (enable)
			{
				harmony.Patch(getMethod, null, new HarmonyMethod(method), null, null);
			}
			else
			{
				harmony.Unpatch(getMethod, HarmonyPatchType.Postfix, HarmonyLibrary.harmony_id);
			}
			HarmonyLibrary.partySizePatchApplied = enable;
		}

		// Token: 0x040000A3 RID: 163
		public static string harmony_id = "small_squad_vanguard";

		// Token: 0x040000A4 RID: 164
		private static bool applied = false;

		// Token: 0x040000A5 RID: 165
		private static bool partySizePatchApplied = false;
	}
}
