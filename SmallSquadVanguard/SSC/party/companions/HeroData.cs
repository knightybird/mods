using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;

namespace SSC.party.companions
{
	// Token: 0x02000065 RID: 101
	public class HeroData
	{
		// Token: 0x060002DE RID: 734 RVA: 0x0000FDE5 File Offset: 0x0000DFE5
		public HeroData(Dictionary<Hero, int> data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data", "Data cannot be null");
			}
			this.map = data;
		}

		// Token: 0x060002DF RID: 735 RVA: 0x0000FE07 File Offset: 0x0000E007
		public int getHeroFlags(Hero hero)
		{
			if (hero == null)
			{
				return 0;
			}
			if (this.map.ContainsKey(hero))
			{
				return this.map[hero];
			}
			return 0;
		}

		// Token: 0x060002E0 RID: 736 RVA: 0x0000FE2A File Offset: 0x0000E02A
		public bool isFlagSet(Hero hero, HeroFlags flag)
		{
			return hero != null && this.map.ContainsKey(hero) && (this.map[hero] & (int)flag) != 0;
		}

		// Token: 0x060002E1 RID: 737 RVA: 0x0000FE52 File Offset: 0x0000E052
		public bool flagNotSet(Hero hero, HeroFlags flag)
		{
			return hero == null || !this.map.ContainsKey(hero) || (this.map[hero] & (int)flag) == 0;
		}

		// Token: 0x060002E2 RID: 738 RVA: 0x0000FE7C File Offset: 0x0000E07C
		public void setFlag(Hero hero, HeroFlags flag)
		{
			if (hero == null)
			{
				return;
			}
			if (this.map.ContainsKey(hero))
			{
				Dictionary<Hero, int> dictionary = this.map;
				dictionary[hero] |= (int)flag;
				this.removeHeroIfNeeded(hero);
				return;
			}
			this.map.Add(hero, (int)flag);
		}

		// Token: 0x060002E3 RID: 739 RVA: 0x0000FECC File Offset: 0x0000E0CC
		public void toggleFlag(Hero hero, HeroFlags flag)
		{
			if (hero == null)
			{
				return;
			}
			if (this.map.ContainsKey(hero))
			{
				Dictionary<Hero, int> dictionary = this.map;
				dictionary[hero] ^= (int)flag;
				this.removeHeroIfNeeded(hero);
				return;
			}
			this.map.Add(hero, (int)flag);
		}

		// Token: 0x060002E4 RID: 740 RVA: 0x0000FF1C File Offset: 0x0000E11C
		public void clearFlag(Hero hero, HeroFlags flag)
		{
			if (hero == null)
			{
				return;
			}
			if (this.map.ContainsKey(hero))
			{
				Dictionary<Hero, int> dictionary = this.map;
				dictionary[hero] &= (int)(~(int)flag);
				this.removeHeroIfNeeded(hero);
			}
		}

		// Token: 0x060002E5 RID: 741 RVA: 0x0000FF5C File Offset: 0x0000E15C
		private void removeHeroIfNeeded(Hero hero)
		{
			if (this.map[hero] == 0)
			{
				this.map.Remove(hero);
			}
		}

		// Token: 0x060002E6 RID: 742 RVA: 0x0000FF79 File Offset: 0x0000E179
		public void clearAllFlags(Hero hero)
		{
			if (hero == null)
			{
				return;
			}
			if (this.map.ContainsKey(hero))
			{
				this.map.Remove(hero);
			}
		}

		// Token: 0x040000E4 RID: 228
		private readonly Dictionary<Hero, int> map;
	}
}
