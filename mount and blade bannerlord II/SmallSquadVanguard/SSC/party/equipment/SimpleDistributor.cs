using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace SSC.party.equipment
{
	// Token: 0x0200005F RID: 95
	internal class SimpleDistributor
	{
		// Token: 0x060002BF RID: 703 RVA: 0x0000F34C File Offset: 0x0000D54C
		public static void distributeInventory(List<ItemRosterElement> newItems, SimpleDistributor.GiveToCompanion giveToCompanion, List<Hero> eligibleCompanions)
		{
			if (newItems.Count <= 0)
			{
				return;
			}
			List<Hero> list = new List<Hero>();
			Equip.fillCompanionList(list, eligibleCompanions);
			list.Shuffle<Hero>();
			for (int i = 0; i < newItems.Count; i++)
			{
				ItemRosterElement itemRosterElement = newItems[i];
				if (SimpleDistributor.distributeItem(itemRosterElement, list, giveToCompanion))
				{
					if (itemRosterElement.Amount > 1)
					{
						int amount = itemRosterElement.Amount;
						itemRosterElement.Amount = amount - 1;
						newItems[i] = itemRosterElement;
						i--;
					}
					else
					{
						newItems.Remove(itemRosterElement);
						i--;
					}
				}
			}
		}

		// Token: 0x060002C0 RID: 704 RVA: 0x0000F3D0 File Offset: 0x0000D5D0
		private static bool distributeItem(ItemRosterElement itemElement, List<Hero> companions, SimpleDistributor.GiveToCompanion giveToCompanion)
		{
			for (int i = 0; i < companions.Count; i++)
			{
				Hero hero = companions[i];
				EquipmentElement equipmentElement = itemElement.EquipmentElement;
				ReturnedEquipment returnedEquipment = giveToCompanion(equipmentElement, hero);
				if (returnedEquipment.outcome == Outcome.TAKEN)
				{
					Equip.removeFromPartyInventory(equipmentElement);
					if (returnedEquipment.oldEquipment.Item != null)
					{
						Equip.returnToPartyInventory(returnedEquipment.oldEquipment);
					}
					companions.Remove(hero);
					return true;
				}
				if (returnedEquipment.outcome == Outcome.NOT_GOOD_ENOUGH)
				{
					companions.Remove(hero);
					i--;
				}
				else if (returnedEquipment.outcome != Outcome.UNABLE_TO_USE)
				{
					Outcome outcome = returnedEquipment.outcome;
				}
			}
			return false;
		}

		// Token: 0x020000AD RID: 173
		// (Invoke) Token: 0x0600043D RID: 1085
		public delegate ReturnedEquipment GiveToCompanion(EquipmentElement newitem, Hero hero);
	}
}
