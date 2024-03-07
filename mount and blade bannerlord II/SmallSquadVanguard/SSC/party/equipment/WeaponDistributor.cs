using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SSC.party.equipment
{
	// Token: 0x02000063 RID: 99
	public class WeaponDistributor : Distributor
	{
		// Token: 0x060002D4 RID: 724 RVA: 0x0000F8EC File Offset: 0x0000DAEC
		protected override int compare(EquipmentElement first, EquipmentElement second)
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

		// Token: 0x060002D5 RID: 725 RVA: 0x0000F95C File Offset: 0x0000DB5C
		protected override bool companionWantsIt(EquipmentElement newEquipment, Hero companion)
		{
			if (newEquipment.Item == null)
			{
				return false;
			}
			bool flag = this.isUnarmed(companion);
			if (newEquipment.Item.ItemType == ItemObject.ItemTypeEnum.Shield)
			{
				int difficulty = newEquipment.Item.Difficulty;
				if (companion.GetSkillValue(newEquipment.Item.WeaponComponent.PrimaryWeapon.RelevantSkill) < difficulty)
				{
					return false;
				}
				if (flag)
				{
					return false;
				}
				if (this.wantsAShield(companion))
				{
					return true;
				}
			}
			if (flag)
			{
				return true;
			}
			if (this.onlyHasZeroTierWeapons(companion) && (newEquipment.Item.ItemType == ItemObject.ItemTypeEnum.OneHandedWeapon || newEquipment.Item.ItemType == ItemObject.ItemTypeEnum.TwoHandedWeapon))
			{
				return true;
			}
			ItemObject.ItemTypeEnum itemType = newEquipment.Item.ItemType;
			WeaponClass weaponClass = newEquipment.Item.WeaponComponent.PrimaryWeapon.WeaponClass;
			for (int i = 0; i <= 3; i++)
			{
				EquipmentIndex equipmentIndex = (EquipmentIndex)i;
				EquipmentElement equipmentFromSlot = companion.BattleEquipment.GetEquipmentFromSlot(equipmentIndex);
				if (equipmentFromSlot.Item != null && equipmentFromSlot.Item.ItemType == itemType && equipmentFromSlot.Item.WeaponComponent.PrimaryWeapon.WeaponClass == weaponClass)
				{
					return newEquipment.Item.ItemType != ItemObject.ItemTypeEnum.Polearm || newEquipment.Item.WeaponComponent.PrimaryWeapon.WeaponFlags == equipmentFromSlot.Item.WeaponComponent.PrimaryWeapon.WeaponFlags;
				}
			}
			return false;
		}

		// Token: 0x060002D6 RID: 726 RVA: 0x0000FABC File Offset: 0x0000DCBC
		private bool onlyHasZeroTierWeapons(Hero companion)
		{
			bool result = false;
			for (int i = 0; i <= 3; i++)
			{
				EquipmentIndex equipmentIndex = (EquipmentIndex)i;
				EquipmentElement equipmentFromSlot = companion.BattleEquipment.GetEquipmentFromSlot(equipmentIndex);
				if (equipmentFromSlot.Item != null && equipmentFromSlot.Item.Tier >= ItemObject.ItemTiers.Tier1)
				{
					return false;
				}
				if (equipmentFromSlot.Item == null)
				{
					result = true;
				}
			}
			return result;
		}

		// Token: 0x060002D7 RID: 727 RVA: 0x0000FB0C File Offset: 0x0000DD0C
		protected override bool companionCannotUse(EquipmentElement newEquipment, Hero companion)
		{
			return newEquipment.Item == null || (companion.CharacterObject.HasMount() && MBItem.GetItemUsageSetFlags(newEquipment.Item.PrimaryWeapon.ItemUsage).HasAnyFlag(ItemObject.ItemUsageSetFlags.RequiresNoMount)) || !Equip.isAllowedToUseIt(newEquipment, companion);
		}

		// Token: 0x060002D8 RID: 728 RVA: 0x0000FB5C File Offset: 0x0000DD5C
		protected override void equip(EquipmentElement newEquipment, Hero companion)
		{
			if (this.isUnarmed(companion))
			{
				companion.BattleEquipment.AddEquipmentToSlotWithoutAgent(EquipmentIndex.WeaponItemBeginSlot, newEquipment);
				return;
			}
			ItemObject.ItemTypeEnum itemType = newEquipment.Item.ItemType;
			WeaponClass weaponClass = newEquipment.Item.WeaponComponent.PrimaryWeapon.WeaponClass;
			EquipmentIndex equipmentIndex = EquipmentIndex.None;
			for (int i = 0; i <= 3; i++)
			{
				EquipmentIndex equipmentIndex2 = (EquipmentIndex)i;
				EquipmentElement equipmentFromSlot = companion.BattleEquipment.GetEquipmentFromSlot(equipmentIndex2);
				if (equipmentFromSlot.Item != null && equipmentFromSlot.Item.ItemType == itemType && equipmentFromSlot.Item.WeaponComponent.PrimaryWeapon.WeaponClass == weaponClass)
				{
					equipmentIndex = equipmentIndex2;
					break;
				}
			}
			if (equipmentIndex == EquipmentIndex.None)
			{
				if (itemType == ItemObject.ItemTypeEnum.Shield)
				{
					this.equipShieldInAnEmptySlot(newEquipment, companion);
					return;
				}
				if (this.onlyHasZeroTierWeapons(companion))
				{
					for (int j = 0; j <= 3; j++)
					{
						EquipmentIndex equipmentIndex3 = (EquipmentIndex)j;
						if (companion.BattleEquipment.GetEquipmentFromSlot(equipmentIndex3).Item == null)
						{
							companion.BattleEquipment.AddEquipmentToSlotWithoutAgent(equipmentIndex3, newEquipment);
							return;
						}
					}
					return;
				}
			}
			else
			{
				companion.BattleEquipment.AddEquipmentToSlotWithoutAgent(equipmentIndex, newEquipment);
			}
		}

		// Token: 0x060002D9 RID: 729 RVA: 0x0000FC60 File Offset: 0x0000DE60
		protected override EquipmentElement getOldEquipment(ItemObject.ItemTypeEnum typeEnum, Hero companion)
		{
			EquipmentIndex equipmentIndex = Equip.hasWeaponType(typeEnum, companion);
			if (equipmentIndex != EquipmentIndex.None)
			{
				return companion.BattleEquipment.GetEquipmentFromSlot(equipmentIndex);
			}
			return EquipmentElement.Invalid;
		}

		// Token: 0x060002DA RID: 730 RVA: 0x0000FC8C File Offset: 0x0000DE8C
		private bool isUnarmed(Hero companion)
		{
			for (int i = 0; i <= 3; i++)
			{
				EquipmentIndex equipmentIndex = (EquipmentIndex)i;
				if (!companion.BattleEquipment.GetEquipmentFromSlot(equipmentIndex).IsEmpty)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060002DB RID: 731 RVA: 0x0000FCC0 File Offset: 0x0000DEC0
		private void equipShieldInAnEmptySlot(EquipmentElement newEquipment, Hero companion)
		{
			for (int i = 0; i <= 3; i++)
			{
				EquipmentIndex equipmentIndex = (EquipmentIndex)i;
				if (companion.BattleEquipment.GetEquipmentFromSlot(equipmentIndex).Item == null)
				{
					companion.BattleEquipment.AddEquipmentToSlotWithoutAgent(equipmentIndex, newEquipment);
					return;
				}
			}
		}

		// Token: 0x060002DC RID: 732 RVA: 0x0000FD00 File Offset: 0x0000DF00
		private bool wantsAShield(Hero companion)
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			for (int i = 0; i <= 3; i++)
			{
				EquipmentIndex equipmentIndex = (EquipmentIndex)i;
				EquipmentElement equipmentFromSlot = companion.BattleEquipment.GetEquipmentFromSlot(equipmentIndex);
				if (equipmentFromSlot.Item == null)
				{
					flag = true;
				}
				else if (equipmentFromSlot.Item.ItemType == ItemObject.ItemTypeEnum.Shield)
				{
					flag2 = true;
				}
				else if (equipmentFromSlot.Item.WeaponComponent != null && !equipmentFromSlot.Item.WeaponComponent.PrimaryWeapon.WeaponFlags.HasFlag(WeaponFlags.NotUsableWithOneHand))
				{
					flag3 = true;
				}
			}
			return flag2 || (flag3 && flag);
		}

		// Token: 0x040000DE RID: 222
		public List<ItemObject.ItemTypeEnum> supportedTypes = new List<ItemObject.ItemTypeEnum>
		{
			ItemObject.ItemTypeEnum.OneHandedWeapon,
			ItemObject.ItemTypeEnum.TwoHandedWeapon,
			ItemObject.ItemTypeEnum.Polearm,
			ItemObject.ItemTypeEnum.Bow,
			ItemObject.ItemTypeEnum.Crossbow,
			ItemObject.ItemTypeEnum.Thrown,
			ItemObject.ItemTypeEnum.Shield
		};
	}
}
