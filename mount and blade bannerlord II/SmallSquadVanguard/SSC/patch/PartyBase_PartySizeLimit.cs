using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;

namespace SSC.patch
{
	// Token: 0x0200003E RID: 62
	public static class PartyBase_PartySizeLimit
	{
		// Token: 0x06000209 RID: 521 RVA: 0x0000B618 File Offset: 0x00009818
		public static void PartySizeLimit_Postfix(PartyBase __instance, ref int __result)
		{
			if (__instance.MobileParty == MobileParty.MainParty)
			{
				int num = 0;
				List<Hero> list = Hero.MainHero.Clan.Heroes.ToList<Hero>();
				list.Remove(Hero.MainHero);
				foreach (Hero item in Hero.MainHero.Clan.Companions)
				{
					list.Remove(item);
				}
				foreach (Hero hero in list)
				{
					if (hero.IsAlive && hero.PartyBelongedTo == MobileParty.MainParty)
					{
						num++;
					}
				}
				__result = 30 + num;
			}
		}
	}
}
