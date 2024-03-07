using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;

namespace SSC.party.equipment
{
	// Token: 0x02000062 RID: 98
	public abstract class Distributor
	{
		// Token: 0x060002C9 RID: 713
		protected abstract int compare(EquipmentElement first, EquipmentElement second);

		// Token: 0x060002CA RID: 714
		protected abstract EquipmentElement getOldEquipment(ItemObject.ItemTypeEnum typeEnum, Hero companion);

		// Token: 0x060002CB RID: 715 RVA: 0x0000F60E File Offset: 0x0000D80E
		protected virtual bool companionWantsIt(EquipmentElement newEquipment, Hero companion)
		{
			return true;
		}

		// Token: 0x060002CC RID: 716 RVA: 0x0000F611 File Offset: 0x0000D811
		protected virtual bool companionCannotUse(EquipmentElement newEquipment, Hero companion)
		{
			return false;
		}

		// Token: 0x060002CD RID: 717
		protected abstract void equip(EquipmentElement newEquipment, Hero companion);

		// Token: 0x060002CE RID: 718 RVA: 0x0000F614 File Offset: 0x0000D814
		public void distributeInventory(ItemRoster roster, ItemObject.ItemTypeEnum typeEnum, List<Hero> eligibleCompanions)
		{
			this.roster = roster;
			Equip.fillEquipmentList(roster, typeEnum, this.newEquipment, new Equip.CompareEquipment(this.compare));
			if (this.newEquipment.Count <= 0)
			{
				return;
			}
			this.fillCompanionList(typeEnum, eligibleCompanions);
			for (int i = 0; i < this.newEquipment.Count; i++)
			{
				ItemRosterElement itemRosterElement = this.newEquipment[i];
				DistributionOutcome distributionOutcome = this.distributeEquipment(itemRosterElement, typeEnum);
				if (distributionOutcome == DistributionOutcome.DISTRIBUTED)
				{
					if (itemRosterElement.Amount > 1)
					{
						int amount = itemRosterElement.Amount;
						itemRosterElement.Amount = amount - 1;
						this.newEquipment[i] = itemRosterElement;
						i--;
					}
					else
					{
						this.newEquipment.Remove(itemRosterElement);
						i--;
					}
				}
				else if (distributionOutcome == DistributionOutcome.NOT_GOOD_ENOUGH)
				{
					break;
				}
			}
		}

		// Token: 0x060002CF RID: 719 RVA: 0x0000F6D4 File Offset: 0x0000D8D4
		private DistributionOutcome distributeEquipment(ItemRosterElement itemElement, ItemObject.ItemTypeEnum typeEnum)
		{
			bool flag = false;
			int i = 0;
			while (i < this.companions.Count)
			{
				Hero hero = this.companions[i];
				EquipmentElement oldEquipment = this.getOldEquipment(typeEnum, hero);
				bool isEmpty = oldEquipment.IsEmpty;
				EquipmentElement equipmentElement = itemElement.EquipmentElement;
				if (oldEquipment.IsEmpty || this.compare(equipmentElement, oldEquipment) > 0)
				{
					if (this.companionCannotUse(equipmentElement, hero))
					{
						flag = true;
					}
					else
					{
						if (this.companionWantsIt(equipmentElement, hero))
						{
							this.equip(equipmentElement, hero);
							Equip.removeFromPartyInventory(equipmentElement);
							Equip.returnToPartyInventory(oldEquipment);
							this.companions.Remove(hero);
							return DistributionOutcome.DISTRIBUTED;
						}
						this.companions.Remove(hero);
						i--;
					}
					i++;
				}
				else
				{
					if (flag)
					{
						return DistributionOutcome.CANNOT_USE;
					}
					return DistributionOutcome.NOT_GOOD_ENOUGH;
				}
			}
			return DistributionOutcome.CANNOT_USE;
		}

		// Token: 0x060002D0 RID: 720 RVA: 0x0000F798 File Offset: 0x0000D998
		private void fillCompanionList(ItemObject.ItemTypeEnum typeEnum, List<Hero> eligibleCompanions)
		{
			this.companions.Clear();
			foreach (Hero item in eligibleCompanions)
			{
				this.companions.Add(item);
			}
			this.companions.Sort((Hero x, Hero y) => this.compare(this.getOldEquipment(typeEnum, x), this.getOldEquipment(typeEnum, y)));
		}

		// Token: 0x060002D1 RID: 721 RVA: 0x0000F824 File Offset: 0x0000DA24
		protected int nullAndTierCompare(EquipmentElement first, EquipmentElement second)
		{
			if (first.Item != null && second.Item == null)
			{
				return 1;
			}
			if (first.Item == null && second.Item != null)
			{
				return -1;
			}
			if (first.Item == null && second.Item == null)
			{
				return 0;
			}
			if (first.Item.Tier > second.Item.Tier)
			{
				return 1;
			}
			if (second.Item.Tier > first.Item.Tier)
			{
				return -1;
			}
			return 0;
		}

		// Token: 0x060002D2 RID: 722 RVA: 0x0000F8A6 File Offset: 0x0000DAA6
		protected int itemValueCompare(EquipmentElement first, EquipmentElement second)
		{
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

		// Token: 0x040000DB RID: 219
		private List<Hero> companions = new List<Hero>();

		// Token: 0x040000DC RID: 220
		private List<ItemRosterElement> newEquipment = new List<ItemRosterElement>();

		// Token: 0x040000DD RID: 221
		private ItemRoster roster;
	}
}
