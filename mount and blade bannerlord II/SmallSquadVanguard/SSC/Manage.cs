using System;
using SSC.misc;
using SSC.party;

namespace SSC
{
	// Token: 0x02000006 RID: 6
	public static class Manage
	{
		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600001F RID: 31 RVA: 0x000029C8 File Offset: 0x00000BC8
		public static EquipmentManager Equipment { get; } = new EquipmentManager();

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000020 RID: 32 RVA: 0x000029CF File Offset: 0x00000BCF
		public static StaticBodyManager SBP { get; } = new StaticBodyManager();

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000021 RID: 33 RVA: 0x000029D6 File Offset: 0x00000BD6
		public static Healing Healing { get; } = new Healing();

		// Token: 0x0400000F RID: 15
		public static Random rand = new Random();
	}
}
