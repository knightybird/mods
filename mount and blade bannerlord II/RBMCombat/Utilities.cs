using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using RBMConfig;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace RBMCombat
{
	// Token: 0x02000017 RID: 23
	public static class Utilities
	{
		// Token: 0x0600005B RID: 91
		public static bool ThurstWithTip(in AttackCollisionData collisionData, in MissionWeapon attackerWeapon)
		{
			MissionWeapon missionWeapon = attackerWeapon;
			WeaponComponentData currentUsageItem = missionWeapon.CurrentUsageItem;
			missionWeapon = attackerWeapon;
			bool flag;
			if (missionWeapon.Item != null && currentUsageItem != null)
			{
				missionWeapon = attackerWeapon;
				if (missionWeapon.Item.WeaponDesign != null)
				{
					missionWeapon = attackerWeapon;
					if (missionWeapon.Item.WeaponDesign.UsedPieces != null)
					{
						missionWeapon = attackerWeapon;
						flag = (missionWeapon.Item.WeaponDesign.UsedPieces.Length != 0);
						goto IL_78;
					}
				}
			}
			flag = false;
			IL_78:
			bool result;
			if (flag)
			{
				missionWeapon = attackerWeapon;
				if (missionWeapon.CurrentUsageItem != null)
				{
					missionWeapon = attackerWeapon;
					if (missionWeapon.CurrentUsageItem.WeaponClass - WeaponClass.Dagger <= 2)
					{
					}
				}
				missionWeapon = attackerWeapon;
				float scaledBladeLength = missionWeapon.Item.WeaponDesign.UsedPieces[0].ScaledBladeLength;
				float realWeaponLength = currentUsageItem.GetRealWeaponLength();
				AttackCollisionData attackCollisionData = collisionData;
				result = (attackCollisionData.CollisionDistanceOnWeapon / realWeaponLength >= 0.85f);
			}
			else
			{
				result = true;
			}
			return result;
		}

		// Token: 0x0600005C RID: 92
		public static bool HitWithWeaponBlade(in AttackCollisionData collisionData, in MissionWeapon attackerWeapon)
		{
			MissionWeapon missionWeapon = attackerWeapon;
			WeaponComponentData currentUsageItem = missionWeapon.CurrentUsageItem;
			missionWeapon = attackerWeapon;
			bool flag;
			if (missionWeapon.Item != null && currentUsageItem != null)
			{
				missionWeapon = attackerWeapon;
				if (missionWeapon.Item.WeaponDesign != null)
				{
					missionWeapon = attackerWeapon;
					if (missionWeapon.Item.WeaponDesign.UsedPieces != null)
					{
						missionWeapon = attackerWeapon;
						flag = (missionWeapon.Item.WeaponDesign.UsedPieces.Length != 0);
						goto IL_78;
					}
				}
			}
			flag = false;
			IL_78:
			bool result;
			if (flag)
			{
				bool isSwordType = false;
				missionWeapon = attackerWeapon;
				if (missionWeapon.CurrentUsageItem != null)
				{
					missionWeapon = attackerWeapon;
					if (missionWeapon.CurrentUsageItem.WeaponClass - WeaponClass.Dagger <= 2)
					{
						isSwordType = true;
					}
				}
				missionWeapon = attackerWeapon;
				float bladeLength = missionWeapon.Item.WeaponDesign.UsedPieces[0].ScaledBladeLength + (isSwordType ? 0f : 0.15f);
				float realWeaponLength = currentUsageItem.GetRealWeaponLength();
				AttackCollisionData attackCollisionData = collisionData;
				result = (attackCollisionData.CollisionDistanceOnWeapon >= realWeaponLength - bladeLength);
			}
			else
			{
				result = true;
			}
			return result;
		}

		// Token: 0x0600005D RID: 93
		public static void SimulateThrustLayer(double distance, double usablePower, double maxUsableForce, double mass, out double finalSpeed, out double finalTime)
		{
			double num = 0.0;
			double num2 = 0.01;
			double num3 = 0.0;
			while (num < distance)
			{
				double num4 = usablePower / num2;
				if (num4 > maxUsableForce)
				{
					num4 = maxUsableForce;
				}
				double num5 = 0.01 * num4 / mass;
				num2 += num5;
				num += num2 * 0.01;
				num3 += 0.01;
			}
			finalSpeed = num2;
			finalTime = num3;
		}

		// Token: 0x0600005E RID: 94
		public static float CalculateThrustSpeed(float _currentWeaponWeight, float inertia, float com)
		{
			float _currentWeaponInertiaAroundGrip = inertia + _currentWeaponWeight * com * com;
			double num = 1.8 + (double)_currentWeaponWeight + (double)_currentWeaponInertiaAroundGrip * 0.2;
			double num2 = 170.0;
			double num3 = 90.0;
			double num4 = 24.0;
			double num5 = 15.0;
			double finalSpeed;
			double finalTime;
			Utilities.SimulateThrustLayer(0.6, 250.0, 48.0, 4.0 + num, out finalSpeed, out finalTime);
			double finalSpeed2;
			double finalTime2;
			Utilities.SimulateThrustLayer(0.6, num2, num4, 2.0 + num, out finalSpeed2, out finalTime2);
			double finalSpeed3;
			double finalTime3;
			Utilities.SimulateThrustLayer(0.6, num3, num5, 0.5 + num, out finalSpeed3, out finalTime3);
			double num6 = 0.33 * (finalTime + finalTime2 + finalTime3);
			return (float)(3.8500000000000005 / num6);
		}

		// Token: 0x0600005F RID: 95
		public static float CalculateSkillModifier(int relevantSkillLevel)
		{
			return MBMath.ClampFloat((float)relevantSkillLevel / 250f, 0f, 1f);
		}

		// Token: 0x06000060 RID: 96
		public static float CalculateSkillModifier(float relevantSkillLevel)
		{
			return MBMath.ClampFloat(relevantSkillLevel / 250f, 0f, 1f);
		}

		// Token: 0x06000061 RID: 97
		public static float GetEffectiveSkillWithDR(int effectiveSkill)
		{
			return 600f / (600f + (float)effectiveSkill) * (float)effectiveSkill;
		}

		// Token: 0x06000062 RID: 98
		public static bool HitWithWeaponBladeTip(in AttackCollisionData collisionData, in MissionWeapon attackerWeapon)
		{
			MissionWeapon missionWeapon = attackerWeapon;
			WeaponComponentData currentUsageItem = missionWeapon.CurrentUsageItem;
			bool result;
			if (currentUsageItem != null)
			{
				missionWeapon = attackerWeapon;
				WeaponClass weaponClass = missionWeapon.CurrentUsageItem.WeaponClass;
				AttackCollisionData attackCollisionData = collisionData;
				result = (attackCollisionData.CollisionDistanceOnWeapon > currentUsageItem.GetRealWeaponLength() * 0.95f);
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000063 RID: 99
		public static int calculateMissileSpeed(float ammoWeight, string rangedWeaponType, int drawWeight)
		{
			if (rangedWeaponType != null)
			{
				switch (rangedWeaponType.Length)
				{
				case 3:
					if (rangedWeaponType == "bow")
					{
						float powerstroke = 0.635f;
						double num = (double)(0.5f * ((float)drawWeight * 4.448f) * powerstroke * 0.91f);
						ammoWeight += (float)drawWeight * 0.00012f;
						return (int)Math.Floor(Math.Sqrt(num * 2.0 / (double)ammoWeight));
					}
					return 10;
				case 4:
				case 5:
				case 6:
				case 7:
				case 11:
					return 10;
				case 8:
				{
					char c = rangedWeaponType[1];
					if (c != 'l')
					{
						if (c != 'o')
						{
							if (c != 'r')
							{
								return 10;
							}
							if (!(rangedWeaponType == "crossbow"))
							{
								return 10;
							}
						}
						else
						{
							if (rangedWeaponType == "long_bow")
							{
								float powerstroke2 = 0.635f;
								double num2 = (double)(0.5f * ((float)drawWeight * 4.448f) * powerstroke2 * 0.89f);
								ammoWeight += (float)drawWeight * 0.0002f;
								return (int)Math.Floor(Math.Sqrt(num2 * 2.0 / (double)ammoWeight));
							}
							return 10;
						}
					}
					else
					{
						if (rangedWeaponType == "cla_bomb")
						{
							return (int)Math.Floor(Math.Sqrt(150.0 * 2.0 / (double)ammoWeight));
						}
						return 10;
					}
					break;
				}
				case 9:
					if (rangedWeaponType == "osa_sling")
					{
						return (int)Math.Floor(Math.Sqrt((double)(0.5f * (float)(drawWeight * drawWeight) * 0.12f) * 2.0 / (double)(ammoWeight + 0.04f)));
					}
					return 10;
				case 10:
				{
					char c2 = rangedWeaponType[4];
					if (c2 != 'c')
					{
						if (c2 != 'm')
						{
							if (c2 == 'p' && rangedWeaponType == "cla_pistol")
							{
								return (int)Math.Floor(Math.Sqrt((double)drawWeight * 2.0 / (double)ammoWeight));
							}
							return 10;
						}
						else
						{
							if (rangedWeaponType == "cla_musket")
							{
								return (int)Math.Floor(Math.Sqrt((double)drawWeight * 2.0 / (double)ammoWeight));
							}
							return 10;
						}
					}
					else
					{
						if (rangedWeaponType == "cla_cannon")
						{
							return (int)Math.Floor(Math.Sqrt((double)drawWeight * 2.0 / (double)ammoWeight));
						}
						return 10;
					}
					break;
				}
				case 12:
					if (rangedWeaponType == "cla_revolver")
					{
						return (int)Math.Floor(Math.Sqrt((double)drawWeight * 2.0 / (double)ammoWeight));
					}
					return 10;
				case 13:
					if (!(rangedWeaponType == "crossbow_fast"))
					{
						return 10;
					}
					break;
				case 14:
					if (rangedWeaponType == "cla_bolt_rifle")
					{
						return (int)Math.Floor(Math.Sqrt((double)drawWeight * 2.0 / (double)ammoWeight));
					}
					return 10;
				case 15:
					if (rangedWeaponType == "cla_flint_rifle")
					{
						return (int)Math.Floor(Math.Sqrt((double)drawWeight * 2.0 / (double)ammoWeight));
					}
					return 10;
				default:
					return 10;
				}
				float powerstroke3 = 0.3048f;
				double num3 = (double)(0.5f * ((float)drawWeight * 4.448f) * powerstroke3 * 0.7f);
				ammoWeight += (float)drawWeight * 0.00013f;
				return (int)Math.Floor(Math.Sqrt(num3 * 2.0 / (double)ammoWeight));
			}
			return 10;
		}

		// Token: 0x06000064 RID: 100
		public static int calculateThrowableSpeed(float ammoWeight, float effectiveSkill)
		{
			int calculatedThrowingSpeed = (int)Math.Ceiling(Math.Sqrt((double)((60f + effectiveSkill * 1f) * 2f / ammoWeight)));
			if (calculatedThrowingSpeed > 32)
			{
				calculatedThrowingSpeed = 32;
			}
			return calculatedThrowingSpeed;
		}

		// Token: 0x06000065 RID: 101
		public static int assignThrowableMissileSpeedForMenu(float ammoWeight, int correctiveMissileSpeed, float effectiveSkill)
		{
			return Utilities.calculateThrowableSpeed(ammoWeight, effectiveSkill) + correctiveMissileSpeed;
		}

		// Token: 0x06000066 RID: 102
		public static int assignThrowableMissileSpeed(float ammoWeight, int correctiveMissileSpeed, float effectiveSkill, float equipmentWeight, WeaponClass shieldType)
		{
			float shieldTypeModifier = 1f;
			float equipmentWeightModifier = (float)Math.Sqrt((double)MBMath.ClampFloat(1f - equipmentWeight * 0.012f, 0.5f, 1f));
			if (shieldType != WeaponClass.SmallShield)
			{
				if (shieldType == WeaponClass.LargeShield)
				{
					shieldTypeModifier = 0.87f;
				}
			}
			else
			{
				shieldTypeModifier = 0.96f;
			}
			return (int)Math.Round((double)((float)Utilities.calculateThrowableSpeed(ammoWeight, effectiveSkill) * shieldTypeModifier * equipmentWeightModifier)) + correctiveMissileSpeed;
		}

		// Token: 0x06000067 RID: 103
		public static int assignStoneMissileSpeed(MissionWeapon throwable)
		{
			return 25;
		}

		// Token: 0x06000068 RID: 104
		public static void initiateCheckForArmor(ref Agent victim, AttackCollisionData attackCollisionData, Blow blow, Agent affectorAgent, in MissionWeapon attackerWeapon)
		{
			BoneBodyPartType bodyPartHit = attackCollisionData.VictimHitBodyPart;
			EquipmentIndex equipmentIndex = EquipmentIndex.None;
			ItemObject.ItemTypeEnum itemType = ItemObject.ItemTypeEnum.Invalid;
			if (!victim.IsHuman)
			{
				equipmentIndex = EquipmentIndex.HorseHarness;
				itemType = ItemObject.ItemTypeEnum.HorseHarness;
			}
			else
			{
				switch (bodyPartHit)
				{
				case BoneBodyPartType.Head:
				case BoneBodyPartType.Neck:
					equipmentIndex = EquipmentIndex.NumAllWeaponSlots;
					itemType = ItemObject.ItemTypeEnum.HeadArmor;
					break;
				case BoneBodyPartType.Chest:
				case BoneBodyPartType.Abdomen:
					equipmentIndex = EquipmentIndex.Body;
					itemType = ItemObject.ItemTypeEnum.BodyArmor;
					break;
				case BoneBodyPartType.ShoulderLeft:
				case BoneBodyPartType.ShoulderRight:
					equipmentIndex = EquipmentIndex.Cape;
					itemType = ItemObject.ItemTypeEnum.Cape;
					break;
				case BoneBodyPartType.ArmLeft:
				case BoneBodyPartType.ArmRight:
					equipmentIndex = EquipmentIndex.Gloves;
					itemType = ItemObject.ItemTypeEnum.HandArmor;
					break;
				case BoneBodyPartType.Legs:
					equipmentIndex = EquipmentIndex.Leg;
					itemType = ItemObject.ItemTypeEnum.LegArmor;
					break;
				}
			}
			if (equipmentIndex != EquipmentIndex.None && itemType > ItemObject.ItemTypeEnum.Invalid)
			{
				Utilities.lowerArmorQualityCheck(ref victim, equipmentIndex, itemType, attackCollisionData, blow, affectorAgent, attackerWeapon);
			}
		}

		// Token: 0x06000069 RID: 105
		public static void lowerArmorQualityCheck(ref Agent agent, EquipmentIndex equipmentIndex, ItemObject.ItemTypeEnum itemType, AttackCollisionData attackCollisionData, Blow blow, Agent attacker, in MissionWeapon attackerWeapon)
		{
			EquipmentElement equipmentElement = agent.SpawnEquipment[equipmentIndex];
			bool flag;
			if (equipmentElement.Item != null && equipmentElement.Item.ItemType == itemType)
			{
				MissionWeapon missionWeapon = attackerWeapon;
				if (!missionWeapon.IsEmpty && blow.InflictedDamage > 1)
				{
					flag = !blow.IsFallDamage;
					goto IL_50;
				}
			}
			flag = false;
			IL_50:
			if (flag)
			{
				MissionWeapon missionWeapon2 = attackerWeapon;
				WeaponClass weaponType = missionWeapon2.CurrentUsageItem.WeaponClass;
				float weaponTypeScaling = 1f;
				float magnitude = blow.BaseMagnitude;
				RBMCombatConfigWeaponType rbmCombatConfigWeaponType = getWeaponTypeFactors(weaponType.ToString());
				float armorThreshold = 4f;
				float armorValue = ArmorRework.GetBaseArmorEffectivenessForBodyPartRBM(agent, attackCollisionData.VictimHitBodyPart);
				ArmorComponent.ArmorMaterialTypes armorMaterialType = equipmentElement.Item.ArmorComponent.MaterialType;
				DamageTypes damageType = (DamageTypes)attackCollisionData.DamageType;
				if (attacker.IsHuman && attacker.GetWieldedItemIndex(Agent.HandIndex.MainHand) != EquipmentIndex.None)
				{
					missionWeapon2 = attackerWeapon;
					WeaponComponentData wcd = missionWeapon2.CurrentUsageItem;
					ItemModifier itemModifier = null;
					if (!attackCollisionData.IsAlternativeAttack && attacker.IsHuman && !attackCollisionData.IsFallDamage && attacker.Origin != null && !attackCollisionData.IsMissile && wcd != null)
					{
						if (!attackCollisionData.IsMissile)
						{
							float wdm = MissionGameModels.Current.AgentStatCalculateModel.GetWeaponDamageMultiplier(attacker, wcd);
							magnitude = attackCollisionData.BaseMagnitude / wdm;
						}
						SkillObject skill = (wcd == null) ? DefaultSkills.Athletics : wcd.RelevantSkill;
						if (skill != null)
						{
							int effectiveSkill3 = MissionGameModels.Current.AgentStatCalculateModel.GetEffectiveSkill(attacker, skill);
							float effectiveSkill = Utilities.GetEffectiveSkillWithDR(effectiveSkill3);
							float skillModifier = Utilities.CalculateSkillModifier(effectiveSkill3);
							if (attacker != null && attacker.Equipment != null && attacker.GetWieldedItemIndex(Agent.HandIndex.MainHand) != EquipmentIndex.None)
							{
								missionWeapon2 = attacker.Equipment[attacker.GetWieldedItemIndex(Agent.HandIndex.MainHand)];
								itemModifier = missionWeapon2.ItemModifier;
								float baseMagnitude = blow.BaseMagnitude;
								bool isDoingPassiveAttack = attacker.IsDoingPassiveAttack;
								string weaponType2 = weaponType.ToString();
								DamageTypes damageType2 = damageType;
								float effectiveSkill2 = effectiveSkill;
								float skillModifier2 = skillModifier;
								StrikeType strikeType = (StrikeType)attackCollisionData.StrikeType;
								missionWeapon2 = attacker.Equipment[attacker.GetWieldedItemIndex(Agent.HandIndex.MainHand)];
								magnitude = Utilities.GetSkillBasedDamage(baseMagnitude, isDoingPassiveAttack, weaponType2, damageType2, effectiveSkill2, skillModifier2, strikeType, missionWeapon2.GetWeight());
							}
						}
					}
					Math.Sqrt((double)((attackCollisionData.StrikeType == 1) ? Utilities.getThrustDamageFactor(wcd, itemModifier) : Utilities.getSwingDamageFactor(wcd, itemModifier)));
				}
				bool flag2;
				if (attacker != null && attackCollisionData.StrikeType == 0 && !attackCollisionData.AttackBlockedWithShield)
				{
					missionWeapon2 = attacker.WieldedWeapon;
					if (!missionWeapon2.IsEmpty)
					{
						missionWeapon2 = attacker.WieldedWeapon;
						flag2 = !Utilities.HitWithWeaponBlade(attackCollisionData, missionWeapon2);
						goto IL_2A7;
					}
				}
				flag2 = false;
				IL_2A7:
				if (flag2)
				{
					damageType = DamageTypes.Blunt;
				}
				switch (damageType)
				{
				case DamageTypes.Cut:
				{
					if (rbmCombatConfigWeaponType != null)
					{
						armorThreshold = rbmCombatConfigWeaponType.ExtraArmorThresholdFactorCut;
					}
					WeaponClass weaponClass2 = weaponType;
					if (weaponClass2 - WeaponClass.Dagger > 1)
					{
						if (weaponClass2 != WeaponClass.TwoHandedSword)
						{
							switch (armorMaterialType)
							{
							case ArmorComponent.ArmorMaterialTypes.Cloth:
							case ArmorComponent.ArmorMaterialTypes.Leather:
								weaponTypeScaling = 2f;
								break;
							case ArmorComponent.ArmorMaterialTypes.Chainmail:
								weaponTypeScaling = 2f;
								break;
							case ArmorComponent.ArmorMaterialTypes.Plate:
								weaponTypeScaling = 4f;
								break;
							}
						}
						else
						{
							switch (armorMaterialType)
							{
							case ArmorComponent.ArmorMaterialTypes.Cloth:
							case ArmorComponent.ArmorMaterialTypes.Leather:
								weaponTypeScaling = 5f;
								break;
							case ArmorComponent.ArmorMaterialTypes.Chainmail:
								weaponTypeScaling = 1.25f;
								break;
							case ArmorComponent.ArmorMaterialTypes.Plate:
								weaponTypeScaling = 2.5f;
								break;
							}
						}
					}
					else
					{
						switch (armorMaterialType)
						{
						case ArmorComponent.ArmorMaterialTypes.Cloth:
						case ArmorComponent.ArmorMaterialTypes.Leather:
							weaponTypeScaling = 5f;
							break;
						case ArmorComponent.ArmorMaterialTypes.Chainmail:
							weaponTypeScaling = 1f;
							break;
						case ArmorComponent.ArmorMaterialTypes.Plate:
							weaponTypeScaling = 2f;
							break;
						}
					}
					break;
				}
				case DamageTypes.Pierce:
					if (rbmCombatConfigWeaponType != null)
					{
						armorThreshold = rbmCombatConfigWeaponType.ExtraArmorThresholdFactorPierce;
					}
					weaponTypeScaling = 1f;
					break;
				case DamageTypes.Blunt:
				{
					ArmorComponent.ArmorMaterialTypes armorMaterialTypes2 = armorMaterialType;
					if (armorMaterialTypes2 - ArmorComponent.ArmorMaterialTypes.Cloth > 2)
					{
						if (armorMaterialTypes2 == ArmorComponent.ArmorMaterialTypes.Plate)
						{
							weaponTypeScaling = 12f;
						}
					}
					else
					{
						weaponTypeScaling = 1f;
					}
					break;
				}
				}
				float num = 0.05f;
				if (damageType == DamageTypes.Pierce && !blow.IsMissile)
				{
					magnitude *= 20f;
				}
				float magScaling = blow.AbsorbedByArmor / (armorValue * armorThreshold) / 5f;
				float scaledProbability = num + magScaling * weaponTypeScaling;
				if (MBRandom.RandomFloat <= scaledProbability)
				{
					Utilities.lowerArmorQuality(ref agent, equipmentIndex, itemType);
				}
			}
		}

		// Token: 0x0600006A RID: 106
		public static void lowerArmorQuality(ref Agent agent, EquipmentIndex equipmentIndex, ItemObject.ItemTypeEnum itemType)
		{
			EquipmentElement equipmentElement = agent.SpawnEquipment[equipmentIndex];
			if (equipmentElement.Item != null && equipmentElement.Item.ItemType == itemType && equipmentElement.Item != null)
			{
				int currentModifier = 0;
				if (equipmentElement.ItemModifier != null)
				{
					string stringId = equipmentElement.ItemModifier.StringId;
					currentModifier = equipmentElement.ItemModifier.ModifyArmor(100) - 100;
				}
				ItemModifier newIM = equipmentElement.ItemModifier;
				ItemObject item = equipmentElement.Item;
				IReadOnlyList<ItemModifier> readOnlyList;
				if (item == null)
				{
					readOnlyList = null;
				}
				else
				{
					ItemComponent itemComponent = item.ItemComponent;
					if (itemComponent == null)
					{
						readOnlyList = null;
					}
					else
					{
						ItemModifierGroup itemModifierGroup = itemComponent.ItemModifierGroup;
						readOnlyList = ((itemModifierGroup != null) ? itemModifierGroup.ItemModifiers : null);
					}
				}
				IReadOnlyList<ItemModifier> itemModifiers = readOnlyList;
				if (itemModifiers != null && itemModifiers.Count > 0)
				{
					foreach (ItemModifier im in itemModifiers)
					{
						int tempIm = im.ModifyArmor(100) - 100;
						if (equipmentElement.ItemModifier == null && tempIm < 0)
						{
							newIM = im;
							break;
						}
						if (!currentModifier.Equals(im) && currentModifier > tempIm)
						{
							newIM = im;
							break;
						}
					}
				}
				if (currentModifier > 0 && newIM != null && newIM.ModifyArmor(100) - 100 < 0)
				{
					equipmentElement.SetModifier(null);
					agent.SpawnEquipment[equipmentIndex] = equipmentElement;
					return;
				}
				if (newIM != null || equipmentElement.ItemModifier == null)
				{
					equipmentElement.SetModifier(newIM);
					agent.SpawnEquipment[equipmentIndex] = equipmentElement;
				}
			}
		}

		// Token: 0x0600006B RID: 107
		public static float GetSkillBasedDamage(float magnitude, bool isPassiveUsage, string weaponType, DamageTypes damageType, float effectiveSkill, float skillModifier, StrikeType strikeType, float weaponWeight)
		{
			float skillBasedDamage = 0f;
			float BraceBonus = 0f;
			float BraceModifier = 1f;
			if (weaponType != null)
			{
				int length = weaponType.Length;
				switch (length)
				{
				case 4:
					if (!(weaponType == "Mace"))
					{
						return magnitude;
					}
					if (damageType == DamageTypes.Pierce)
					{
						skillBasedDamage = magnitude;
					}
					else
					{
						float value = magnitude + effectiveSkill * 0.075f;
						float min = 10f * (1f + skillModifier);
						float max = 15f * (1f + 2f * skillModifier);
						skillBasedDamage = MBMath.ClampFloat(value, min, max) * 4.6f;
					}
					if (magnitude > 1f)
					{
						magnitude = skillBasedDamage;
					}
					return magnitude;
				case 5:
				case 7:
				case 8:
				case 9:
				case 10:
				case 15:
					return magnitude;
				case 6:
					if (!(weaponType == "Dagger"))
					{
						return magnitude;
					}
					break;
				case 11:
					if (!(weaponType == "ThrowingAxe"))
					{
						return magnitude;
					}
					goto IL_942;
				case 12:
				{
					char c = weaponType[0];
					if (c != 'O')
					{
						if (c != 'T')
						{
							return magnitude;
						}
						if (!(weaponType == "TwoHandedAxe"))
						{
							return magnitude;
						}
						float value2 = magnitude + effectiveSkill * 0.15f;
						float min2 = 15f * (1f + skillModifier);
						float max2 = 24f * (1f + 2f * skillModifier);
						skillBasedDamage = MBMath.ClampFloat(value2, min2, max2) * 4.6f;
						if (damageType == DamageTypes.Blunt)
						{
							skillBasedDamage = MBMath.ClampFloat(magnitude + effectiveSkill * 0.112f, 20f * (1f + skillModifier), 26f * (1f + 2f * skillModifier)) * 4f * 0.3f;
						}
						if (magnitude > 1f)
						{
							magnitude = skillBasedDamage;
						}
						return magnitude;
					}
					else
					{
						if (!(weaponType == "OneHandedAxe"))
						{
							return magnitude;
						}
						goto IL_942;
					}
					break;
				}
				case 13:
				{
					char c2 = weaponType[1];
					if (c2 != 'h')
					{
						if (c2 != 'w')
						{
							return magnitude;
						}
						if (!(weaponType == "TwoHandedMace"))
						{
							return magnitude;
						}
						if (damageType == DamageTypes.Pierce)
						{
							skillBasedDamage = (magnitude * 0.2f + 40f * 0.05f + effectiveSkill * 0.4f * 0.05f) * 1.3f;
						}
						else
						{
							skillBasedDamage = MBMath.ClampFloat(magnitude + effectiveSkill * 0.112f, 20f * (1f + skillModifier), 26f * (1f + 2f * skillModifier)) * 4.6f;
						}
						if (magnitude > 1f)
						{
							magnitude = skillBasedDamage;
						}
						return magnitude;
					}
					else if (!(weaponType == "ThrowingKnife"))
					{
						return magnitude;
					}
					break;
				}
				case 14:
				{
					char c3 = weaponType[0];
					if (c3 != 'O')
					{
						if (c3 != 'T')
						{
							return magnitude;
						}
						if (!(weaponType == "TwoHandedSword"))
						{
							return magnitude;
						}
						if (damageType == DamageTypes.Cut)
						{
							float value3 = magnitude + effectiveSkill * 0.199f;
							float min3 = 12f * (1f + skillModifier);
							float max3 = 20f * (1f + 2f * skillModifier);
							skillBasedDamage = MBMath.ClampFloat(value3, min3, max3) * 4.6f;
						}
						else if (damageType == DamageTypes.Blunt)
						{
							skillBasedDamage = MBMath.ClampFloat(magnitude + effectiveSkill * 0.112f, 20f * (1f + skillModifier), 26f * (1f + 2f * skillModifier)) * 4f * 0.4f;
						}
						else if (strikeType == StrikeType.Swing)
						{
							skillBasedDamage = MBMath.ClampFloat(magnitude + effectiveSkill * 0.199f, 12f * (1f + skillModifier), 20f * (1f + 2f * skillModifier)) * 4f * 0.05f;
						}
						else
						{
							skillBasedDamage = magnitude;
						}
						if (magnitude > 1f)
						{
							magnitude = skillBasedDamage;
						}
						return magnitude;
					}
					else if (!(weaponType == "OneHandedSword"))
					{
						return magnitude;
					}
					break;
				}
				case 16:
				{
					char c4 = weaponType[0];
					if (c4 != 'O')
					{
						if (c4 != 'T')
						{
							return magnitude;
						}
						if (!(weaponType == "TwoHandedPolearm"))
						{
							return magnitude;
						}
						if (damageType == DamageTypes.Cut)
						{
							float value4 = magnitude + effectiveSkill * 0.1495f;
							float min4 = 18f * (1f + skillModifier);
							float max4 = 28f * (1f + 2f * skillModifier);
							skillBasedDamage = MBMath.ClampFloat(value4, min4, max4) * 4f;
						}
						else if (damageType == DamageTypes.Blunt && !isPassiveUsage)
						{
							skillBasedDamage = MBMath.ClampFloat(magnitude + effectiveSkill * 0.0975f, 20f * (1f + skillModifier), 26f * (1f + 2f * skillModifier)) * 4f * 0.3f;
						}
						else if (isPassiveUsage)
						{
							float couchedSkill = 0.5f + effectiveSkill * 0.02f;
							float skillCap = 150f + effectiveSkill * 1.5f;
							if (weaponWeight < 2.1f)
							{
								BraceBonus += 0.5f;
								BraceModifier *= 1f;
							}
							float lanceBalistics = magnitude * BraceModifier / weaponWeight;
							float CouchedMagnitude = lanceBalistics * (weaponWeight + couchedSkill + BraceBonus);
							float BluntLanceBalistics = magnitude * BraceModifier / weaponWeight * 20f;
							float BluntCouchedMagnitude = lanceBalistics * (weaponWeight + couchedSkill + BraceBonus) * 20f;
							magnitude = CouchedMagnitude;
							if (damageType == DamageTypes.Blunt)
							{
								magnitude = BluntCouchedMagnitude;
								if (BluntCouchedMagnitude > skillCap && BluntLanceBalistics * (weaponWeight + BraceBonus) < skillCap)
								{
									magnitude = skillCap;
								}
								if (BluntLanceBalistics * (weaponWeight + BraceBonus) >= skillCap)
								{
									magnitude = BluntLanceBalistics * (weaponWeight + BraceBonus);
								}
								if (magnitude > 260f)
								{
									magnitude = 260f;
								}
								magnitude *= 1f;
							}
							else
							{
								if (CouchedMagnitude > skillCap * 0.05f && lanceBalistics * (weaponWeight + BraceBonus) < skillCap * 0.05f)
								{
									magnitude = skillCap * 0.05f;
								}
								if (lanceBalistics * (weaponWeight + BraceBonus) >= skillCap * 0.05f)
								{
									magnitude = lanceBalistics * (weaponWeight + BraceBonus);
								}
								if (magnitude > 430f * 0.05f)
								{
									magnitude = 430f * 0.05f;
								}
							}
						}
						else
						{
							skillBasedDamage = magnitude;
						}
						if (magnitude > 0.15f && !isPassiveUsage)
						{
							magnitude = skillBasedDamage;
						}
						return magnitude;
					}
					else
					{
						if (!(weaponType == "OneHandedPolearm"))
						{
							return magnitude;
						}
						if (damageType == DamageTypes.Cut)
						{
							skillBasedDamage = MBMath.ClampFloat(magnitude + effectiveSkill * 0.1f, 15f * (1f + skillModifier), 24f * (1f + 2f * skillModifier)) * 4f;
						}
						else if (damageType == DamageTypes.Blunt && !isPassiveUsage)
						{
							skillBasedDamage = MBMath.ClampFloat(magnitude + effectiveSkill * 0.075f, 15f * (1f + skillModifier), 20f * (1f + 2f * skillModifier)) * 4f * 0.3f;
						}
						else if (isPassiveUsage)
						{
							float couchedSkill2 = 0.5f + effectiveSkill * 0.02f;
							float skillCap2 = 150f + effectiveSkill * 1.5f;
							if (weaponWeight < 2.1f)
							{
								BraceBonus += 0.5f;
								BraceModifier *= 1f;
							}
							float lanceBalistics2 = magnitude * BraceModifier / weaponWeight;
							float CouchedMagnitude2 = lanceBalistics2 * (weaponWeight + couchedSkill2 + BraceBonus);
							float BluntLanceBalistics2 = magnitude * BraceModifier / weaponWeight * 20f;
							float BluntCouchedMagnitude2 = lanceBalistics2 * (weaponWeight + couchedSkill2 + BraceBonus) * 20f;
							magnitude = CouchedMagnitude2;
							if (damageType == DamageTypes.Blunt)
							{
								magnitude = BluntCouchedMagnitude2;
								if (BluntCouchedMagnitude2 > skillCap2 && BluntLanceBalistics2 * (weaponWeight + BraceBonus) < skillCap2)
								{
									magnitude = skillCap2;
								}
								if (BluntLanceBalistics2 * (weaponWeight + BraceBonus) >= skillCap2)
								{
									magnitude = BluntLanceBalistics2 * (weaponWeight + BraceBonus);
								}
								if (magnitude > 260f)
								{
									magnitude = 260f;
								}
								magnitude *= 1f;
							}
							else
							{
								if (CouchedMagnitude2 > skillCap2 * 0.05f && lanceBalistics2 * (weaponWeight + BraceBonus) < skillCap2 * 0.05f)
								{
									magnitude = skillCap2 * 0.05f;
								}
								if (lanceBalistics2 * (weaponWeight + BraceBonus) >= skillCap2 * 0.05f)
								{
									magnitude = lanceBalistics2 * (weaponWeight + BraceBonus);
								}
								if (magnitude > 430f * 0.05f)
								{
									magnitude = 430f * 0.05f;
								}
							}
						}
						else
						{
							Math.Sqrt((double)(magnitude * 2f / 8f));
							skillBasedDamage = magnitude;
						}
						if (magnitude > 0.15f && !isPassiveUsage)
						{
							magnitude = skillBasedDamage;
						}
						return magnitude;
					}
					break;
				}
				default:
					if (length != 19)
					{
						return magnitude;
					}
					if (!(weaponType == "OneHandedBastardAxe"))
					{
						return magnitude;
					}
					skillBasedDamage = MBMath.ClampFloat(magnitude + effectiveSkill * 0.13f, 12f * (1f + skillModifier), 20f * (1f + 2f * skillModifier)) * 4.6f;
					if (damageType == DamageTypes.Blunt)
					{
						skillBasedDamage = MBMath.ClampFloat(magnitude + effectiveSkill * 0.09375f, 20f * (1f + skillModifier), 26f * (1f + 2f * skillModifier)) * 4f * 0.3f;
					}
					if (magnitude > 1f)
					{
						magnitude = skillBasedDamage;
					}
					return magnitude;
				}
				if (damageType == DamageTypes.Cut)
				{
					float value5 = magnitude + effectiveSkill * 0.133f;
					float min5 = 5f * (1f + skillModifier);
					float max5 = 15f * (1f + 2f * skillModifier);
					skillBasedDamage = MBMath.ClampFloat(value5, min5, max5) * 4.6f;
				}
				else if (damageType == DamageTypes.Blunt)
				{
					skillBasedDamage = MBMath.ClampFloat(magnitude + effectiveSkill * 0.075f, 15f * (1f + skillModifier), 20f * (1f + 2f * skillModifier)) * 4f * 0.4f;
				}
				else if (strikeType == StrikeType.Swing)
				{
					skillBasedDamage = MBMath.ClampFloat(magnitude + effectiveSkill * 0.133f, 5f * (1f + skillModifier), 15f * (1f + 2f * skillModifier)) * 4f * 0.05f;
				}
				else
				{
					skillBasedDamage = magnitude;
				}
				if (magnitude > 1f)
				{
					magnitude = skillBasedDamage;
				}
				return magnitude;
				IL_942:
				float value6 = magnitude + effectiveSkill * 0.1f;
				float min6 = 10f * (1f + skillModifier);
				float max6 = 18f * (1f + 2f * skillModifier);
				skillBasedDamage = MBMath.ClampFloat(value6, min6, max6) * 4.6f;
				if (damageType == DamageTypes.Blunt)
				{
					skillBasedDamage = MBMath.ClampFloat(magnitude + effectiveSkill * 0.075f, 15f * (1f + skillModifier), 20f * (1f + 2f * skillModifier)) * 4f * 0.3f;
				}
				if (magnitude > 1f)
				{
					magnitude = skillBasedDamage;
				}
			}
			return magnitude;
		}


		public static RBMCombatConfigWeaponType getWeaponTypeFactors(string weaponType)
		{
			
			foreach (RBMCombatConfigWeaponType weaponTypeFactors in weaponTypesFactors)
			{
				bool flag = weaponTypeFactors.weaponType.Equals(weaponType);
				if (flag)
				{
					return weaponTypeFactors;
				}
			}
			return null;
		}

		// Token: 0x0600006C RID: 108
		public static float RBMComputeDamage(string weaponType, DamageTypes damageType, float magnitude, float armorEffectiveness, float absorbedDamageRatio, out float penetratedDamage, out float bluntTraumaAfterArmor, float weaponDamageFactor = 1f, BasicCharacterObject player = null, bool isPlayerVictim = false)
		{
			float armorReduction = 100f / (100f + armorEffectiveness * 2f);
			float mag_1h_thrust;
			float mag_2h_thrust;
			float mag_1h_sword_thrust;
			float mag_2h_sword_thrust;
			if (damageType == DamageTypes.Pierce)
			{
				mag_1h_thrust = magnitude * 20f;
				mag_2h_thrust = magnitude * 1f * 20f;
				mag_1h_sword_thrust = magnitude * 1f * 20f;
				mag_2h_sword_thrust = magnitude * 1f * 20f;
			}
			else if (damageType == DamageTypes.Cut)
			{
				mag_1h_thrust = magnitude;
				mag_2h_thrust = magnitude;
				mag_1h_sword_thrust = magnitude * 1f;
				mag_2h_sword_thrust = magnitude * 1f;
			}
			else
			{
				mag_1h_thrust = magnitude;
				mag_2h_thrust = magnitude;
				mag_1h_sword_thrust = magnitude;
				mag_2h_sword_thrust = magnitude;
			}
			float damage;
			if (weaponType != null)
			{
				switch (weaponType.Length)
				{
				case 4:
				{
					char c = weaponType[0];
					if (c != 'B')
					{
						if (c == 'M' && weaponType == "Mace")
						{
							damage = Utilities.WeaponTypeDamage(getWeaponTypeFactors(weaponType), mag_1h_thrust, armorReduction, damageType, armorEffectiveness, player, isPlayerVictim, weaponDamageFactor, out penetratedDamage, out bluntTraumaAfterArmor, 0f);
							goto IL_49B;
						}
					}
					else if (weaponType == "Bolt")
					{
						damage = Utilities.WeaponTypeDamage(getWeaponTypeFactors(weaponType), magnitude, armorReduction, damageType, armorEffectiveness, player, isPlayerVictim, weaponDamageFactor, out penetratedDamage, out bluntTraumaAfterArmor, 0f);
						goto IL_49B;
					}
					break;
				}
				case 5:
					if (weaponType == "Arrow")
					{
						damage = Utilities.WeaponTypeDamage(getWeaponTypeFactors(weaponType), magnitude, armorReduction, damageType, armorEffectiveness, player, isPlayerVictim, weaponDamageFactor, out penetratedDamage, out bluntTraumaAfterArmor, 0f);
						goto IL_49B;
					}
					break;
				case 6:
					if (weaponType == "Dagger")
					{
						damage = Utilities.WeaponTypeDamage(getWeaponTypeFactors(weaponType), mag_1h_sword_thrust, armorReduction, damageType, armorEffectiveness, player, isPlayerVictim, weaponDamageFactor, out penetratedDamage, out bluntTraumaAfterArmor, 2f);
						goto IL_49B;
					}
					break;
				case 7:
					if (weaponType == "Javelin")
					{
						damage = Utilities.WeaponTypeDamage(getWeaponTypeFactors(weaponType), mag_1h_thrust, armorReduction, damageType, armorEffectiveness, player, isPlayerVictim, weaponDamageFactor, out penetratedDamage, out bluntTraumaAfterArmor, 2f);
						goto IL_49B;
					}
					break;
				case 11:
					if (weaponType == "ThrowingAxe")
					{
						damage = Utilities.WeaponTypeDamage(getWeaponTypeFactors(weaponType), mag_1h_thrust, armorReduction, damageType, armorEffectiveness, player, isPlayerVictim, weaponDamageFactor, out penetratedDamage, out bluntTraumaAfterArmor, 2f);
						goto IL_49B;
					}
					break;
				case 12:
				{
					char c2 = weaponType[0];
					if (c2 != 'O')
					{
						if (c2 == 'T' && weaponType == "TwoHandedAxe")
						{
							damage = Utilities.WeaponTypeDamage(getWeaponTypeFactors(weaponType), magnitude, armorReduction, damageType, armorEffectiveness, player, isPlayerVictim, weaponDamageFactor, out penetratedDamage, out bluntTraumaAfterArmor, 2f);
							goto IL_49B;
						}
					}
					else if (weaponType == "OneHandedAxe")
					{
						damage = Utilities.WeaponTypeDamage(getWeaponTypeFactors(weaponType), magnitude, armorReduction, damageType, armorEffectiveness, player, isPlayerVictim, weaponDamageFactor, out penetratedDamage, out bluntTraumaAfterArmor, 2f);
						goto IL_49B;
					}
					break;
				}
				case 13:
				{
					char c3 = weaponType[1];
					if (c3 != 'h')
					{
						if (c3 == 'w' && weaponType == "TwoHandedMace")
						{
							damage = Utilities.WeaponTypeDamage(getWeaponTypeFactors(weaponType), mag_2h_thrust, armorReduction, damageType, armorEffectiveness, player, isPlayerVictim, weaponDamageFactor, out penetratedDamage, out bluntTraumaAfterArmor, 2f);
							goto IL_49B;
						}
					}
					else if (weaponType == "ThrowingKnife")
					{
						damage = Utilities.WeaponTypeDamage(getWeaponTypeFactors(weaponType), mag_1h_sword_thrust, armorReduction, damageType, armorEffectiveness, player, isPlayerVictim, weaponDamageFactor, out penetratedDamage, out bluntTraumaAfterArmor, 2f);
						goto IL_49B;
					}
					break;
				}
				case 14:
				{
					char c4 = weaponType[0];
					if (c4 != 'O')
					{
						if (c4 == 'T' && weaponType == "TwoHandedSword")
						{
							damage = Utilities.WeaponTypeDamage(getWeaponTypeFactors(weaponType), mag_2h_sword_thrust, armorReduction, damageType, armorEffectiveness, player, isPlayerVictim, weaponDamageFactor, out penetratedDamage, out bluntTraumaAfterArmor, 2f);
							goto IL_49B;
						}
					}
					else if (weaponType == "OneHandedSword")
					{
						damage = Utilities.WeaponTypeDamage(getWeaponTypeFactors(weaponType), mag_1h_sword_thrust, armorReduction, damageType, armorEffectiveness, player, isPlayerVictim, weaponDamageFactor, out penetratedDamage, out bluntTraumaAfterArmor, 2f);
						goto IL_49B;
					}
					break;
				}
				case 16:
				{
					char c5 = weaponType[0];
					if (c5 != 'O')
					{
						if (c5 == 'T' && weaponType == "TwoHandedPolearm")
						{
							damage = Utilities.WeaponTypeDamage(getWeaponTypeFactors(weaponType), mag_2h_thrust, armorReduction, damageType, armorEffectiveness, player, isPlayerVictim, weaponDamageFactor, out penetratedDamage, out bluntTraumaAfterArmor, 2f);
							goto IL_49B;
						}
					}
					else if (weaponType == "OneHandedPolearm")
					{
						damage = Utilities.WeaponTypeDamage(getWeaponTypeFactors(weaponType), mag_1h_thrust, armorReduction, damageType, armorEffectiveness, player, isPlayerVictim, weaponDamageFactor, out penetratedDamage, out bluntTraumaAfterArmor, 2f);
						goto IL_49B;
					}
					break;
				}
				case 19:
					if (weaponType == "OneHandedBastardAxe")
					{
						damage = Utilities.WeaponTypeDamage(getWeaponTypeFactors(weaponType), magnitude, armorReduction, damageType, armorEffectiveness, player, isPlayerVictim, weaponDamageFactor, out penetratedDamage, out bluntTraumaAfterArmor, 2f);
						goto IL_49B;
					}
					break;
				}
			}
			damage = Utilities.WeaponTypeDamage(new RBMCombatConfigWeaponType("default", 1f, 1f, 1f, 1f, 1f, 1f), magnitude, armorReduction, damageType, armorEffectiveness, player, isPlayerVictim, weaponDamageFactor, out penetratedDamage, out bluntTraumaAfterArmor, 2f);
			IL_49B:
			return damage * absorbedDamageRatio;
		}

		// Token: 0x0600006D RID: 109
		private static float WeaponTypeDamage(RBMCombatConfigWeaponType weaponTypeFactors, float magnitude, float armorReduction, DamageTypes damageType, float armorEffectiveness, BasicCharacterObject player, bool isPlayerVictim, float weaponDamageFactor, out float penetratedDamage, out float bluntTraumaAfterArmor, float partialPenetrationThreshold = 2f)
		{
			float damage = 0f;
			float armorThresholdModifier = 1f / weaponDamageFactor;
			switch (damageType)
			{
			case DamageTypes.Cut:
			{
				penetratedDamage = Math.Max(0f, magnitude - armorEffectiveness * weaponTypeFactors.ExtraArmorThresholdFactorCut * armorThresholdModifier);
				float bluntFraction = 0f;
				if (magnitude > 0f)
				{
					bluntFraction = (magnitude - penetratedDamage) / magnitude;
				}
				damage += penetratedDamage;
				float bluntTrauma = magnitude * (weaponTypeFactors.ExtraBluntFactorCut + 1f) * bluntFraction;
				bluntTraumaAfterArmor = Math.Max(0f, bluntTrauma * armorReduction);
				damage += bluntTraumaAfterArmor;
				if (false && player != null)
				{
					if (isPlayerVictim)
					{
						InformationManager.DisplayMessage(new InformationMessage(string.Concat(new string[]
						{
							"You received ",
							((int)bluntTraumaAfterArmor).ToString(),
							" blunt trauma, ",
							((int)penetratedDamage).ToString(),
							" armor penetration damage"
						})));
					}
					else
					{
						InformationManager.DisplayMessage(new InformationMessage(string.Concat(new string[]
						{
							"You dealt ",
							((int)bluntTraumaAfterArmor).ToString(),
							" blunt trauma, ",
							((int)penetratedDamage).ToString(),
							" armor penetration damage"
						})));
					}
				}
				break;
			}
			case DamageTypes.Pierce:
			{
				float partialPenetration = Math.Max(0f, magnitude - armorEffectiveness * partialPenetrationThreshold * armorThresholdModifier);
				if (partialPenetration > 15f)
				{
					partialPenetration = 15f;
				}
				penetratedDamage = Math.Max(0f, magnitude - armorEffectiveness * weaponTypeFactors.ExtraArmorThresholdFactorPierce * armorThresholdModifier) - partialPenetration;
				float bluntFraction2 = 0f;
				if (magnitude > 0f)
				{
					bluntFraction2 = (magnitude - (penetratedDamage + partialPenetration)) / magnitude;
				}
				penetratedDamage += partialPenetration;
				damage += penetratedDamage;
				float bluntTrauma2 = magnitude * (weaponTypeFactors.ExtraBluntFactorPierce + 1f) * bluntFraction2;
				bluntTraumaAfterArmor = Math.Max(0f, bluntTrauma2 * armorReduction);
				damage += bluntTraumaAfterArmor;
				if (false && player != null)
				{
					if (isPlayerVictim)
					{
						InformationManager.DisplayMessage(new InformationMessage(string.Concat(new string[]
						{
							"You received ",
							((int)bluntTraumaAfterArmor).ToString(),
							" blunt trauma, ",
							((int)penetratedDamage).ToString(),
							" armor penetration damage"
						})));
					}
					else
					{
						InformationManager.DisplayMessage(new InformationMessage(string.Concat(new string[]
						{
							"You dealt ",
							((int)bluntTraumaAfterArmor).ToString(),
							" blunt trauma, ",
							((int)penetratedDamage).ToString(),
							" armor penetration damage"
						})));
					}
				}
				break;
			}
			case DamageTypes.Blunt:
			{
				penetratedDamage = Math.Max(0f, magnitude - armorEffectiveness * 5f * armorThresholdModifier);
				float bluntFraction3 = 0f;
				if (magnitude > 0f)
				{
					bluntFraction3 = (magnitude - penetratedDamage) / magnitude;
				}
				damage += penetratedDamage;
				float bluntTrauma3 = magnitude * (0.7f * 1f) * bluntFraction3;
				bluntTraumaAfterArmor = Math.Max(0f, bluntTrauma3 * armorReduction);
				damage += bluntTraumaAfterArmor;
				break;
			}
			default:
				penetratedDamage = 0f;
				bluntTraumaAfterArmor = 0f;
				damage = 0f;
				break;
			}
			return damage;
		}

		// Token: 0x0600006E RID: 110
		public static float CalculateThrustMagnitudeForOneHandedWeapon(float weaponWeight, float effectiveSkill, float thrustSpeed, float exraLinearSpeed, Agent.UsageDirection attackDirection)
		{
			bool flag = attackDirection == Agent.UsageDirection.AttackUp;
			thrustSpeed = (flag ? (thrustSpeed * 1.33f) : thrustSpeed);
			if (thrustSpeed > 9f)
			{
				thrustSpeed = 9f;
			}
			float combinedSpeed = thrustSpeed + exraLinearSpeed;
			float skillModifier = Utilities.CalculateSkillModifier(effectiveSkill) * 2f;
			float spearKineticEnergy = 0.5f * weaponWeight * (combinedSpeed * combinedSpeed);
			float armStrength = flag ? 1.5f : 2.5f;
			float thrustStrength = weaponWeight + armStrength * (1f + skillModifier);
			float thrustStrengthWithWeaponWeight = weaponWeight + armStrength * (1f + skillModifier);
			float thrustEnergyCap = MathF.Clamp(0.5f * thrustStrength * (thrustSpeed * thrustSpeed) * 1.5f, 0f, 180f);
			float thrustEnergy = 0.5f * thrustStrengthWithWeaponWeight * (combinedSpeed * combinedSpeed);
			if (thrustEnergy > thrustEnergyCap)
			{
				thrustEnergy = thrustEnergyCap;
			}
			float magnitude = thrustEnergy;
			if (spearKineticEnergy > magnitude)
			{
				magnitude = spearKineticEnergy;
			}
			if (magnitude > thrustEnergyCap)
			{
				magnitude = thrustEnergyCap;
			}
			return magnitude * 0.05f;
		}

		// Token: 0x0600006F RID: 111
		public static float CalculateThrustMagnitudeForTwoHandedWeapon(float weaponWeight, float effectiveSkill, float thrustSpeed, float exraLinearSpeed, Agent.UsageDirection attackDirection)
		{
			bool flag = attackDirection == Agent.UsageDirection.AttackUp;
			thrustSpeed = (flag ? (thrustSpeed + 1f) : thrustSpeed);
			if (thrustSpeed > 6f)
			{
				thrustSpeed = 6f;
			}
			float combinedSpeed = thrustSpeed + exraLinearSpeed;
			float skillModifier = Utilities.CalculateSkillModifier(effectiveSkill) * 2f;
			float spearKineticEnergy = 0.5f * weaponWeight * (combinedSpeed * combinedSpeed);
			float armStrength = flag ? 4f : 5f;
			float thrustStrength = armStrength * (1f + skillModifier);
			float thrustStrengthWithWeaponWeight = weaponWeight + armStrength * (1f + skillModifier);
			float thrustEnergyCap = MathF.Clamp(0.5f * thrustStrength * (thrustSpeed * thrustSpeed) * 1.5f, 0f, 250f);
			float thrustEnergy = 0.5f * thrustStrengthWithWeaponWeight * (combinedSpeed * combinedSpeed);
			if (thrustEnergy > thrustEnergyCap)
			{
				thrustEnergy = thrustEnergyCap;
			}
			float magnitude = thrustEnergy;
			if (spearKineticEnergy > magnitude)
			{
				magnitude = spearKineticEnergy;
			}
			if (magnitude > thrustEnergyCap)
			{
				magnitude = thrustEnergyCap;
			}
			return magnitude * 0.05f;
		}

		// Token: 0x06000070 RID: 112
		public static void CalculateVisualSpeeds(MissionWeapon weapon, WeaponData weaponData, WeaponClass weaponClass, float effectiveSkillDR, out int swingSpeedReal, out int thrustSpeedReal, out int handlingReal)
		{
			swingSpeedReal = -1;
			thrustSpeedReal = -1;
			handlingReal = -1;
			if (!weapon.IsEmpty && weapon.Item != null)
			{
				int swingSpeed = weapon.GetModifiedSwingSpeedForCurrentUsage();
				int handling = weapon.GetModifiedHandlingForCurrentUsage();
				switch (weaponClass)
				{
				case WeaponClass.Dagger:
				case WeaponClass.OneHandedSword:
				case WeaponClass.TwoHandedSword:
				{
					float swingskillModifier = 1f + effectiveSkillDR / 800f;
					float thrustskillModifier = 1f + effectiveSkillDR / 800f;
					float handlingskillModifier = 1f + effectiveSkillDR / 800f;
					swingSpeedReal = MathF.Ceiling((float)swingSpeed * 0.9f * swingskillModifier);
					thrustSpeedReal = MathF.Floor(Utilities.CalculateThrustSpeed(weapon.Item.Weight, weaponData.Inertia, weaponData.CenterOfMass) * Utilities.thrustSpeedTransfer);
					thrustSpeedReal = MathF.Ceiling((float)thrustSpeedReal * 1.15f * thrustskillModifier);
					handlingReal = MathF.Ceiling((float)handling * 0.9f * handlingskillModifier);
					return;
				}
				case WeaponClass.OneHandedAxe:
				case WeaponClass.Mace:
				case WeaponClass.TwoHandedMace:
				case WeaponClass.OneHandedPolearm:
				case WeaponClass.LowGripPolearm:
				{
					float swingskillModifier2 = 1f + effectiveSkillDR / 1000f;
					float thrustskillModifier2 = 1f + effectiveSkillDR / 1000f;
					float handlingskillModifier2 = 1f + effectiveSkillDR / 700f;
					swingSpeedReal = MathF.Ceiling((float)swingSpeed * 0.83f * swingskillModifier2);
					thrustSpeedReal = MathF.Floor(Utilities.CalculateThrustSpeed(weapon.Item.Weight, weaponData.Inertia, weaponData.CenterOfMass) * Utilities.thrustSpeedTransfer);
					thrustSpeedReal = MathF.Ceiling((float)thrustSpeedReal * 1.1f * thrustskillModifier2);
					handlingReal = MathF.Ceiling((float)handling * 0.83f * handlingskillModifier2);
					return;
				}
				case WeaponClass.TwoHandedAxe:
				{
					float swingskillModifier3 = 1f + effectiveSkillDR / 800f;
					float thrustskillModifier3 = 1f + effectiveSkillDR / 1000f;
					float handlingskillModifier3 = 1f + effectiveSkillDR / 700f;
					swingSpeedReal = MathF.Ceiling((float)swingSpeed * 0.75f * swingskillModifier3);
					thrustSpeedReal = MathF.Floor(Utilities.CalculateThrustSpeed(weapon.Item.Weight, weaponData.Inertia, weaponData.CenterOfMass) * Utilities.thrustSpeedTransfer);
					thrustSpeedReal = MathF.Ceiling((float)thrustSpeedReal * 0.9f * thrustskillModifier3);
					handlingReal = MathF.Ceiling((float)handling * 0.83f * handlingskillModifier3);
					return;
				}
				case WeaponClass.Pick:
					break;
				case WeaponClass.TwoHandedPolearm:
				{
					float swingskillModifier4 = 1f + effectiveSkillDR / 1000f;
					float thrustskillModifier4 = 1f + effectiveSkillDR / 1000f;
					float handlingskillModifier4 = 1f + effectiveSkillDR / 700f;
					swingSpeedReal = MathF.Ceiling((float)swingSpeed * 0.83f * swingskillModifier4);
					thrustSpeedReal = MathF.Floor(Utilities.CalculateThrustSpeed(weapon.Item.Weight, weaponData.Inertia, weaponData.CenterOfMass) * Utilities.thrustSpeedTransfer);
					thrustSpeedReal = MathF.Ceiling((float)thrustSpeedReal * 1.05f * thrustskillModifier4);
					handlingReal = MathF.Ceiling((float)handling * 5f * handlingskillModifier4);
					break;
				}
				default:
					return;
				}
			}
		}

		// Token: 0x06000071 RID: 113
		public static void CalculateVisualSpeeds(EquipmentElement weapon, int weaponUsageIndex, float effectiveSkillDR, out int swingSpeedReal, out int thrustSpeedReal, out int handlingReal)
		{
			swingSpeedReal = -1;
			thrustSpeedReal = -1;
			handlingReal = -1;
			if (!weapon.IsEmpty && weapon.Item != null && weapon.Item.GetWeaponWithUsageIndex(weaponUsageIndex) != null)
			{
				int swingSpeed = weapon.GetModifiedSwingSpeedForUsage(weaponUsageIndex);
				int handling = weapon.GetModifiedHandlingForUsage(weaponUsageIndex);
				switch (weapon.Item.GetWeaponWithUsageIndex(weaponUsageIndex).WeaponClass)
				{
				case WeaponClass.Dagger:
				case WeaponClass.OneHandedSword:
				case WeaponClass.TwoHandedSword:
				{
					float swingskillModifier = 1f + effectiveSkillDR / 800f;
					float thrustskillModifier = 1f + effectiveSkillDR / 800f;
					float handlingskillModifier = 1f + effectiveSkillDR / 800f;
					swingSpeedReal = MathF.Ceiling((float)swingSpeed * 0.83f * swingskillModifier);
					thrustSpeedReal = MathF.Floor(Utilities.CalculateThrustSpeed(weapon.Weight, weapon.Item.GetWeaponWithUsageIndex(weaponUsageIndex).Inertia, weapon.Item.GetWeaponWithUsageIndex(weaponUsageIndex).CenterOfMass) * Utilities.thrustSpeedTransfer);
					thrustSpeedReal = MathF.Ceiling((float)thrustSpeedReal * 1.15f * thrustskillModifier);
					handlingReal = MathF.Ceiling((float)handling * 0.9f * handlingskillModifier);
					return;
				}
				case WeaponClass.OneHandedAxe:
				case WeaponClass.Mace:
				case WeaponClass.TwoHandedMace:
				case WeaponClass.OneHandedPolearm:
				case WeaponClass.LowGripPolearm:
				{
					float swingskillModifier2 = 1f + effectiveSkillDR / 1000f;
					float thrustskillModifier2 = 1f + effectiveSkillDR / 1000f;
					float handlingskillModifier2 = 1f + effectiveSkillDR / 700f;
					swingSpeedReal = MathF.Ceiling((float)swingSpeed * 0.83f * swingskillModifier2);
					thrustSpeedReal = MathF.Floor(Utilities.CalculateThrustSpeed(weapon.Weight, weapon.Item.GetWeaponWithUsageIndex(weaponUsageIndex).Inertia, weapon.Item.GetWeaponWithUsageIndex(weaponUsageIndex).CenterOfMass) * Utilities.thrustSpeedTransfer);
					thrustSpeedReal = MathF.Ceiling((float)thrustSpeedReal * 1.1f * thrustskillModifier2);
					handlingReal = MathF.Ceiling((float)handling * 0.83f * handlingskillModifier2);
					return;
				}
				case WeaponClass.TwoHandedAxe:
				{
					float swingskillModifier3 = 1f + effectiveSkillDR / 800f;
					float thrustskillModifier3 = 1f + effectiveSkillDR / 1000f;
					float handlingskillModifier3 = 1f + effectiveSkillDR / 700f;
					swingSpeedReal = MathF.Ceiling((float)swingSpeed * 0.75f * swingskillModifier3);
					thrustSpeedReal = MathF.Ceiling((float)weapon.GetModifiedThrustSpeedForUsage(weaponUsageIndex) * 0.9f * thrustskillModifier3);
					handlingReal = MathF.Ceiling((float)handling * 0.83f * handlingskillModifier3);
					return;
				}
				case WeaponClass.Pick:
					break;
				case WeaponClass.TwoHandedPolearm:
				{
					float swingskillModifier4 = 1f + effectiveSkillDR / 1000f;
					float thrustskillModifier4 = 1f + effectiveSkillDR / 1000f;
					float handlingskillModifier4 = 1f + effectiveSkillDR / 700f;
					swingSpeedReal = MathF.Ceiling((float)swingSpeed * 0.83f * swingskillModifier4);
					thrustSpeedReal = MathF.Floor(Utilities.CalculateThrustSpeed(weapon.Weight, weapon.Item.GetWeaponWithUsageIndex(weaponUsageIndex).Inertia, weapon.Item.GetWeaponWithUsageIndex(weaponUsageIndex).CenterOfMass) * Utilities.thrustSpeedTransfer);
					thrustSpeedReal = MathF.Ceiling((float)thrustSpeedReal * 1.05f * thrustskillModifier4);
					handlingReal = MathF.Ceiling((float)handling * 5f * handlingskillModifier4);
					break;
				}
				default:
					return;
				}
			}
		}

		// Token: 0x06000072 RID: 114
		public static float getSwingDamageFactor(WeaponComponentData wcd, ItemModifier itemModifier)
		{
			float result;
			if (itemModifier == null)
			{
				result = wcd.SwingDamageFactor;
			}
			else
			{
				float factorBonus = (float)(itemModifier.ModifyDamage(100) - 100) / 100f;
				result = wcd.SwingDamageFactor + factorBonus;
			}
			return result;
		}

		// Token: 0x06000073 RID: 115
		public static float getThrustDamageFactor(WeaponComponentData wcd, ItemModifier itemModifier)
		{
			float result;
			if (itemModifier == null)
			{
				result = wcd.ThrustDamageFactor;
			}
			else
			{
				float factorBonus = (float)(itemModifier.ModifyDamage(100) - 100) / 100f;
				result = wcd.ThrustDamageFactor + factorBonus;
			}
			return result;
		}

		// Token: 0x06000074 RID: 116
		public static string GetConfigFilePath()
		{
			string filepath = @"C:\Users\Owl\Documents\Mount and Blade II Bannerlord\Configs\RBM\";
			return System.IO.Path.Combine(filepath, "config2.xml");
		}
			
		

		// Token: 0x06000075 RID: 117
		public static string GetConfigFolderPath()
		{
			return System.IO.Path(@"C:\Users\Owl\Documents\Mount and Blade II Bannerlord\Configs\RBM");
		}

		public static void parseXmlConfig()
		{
			bool flag = xmlConfig.SelectSingleNode("/Config/DeveloperMode") != null;
			if (flag)
			{
				developerMode = true;
			}
			rbmTournamentEnabled = xmlConfig.SelectSingleNode("/Config/RBMTournament/Enabled").InnerText.Equals("1");
			rbmAiEnabled = xmlConfig.SelectSingleNode("/Config/RBMAI/Enabled").InnerText.Equals("1");
			rbmCombatEnabled = xmlConfig.SelectSingleNode("/Config/RBMCombat/Enabled").InnerText.Equals("1");
			postureEnabled = xmlConfig.SelectSingleNode("/Config/RBMAI/PostureEnabled").InnerText.Equals("1");
			postureGUIEnabled = xmlConfig.SelectSingleNode("/Config/RBMAI/PostureGUIEnabled").InnerText.Equals("1");
			vanillaCombatAi = xmlConfig.SelectSingleNode("/Config/RBMAI/VanillaCombatAi").InnerText.Equals("1");
			bool flag2 = xmlConfig.SelectSingleNode("/Config/RBMAI/PlayerPostureMultiplier") != null;
			if (flag2)
			{
				string innerText = xmlConfig.SelectSingleNode("/Config/RBMAI/PlayerPostureMultiplier").InnerText;
				string a = innerText;
				if (!(a == "0"))
				{
					if (!(a == "1"))
					{
						if (a == "2")
						{
							playerPostureMultiplier = 2f;
						}
					}
					else
					{
						playerPostureMultiplier = 1.5f;
					}
				}
				else
				{
					playerPostureMultiplier = 1f;
				}
			}
			bool flag3 = xmlConfig.SelectSingleNode("/Config/RBMCombat/Global/ArmorStatusUIEnabled") != null;
			if (flag3)
			{
				armorStatusUIEnabled = xmlConfig.SelectSingleNode("/Config/RBMCombat/Global/ArmorStatusUIEnabled").InnerText.Equals("1");
			}
			else
			{
				XmlNode ArmorStatusUIEnabled = xmlConfig.CreateNode(XmlNodeType.Element, "ArmorStatusUIEnabled", null);
				ArmorStatusUIEnabled.InnerText = "1";
				xmlConfig.SelectSingleNode("/Config/RBMCombat/Global").AppendChild(ArmorStatusUIEnabled);
			}
			bool flag4 = xmlConfig.SelectSingleNode("/Config/RBMCombat/Global/RealisticArrowArc") != null;
			if (flag4)
			{
				realisticArrowArc = xmlConfig.SelectSingleNode("/Config/RBMCombat/Global/RealisticArrowArc").InnerText.Equals("1");
			}
			else
			{
				XmlNode RealisticArrowArc = xmlConfig.CreateNode(XmlNodeType.Element, "RealisticArrowArc", null);
				RealisticArrowArc.InnerText = "0";
				xmlConfig.SelectSingleNode("/Config/RBMCombat/Global").AppendChild(RealisticArrowArc);
			}
			armorMultiplier = float.Parse(xmlConfig.SelectSingleNode("/Config/RBMCombat/Global/ArmorMultiplier").InnerText);
			armorPenetrationMessage = xmlConfig.SelectSingleNode("/Config/RBMCombat/Global/ArmorPenetrationMessage").InnerText.Equals("1");
			betterArrowVisuals = xmlConfig.SelectSingleNode("/Config/RBMCombat/Global/BetterArrowVisuals").InnerText.Equals("1");
			passiveShoulderShields = xmlConfig.SelectSingleNode("/Config/RBMCombat/Global/PassiveShoulderShields").InnerText.Equals("1");
			troopOverhaulActive = xmlConfig.SelectSingleNode("/Config/RBMCombat/Global/TroopOverhaulActive").InnerText.Equals("1");
			realisticRangedReload = xmlConfig.SelectSingleNode("/Config/RBMCombat/Global/RealisticRangedReload").InnerText;
			maceBluntModifier = float.Parse(xmlConfig.SelectSingleNode("/Config/RBMCombat/Global/MaceBluntModifier").InnerText);
			armorThresholdModifier = float.Parse(xmlConfig.SelectSingleNode("/Config/RBMCombat/Global/ArmorThresholdModifier").InnerText);
			bluntTraumaBonus = float.Parse(xmlConfig.SelectSingleNode("/Config/RBMCombat/Global/BluntTraumaBonus").InnerText);
			foreach (object obj in xmlConfig.SelectSingleNode("/Config/RBMCombat/WeaponTypes").ChildNodes)
			{
				XmlNode weaponTypeNode = (XmlNode)obj;
				RBMCombatConfigWeaponType weaponTypeFactors = new RBMCombatConfigWeaponType();
				weaponTypeFactors.weaponType = weaponTypeNode.Name;
				weaponTypeFactors.ExtraBluntFactorCut = float.Parse(xmlConfig.SelectSingleNode("/Config/RBMCombat/WeaponTypes/" + weaponTypeNode.Name + "/ExtraBluntFactorCut").InnerText);
				weaponTypeFactors.ExtraBluntFactorPierce = float.Parse(xmlConfig.SelectSingleNode("/Config/RBMCombat/WeaponTypes/" + weaponTypeNode.Name + "/ExtraBluntFactorPierce").InnerText);
				weaponTypeFactors.ExtraBluntFactorBlunt = float.Parse(xmlConfig.SelectSingleNode("/Config/RBMCombat/WeaponTypes/" + weaponTypeNode.Name + "/ExtraBluntFactorBlunt").InnerText);
				weaponTypeFactors.ExtraArmorThresholdFactorPierce = float.Parse(xmlConfig.SelectSingleNode("/Config/RBMCombat/WeaponTypes/" + weaponTypeNode.Name + "/ExtraArmorThresholdFactorPierce").InnerText);
				weaponTypeFactors.ExtraArmorThresholdFactorCut = float.Parse(xmlConfig.SelectSingleNode("/Config/RBMCombat/WeaponTypes/" + weaponTypeNode.Name + "/ExtraArmorThresholdFactorCut").InnerText);
				weaponTypeFactors.ExtraArmorSkillDamageAbsorb = float.Parse(xmlConfig.SelectSingleNode("/Config/RBMCombat/WeaponTypes/" + weaponTypeNode.Name + "/ExtraArmorSkillDamageAbsorb").InnerText);
				weaponTypesFactors.Add(weaponTypeFactors);
			}
			priceMultipliers.ArmorPriceModifier = float.Parse(xmlConfig.SelectSingleNode("/Config/RBMCombat/PriceModifiers/ArmorPriceModifier").InnerText);
			priceMultipliers.WeaponPriceModifier = float.Parse(xmlConfig.SelectSingleNode("/Config/RBMCombat/PriceModifiers/WeaponPriceModifier").InnerText);
			priceMultipliers.HorsePriceModifier = float.Parse(xmlConfig.SelectSingleNode("/Config/RBMCombat/PriceModifiers/HorsePriceModifier").InnerText);
			priceMultipliers.TradePriceModifier = float.Parse(xmlConfig.SelectSingleNode("/Config/RBMCombat/PriceModifiers/TradePriceModifier").InnerText);
			saveXmlConfig();
		}

		public static void LoadConfig()
		{
			string defaultConfigFilePath = System.IO.Path(@"F:\_Home\Nick\Entertainment\Mount & Blade 2 - Bannerlord\Modules\RBM\DefaultConfigDONOTEDIT.xml");
			string configFolderPath = GetConfigFolderPath();
			string configFilePath = GetConfigFilePath();
			bool flag = !Directory.Exists(configFolderPath);
			if (flag)
			{
				Directory.CreateDirectory(configFolderPath);
			}
			bool flag2 = File.Exists(configFilePath);
			if (flag2)
			{
				xmlConfig.Load(configFilePath);
			}
			else
			{
				File.Copy(defaultConfigFilePath, configFilePath);
				xmlConfig.Load(configFilePath);
			}
			parseXmlConfig();
		}


		// Token: 0x06000076 RID: 118
		static Utilities()
		{
		}

		// Token: 0x0400002A RID: 42
		public static int numOfHits = 0;

		// Token: 0x0400002B RID: 43
		public static int numOfDurabilityDowngrade = 0;

		// Token: 0x0400002C RID: 44
		public static float throwableCorrectionSpeed = 7f;

		// Token: 0x0400002D RID: 45
		public static float swingSpeedTransfer = 4.5454545f;

		// Token: 0x0400002E RID: 46
		public static float thrustSpeedTransfer = 11.764706f;

		// Token: 0x0400002F RID: 47
		public const float oneHandedPolearmThrustStrength = 2.5f;

		// Token: 0x04000030 RID: 48
		public const float twoHandedPolearmThrustStrength = 5f;

		public static XmlDocument xmlConfig = new XmlDocument();
		public static float ThrustMagnitudeModifier = 0.05f;
		public static float OneHandedThrustDamageBonus = 20f;
		public static float TwoHandedThrustDamageBonus = 20f;
		public static bool rbmTournamentEnabled = true;
		public static bool rbmAiEnabled = true;
		public static bool rbmCombatEnabled = true;
		public static bool developerMode = false;
		public static bool postureEnabled = true;
		public static float playerPostureMultiplier = 1f;
		public static bool postureGUIEnabled = true;
		public static bool vanillaCombatAi = false;
		public static bool realisticArrowArc = false;
		public static bool armorStatusUIEnabled = true;
		public static float armorMultiplier = 2f;
		public static bool armorPenetrationMessage = false;
		public static bool betterArrowVisuals = true;
		public static bool passiveShoulderShields = false;
		public static bool troopOverhaulActive = true;
		public static string realisticRangedReload = "1";
		public static float maceBluntModifier = 1f;
		public static float armorThresholdModifier = 1f;
		public static float bluntTraumaBonus = 1f;
		public static RBMCombatConfigPriceMultipliers priceMultipliers = new RBMCombatConfigPriceMultipliers();
		public static List<RBMCombatConfigWeaponType> weaponTypesFactors = new List<RBMCombatConfigWeaponType>();
	}
}
