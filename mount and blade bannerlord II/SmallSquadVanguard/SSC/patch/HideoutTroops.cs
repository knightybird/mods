using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using SSC.misc;
using SSC.party;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace SSC.patch
{
	// Token: 0x0200003C RID: 60
	[HarmonyPatch(typeof(HideoutCampaignBehavior), "ArrangeHideoutTroopCountsForMission")]
	internal class HideoutTroops
	{
		// Token: 0x060001F7 RID: 503 RVA: 0x0000AC68 File Offset: 0x00008E68
		public static bool Prefix(HideoutCampaignBehavior __instance)
		{
			try
			{
				if ((from x in Settlement.CurrentSettlement.Parties
				where x.IsBandit && !x.IsBanditBossParty
				select x).Count<MobileParty>() < 1)
				{
					HideoutTroops.missingBossHandler();
					return true;
				}
				MBList<MobileParty> parties = (from x in Settlement.CurrentSettlement.Parties
				where x.IsBandit
				select x).ToMBList<MobileParty>();
				MBList<MobileParty> mblist = (from x in Settlement.CurrentSettlement.Parties
				where x.IsBanditBossParty
				select x).ToMBList<MobileParty>();
				HideoutTroops.bossId = HideoutTroops.getTheBoss(mblist[0]).Character.StringId;
				List<TroopRosterElement> troopTypes = HideoutTroops.getTroopTypes(parties);
				MBList<MobileParty> parties2 = (from x in Settlement.CurrentSettlement.Parties
				where x.IsBandit && !x.IsBanditBossParty
				select x).ToMBList<MobileParty>();
				HideoutTroops.setTierTargets(parties2, World.FirstPhaseTiers.Value, troopTypes);
				if ((from x in Settlement.CurrentSettlement.Parties
				where x.IsBanditBossParty
				select x).Count<MobileParty>() < 1)
				{
					HideoutTroops.missingBossHandler();
					return true;
				}
				HideoutTroops.setTierTargets(mblist, World.BossPhaseTiers.Value, troopTypes);
				World.HideoutFirstFightMaximum = HideoutTroops.troopCount(parties2);
				int num = HideoutTroops.tierCount(parties2);
				int num2 = HideoutTroops.tierCount(mblist);
				World.HideoutTiersAtStart = num + num2;
			}
			catch (Exception e)
			{
				World.queueAction(HideoutTroops.constructCrashHandler(e));
				return true;
			}
			return false;
		}

		// Token: 0x060001F8 RID: 504 RVA: 0x0000AE28 File Offset: 0x00009028
		private static int troopCount(MBList<MobileParty> parties)
		{
			int num = 0;
			foreach (MobileParty mobileParty in parties)
			{
				num += mobileParty.MemberRoster.TotalHealthyCount;
			}
			return num;
		}

		// Token: 0x060001F9 RID: 505 RVA: 0x0000AE80 File Offset: 0x00009080
		private static QueuedAction constructCrashHandler(Exception e)
		{
			HideoutTroops.HideoutCrashMessage = World.MOD_TITLE + " was unable to construct the bandit forces for the last hideout fight, so the vanilla fight was let to run instead. \n\n " + e.Message + " \n\n Please tell the mod developer for this issue.";
			QueuedAction queuedAction = new QueuedAction();
			queuedAction.Condition = (() => HideoutTroops.HideoutCrashMessage != null);
			queuedAction.Obsolete = (() => HideoutTroops.HideoutCrashMessage == null);
			queuedAction.ActionToPerform = delegate()
			{
				Message.ShowDialog(HideoutTroops.HideoutCrashMessage);
				HideoutTroops.HideoutCrashMessage = null;
			};
			queuedAction.CheckOnNextTick = (() => true);
			return queuedAction;
		}

		// Token: 0x060001FA RID: 506 RVA: 0x0000AF48 File Offset: 0x00009148
		private static QueuedAction missingBossHandler()
		{
			HideoutTroops.HideoutCrashMessage = World.MOD_TITLE + " was unable to find the boss troop for the last hideout fight, so the vanilla fight was let to run instead. \n\n Please tell the mod developer for this issue.";
			QueuedAction queuedAction = new QueuedAction();
			queuedAction.Condition = (() => HideoutTroops.HideoutCrashMessage != null);
			queuedAction.Obsolete = (() => HideoutTroops.HideoutCrashMessage == null);
			queuedAction.ActionToPerform = delegate()
			{
				Message.ShowDialog(HideoutTroops.HideoutCrashMessage);
				HideoutTroops.HideoutCrashMessage = null;
			};
			queuedAction.CheckOnNextTick = (() => true);
			return queuedAction;
		}

		// Token: 0x060001FB RID: 507 RVA: 0x0000B004 File Offset: 0x00009204
		private static int tierCount(MBList<MobileParty> parties)
		{
			int num = 0;
			foreach (MobileParty mobileParty in parties)
			{
				foreach (TroopRosterElement troopRosterElement in mobileParty.MemberRoster.GetTroopRoster())
				{
					num += troopRosterElement.Character.Tier * troopRosterElement.Number;
				}
			}
			return num;
		}

		// Token: 0x060001FC RID: 508 RVA: 0x0000B0A4 File Offset: 0x000092A4
		private static void setTierTargets(MBList<MobileParty> parties, int tierTarget, List<TroopRosterElement> troopTypes)
		{
			foreach (MobileParty mobileParty in parties)
			{
				if (mobileParty.IsBanditBossParty)
				{
					TroopRosterElement theBoss = HideoutTroops.getTheBoss(mobileParty);
					mobileParty.MemberRoster.Clear();
					mobileParty.MemberRoster.AddToCounts(theBoss.Character, 1, false, 0, 0, true, -1);
				}
				else
				{
					mobileParty.MemberRoster.Clear();
				}
			}
			int targetTiers = tierTarget / parties.Count;
			int num = tierTarget % parties.Count;
			foreach (MobileParty party in parties)
			{
				HideoutTroops.addTiers(party, troopTypes, targetTiers);
			}
		}

		// Token: 0x060001FD RID: 509 RVA: 0x0000B178 File Offset: 0x00009378
		public static TroopRosterElement testGetTheBoss(MobileParty party)
		{
			return HideoutTroops.getTheBoss(party);
		}

		// Token: 0x060001FE RID: 510 RVA: 0x0000B180 File Offset: 0x00009380
		private static TroopRosterElement getTheBoss(MobileParty party)
		{
			int num = 0;
			TroopRosterElement result = default(TroopRosterElement);
			foreach (TroopRosterElement troopRosterElement in party.MemberRoster.GetTroopRoster())
			{
				if (troopRosterElement.Character.Tier > num)
				{
					num = troopRosterElement.Character.Tier;
					result = troopRosterElement;
				}
			}
			return result;
		}

		// Token: 0x060001FF RID: 511 RVA: 0x0000B1F8 File Offset: 0x000093F8
		private static void addTiers(MobileParty party, List<TroopRosterElement> troopTypes, int targetTiers)
		{
			TroopRosterElement randomTroop;
			for (int i = targetTiers; i > 0; i -= randomTroop.Character.Tier)
			{
				randomTroop = HideoutTroops.getRandomTroop(troopTypes);
				party.MemberRoster.AddToCounts(randomTroop.Character, 1, false, 0, 0, true, -1);
			}
		}

		// Token: 0x06000200 RID: 512 RVA: 0x0000B23A File Offset: 0x0000943A
		private static TroopRosterElement getRandomTroop(List<TroopRosterElement> troopTypes)
		{
			return troopTypes[MBRandom.RandomInt(troopTypes.Count)];
		}

		// Token: 0x06000201 RID: 513 RVA: 0x0000B250 File Offset: 0x00009450
		private static List<TroopRosterElement> getTroopTypes(MBList<MobileParty> parties)
		{
			List<TroopRosterElement> list = new List<TroopRosterElement>();
			foreach (MobileParty mobileParty in parties)
			{
				foreach (TroopRosterElement troopRosterElement in mobileParty.MemberRoster.GetTroopRoster())
				{
					if (!(troopRosterElement.Character.StringId == HideoutTroops.bossId))
					{
						list.Add(troopRosterElement);
					}
				}
			}
			return list;
		}

		// Token: 0x06000202 RID: 514 RVA: 0x0000B2FC File Offset: 0x000094FC
		private static void oldVersion(HideoutCampaignBehavior __instance)
		{
			int numberOfMinimumBanditTroopsInHideoutMission = Campaign.Current.Models.BanditDensityModel.NumberOfMinimumBanditTroopsInHideoutMission;
			int numberOfMaximumTroopCountForFirstFightInHideout = Campaign.Current.Models.BanditDensityModel.NumberOfMaximumTroopCountForFirstFightInHideout;
			int numberOfMaximumTroopCountForBossFightInHideout = Campaign.Current.Models.BanditDensityModel.NumberOfMaximumTroopCountForBossFightInHideout;
			int num = numberOfMaximumTroopCountForFirstFightInHideout + numberOfMaximumTroopCountForBossFightInHideout;
			MBList<MobileParty> mblist = (from x in Settlement.CurrentSettlement.Parties
			where x.IsBandit || x.IsBanditBossParty
			select x).ToMBList<MobileParty>();
			int num2 = mblist.Sum((MobileParty x) => x.MemberRoster.TotalHealthyCount);
			if (num2 > num)
			{
				HideoutTroops.enoughHealthyTroops(mblist, num2, num);
				return;
			}
			if (num2 >= numberOfMinimumBanditTroopsInHideoutMission)
			{
				return;
			}
			HideoutTroops.notEnoughHealthyTroops(mblist, numberOfMinimumBanditTroopsInHideoutMission, num2);
		}

		// Token: 0x06000203 RID: 515 RVA: 0x0000B3C4 File Offset: 0x000095C4
		private static void enoughHealthyTroops(MBList<MobileParty> mbList, int totalHealthyTroopCount, int totalMaximumTroopCount)
		{
			int num = totalHealthyTroopCount - totalMaximumTroopCount;
			mbList.RemoveAll((MobileParty x) => x.IsBanditBossParty || x.MemberRoster.TotalHealthyCount == 1);
			while (num > 0 && mbList.Count > 0)
			{
				MobileParty randomElement = mbList.GetRandomElement<MobileParty>();
				List<TroopRosterElement> troopRoster = randomElement.MemberRoster.GetTroopRoster();
				List<ValueTuple<TroopRosterElement, float>> list = new List<ValueTuple<TroopRosterElement, float>>();
				foreach (TroopRosterElement item in troopRoster)
				{
					list.Add(new ValueTuple<TroopRosterElement, float>(item, (float)(item.Number - item.WoundedNumber)));
				}
				TroopRosterElement troopRosterElement = MBRandom.ChooseWeighted<TroopRosterElement>(list);
				randomElement.MemberRoster.AddToCounts(troopRosterElement.Character, -1, false, 0, 0, true, -1);
				num--;
				if (randomElement.MemberRoster.TotalHealthyCount == 1)
				{
					mbList.Remove(randomElement);
				}
			}
		}

		// Token: 0x06000204 RID: 516 RVA: 0x0000B4BC File Offset: 0x000096BC
		private static void notEnoughHealthyTroops(MBList<MobileParty> mbList, int inHideoutMission, int totalHealthyTroopCount)
		{
			int num = inHideoutMission - totalHealthyTroopCount;
			mbList.RemoveAll((MobileParty x) => x.MemberRoster.GetTroopRoster().All((TroopRosterElement y) => y.Number == 0 || y.Character.Culture.BanditBoss == y.Character || y.Character.IsHero));
			while (num > 0 && mbList.Count > 0)
			{
				MobileParty randomElement = mbList.GetRandomElement<MobileParty>();
				List<TroopRosterElement> troopRoster = randomElement.MemberRoster.GetTroopRoster();
				List<ValueTuple<TroopRosterElement, float>> list = new List<ValueTuple<TroopRosterElement, float>>();
				foreach (TroopRosterElement troopRosterElement in troopRoster)
				{
					list.Add(new ValueTuple<TroopRosterElement, float>(troopRosterElement, (float)(troopRosterElement.Number * ((troopRosterElement.Character.Culture.BanditBoss == troopRosterElement.Character || troopRosterElement.Character.IsHero) ? 0 : 1))));
				}
				TroopRosterElement troopRosterElement2 = MBRandom.ChooseWeighted<TroopRosterElement>(list);
				randomElement.MemberRoster.AddToCounts(troopRosterElement2.Character, 1, false, 0, 0, true, -1);
				num--;
			}
		}

		// Token: 0x040000A6 RID: 166
		public static string HideoutCrashMessage = null;

		// Token: 0x040000A7 RID: 167
		private static string bossId = "unknown";
	}
}
