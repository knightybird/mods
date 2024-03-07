using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.SaveSystem;

namespace SSC.world
{
	// Token: 0x0200000D RID: 13
	internal class HideoutGrowthBehavior : CampaignBehaviorBase
	{
		// Token: 0x060000B1 RID: 177 RVA: 0x00004250 File Offset: 0x00002450
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<int>("first_fight_tiers", ref this.firstPhaseTiers);
			dataStore.SyncData<int>("boss_fight_tiers", ref this.bossPhaseTiers);
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x00004278 File Offset: 0x00002478
		public override void RegisterEvents()
		{
			if (this.firstPhaseTiers == 0)
			{
				this.firstPhaseTiers = 20;
			}
			if (this.bossPhaseTiers == 0)
			{
				this.bossPhaseTiers = 3;
			}
			World.FirstPhaseTiers = new FieldProxy<int>(() => this.firstPhaseTiers, delegate(int value)
			{
				this.firstPhaseTiers = value;
			});
			World.BossPhaseTiers = new FieldProxy<int>(() => this.bossPhaseTiers, delegate(int value)
			{
				this.bossPhaseTiers = value;
			});
			CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.OnMapEventEnded));
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x00004300 File Offset: 0x00002500
		private void OnMapEventEnded(MapEvent mapEvent)
		{
			if (mapEvent.EventType != MapEvent.BattleTypes.Hideout)
			{
				return;
			}
			if (!mapEvent.IsPlayerMapEvent)
			{
				return;
			}
			if (mapEvent.Winner == null)
			{
				return;
			}
			if (mapEvent.Winner.LeaderParty == null)
			{
				return;
			}
			bool diplomaticallyFinished = mapEvent.DiplomaticallyFinished;
			int hideoutPlayerPartyCountAtStart = World.HideoutPlayerPartyCountAtStart;
			if (hideoutPlayerPartyCountAtStart == 0)
			{
				World.FirstPhaseTiers.Value = (int)Math.Round((double)((float)World.FirstPhaseTiers.Value * 1.5f));
				World.BossPhaseTiers.Value = (int)Math.Round((double)((float)World.BossPhaseTiers.Value * 1.5f));
				return;
			}
			int casualties = mapEvent.AttackerSide.Casualties;
			int num = hideoutPlayerPartyCountAtStart - casualties - 2;
			int casualties2 = mapEvent.DefenderSide.Casualties;
			int num2 = World.HideoutTiersAtStart / hideoutPlayerPartyCountAtStart;
			int num3 = num;
			if (num3 > 0)
			{
				int num4 = num3 / 3;
				int num5 = num3 - num4;
				World.FirstPhaseTiers.Value += num5;
				World.BossPhaseTiers.Value += num4;
			}
		}

		// Token: 0x04000036 RID: 54
		[SaveableField(1)]
		private int firstPhaseTiers = 20;

		// Token: 0x04000037 RID: 55
		[SaveableField(2)]
		private int bossPhaseTiers = 3;
	}
}
