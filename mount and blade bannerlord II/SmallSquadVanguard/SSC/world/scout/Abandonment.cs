using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace SSC.world.scout
{
	// Token: 0x0200000E RID: 14
	internal static class Abandonment
	{
		// Token: 0x060000B9 RID: 185 RVA: 0x0000441D File Offset: 0x0000261D
		public static void PlayerCaptured(Hero leader)
		{
		}

		// Token: 0x060000BA RID: 186 RVA: 0x00004420 File Offset: 0x00002620
		public static void FactionsAtWar(Hero leader, MobileParty party)
		{
			string message = string.Format("{0} stopped following you, because your factions are now at war.", leader);
			Abandonment.VisualNotification(leader, message);
			MobileParty.MainParty.IgnoreByOtherPartiesTill(CampaignTime.HoursFromNow(5f));
		}

		// Token: 0x060000BB RID: 187 RVA: 0x00004454 File Offset: 0x00002654
		public static void LeaderCaptured(Hero leader, ScoutMission scoutData)
		{
			if (World.leadingSubordinate(scoutData))
			{
				return;
			}
			string arg = leader.IsFemale ? "she" : "he";
			string message = string.Format("Scout contract with {0} abandoned because {1} was captured.", leader, arg);
			Abandonment.VisualNotification(leader, message);
			scoutData.reputationChange(-20f);
			string message2 = string.Format("Your scout reputation decreased by 20 and is now {0}", scoutData.Reputation);
			Abandonment.VisualNotification(null, message2);
		}

		// Token: 0x060000BC RID: 188 RVA: 0x000044BC File Offset: 0x000026BC
		internal static void PartyLeaderKilled(Hero leader, ScoutMission scoutData)
		{
			if (World.leadingSubordinate(scoutData))
			{
				return;
			}
			string arg = leader.IsFemale ? "she" : "he";
			string message = string.Format("Scout contract with {0} abandoned because {1} was killed.", leader, arg);
			Abandonment.VisualNotification(leader, message);
			scoutData.reputationChange(-40f);
			string message2 = string.Format("Your scout reputation decreased by 40 and is now {0}", scoutData.Reputation);
			Abandonment.VisualNotification(null, message2);
		}

		// Token: 0x060000BD RID: 189 RVA: 0x00004524 File Offset: 0x00002724
		public static void PlayerDecision(ScoutMission scoutData)
		{
			if (!World.leadingSubordinate(scoutData))
			{
				scoutData.reputationChange(-1f);
				string message = string.Format("Scout contract with {0} abandoned by {1}", scoutData.Leader, Hero.MainHero.Name);
				Abandonment.VisualNotification(scoutData.Leader, message);
			}
		}

		// Token: 0x060000BE RID: 190 RVA: 0x0000456C File Offset: 0x0000276C
		internal static void ArmyDisbanded(Hero leader, MobileParty mobileParty)
		{
			string message = string.Format("{0}'s army was disbanded.", leader);
			Abandonment.VisualNotification(leader, message);
		}

		// Token: 0x060000BF RID: 191 RVA: 0x0000458C File Offset: 0x0000278C
		private static void VisualNotification(Hero leader, string message)
		{
			CharacterObject announcerCharacter = (leader != null) ? leader.CharacterObject : null;
			MBInformationManager.AddQuickInformation(new TextObject(message, null), 4000, announcerCharacter, "");
		}

		// Token: 0x0200007D RID: 125
		public enum Reason
		{
			// Token: 0x040000FF RID: 255
			Captured,
			// Token: 0x04000100 RID: 256
			PlayerKilled,
			// Token: 0x04000101 RID: 257
			FactionsAtWar,
			// Token: 0x04000102 RID: 258
			PlayerDecision,
			// Token: 0x04000103 RID: 259
			PlayerCaptured,
			// Token: 0x04000104 RID: 260
			ArmyDisbanded,
			// Token: 0x04000105 RID: 261
			PartyLeaderKilled
		}
	}
}
