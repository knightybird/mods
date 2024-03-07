using System;

namespace SSC.world.escort
{
	// Token: 0x02000024 RID: 36
	public struct Distance
	{
		// Token: 0x1700002D RID: 45
		// (get) Token: 0x0600019E RID: 414 RVA: 0x000093C7 File Offset: 0x000075C7
		public float Value { get; }

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x0600019F RID: 415 RVA: 0x000093CF File Offset: 0x000075CF
		public bool IsValid { get; }

		// Token: 0x060001A0 RID: 416 RVA: 0x000093D7 File Offset: 0x000075D7
		public Distance(float value, bool valid)
		{
			this.Value = value;
			this.IsValid = valid;
		}
	}
}
