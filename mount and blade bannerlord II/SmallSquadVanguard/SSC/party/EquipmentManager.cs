using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using SSC.misc;
using SSC.party.companions;
using SSC.party.equipment;
using SSC.settings;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

namespace SSC.party
{
	// Token: 0x02000052 RID: 82
	public class EquipmentManager : BaseManager
	{
		// Token: 0x06000272 RID: 626 RVA: 0x0000DE00 File Offset: 0x0000C000
		public void distributeInventory()
		{
			if (ModSettings.AutoEquipmentDisabled)
			{
				return;
			}
			Stopwatch stopwatch = Stopwatch.StartNew();
			if (ModSettings.AutoEquipArmors)
			{
				ItemRoster itemRoster = MobileParty.MainParty.ItemRoster;
				ArmorDistributor armorDistributor = new ArmorDistributor();
				foreach (ItemObject.ItemTypeEnum typeEnum in armorDistributor.supportedTypes)
				{
					List<Hero> eligibleCompanions = Equip.CompanionsToGetEquipmentExcept(HeroFlags.ArmorLocked);
					armorDistributor.distributeInventory(itemRoster, typeEnum, eligibleCompanions);
				}
			}
			if (ModSettings.AutoEquipWeapons)
			{
				ItemRoster itemRoster2 = MobileParty.MainParty.ItemRoster;
				WeaponDistributor weaponDistributor = new WeaponDistributor();
				foreach (ItemObject.ItemTypeEnum typeEnum2 in weaponDistributor.supportedTypes)
				{
					List<Hero> eligibleCompanions2 = Equip.CompanionsToGetEquipmentExcept(HeroFlags.WeaponsLocked);
					weaponDistributor.distributeInventory(itemRoster2, typeEnum2, eligibleCompanions2);
				}
				MissileDistributor.distributeThrown();
			}
			if (ModSettings.AutoEquipAmmo)
			{
				MissileDistributor.distributeArrows();
				MissileDistributor.distributeBolts();
			}
			if (ModSettings.AutoEquipHorse)
			{
				HorseDistributor.distributeHorses();
				HorseDistributor.distributeHarness();
			}
			stopwatch.Stop();
			long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
			if (ModSettings.AllAutoEquipmentIsEnabled)
			{
				Message.display(string.Format("Inventory distribution took {0} ms.", elapsedMilliseconds));
				return;
			}
			Message.display(string.Format("{0} distribution took {1} ms.", EquipmentManager.ActiveEquipment(), elapsedMilliseconds));
		}

