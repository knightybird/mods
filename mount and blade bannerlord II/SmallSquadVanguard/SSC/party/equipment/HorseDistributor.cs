using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace SSC.party.equipment
{
	// Token: 0x02000058 RID: 88
	internal class HorseDistributor
	{
		// Token: 0x0600029C RID: 668 RVA: 0x0000EBA4 File Offset: 0x0000CDA4
		public static void distributeHorses()
		{
			List<ItemRosterElement> list = new List<ItemRosterElement>();
			Equip.fillEquipmentList(MobileParty.MainParty.ItemRoster, ItemObject.ItemTypeEnum.Horse, list, new Equip.CompareEquipment(HorseDistributor.compare));
			HorseDistributor.reverseSort(list);
			List<Hero> eligibleCompanions = Equip.CompanionsToGetEquipment();
			SimpleDistributor.distributeInventory(list, new SimpleDistributor.GiveToCompanion(HorseDistributor.giveToCompanion), eligibleCompanions);
		}

		// Token: 0x0600029D RID: 669 RVA: 0x0000EBF4 File Offset: 0x0000CDF4
		public static void distributeHarness()
		{
			List<ItemRosterElement> list = new List<ItemRosterElement>();
			Equip.fillEquipmentList(MobileParty.MainParty.ItemRoster, ItemObject.ItemTypeEnum.HorseHarness, list, new Equip.CompareEquipment(HorseDistributor.compare));
			HorseDistributor.reverseSort(list);
			SimpleDistributor.distributeInventory(list, new SimpleDistributor.GiveToCompanion(HorseDistributor.giveToCompanion), Equip.CompanionsToGetEquipment());
		}

		// Token: 0x0600029E RID: 670 RVA: 0x0000EC44 File Offset: 0x0000CE44
		private static ReturnedEquipment giveToCompanion(EquipmentElement newitem, Hero hero)
		{
			ItemObject.ItemTypeEnum itemType = newitem.Item.ItemType;
			if (itemType == ItemObject.ItemTypeEnum.HorseHarness)
			{
				return HorseDistributor.giveSaddleToCompanion(newitem, hero);
			}
			EquipmentIndex equipmentIndex = EquipmentIndex.ArmorItemEndSlot;
			EquipmentElement equipmentFromSlot = hero.BattleEquipment.GetEquipmentFromSlot(equipmentIndex);
			if (equipmentFromSlot.Item == null || equipmentFromSlot.Item.ItemType != itemType || HorseDistributor.compare(newitem, equipmentFromSlot) <= 0)
			{
				return new ReturnedEquipment(EquipmentElement.Invalid, Outcome.NOT_INTERESTED_IN_THIS_TYPE);
			}
			if (Equip.isAllowedToUseIt(newitem, hero))
			{
				hero.BattleEquipment.AddEquipmentToSlotWithoutAgent(equipmentIndex, newitem);
				HorseDistributor.removeSaddleIfNecessary(hero);
				return new ReturnedEquipment(equipmentFromSlot, Outcome.TAKEN);
			}
			return new ReturnedEquipment(EquipmentElement.Invalid, Outcome.UNABLE_TO_USE);
		}

		// Token: 0x0600029F RID: 671 RVA: 0x0000ECD8 File Offset: 0x0000CED8
		private static void removeSaddleIfNecessary(Hero hero)
		{
			EquipmentElement equipmentFromSlot = hero.BattleEquipment.GetEquipmentFromSlot(EquipmentIndex.HorseHarness);
			if (equipmentFromSlot.Item != null && !HorseDistributor.canEquipSaddle(equipmentFromSlot, hero))
			{
				Equip.returnToPartyInventory(equipmentFromSlot);
				hero.BattleEquipment.AddEquipmentToSlotWithoutAgent(EquipmentIndex.HorseHarness, EquipmentElement.Invalid);
			}
		}

		// Token: 0x060002A0 RID: 672 RVA: 0x0000ED20 File Offset: 0x0000CF20
		private static ReturnedEquipment giveSaddleToCompanion(EquipmentElement newItem, Hero hero)
		{
			EquipmentIndex equipmentIndex = EquipmentIndex.HorseHarness;
			EquipmentElement equipmentFromSlot = hero.BattleEquipment.GetEquipmentFromSlot(equipmentIndex);
			if (!hero.CharacterObject.HasMount())
			{
				return new ReturnedEquipment(EquipmentElement.Invalid, Outcome.NOT_INTERESTED_IN_THIS_TYPE);
			}
			if (equipmentFromSlot.Item != null && HorseDistributor.compare(newItem, equipmentFromSlot) <= 0)
			{
				return new ReturnedEquipment(EquipmentElement.Invalid, Outcome.NOT_INTERESTED_IN_THIS_TYPE);
			}
			if (Equip.isAllowedToUseIt(newItem, hero) && HorseDistributor.canEquipSaddle(newItem, hero))
			{
				hero.BattleEquipment.AddEquipmentToSlotWithoutAgent(equipmentIndex, newItem);
				return new ReturnedEquipment(equipmentFromSlot, Outcome.TAKEN);
			}
			return new ReturnedEquipment(EquipmentElement.Invalid, Outcome.UNABLE_TO_USE);
		}

		// Token: 0x060002A1 RID: 673 RVA: 0x0000EDA8 File Offset: 0x0000CFA8
		private static bool canEquipSaddle(EquipmentElement saddle, Hero hero)
		{
			ItemObject item = hero.BattleEquipment.GetEquipmentFromSlot(EquipmentIndex.ArmorItemEndSlot).Item;
			return saddle.Item != null && item != null && saddle.Item.ItemType == ItemObject.ItemTypeEnum.HorseHarness && (!string.IsNullOrEmpty(item.StringId) && item.HorseComponent.Monster.FamilyType == saddle.Item.ArmorComponent.FamilyType);
		}

		// Token: 0x060002A2 RID: 674 RVA: 0x0000EE20 File Offset: 0x0000D020
		private static int compare(EquipmentElement first, EquipmentElement second)
		{
			if (first.Item == second.Item)
			{
				return 0;
			}
			if (first.Item == null)
			{
				return 1;
			}
			if (second.Item == null)
			{
				return -1;
			}
			if (first.ItemValue > second.ItemValue)
			{
				return 1;
			}
			if (first.ItemValue < second.ItemValue)
			{
				return -1;
			}
			return 0;
		}

		// Token: 0x060002A3 RID: 675 RVA: 0x0000EE7A File Offset: 0x0000D07A
		private static void reverseSort(List<ItemRosterElement> items)
		{
			items.Sort((ItemRosterElement y, ItemRosterElement x) => HorseDistributor.compare(x.EquipmentElement, y.EquipmentElement));
		}
	}
}
