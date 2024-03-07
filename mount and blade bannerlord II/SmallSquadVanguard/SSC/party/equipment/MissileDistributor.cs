using System;
using System.Collections.Generic;
using SSC.party.companions;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace SSC.party.equipment
{
	// Token: 0x0200005C RID: 92
	internal class MissileDistributor
	{
		// Token: 0x060002B6 RID: 694 RVA: 0x0000F1B2 File Offset: 0x0000D3B2
		public static void distributeArrows()
		{
			MissileDistributor.distributeEquipment(ItemObject.ItemTypeEnum.Arrows);
		}

		// Token: 0x060002B7 RID: 695 RVA: 0x0000F1BA File Offset: 0x0000D3BA
		public static void distributeBolts()
		{
			MissileDistributor.distributeEquipment(ItemObject.ItemTypeEnum.Bolts);
		}

		// Token: 0x060002B8 RID: 696 RVA: 0x0000F1C2 File Offset: 0x0000D3C2
		public static void distributeThrown()
		{
			MissileDistributor.distributeEquipment(ItemObject.ItemTypeEnum.Thrown);
		}

		// Token: 0x060002B9 RID: 697 RVA: 0x0000F1CC File Offset: 0x0000D3CC
		private static void distributeEquipment(ItemObject.ItemTypeEnum itemType)
		{
			List<ItemRosterElement> list = new List<ItemRosterElement>();
			Equip.fillEquipmentList(MobileParty.MainParty.ItemRoster, itemType, list, new Equip.CompareEquipment(MissileDistributor.compare));
			MissileDistributor.reverseSort(list);
			List<Hero> eligibleCompanions = Equip.CompanionsToGetEquipmentExcept(HeroFlags.WeaponsLocked);
			SimpleDistributor.distributeInventory(list, new SimpleDistributor.GiveToCompanion(MissileDistributor.giveToCompanion), eligibleCompanions);
			SimpleDistributor.distributeInventory(list, new SimpleDistributor.GiveToCompanion(MissileDistributor.giveToCompanion), eligibleCompanions);
			SimpleDistributor.distributeInventory(list, new SimpleDistributor.GiveToCompanion(MissileDistributor.giveToCompanion), eligibleCompanions);
		}

		// Token: 0x060002BA RID: 698 RVA: 0x0000F244 File Offset: 0x0000D444
		private static ReturnedEquipment giveToCompanion(EquipmentElement item, Hero hero)
		{
			ItemObject.ItemTypeEnum itemType = item.Item.ItemType;
			int i = 0;
			while (i <= 3)
			{
				EquipmentIndex equipmentIndex = (EquipmentIndex)i;
				EquipmentElement equipmentFromSlot = hero.BattleEquipment.GetEquipmentFromSlot(equipmentIndex);
				if (equipmentFromSlot.Item != null && equipmentFromSlot.Item.ItemType == itemType)
				{
					if (MissileDistributor.compare(item, equipmentFromSlot) > 0)
					{
						hero.BattleEquipment.AddEquipmentToSlotWithoutAgent(equipmentIndex, item);
						return new ReturnedEquipment(equipmentFromSlot, Outcome.TAKEN);
					}
					return new ReturnedEquipment(EquipmentElement.Invalid, Outcome.NOT_GOOD_ENOUGH);
				}
				else
				{
					i++;
				}
			}
			return new ReturnedEquipment(EquipmentElement.Invalid, Outcome.NOT_INTERESTED_IN_THIS_TYPE);
		}

		// Token: 0x060002BB RID: 699 RVA: 0x0000F2C7 File Offset: 0x0000D4C7
		private static int compare(EquipmentElement first, EquipmentElement second)
		{
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

		// Token: 0x060002BC RID: 700 RVA: 0x0000F304 File Offset: 0x0000D504
		private static void reverseSort(List<ItemRosterElement> items)
		{
			items.Sort((ItemRosterElement y, ItemRosterElement x) => MissileDistributor.compare(x.EquipmentElement, y.EquipmentElement));
		}
	}
}
