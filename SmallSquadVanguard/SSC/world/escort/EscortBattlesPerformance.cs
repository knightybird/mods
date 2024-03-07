using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.SaveSystem;

namespace SSC.world.escort
{
	// Token: 0x0200001F RID: 31
	public class EscortBattlesPerformance
	{
		// Token: 0x1700002C RID: 44
		// (get) Token: 0x06000165 RID: 357 RVA: 0x00008093 File Offset: 0x00006293
		public BanditFormation currentFormation
		{
			get
			{
				return this.formations[this.currentFormationIndex];
			}
		}

		// Token: 0x06000166 RID: 358 RVA: 0x000080A6 File Offset: 0x000062A6
		public void newEscortTaskStarted()
		{
			this.mainBattleFinished = false;
		}

		// Token: 0x06000167 RID: 359 RVA: 0x000080AF File Offset: 0x000062AF
		public void setNextFormation()
		{
			this.currentFormationIndex++;
			if (this.currentFormationIndex >= this.formations.Count)
			{
				this.currentFormationIndex = 0;
				this.formations.Shuffle<BanditFormation>();
			}
		}

		// Token: 0x06000168 RID: 360 RVA: 0x000080E4 File Offset: 0x000062E4
		public void battleStarted(MobileParty playerParty, MobileParty bandits)
		{
		}

		// Token: 0x06000169 RID: 361 RVA: 0x000080E6 File Offset: 0x000062E6
		public void battleFinished(MapEvent mapEvent, MobileParty playerParty, MobileParty bandits)
		{
			if (this.mainBattleFinished)
			{
				return;
			}
			if (mapEvent.WinningSide == mapEvent.PlayerSide)
			{
				this.increaseBanditCount(playerParty);
				this.mainBattleFinished = true;
			}
		}

		// Token: 0x0600016A RID: 362 RVA: 0x00008110 File Offset: 0x00006310
		private void increaseBanditCount(MobileParty playerParty)
		{
			int totalHealthyCount = playerParty.MemberRoster.TotalHealthyCount;
			if (totalHealthyCount <= 3)
			{
				return;
			}
			int val = this.currentFormation.upperBound();
			int upgradePoints = Math.Min(totalHealthyCount - 3, val);
			this.currentFormation.grow(upgradePoints, true);
		}

		// Token: 0x0400007A RID: 122
		[SaveableField(1)]
		private int currentFormationIndex;

		// Token: 0x0400007B RID: 123
		[SaveableField(2)]
		public bool mainBattleFinished;

		// Token: 0x0400007C RID: 124
		[SaveableField(3)]
		public List<BanditFormation> formations;

		// Token: 0x0400007D RID: 125
		private const int balanceThreshold = 3;
	}
}
