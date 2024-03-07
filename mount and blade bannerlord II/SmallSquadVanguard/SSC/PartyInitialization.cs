using System;
using System.Collections.Generic;
using System.Reflection;
using leveling;
using SSC.misc;
using SSC.party;
using SSC.party.companions;
using SSC.settings;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace SSC
{
	// Token: 0x02000008 RID: 8
	internal class PartyInitialization
	{
		// Token: 0x06000029 RID: 41 RVA: 0x00002C68 File Offset: 0x00000E68
		public static void InitializeParty(bool storyMode)
		{
			PartyInitialization.CreateCompanions();
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00002C70 File Offset: 0x00000E70
		public static void SetAllLegacyCompanionsToCustom()
		{
			foreach (Hero hero in Clan.PlayerClan.Companions)
			{
				if (hero.CharacterObject.OriginalCharacter != null)
				{
					string stringId = hero.CharacterObject.OriginalCharacter.StringId;
					if (PartyInitialization.map.ContainsKey(stringId))
					{
						World.heroData.setFlag(hero, HeroFlags.IsCustomCompanion);
					}
				}
			}
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00002CF8 File Offset: 0x00000EF8
		private static void CreateCompanions()
		{
			PartyInitialization.AddCompanionsToParty(PartyInitialization.CreateCompanionsToList());
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00002D04 File Offset: 0x00000F04
		public static void AddCompanionsToParty(List<Hero> companions)
		{
			foreach (Hero companion in companions)
			{
				PartyInitialization.AddNewCompanion(companion);
			}
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00002D50 File Offset: 0x00000F50
		public static List<Hero> CreateCompanionsToList()
		{
			Dictionary<string, PlayerDefinedCompanion> playerDefinedCompanions = BodyKeys.getPlayerDefinedCompanions();
			List<Hero> list = new List<Hero>();
			foreach (string templateId in PartyInitialization.randomList(ModSettings.InitialCompanions))
			{
				Hero item = PartyInitialization.CreateCompanion(templateId, playerDefinedCompanions);
				list.Add(item);
			}
			PartyInitialization.storePartyKeys();
			return list;
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00002DC0 File Offset: 0x00000FC0
		private static List<string> randomList(int count)
		{
			List<string> list = new List<string>
			{
				"spc_companion_flavios",
				"spc_companion_culharn",
				"spc_companion_griff",
				"spc_companion_sein",
				"spc_companion_morcar",
				"spc_companion_de_ker",
				"spc_companion_montfort",
				"spc_companion_meroc",
				"spc_companion_armand",
				"spc_companion_folc",
				"spc_companion_lamarc",
				"spc_companion_cherald",
				"spc_companion_furcred",
				"spc_companion_malbert",
				"spc_companion_sevelar",
				"spc_companion_sicard",
				"spc_companion_avigos",
				"spc_companion_eutos",
				"spc_companion_mavinon",
				"spc_companion_de_marly",
				"spc_companion_du_fay",
				"spc_companion_buchard",
				"spc_companion_leofric",
				"spc_companion_ragnar",
				"spc_companion_haqr",
				"spc_companion_jassid",
				"spc_companion_draco",
				"spc_companion_maximos",
				"spc_companion_brutos"
			};
			list.Shuffle<string>();
			list.RemoveRange(count, list.Count - count);
			if (list.Count > 0 && !list.Contains("spc_companion_avigos"))
			{
				list.RemoveAt(0);
				list.Add("spc_companion_avigos");
			}
			return list;
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00002F50 File Offset: 0x00001150
		public static Hero CreateCompanion(string templateId, Dictionary<string, PlayerDefinedCompanion> playerMap)
		{
			CharacterObject characterObject = CharacterObject.Find(templateId);
			Hero hero = HeroCreator.CreateSpecialHero(characterObject, null, null, null, (int)characterObject.Age);
			PartyInitialization.modifyFromKey(hero, PartyInitialization.map, playerMap);
			PartyInitialization.SetInitialStateForPeasantWithPitchfork(hero);
			return hero;
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00002F86 File Offset: 0x00001186
		public static void SetInitialStateForPeasantWithPitchfork(Hero hero)
		{
			hero.ClearSkills();
			hero.ClearAttributes();
			hero.ClearPerks();
			XP.RemoveAllFocus(hero);
			XP.ClearSkillXp(hero);
			hero.ChangeState(Hero.CharacterStates.Active);
			XP.UpdateFocusAndAttributes_CHECK(hero);
			Manage.Equipment.setPitchforkBasedEquipment(hero);
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00002FC0 File Offset: 0x000011C0
		private static void modifyAppearance(Hero companion, int seed)
		{
			CharacterObject characterObject = companion.CharacterObject;
			BodyProperties bodyPropertiesMin = characterObject.GetBodyPropertiesMin(false);
			BodyProperties bodyPropertiesMax = characterObject.GetBodyPropertiesMax();
			StaticBodyProperties sbp = FaceGen.GetRandomBodyProperties(0, false, bodyPropertiesMin, bodyPropertiesMax, 0, seed, "", "", "").StaticProperties;
			HexEditor hexEditor = new HexEditor(sbp);
			hexEditor.removeBeards();
			sbp = hexEditor.getResult();
			Manage.SBP.setStaticBodyProperties(companion, sbp);
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00003022 File Offset: 0x00001222
		private static void AddNewCompanion(string templateId)
		{
			PartyInitialization.AddNewCompanion(PartyInitialization.CreateCompanion(templateId, null));
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00003030 File Offset: 0x00001230
		private static void modifyFromKey(Hero companion, Dictionary<string, string> map, Dictionary<string, PlayerDefinedCompanion> playerMap)
		{
			PlayerDefinedCompanion playerDefinedCompanion;
			if (playerMap != null && playerMap.TryGetValue(companion.Template.StringId, out playerDefinedCompanion))
			{
				HexEditor hexEditor = new HexEditor(Manage.SBP.fromKey(playerDefinedCompanion.bodyKey));
				Manage.SBP.setStaticBodyProperties(companion, hexEditor.getResult());
				companion.SetName(new TextObject(playerDefinedCompanion.companionName, null), new TextObject(playerDefinedCompanion.companionName, null));
				return;
			}
			string key;
			if (map.TryGetValue(companion.Template.StringId, out key))
			{
				HexEditor hexEditor2 = new HexEditor(Manage.SBP.fromKey(key));
				Manage.SBP.setStaticBodyProperties(companion, hexEditor2.getResult());
				companion.SetName(companion.Name, companion.Name);
			}
		}

		// Token: 0x06000034 RID: 52 RVA: 0x000030E4 File Offset: 0x000012E4
		public static void applyBodyKey(Hero hero, string key)
		{
			HexEditor hexEditor = new HexEditor(Manage.SBP.fromKey(key));
			Manage.SBP.setStaticBodyProperties(hero, hexEditor.getResult());
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00003113 File Offset: 0x00001313
		public static void AddNewCompanion(Hero companion)
		{
			AddCompanionAction.Apply(Clan.PlayerClan, companion);
			AddHeroToPartyAction.Apply(companion, MobileParty.MainParty, false);
			companion.SetHasMet();
			World.heroData.setFlag(companion, HeroFlags.IsCustomCompanion);
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00003140 File Offset: 0x00001340
		private static void storePartyKeys()
		{
			List<string> list = new List<string>();
			list.Add(Hero.MainHero.Name.ToString() + " " + Hero.MainHero.BodyProperties.StaticProperties.ToString());
			foreach (Hero hero in Clan.PlayerClan.Companions)
			{
				list.Add(hero.Name.ToString() + " " + hero.BodyProperties.StaticProperties.ToString());
			}
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00003210 File Offset: 0x00001410
		public static CampaignTime calculateBirthday(float age)
		{
			return CampaignTime.YearsFromNow(-age);
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00003219 File Offset: 0x00001419
		public static void setBirthDay(CampaignTime newBirthDay, Hero hero)
		{
			typeof(Hero).GetField("_birthDay", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(hero, newBirthDay);
		}

		// Token: 0x06000039 RID: 57 RVA: 0x0000323D File Offset: 0x0000143D
		public static void SetAge(Hero hero, int years)
		{
			CampaignTime birthDay = hero.BirthDay;
			PartyInitialization.setBirthDay(CampaignTime.YearsFromNow((float)(-(float)years)), hero);
		}

		// Token: 0x04000013 RID: 19
		private static Dictionary<string, string> map = BodyKeys.getBodyKeys();
	}
}