		// Token: 0x06000273 RID: 627 RVA: 0x0000DF60 File Offset: 0x0000C160
		private static string ActiveEquipment()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (ModSettings.AutoEquipArmors)
			{
				stringBuilder.Append("armors, ");
			}
			if (ModSettings.AutoEquipWeapons)
			{
				stringBuilder.Append("weapons, ");
			}
			if (ModSettings.AutoEquipAmmo)
			{
				stringBuilder.Append("ammo, ");
			}
			if (ModSettings.AutoEquipHorse)
			{
				stringBuilder.Append("mounts, ");
			}
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Remove(stringBuilder.Length - 2, 2);
			}
			if (stringBuilder.Length > 0)
			{
				stringBuilder[0] = char.ToUpper(stringBuilder[0]);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000274 RID: 628 RVA: 0x0000DFFC File Offset: 0x0000C1FC
		private void fillTunicsQueue()
		{
			this.tunics.Enqueue("light_tunic");
			this.tunics.Enqueue("nordic_tunic");
			this.tunics.Enqueue("fine_town_tunic");
			this.tunics.Enqueue("cloth_tunic");
			this.tunics.Enqueue("sackcloth_tunic");
			this.tunics.Enqueue("hemp_tunic");
			this.tunics.Enqueue("tundra_tunic");
			this.tunics.Enqueue("footmans_tunic");
			this.tunics.Enqueue("northern_tunic");
			this.tunics.Enqueue("tied_cloth_tunic");
			this.tunics.Enqueue("tattered_rags");
		}

		// Token: 0x06000275 RID: 629 RVA: 0x0000E0BC File Offset: 0x0000C2BC
		private void fillWeaponsQueue()
		{
			this.weapons.Enqueue("peasant_pitchfork_1_t1");
			this.weapons.Enqueue("peasant_pitchfork_1_t1");
			this.weapons.Enqueue("peasant_hammer_2_t1");
			this.weapons.Enqueue("peasant_pitchfork_1_t1");
			this.weapons.Enqueue("peasant_pitchfork_1_t1");
			this.weapons.Enqueue("peasant_pitchfork_1_t1");
			this.weapons.Enqueue("peasant_hammer_2_t1");
			this.weapons.Enqueue("peasant_pitchfork_2_t1");
			this.weapons.Enqueue("peasant_pitchfork_1_t1");
			this.weapons.Enqueue("peasant_polearm_1_t1");
			this.weapons.Enqueue("peasant_hammer_2_t1");
		}

		// Token: 0x06000276 RID: 630 RVA: 0x0000E17C File Offset: 0x0000C37C
		public void removeAll(Equipment equipment)
		{
			for (int i = 0; i < 12; i++)
			{
				EquipmentIndex index = (EquipmentIndex)i;
				this.remove(index, equipment);
			}
		}

		// Token: 0x06000277 RID: 631 RVA: 0x0000E1A1 File Offset: 0x0000C3A1
		public EquipmentElement remove(EquipmentIndex index, Equipment equipment)
		{
			EquipmentElement equipmentFromSlot = equipment.GetEquipmentFromSlot(index);
			equipment.AddEquipmentToSlotWithoutAgent(index, EquipmentElement.Invalid);
			return equipmentFromSlot;
		}

		// Token: 0x06000278 RID: 632 RVA: 0x0000E1B8 File Offset: 0x0000C3B8
		public void setPitchforkBasedEquipment(Hero companion)
		{
			Manage.Equipment.removeAll(companion.BattleEquipment);
			Manage.Equipment.removeAll(companion.CivilianEquipment);
			if (this.tunics.IsEmpty<string>())
			{
				this.fillTunicsQueue();
			}
			ItemObject item = this.load("cloth_tunic");
			this.putElement(item, EquipmentIndex.Body, companion.BattleEquipment);
			this.putElement(item, EquipmentIndex.Body, companion.CivilianEquipment);
			if (this.weapons.IsEmpty<string>())
			{
				this.fillWeaponsQueue();
			}
			ItemObject item2 = this.load("peasant_pitchfork_1_t1");
			this.putElement(item2, EquipmentIndex.WeaponItemBeginSlot, companion.BattleEquipment);
		}

		// Token: 0x06000279 RID: 633 RVA: 0x0000E24D File Offset: 0x0000C44D
		public ItemObject load(string stringId)
		{
			return MBObjectManager.Instance.GetObject<ItemObject>(stringId);
		}

		// Token: 0x0600027A RID: 634 RVA: 0x0000E25A File Offset: 0x0000C45A
		public void putArmor(string stringId, Equipment equipment)
		{
			this.putElement(this.load(stringId), EquipmentIndex.Body, equipment);
		}

		// Token: 0x0600027B RID: 635 RVA: 0x0000E26B File Offset: 0x0000C46B
		public void putElement(ItemObject item, EquipmentIndex index, Equipment equipment)
		{
			if (Equipment.IsItemFitsToSlot(index, item))
			{
				equipment[index] = new EquipmentElement(item, null, null, false);
			}
		}

		// Token: 0x040000C5 RID: 197
		private Queue<string> tunics = new Queue<string>();

		// Token: 0x040000C6 RID: 198
		private Queue<string> weapons = new Queue<string>();
	}
}
