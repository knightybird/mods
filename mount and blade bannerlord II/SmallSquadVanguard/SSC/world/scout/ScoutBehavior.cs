using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.SaveSystem;

namespace SSC.world.scout
{
	// Token: 0x02000012 RID: 18
	internal class ScoutBehavior : CampaignBehaviorBase
	{
		// Token: 0x060000CF RID: 207 RVA: 0x0000499D File Offset: 0x00002B9D
		public void setScoutReputaiton(float reputation)
		{
			this.scoutData.setReputation(reputation);
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x000049AB File Offset: 0x00002BAB
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<ScoutMission>("scout_data", ref this.scoutData);
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x000049C0 File Offset: 0x00002BC0
		private void initialize()
		{
			if (this.scoutData == null)
			{
				this.scoutData = new ScoutMission();
			}
			World.OnSetScoutReputation += this.setScoutReputaiton;
			this.scoutEvents = new ScoutEvents(this.scoutData);
			this.scoutDialogs = new ScoutDialogs(this.scoutData);
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x00004A14 File Offset: 0x00002C14
		public override void RegisterEvents()
		{
			this.initialize();
			this.scoutData.initialize();
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
			CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(this.scoutEvents.OnHourlyTick));
			CampaignEvents.ConversationEnded.AddNonSerializedListener(this, new Action<IEnumerable<CharacterObject>>(this.scoutDialogs.OnConversationEnded));
			CampaignEvents.HeroPrisonerTaken.AddNonSerializedListener(this, new Action<PartyBase, Hero>(this.scoutEvents.OnHeroPrisonerTaken));
			CampaignEvents.HeroKilledEvent.AddNonSerializedListener(this, new Action<Hero, Hero, KillCharacterAction.KillCharacterActionDetail, bool>(this.scoutEvents.OnHeroKilled));
			CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.scoutEvents.OnSettlementEntered));
			CampaignEvents.ArmyDispersed.AddNonSerializedListener(this, new Action<Army, Army.ArmyDispersionReason, bool>(this.scoutEvents.OnArmyDispersed));
			CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.scoutEvents.OnMapEventEnded));
			CampaignEvents.WarDeclared.AddNonSerializedListener(this, new Action<IFaction, IFaction, DeclareWarAction.DeclareWarDetail>(this.scoutEvents.OnWarDeclared));
			CampaignEvents.MakePeace.AddNonSerializedListener(this, new Action<IFaction, IFaction, MakePeaceAction.MakePeaceDetail>(this.scoutEvents.OnMakePeace));
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x00004B45 File Offset: 0x00002D45
		private void OnSessionLaunched(CampaignGameStarter starter)
		{
			this.scoutDialogs.AddMenus(starter);
			this.scoutDialogs.AddDialogs(starter);
			this.scoutDialogs.AddMoreDialogs(starter);
			this.scoutDialogs.AddDialogsSubordinates(starter);
			this.scoutEvents.sessionLaunched();
		}

		// Token: 0x04000046 RID: 70
		[SaveableField(1)]
		private ScoutMission scoutData = new ScoutMission();

		// Token: 0x04000047 RID: 71
		private ScoutEvents scoutEvents;

		// Token: 0x04000048 RID: 72
		private ScoutDialogs scoutDialogs;

		// Token: 0x0200007E RID: 126
		public class ScoutTypeDefiner : SaveableTypeDefiner
		{
			// Token: 0x06000397 RID: 919 RVA: 0x00013BA4 File Offset: 0x00011DA4
			public ScoutTypeDefiner() : base(41798000)
			{
			}

			// Token: 0x06000398 RID: 920 RVA: 0x00013BB1 File Offset: 0x00011DB1
			protected override void DefineClassTypes()
			{
				base.AddClassDefinition(typeof(ScoutMission), 1, null);
			}
		}
	}
}
