using System;
using System.Linq;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Roster;

namespace SSC.world.bandits
{
	// Token: 0x0200002C RID: 44
	[HarmonyPatch(typeof(CampaignMission), "OpenHideoutBattleMission")]
	public static class StartHideoutBattlePatch
	{
		// Token: 0x060001D0 RID: 464 RVA: 0x0000A3F2 File Offset: 0x000085F2
		public static void Prefix(string scene, FlattenedTroopRoster playerTroops)
		{
			World.HideoutPlayerPartyCountAtStart = playerTroops.Troops.Count<CharacterObject>();
		}
	}
}
