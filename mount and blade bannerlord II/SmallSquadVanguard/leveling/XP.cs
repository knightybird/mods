using System;
using System.Collections.Generic;
using SSC;
using SSC.misc;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace leveling
{
	// Token: 0x02000002 RID: 2
	public static class XP
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		// (set) Token: 0x06000002 RID: 2 RVA: 0x00002057 File Offset: 0x00000257
		public static List<SkillObject> AllSkills { get; private set; }

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000003 RID: 3 RVA: 0x0000205F File Offset: 0x0000025F
		// (set) Token: 0x06000004 RID: 4 RVA: 0x00002066 File Offset: 0x00000266
		public static List<CharacterAttribute> AllAttributes { get; private set; }

		// Token: 0x06000005 RID: 5 RVA: 0x00002070 File Offset: 0x00000270
		public static void recreateCollections()
		{
			XP.AllSkills = new List<SkillObject>
			{
				DefaultSkills.Athletics,
				DefaultSkills.Bow,
				DefaultSkills.Charm,
				DefaultSkills.Crafting,
				DefaultSkills.Crossbow,
				DefaultSkills.Engineering,
				DefaultSkills.Leadership,
				DefaultSkills.Medicine,
				DefaultSkills.OneHanded,
				DefaultSkills.Polearm,
				DefaultSkills.Riding,
				DefaultSkills.Roguery,
				DefaultSkills.Scouting,
				DefaultSkills.Steward,
				DefaultSkills.Tactics,
				DefaultSkills.Throwing,
				DefaultSkills.Trade,
				DefaultSkills.TwoHanded
			};
			XP.AllAttributes = new List<CharacterAttribute>
			{
				DefaultCharacterAttributes.Vigor,
				DefaultCharacterAttributes.Control,
				DefaultCharacterAttributes.Endurance,
				DefaultCharacterAttributes.Cunning,
				DefaultCharacterAttributes.Social,
				DefaultCharacterAttributes.Intelligence
			};
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00002199 File Offset: 0x00000399
		public static void UpdateFocusAndAttributes_CHECK(Hero hero)
		{
			if (hero == Hero.MainHero)
			{
				return;
			}
			hero.HeroDeveloper.ClearUnspentPoints();
			XP.updateFocusPoints(hero);
			XP.updateAttributePoints(hero);
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000021BC File Offset: 0x000003BC
		private static void updateFocusPoints(Hero companion)
		{
			foreach (SkillObject skill in XP.AllSkills)
			{
				int skillValue = companion.GetSkillValue(skill);
				int focus = companion.HeroDeveloper.GetFocus(skill);
				int num = 0;
				if (skillValue >= 2)
				{
					num++;
				}
				if (skillValue >= 10)
				{
					num++;
				}
				if (skillValue >= 20)
				{
					num++;
				}
				if (skillValue >= 30)
				{
					num++;
				}
				if (skillValue >= 50)
				{
					num++;
				}
				int num2 = Math.Min(num - focus, 5 - focus);
				if (num2 > 0)
				{
					companion.HeroDeveloper.AddFocus(skill, num2, false);
				}
			}
		}

		// Token: 0x06000008 RID: 8 RVA: 0x0000226C File Offset: 0x0000046C
		private static void updateAttributePoints(Hero companion)
		{
			foreach (CharacterAttribute characterAttribute in XP.AllAttributes)
			{
				int num = 0;
				foreach (SkillObject skill in characterAttribute.Skills)
				{
					num += companion.GetSkillValue(skill);
				}
				int num2 = 0;
				if (num >= 1)
				{
					num2++;
				}
				if (num >= 15)
				{
					num2++;
				}
				if (num >= 30)
				{
					num2++;
				}
				if (num >= 45)
				{
					num2++;
				}
				if (num >= 60)
				{
					num2++;
				}
				if (num >= 75)
				{
					num2++;
				}
				if (num >= 90)
				{
					num2++;
				}
				if (num >= 105)
				{
					num2++;
				}
				if (num >= 120)
				{
					num2++;
				}
				if (num >= 135)
				{
					num2++;
				}
				int num3 = Math.Min(num2 - companion.GetAttributeValue(characterAttribute), 10 - companion.GetAttributeValue(characterAttribute));
				if (num3 > 0)
				{
					companion.HeroDeveloper.AddAttribute(characterAttribute, num3, false);
				}
			}
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002394 File Offset: 0x00000594
		public static void ClearSkillXp(Hero hero)
		{
			if (hero == Hero.MainHero)
			{
				return;
			}
			foreach (SkillObject skill in XP.AllSkills)
			{
				hero.HeroDeveloper.InitializeSkillXp(skill);
			}
		}

		// Token: 0x0600000A RID: 10 RVA: 0x000023F4 File Offset: 0x000005F4
		public static void RemoveAllFocus(Hero hero)
		{
			foreach (SkillObject skill in XP.AllSkills)
			{
				hero.HeroDeveloper.GetFocus(skill);
				hero.HeroDeveloper.RemoveFocus(skill, hero.HeroDeveloper.GetFocus(skill));
				hero.HeroDeveloper.GetFocus(skill);
			}
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002474 File Offset: 0x00000674
		public static void AssignPerks_CHECK(Hero hero)
		{
			XP.ChoosePerk_CHECK(hero);
		}

		// Token: 0x0600000C RID: 12 RVA: 0x00002480 File Offset: 0x00000680
		public static bool ChoosePerk_CHECK(Hero hero)
		{
			bool result = false;
			foreach (PerkObject perkObject in PerkObject.All)
			{
				if (!hero.HeroDeveloper.GetPerkValue(perkObject) && perkObject.Skill != null && (float)hero.GetSkillValue(perkObject.Skill) >= perkObject.RequiredSkillValue)
				{
					hero.HeroDeveloper.AddPerk(perkObject);
					result = true;
					string text = string.Format("{0} learned {1}", hero.Name, perkObject.Name);
					if (perkObject.AlternativePerk != null && !hero.HeroDeveloper.GetPerkValue(perkObject.AlternativePerk))
					{
						hero.HeroDeveloper.AddPerk(perkObject.AlternativePerk);
						text = string.Format("{0} and {1}", text, perkObject.AlternativePerk.Name);
					}
					Message.display(text, Colors.Yellow);
				}
			}
			return result;
		}

		// Token: 0x0600000D RID: 13 RVA: 0x00002578 File Offset: 0x00000778
		public static void hourlyMoving(Hero hero)
		{
			if (hero.CharacterObject.IsMounted)
			{
				XP.AddSkillXp(hero, DefaultSkills.Riding, Manage.rand.Next(0, 3));
			}
			else
			{
				XP.AddSkillXp(hero, DefaultSkills.Athletics, Manage.rand.Next(0, 3));
			}
			Hero effectiveScout = MobileParty.MainParty.EffectiveScout;
			if (hero == effectiveScout)
			{
				XP.AddSkillXp(hero, DefaultSkills.Scouting, Manage.rand.Next(0, 5));
			}
		}

		// Token: 0x0600000E RID: 14 RVA: 0x000025E8 File Offset: 0x000007E8
		public static void AddSkillXp(Hero companion, SkillObject skill, int xp)
		{
			if (xp > 0)
			{
				companion.HeroDeveloper.AddSkillXp(skill, (float)xp, false, true);
			}
		}

		// Token: 0x0600000F RID: 15 RVA: 0x00002600 File Offset: 0x00000800
		public static void GainBattleStartBonus(Hero companion)
		{
			HashSet<SkillObject> alreadyAwarded = new HashSet<SkillObject>();
			XP.addToRelevantSkill(XP.slot(EquipmentIndex.WeaponItemBeginSlot, companion), companion, alreadyAwarded);
			XP.addToRelevantSkill(XP.slot(EquipmentIndex.Weapon1, companion), companion, alreadyAwarded);
			XP.addToRelevantSkill(XP.slot(EquipmentIndex.Weapon2, companion), companion, alreadyAwarded);
			XP.addToRelevantSkill(XP.slot(EquipmentIndex.Weapon3, companion), companion, alreadyAwarded);
		}

		// Token: 0x06000010 RID: 16 RVA: 0x0000264C File Offset: 0x0000084C
		private static int requiredToLevelUp(Hero companion, SkillObject skill)
		{
			int skillXpProgress = companion.HeroDeveloper.GetSkillXpProgress(skill);
			int skillValue = companion.GetSkillValue(skill);
			return Campaign.Current.Models.CharacterDevelopmentModel.GetXpRequiredForSkillLevel(skillValue + 1) - Campaign.Current.Models.CharacterDevelopmentModel.GetXpRequiredForSkillLevel(skillValue) - skillXpProgress;
		}

		// Token: 0x06000011 RID: 17 RVA: 0x000026A0 File Offset: 0x000008A0
		private static int calculateBattleParticipationBonus(Hero companion, SkillObject skill)
		{
			int skillValue = companion.GetSkillValue(skill);
			int num = XP.requiredToLevelUp(companion, skill);
			int num2;
			if (skillValue < 20)
			{
				num2 = num - 1;
			}
			else if (skillValue < 150)
			{
				num2 = (int)((float)num * XP.MAIN_XP_MODIFIER);
			}
			else if (skillValue < 200)
			{
				num2 = (int)((float)num * XP.XP_MODIFIER_150);
			}
			else
			{
				num2 = (int)((float)num * XP.XP_MODIFIER_200);
			}
			if (num2 < 10)
			{
				num2 = 10;
			}
			return num2;
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00002704 File Offset: 0x00000904
		private static void addToRelevantSkill(EquipmentElement weapon, Hero companion, HashSet<SkillObject> alreadyAwarded)
		{
			if (weapon.Item == null)
			{
				return;
			}
			if (weapon.Item.HasWeaponComponent)
			{
				SkillObject relevantSkill = weapon.Item.WeaponComponent.PrimaryWeapon.RelevantSkill;
				if (!alreadyAwarded.Contains(relevantSkill))
				{
					int num = XP.calculateBattleParticipationBonus(companion, relevantSkill);
					companion.HeroDeveloper.AddSkillXp(relevantSkill, (float)num, false, true);
					alreadyAwarded.Add(relevantSkill);
				}
			}
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002769 File Offset: 0x00000969
		private static EquipmentElement slot(EquipmentIndex index, Hero companion)
		{
			return companion.BattleEquipment.GetEquipmentFromSlot(index);
		}

		// Token: 0x04000003 RID: 3
		public static float MAIN_XP_MODIFIER = 0.9f;

		// Token: 0x04000004 RID: 4
		public static float XP_MODIFIER_150 = 0.6f;

		// Token: 0x04000005 RID: 5
		public static float XP_MODIFIER_200 = 0.3f;
	}
}
