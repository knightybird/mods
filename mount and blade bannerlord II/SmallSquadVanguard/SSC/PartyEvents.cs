using System;
using System.Collections.Generic;
using leveling;
using SSC.misc;
using SSC.party;
using SSC.party.companions;
using SSC.party.equipment;
using SSC.patch;
using SSC.settings;
using SSC.world.escort;
using StoryMode;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem;
using TaleWorlds.ScreenSystem;

namespace SSC
{
	// Token: 0x02000009 RID: 9
	internal class PartyEvents : CampaignBehaviorBase
	{
		// Token: 0x0600003C RID: 60 RVA: 0x00003268 File Offset: 0x00001468
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<Hero>("designated_scout", ref this.designatedScout);
			dataStore.SyncData<Hero>("designated_surgeon", ref this.designatedSurgeon);
			dataStore.SyncData<Hero>("designated_quartermaster", ref this.designatedQuartermaster);
			dataStore.SyncData<Hero>("designated_engineer", ref this.designatedEngineer);
			dataStore.SyncData<long>("party_events_flags", ref this.partyEventsFlags);
			dataStore.SyncData<Dictionary<Hero, int>>("vanguard_hero_flags", ref this.heroFlags);
			dataStore.SyncData<int>("companion_limit", ref this.companionLimit);
			dataStore.SyncData<bool>("large_squad_mode", ref this.largeSquadMode);
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00003308 File Offset: 0x00001508
		public override void RegisterEvents()
		{
			if (CampaignOptions.AutoAllocateClanMemberPerks)
			{
				CampaignOptions.AutoAllocateClanMemberPerks = false;
			}
			World.CompanionLimit = new FieldProxy<int>(() => this.companionLimit, delegate(int value)
			{
				this.companionLimit = value;
			});
			World.LargeSquadMode = new FieldProxy<bool>(() => this.largeSquadMode, delegate(bool value)
			{
				this.largeSquadMode = value;
			});
			XP.recreateCollections();
			World.BeingReleased.Clear();
			World.CaptorsHack.Clear();
			this.removeDeadHeroesFromHeroMap();
			World.DesignatedScout = new FieldProxy<Hero>(() => this.designatedScout, delegate(Hero value)
			{
				this.designatedScout = value;
			});
			World.DesignatedSurgeon = new FieldProxy<Hero>(() => this.designatedSurgeon, delegate(Hero value)
			{
				this.designatedSurgeon = value;
			});
			World.DesignatedEngineer = new FieldProxy<Hero>(() => this.designatedEngineer, delegate(Hero value)
			{
				this.designatedEngineer = value;
			});
			World.DesignatedQuartermaster = new FieldProxy<Hero>(() => this.designatedQuartermaster, delegate(Hero value)
			{
				this.designatedQuartermaster = value;
			});
			World.setFlags(new BitFlags(this.partyEventsFlags));
			World.OnWorldFlagsChange += delegate(BitFlags flags)
			{
				this.partyEventsFlags = flags.getFlags();
			};
			World.setHeroData(new HeroData(this.heroFlags));
			ScreenManager.OnPopScreen -= LockLayer.popScreen;
			ScreenManager.OnPushScreen -= LockLayer.pushScreen;
			ScreenManager.OnPopScreen += LockLayer.popScreen;
			ScreenManager.OnPushScreen += LockLayer.pushScreen;
			CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(this.hourlyTick));
			CampaignEvents.HeroLevelledUp.AddNonSerializedListener(this, new Action<Hero, bool>(this.HeroLevelsUp));
			CampaignEvents.CollectLootsEvent.AddNonSerializedListener(this, new Action<MapEvent, PartyBase, Dictionary<PartyBase, ItemRoster>, ItemRoster, MBList<TroopRosterElement>, float>(this.collectLoot));
			CampaignEvents.HeroGainedSkill.AddNonSerializedListener(this, new Action<Hero, SkillObject, int, bool>(this.heroGainedSkill));
			CampaignEvents.OnPlayerBattleEndEvent.AddNonSerializedListener(this, new Action<MapEvent>(this.battleEnded));
			CampaignEvents.ConversationEnded.AddNonSerializedListener(this, new Action<IEnumerable<CharacterObject>>(this.ConversationEnded));
			CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.SettlementEntered));
			CampaignEvents.OnSettlementLeftEvent.AddNonSerializedListener(this, new Action<MobileParty, Settlement>(this.SettlementLeft));
			CampaignEvents.HeroPrisonerReleased.AddNonSerializedListener(this, new Action<Hero, PartyBase, IFaction, EndCaptivityDetail>(this.prisonerReleased));
			CampaignEvents.OnHeroJoinedPartyEvent.AddNonSerializedListener(this, new Action<Hero, MobileParty>(this.HeroJoinsParty));
			CampaignEvents.OnQuarterDailyPartyTick.AddNonSerializedListener(this, new Action<MobileParty>(this.QuarterlyTick));
			CampaignEvents.OnPlayerBattleEndEvent.AddNonSerializedListener(this, new Action<MapEvent>(this.playerBattleEnd));
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.sessionLaunched));
			CampaignEvents.DailyTickPartyEvent.AddNonSerializedListener(this, new Action<MobileParty>(this.dailyTickPartyEvent));
			CampaignEvents.OnGameLoadFinishedEvent.AddNonSerializedListener(this, new Action(this.GameLoadFinished));
			CampaignEvents.CharacterBecameFugitive.AddNonSerializedListener(this, new Action<Hero>(this.CharacterBecameFugitive));
			CampaignEvents.OnCharacterCreationIsOverEvent.AddNonSerializedListener(this, new Action(this.characterCreationIsOver));
			CampaignEvents.OnHeroTeleportationRequestedEvent.AddNonSerializedListener(this, new Action<Hero, Settlement, MobileParty, TeleportHeroAction.TeleportationDetail>(this.TeleportationRequested));
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00003620 File Offset: 0x00001820
		private void sessionLaunched(CampaignGameStarter starter)
		{
			HiredCompanion.AddMenus(starter);
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00003628 File Offset: 0x00001828
		private void hourlyTick()
		{
			if (this.redistributionIsNeeded)
			{
				Manage.Equipment.distributeInventory();
				this.redistributionIsNeeded = false;
			}
			if (Settlement.CurrentSettlement != null)
			{
				Manage.Healing.inSettlement(Settlement.CurrentSettlement);
			}
			if (MobileParty.MainParty.IsMoving)
			{
				Companion.forEachCustomCompanionInPartyDo(new Action<Hero>(XP.hourlyMoving));
			}
		}

		// Token: 0x06000040 RID: 64 RVA: 0x00003681 File Offset: 0x00001881
		private void QuarterlyTick(MobileParty party)
		{
			if (party != MobileParty.MainParty)
			{
				return;
			}
			PartyEvents.quarterlySteward();
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00003694 File Offset: 0x00001894
		private static void quarterlySteward()
		{
			Hero effectiveQuartermaster = MobileParty.MainParty.EffectiveQuartermaster;
			if (effectiveQuartermaster == null)
			{
				return;
			}
			if (Companion.isCustomCompanion(effectiveQuartermaster))
			{
				XP.AddSkillXp(effectiveQuartermaster, DefaultSkills.Steward, 40);
			}
		}

		// Token: 0x06000042 RID: 66 RVA: 0x000036C8 File Offset: 0x000018C8
		private void characterCreationIsOver()
		{
			if (Campaign.Current.GetType() == typeof(Campaign))
			{
				PartyInitialization.InitializeParty(false);
				World.setFlag(Flags.PartyInitialized);
			}
			else if (Campaign.Current.GetType() == typeof(CampaignStoryMode))
			{
				HarmonyLibrary.applyOnSettlementLeftPatch();
			}
			this.largeSquadMode = ModSettings.LargeSquadMode;
			this.companionLimit = (this.largeSquadMode ? ModSettings.InitialCompanions : (World.SMALL_SQUAD_SIZE - 1));
			if (this.companionLimit < 4)
			{
				this.companionLimit = 4;
			}
			HarmonyLibrary.RestrictPartySize(!this.largeSquadMode);
		}

		// Token: 0x06000043 RID: 67 RVA: 0x00003764 File Offset: 0x00001964
		private void GameLoadFinished()
		{
			if (World.flagNotSet(Flags.PartyInitialized) && Campaign.Current.GetType() == typeof(CampaignStoryMode))
			{
				HarmonyLibrary.applyOnSettlementLeftPatch();
			}
			PartyInitialization.SetAllLegacyCompanionsToCustom();
			HarmonyLibrary.RestrictPartySize(!this.largeSquadMode);
		}

		// Token: 0x06000044 RID: 68 RVA: 0x000037A2 File Offset: 0x000019A2
		private void ConversationEnded(IEnumerable<CharacterObject> enumerable)
		{
			if (World.ConversationFinished != null)
			{
				Message.display("Conversation ended.");
				World.ConversationFinished();
				World.ConversationFinished = null;
			}
		}

		// Token: 0x06000045 RID: 69 RVA: 0x000037C5 File Offset: 0x000019C5
		private void SettlementLeft(MobileParty party, Settlement settlement)
		{
			if (party != MobileParty.MainParty)
			{
				return;
			}
			this.redistributionIsNeeded = true;
		}

		// Token: 0x06000046 RID: 70 RVA: 0x000037D8 File Offset: 0x000019D8
		private void SettlementEntered(MobileParty party, Settlement settlement, Hero hero)
		{
			if (hero != null && settlement.IsHideout && hero.IsPlayerCompanion && hero.IsPrisoner)
			{
				Hideout hideout = settlement.Hideout;
				if (!hideout.IsInfested || !hideout.IsSpotted)
				{
					hideout.IsSpotted = true;
				}
			}
		}

		// Token: 0x06000047 RID: 71 RVA: 0x0000381E File Offset: 0x00001A1E
		private void TeleportationRequested(Hero hero, Settlement settlement, MobileParty party, TeleportHeroAction.TeleportationDetail detail)
		{
			if (party == MobileParty.MainParty && Companion.isCustomCompanion(hero) && detail == TeleportHeroAction.TeleportationDetail.ImmediateTeleportToParty)
			{
				PartyRoles.Report(hero, false);
			}
		}

		// Token: 0x06000048 RID: 72 RVA: 0x0000383C File Offset: 0x00001A3C
		private void playerBattleEnd(MapEvent mapEvent)
		{
			Companion.forEachCustomCompanionInPartyDo(new Action<Hero>(XP.GainBattleStartBonus));
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00003850 File Offset: 0x00001A50
		private void sendToPartyWhileCheckingTheDistance(Hero hero, PartyBase captors)
		{
			Distance distance = DistanceManager.distanceToMainHero(captors);
			if (distance.IsValid && distance.Value < 10f)
			{
				TeleportHeroAction.ApplyImmediateTeleportToParty(hero, MobileParty.MainParty);
				if (hero.IsFugitive)
				{
					hero.ChangeState(Hero.CharacterStates.Active);
					return;
				}
			}
			else
			{
				TeleportHeroAction.ApplyDelayedTeleportToParty(hero, MobileParty.MainParty);
			}
		}

		// Token: 0x0600004A RID: 74 RVA: 0x000038A4 File Offset: 0x00001AA4
		private void CharacterBecameFugitive(Hero hero)
		{
			if (!hero.IsPlayerCompanion)
			{
				return;
			}
			if (Companion.isCustomCompanion(hero))
			{
				if (World.BeingReleased.ContainsKey(hero))
				{
					return;
				}
				if (World.CaptorsHack.ContainsKey(hero))
				{
					World.CaptorsHack.Remove(hero);
					return;
				}
				TeleportHeroAction.ApplyDelayedTeleportToParty(hero, MobileParty.MainParty);
			}
		}

		// Token: 0x0600004B RID: 75 RVA: 0x000038F5 File Offset: 0x00001AF5
		private void prisonerReleased(Hero hero, PartyBase party, IFaction faction, EndCaptivityDetail detail)
		{
			if (Companion.isCustomCompanion(hero))
			{
				if (World.BeingReleased.ContainsKey(hero))
				{
					World.BeingReleased.Remove(hero);
					return;
				}
				this.sendToPartyWhileCheckingTheDistance(hero, party);
			}
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00003921 File Offset: 0x00001B21
		private void dailyTickPartyEvent(MobileParty party)
		{
			if (party == MobileParty.MainParty)
			{
				Companion.forEachCustomCompanionInPartyDo(new Action<Hero>(XP.AssignPerks_CHECK));
				PartyRoles.designateHeroesIfNeeded();
			}
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00003941 File Offset: 0x00001B41
		private void collectLoot(MapEvent mapEvent, PartyBase winnerParty, Dictionary<PartyBase, ItemRoster> loot, ItemRoster lootedItemsForParty, MBList<TroopRosterElement> shareFromCasualties, float lootAmount)
		{
			if (MobileParty.MainParty.MapEvent == mapEvent)
			{
				this.redistributionIsNeeded = true;
			}
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00003957 File Offset: 0x00001B57
		private static string randomMedicTitle()
		{
			string[] array = new string[]
			{
				"volunteer",
				"helper",
				"aid-giver"
			};
			return array[MBRandom.RandomInt(array.Length)];
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00003980 File Offset: 0x00001B80
		private void heroGainedSkill(Hero hero, SkillObject skill, int change, bool shouldNotify)
		{
			if (Companion.isCustomCompanion(hero))
			{
				if (skill == DefaultSkills.Medicine)
				{
					bool flag = MobileParty.MainParty.EffectiveSurgeon == hero;
					string str = string.Format("{0} ({1}) gained {2} {3}", new object[]
					{
						hero.Name,
						flag ? "Party surgeon" : PartyEvents.randomMedicTitle(),
						change,
						skill.Name
					});
					string str2 = string.Format(" and is now at {0}", hero.GetSkillValue(skill));
					Message.display(str + str2);
				}
				Companion.companionGainedSkill(hero, skill, change, shouldNotify);
				XP.UpdateFocusAndAttributes_CHECK(hero);
				XP.ChoosePerk_CHECK(hero);
			}
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00003A24 File Offset: 0x00001C24
		private void battleEnded(MapEvent mapEvent)
		{
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00003A26 File Offset: 0x00001C26
		private void HeroLevelsUp(Hero hero, bool shouldNotify)
		{
			if (Companion.isCustomCompanion(hero))
			{
				XP.UpdateFocusAndAttributes_CHECK(hero);
			}
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00003A36 File Offset: 0x00001C36
		private void HeroJoinsParty(Hero hero, MobileParty party)
		{
			if (Companion.isCustomCompanion(hero))
			{
				XP.UpdateFocusAndAttributes_CHECK(hero);
			}
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00003A48 File Offset: 0x00001C48
		private void removeDeadHeroesFromHeroMap()
		{
			if (this.heroFlags == null)
			{
				return;
			}
			List<Hero> list = new List<Hero>();
			foreach (KeyValuePair<Hero, int> keyValuePair in this.heroFlags)
			{
				Hero key = keyValuePair.Key;
				if (key != null && key.IsDead)
				{
					list.Add(keyValuePair.Key);
				}
			}
			foreach (Hero key2 in list)
			{
				this.heroFlags.Remove(key2);
			}
		}

		// Token: 0x04000014 RID: 20
		[SaveableField(1)]
		private Hero designatedScout;

		// Token: 0x04000015 RID: 21
		[SaveableField(2)]
		private Hero designatedSurgeon;

		// Token: 0x04000016 RID: 22
		[SaveableField(3)]
		private Hero designatedQuartermaster;

		// Token: 0x04000017 RID: 23
		[SaveableField(4)]
		private Hero designatedEngineer;

		// Token: 0x04000018 RID: 24
		[SaveableField(5)]
		private long partyEventsFlags;

		// Token: 0x04000019 RID: 25
		[SaveableField(6)]
		private Dictionary<Hero, int> heroFlags = new Dictionary<Hero, int>();

		// Token: 0x0400001A RID: 26
		[SaveableField(7)]
		private int companionLimit = World.SMALL_SQUAD_SIZE - 1;

		// Token: 0x0400001B RID: 27
		[SaveableField(8)]
		private bool largeSquadMode;

		// Token: 0x0400001C RID: 28
		private bool redistributionIsNeeded = true;
	}
}
