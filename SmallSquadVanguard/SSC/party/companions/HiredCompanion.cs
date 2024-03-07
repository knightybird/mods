using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Overlay;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace SSC.party.companions
{
	// Token: 0x02000068 RID: 104
	internal class HiredCompanion
	{
		// Token: 0x06000327 RID: 807 RVA: 0x00011F78 File Offset: 0x00010178
		public static Hero CreatePeasantWithPitchfork(Settlement settlement, bool isFemale)
		{
			CharacterObject villager = isFemale ? settlement.Culture.VillageWoman : settlement.Culture.Villager;
			Func<CharacterObject, bool> predicate = (CharacterObject x) => x.Occupation == Occupation.Wanderer && x.IsFemale == villager.IsFemale;
			CharacterObject characterObject = villager.Culture.NotableAndWandererTemplates.GetRandomElementWithPredicate(predicate) ?? CharacterObject.PlayerCharacter.Culture.NotableAndWandererTemplates.GetRandomElementWithPredicate(predicate);
			characterObject.Age = (float)Manage.rand.Next(20, 23);
			Hero hero = HeroCreator.CreateSpecialHero(characterObject, settlement, null, null, (int)characterObject.Age);
			hero.ClearTraits();
			PartyInitialization.SetInitialStateForPeasantWithPitchfork(hero);
			hero.HeroDeveloper.ClearHero();
			hero.HeroDeveloper.SetInitialLevel(1);
			hero.Level = 1;
			string value = hero.FirstName.ToString();
			hero.SetName(new TextObject(value, null), new TextObject(value, null));
			hero.ChangeState(Hero.CharacterStates.Active);
			return hero;
		}

		// Token: 0x06000328 RID: 808 RVA: 0x00012060 File Offset: 0x00010260
		public static void AddMenus(CampaignGameStarter starter)
		{
			starter.AddGameMenuOption("village", "vanguard_hire_companion", "Recruit a villager", new GameMenuOption.OnConditionDelegate(HiredCompanion.hire_companion_condition), delegate(MenuCallbackArgs args)
			{
				GameMenu.SwitchToMenu("vanguard_available_villagers_for_hire");
			}, false, 0, true, null);
			starter.AddGameMenu("vanguard_available_villagers_for_hire", "Within the village, you find several locals ready to leave their old lives behind. They stand prepared to join your cause.", delegate(MenuCallbackArgs args)
			{
			}, GameOverlays.MenuOverlayType.SettlementWithBoth, GameMenu.MenuFlags.None, null);
			starter.AddGameMenuOption("vanguard_available_villagers_for_hire", "vanguard_hire_named_male_villager", "Male", new GameMenuOption.OnConditionDelegate(HiredCompanion.hire_menu), delegate(MenuCallbackArgs args)
			{
				HiredCompanion.hire(false);
			}, false, -1, true, null);
			starter.AddGameMenuOption("vanguard_available_villagers_for_hire", "vanguard_hire_named_female_villager", "Female", new GameMenuOption.OnConditionDelegate(HiredCompanion.hire_menu), delegate(MenuCallbackArgs args)
			{
				HiredCompanion.hire(true);
			}, false, -1, true, null);
			starter.AddGameMenuOption("vanguard_available_villagers_for_hire", "vanguard_hire_companion_leave", "Leave", new GameMenuOption.OnConditionDelegate(HiredCompanion.hire_menu_leave), delegate(MenuCallbackArgs args)
			{
				GameMenu.SwitchToMenu("village");
			}, true, -1, true, null);
			HiredCompanion.AddTownMenuOption(starter);
		}

		// Token: 0x06000329 RID: 809 RVA: 0x000121B8 File Offset: 0x000103B8
		private static void AddTownMenuOption(CampaignGameStarter starter)
		{
			starter.AddGameMenuOption("town", "vanguard_town_hire_companion", "Recruit a villager", new GameMenuOption.OnConditionDelegate(HiredCompanion.hire_companion_condition), delegate(MenuCallbackArgs args)
			{
				GameMenu.SwitchToMenu("vanguard_town_available_villagers_for_hire");
			}, false, 0, true, null);
			starter.AddGameMenu("vanguard_town_available_villagers_for_hire", "You notice a group of peasants from neighboring villages, drawn to the town by necessity or hope. Their eyes dart around the bustling town square, their hands clutching the pitchforks a little too tightly. You may approach them and offer them a chance to trade their tools for weapons.", delegate(MenuCallbackArgs args)
			{
			}, GameOverlays.MenuOverlayType.SettlementWithBoth, GameMenu.MenuFlags.None, null);
			starter.AddGameMenuOption("vanguard_town_available_villagers_for_hire", "vanguard_town_hire_named_male_villager", "Male", new GameMenuOption.OnConditionDelegate(HiredCompanion.hire_menu), delegate(MenuCallbackArgs args)
			{
				HiredCompanion.hire(false);
			}, false, -1, true, null);
			starter.AddGameMenuOption("vanguard_town_available_villagers_for_hire", "vanguard_town_hire_female_villager", "Female", new GameMenuOption.OnConditionDelegate(HiredCompanion.hire_menu), delegate(MenuCallbackArgs args)
			{
				HiredCompanion.hire(true);
			}, false, -1, true, null);
			starter.AddGameMenuOption("vanguard_town_available_villagers_for_hire", "vanguard_town_hire_companion_leave", "Leave", new GameMenuOption.OnConditionDelegate(HiredCompanion.hire_menu_leave), delegate(MenuCallbackArgs args)
			{
				GameMenu.SwitchToMenu("town");
			}, true, -1, true, null);
		}

		// Token: 0x0600032A RID: 810 RVA: 0x00012308 File Offset: 0x00010508
		private static void hire(bool isFemale)
		{
			Settlement currentSettlement = Settlement.CurrentSettlement;
			Hero hero = HiredCompanion.CreatePeasantWithPitchfork(currentSettlement.IsVillage ? HiredCompanion.findVillage(currentSettlement) : currentSettlement, isFemale);
			PartyInitialization.AddNewCompanion(hero);
			if (World.LargeSquadMode.Value)
			{
				int companionCount = HiredCompanion.getCompanionCount();
				int value = World.CompanionLimit.Value;
				if (companionCount >= value)
				{
					World.CompanionLimit.Value = companionCount + 1;
				}
			}
			CharacterObject characterObject = hero.CharacterObject;
			MBInformationManager.AddQuickInformation(new TextObject(string.Format("{0} has joined your party.", characterObject.Name), null), 1100, characterObject, "");
			if (currentSettlement.IsVillage)
			{
				GameMenu.SwitchToMenu("vanguard_available_villagers_for_hire");
				return;
			}
			GameMenu.SwitchToMenu("vanguard_town_available_villagers_for_hire");
		}

		// Token: 0x0600032B RID: 811 RVA: 0x000123B0 File Offset: 0x000105B0
		private static Settlement findVillage(Settlement town)
		{
			int count = town.BoundVillages.Count;
			if (count == 0)
			{
				return town;
			}
			int index = Manage.rand.Next(count);
			return town.BoundVillages[index].Settlement;
		}

		// Token: 0x0600032C RID: 812 RVA: 0x000123EB File Offset: 0x000105EB
		private static bool hire_menu(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Recruit;
			args.IsEnabled = HiredCompanion.partySizeOk();
			if (!args.IsEnabled && !HiredCompanion.partySizeOk())
			{
				args.Tooltip = new TextObject("Party size limit reached.", null);
			}
			return true;
		}

		// Token: 0x0600032D RID: 813 RVA: 0x00012421 File Offset: 0x00010621
		private static bool hire_menu_leave(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Leave;
			return true;
		}

		// Token: 0x0600032E RID: 814 RVA: 0x0001242C File Offset: 0x0001062C
		private static bool hire_companion_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Recruit;
			Settlement currentSettlement = Settlement.CurrentSettlement;
			if (currentSettlement.IsTown && !HiredCompanion.partySizeOk())
			{
				return false;
			}
			if (currentSettlement.IsVillage && currentSettlement.IsRaided)
			{
				return false;
			}
			if (currentSettlement.IsTown && currentSettlement.IsUnderSiege)
			{
				return false;
			}
			if (!World.LargeSquadMode.Value && HiredCompanion.getCompanionCount() >= World.CompanionLimit.Value)
			{
				return false;
			}
			args.IsEnabled = HiredCompanion.partySizeOk();
			if (!args.IsEnabled)
			{
				args.Tooltip = new TextObject("Party size limit reached.", null);
			}
			return true;
		}

		// Token: 0x0600032F RID: 815 RVA: 0x000124C0 File Offset: 0x000106C0
		private static bool partySizeOk()
		{
			int partySizeLimit = MobileParty.MainParty.Party.PartySizeLimit;
			return MobileParty.MainParty.MemberRoster.TotalManCount < partySizeLimit;
		}

		// Token: 0x06000330 RID: 816 RVA: 0x000124EF File Offset: 0x000106EF
		private static bool companionLimitOk()
		{
			return World.LargeSquadMode.Value || HiredCompanion.getCompanionCount() < World.CompanionLimit.Value;
		}

		// Token: 0x06000331 RID: 817 RVA: 0x00012510 File Offset: 0x00010710
		private static int getCompanionCount()
		{
			Hero mainHero = Hero.MainHero;
			bool flag;
			if (mainHero == null)
			{
				flag = (null != null);
			}
			else
			{
				Clan clan = mainHero.Clan;
				flag = (((clan != null) ? clan.Companions : null) != null);
			}
			if (flag)
			{
				return Hero.MainHero.Clan.Companions.Count;
			}
			return 0;
		}
	}
}
