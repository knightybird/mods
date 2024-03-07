using System;
using TaleWorlds.SaveSystem;

namespace SSC.world.escort
{
	// Token: 0x0200001C RID: 28
	public class Troop
	{
		// Token: 0x04000068 RID: 104
		[SaveableField(1)]
		public TroopType type;

		// Token: 0x04000069 RID: 105
		[SaveableField(2)]
		public int tier;

		// Token: 0x0400006A RID: 106
		[SaveableField(3)]
		public int count;
	}
}
