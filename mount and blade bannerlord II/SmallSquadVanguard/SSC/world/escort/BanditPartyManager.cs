using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace SSC.world.escort
{
	// Token: 0x02000019 RID: 25
	public class BanditPartyManager
	{
		// Token: 0x06000136 RID: 310 RVA: 0x00006894 File Offset: 0x00004A94
		public static MobileParty SpawnBanditParty(EscortData escort, EscortBattlesPerformance performance)
		{
			Settlement closestHideout = SettlementHelper.FindNearestHideout((Settlement x) => x.IsActive, null);
			Clan clan = Clan.BanditFactions.FirstOrDefault((Clan t) => t.Culture == closestHideout.Culture);
			MobileParty mobileParty = BanditPartyComponent.CreateBanditParty(BanditPartyManager.banditStringId, clan, closestHideout.Hideout, false);
			TroopRoster memberRoster = new TroopRoster(mobileParty.Party);
			TroopRoster prisonerRoster = new TroopRoster(mobileParty.Party);
			mobileParty.InitializeMobilePartyAtPosition(memberRoster, prisonerRoster, escort.caravan.TargetSettlement.GatePosition);
			mobileParty.SetCustomName(new TextObject("{=u1Pkt4HC}Raiders", null));
			mobileParty.ActualClan = clan;
			performance.setNextFormation();
			performance.currentFormation.loadTroopsTo(mobileParty, BanditCultures.appropriateCultureIds(escort.destination, closestHideout));
			int totalManCount = mobileParty.MemberRoster.TotalManCount;
			int num = 0;
			foreach (TroopRosterElement troopRosterElement in mobileParty.MemberRoster.GetTroopRoster())
			{
				if (!troopRosterElement.Character.IsMounted)
				{
					num += troopRosterElement.Number;
				}
			}
			if (totalManCount > 25)
			{
				string objectName = BanditPartyManager.cultureHorseId(escort.destination.Culture.StringId);
				mobileParty.ItemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>(objectName), num);
			}
			mobileParty.IgnoreForHours(24f);
			return mobileParty;
		}

		// Token: 0x06000137 RID: 311 RVA: 0x00006A24 File Offset: 0x00004C24
		private static string cultureHorseId(string cultureId)
		{
			if (cultureId == "battania")
			{
				return "battania_horse";
			}
			if (cultureId == "vlandia")
			{
				return "vlandia_horse";
			}
			if (cultureId == "empire")
			{
				return "empire_horse";
			}
			if (cultureId == "sturgia")
			{
				return "sturgia_horse";
			}
			if (cultureId == "khuzait")
			{
				return "khuzait_horse";
			}
			return "empire_horse";
		}

		// Token: 0x06000138 RID: 312 RVA: 0x00006A95 File Offset: 0x00004C95
		public static void attackCaravan(MobileParty banditParty, MobileParty caravan)
		{
			SetPartyAiAction.GetActionForEngagingParty(banditParty, caravan);
			banditParty.Ai.SetDoNotMakeNewDecisions(true);
		}

		// Token: 0x06000139 RID: 313 RVA: 0x00006AAC File Offset: 0x00004CAC
		private static void initializeDefaultBanditTroops(MobileParty banditParty, PartyTemplateObject partyTemplateObject)
		{
			for (int i = 0; i < 20; i++)
			{
				List<ValueTuple<PartyTemplateStack, float>> list = new List<ValueTuple<PartyTemplateStack, float>>();
				foreach (PartyTemplateStack partyTemplateStack in partyTemplateObject.Stacks)
				{
					list.Add(new ValueTuple<PartyTemplateStack, float>(partyTemplateStack, (float)(64 - partyTemplateStack.Character.Level)));
				}
				PartyTemplateStack partyTemplateStack2 = MBRandom.ChooseWeighted<PartyTemplateStack>(list);
				banditParty.MemberRoster.AddToCounts(partyTemplateStack2.Character, 1, false, 0, 0, true, -1);
			}
		}

		// Token: 0x0400005A RID: 90
		private static string banditStringId = "escort_task_bandit_party";
	}
}
