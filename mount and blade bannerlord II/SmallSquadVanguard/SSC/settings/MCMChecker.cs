using System;

namespace SSC.settings
{
	// Token: 0x02000030 RID: 48
	internal static class MCMChecker
	{
		// Token: 0x060001D9 RID: 473 RVA: 0x0000A599 File Offset: 0x00008799
		public static bool IsMCMLoaded()
		{
			return Type.GetType("ModConfigurationMenu.GlobalSettings`1") != null;
		}
	}
}
