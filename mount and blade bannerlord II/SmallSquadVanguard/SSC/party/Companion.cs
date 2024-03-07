using System;
using leveling;
using SSC.party.companions;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;

namespace SSC.party
{
	// Token: 0x02000051 RID: 81
	internal class Companion
	{
		// Token: 0x0600026A RID: 618 RVA: 0x0000DC24 File Offset: 0x0000BE24
		public static void updateAllCustomCompanions()
		{
			foreach (Hero companion in Clan.PlayerClan.Companions)
			{
				Companion.UpdateFocusAndAttributes(companion);
			}
		}

		// Token: 0x0600026B RID: 619 RVA: 0x0000DC78 File Offset: 0x0000BE78
		public static void UpdateFocusAndAttributes(Hero companion)
		{
			if (Companion.isCustomCompanion(companion))
			{
				XP.UpdateFocusAndAttributes_CHECK(companion);
			}
		}

		// Token: 0x0600026C RID: 620 RVA: 0x0000DC88 File Offset: 0x0000BE88
		public static void companionGainedSkill(Hero hero, SkillObject skill, int change, bool shouldNotify)
		{
			if (hero.HeroDeveloper.UnspentFocusPoints > 0 || hero.HeroDeveloper.UnspentAttributePoints > 0)
			{
				Companion.UpdateFocusAndAttributes(hero);
			}
		}

		// Token: 0x0600026D RID: 621 RVA: 0x0000DCAC File Offset: 0x0000BEAC
		public static void forEachCustomCompanionInPartyDo(Action<Hero> action)
		{
			foreach (TroopRosterElement troopRosterElement in MobileParty.MainParty.MemberRoster.GetTroopRoster())
			{
				CharacterObject character = troopRosterElement.Character;
				if (character.IsHero && Companion.isCustomCompanion(character.HeroObject))
				{
					Hero heroObject = character.HeroObject;
					if (heroObject.PartyBelongedTo == MobileParty.MainParty)
					{
						action(heroObject);
					}
				}
			}
		}

		// Token: 0x0600026E RID: 622 RVA: 0x0000DD38 File Offset: 0x0000BF38
		public static void forEachCustomCompanionEverywhereDo(Action<Hero> action)
		{
			foreach (TroopRosterElement troopRosterElement in MobileParty.MainParty.MemberRoster.GetTroopRoster())
			{
				CharacterObject character = troopRosterElement.Character;
				if (character.IsHero && Companion.isCustomCompanion(character.HeroObject))
				{
					Hero heroObject = character.HeroObject;
					action(heroObject);
				}
			}
		}

		// Token: 0x0600026F RID: 623 RVA: 0x0000DDB8 File Offset: 0x0000BFB8
		public static bool isCustomCompanion(Hero hero)
		{
			return hero != null && World.heroData != null && World.heroData.isFlagSet(hero, HeroFlags.IsCustomCompanion);
		}

		// Token: 0x06000270 RID: 624 RVA: 0x0000DDD4 File Offset: 0x0000BFD4
		public static BodyProperties createBodyPropertiesFrom(BodyProperties old, float newAge, float newWeight, float newBuild)
		{
			DynamicBodyProperties dynamicBodyProperties = new DynamicBodyProperties(newAge, newWeight, newBuild);
			StaticBodyProperties staticProperties = old.StaticProperties;
			return new BodyProperties(dynamicBodyProperties, staticProperties);
		}
	}
}
