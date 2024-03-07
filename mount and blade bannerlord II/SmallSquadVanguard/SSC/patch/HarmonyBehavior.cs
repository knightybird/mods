using System;
using TaleWorlds.CampaignSystem;

namespace SSC.patch
{
	// Token: 0x0200003A RID: 58
	internal class HarmonyBehavior : CampaignBehaviorBase
	{
		// Token: 0x060001ED RID: 493 RVA: 0x0000AA89 File Offset: 0x00008C89
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x060001EE RID: 494 RVA: 0x0000AA8B File Offset: 0x00008C8B
		public override void RegisterEvents()
		{
			CampaignEvents.OnCharacterCreationIsOverEvent.AddNonSerializedListener(this, new Action(this.OnCharacterCreationIsOver));
		}

		// Token: 0x060001EF RID: 495 RVA: 0x0000AAA4 File Offset: 0x00008CA4
		private void OnCharacterCreationIsOver()
		{
		}
	}
}
