using System;

namespace SSC.party
{
	// Token: 0x02000040 RID: 64
	public class QueuedAction
	{
		// Token: 0x17000033 RID: 51
		// (get) Token: 0x0600021E RID: 542 RVA: 0x0000BE65 File Offset: 0x0000A065
		// (set) Token: 0x0600021F RID: 543 RVA: 0x0000BE6D File Offset: 0x0000A06D
		public Func<bool> Condition { get; set; }

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x06000220 RID: 544 RVA: 0x0000BE76 File Offset: 0x0000A076
		// (set) Token: 0x06000221 RID: 545 RVA: 0x0000BE7E File Offset: 0x0000A07E
		public Func<bool> Obsolete { get; set; }

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x06000222 RID: 546 RVA: 0x0000BE87 File Offset: 0x0000A087
		// (set) Token: 0x06000223 RID: 547 RVA: 0x0000BE8F File Offset: 0x0000A08F
		public Action ActionToPerform { get; set; }

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x06000224 RID: 548 RVA: 0x0000BE98 File Offset: 0x0000A098
		// (set) Token: 0x06000225 RID: 549 RVA: 0x0000BEA0 File Offset: 0x0000A0A0
		public Func<bool> CheckOnNextTick { get; set; } = () => false;
	}
}
