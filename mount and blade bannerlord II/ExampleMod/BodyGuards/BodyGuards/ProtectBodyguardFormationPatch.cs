using System;
using HarmonyLib;
using TaleWorlds.MountAndBlade;

namespace Bodyguards
{
	// Token: 0x02000007 RID: 7
	[HarmonyPatch(typeof(Agent), "Formation", MethodType.Setter)]
	internal class ProtectBodyguardFormationPatch
	{
		// Token: 0x06000050 RID: 80 RVA: 0x00003444 File Offset: 0x00001644
		private static bool Prefix(Agent __instance, Formation value)
		{
			bool flag;
			if (__instance == null)
			{
				flag = (null != null);
			}
			else
			{
				Formation formation = __instance.Formation;
				flag = (((formation != null) ? formation.AI : null) != null);
			}
			bool flag2 = !flag || !__instance.IsActive() || Mission.Current == null || Mission.Current.IsMissionEnding;
			bool result;
			if (flag2)
			{
				result = true;
			}
			else
			{
				BehaviorProtectVIPAgent behavior = __instance.Formation.AI.GetBehavior<BehaviorProtectVIPAgent>();
				bool flag3 = behavior != null && !behavior.DisableThisBehaviorManually && behavior.IsAlive();
				result = !flag3;
			}
			return result;
		}
	}
}
