using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace SSC.party
{
	// Token: 0x0200004B RID: 75
	public static class EquipmentDistribution
	{
		// Token: 0x06000253 RID: 595 RVA: 0x0000D20F File Offset: 0x0000B40F
		private static bool newEquipmentIsBetter(EquipmentElement newElement, EquipmentElement old)
		{
			return false;
		}

		// Token: 0x06000254 RID: 596 RVA: 0x0000D212 File Offset: 0x0000B412
		private static bool distributeOneHanded(ItemRosterElement element)
		{
			Hero.MainHero.BattleEquipment.AddEquipmentToSlotWithoutAgent(EquipmentIndex.Body, element.EquipmentElement);
			return false;
		}
	}
}
