using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.SaveSystem;

namespace SSC.world.castle_party
{
	// Token: 0x02000029 RID: 41
	internal class CastlePartyBehavior : CampaignBehaviorBase
	{
		// Token: 0x060001B6 RID: 438 RVA: 0x00009888 File Offset: 0x00007A88
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<CastleParty>("castle_party_data", ref this.castleParty);
		}

		// Token: 0x060001B7 RID: 439 RVA: 0x0000989C File Offset: 0x00007A9C
		public override void RegisterEvents()
		{
			World.CastleParty = this.castleParty;
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
			CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(this.castleParty.OnHourlyTick));
			CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.castleParty.OnMapEventEnded));
			CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.castleParty.OnSettlementEntered));
		}

		// Token: 0x060001B8 RID: 440 RVA: 0x0000991F File Offset: 0x00007B1F
		private void OnSessionLaunched(CampaignGameStarter starter)
		{
			this.castleParty.AddDialogs(starter);
		}

		// Token: 0x0400008E RID: 142
		[SaveableField(1)]
		private CastleParty castleParty = new CastleParty();

		// Token: 0x02000093 RID: 147
		public class CastlePartyTypeDefiner : SaveableTypeDefiner
		{
			// Token: 0x060003D7 RID: 983 RVA: 0x00013F73 File Offset: 0x00012173
			public CastlePartyTypeDefiner() : base(41799000)
			{
			}

			// Token: 0x060003D8 RID: 984 RVA: 0x00013F80 File Offset: 0x00012180
			protected override void DefineClassTypes()
			{
				base.AddClassDefinition(typeof(CastleParty), 1, null);
			}
		}
	}
}
