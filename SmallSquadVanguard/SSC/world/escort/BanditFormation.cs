using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.SaveSystem;

namespace SSC.world.escort
{
	// Token: 0x0200001D RID: 29
	public class BanditFormation
	{
		// Token: 0x06000149 RID: 329 RVA: 0x000077E7 File Offset: 0x000059E7
		public void setTargetsAtStart(int targetMelee, int targetRanged, int targetMounted, int targetHorseArcher)
		{
			this.targetMelee = targetMelee;
			this.targetRanged = targetRanged;
			this.targetMounted = targetMounted;
			this.targetHorseArcher = targetHorseArcher;
		}

		// Token: 0x0600014A RID: 330 RVA: 0x00007808 File Offset: 0x00005A08
		public int upperBound()
		{
			float num = (float)this.targetMounted / (float)this.maxTotalTroops;
			double num2 = (double)12f;
			float num3 = 1f - num / 2f;
			float num4 = 12f * Campaign.Current.PlayerProgress;
			return (int)Math.Round(num2 * (double)num3 + (double)num4);
		}

		// Token: 0x0600014B RID: 331 RVA: 0x00007854 File Offset: 0x00005A54
		public void loadTroopsTo(MobileParty banditParty, List<string> allowedCultureIds)
		{
			List<TroopRosterElement> list = BanditTypes.constructTroopList(this.troops, allowedCultureIds);
			banditParty.MemberRoster.Clear();
			foreach (TroopRosterElement troopRosterElement in list)
			{
				banditParty.MemberRoster.AddToCounts(troopRosterElement.Character, troopRosterElement.Number, false, 0, 0, true, -1);
			}
		}

		// Token: 0x0600014C RID: 332 RVA: 0x000078D0 File Offset: 0x00005AD0
		public static List<BanditFormation> getStartingFormations()
		{
			List<BanditFormation> list = new List<BanditFormation>();
			BanditFormation banditFormation = new BanditFormation();
			banditFormation.setTargetsAtStart(35, 0, 0, 0);
			banditFormation.grow(28, false);
			list.Add(banditFormation);
			BanditFormation banditFormation2 = new BanditFormation();
			banditFormation2.setTargetsAtStart(0, 35, 0, 0);
			banditFormation2.grow(24, false);
			list.Add(banditFormation2);
			BanditFormation banditFormation3 = new BanditFormation();
			banditFormation3.setTargetsAtStart(0, 0, 35, 0);
			banditFormation3.grow(10, false);
			list.Add(banditFormation3);
			BanditFormation banditFormation4 = new BanditFormation();
			banditFormation4.setTargetsAtStart(15, 10, 10, 0);
			banditFormation4.grow(25, false);
			list.Add(banditFormation4);
			return list;
		}

		// Token: 0x0600014D RID: 333 RVA: 0x00007968 File Offset: 0x00005B68
		public void grow(int upgradePoints, bool upgradesAllowed = true)
		{
			if (this.troops == null)
			{
				this.troops = new List<Troop>();
			}
			this.sortTroops();
			while (upgradePoints > 0 && (this.totalTroops() < this.maxTotalTroops || this.singleUpgradeAcrossAllTypes()) && this.singleIncreaseTroops(ref upgradePoints))
			{
			}
			if (!upgradesAllowed)
			{
				return;
			}
			for (int i = 0; i < this.upgradesPerGrowth(); i++)
			{
				if (!this.singleUpgradeAcrossAllTypes())
				{
					return;
				}
			}
		}

		// Token: 0x0600014E RID: 334 RVA: 0x000079D4 File Offset: 0x00005BD4
		private int upgradesPerGrowth()
		{
			int num = 0;
			int num2 = this.totalTroops();
			if (num2 >= 10)
			{
				num++;
			}
			if (num2 >= 20)
			{
				num++;
			}
			if (num2 >= 30)
			{
				num++;
			}
			return num;
		}

