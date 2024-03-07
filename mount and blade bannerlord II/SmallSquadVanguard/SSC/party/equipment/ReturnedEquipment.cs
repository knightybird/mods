using System;
using TaleWorlds.Core;

namespace SSC.party.equipment
{
	// Token: 0x0200005E RID: 94
	public class ReturnedEquipment
	{
		// Token: 0x060002BE RID: 702 RVA: 0x0000F333 File Offset: 0x0000D533
		public ReturnedEquipment(EquipmentElement oldEquipment, Outcome outcome)
		{
			this.oldEquipment = oldEquipment;
			this.outcome = outcome;
		}

		// Token: 0x040000D1 RID: 209
		public EquipmentElement oldEquipment;

		// Token: 0x040000D2 RID: 210
		public Outcome outcome;
	}
}
