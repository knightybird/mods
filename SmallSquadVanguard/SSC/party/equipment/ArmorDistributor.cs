using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace SSC.party.equipment
{
	// Token: 0x02000060 RID: 96
	public class ArmorDistributor : Distributor
	{
		// Token: 0x060002C2 RID: 706 RVA: 0x0000F46C File Offset: 0x0000D66C
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
			int num = this.armorCompare(first, second);
			if (num == 0)
			{
				return base.itemValueCompare(first, second);
			}
			return num;
		}

		// Token: 0x060002C3 RID: 707 RVA: 0x0000F4CB File Offset: 0x0000D6CB
		protected override bool companionWantsIt(EquipmentElement newEquipment, Hero companion)
		{
			return true;
		}

		// Token: 0x060002C4 RID: 708 RVA: 0x0000F4CE File Offset: 0x0000D6CE
		private float armorValue(EquipmentElement element)
		{
			return (float)element.GetModifiedBodyArmor() * this.bodyArmorWeight + (float)element.GetModifiedHeadArmor() + (float)element.GetModifiedArmArmor() + (float)element.GetModifiedLegArmor();
		}

		// Token: 0x060002C5 RID: 709 RVA: 0x0000F4FC File Offset: 0x0000D6FC
		private int armorCompare(EquipmentElement first, EquipmentElement second)
		{
			float num = this.armorValue(first);
			float num2 = this.armorValue(second);
			if (num > num2)
			{
				return 1;
			}
			if (num < num2)
			{
				return -1;
			}
			return 0;
		}

		// Token: 0x060002C6 RID: 710 RVA: 0x0000F528 File Offset: 0x0000D728
		protected override void equip(EquipmentElement newEquipment, Hero companion)
		{
			EquipmentIndex equipmentIndex = this.equipmentIndexMap[newEquipment.Item.ItemType];
			companion.BattleEquipment.AddEquipmentToSlotWithoutAgent(equipmentIndex, newEquipment);
		}

		// Token: 0x060002C7 RID: 711 RVA: 0x0000F55C File Offset: 0x0000D75C
		protected override EquipmentElement getOldEquipment(ItemObject.ItemTypeEnum typeEnum, Hero companion)
		{
			EquipmentIndex equipmentIndex = this.equipmentIndexMap[typeEnum];
			return companion.BattleEquipment.GetEquipmentFromSlot(equipmentIndex);
		}

		// Token: 0x040000D3 RID: 211
		public List<ItemObject.ItemTypeEnum> supportedTypes = new List<ItemObject.ItemTypeEnum>
		{
			ItemObject.ItemTypeEnum.BodyArmor,
			ItemObject.ItemTypeEnum.LegArmor,
			ItemObject.ItemTypeEnum.Cape,
			ItemObject.ItemTypeEnum.HandArmor,
			ItemObject.ItemTypeEnum.HeadArmor
		};

		// Token: 0x040000D4 RID: 212
		private Dictionary<ItemObject.ItemTypeEnum, EquipmentIndex> equipmentIndexMap = new Dictionary<ItemObject.ItemTypeEnum, EquipmentIndex>
		{
			{
				ItemObject.ItemTypeEnum.BodyArmor,
				EquipmentIndex.Body
			},
			{
				ItemObject.ItemTypeEnum.LegArmor,
				EquipmentIndex.Leg
			},
			{
				ItemObject.ItemTypeEnum.Cape,
				EquipmentIndex.Cape
			},
			{
				ItemObject.ItemTypeEnum.HandArmor,
				EquipmentIndex.Gloves
			},
			{
				ItemObject.ItemTypeEnum.HeadArmor,
				EquipmentIndex.NumAllWeaponSlots
			}
		};

		// Token: 0x040000D5 RID: 213
		private float bodyArmorWeight = 1.5f;
	}
}