		// Token: 0x0600014F RID: 335 RVA: 0x00007A04 File Offset: 0x00005C04
		private bool singleUpgradeAcrossAllTypes()
		{
			List<TroopType> list = (from TroopType t in Enum.GetValues(typeof(TroopType))
			where t > TroopType.None
			select t).ToList<TroopType>();
			list.Shuffle<TroopType>();
			foreach (TroopType troopType in list)
			{
				if (this.singleUpgrade(troopType))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000150 RID: 336 RVA: 0x00007AA0 File Offset: 0x00005CA0
		private bool singleUpgrade(TroopType troopType)
		{
			this.sortTroops();
			Troop troop = (from t in this.troops
			where t.type == troopType
			select t).FirstOrDefault<Troop>();
			if (troop == null)
			{
				return false;
			}
			if (troop.count > 2)
			{
				troop.count -= 2;
			}
			else if (troop.count == 1)
			{
				troop.count = 0;
			}
			int nextTier = troop.tier + 1;
			Troop troop2 = (from t in this.troops
			where t.type == troopType && t.tier == nextTier
			select t).FirstOrDefault<Troop>();
			if (troop2 == null)
			{
				troop2 = new Troop
				{
					type = troopType,
					tier = nextTier,
					count = 1
				};
				this.troops.Add(troop2);
			}
			else
			{
				troop2.count++;
			}
			return true;
		}

		// Token: 0x06000151 RID: 337 RVA: 0x00007B7C File Offset: 0x00005D7C
		private void addTroopToFormation(Troop troop)
		{
			Troop troop2 = this.troops.FirstOrDefault((Troop t) => t.type == troop.type && t.tier == troop.tier);
			if (troop2 == null)
			{
				this.troops.Add(troop);
				return;
			}
			troop2.count += troop.count;
		}

		// Token: 0x06000152 RID: 338 RVA: 0x00007BDC File Offset: 0x00005DDC
		private bool singleIncreaseTroops(ref int upgradePoints)
		{
			TroopType troopType = this.chooseBasedOnFormation();
			if (troopType == TroopType.None)
			{
				return false;
			}
			if (this.currentCountOfType(troopType) >= this.targetByType(troopType))
			{
				return false;
			}
			Troop troop = this.findLowestTieredTroop(troopType);
			if (troop.type == TroopType.None)
			{
				int num = BanditTypes.lowestTierByType(troopType);
				Troop troop2 = new Troop
				{
					type = troopType,
					tier = num,
					count = 1
				};
				upgradePoints -= num;
				this.addTroopToFormation(troop2);
				return true;
			}
			int tier = troop.tier;
			if (tier > upgradePoints)
			{
				return false;
			}
			upgradePoints -= tier;
			troop.count++;
			return true;
		}

		// Token: 0x06000153 RID: 339 RVA: 0x00007C70 File Offset: 0x00005E70
		private Troop findLowestTieredTroop(TroopType troopType)
		{
			Troop troop = new Troop
			{
				tier = BanditFormation.maxTier + 1,
				type = troopType
			};
			foreach (Troop troop2 in this.troops)
			{
				if (troop2.type == troopType && troop2.tier < troop.tier)
				{
					troop = troop2;
				}
			}
			if (troop.tier == BanditFormation.maxTier + 1)
			{
				return new Troop
				{
					type = TroopType.None
				};
			}
			return troop;
		}

		// Token: 0x06000154 RID: 340 RVA: 0x00007D0C File Offset: 0x00005F0C
		private TroopType chooseBasedOnFormation()
		{
			int num = this.targetMelee - this.currentCountOfType(TroopType.Melee);
			int num2 = this.targetRanged - this.currentCountOfType(TroopType.Ranged);
			int num3 = this.targetMounted - this.currentCountOfType(TroopType.Mounted);
			int num4 = this.targetHorseArcher - this.currentCountOfType(TroopType.HorseArcher);
			int num5 = num + num2 + num3 + num4;
			if (num5 <= 0)
			{
				return TroopType.None;
			}
			int num6 = (num5 == 1) ? 1 : Manage.rand.Next(1, num5 + 1);
			if (num6 <= num)
			{
				return TroopType.Melee;
			}
			if (num6 <= num + num2)
			{
				return TroopType.Ranged;
			}
			if (num6 <= num + num2 + num3)
			{
				return TroopType.Mounted;
			}
			if (num6 <= num + num2 + num3 + num4)
			{
				return TroopType.HorseArcher;
			}
			return TroopType.None;
		}

		// Token: 0x06000155 RID: 341 RVA: 0x00007DA7 File Offset: 0x00005FA7
		private int targetByType(TroopType troopType)
		{
			if (troopType == TroopType.Melee)
			{
				return this.targetMelee;
			}
			if (troopType == TroopType.Ranged)
			{
				return this.targetRanged;
			}
			if (troopType == TroopType.Mounted)
			{
				return this.targetMounted;
			}
			if (troopType == TroopType.HorseArcher)
			{
				return this.targetHorseArcher;
			}
			return 0;
		}

		// Token: 0x06000156 RID: 342 RVA: 0x00007DD8 File Offset: 0x00005FD8
		private int currentCountOfType(TroopType troopType)
		{
			return (from t in this.troops
			where t.type == troopType
			select t).Sum((Troop t) => t.count);
		}

		// Token: 0x06000157 RID: 343 RVA: 0x00007E2D File Offset: 0x0000602D
		private int totalTroops()
		{
			return this.troops.Sum((Troop t) => t.count);
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x06000158 RID: 344 RVA: 0x00007E59 File Offset: 0x00006059
		private int maxTotalTroops
		{
			get
			{
				return this.targetMelee + this.targetRanged + this.targetMounted + this.targetHorseArcher;
			}
		}

		// Token: 0x06000159 RID: 345 RVA: 0x00007E76 File Offset: 0x00006076
		private void sortTroops()
		{
			this.troops.Sort(delegate(Troop a, Troop b)
			{
				if (a.tier == b.tier)
				{
					return a.type - b.type;
				}
				return a.tier - b.tier;
			});
		}

		// Token: 0x0400006B RID: 107
		[SaveableField(1)]
		private int targetMelee;

		// Token: 0x0400006C RID: 108
		[SaveableField(2)]
		private int targetRanged;

		// Token: 0x0400006D RID: 109
		[SaveableField(3)]
		private int targetMounted;

		// Token: 0x0400006E RID: 110
		[SaveableField(4)]
		private int targetHorseArcher;

		// Token: 0x0400006F RID: 111
		[SaveableField(5)]
		public List<Troop> troops = new List<Troop>();

		// Token: 0x04000070 RID: 112
		public static int maxTier = 6;
	}
}
