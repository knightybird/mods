using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Settlements;

namespace SSC.world.escort
{
	// Token: 0x02000018 RID: 24
	public static class BanditCultures
	{
		// Token: 0x06000134 RID: 308 RVA: 0x000067E4 File Offset: 0x000049E4
		public static List<string> appropriateCultureIds(Settlement destinationTown, Settlement hideoutOrigin)
		{
			List<string> list = new List<string>();
			list.Add(destinationTown.Culture.StringId);
			list.Add(hideoutOrigin.Culture.StringId);
			foreach (Settlement settlement in DistanceManager.findClosestTowns(destinationTown, 5))
			{
				if (!list.Contains(settlement.Culture.StringId))
				{
					list.Add(settlement.Culture.StringId);
				}
			}
			return list;
		}

		// Token: 0x06000135 RID: 309 RVA: 0x00006880 File Offset: 0x00004A80
		private static void addCulture(string cultureId, List<string> appropriateCultureIds)
		{
			if (!appropriateCultureIds.Contains(cultureId))
			{
				appropriateCultureIds.Add(cultureId);
			}
		}
	}
}
