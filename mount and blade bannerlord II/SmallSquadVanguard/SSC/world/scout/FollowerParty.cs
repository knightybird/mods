using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace SSC.world.scout
{
	// Token: 0x02000011 RID: 17
	internal class FollowerParty
	{
		// Token: 0x17000023 RID: 35
		// (get) Token: 0x060000C0 RID: 192 RVA: 0x000045BD File Offset: 0x000027BD
		public bool IsRaiding
		{
			get
			{
				return this.party.DefaultBehavior == AiBehavior.RaidSettlement;
			}
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x000045CD File Offset: 0x000027CD
		public FollowerParty(MobileParty party)
		{
			this.party = party;
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x000045DC File Offset: 0x000027DC
		public string status()
		{
			string text;
			if (this.IsRaiding)
			{
				string str = (this.party.TargetSettlement == null) ? "target unknown" : this.party.TargetSettlement.Name.ToString();
				text = "Raiding: " + str + ".";
			}
			else if (this.party.CurrentSettlement != null)
			{
				text = string.Format("Staying at {0}.", this.party.CurrentSettlement.Name);
			}
			else if (this.party.BesiegedSettlement != null)
			{
				text = string.Format("Besieging {0}.", this.party.BesiegedSettlement.Name);
			}
			else if (this.party.IsMoving)
			{
				text = "Moving.";
			}
			else if (this.party.IsEngaging)
			{
				Settlement targetSettlement = this.party.TargetSettlement;
				if (targetSettlement != null)
				{
					return string.Format("Command: {0}. Engaging {1}.", this.command, targetSettlement.Name);
				}
				MobileParty targetParty = this.party.TargetParty;
				if (this.party != null)
				{
					return string.Format("Command: {0}. Engaging {1}.", this.command, targetParty.Name);
				}
				return string.Format("Command: {0}. Engaging.", this.command);
			}
			else
			{
				text = "Unknown.";
			}
			Hero leaderHero = this.party.LeaderHero;
			int num = (leaderHero != null) ? leaderHero.Gold : 0;
			return string.Format("Command: {0}. {1}. {2} Gold: {3}", new object[]
			{
				this.command,
				this.party.Ai.DefaultBehavior,
				text,
				num
			});
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x00004784 File Offset: 0x00002984
		public void Follow()
		{
			this.disableAi();
			if (this.party.BesiegedSettlement == null)
			{
				if (this.party.CurrentSettlement != null)
				{
					SetPartyAiAction.GetActionForEscortingParty(this.party, MobileParty.MainParty);
				}
				else if (Settlement.CurrentSettlement != null)
				{
					Settlement currentSettlement = Settlement.CurrentSettlement;
					if (currentSettlement.IsVillage && this.isHostileTo(currentSettlement))
					{
						this.Raid(currentSettlement);
					}
				}
				else
				{
					SetPartyAiAction.GetActionForEscortingParty(this.party, MobileParty.MainParty);
				}
			}
			this.command = Command.Follow;
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x00004801 File Offset: 0x00002A01
		public bool isHostileTo(Settlement settlement)
		{
			return settlement.MapFaction != null && settlement.MapFaction.IsAtWarWith(this.party.MapFaction);
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x00004823 File Offset: 0x00002A23
		public bool isHostileTo(MobileParty party)
		{
			return party.MapFaction != null && party.MapFaction.IsAtWarWith(this.party.MapFaction);
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x00004845 File Offset: 0x00002A45
		internal void Raid(Settlement village)
		{
			this.disableAi();
			if (this.party.DefaultBehavior != AiBehavior.RaidSettlement || this.party.TargetSettlement != village)
			{
				SetPartyAiAction.GetActionForRaidingSettlement(this.party, village);
				this.command = Command.Raid;
			}
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x0000487C File Offset: 0x00002A7C
		public void Besiege(Settlement settlement)
		{
			this.disableAi();
			if (this.party.BesiegedSettlement != null && this.party.BesiegedSettlement == settlement)
			{
				return;
			}
			SetPartyAiAction.GetActionForBesiegingSettlement(this.party, settlement);
			this.command = Command.Besiege;
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x000048B8 File Offset: 0x00002AB8
		public void Visit(Settlement currentSettlement)
		{
			this.disableAi();
			if (this.party.CurrentSettlement != null && this.party.CurrentSettlement == currentSettlement)
			{
				return;
			}
			SetPartyAiAction.GetActionForVisitingSettlement(this.party, currentSettlement);
			this.command = Command.Visit;
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x000048EF File Offset: 0x00002AEF
		public void PatrolAround(Settlement currentSettlement)
		{
			this.disableAi();
			SetPartyAiAction.GetActionForPatrollingAroundSettlement(this.party, currentSettlement);
			this.command = Command.PatrolAround;
		}

		// Token: 0x060000CA RID: 202 RVA: 0x0000490A File Offset: 0x00002B0A
		private void disableAi()
		{
			if (!this.party.Ai.DoNotMakeNewDecisions)
			{
				this.party.Ai.SetDoNotMakeNewDecisions(true);
			}
		}

		// Token: 0x060000CB RID: 203 RVA: 0x0000492F File Offset: 0x00002B2F
		internal void RaidIfHostile(Settlement village)
		{
			if (this.isHostileTo(village))
			{
				this.Raid(village);
			}
		}

		// Token: 0x060000CC RID: 204 RVA: 0x00004941 File Offset: 0x00002B41
		internal void Engage(MobileParty targetParty)
		{
			this.disableAi();
			if (this.party.IsEngaging && this.party.TargetParty == targetParty)
			{
				return;
			}
			SetPartyAiAction.GetActionForEngagingParty(this.party, targetParty);
			this.command = Command.Engage;
		}

		// Token: 0x060000CD RID: 205 RVA: 0x00004978 File Offset: 0x00002B78
		internal void Hold()
		{
			this.party.Ai.SetMoveModeHold();
		}

		// Token: 0x060000CE RID: 206 RVA: 0x0000498A File Offset: 0x00002B8A
		public void DecideForYourself()
		{
			this.party.Ai.SetDoNotMakeNewDecisions(false);
		}

		// Token: 0x04000044 RID: 68
		private readonly MobileParty party;

		// Token: 0x04000045 RID: 69
		private Command command;
	}
}
