using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.SaveSystem;

namespace SSC.world.escort
{
	// Token: 0x0200001E RID: 30
	public class EscortData
	{
		// Token: 0x0600015C RID: 348 RVA: 0x00007EC0 File Offset: 0x000060C0
		public int calculatePayment(Settlement origin, Settlement destination)
		{
			int num = 0;
			foreach (TroopRosterElement troopRosterElement in MobileParty.MainParty.MemberRoster.GetTroopRoster())
			{
				if (troopRosterElement.Character.IsHero)
				{
					num += troopRosterElement.Character.Level * 10;
				}
				else
				{
					num += troopRosterElement.Character.Tier * 15;
				}
			}
			float num2 = EscortData.getDistance(origin, destination) / 80f;
			num = (int)((float)num * num2);
			if (num > 5000)
			{
				num = 5000;
			}
			return num;
		}

		// Token: 0x0600015D RID: 349 RVA: 0x00007F6C File Offset: 0x0000616C
		public static float getDistance(Settlement origin, Settlement destination)
		{
			return origin.GatePosition.Distance(destination.GatePosition);
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x0600015E RID: 350 RVA: 0x00007F8D File Offset: 0x0000618D
		// (set) Token: 0x0600015F RID: 351 RVA: 0x00007F95 File Offset: 0x00006195
		public bool IsOngoing
		{
			get
			{
				return this.isOngoing;
			}
			set
			{
				this.isOngoing = value;
			}
		}

		// Token: 0x06000160 RID: 352 RVA: 0x00007F9E File Offset: 0x0000619E
		public bool isTracked(ITrackableCampaignObject o)
		{
			return Campaign.Current.VisualTrackerManager.CheckTracked(o);
		}

		// Token: 0x06000161 RID: 353 RVA: 0x00007FB0 File Offset: 0x000061B0
		public void stopTracking(MobileParty party)
		{
			Campaign.Current.VisualTrackerManager.RemoveTrackedObject(party, false);
		}

		// Token: 0x06000162 RID: 354 RVA: 0x00007FC4 File Offset: 0x000061C4
		public void removeBandits()
		{
			if (this.bandits != null)
			{
				if (this.isTracked(this.bandits))
				{
					this.stopTracking(this.bandits);
				}
				if (this.bandits.IsActive)
				{
					this.bandits.Ai.SetDoNotMakeNewDecisions(false);
					this.bandits.IgnoreByOtherPartiesTill(CampaignTime.Now);
				}
				this.bandits = null;
			}
		}

		// Token: 0x06000163 RID: 355 RVA: 0x00008028 File Offset: 0x00006228
		public void finalizeEscortTask()
		{
			Campaign.Current.VisualTrackerManager.RemoveTrackedObject(this.destination, false);
			this.removeBandits();
			if (this.caravan != null)
			{
				DestroyPartyAction.Apply(this.caravan.Party, this.caravan);
			}
			this.caravan = null;
			this.originTown = null;
			this.destination = null;
			this.isOngoing = false;
		}

		// Token: 0x04000071 RID: 113
		[SaveableField(1)]
		public MobileParty caravan;

		// Token: 0x04000072 RID: 114
		[SaveableField(2)]
		private bool isOngoing;

		// Token: 0x04000073 RID: 115
		[SaveableField(3)]
		public Settlement originTown;

		// Token: 0x04000074 RID: 116
		[SaveableField(4)]
		public Settlement destination;

		// Token: 0x04000075 RID: 117
		[SaveableField(5)]
		public MobileParty bandits;

		// Token: 0x04000076 RID: 118
		[SaveableField(6)]
		public bool banditsSpawned;

		// Token: 0x04000077 RID: 119
		[SaveableField(7)]
		public int agreedPayment;

		// Token: 0x04000078 RID: 120
		public const int townCount = 6;

		// Token: 0x04000079 RID: 121
		public List<Settlement> closestTowns;
	}
}
