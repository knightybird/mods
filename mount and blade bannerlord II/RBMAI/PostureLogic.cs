using System;
using System.Collections.Generic;
using System.Diagnostics;
using HarmonyLib;
using JetBrains.Annotations;
using TaleWorlds.CampaignSystem.TournamentGames;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace RBMAI
{
	// Token: 0x02000021 RID: 33
	public class PostureLogic : MissionLogic
	{
		// Token: 0x0600007C RID: 124
		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
			if (Mission.Current.AllowAiTicking)
			{
				if (PostureLogic.currentDt < PostureLogic.timeToCalc)
				{
					PostureLogic.currentDt += dt;
					return;
				}
				MBArrayList<Agent> mbarrayList = new MBArrayList<Agent>();
				foreach (KeyValuePair<Agent, Posture> keyValuePair in AgentPostures.values)
				{
					if (keyValuePair.Key != null && keyValuePair.Key.Mission != null && !keyValuePair.Key.IsActive())
					{
						mbarrayList.Add(keyValuePair.Key);
					}
					else if (keyValuePair.Value.posture < keyValuePair.Value.maxPosture)
					{
						if (true)
						{
							if (keyValuePair.Key.IsPlayerControlled && (AgentPostures.postureVisual != null && AgentPostures.postureVisual._dataSource.ShowPlayerPostureStatus))
							{
								AgentPostures.postureVisual._dataSource.PlayerPosture = (int)keyValuePair.Value.posture;
								AgentPostures.postureVisual._dataSource.PlayerPostureMax = (int)keyValuePair.Value.maxPosture;
							}
							if (AgentPostures.postureVisual != null && AgentPostures.postureVisual._dataSource.ShowEnemyStatus && AgentPostures.postureVisual.affectedAgent == keyValuePair.Key)
							{
								AgentPostures.postureVisual._dataSource.EnemyPosture = (int)keyValuePair.Value.posture;
								AgentPostures.postureVisual._dataSource.EnemyPostureMax = (int)keyValuePair.Value.maxPosture;
							}
						}
						keyValuePair.Value.posture += keyValuePair.Value.regenPerTick * 30f;
					}
				}
				foreach (Agent key in mbarrayList)
				{
					AgentPostures.values.Remove(key);
				}
				mbarrayList.Clear();
				foreach (KeyValuePair<Agent, FormationClass> keyValuePair2 in PostureLogic.agentsToChangeFormation)
				{
					if (keyValuePair2.Key != null && keyValuePair2.Key.Mission != null && keyValuePair2.Key.IsActive() && keyValuePair2.Key.Team != null)
					{
						keyValuePair2.Key.Formation = keyValuePair2.Key.Team.GetFormation(keyValuePair2.Value);
						keyValuePair2.Key.DisableScriptedMovement();
					}
				}
				PostureLogic.agentsToChangeFormation.Clear();
				MBArrayList<Agent> mbarrayList2 = new MBArrayList<Agent>();
				for (int i = PostureLogic.agentsToDropShield.Count - 1; i >= 0; i--)
				{
					if (PostureLogic.agentsToDropShield[i] != null && PostureLogic.agentsToDropShield[i].Mission != null && PostureLogic.agentsToDropShield[i].IsActive())
					{
						Agent.ActionCodeType currentActionType = PostureLogic.agentsToDropShield[i].GetCurrentActionType(1);
						if (currentActionType != Agent.ActionCodeType.ReleaseMelee && currentActionType != Agent.ActionCodeType.ReleaseRanged && currentActionType != Agent.ActionCodeType.ReleaseThrowing && currentActionType != Agent.ActionCodeType.WeaponBash)
						{
							mbarrayList2.Add(PostureLogic.agentsToDropShield[i]);
						}
					}
					else
					{
						mbarrayList2.Add(PostureLogic.agentsToDropShield[i]);
					}
				}
				foreach (Agent agent in mbarrayList2)
				{
					if (agent != null && agent.Mission != null && agent.IsActive())
					{
						EquipmentIndex wieldedItemIndex = agent.GetWieldedItemIndex(Agent.HandIndex.OffHand);
						if (wieldedItemIndex != EquipmentIndex.None)
						{
							agent.DropItem(wieldedItemIndex, WeaponClass.Undefined);
							agent.UpdateAgentProperties();
						}
					}
					PostureLogic.agentsToDropShield.Remove(agent);
				}
				mbarrayList2.Clear();
				MBArrayList<Agent> mbarrayList3 = new MBArrayList<Agent>();
				for (int j = PostureLogic.agentsToDropWeapon.Count - 1; j >= 0; j--)
				{
					if (PostureLogic.agentsToDropWeapon[j] != null && PostureLogic.agentsToDropWeapon[j].Mission != null && PostureLogic.agentsToDropWeapon[j].IsActive())
					{
						Agent.ActionCodeType currentActionType2 = PostureLogic.agentsToDropWeapon[j].GetCurrentActionType(1);
						if (currentActionType2 != Agent.ActionCodeType.ReleaseMelee && currentActionType2 != Agent.ActionCodeType.ReleaseRanged && currentActionType2 != Agent.ActionCodeType.ReleaseThrowing && currentActionType2 != Agent.ActionCodeType.WeaponBash)
						{
							mbarrayList3.Add(PostureLogic.agentsToDropWeapon[j]);
						}
					}
					else
					{
						mbarrayList3.Add(PostureLogic.agentsToDropWeapon[j]);
					}
				}
				foreach (Agent agent2 in mbarrayList3)
				{
					if (agent2 != null && agent2.Mission != null && agent2.IsActive())
					{
						EquipmentIndex wieldedItemIndex2 = agent2.GetWieldedItemIndex(Agent.HandIndex.MainHand);
						if (wieldedItemIndex2 != EquipmentIndex.None)
						{
							agent2.DropItem(wieldedItemIndex2, WeaponClass.Undefined);
							agent2.UpdateAgentProperties();
						}
					}
					PostureLogic.agentsToDropWeapon.Remove(agent2);
				}
				mbarrayList3.Clear();
				PostureLogic.currentDt = 0f;
			}
		}

		// Token: 0x04000045 RID: 69
		private static float timeToCalc = 0.5f;

		// Token: 0x04000046 RID: 70
		private static float currentDt = 0f;

		// Token: 0x04000047 RID: 71
		private static int postureEffectCheck = 0;

		// Token: 0x04000048 RID: 72
		private static int postureEffectCheckCooldown = 15;

		// Token: 0x04000049 RID: 73
		private static float weaponLengthPostureFactor = 0.2f;

		// Token: 0x0400004A RID: 74
		private static float weaponWeightPostureFactor = 0.5f;

		// Token: 0x0400004B RID: 75
		private static float relativeSpeedPostureFactor = 0.6f;

		// Token: 0x0400004C RID: 76
		private static float lwrResultModifier = 3f;

		// Token: 0x0400004D RID: 77
		public static MBArrayList<Agent> agentsToDropShield = new MBArrayList<Agent>();

		// Token: 0x0400004E RID: 78
		public static MBArrayList<Agent> agentsToDropWeapon = new MBArrayList<Agent>();

		// Token: 0x0400004F RID: 79
		public static Dictionary<Agent, FormationClass> agentsToChangeFormation = new Dictionary<Agent, FormationClass>();

		// Token: 0x02000063 RID: 99
		[HarmonyPatch(typeof(Agent))]
		[HarmonyPatch("EquipItemsFromSpawnEquipment")]
		private class EquipItemsFromSpawnEquipmentPatch
		{
			// Token: 0x0600020F RID: 527
			private static void Prefix(ref Agent __instance)
			{
				if (__instance.IsHuman)
				{
					AgentPostures.values[__instance] = new Posture();
				}
			}
		}

		// Token: 0x02000064 RID: 100
		[HarmonyPatch(typeof(Agent))]
		[HarmonyPatch("OnWieldedItemIndexChange")]
		private class OnWieldedItemIndexChangePatch
		{
			// Token: 0x06000211 RID: 529
			private static void Postfix(ref Agent __instance, bool isOffHand, bool isWieldedInstantly, bool isWieldedOnSpawn)
			{
				float num = 1f;
				float num2;
				if (num != 0f)
				{
					if (num != 1f)
					{
						if (num != 2f)
						{
							num2 = 1f;
						}
						else
						{
							num2 = 2f;
						}
					}
					else
					{
						num2 = 1.5f;
					}
				}
				else
				{
					num2 = 1f;
				}
				if (true)
				{
					Posture posture = null;
					AgentPostures.values.TryGetValue(__instance, out posture);
					if (posture == null)
					{
						AgentPostures.values[__instance] = new Posture();
					}
					AgentPostures.values.TryGetValue(__instance, out posture);
					if (posture != null)
					{
						float posture2 = posture.posture;
						float maxPosture = posture.maxPosture;
						float num3 = posture2 / maxPosture;
						EquipmentIndex wieldedItemIndex = __instance.GetWieldedItemIndex(Agent.HandIndex.MainHand);
						if (wieldedItemIndex != EquipmentIndex.None)
						{
							int currentUsageIndex = __instance.Equipment[wieldedItemIndex].CurrentUsageIndex;
							SkillObject relevantSkillFromWeaponClass = WeaponComponentData.GetRelevantSkillFromWeaponClass(__instance.Equipment[wieldedItemIndex].GetWeaponComponentDataForUsage(currentUsageIndex).WeaponClass);
							int num4 = 0;
							if (relevantSkillFromWeaponClass != null)
							{
								num4 = MissionGameModels.Current.AgentStatCalculateModel.GetEffectiveSkill(__instance, relevantSkillFromWeaponClass);
							}
							float num5 = 20f;
							float num6 = 80f;
							float num7 = 500f;
							float num8 = 500f;
							float num9 = 0.008f;
							float num10 = 0.032f;
							float num11 = 1f;
							if (__instance.HasMount)
							{
								int effectiveSkill = MissionGameModels.Current.AgentStatCalculateModel.GetEffectiveSkill(__instance, DefaultSkills.Riding);
								posture.maxPosture = num5 * (num11 + (float)effectiveSkill / num7) + num6 * (num11 + (float)num4 / num8);
								posture.regenPerTick = num9 * (num11 + (float)effectiveSkill / num7) + num10 * (num11 + (float)num4 / num8);
							}
							else
							{
								int effectiveSkill2 = MissionGameModels.Current.AgentStatCalculateModel.GetEffectiveSkill(__instance, DefaultSkills.Athletics);
								posture.maxPosture = num5 * (num11 + (float)effectiveSkill2 / num7) + num6 * (num11 + (float)num4 / num8);
								posture.regenPerTick = num9 * (num11 + (float)effectiveSkill2 / num7) + num10 * (num11 + (float)num4 / num8);
							}
							if (__instance.IsPlayerControlled)
							{
								posture.maxPosture *= num2;
								posture.regenPerTick *= num2;
							}
							posture.posture = posture.maxPosture * num3;
						}
					}
				}
			}
		}

		// Token: 0x02000065 RID: 101
		[HarmonyPatch(typeof(MissionState))]
		[HarmonyPatch("LoadMission")]
		public class LoadMissionPatch
		{
			// Token: 0x06000213 RID: 531
			private static void Postfix()
			{
				AgentPostures.values.Clear();
				PostureLogic.agentsToDropShield.Clear();
				PostureLogic.agentsToDropWeapon.Clear();
				PostureLogic.agentsToChangeFormation.Clear();
			}
		}

		// Token: 0x02000066 RID: 102
		[HarmonyPatch(typeof(MissionState))]
		[HarmonyPatch("OnDeactivate")]
		public class OnDeactivatePatch
		{
			// Token: 0x06000215 RID: 533
			private static void Postfix()
			{
				AgentPostures.values.Clear();
				PostureLogic.agentsToDropShield.Clear();
				PostureLogic.agentsToDropWeapon.Clear();
				PostureLogic.agentsToChangeFormation.Clear();
			}
		}

		// Token: 0x02000067 RID: 103
		[HarmonyPatch(typeof(Mission))]
		[HarmonyPatch("CreateMeleeBlow")]
		private class CreateMeleeBlowPatch
		{
			// Token: 0x06000217 RID: 535
			public static void ResetPostureForAgent(ref Posture posture, float postureResetModifier, Agent agent)
			{
				if (posture != null)
				{
					if (agent != null && posture.maxPostureLossCount >= 1)
					{
						agent.AgentDrivenProperties.WeaponInaccuracy *= (float)posture.maxPostureLossCount * 1.1f;
					}
					float currentTime = Mission.Current.CurrentTime;
					int num = (posture.lastPostureLossTime > 0f) ? MathF.Floor((currentTime - posture.lastPostureLossTime) / 20f) : 0;
					posture.maxPostureLossCount -= num;
					if (posture.maxPostureLossCount < 10)
					{
						posture.maxPostureLossCount++;
					}
					posture.posture = posture.maxPosture * (postureResetModifier * (1f - 0.05f * (float)posture.maxPostureLossCount));
					posture.lastPostureLossTime = Mission.Current.CurrentTime;
				}
			}

			// Token: 0x06000218 RID: 536
			private static void Postfix(ref Mission __instance, ref Blow __result, Agent attackerAgent, Agent victimAgent, ref AttackCollisionData collisionData, in MissionWeapon attackerWeapon, CrushThroughState crushThroughState, Vec3 blowDirection, Vec3 swingDirection, bool cancelDamage)
			{
				if (new StackTrace().GetFrame(3).GetMethod().Name.Contains("MeleeHit") && victimAgent != null && victimAgent.IsHuman)
				{
					bool flag2;
					if (attackerAgent != null && victimAgent != null && !attackerAgent.IsFriendOf(victimAgent))
					{
						MissionWeapon missionWeapon = attackerWeapon;
						if (missionWeapon.CurrentUsageItem != null)
						{
							missionWeapon = attackerWeapon;
							flag2 = (missionWeapon.CurrentUsageItem != null);
							goto IL_69;
						}
					}
					flag2 = false;
					IL_69:
					if (flag2)
					{
						Posture posture = null;
						Posture posture2 = null;
						AgentPostures.values.TryGetValue(victimAgent, out posture);
						AgentPostures.values.TryGetValue(attackerAgent, out posture2);
						float postureResetModifier = 0.75f;
						float postureResetModifier2 = 1f;
						float absoluteDamageModifier = 3f;
						float absoluteDamageModifier2 = 1.2f;
						bool flag4 = attackerAgent.GetCurrentVelocity().Length >= attackerAgent.WalkSpeedCached;
						float comHitModifier = Utilities.GetComHitModifier(collisionData, attackerWeapon);
						if (!collisionData.AttackBlockedWithShield)
						{
							if (collisionData.CollisionResult == CombatCollisionResult.Blocked)
							{
								if (posture != null)
								{
									float num = PostureLogic.CreateMeleeBlowPatch.calculateDefenderPostureDamage(victimAgent, attackerAgent, absoluteDamageModifier, 0.85f, ref collisionData, attackerWeapon, comHitModifier);
									posture.posture -= num;
									PostureLogic.CreateMeleeBlowPatch.addPosturedamageVisual(attackerAgent, victimAgent);
									if (posture.posture <= 0f)
									{
										float f = PostureLogic.CreateMeleeBlowPatch.calculateHealthDamage(attackerWeapon, attackerAgent, victimAgent, num, __result, victimAgent);
										if (victimAgent.GetWieldedItemIndex(Agent.HandIndex.MainHand) != EquipmentIndex.None)
										{
											if (victimAgent.IsPlayerControlled)
											{
												InformationManager.DisplayMessage(new InformationMessage("Posture break: Posture depleted, " + MathF.Floor(f).ToString() + " damage crushed through", Color.FromUint(4282569842U)));
											}
											if (attackerAgent.IsPlayerControlled)
											{
												InformationManager.DisplayMessage(new InformationMessage("Enemy Posture break: Posture depleted, " + MathF.Floor(f).ToString() + " damage crushed through", Color.FromUint(4282569842U)));
											}
											if (!victimAgent.HasMount)
											{
												if (num >= posture.maxPosture * 0.33f)
												{
													PostureLogic.CreateMeleeBlowPatch.makePostureCrashThroughBlow(ref __instance, __result, attackerAgent, victimAgent, MathF.Floor(f), ref collisionData, attackerWeapon, crushThroughState, blowDirection, swingDirection, cancelDamage, BlowFlags.KnockBack);
													PostureLogic.agentsToDropWeapon.Add(victimAgent);
													if (!PostureLogic.agentsToDropWeapon.Contains(victimAgent))
													{
														PostureLogic.agentsToDropWeapon.Add(victimAgent);
													}
												}
												else
												{
													PostureLogic.CreateMeleeBlowPatch.makePostureCrashThroughBlow(ref __instance, __result, attackerAgent, victimAgent, MathF.Floor(f), ref collisionData, attackerWeapon, crushThroughState, blowDirection, swingDirection, cancelDamage, BlowFlags.KnockBack);
												}
											}
											else
											{
												PostureLogic.CreateMeleeBlowPatch.makePostureCrashThroughBlow(ref __instance, __result, attackerAgent, victimAgent, MathF.Floor(f), ref collisionData, attackerWeapon, crushThroughState, blowDirection, swingDirection, cancelDamage, BlowFlags.CanDismount);
											}
										}
										PostureLogic.CreateMeleeBlowPatch.ResetPostureForAgent(ref posture, postureResetModifier, victimAgent);
										PostureLogic.CreateMeleeBlowPatch.addPosturedamageVisual(attackerAgent, victimAgent);
									}
								}
								if (posture2 != null)
								{
									float num2 = PostureLogic.CreateMeleeBlowPatch.calculateAttackerPostureDamage(victimAgent, attackerAgent, absoluteDamageModifier, 0.25f, ref collisionData, attackerWeapon, comHitModifier);
									posture2.posture -= num2;
									if (posture2.posture <= 0f)
									{
										if (num2 >= posture2.maxPosture * 0.33f)
										{
											PostureLogic.CreateMeleeBlowPatch.makePostureRiposteBlow(ref __instance, __result, attackerAgent, victimAgent, ref collisionData, attackerWeapon, crushThroughState, blowDirection, swingDirection, cancelDamage, BlowFlags.KnockBack);
											PostureLogic.agentsToDropWeapon.Add(attackerAgent);
											if (!PostureLogic.agentsToDropWeapon.Contains(attackerAgent))
											{
												PostureLogic.agentsToDropWeapon.Add(attackerAgent);
											}
											PostureLogic.CreateMeleeBlowPatch.ResetPostureForAgent(ref posture2, postureResetModifier, attackerAgent);
											PostureLogic.CreateMeleeBlowPatch.addPosturedamageVisual(attackerAgent, victimAgent);
										}
										else
										{
											posture2.posture = 0f;
										}
									}
									PostureLogic.CreateMeleeBlowPatch.addPosturedamageVisual(attackerAgent, victimAgent);
									return;
								}
							}
							else if (collisionData.CollisionResult == CombatCollisionResult.Parried)
							{
								if (posture != null)
								{
									float num3 = PostureLogic.CreateMeleeBlowPatch.calculateDefenderPostureDamage(victimAgent, attackerAgent, absoluteDamageModifier, 0.5f, ref collisionData, attackerWeapon, comHitModifier);
									posture.posture -= num3;
									PostureLogic.CreateMeleeBlowPatch.addPosturedamageVisual(attackerAgent, victimAgent);
									if (posture.posture <= 0f)
									{
										float f2 = PostureLogic.CreateMeleeBlowPatch.calculateHealthDamage(attackerWeapon, attackerAgent, victimAgent, num3, __result, victimAgent);
										if (victimAgent.GetWieldedItemIndex(Agent.HandIndex.MainHand) != EquipmentIndex.None)
										{
											if (victimAgent.IsPlayerControlled)
											{
												InformationManager.DisplayMessage(new InformationMessage("Posture break: Posture depleted, perfect parry, " + MathF.Floor(f2).ToString() + " damage crushed through", Color.FromUint(4282569842U)));
											}
											if (attackerAgent.IsPlayerControlled)
											{
												InformationManager.DisplayMessage(new InformationMessage("Enemy Posture break: Posture depleted, perfect parry, " + MathF.Floor(f2).ToString() + " damage crushed through", Color.FromUint(4282569842U)));
											}
											if (num3 >= posture.maxPosture * 0.33f)
											{
												PostureLogic.CreateMeleeBlowPatch.makePostureCrashThroughBlow(ref __instance, __result, attackerAgent, victimAgent, MathF.Floor(f2), ref collisionData, attackerWeapon, crushThroughState, blowDirection, swingDirection, cancelDamage, BlowFlags.KnockBack);
												PostureLogic.agentsToDropWeapon.Add(victimAgent);
												if (!PostureLogic.agentsToDropWeapon.Contains(victimAgent))
												{
													PostureLogic.agentsToDropWeapon.Add(victimAgent);
												}
											}
											else
											{
												PostureLogic.CreateMeleeBlowPatch.makePostureCrashThroughBlow(ref __instance, __result, attackerAgent, victimAgent, MathF.Floor(f2), ref collisionData, attackerWeapon, crushThroughState, blowDirection, swingDirection, cancelDamage, BlowFlags.KnockBack);
											}
										}
										PostureLogic.CreateMeleeBlowPatch.ResetPostureForAgent(ref posture, postureResetModifier, victimAgent);
										PostureLogic.CreateMeleeBlowPatch.addPosturedamageVisual(attackerAgent, victimAgent);
									}
								}
								if (posture2 != null)
								{
									float num4 = PostureLogic.CreateMeleeBlowPatch.calculateAttackerPostureDamage(victimAgent, attackerAgent, absoluteDamageModifier, 0.75f, ref collisionData, attackerWeapon, comHitModifier);
									posture2.posture -= num4;
									PostureLogic.CreateMeleeBlowPatch.addPosturedamageVisual(attackerAgent, victimAgent);
									if (posture2.posture <= 0f)
									{
										if (attackerAgent.IsPlayerControlled)
										{
											InformationManager.DisplayMessage(new InformationMessage("Posture break: Posture depleted, perfect parry", Color.FromUint(4282569842U)));
										}
										if (!attackerAgent.HasMount)
										{
											if (num4 >= posture2.maxPosture * 0.33f)
											{
												PostureLogic.CreateMeleeBlowPatch.makePostureRiposteBlow(ref __instance, __result, attackerAgent, victimAgent, ref collisionData, attackerWeapon, crushThroughState, blowDirection, swingDirection, cancelDamage, BlowFlags.KnockBack);
												PostureLogic.agentsToDropWeapon.Add(attackerAgent);
												if (!PostureLogic.agentsToDropWeapon.Contains(attackerAgent))
												{
													PostureLogic.agentsToDropWeapon.Add(attackerAgent);
												}
											}
											else
											{
												PostureLogic.CreateMeleeBlowPatch.makePostureRiposteBlow(ref __instance, __result, attackerAgent, victimAgent, ref collisionData, attackerWeapon, crushThroughState, blowDirection, swingDirection, cancelDamage, BlowFlags.KnockBack);
											}
										}
										else
										{
											PostureLogic.CreateMeleeBlowPatch.makePostureRiposteBlow(ref __instance, __result, attackerAgent, victimAgent, ref collisionData, attackerWeapon, crushThroughState, blowDirection, swingDirection, cancelDamage, BlowFlags.CanDismount);
										}
										PostureLogic.CreateMeleeBlowPatch.ResetPostureForAgent(ref posture2, postureResetModifier, attackerAgent);
										PostureLogic.CreateMeleeBlowPatch.addPosturedamageVisual(attackerAgent, victimAgent);
										return;
									}
								}
							}
							else if (victimAgent.IsHuman && attackerAgent.IsHuman && collisionData.CollisionResult == CombatCollisionResult.StrikeAgent)
							{
								if (posture != null)
								{
									float num5 = PostureLogic.CreateMeleeBlowPatch.calculateDefenderPostureDamage(victimAgent, attackerAgent, absoluteDamageModifier, 0.8f, ref collisionData, attackerWeapon, comHitModifier);
									posture.posture -= num5;
									PostureLogic.CreateMeleeBlowPatch.addPosturedamageVisual(attackerAgent, victimAgent);
									if (posture.posture <= 0f)
									{
										if (victimAgent.GetWieldedItemIndex(Agent.HandIndex.MainHand) != EquipmentIndex.None)
										{
											if (!victimAgent.HasMount)
											{
												bool flag3;
												if (num5 >= posture.maxPosture * 0.33f)
												{
													MissionWeapon missionWeapon2 = attackerWeapon;
													if (!missionWeapon2.IsEmpty)
													{
														missionWeapon2 = attackerWeapon;
														if (missionWeapon2.CurrentUsageItem.WeaponClass != WeaponClass.TwoHandedAxe)
														{
															missionWeapon2 = attackerWeapon;
															if (missionWeapon2.CurrentUsageItem.WeaponClass != WeaponClass.TwoHandedMace)
															{
																missionWeapon2 = attackerWeapon;
																if (missionWeapon2.CurrentUsageItem.WeaponClass != WeaponClass.TwoHandedPolearm)
																{
																	missionWeapon2 = attackerWeapon;
																	flag3 = (missionWeapon2.CurrentUsageItem.WeaponClass == WeaponClass.TwoHandedSword);
																	goto IL_741;
																}
															}
														}
														flag3 = true;
														goto IL_741;
													}
												}
												flag3 = false;
												IL_741:
												if (flag3)
												{
													PostureLogic.CreateMeleeBlowPatch.makePostureBlow(ref __instance, __result, attackerAgent, victimAgent, ref collisionData, attackerWeapon, crushThroughState, blowDirection, swingDirection, cancelDamage, BlowFlags.KnockDown);
												}
												else
												{
													PostureLogic.CreateMeleeBlowPatch.makePostureBlow(ref __instance, __result, attackerAgent, victimAgent, ref collisionData, attackerWeapon, crushThroughState, blowDirection, swingDirection, cancelDamage, BlowFlags.KnockBack);
												}
											}
											else
											{
												PostureLogic.CreateMeleeBlowPatch.makePostureBlow(ref __instance, __result, attackerAgent, victimAgent, ref collisionData, attackerWeapon, crushThroughState, blowDirection, swingDirection, cancelDamage, BlowFlags.CanDismount);
											}
										}
										PostureLogic.CreateMeleeBlowPatch.ResetPostureForAgent(ref posture, postureResetModifier, victimAgent);
										PostureLogic.CreateMeleeBlowPatch.addPosturedamageVisual(attackerAgent, victimAgent);
									}
								}
								if (posture2 != null)
								{
									posture2.posture -= PostureLogic.CreateMeleeBlowPatch.calculateAttackerPostureDamage(victimAgent, attackerAgent, absoluteDamageModifier, 0.5f, ref collisionData, attackerWeapon, comHitModifier);
									PostureLogic.CreateMeleeBlowPatch.addPosturedamageVisual(attackerAgent, victimAgent);
									if (posture2.posture <= 0f)
									{
										posture2.posture = 0f;
										return;
									}
								}
							}
						}
						else if (collisionData.AttackBlockedWithShield)
						{
							if (collisionData.CollisionResult == CombatCollisionResult.Blocked && !collisionData.CorrectSideShieldBlock)
							{
								if (posture != null)
								{
									float num6 = PostureLogic.CreateMeleeBlowPatch.calculateDefenderPostureDamage(victimAgent, attackerAgent, absoluteDamageModifier2, 1f, ref collisionData, attackerWeapon, comHitModifier);
									posture.posture -= num6;
									PostureLogic.CreateMeleeBlowPatch.addPosturedamageVisual(attackerAgent, victimAgent);
									float f3 = PostureLogic.CreateMeleeBlowPatch.calculateHealthDamage(attackerWeapon, attackerAgent, victimAgent, num6, __result, victimAgent);
									if (posture.posture <= 0f)
									{
										PostureLogic.CreateMeleeBlowPatch.damageShield(victimAgent, 150);
										if (victimAgent.GetWieldedItemIndex(Agent.HandIndex.MainHand) != EquipmentIndex.None)
										{
											if (victimAgent.IsPlayerControlled)
											{
												InformationManager.DisplayMessage(new InformationMessage("Posture break: Posture depleted, incorrect side block", Color.FromUint(4282569842U)));
											}
											if (!victimAgent.HasMount)
											{
												if (num6 >= posture.maxPosture * 0.33f)
												{
													PostureLogic.CreateMeleeBlowPatch.makePostureCrashThroughBlow(ref __instance, __result, attackerAgent, victimAgent, MathF.Floor(f3), ref collisionData, attackerWeapon, crushThroughState, blowDirection, swingDirection, cancelDamage, BlowFlags.KnockBack);
													PostureLogic.CreateMeleeBlowPatch.makePostureBlow(ref __instance, __result, attackerAgent, victimAgent, ref collisionData, attackerWeapon, crushThroughState, blowDirection, swingDirection, cancelDamage, BlowFlags.KnockBack);
													if (!PostureLogic.agentsToDropShield.Contains(victimAgent))
													{
														PostureLogic.agentsToDropShield.Add(victimAgent);
													}
												}
												else
												{
													PostureLogic.CreateMeleeBlowPatch.makePostureBlow(ref __instance, __result, attackerAgent, victimAgent, ref collisionData, attackerWeapon, crushThroughState, blowDirection, swingDirection, cancelDamage, BlowFlags.KnockBack);
												}
											}
											else
											{
												PostureLogic.CreateMeleeBlowPatch.makePostureBlow(ref __instance, __result, attackerAgent, victimAgent, ref collisionData, attackerWeapon, crushThroughState, blowDirection, swingDirection, cancelDamage, BlowFlags.CanDismount);
											}
										}
										PostureLogic.CreateMeleeBlowPatch.ResetPostureForAgent(ref posture, postureResetModifier2, victimAgent);
										PostureLogic.CreateMeleeBlowPatch.addPosturedamageVisual(attackerAgent, victimAgent);
									}
								}
								if (posture2 != null)
								{
									posture2.posture -= PostureLogic.CreateMeleeBlowPatch.calculateAttackerPostureDamage(victimAgent, attackerAgent, absoluteDamageModifier2, 0.2f, ref collisionData, attackerWeapon, comHitModifier);
									PostureLogic.CreateMeleeBlowPatch.addPosturedamageVisual(attackerAgent, victimAgent);
									if (posture2.posture <= 0f)
									{
										posture2.posture = 0f;
										return;
									}
								}
							}
							else if ((collisionData.CollisionResult == CombatCollisionResult.Blocked && collisionData.CorrectSideShieldBlock) || (collisionData.CollisionResult == CombatCollisionResult.Parried && !collisionData.CorrectSideShieldBlock))
							{
								if (posture != null)
								{
									float num7 = PostureLogic.CreateMeleeBlowPatch.calculateDefenderPostureDamage(victimAgent, attackerAgent, absoluteDamageModifier2, 1f, ref collisionData, attackerWeapon, comHitModifier);
									posture.posture -= num7;
									PostureLogic.CreateMeleeBlowPatch.addPosturedamageVisual(attackerAgent, victimAgent);
									if (posture.posture <= 0f)
									{
										PostureLogic.CreateMeleeBlowPatch.damageShield(victimAgent, 125);
										if (victimAgent.GetWieldedItemIndex(Agent.HandIndex.MainHand) != EquipmentIndex.None)
										{
											if (victimAgent.IsPlayerControlled)
											{
												InformationManager.DisplayMessage(new InformationMessage("Posture break: Posture depleted, correct side block", Color.FromUint(4282569842U)));
											}
											if (!victimAgent.HasMount)
											{
												if (num7 >= posture.maxPosture * 0.33f)
												{
													PostureLogic.CreateMeleeBlowPatch.makePostureBlow(ref __instance, __result, attackerAgent, victimAgent, ref collisionData, attackerWeapon, crushThroughState, blowDirection, swingDirection, cancelDamage, BlowFlags.KnockBack);
													if (!PostureLogic.agentsToDropShield.Contains(victimAgent))
													{
														PostureLogic.agentsToDropShield.Add(victimAgent);
													}
												}
												else
												{
													PostureLogic.CreateMeleeBlowPatch.makePostureBlow(ref __instance, __result, attackerAgent, victimAgent, ref collisionData, attackerWeapon, crushThroughState, blowDirection, swingDirection, cancelDamage, BlowFlags.KnockBack);
												}
											}
											else
											{
												PostureLogic.CreateMeleeBlowPatch.makePostureBlow(ref __instance, __result, attackerAgent, victimAgent, ref collisionData, attackerWeapon, crushThroughState, blowDirection, swingDirection, cancelDamage, BlowFlags.CanDismount);
											}
										}
										PostureLogic.CreateMeleeBlowPatch.ResetPostureForAgent(ref posture, postureResetModifier2, victimAgent);
										PostureLogic.CreateMeleeBlowPatch.addPosturedamageVisual(attackerAgent, victimAgent);
									}
								}
								if (posture2 != null)
								{
									posture2.posture -= PostureLogic.CreateMeleeBlowPatch.calculateAttackerPostureDamage(victimAgent, attackerAgent, absoluteDamageModifier2, 0.3f, ref collisionData, attackerWeapon, comHitModifier);
									PostureLogic.CreateMeleeBlowPatch.addPosturedamageVisual(attackerAgent, victimAgent);
									if (posture2.posture <= 0f)
									{
										posture2.posture = 0f;
										return;
									}
								}
							}
							else if (collisionData.CollisionResult == CombatCollisionResult.Parried && collisionData.CorrectSideShieldBlock)
							{
								if (posture != null)
								{
									float num8 = PostureLogic.CreateMeleeBlowPatch.calculateDefenderPostureDamage(victimAgent, attackerAgent, absoluteDamageModifier2, 0.8f, ref collisionData, attackerWeapon, comHitModifier);
									posture.posture -= num8;
									PostureLogic.CreateMeleeBlowPatch.addPosturedamageVisual(attackerAgent, victimAgent);
									if (posture.posture <= 0f)
									{
										PostureLogic.CreateMeleeBlowPatch.damageShield(victimAgent, 100);
										if (victimAgent.GetWieldedItemIndex(Agent.HandIndex.MainHand) != EquipmentIndex.None)
										{
											if (victimAgent.IsPlayerControlled)
											{
												InformationManager.DisplayMessage(new InformationMessage("Posture break: Posture depleted, perfect parry, correct side block", Color.FromUint(4282569842U)));
											}
											if (!victimAgent.HasMount)
											{
												if (num8 >= posture.maxPosture * 0.33f)
												{
													PostureLogic.CreateMeleeBlowPatch.makePostureBlow(ref __instance, __result, attackerAgent, victimAgent, ref collisionData, attackerWeapon, crushThroughState, blowDirection, swingDirection, cancelDamage, BlowFlags.KnockBack);
													if (!PostureLogic.agentsToDropShield.Contains(victimAgent))
													{
														PostureLogic.agentsToDropShield.Add(victimAgent);
													}
												}
												else
												{
													PostureLogic.CreateMeleeBlowPatch.makePostureBlow(ref __instance, __result, attackerAgent, victimAgent, ref collisionData, attackerWeapon, crushThroughState, blowDirection, swingDirection, cancelDamage, BlowFlags.KnockBack);
												}
											}
											else
											{
												PostureLogic.CreateMeleeBlowPatch.makePostureBlow(ref __instance, __result, attackerAgent, victimAgent, ref collisionData, attackerWeapon, crushThroughState, blowDirection, swingDirection, cancelDamage, BlowFlags.KnockBack);
											}
										}
										PostureLogic.CreateMeleeBlowPatch.ResetPostureForAgent(ref posture, postureResetModifier2, victimAgent);
										PostureLogic.CreateMeleeBlowPatch.addPosturedamageVisual(attackerAgent, victimAgent);
									}
								}
								if (posture2 != null)
								{
									float num9 = PostureLogic.CreateMeleeBlowPatch.calculateAttackerPostureDamage(victimAgent, attackerAgent, absoluteDamageModifier2, 0.5f, ref collisionData, attackerWeapon, comHitModifier);
									posture2.posture -= num9;
									PostureLogic.CreateMeleeBlowPatch.addPosturedamageVisual(attackerAgent, victimAgent);
									if (posture2.posture <= 0f)
									{
										if (attackerAgent.IsPlayerControlled)
										{
											InformationManager.DisplayMessage(new InformationMessage("Posture break: Posture depleted, perfect parry, correct side block", Color.FromUint(4282569842U)));
										}
										if (num9 >= posture2.maxPosture * 0.33f)
										{
											PostureLogic.CreateMeleeBlowPatch.makePostureRiposteBlow(ref __instance, __result, attackerAgent, victimAgent, ref collisionData, attackerWeapon, crushThroughState, blowDirection, swingDirection, cancelDamage, BlowFlags.KnockBack);
											PostureLogic.agentsToDropWeapon.Add(attackerAgent);
											if (!PostureLogic.agentsToDropWeapon.Contains(attackerAgent))
											{
												PostureLogic.agentsToDropWeapon.Add(attackerAgent);
											}
											PostureLogic.CreateMeleeBlowPatch.ResetPostureForAgent(ref posture2, postureResetModifier, attackerAgent);
											PostureLogic.CreateMeleeBlowPatch.addPosturedamageVisual(attackerAgent, victimAgent);
										}
										else
										{
											PostureLogic.CreateMeleeBlowPatch.makePostureRiposteBlow(ref __instance, __result, attackerAgent, victimAgent, ref collisionData, attackerWeapon, crushThroughState, blowDirection, swingDirection, cancelDamage, BlowFlags.KnockBack);
										}
										PostureLogic.CreateMeleeBlowPatch.ResetPostureForAgent(ref posture2, postureResetModifier, attackerAgent);
										PostureLogic.CreateMeleeBlowPatch.addPosturedamageVisual(attackerAgent, victimAgent);
										return;
									}
								}
							}
						}
						else if (collisionData.CollisionResult == CombatCollisionResult.ChamberBlocked)
						{
							if (posture != null)
							{
								posture.posture -= PostureLogic.CreateMeleeBlowPatch.calculateDefenderPostureDamage(victimAgent, attackerAgent, absoluteDamageModifier, 0.25f, ref collisionData, attackerWeapon, comHitModifier);
								PostureLogic.CreateMeleeBlowPatch.addPosturedamageVisual(attackerAgent, victimAgent);
								if (posture.posture <= 0f)
								{
									if (victimAgent.GetWieldedItemIndex(Agent.HandIndex.MainHand) != EquipmentIndex.None)
									{
										if (victimAgent.IsPlayerControlled)
										{
											InformationManager.DisplayMessage(new InformationMessage("Posture break: Posture depleted, chamber block", Color.FromUint(4282569842U)));
										}
										PostureLogic.CreateMeleeBlowPatch.makePostureBlow(ref __instance, __result, attackerAgent, victimAgent, ref collisionData, attackerWeapon, crushThroughState, blowDirection, swingDirection, cancelDamage, BlowFlags.NonTipThrust);
									}
									PostureLogic.CreateMeleeBlowPatch.ResetPostureForAgent(ref posture, postureResetModifier, victimAgent);
									PostureLogic.CreateMeleeBlowPatch.addPosturedamageVisual(attackerAgent, victimAgent);
								}
							}
							if (posture2 != null)
							{
								float num10 = PostureLogic.CreateMeleeBlowPatch.calculateAttackerPostureDamage(victimAgent, attackerAgent, absoluteDamageModifier, 2f, ref collisionData, attackerWeapon, comHitModifier);
								posture2.posture -= num10;
								PostureLogic.CreateMeleeBlowPatch.addPosturedamageVisual(attackerAgent, victimAgent);
								if (posture2.posture <= 0f)
								{
									float f4 = PostureLogic.CreateMeleeBlowPatch.calculateHealthDamage(attackerWeapon, attackerAgent, victimAgent, num10, __result, attackerAgent);
									if (attackerAgent.IsPlayerControlled)
									{
										InformationManager.DisplayMessage(new InformationMessage("Posture break: Posture depleted, chamber block " + MathF.Floor(f4).ToString() + " damage crushed through", Color.FromUint(4282569842U)));
									}
									PostureLogic.CreateMeleeBlowPatch.makePostureCrashThroughBlow(ref __instance, __result, attackerAgent, victimAgent, MathF.Floor(f4), ref collisionData, attackerWeapon, crushThroughState, blowDirection, swingDirection, cancelDamage, BlowFlags.KnockDown);
									PostureLogic.CreateMeleeBlowPatch.ResetPostureForAgent(ref posture2, postureResetModifier, attackerAgent);
									PostureLogic.CreateMeleeBlowPatch.addPosturedamageVisual(attackerAgent, victimAgent);
									return;
								}
								float f5 = PostureLogic.CreateMeleeBlowPatch.calculateHealthDamage(attackerWeapon, attackerAgent, victimAgent, num10, __result, attackerAgent);
								if (attackerAgent.IsPlayerControlled)
								{
									InformationManager.DisplayMessage(new InformationMessage("Chamber block " + MathF.Floor(f5).ToString() + " damage crushed through", Color.FromUint(4282569842U)));
								}
								PostureLogic.CreateMeleeBlowPatch.makePostureCrashThroughBlow(ref __instance, __result, attackerAgent, victimAgent, MathF.Floor(f5), ref collisionData, attackerWeapon, crushThroughState, blowDirection, swingDirection, cancelDamage, BlowFlags.KnockBack);
								PostureLogic.CreateMeleeBlowPatch.ResetPostureForAgent(ref posture2, postureResetModifier, attackerAgent);
								PostureLogic.CreateMeleeBlowPatch.addPosturedamageVisual(attackerAgent, victimAgent);
							}
						}
					}
				}
			}

			// Token: 0x06000219 RID: 537
			private static void damageShield(Agent victim, int ammount)
			{
				for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; equipmentIndex++)
				{
					if (victim.Equipment != null && !victim.Equipment[equipmentIndex].IsEmpty && (victim.Equipment[equipmentIndex].Item.Type == ItemObject.ItemTypeEnum.Shield && !victim.WieldedOffhandWeapon.IsEmpty && victim.WieldedOffhandWeapon.Item.Id == victim.Equipment[equipmentIndex].Item.Id))
					{
						int num = MathF.Max(0, (int)victim.Equipment[equipmentIndex].HitPoints - ammount);
						victim.ChangeWeaponHitPoints(equipmentIndex, (short)num);
						return;
					}
				}
			}

			// Token: 0x0600021A RID: 538
			private static void addPosturedamageVisual(Agent attackerAgent, Agent victimAgent)
			{
				if (true && (victimAgent != null && attackerAgent != null && (victimAgent.IsPlayerControlled || attackerAgent.IsPlayerControlled)))
				{
					Agent agent;
					if (victimAgent.IsPlayerControlled)
					{
						agent = attackerAgent;
						Posture posture = null;
						if (AgentPostures.values.TryGetValue(victimAgent, out posture) && (AgentPostures.postureVisual != null && AgentPostures.postureVisual._dataSource.ShowPlayerPostureStatus))
						{
							AgentPostures.postureVisual._dataSource.PlayerPosture = (int)posture.posture;
							AgentPostures.postureVisual._dataSource.PlayerPostureMax = (int)posture.maxPosture;
						}
					}
					else
					{
						agent = victimAgent;
						Posture posture2 = null;
						if (AgentPostures.values.TryGetValue(attackerAgent, out posture2) && (AgentPostures.postureVisual != null && AgentPostures.postureVisual._dataSource.ShowPlayerPostureStatus))
						{
							AgentPostures.postureVisual._dataSource.PlayerPosture = (int)posture2.posture;
							AgentPostures.postureVisual._dataSource.PlayerPostureMax = (int)posture2.maxPosture;
						}
					}
					if (AgentPostures.postureVisual != null)
					{
						Posture posture3 = null;
						if (AgentPostures.values.TryGetValue(agent, out posture3))
						{
							AgentPostures.postureVisual._dataSource.ShowEnemyStatus = true;
							AgentPostures.postureVisual.affectedAgent = agent;
							if (AgentPostures.postureVisual._dataSource.ShowEnemyStatus && AgentPostures.postureVisual.affectedAgent == agent)
							{
								AgentPostures.postureVisual.timer = AgentPostures.postureVisual.DisplayTime;
								AgentPostures.postureVisual._dataSource.EnemyPosture = (int)posture3.posture;
								AgentPostures.postureVisual._dataSource.EnemyPostureMax = (int)posture3.maxPosture;
								AgentPostures.postureVisual._dataSource.EnemyHealth = (int)agent.Health;
								AgentPostures.postureVisual._dataSource.EnemyHealthMax = (int)agent.HealthLimit;
								if (agent.IsMount)
								{
									PostureVisualVM dataSource = AgentPostures.postureVisual._dataSource;
									Agent riderAgent = agent.RiderAgent;
									dataSource.EnemyName = ((riderAgent != null) ? riderAgent.Name : null) + " (Mount)";
									return;
								}
								AgentPostures.postureVisual._dataSource.EnemyName = agent.Name;
							}
						}
					}
				}
			}

			// Token: 0x0600021B RID: 539
			public static float CalculateSweetSpotSwingMagnitude(BasicCharacterObject character, MissionWeapon weapon, int weaponUsageIndex, int relevantSkill)
			{
				float num = 1f;
				float num2 = -1f;
				if (weapon.Item != null && weapon.Item.GetWeaponWithUsageIndex(weaponUsageIndex) != null)
				{
					float num3 = (float)weapon.GetModifiedSwingSpeedForCurrentUsage() / 4.5454545f * num;
					float effectiveSkillWithDR = Utilities.GetEffectiveSkillWithDR(relevantSkill);
					switch (weapon.Item.GetWeaponWithUsageIndex(weaponUsageIndex).WeaponClass)
					{
					case WeaponClass.Dagger:
					case WeaponClass.OneHandedSword:
					case WeaponClass.TwoHandedSword:
					{
						float num4 = 1f + effectiveSkillWithDR / 800f;
						num3 = num3 * 0.83f * num4 * num;
						break;
					}
					case WeaponClass.OneHandedAxe:
					case WeaponClass.Mace:
					case WeaponClass.TwoHandedMace:
					case WeaponClass.OneHandedPolearm:
					case WeaponClass.LowGripPolearm:
					{
						float num5 = 1f + effectiveSkillWithDR / 1000f;
						num3 = num3 * 0.83f * num5 * num;
						break;
					}
					case WeaponClass.TwoHandedAxe:
					{
						float num6 = 1f + effectiveSkillWithDR / 800f;
						num3 = num3 * 0.75f * num6 * num;
						break;
					}
					case WeaponClass.TwoHandedPolearm:
					{
						float num7 = 1f + effectiveSkillWithDR / 1000f;
						num3 = num3 * 0.83f * num7 * num;
						break;
					}
					}
					float weight = weapon.Item.Weight;
					float inertia = weapon.Item.GetWeaponWithUsageIndex(weaponUsageIndex).Inertia;
					float centerOfMass = weapon.Item.GetWeaponWithUsageIndex(weaponUsageIndex).CenterOfMass;
					for (float num8 = 1f; num8 > 0.35f; num8 -= 0.01f)
					{
						float num9 = CombatStatCalculator.CalculateStrikeMagnitudeForSwing(num3, num8, weight, weapon.Item.GetWeaponWithUsageIndex(weaponUsageIndex).GetRealWeaponLength(), inertia, centerOfMass, 0f);
						if (num9 > num2)
						{
							num2 = num9;
						}
					}
				}
				return num2;
			}

			// Token: 0x0600021C RID: 540
			public static float CalculateThrustMagnitude(BasicCharacterObject character, MissionWeapon weapon, int weaponUsageIndex, int relevantSkill)
			{
				float num = 1f;
				float result = -1f;
				if (weapon.Item != null && weapon.Item.GetWeaponWithUsageIndex(weaponUsageIndex) != null)
				{
					float num2 = (float)weapon.GetModifiedThrustSpeedForCurrentUsage() / 11.764706f * num;
					float effectiveSkillWithDR = Utilities.GetEffectiveSkillWithDR(relevantSkill);
					float weight = weapon.Item.Weight;
					float inertia = weapon.Item.GetWeaponWithUsageIndex(weaponUsageIndex).Inertia;
					float centerOfMass = weapon.Item.GetWeaponWithUsageIndex(weaponUsageIndex).CenterOfMass;
					switch (weapon.Item.GetWeaponWithUsageIndex(weaponUsageIndex).WeaponClass)
					{
					case WeaponClass.Dagger:
					case WeaponClass.OneHandedSword:
					case WeaponClass.TwoHandedSword:
					{
						float num3 = 1f + effectiveSkillWithDR / 800f;
						num2 = Utilities.CalculateThrustSpeed(weight, inertia, centerOfMass);
						num2 = num2 * 0.7f * num3 * num;
						break;
					}
					case WeaponClass.OneHandedAxe:
					case WeaponClass.Mace:
					case WeaponClass.TwoHandedMace:
					case WeaponClass.OneHandedPolearm:
					case WeaponClass.LowGripPolearm:
					{
						float num4 = 1f + effectiveSkillWithDR / 1000f;
						num2 = Utilities.CalculateThrustSpeed(weight, inertia, centerOfMass);
						num2 = num2 * 0.75f * num4 * num;
						break;
					}
					case WeaponClass.TwoHandedAxe:
					{
						float num5 = 1f + effectiveSkillWithDR / 1000f;
						num2 = Utilities.CalculateThrustSpeed(weight, inertia, centerOfMass);
						num2 = num2 * 0.9f * num5 * num;
						break;
					}
					case WeaponClass.TwoHandedPolearm:
					{
						float num6 = 1f + effectiveSkillWithDR / 1000f;
						num2 = Utilities.CalculateThrustSpeed(weight, inertia, centerOfMass);
						num2 = num2 * 0.7f * num6 * num;
						break;
					}
					}
					switch (weapon.Item.GetWeaponWithUsageIndex(weaponUsageIndex).WeaponClass)
					{
					case WeaponClass.Dagger:
					case WeaponClass.OneHandedSword:
					case WeaponClass.Mace:
					case WeaponClass.OneHandedPolearm:
						result = Utilities.CalculateThrustMagnitudeForOneHandedWeapon(weight, effectiveSkillWithDR, num2, 0f, Agent.UsageDirection.AttackDown);
						break;
					case WeaponClass.TwoHandedSword:
					case WeaponClass.TwoHandedPolearm:
						result = Utilities.CalculateThrustMagnitudeForTwoHandedWeapon(weight, effectiveSkillWithDR, num2, 0f, Agent.UsageDirection.AttackDown);
						break;
					}
				}
				return result;
			}

			// Token: 0x0600021D RID: 541
			public static float calculateHealthDamage(MissionWeapon targetWeapon, Agent attacker, Agent vicitm, float overPostureDamage, Blow b, Agent victimAgent)
			{
				return 0f;
				/*float num = victimAgent.GetBaseArmorEffectivenessForBodyPart(BoneBodyPartType.Head);
				num += victimAgent.GetBaseArmorEffectivenessForBodyPart(BoneBodyPartType.Neck);
				num += victimAgent.GetBaseArmorEffectivenessForBodyPart(BoneBodyPartType.Chest);
				num += victimAgent.GetBaseArmorEffectivenessForBodyPart(BoneBodyPartType.Abdomen);
				num += victimAgent.GetBaseArmorEffectivenessForBodyPart(BoneBodyPartType.ShoulderLeft);
				num += victimAgent.GetBaseArmorEffectivenessForBodyPart(BoneBodyPartType.ShoulderRight);
				num += victimAgent.GetBaseArmorEffectivenessForBodyPart(BoneBodyPartType.ArmLeft);
				num += victimAgent.GetBaseArmorEffectivenessForBodyPart(BoneBodyPartType.ArmRight);
				num += victimAgent.GetBaseArmorEffectivenessForBodyPart(BoneBodyPartType.Legs);
				num /= 9f;
				if (true)
				{
					int currentUsageIndex = targetWeapon.CurrentUsageIndex;
					BasicCharacterObject character = attacker.Character;
					if (character != null && !targetWeapon.IsEmpty && targetWeapon.Item.GetWeaponWithUsageIndex(currentUsageIndex) != null && targetWeapon.Item.GetWeaponWithUsageIndex(currentUsageIndex).IsMeleeWeapon && character != null)
					{
						SkillObject relevantSkill = targetWeapon.Item.GetWeaponWithUsageIndex(currentUsageIndex).RelevantSkill;
						int skillValue = character.GetSkillValue(relevantSkill);
						float effectiveSkillWithDR = Utilities.GetEffectiveSkillWithDR(skillValue);
						float skillModifier = Utilities.CalculateSkillModifier(skillValue);
						int num2;
						int num3;
						int num4;
						Utilities.CalculateVisualSpeeds(targetWeapon, currentUsageIndex, effectiveSkillWithDR, out num2, out num3, out num4);
						float num10 = (float)num2 / Utilities.swingSpeedTransfer;
						float num11 = (float)num3 / Utilities.thrustSpeedTransfer;
						if (b.StrikeType == StrikeType.Swing)
						{
							float skillBasedDamage = Utilities.GetSkillBasedDamage(PostureLogic.CreateMeleeBlowPatch.CalculateSweetSpotSwingMagnitude(character, targetWeapon, currentUsageIndex, skillValue), false, targetWeapon.Item.GetWeaponWithUsageIndex(currentUsageIndex).WeaponClass.ToString(), targetWeapon.Item.GetWeaponWithUsageIndex(currentUsageIndex).SwingDamageType, effectiveSkillWithDR, skillModifier, StrikeType.Swing, targetWeapon.Item.Weight);
							float weaponDamageFactor = (float)Math.Sqrt((double)Utilities.getSwingDamageFactor(targetWeapon.Item.GetWeaponWithUsageIndex(currentUsageIndex), targetWeapon.ItemModifier));
							float num5;
							float num6;
							return (float)MathF.Floor((float)MBMath.ClampInt(MathF.Floor(Utilities.RBMComputeDamage(targetWeapon.Item.GetWeaponWithUsageIndex(currentUsageIndex).WeaponClass.ToString(), targetWeapon.Item.GetWeaponWithUsageIndex(currentUsageIndex).SwingDamageType, skillBasedDamage, num, 1f, out num5, out num6, weaponDamageFactor, null, false)), 0, 2000) * 1f);
						}
						float skillBasedDamage2 = Utilities.GetSkillBasedDamage(PostureLogic.CreateMeleeBlowPatch.CalculateThrustMagnitude(character, targetWeapon, currentUsageIndex, skillValue), false, targetWeapon.Item.GetWeaponWithUsageIndex(currentUsageIndex).WeaponClass.ToString(), targetWeapon.Item.GetWeaponWithUsageIndex(currentUsageIndex).ThrustDamageType, effectiveSkillWithDR, skillModifier, StrikeType.Thrust, targetWeapon.Item.Weight);
						float weaponDamageFactor2 = (float)Math.Sqrt((double)Utilities.getThrustDamageFactor(targetWeapon.Item.GetWeaponWithUsageIndex(currentUsageIndex), targetWeapon.ItemModifier));
						float num7;
						float num8;
						return (float)MathF.Floor((float)MBMath.ClampInt(MathF.Floor(Utilities.RBMComputeDamage(targetWeapon.Item.GetWeaponWithUsageIndex(currentUsageIndex).WeaponClass.ToString(), targetWeapon.Item.GetWeaponWithUsageIndex(currentUsageIndex).ThrustDamageType, skillBasedDamage2, num, 1f, out num7, out num8, weaponDamageFactor2, null, false)), 0, 2000) * 1f);
					}
				}
				int num9;
				if (b.StrikeType == StrikeType.Swing)
				{
					num9 = targetWeapon.GetModifiedSwingDamageForCurrentUsage();
				}
				else
				{
					num9 = targetWeapon.GetModifiedThrustDamageForCurrentUsage();
				}
				return (float)MBMath.ClampInt(MathF.Ceiling(MissionGameModels.Current.StrikeMagnitudeModel.ComputeRawDamage(b.DamageType, (float)num9, num, 1f)), 0, 2000);
			*/
			}

			// Token: 0x0600021E RID: 542
			private static float calculateDefenderPostureDamage(Agent defenderAgent, Agent attackerAgent, float absoluteDamageModifier, float actionTypeDamageModifier, ref AttackCollisionData collisionData, MissionWeapon weapon, float comHitModifier)
			{
				float num = 500f;
				float num2 = 500f;
				float num3 = 20f;
				SkillObject relevantSkillFromWeaponClass = WeaponComponentData.GetRelevantSkillFromWeaponClass(weapon.CurrentUsageItem.WeaponClass);
				float num4 = 0f;
				if (relevantSkillFromWeaponClass != null)
				{
					num4 = (float)MissionGameModels.Current.AgentStatCalculateModel.GetEffectiveSkill(attackerAgent, relevantSkillFromWeaponClass);
				}
				float num5;
				if (attackerAgent.HasMount)
				{
					num5 = (float)MissionGameModels.Current.AgentStatCalculateModel.GetEffectiveSkill(attackerAgent, DefaultSkills.Riding);
				}
				else
				{
					num5 = (float)MissionGameModels.Current.AgentStatCalculateModel.GetEffectiveSkill(attackerAgent, DefaultSkills.Athletics);
				}
				float num6 = 0f;
				float defenderWeaponLength = -1f;
				float attackerWeaponLength = -1f;
				float attackerWeaponWeight = -1f;
				if (weapon.CurrentUsageItem != null)
				{
					attackerWeaponLength = (float)weapon.CurrentUsageItem.WeaponLength;
					attackerWeaponWeight = weapon.GetWeight();
				}
				if (defenderAgent.GetWieldedItemIndex(Agent.HandIndex.MainHand) != EquipmentIndex.None)
				{
					MissionWeapon missionWeapon = defenderAgent.Equipment[defenderAgent.GetWieldedItemIndex(Agent.HandIndex.MainHand)];
					SkillObject relevantSkillFromWeaponClass2 = WeaponComponentData.GetRelevantSkillFromWeaponClass(missionWeapon.CurrentUsageItem.WeaponClass);
					if (relevantSkillFromWeaponClass2 != null)
					{
						num6 = (float)MissionGameModels.Current.AgentStatCalculateModel.GetEffectiveSkill(defenderAgent, relevantSkillFromWeaponClass2);
					}
					if (defenderAgent.GetWieldedItemIndex(Agent.HandIndex.OffHand) != EquipmentIndex.None)
					{
						if (defenderAgent.Equipment[defenderAgent.GetWieldedItemIndex(Agent.HandIndex.OffHand)].IsShield())
						{
							num6 += 20f;
						}
					}
					else if (missionWeapon.CurrentUsageItem != null)
					{
						defenderWeaponLength = (float)missionWeapon.CurrentUsageItem.WeaponLength;
						missionWeapon.GetWeight();
					}
				}
				float num7;
				if (defenderAgent.HasMount)
				{
					num7 = (float)MissionGameModels.Current.AgentStatCalculateModel.GetEffectiveSkill(defenderAgent, DefaultSkills.Riding);
				}
				else
				{
					num7 = (float)MissionGameModels.Current.AgentStatCalculateModel.GetEffectiveSkill(defenderAgent, DefaultSkills.Athletics);
				}
				num6 /= num2;
				num7 /= num;
				num4 /= num2;
				num5 /= num;
				bool attackBlockedByOneHanded = false;
				if (!collisionData.AttackBlockedWithShield && defenderAgent.GetWieldedItemIndex(Agent.HandIndex.MainHand) != EquipmentIndex.None)
				{
					WeaponClass weaponClass = defenderAgent.Equipment[defenderAgent.GetWieldedItemIndex(Agent.HandIndex.MainHand)].CurrentUsageItem.WeaponClass;
					if (weaponClass == WeaponClass.OneHandedAxe || weaponClass == WeaponClass.OneHandedPolearm || weaponClass == WeaponClass.OneHandedSword || weaponClass == WeaponClass.Mace)
					{
						attackBlockedByOneHanded = true;
					}
				}
				float num8 = PostureLogic.CreateMeleeBlowPatch.calculateDefenderLWRPostureModifier(attackerAgent, defenderAgent, attackerWeaponLength, defenderWeaponLength, attackerWeaponWeight, attackBlockedByOneHanded, collisionData.AttackBlockedWithShield);
				num3 = num3 * ((1f + num5 + num4) / (1f + num7 + num6)) * num8;
				float num9 = 1f;
				WeaponClass weaponClass2 = WeaponClass.Undefined;
				if (defenderAgent.GetWieldedItemIndex(Agent.HandIndex.OffHand) != EquipmentIndex.None)
				{
					if (defenderAgent.Equipment[defenderAgent.GetWieldedItemIndex(Agent.HandIndex.OffHand)].IsShield())
					{
						weaponClass2 = defenderAgent.Equipment[defenderAgent.GetWieldedItemIndex(Agent.HandIndex.OffHand)].CurrentUsageItem.WeaponClass;
					}
				}
				else if (defenderAgent.GetWieldedItemIndex(Agent.HandIndex.MainHand) != EquipmentIndex.None)
				{
					weaponClass2 = defenderAgent.Equipment[defenderAgent.GetWieldedItemIndex(Agent.HandIndex.MainHand)].CurrentUsageItem.WeaponClass;
				}
				WeaponClass weaponClass3 = weaponClass2;
				float num10;
				switch (weaponClass3)
				{
				case WeaponClass.Dagger:
				case WeaponClass.OneHandedSword:
					num10 = 0.85f;
					goto IL_351;
				case WeaponClass.TwoHandedSword:
					num10 = 0.75f;
					goto IL_351;
				case WeaponClass.OneHandedAxe:
				case WeaponClass.OneHandedPolearm:
					break;
				case WeaponClass.TwoHandedAxe:
				case WeaponClass.TwoHandedMace:
				case WeaponClass.TwoHandedPolearm:
					num10 = 0.9f;
					goto IL_351;
				case WeaponClass.Mace:
				case WeaponClass.Pick:
					num10 = 1.15f;
					goto IL_351;
				default:
					if (weaponClass3 - WeaponClass.SmallShield <= 1)
					{
						num10 = 0.8f;
						goto IL_351;
					}
					break;
				}
				num10 = 1f;
				IL_351:
				actionTypeDamageModifier += actionTypeDamageModifier * 0.5f * comHitModifier;
				return num3 * actionTypeDamageModifier * num10 * num9;
			}

			// Token: 0x0600021F RID: 543
			private static float calculateAttackerPostureDamage(Agent defenderAgent, Agent attackerAgent, float absoluteDamageModifier, float actionTypeDamageModifier, ref AttackCollisionData collisionData, MissionWeapon weapon, float comHitModifier)
			{
				float num = 500f;
				float num2 = 500f;
				float num3 = 20f;
				SkillObject relevantSkillFromWeaponClass = WeaponComponentData.GetRelevantSkillFromWeaponClass(weapon.CurrentUsageItem.WeaponClass);
				float num4 = 0f;
				if (relevantSkillFromWeaponClass != null)
				{
					num4 = (float)MissionGameModels.Current.AgentStatCalculateModel.GetEffectiveSkill(attackerAgent, relevantSkillFromWeaponClass);
				}
				float num5;
				if (attackerAgent.HasMount)
				{
					num5 = (float)MissionGameModels.Current.AgentStatCalculateModel.GetEffectiveSkill(attackerAgent, DefaultSkills.Riding);
				}
				else
				{
					num5 = (float)MissionGameModels.Current.AgentStatCalculateModel.GetEffectiveSkill(attackerAgent, DefaultSkills.Athletics);
				}
				float num6 = 0f;
				float defenderWeaponLength = -1f;
				float attackerWeaponLength = -1f;
				float attackerWeaponWeight = -1f;
				float defenderWeaponWeight = -1f;
				if (weapon.CurrentUsageItem != null)
				{
					attackerWeaponLength = (float)weapon.CurrentUsageItem.WeaponLength;
					attackerWeaponWeight = weapon.GetWeight();
				}
				if (defenderAgent.GetWieldedItemIndex(Agent.HandIndex.MainHand) != EquipmentIndex.None)
				{
					MissionWeapon missionWeapon = defenderAgent.Equipment[defenderAgent.GetWieldedItemIndex(Agent.HandIndex.MainHand)];
					SkillObject relevantSkillFromWeaponClass2 = WeaponComponentData.GetRelevantSkillFromWeaponClass(missionWeapon.CurrentUsageItem.WeaponClass);
					if (relevantSkillFromWeaponClass2 != null)
					{
						num6 = (float)MissionGameModels.Current.AgentStatCalculateModel.GetEffectiveSkill(defenderAgent, relevantSkillFromWeaponClass2);
					}
					if (defenderAgent.GetWieldedItemIndex(Agent.HandIndex.OffHand) != EquipmentIndex.None)
					{
						if (defenderAgent.Equipment[defenderAgent.GetWieldedItemIndex(Agent.HandIndex.OffHand)].IsShield())
						{
							num6 += 20f;
						}
						else if (missionWeapon.CurrentUsageItem != null)
						{
							defenderWeaponLength = (float)missionWeapon.CurrentUsageItem.WeaponLength;
							defenderWeaponWeight = missionWeapon.GetWeight();
						}
					}
				}
				float num7;
				if (defenderAgent.HasMount)
				{
					num7 = (float)MissionGameModels.Current.AgentStatCalculateModel.GetEffectiveSkill(defenderAgent, DefaultSkills.Riding);
				}
				else
				{
					num7 = (float)MissionGameModels.Current.AgentStatCalculateModel.GetEffectiveSkill(defenderAgent, DefaultSkills.Athletics);
				}
				num6 /= num2;
				num7 /= num;
				num4 /= num2;
				num5 /= num;
				bool attackBlockedByOneHanded = false;
				if (!collisionData.AttackBlockedWithShield && defenderAgent.GetWieldedItemIndex(Agent.HandIndex.MainHand) != EquipmentIndex.None)
				{
					WeaponClass weaponClass = defenderAgent.Equipment[defenderAgent.GetWieldedItemIndex(Agent.HandIndex.MainHand)].CurrentUsageItem.WeaponClass;
					if (weaponClass == WeaponClass.OneHandedAxe || weaponClass == WeaponClass.OneHandedPolearm || weaponClass == WeaponClass.OneHandedSword || weaponClass == WeaponClass.Mace)
					{
						attackBlockedByOneHanded = true;
					}
				}
				float num8 = PostureLogic.CreateMeleeBlowPatch.calculateAttackerLWRPostureModifier(attackerAgent, defenderAgent, attackerWeaponLength, defenderWeaponLength, attackerWeaponWeight, defenderWeaponWeight, attackBlockedByOneHanded, collisionData.AttackBlockedWithShield);
				num3 = num3 * ((1f + num7 + num6) / (1f + num5 + num4)) * num8;
				float num9;
				switch (weapon.CurrentUsageItem.WeaponClass)
				{
				case WeaponClass.Dagger:
				case WeaponClass.OneHandedSword:
					num9 = 0.85f;
					goto IL_2C9;
				case WeaponClass.TwoHandedSword:
					num9 = 0.75f;
					goto IL_2C9;
				case WeaponClass.TwoHandedAxe:
				case WeaponClass.TwoHandedMace:
				case WeaponClass.TwoHandedPolearm:
					num9 = 1f;
					goto IL_2C9;
				case WeaponClass.Mace:
				case WeaponClass.Pick:
					num9 = 1.15f;
					goto IL_2C9;
				}
				num9 = 1f;
				IL_2C9:
				actionTypeDamageModifier += actionTypeDamageModifier * 0.5f * comHitModifier;
				return num3 * actionTypeDamageModifier * num9;
			}

			// Token: 0x06000220 RID: 544
			public static float calculateDefenderLWRPostureModifier(Agent attackerAgent, Agent defenderAgent, float attackerWeaponLength, float defenderWeaponLength, float attackerWeaponWeight, bool attackBlockedByOneHanded, bool attackBlockedByShield)
			{
				float num = (defenderAgent.Velocity - attackerAgent.Velocity).Length * PostureLogic.relativeSpeedPostureFactor;
				float result;
				if (attackBlockedByShield)
				{
					result = 1f + (attackerWeaponWeight / 2f + num) / 4f;
				}
				else if (attackBlockedByOneHanded)
				{
					result = 1f + (attackerWeaponWeight / 2f + num) / 2f;
				}
				else
				{
					result = 1f + (attackerWeaponWeight / 2f + num) / 3f;
				}
				return result;
			}

			// Token: 0x06000221 RID: 545
			public static float calculateAttackerLWRPostureModifier(Agent attackerAgent, Agent defenderAgent, float attackerWeaponLength, float defenderWeaponLength, float attackerWeaponWeight, float defenderWeaponWeight, bool attackBlockedByOneHanded, bool attackBlockedByShield)
			{
				float num = (defenderAgent.Velocity - attackerAgent.Velocity).Length * PostureLogic.relativeSpeedPostureFactor;
				float result;
				if (attackBlockedByShield)
				{
					result = 1f + (attackerWeaponWeight / 2f + num) / 2f;
				}
				else if (attackBlockedByOneHanded)
				{
					result = 1f + (attackerWeaponWeight / 2f + num) / 4f;
				}
				else
				{
					result = 1f + (attackerWeaponWeight / 2f + num) / 3f;
				}
				return result;
			}

			// Token: 0x06000222 RID: 546
			private static void makePostureRiposteBlow(ref Mission mission, Blow blow, Agent attackerAgent, Agent victimAgent, ref AttackCollisionData collisionData, in MissionWeapon attackerWeapon, CrushThroughState crushThroughState, Vec3 blowDirection, Vec3 swingDirection, bool cancelDamage, BlowFlags addedBlowFlag)
			{
				Blow blow2 = blow;
				blow2.BaseMagnitude = collisionData.BaseMagnitude;
				blow2.MovementSpeedDamageModifier = collisionData.MovementSpeedDamageModifier;
				blow2.InflictedDamage = 1;
				blow2.SelfInflictedDamage = collisionData.SelfInflictedDamage;
				blow2.AbsorbedByArmor = (float)collisionData.AbsorbedByArmor;
				MissionWeapon missionWeapon = attackerWeapon;
				sbyte b;
				if (!missionWeapon.IsEmpty)
				{
					Monster monster = attackerAgent.Monster;
					missionWeapon = attackerWeapon;
					b = monster.GetBoneToAttachForItemFlags(missionWeapon.Item.ItemFlags);
				}
				else
				{
					b = -1;
				}
				sbyte weaponAttachBoneIndex = b;
				missionWeapon = attackerWeapon;
				ItemObject item = missionWeapon.Item;
				missionWeapon = attackerWeapon;
				blow2.WeaponRecord.FillAsMeleeBlow(item, missionWeapon.CurrentUsageItem, collisionData.AffectorWeaponSlotOrMissileIndex, weaponAttachBoneIndex);
				blow2.StrikeType = (StrikeType)collisionData.StrikeType;
				missionWeapon = attackerWeapon;
				blow2.DamageType = (DamageTypes)((!missionWeapon.IsEmpty && !collisionData.IsAlternativeAttack) ? collisionData.DamageType : 2);
				blow2.NoIgnore = collisionData.IsAlternativeAttack;
				blow2.AttackerStunPeriod = collisionData.AttackerStunPeriod;
				blow2.DefenderStunPeriod = collisionData.DefenderStunPeriod;
				blow2.BlowFlag = BlowFlags.None;
				blow2.GlobalPosition = collisionData.CollisionGlobalPosition;
				blow2.BoneIndex = collisionData.CollisionBoneIndex;
				blow2.Direction = blowDirection;
				blow2.SwingDirection = swingDirection;
				blow2.VictimBodyPart = collisionData.VictimHitBodyPart;
				blow2.BlowFlag |= addedBlowFlag;
				attackerAgent.RegisterBlow(blow2, collisionData);
				foreach (MissionBehavior missionBehavior in mission.MissionBehaviors)
				{
					missionBehavior.OnRegisterBlow(victimAgent, attackerAgent, null, blow2, ref collisionData, attackerWeapon);
				}
			}

			// Token: 0x06000223 RID: 547
			private static void makePostureBlow(ref Mission mission, Blow blow, Agent attackerAgent, Agent victimAgent, ref AttackCollisionData collisionData, in MissionWeapon attackerWeapon, CrushThroughState crushThroughState, Vec3 blowDirection, Vec3 swingDirection, bool cancelDamage, BlowFlags addedBlowFlag)
			{
				Blow blow2 = blow;
				blow2.BaseMagnitude = collisionData.BaseMagnitude;
				blow2.MovementSpeedDamageModifier = collisionData.MovementSpeedDamageModifier;
				blow2.InflictedDamage = 1;
				blow2.SelfInflictedDamage = collisionData.SelfInflictedDamage;
				blow2.AbsorbedByArmor = (float)collisionData.AbsorbedByArmor;
				MissionWeapon missionWeapon = attackerWeapon;
				sbyte b;
				if (!missionWeapon.IsEmpty)
				{
					Monster monster = attackerAgent.Monster;
					missionWeapon = attackerWeapon;
					b = monster.GetBoneToAttachForItemFlags(missionWeapon.Item.ItemFlags);
				}
				else
				{
					b = -1;
				}
				sbyte weaponAttachBoneIndex = b;
				missionWeapon = attackerWeapon;
				ItemObject item = missionWeapon.Item;
				missionWeapon = attackerWeapon;
				blow2.WeaponRecord.FillAsMeleeBlow(item, missionWeapon.CurrentUsageItem, collisionData.AffectorWeaponSlotOrMissileIndex, weaponAttachBoneIndex);
				blow2.StrikeType = (StrikeType)collisionData.StrikeType;
				missionWeapon = attackerWeapon;
				blow2.DamageType = (DamageTypes)((!missionWeapon.IsEmpty && !collisionData.IsAlternativeAttack) ? collisionData.DamageType : 2);
				blow2.NoIgnore = collisionData.IsAlternativeAttack;
				blow2.AttackerStunPeriod = collisionData.AttackerStunPeriod;
				blow2.DefenderStunPeriod = collisionData.DefenderStunPeriod;
				blow2.BlowFlag = BlowFlags.None;
				blow2.GlobalPosition = collisionData.CollisionGlobalPosition;
				blow2.BoneIndex = collisionData.CollisionBoneIndex;
				blow2.Direction = blowDirection;
				blow2.SwingDirection = swingDirection;
				blow2.VictimBodyPart = collisionData.VictimHitBodyPart;
				blow2.BlowFlag |= addedBlowFlag;
				victimAgent.RegisterBlow(blow2, collisionData);
				foreach (MissionBehavior missionBehavior in mission.MissionBehaviors)
				{
					missionBehavior.OnRegisterBlow(attackerAgent, victimAgent, null, blow2, ref collisionData, attackerWeapon);
				}
			}

			// Token: 0x06000224 RID: 548
			private static void makePostureCrashThroughBlow(ref Mission mission, Blow blow, Agent attackerAgent, Agent victimAgent, int inflictedHpDmg, ref AttackCollisionData collisionData, in MissionWeapon attackerWeapon, CrushThroughState crushThroughState, Vec3 blowDirection, Vec3 swingDirection, bool cancelDamage, BlowFlags addedBlowFlag)
			{
				Blow blow2 = blow;
				blow2.BaseMagnitude = collisionData.BaseMagnitude;
				blow2.MovementSpeedDamageModifier = collisionData.MovementSpeedDamageModifier;
				blow2.InflictedDamage = inflictedHpDmg;
				blow2.SelfInflictedDamage = collisionData.SelfInflictedDamage;
				blow2.AbsorbedByArmor = (float)collisionData.AbsorbedByArmor;
				MissionWeapon missionWeapon = attackerWeapon;
				sbyte b;
				if (!missionWeapon.IsEmpty)
				{
					Monster monster = attackerAgent.Monster;
					missionWeapon = attackerWeapon;
					b = monster.GetBoneToAttachForItemFlags(missionWeapon.Item.ItemFlags);
				}
				else
				{
					b = -1;
				}
				sbyte weaponAttachBoneIndex = b;
				missionWeapon = attackerWeapon;
				ItemObject item = missionWeapon.Item;
				missionWeapon = attackerWeapon;
				blow2.WeaponRecord.FillAsMeleeBlow(item, missionWeapon.CurrentUsageItem, collisionData.AffectorWeaponSlotOrMissileIndex, weaponAttachBoneIndex);
				blow2.StrikeType = (StrikeType)collisionData.StrikeType;
				missionWeapon = attackerWeapon;
				blow2.DamageType = (DamageTypes)((!missionWeapon.IsEmpty && !collisionData.IsAlternativeAttack) ? collisionData.DamageType : 2);
				blow2.NoIgnore = collisionData.IsAlternativeAttack;
				blow2.AttackerStunPeriod = collisionData.AttackerStunPeriod / 5f;
				blow2.DefenderStunPeriod = collisionData.DefenderStunPeriod * 5f;
				blow2.BlowFlag = BlowFlags.None;
				blow2.GlobalPosition = collisionData.CollisionGlobalPosition;
				blow2.BoneIndex = collisionData.CollisionBoneIndex;
				blow2.Direction = blowDirection;
				blow2.SwingDirection = swingDirection;
				blow2.VictimBodyPart = collisionData.VictimHitBodyPart;
				blow2.BlowFlag |= addedBlowFlag;
				victimAgent.RegisterBlow(blow2, collisionData);
				foreach (MissionBehavior missionBehavior in mission.MissionBehaviors)
				{
					missionBehavior.OnRegisterBlow(attackerAgent, victimAgent, null, blow2, ref collisionData, attackerWeapon);
				}
			}

			// Token: 0x06000225 RID: 549
			public static float calculateRangedPostureLoss(float fixedPS, float dynamicPS, Agent shooterAgent, WeaponClass wc)
			{
				SkillObject relevantSkillFromWeaponClass = WeaponComponentData.GetRelevantSkillFromWeaponClass(wc);
				float num = 0f;
				if (relevantSkillFromWeaponClass != null)
				{
					num = (float)MissionGameModels.Current.AgentStatCalculateModel.GetEffectiveSkill(shooterAgent, relevantSkillFromWeaponClass);
				}
				float num2;
				if (shooterAgent.HasMount)
				{
					num2 = (float)MissionGameModels.Current.AgentStatCalculateModel.GetEffectiveSkill(shooterAgent, DefaultSkills.Riding);
				}
				else
				{
					num2 = (float)MissionGameModels.Current.AgentStatCalculateModel.GetEffectiveSkill(shooterAgent, DefaultSkills.Athletics);
				}
				float num3 = dynamicPS - MBMath.Lerp(0f, 1f, 1f - num / 200f, 1E-05f) * (dynamicPS * 0.5f);
				num3 -= MBMath.Lerp(0f, 1f, 1f - num2 / 200f, 1E-05f) * (dynamicPS * 0.5f);
				return fixedPS + num3;
			}

			// Token: 0x020000A4 RID: 164
			[HarmonyPatch(typeof(Mission))]
			[HarmonyPatch("OnAgentShootMissile")]
			[UsedImplicitly]
			[MBCallback]
			private class OverrideOnAgentShootMissile
			{
				// Token: 0x060002BD RID: 701
				private static void Postfix(ref Agent shooterAgent, EquipmentIndex weaponIndex, Vec3 position, Vec3 velocity, Mat3 orientation, bool hasRigidBody, bool isPrimaryWeaponShot, int forcedMissileIndex, Mission __instance)
				{
					if (true)
					{
						WeaponClass weaponClass = shooterAgent.Equipment[weaponIndex].CurrentUsageItem.WeaponClass;
						Posture posture = null;
						AgentPostures.values.TryGetValue(shooterAgent, out posture);
						if (posture != null)
						{
							float currentTime = Mission.Current.CurrentTime;
							switch (weaponClass)
							{
							case WeaponClass.Bow:
								posture.posture -= PostureLogic.CreateMeleeBlowPatch.calculateRangedPostureLoss(35f, 25f, shooterAgent, weaponClass);
								break;
							case WeaponClass.Crossbow:
								posture.posture -= PostureLogic.CreateMeleeBlowPatch.calculateRangedPostureLoss(5f, 5f, shooterAgent, weaponClass);
								break;
							case WeaponClass.ThrowingAxe:
							case WeaponClass.ThrowingKnife:
								posture.posture -= PostureLogic.CreateMeleeBlowPatch.calculateRangedPostureLoss(25f, 25f, shooterAgent, weaponClass);
								break;
							case WeaponClass.Javelin:
								posture.posture -= PostureLogic.CreateMeleeBlowPatch.calculateRangedPostureLoss(25f, 25f, shooterAgent, weaponClass);
								break;
							}
							if (posture.posture < 0f)
							{
								float postureResetModifier = 0.5f;
								PostureLogic.CreateMeleeBlowPatch.ResetPostureForAgent(ref posture, postureResetModifier, shooterAgent);
							}
						}
					}
				}
			}
		}

		// Token: 0x02000068 RID: 104
		[HarmonyPatch(typeof(Agent))]
		[HarmonyPatch("OnShieldDamaged")]
		private class OnShieldDamagedPatch
		{
			// Token: 0x06000227 RID: 551
			private static bool Prefix(ref Agent __instance, ref EquipmentIndex slotIndex, ref int inflictedDamage)
			{
				int num = MathF.Max(0, (int)__instance.Equipment[slotIndex].HitPoints - inflictedDamage);
				__instance.ChangeWeaponHitPoints(slotIndex, (short)num);
				if (num == 0)
				{
					__instance.RemoveEquippedWeapon(slotIndex);
				}
				return false;
			}
		}

		// Token: 0x02000069 RID: 105
		[HarmonyPatch(typeof(TournamentRound))]
		[HarmonyPatch("EndMatch")]
		private class EndMatchPatch
		{
			// Token: 0x06000229 RID: 553
			private static void Postfix(ref TournamentRound __instance)
			{
				foreach (KeyValuePair<Agent, Posture> keyValuePair in AgentPostures.values)
				{
					keyValuePair.Value.posture = keyValuePair.Value.maxPosture;
					if (true)
					{
						if (keyValuePair.Key.IsPlayerControlled && (AgentPostures.postureVisual != null && AgentPostures.postureVisual._dataSource.ShowPlayerPostureStatus))
						{
							AgentPostures.postureVisual._dataSource.PlayerPosture = (int)keyValuePair.Value.posture;
							AgentPostures.postureVisual._dataSource.PlayerPostureMax = (int)keyValuePair.Value.maxPosture;
						}
						if (AgentPostures.postureVisual != null && AgentPostures.postureVisual._dataSource.ShowEnemyStatus && AgentPostures.postureVisual.affectedAgent == keyValuePair.Key)
						{
							AgentPostures.postureVisual._dataSource.EnemyPosture = (int)keyValuePair.Value.posture;
							AgentPostures.postureVisual._dataSource.EnemyPostureMax = (int)keyValuePair.Value.maxPosture;
						}
					}
				}
				PostureLogic.agentsToDropShield.Clear();
				PostureLogic.agentsToDropWeapon.Clear();
				PostureLogic.agentsToChangeFormation.Clear();
				AgentPostures.values.Clear();
			}
		}

		// Token: 0x0200006A RID: 106
		[HarmonyPatch(typeof(Mission))]
		[HarmonyPatch("OnAgentDismount")]
		public class OnAgentDismountPatch
		{
			// Token: 0x0600022B RID: 555
			private static void Postfix(Agent agent, Mission __instance)
			{
				if (!agent.IsPlayerControlled && agent.Formation != null && Mission.Current != null && Mission.Current.IsFieldBattle && agent.IsActive())
				{
					bool flag2 = agent.Team.GetFormation(FormationClass.Infantry) != null && agent.Team.GetFormation(FormationClass.Infantry).CountOfUnits > 0;
					bool flag3 = agent.Team.GetFormation(FormationClass.Ranged) != null && agent.Team.GetFormation(FormationClass.Ranged).CountOfUnits > 0;
					if (agent.Equipment.HasRangedWeapon(WeaponClass.Arrow) || agent.Equipment.HasRangedWeapon(WeaponClass.Bolt))
					{
						float num = -1f;
						float num2 = -1f;
						if (agent.Formation != null && flag2)
						{
							num = agent.Team.GetFormation(FormationClass.Infantry).QuerySystem.MedianPosition.AsVec2.Distance(agent.Formation.QuerySystem.MedianPosition.AsVec2);
						}
						if (agent.Formation != null && flag3)
						{
							num2 = agent.Team.GetFormation(FormationClass.Ranged).QuerySystem.MedianPosition.AsVec2.Distance(agent.Formation.QuerySystem.MedianPosition.AsVec2);
						}
						if (num2 > 0f && num2 < num)
						{
							if (agent == null || !agent.IsActive())
							{
								return;
							}
							try
							{
								PostureLogic.agentsToChangeFormation[agent] = FormationClass.Ranged;
								return;
							}
							catch (Exception)
							{
								return;
							}
						}
						if (num > 0f && num < num2)
						{
							if (agent == null || !agent.IsActive())
							{
								return;
							}
							try
							{
								PostureLogic.agentsToChangeFormation[agent] = FormationClass.Infantry;
								return;
							}
							catch (Exception)
							{
								return;
							}
						}
						if (num > 0f)
						{
							if (agent == null || !agent.IsActive())
							{
								return;
							}
							try
							{
								PostureLogic.agentsToChangeFormation[agent] = FormationClass.Infantry;
								return;
							}
							catch (Exception)
							{
								return;
							}
						}
						if (num2 <= 0f || (agent == null || !agent.IsActive()))
						{
							return;
						}
						try
						{
							PostureLogic.agentsToChangeFormation[agent] = FormationClass.Ranged;
							return;
						}
						catch (Exception)
						{
							return;
						}
					}
					if (agent.Formation != null && flag2 && (agent != null && agent.IsActive()))
					{
						try
						{
							PostureLogic.agentsToChangeFormation[agent] = FormationClass.Infantry;
						}
						catch (Exception)
						{
						}
					}
				}
			}
		}

		// Token: 0x0200006B RID: 107
		[HarmonyPatch(typeof(Mission))]
		[HarmonyPatch("OnAgentMount")]
		internal class OnAgentMountPatch
		{
			// Token: 0x0600022D RID: 557
			private static void Postfix(Agent agent, Mission __instance)
			{
				if (!agent.IsPlayerControlled && agent.Formation != null && Mission.Current != null && Mission.Current.IsFieldBattle && agent.IsActive())
				{
					bool flag2 = agent.Team.GetFormation(FormationClass.Cavalry) != null && agent.Team.GetFormation(FormationClass.Cavalry).CountOfUnits > 0;
					bool flag3 = agent.Team.GetFormation(FormationClass.HorseArcher) != null && agent.Team.GetFormation(FormationClass.HorseArcher).CountOfUnits > 0;
					if (agent.Equipment.HasRangedWeapon(WeaponClass.Arrow) || agent.Equipment.HasRangedWeapon(WeaponClass.Bolt))
					{
						if (agent.Formation == null || !flag3 || !agent.IsActive())
						{
							return;
						}
						try
						{
							PostureLogic.agentsToChangeFormation[agent] = FormationClass.HorseArcher;
							return;
						}
						catch (Exception)
						{
							return;
						}
					}
					if (agent.Formation != null && flag2 && agent.IsActive())
					{
						try
						{
							PostureLogic.agentsToChangeFormation[agent] = FormationClass.Cavalry;
						}
						catch (Exception)
						{
						}
					}
				}
			}
		}
	}
}
