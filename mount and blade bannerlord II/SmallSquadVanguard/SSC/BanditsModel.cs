using System;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;

namespace SSC
{
	// Token: 0x0200000C RID: 12
	public class BanditsModel : DefaultBanditDensityModel
	{
		// Token: 0x17000019 RID: 25
		// (get) Token: 0x060000A5 RID: 165 RVA: 0x00004214 File Offset: 0x00002414
		public override int NumberOfMaximumLooterParties
		{
			get
			{
				return 150;
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x060000A6 RID: 166 RVA: 0x0000421B File Offset: 0x0000241B
		public override int NumberOfMinimumBanditPartiesInAHideoutToInfestIt
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x060000A7 RID: 167 RVA: 0x0000421E File Offset: 0x0000241E
		public override int NumberOfMaximumBanditPartiesInEachHideout
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x060000A8 RID: 168 RVA: 0x00004221 File Offset: 0x00002421
		public override int NumberOfMaximumBanditPartiesAroundEachHideout
		{
			get
			{
				return 8;
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x060000A9 RID: 169 RVA: 0x00004224 File Offset: 0x00002424
		public override int NumberOfMaximumHideoutsAtEachBanditFaction
		{
			get
			{
				return 10;
			}
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x060000AA RID: 170 RVA: 0x00004228 File Offset: 0x00002428
		public override int NumberOfInitialHideoutsAtEachBanditFaction
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x060000AB RID: 171 RVA: 0x0000422B File Offset: 0x0000242B
		public override int NumberOfMinimumBanditTroopsInHideoutMission
		{
			get
			{
				return 10;
			}
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x060000AC RID: 172 RVA: 0x0000422F File Offset: 0x0000242F
		public override int NumberOfMaximumTroopCountForFirstFightInHideout
		{
			get
			{
				return World.HideoutFirstFightMaximum;
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x060000AD RID: 173 RVA: 0x00004236 File Offset: 0x00002436
		public override int NumberOfMaximumTroopCountForBossFightInHideout
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x060000AE RID: 174 RVA: 0x00004239 File Offset: 0x00002439
		public override float SpawnPercentageForFirstFightInHideoutMission
		{
			get
			{
				return 0.95f;
			}
		}

		// Token: 0x060000AF RID: 175 RVA: 0x00004240 File Offset: 0x00002440
		public override int GetPlayerMaximumTroopCountForHideoutMission(MobileParty party)
		{
			return World.GetPlayerMaximumTroopCountForHideoutMission(party);
		}
	}
}
