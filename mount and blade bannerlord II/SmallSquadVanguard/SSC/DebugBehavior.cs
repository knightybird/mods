using System;
using SSC.settings;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;

namespace SSC
{
	// Token: 0x0200000A RID: 10
	internal class DebugBehavior : CampaignBehaviorBase
	{
		// Token: 0x06000062 RID: 98 RVA: 0x00003BA7 File Offset: 0x00001DA7
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00003BAC File Offset: 0x00001DAC
		public override void RegisterEvents()
		{
			CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(this.OnHourlyTick));
			CampaignEvents.DailyTickPartyEvent.AddNonSerializedListener(this, new Action<MobileParty>(this.OnDailyTickParty));
			CampaignEvents.HeroPrisonerReleased.AddNonSerializedListener(this, new Action<Hero, PartyBase, IFaction, EndCaptivityDetail>(this.OnPrisonerReleased));
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
			CampaignEvents.OnGameLoadFinishedEvent.AddNonSerializedListener(this, new Action(this.OnGameLoadFinished));
			CampaignEvents.OnTutorialCompletedEvent.AddNonSerializedListener(this, new Action<string>(this.TutorialCompleted));
			CampaignEvents.OnQuarterDailyPartyTick.AddNonSerializedListener(this, new Action<MobileParty>(this.OnQuarterDailyPartyTick));
			CampaignEvents.OnCharacterCreationIsOverEvent.AddNonSerializedListener(this, new Action(this.OnCharacterCreationIsOver));
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00003C71 File Offset: 0x00001E71
		private void OnGameLoadFinished()
		{
			ModSettings.printOut();
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00003C79 File Offset: 0x00001E79
		private void OnHourlyTick()
		{
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00003C7B File Offset: 0x00001E7B
		private void OnQuarterDailyPartyTick(MobileParty party)
		{
			if (party != MobileParty.MainParty)
			{
				return;
			}
			MobileParty.MainParty.MemberRoster.UpdateVersion();
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00003C98 File Offset: 0x00001E98
		private void OnDailyTickParty(MobileParty party)
		{
			if (party != MobileParty.MainParty)
			{
				return;
			}
			this.checkHeroCounts();
			Hero.MainHero.Clan.Companions.ForEach(delegate(Hero companion)
			{
				if (companion.IsPrisoner)
				{
					Settlement currentSettlement = companion.CurrentSettlement;
					if (currentSettlement != null && currentSettlement.IsHideout)
					{
						Hideout hideout = currentSettlement.Hideout;
						if (hideout != null && !hideout.IsSpotted)
						{
							hideout.IsSpotted = true;
						}
					}
				}
			});
			ModSettings.printOut();
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00003CF0 File Offset: 0x00001EF0
		private void listPartyUpgradeTargets()
		{
			Hero mainHero = Hero.MainHero;
			foreach (TroopRosterElement troopRosterElement in MobileParty.MainParty.MemberRoster.GetTroopRoster())
			{
			}
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00003D4C File Offset: 0x00001F4C
		private void OnPrisonerReleased(Hero hero, PartyBase @base, IFaction faction, EndCaptivityDetail detail)
		{
			if (hero.IsPlayerCompanion && Hero.MainHero.IsActive && !Hero.MainHero.IsPrisoner && this.trackedHero == null)
			{
				this.trackedHero = hero;
			}
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00003D7D File Offset: 0x00001F7D
		private void OnCharacterCreationIsOver()
		{
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00003D7F File Offset: 0x00001F7F
		private void OnSessionLaunched(CampaignGameStarter starter)
		{
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00003D81 File Offset: 0x00001F81
		private void TutorialCompleted(string obj)
		{
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00003D84 File Offset: 0x00001F84
		private void checkHeroCounts()
		{
			foreach (TroopRosterElement troopRosterElement in MobileParty.MainParty.MemberRoster.GetTroopRoster())
			{
				if (troopRosterElement.Character.IsHero)
				{
					int number = troopRosterElement.Number;
				}
			}
		}

		// Token: 0x0600006E RID: 110 RVA: 0x00003DF0 File Offset: 0x00001FF0
		private void ledPartyMounts()
		{
			if (World.ScoutMission_LedParty != null)
			{
				MobileParty scoutMission_LedParty = World.ScoutMission_LedParty;
				int numberOfMounts = scoutMission_LedParty.Party.NumberOfMounts;
				int numberOfRegularMembers = scoutMission_LedParty.Party.NumberOfRegularMembers;
				int numberOfAllMembers = scoutMission_LedParty.Party.NumberOfAllMembers;
				if (World.ScoutMission_LedParty.LeaderHero != null)
				{
					int gold = World.ScoutMission_LedParty.LeaderHero.Gold;
				}
			}
		}

		// Token: 0x0400001D RID: 29
		private Hero trackedHero;
	}
}
