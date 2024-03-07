using System;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace SSC.world.scout
{
	// Token: 0x02000017 RID: 23
	internal static class ScoutRewards
	{
		// Token: 0x06000131 RID: 305 RVA: 0x00006684 File Offset: 0x00004884
		public static void followerGainsRenown(int renownGain, ScoutRewards.Reason reason, ScoutMission scoutData)
		{
			if (World.leadingSubordinate(scoutData))
			{
				return;
			}
			int num = renownGain;
			if (reason == ScoutRewards.Reason.SiegeAssault)
			{
				num = renownGain / 2;
			}
			else if (reason == ScoutRewards.Reason.Raid)
			{
				num = renownGain / 2;
			}
			else if (reason == ScoutRewards.Reason.BanditsEncounter)
			{
				num = renownGain / 2;
			}
			scoutData.reputationChange((float)num);
			Hero leader = scoutData.Leader;
			if (reason != ScoutRewards.Reason.Raid)
			{
				ScoutRewards.applyRelation(leader, num);
				ScoutRewards.notifications(num, scoutData);
			}
			int val = (int)Math.Ceiling((double)((float)num * scoutData.Reputation));
			int val2 = num * 100;
			GiveGoldAction.ApplyBetweenCharacters(leader, Hero.MainHero, Math.Max(val, val2), false);
		}

		// Token: 0x06000132 RID: 306 RVA: 0x00006704 File Offset: 0x00004904
		private static void applyRelation(Hero hero, int points)
		{
			int num = points / 2;
			int relation = points - num;
			ChangeRelationAction.ApplyPlayerRelation(hero, num, true, false);
			ChangeRelationAction.ApplyPlayerRelation(hero, relation, false, false);
		}

		// Token: 0x06000133 RID: 307 RVA: 0x0000672C File Offset: 0x0000492C
		private static void notifications(int change, ScoutMission scoutData)
		{
			MBInformationManager.AddQuickInformation(new TextObject((change > 0) ? string.Format("Gained {0} relation with {1}", change, scoutData.Leader) : string.Format("{0} relation with {1}", -change, scoutData.Leader), null), 3000, PartyBaseHelper.GetVisualPartyLeader(scoutData.MobileParty.Party), "");
			MBInformationManager.AddQuickInformation(new TextObject((change > 0) ? string.Format("Your scout reputation increased by {0} and is now {1}", change, scoutData.Reputation) : string.Format("Your scout reputation decreased with {0} and is now {1}", -change, scoutData.Reputation), null), 3000, null, "");
		}

		// Token: 0x02000084 RID: 132
		public enum Reason
		{
			// Token: 0x04000127 RID: 295
			FieldBattle,
			// Token: 0x04000128 RID: 296
			Raid,
			// Token: 0x04000129 RID: 297
			SiegeAssault,
			// Token: 0x0400012A RID: 298
			BanditsEncounter
		}
	}
}
