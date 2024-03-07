using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.SaveSystem;

namespace SSC.world.escort
{
	// Token: 0x02000020 RID: 32
	public class EscortBehavior : CampaignBehaviorBase
	{
		// Token: 0x0600016C RID: 364 RVA: 0x00008159 File Offset: 0x00006359
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<EscortData>("escort", ref this.escort);
			dataStore.SyncData<EscortBattlesPerformance>("performance", ref this.performance);
		}

		// Token: 0x0600016D RID: 365 RVA: 0x00008180 File Offset: 0x00006380
		private void initialize()
		{
			if (this.performance == null)
			{
				this.performance = new EscortBattlesPerformance();
			}
			if (this.performance.formations == null)
			{
				this.performance.formations = BanditFormation.getStartingFormations();
			}
			this.events = new EscortEvents(this.escort, this.performance);
		}

		// Token: 0x0600016E RID: 366 RVA: 0x000081D4 File Offset: 0x000063D4
		public override void RegisterEvents()
		{
			this.initialize();
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.events.onSessionLaunched));
			CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(this.events.onHourlyTick));
			CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.onSettlementEntered));
			CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.events.onMapEventEnded));
			CampaignEvents.BattleStarted.AddNonSerializedListener(this, new Action<PartyBase, PartyBase, object, bool>(this.events.battleStarted));
			CampaignEvents.OnPlayerBattleEndEvent.AddNonSerializedListener(this, new Action<MapEvent>(this.events.onPlayerBattleEnd));
		}

		// Token: 0x0600016F RID: 367 RVA: 0x0000828A File Offset: 0x0000648A
		private void onSettlementEntered(MobileParty party, Settlement settlement, Hero hero)
		{
			if (!this.menuInitialized)
			{
				this.menuInitialized = true;
			}
			this.events.onSettlementEntered(party, settlement, hero);
		}

		// Token: 0x0400007E RID: 126
		[SaveableField(1)]
		private EscortData escort = new EscortData();

		// Token: 0x0400007F RID: 127
		[SaveableField(2)]
		private EscortBattlesPerformance performance;

		// Token: 0x04000080 RID: 128
		private EscortEvents events;

		// Token: 0x04000081 RID: 129
		private bool menuInitialized;

		// Token: 0x0200008B RID: 139
		public class EscortTypeDefiner : SaveableTypeDefiner
		{
			// Token: 0x060003BF RID: 959 RVA: 0x00013D70 File Offset: 0x00011F70
			public EscortTypeDefiner() : base(41797000)
			{
			}

			// Token: 0x060003C0 RID: 960 RVA: 0x00013D80 File Offset: 0x00011F80
			protected override void DefineClassTypes()
			{
				base.AddClassDefinition(typeof(EscortData), 1, null);
				base.AddClassDefinition(typeof(EscortBattlesPerformance), 2, null);
				base.AddClassDefinition(typeof(BanditFormation), 3, null);
				base.AddClassDefinition(typeof(Troop), 4, null);
				base.AddEnumDefinition(typeof(TroopType), 5, null);
			}

			// Token: 0x060003C1 RID: 961 RVA: 0x00013DE7 File Offset: 0x00011FE7
			protected override void DefineContainerDefinitions()
			{
				base.ConstructContainerDefinition(typeof(List<BanditFormation>));
				base.ConstructContainerDefinition(typeof(List<Troop>));
			}
		}
	}
}
