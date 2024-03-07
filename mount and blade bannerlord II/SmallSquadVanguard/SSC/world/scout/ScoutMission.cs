using System;
using SSC.misc;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.SaveSystem;

namespace SSC.world.scout
{
	// Token: 0x02000013 RID: 19
	internal class ScoutMission
	{
		// Token: 0x17000024 RID: 36
		// (get) Token: 0x060000D5 RID: 213 RVA: 0x00004B95 File Offset: 0x00002D95
		public bool IsOngoing
		{
			get
			{
				return this.isOngoing;
			}
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060000D6 RID: 214 RVA: 0x00004B9D File Offset: 0x00002D9D
		public MobileParty MobileParty
		{
			get
			{
				return this.mobileParty;
			}
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060000D7 RID: 215 RVA: 0x00004BA5 File Offset: 0x00002DA5
		public FollowerParty Follower
		{
			get
			{
				if (this.follower == null && this.mobileParty != null)
				{
					this.follower = new FollowerParty(this.mobileParty);
				}
				return this.follower;
			}
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060000D8 RID: 216 RVA: 0x00004BCE File Offset: 0x00002DCE
		public float Reputation
		{
			get
			{
				return this.reputation;
			}
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060000D9 RID: 217 RVA: 0x00004BD6 File Offset: 0x00002DD6
		public Hero Leader
		{
			get
			{
				return this.partyLeader;
			}
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060000DA RID: 218 RVA: 0x00004BDE File Offset: 0x00002DDE
		// (set) Token: 0x060000DB RID: 219 RVA: 0x00004BE6 File Offset: 0x00002DE6
		public Settlement Target
		{
			get
			{
				return this.target;
			}
			set
			{
				this.target = value;
			}
		}

		// Token: 0x060000DC RID: 220 RVA: 0x00004BEF File Offset: 0x00002DEF
		public void initialize()
		{
			World.PlayerStrength = new FieldProxy<int>(() => this.playerStrength, delegate(int value)
			{
				this.playerStrength = value;
			});
			if (this.playerStrength == 0)
			{
				this.playerStrength = 10;
			}
			World.ScoutMission_LedParty = this.MobileParty;
		}

		// Token: 0x060000DD RID: 221 RVA: 0x00004C2E File Offset: 0x00002E2E
		public void reputationChange(float change)
		{
			this.reputation += change;
		}

		// Token: 0x060000DE RID: 222 RVA: 0x00004C40 File Offset: 0x00002E40
		public void setFollowingParty(MobileParty party)
		{
			if (party == null)
			{
				return;
			}
			if (this.mobileParty != null)
			{
				this.abandonMission(Abandonment.Reason.PlayerDecision);
			}
			this.mobileParty = party;
			this.partyLeader = party.LeaderHero;
			this.follower = new FollowerParty(this.mobileParty);
			party.SetPartyScout(MobileParty.MainParty.EffectiveScout);
			this.target = null;
			World.ScoutMission_LedParty = this.MobileParty;
			this.isOngoing = true;
		}

		// Token: 0x060000DF RID: 223 RVA: 0x00004CB0 File Offset: 0x00002EB0
		public void abandonMission(Abandonment.Reason reason)
		{
			if (!this.isOngoing)
			{
				return;
			}
			switch (reason)
			{
			case Abandonment.Reason.Captured:
				Abandonment.LeaderCaptured(this.Leader, this);
				break;
			case Abandonment.Reason.FactionsAtWar:
				Abandonment.FactionsAtWar(this.Leader, this.mobileParty);
				break;
			case Abandonment.Reason.PlayerDecision:
				Abandonment.PlayerDecision(this);
				break;
			case Abandonment.Reason.PlayerCaptured:
				Abandonment.PlayerCaptured(this.Leader);
				break;
			case Abandonment.Reason.ArmyDisbanded:
				Abandonment.ArmyDisbanded(this.Leader, this.mobileParty);
				break;
			case Abandonment.Reason.PartyLeaderKilled:
				Abandonment.PartyLeaderKilled(this.Leader, this);
				break;
			}
			this.Follower.Hold();
			this.mobileParty.Ai.SetDoNotMakeNewDecisions(false);
			this.isOngoing = false;
			this.follower = null;
			World.ScoutMission_LedParty = null;
			this.mobileParty = null;
			this.partyLeader = null;
			this.target = null;
			World.unsetFlag(Flags.ScoutMission_BuyHorsesMode);
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x00004D8B File Offset: 0x00002F8B
		internal void setReputation(float reputation)
		{
			this.reputation = reputation;
		}

		// Token: 0x04000049 RID: 73
		[SaveableField(1)]
		private bool isOngoing;

		// Token: 0x0400004A RID: 74
		[SaveableField(2)]
		private Hero partyLeader;

		// Token: 0x0400004B RID: 75
		[SaveableField(3)]
		private MobileParty mobileParty;

		// Token: 0x0400004C RID: 76
		[SaveableField(4)]
		private float reputation;

		// Token: 0x0400004D RID: 77
		[SaveableField(5)]
		private Settlement target;

		// Token: 0x0400004E RID: 78
		[SaveableField(6)]
		private int playerStrength;

		// Token: 0x0400004F RID: 79
		private FollowerParty follower;
	}
}
