using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.ObjectSystem;
using TaleWorlds.ScreenSystem;

namespace Bodyguards
{
	// Token: 0x02000004 RID: 4
	public class AddBodyguardsMissionBehavior : MissionLogic
	{
		// Token: 0x0600002E RID: 46 RVA: 0x0000237C File Offset: 0x0000057C
		public AddBodyguardsMissionBehavior()
		{
			this._settings = new BodyguardsSettings();
		}

		// Token: 0x0600002F RID: 47 RVA: 0x000023F4 File Offset: 0x000005F4
		private void CreatePlayerBodyguards()
		{
			Team playerTeam = this.GetPlayerTeam();
			bool flag = playerTeam == null || playerTeam.TeamAgents == null || base.Mission == null || base.Mission.CombatType != Mission.MissionCombatType.Combat || base.Mission.MainAgent == null;
			if (!flag)
			{
				bool flag2 = base.Mission.IsSiegeBattle && !this._settings.enableBodyguardsDuringSieges;
				if (!flag2)
				{
					bool flag3 = !playerTeam.IsPlayerGeneral;
					if (!flag3)
					{
						IEnumerable<Agent> enumerable = this.FilterAgents(playerTeam.TeamAgents.ToList<Agent>().AsEnumerable<Agent>());
						int num = MathF.Min((int)((float)enumerable.Count<Agent>() * this._settings.maxBodyguardsPercent), this._settings.maxBodyguards);
						FormationClass desiredTroopFormationClass = this._settings.getDesiredTroopFormationClass();
						IEnumerable<Agent> enumerable2 = this.SelectTroops(enumerable, desiredTroopFormationClass);
						bool flag4 = enumerable2.Count<Agent>() < num && (!this._settings.companionGuardMode || !this._settings.doNotBackfill);
						if (flag4)
						{
							IEnumerable<Agent> source = from a in enumerable.Except(enumerable2)
							orderby (base.Mission.MainAgent.MountAgent == null) ? (!a.HasMount) : a.HasMount descending, a.CharacterPowerCached descending
							select a;
							enumerable2 = enumerable2.Concat(source.Take(num - enumerable2.Count<Agent>())).ToList<Agent>();
						}
						int num2 = MathF.Min(enumerable2.Count<Agent>(), num);
						bool flag5 = this._bodyguardFormation != null;
						if (flag5)
						{
							this.ReleaseBodyguards(null);
						}
						bool flag6 = false;
						bool flag7 = num2 > 0;
						if (flag7)
						{
							this._bodyguardFormation = playerTeam.GetFormation(FormationClass.Bodyguard);
							bool useControllableFormation = this._settings.useControllableFormation;
							if (useControllableFormation)
							{
								this._bodyguardFormation = this.FindEmptyFormation(playerTeam);
								bool flag8 = this._bodyguardFormation == null;
								if (flag8)
								{
									this._bodyguardFormation = playerTeam.GetFormation(FormationClass.Bodyguard);
									InformationManager.DisplayMessage(new InformationMessage("No empty formations found, defaulting to Bodyguard formation.", Colors.Green));
								}
								bool flag9 = this._bodyguardFormation.CountOfUnits == 1 && this._bodyguardFormation.GetFirstUnit() == this._bodyguardFormation.Captain && this._bodyguardFormation.Captain != base.Mission.MainAgent;
								if (flag9)
								{
									List<Agent> list = enumerable2.ToList<Agent>();
									list.Remove(this._bodyguardFormation.Captain);
									enumerable2 = list.AsEnumerable<Agent>();
									num2--;
									flag6 = true;
								}
							}
							this._bodyguardFormation.SetMovementOrder(MovementOrder.MovementOrderMove(base.Mission.MainAgent.GetWorldPosition()));
							this._bodyguardFormation.SetControlledByAI(true, false);
							bool companionGuardMode = this._settings.companionGuardMode;
							List<Agent> list2;
							if (companionGuardMode)
							{
								list2 = enumerable2.OrderByDescending(delegate(Agent a)
								{
									BasicCharacterObject character = a.Character;
									return (character != null) ? new bool?(character.IsHero) : null;
								}).ThenByDescending((Agent a) => this.CheckSpecificTroopName(a)).ThenByDescending((Agent a) => a.CharacterPowerCached).ThenByDescending((Agent a) => a.Character.MaxHitPoints()).Take(num2).ToList<Agent>();
							}
							else
							{
								list2 = (from a in enumerable2
								orderby this.CheckSpecificTroopName(a) descending, a.CharacterPowerCached descending
								select a).Take(num2).ToList<Agent>();
							}
							bool flag10 = flag6;
							if (flag10)
							{
								list2.Add(this._bodyguardFormation.Captain);
							}
							this.TransferUnits(list2, this._bodyguardFormation, false);
							TacticComponent.SetDefaultBehaviorWeights(this._bodyguardFormation);
							BehaviorProtectVIPAgent behavior = this._bodyguardFormation.AI.GetBehavior<BehaviorProtectVIPAgent>();
							bool flag11 = behavior == null;
							if (flag11)
							{
								this._bodyguardFormation.AI.AddAiBehavior(new BehaviorProtectVIPAgent(this._bodyguardFormation));
								behavior = this._bodyguardFormation.AI.GetBehavior<BehaviorProtectVIPAgent>();
							}
							behavior.ResetBehavior();
							behavior.VIP = base.Mission.MainAgent;
							this._bodyguardFormation.AI.SetBehaviorWeight<BehaviorProtectVIPAgent>(100f);
							bool flag12 = this._bodyguardFormation.QuerySystem.MainClass == FormationClass.Bodyguard;
							if (flag12)
							{
								playerTeam.BodyGuardFormation = this._bodyguardFormation;
							}
						}
					}
				}
			}
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00002880 File Offset: 0x00000A80
		private void CreateAIBodyguards(Team team)
		{
			bool flag = team == null || team.TeamAgents == null || base.Mission == null || base.Mission.CombatType > Mission.MissionCombatType.Combat;
			if (!flag)
			{
				bool flag2 = team.GeneralAgent == null;
				if (!flag2)
				{
					IEnumerable<Agent> source = this.FilterAIAgents(team.TeamAgents.ToList<Agent>().AsEnumerable<Agent>());
					int b = MathF.Min((int)((float)source.Count<Agent>() * this._AIBodyguardPercent), this._maxAIBodyguards);
					IEnumerable<Agent> source2 = from a in source
					orderby (team.GeneralAgent.MountAgent == null) ? (!a.HasMount) : a.HasMount descending, a.CharacterPowerCached descending
					select a;
					int num = MathF.Min(source2.Count<Agent>(), b);
					bool flag3 = num > 0;
					if (flag3)
					{
						Formation formation = team.GetFormation(FormationClass.Bodyguard);
						bool flag4 = formation.CountOfUnits > 0;
						if (flag4)
						{
							this.TransferUnits(formation.GetUnitsWithoutDetachedOnes().ToList<Agent>(), null, true);
						}
						formation.SetMovementOrder(MovementOrder.MovementOrderMove(team.GeneralAgent.GetWorldPosition()));
						formation.SetControlledByAI(true, false);
						List<Agent> units = source2.Take(num).ToList<Agent>();
						this.TransferUnits(units, formation, false);
						TacticComponent.SetDefaultBehaviorWeights(formation);
						BehaviorProtectVIPAgent behavior = formation.AI.GetBehavior<BehaviorProtectVIPAgent>();
						bool flag5 = behavior == null;
						if (flag5)
						{
							formation.AI.AddAiBehavior(new BehaviorProtectVIPAgent(formation));
							behavior = formation.AI.GetBehavior<BehaviorProtectVIPAgent>();
						}
						behavior.ResetBehavior();
						behavior.VIP = team.GeneralAgent;
						formation.AI.SetBehaviorWeight<BehaviorProtectVIPAgent>(100f);
						bool flag6 = formation.QuerySystem.MainClass == FormationClass.Bodyguard;
						if (flag6)
						{
							team.BodyGuardFormation = formation;
						}
					}
				}
			}
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00002A90 File Offset: 0x00000C90
		private IEnumerable<Agent> FilterAIAgents(IEnumerable<Agent> troopList)
		{
			return troopList.Where(delegate(Agent a)
			{
				bool flag = a == null || a.Character == null;
				bool result;
				if (flag)
				{
					result = false;
				}
				else
				{
					bool isHero = a.Character.IsHero;
					if (isHero)
					{
						result = false;
					}
					else
					{
						bool flag2 = a.Team == null || a.Team.GeneralAgent == null || a.Team.GeneralAgent.Equals(a);
						result = !flag2;
					}
				}
				return result;
			});
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00002AC8 File Offset: 0x00000CC8
		private IEnumerable<Agent> FilterAgents(IEnumerable<Agent> troopList)
		{
			return troopList.Where(delegate(Agent a)
			{
				bool flag = a == null || a.Equals(base.Mission.MainAgent);
				bool result;
				if (flag)
				{
					result = false;
				}
				else
				{
					bool flag2 = a.Character == null;
					if (flag2)
					{
						result = false;
					}
					else
					{
						bool flag3 = a.Team == null || a.Team.GeneralAgent == null || !a.Team.GeneralAgent.Equals(base.Mission.MainAgent);
						result = !flag3;
					}
				}
				return result;
			});
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00002AEC File Offset: 0x00000CEC
		private IEnumerable<Agent> SelectTroops(IEnumerable<Agent> troopList, FormationClass specificFormation)
		{
			return troopList.Where(delegate(Agent a)
			{
				bool flag = this._settings.useSpecificTroop && this.CheckSpecificTroopName(a);
				bool result;
				if (flag)
				{
					result = true;
				}
				else
				{
					bool companionGuardMode = this._settings.companionGuardMode;
					if (companionGuardMode)
					{
						bool flag2 = a.IsHero && !this.IsBodyguardHero(a.Character);
						if (flag2)
						{
							bool onlyUseManuallySelectedCompanions = this._settings.onlyUseManuallySelectedCompanions;
							if (onlyUseManuallySelectedCompanions)
							{
								return false;
							}
							bool flag3 = !this.HeroIsInPlayerClan(a.Character.Id);
							if (flag3)
							{
								return false;
							}
							bool flag4 = a.Formation != null && a.Formation.Captain != null && a.Formation.Captain.Equals(a);
							if (flag4)
							{
								return false;
							}
						}
						bool flag5 = this._settings.doNotBackfill && !a.IsHero;
						if (flag5)
						{
							return false;
						}
					}
					else
					{
						bool isHero = a.IsHero;
						if (isHero)
						{
							return false;
						}
					}
					bool flag6 = a.IsHero || a.Character.DefaultFormationClass == specificFormation;
					result = flag6;
				}
				return result;
			});
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00002B24 File Offset: 0x00000D24
		private void TransferUnits(List<Agent> units, Formation newFormation, bool defaultFormations = false)
		{
			bool flag = newFormation == null && !defaultFormations;
			if (!flag)
			{
				IEnumerable<Formation> enumerable = (from u in units
				select u.Formation).Distinct<Formation>();
				foreach (Agent agent in units)
				{
					bool flag2 = agent == null || agent.Team == null || agent.Character == null;
					if (!flag2)
					{
						if (defaultFormations)
						{
							agent.Formation = agent.Team.GetFormation(agent.Character.DefaultFormationClass);
						}
						else
						{
							bool flag3 = agent.Team == newFormation.Team;
							if (flag3)
							{
								agent.Formation = newFormation;
							}
						}
					}
				}
				enumerable = enumerable.Concat((from bu in units
				select bu.Formation).Distinct<Formation>().Except(enumerable));
				foreach (Formation formation in enumerable)
				{
					formation.Team.TriggerOnFormationsChanged(formation);
					formation.QuerySystem.Expire();
				}
			}
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00002CA4 File Offset: 0x00000EA4
		private bool CheckSpecificTroopName(Agent a)
		{
			bool flag = !this._settings.useSpecificTroop;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				BasicCharacterObject character = a.Character;
				string a2 = (((character != null) ? character.Name : null) == null) ? "character name is null" : a.Character.Name.ToString();
				string b = (this._settings.specificTroopName == null) ? "specific troop name is null" : this._settings.specificTroopName;
				bool flag2 = a2 == b;
				result = flag2;
			}
			return result;
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00002D2C File Offset: 0x00000F2C
		private bool IsBodyguardHero(BasicCharacterObject c)
		{
			bool flag = Campaign.Current == null || this._behavior == null || !c.IsHero;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				CharacterObject @object = Campaign.Current.ObjectManager.GetObject<CharacterObject>(c.StringId);
				bool flag2 = @object == null;
				result = (!flag2 && this._behavior.IsBodyguard(@object.HeroObject));
			}
			return result;
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00002D98 File Offset: 0x00000F98
		private Formation FindEmptyFormation(Team team)
		{
			foreach (FormationClass formationClass in this._formationList)
			{
				Formation formation = team.GetFormation(formationClass);
				bool flag = formation == null;
				if (!flag)
				{
					bool flag2 = formation.CountOfUnits == 0;
					if (flag2)
					{
						return formation;
					}
					bool flag3 = formation.CountOfUnits != 1;
					if (!flag3)
					{
						bool flag4 = formation.GetFirstUnit() != formation.Captain;
						if (!flag4)
						{
							bool flag5 = formation.Captain == base.Mission.MainAgent;
							if (!flag5)
							{
								return formation;
							}
							InformationManager.DisplayMessage(new InformationMessage("Assigning the player as the captain of the Bodyguard formation is currently unsupported. You can assign a companion or family member as the captain though. I will look at supporting this in the future, sorry for the let down. :(", Colors.Green));
						}
					}
				}
			}
			return null;
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00002E88 File Offset: 0x00001088
		private bool HeroIsInPlayerClan(MBGUID id)
		{
			Clan playerClan = Clan.PlayerClan;
			bool flag = ((playerClan != null) ? playerClan.Heroes : null) == null;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				foreach (Hero hero in Clan.PlayerClan.Heroes)
				{
					bool flag2 = ((hero != null) ? hero.CharacterObject : null) != null && hero.CharacterObject.Id == id;
					if (flag2)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00002F2C File Offset: 0x0000112C
		private void DisableBodyguardBehavior(Formation guardFormation)
		{
			bool flag = guardFormation == null || guardFormation.AI == null;
			if (!flag)
			{
				BehaviorProtectVIPAgent behavior = guardFormation.AI.GetBehavior<BehaviorProtectVIPAgent>();
				bool flag2 = behavior != null;
				if (flag2)
				{
					behavior.DisableThisBehaviorManually = true;
				}
			}
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00002F70 File Offset: 0x00001170
		private int GetBodyguardCount()
		{
			Team playerTeam = this.GetPlayerTeam();
			bool flag = playerTeam == null || this._bodyguardFormation == null;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				bool flag2 = this._bodyguardFormation.CountOfUnits == 0;
				if (flag2)
				{
					this.DisableBodyguardBehavior(this._bodyguardFormation);
				}
				result = this._bodyguardFormation.CountOfUnits;
			}
			return result;
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00002FD0 File Offset: 0x000011D0
		private Team GetPlayerTeam()
		{
			bool flag = base.Mission.Teams == null;
			Team result;
			if (flag)
			{
				result = null;
			}
			else
			{
				Team team = null;
				foreach (Team team2 in base.Mission.Teams)
				{
					bool flag2 = team2 != null && team2.IsPlayerTeam;
					if (flag2)
					{
						team = team2;
					}
				}
				result = team;
			}
			return result;
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00003060 File Offset: 0x00001260
		private void ReleaseBodyguards(Formation guardFormation = null)
		{
			bool flag = guardFormation == null;
			if (flag)
			{
				guardFormation = this._bodyguardFormation;
				bool flag2 = guardFormation == null;
				if (flag2)
				{
					return;
				}
			}
			IEnumerable<Agent> unitsWithoutDetachedOnes = guardFormation.GetUnitsWithoutDetachedOnes();
			this.DisableBodyguardBehavior(guardFormation);
			bool flag3 = unitsWithoutDetachedOnes.Count<Agent>() == 0;
			if (!flag3)
			{
				InformationManager.DisplayMessage(new InformationMessage("Returning bodyguards to their usual formations.", Colors.Green));
				this.TransferUnits(unitsWithoutDetachedOnes.ToList<Agent>(), null, true);
			}
		}

		// Token: 0x0600003D RID: 61 RVA: 0x000030D0 File Offset: 0x000012D0
		public override void OnMissionTick(float dt)
		{
			InputKey key = InputKey.F1;
			MissionScreen missionScreen = (MissionScreen)ScreenManager.TopScreen;
			bool flag = base.Mission == null;
			if (!flag)
			{
				bool flag2 = base.Mission.MainAgent == null && this.GetBodyguardCount() > 0;
				if (flag2)
				{
					this.ReleaseBodyguards(null);
				}
				bool flag3 = missionScreen.InputManager.IsControlDown() && missionScreen.InputManager.IsKeyPressed(key);
				if (flag3)
				{
					bool flag4 = this.GetBodyguardCount() > 0;
					if (flag4)
					{
						this.ReleaseBodyguards(null);
					}
					else
					{
						this.CreatePlayerBodyguards();
						InformationManager.DisplayMessage(new InformationMessage("Creating a bodyguard detail of " + this.GetBodyguardCount().ToString() + " soldiers.", Colors.Green));
					}
				}
			}
		}

		// Token: 0x0600003E RID: 62 RVA: 0x0000319C File Offset: 0x0000139C
		public override void OnMissionModeChange(MissionMode oldMissionMode, bool atStart)
		{
			base.OnMissionModeChange(oldMissionMode, atStart);
			bool flag = oldMissionMode != MissionMode.Deployment;
			if (!flag)
			{
				bool createBodyguardsAtBattleStart = this._settings.createBodyguardsAtBattleStart;
				if (createBodyguardsAtBattleStart)
				{
					this.CreatePlayerBodyguards();
				}
				bool flag2 = !this._settings.enableBodyguardsForAIGenerals;
				if (!flag2)
				{
					foreach (Team team in base.Mission.Teams)
					{
						bool flag3 = !team.IsPlayerTeam;
						if (flag3)
						{
							this.CreateAIBodyguards(team);
						}
					}
				}
			}
		}

		// Token: 0x0400000D RID: 13
		private readonly BodyguardsSettings _settings;

		// Token: 0x0400000E RID: 14
		private readonly BodyguardsSaveDataBehavior _behavior = (Campaign.Current == null) ? null : Campaign.Current.GetCampaignBehavior<BodyguardsSaveDataBehavior>();

		// Token: 0x0400000F RID: 15
		private readonly List<FormationClass> _formationList = new List<FormationClass>
		{
			FormationClass.LightCavalry,
			FormationClass.HeavyCavalry,
			FormationClass.HeavyInfantry,
			FormationClass.NumberOfDefaultFormations
		};

		// Token: 0x04000010 RID: 16
		private Formation _bodyguardFormation;

		// Token: 0x04000011 RID: 17
		private readonly int _maxAIBodyguards = 5;

		// Token: 0x04000012 RID: 18
		private readonly float _AIBodyguardPercent = 0.5f;
	}
}
