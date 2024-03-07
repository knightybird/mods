using System;
using System.Collections.Generic;
using SSC.party.companions;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;

namespace SSC.party.equipment
{
	// Token: 0x02000057 RID: 87
	internal static class Equip
	{
		// Token: 0x06000294 RID: 660 RVA: 0x0000E948 File Offset: 0x0000CB48
		public static void fillEquipmentList(ItemRoster roster, ItemObject.ItemTypeEnum equipmentType, List<ItemRosterElement> newEquipment, Equip.CompareEquipment compare)
		{
			newEquipment.Clear();
			foreach (ItemRosterElement item in roster)
			{
				if (item.EquipmentElement.Item.ItemType == equipmentType)
				{
					newEquipment.Add(item);
				}
			}
			newEquipment.Sort((ItemRosterElement x, ItemRosterElement y) => compare(y.EquipmentElement, x.EquipmentElement));
		}

		// Token: 0x06000295 RID: 661 RVA: 0x0000E9CC File Offset: 0x0000CBCC
		public static EquipmentIndex hasWeaponType(ItemObject.ItemTypeEnum weaponType, Hero companion)
		{
			for (int i = 0; i <= 3; i++)
			{
				EquipmentIndex equipmentIndex = (EquipmentIndex)i;
				EquipmentElement equipmentFromSlot = companion.BattleEquipment.GetEquipmentFromSlot(equipmentIndex);
				if (equipmentFromSlot.Item != null && equipmentFromSlot.Item.ItemType == weaponType)
				{
					return equipmentIndex;
				}
				if (equipmentFromSlot.Item != null && equipmentFromSlot.Item.ItemType == weaponType)
				{
					return equipmentIndex;
				}
			}
			return EquipmentIndex.None;
		}

		// Token: 0x06000296 RID: 662 RVA: 0x0000EA2C File Offset: 0x0000CC2C
		public static void returnToPartyInventory(EquipmentElement element)
		{
			if (element.Item == null || element.IsQuestItem)
			{
				return;
			}
			ItemRosterElement itemRosterElement = new ItemRosterElement(element, 1);
			MobileParty.MainParty.ItemRoster.Add(itemRosterElement);
		}

		// Token: 0x06000297 RID: 663 RVA: 0x0000EA65 File Offset: 0x0000CC65
		public static void removeFromPartyInventory(EquipmentElement element)
		{
			MobileParty.MainParty.ItemRoster.AddToCounts(element, -1);
		}

		// Token: 0x06000298 RID: 664 RVA: 0x0000EA7C File Offset: 0x0000CC7C
		public static void fillCompanionList(List<Hero> companions, List<Hero> eligibleCompanions)
		{
			companions.Clear();
			foreach (Hero item in eligibleCompanions)
			{
				companions.Add(item);
			}
		}

		// Token: 0x06000299 RID: 665 RVA: 0x0000EAD0 File Offset: 0x0000CCD0
		public static List<Hero> CompanionsToGetEquipment()
		{
			return Equip.CompanionsToGetEquipmentExcept(HeroFlags.None);
		}

		// Token: 0x0600029A RID: 666 RVA: 0x0000EAD8 File Offset: 0x0000CCD8
		public static List<Hero> CompanionsToGetEquipmentExcept(HeroFlags flag)
		{
			List<Hero> list = new List<Hero>();
			foreach (TroopRosterElement troopRosterElement in MobileParty.MainParty.MemberRoster.GetTroopRoster())
			{
				CharacterObject character = troopRosterElement.Character;
				if (character.IsHero && character != Hero.MainHero.CharacterObject)
				{
					Hero heroObject = character.HeroObject;
					if (flag == HeroFlags.None || World.heroData.flagNotSet(heroObject, flag))
					{
						list.Add(heroObject);
					}
				}
			}
			return list;
		}

		// Token: 0x0600029B RID: 667 RVA: 0x0000EB70 File Offset: 0x0000CD70
		public static bool isAllowedToUseIt(EquipmentElement newEquipment, Hero companion)
		{
			SkillObject relevantSkill = newEquipment.Item.RelevantSkill;
			return companion.GetSkillValue(relevantSkill) >= newEquipment.Item.Difficulty;
		}

		// Token: 0x020000A8 RID: 168
		// (Invoke) Token: 0x0600042E RID: 1070
		public delegate int CompareEquipment(EquipmentElement x, EquipmentElement y);
	}
}
