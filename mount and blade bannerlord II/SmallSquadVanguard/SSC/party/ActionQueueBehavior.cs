using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

namespace SSC.party
{
	// Token: 0x02000042 RID: 66
	internal class ActionQueueBehavior : CampaignBehaviorBase
	{
		// Token: 0x0600022D RID: 557 RVA: 0x0000BFEC File Offset: 0x0000A1EC
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<ActionQueue>("ssc_action_queue", ref this.actionQueue);
		}

		// Token: 0x0600022E RID: 558 RVA: 0x0000C000 File Offset: 0x0000A200
		public override void RegisterEvents()
		{
			World.OnQueueAction += delegate(QueuedAction action)
			{
				this.actionQueue.add(action);
			};
			CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(this.OnHourlyTick));
			CampaignEvents.ConversationEnded.AddNonSerializedListener(this, new Action<IEnumerable<CharacterObject>>(this.OnConversationEnded));
		}

		// Token: 0x0600022F RID: 559 RVA: 0x0000C04C File Offset: 0x0000A24C
		private void OnConversationEnded(IEnumerable<CharacterObject> enumerable)
		{
			if (this.actionQueue.hasActions())
			{
				this.actionQueue.execute();
			}
		}

		// Token: 0x06000230 RID: 560 RVA: 0x0000C066 File Offset: 0x0000A266
		private void OnHourlyTick()
		{
			if (this.actionQueue.hasActions())
			{
				this.actionQueue.execute();
			}
		}

		// Token: 0x040000B3 RID: 179
		[SaveableField(0)]
		private ActionQueue actionQueue = new ActionQueue();

		// Token: 0x02000099 RID: 153
		public class ActionsTypeDefiner : SaveableTypeDefiner
		{
			// Token: 0x06000404 RID: 1028 RVA: 0x00014330 File Offset: 0x00012530
			public ActionsTypeDefiner() : base(41797064)
			{
			}

			// Token: 0x06000405 RID: 1029 RVA: 0x0001433D File Offset: 0x0001253D
			protected override void DefineClassTypes()
			{
				base.AddClassDefinition(typeof(ActionQueue), 1, null);
				base.AddClassDefinition(typeof(QueuedAction), 2, null);
			}

			// Token: 0x06000406 RID: 1030 RVA: 0x00014363 File Offset: 0x00012563
			protected override void DefineContainerDefinitions()
			{
				base.ConstructContainerDefinition(typeof(List<QueuedAction>));
			}
		}
	}
}
