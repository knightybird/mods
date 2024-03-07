using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace SSC.world.escort
{
	// Token: 0x02000027 RID: 39
	public static class DistanceManager
	{
		// Token: 0x060001A5 RID: 421 RVA: 0x000094A1 File Offset: 0x000076A1
		public static IEnumerable<Settlement> findAllHideouts()
		{
			return Settlement.FindAll((Settlement s) => s.IsHideout);
		}

		// Token: 0x060001A6 RID: 422 RVA: 0x000094C8 File Offset: 0x000076C8
		public static List<Settlement> findClosestSettlements<T>(T entity, int settlementCount, Func<Settlement, bool> settlementFilter, IComparer<Settlement> comparer, Func<T, Settlement, float> getDistance)
		{
			IEnumerable<Settlement> enumerable = Settlement.FindAll(settlementFilter);
			SortedSet<Settlement> sortedSet = new SortedSet<Settlement>(comparer);
			foreach (Settlement settlement in enumerable)
			{
				float num = getDistance(entity, settlement);
				if (sortedSet.Count < settlementCount || num < getDistance(entity, sortedSet.Max))
				{
					if (sortedSet.Count == settlementCount)
					{
						sortedSet.Remove(sortedSet.Max);
					}
					sortedSet.Add(settlement);
				}
			}
			return sortedSet.ToList<Settlement>();
		}

		// Token: 0x060001A7 RID: 423 RVA: 0x00009560 File Offset: 0x00007760
		public static List<Settlement> findClosestTowns(Settlement settlement, int townCount)
		{
			return DistanceManager.findClosestSettlements<Settlement>(settlement, townCount, (Settlement s) => s.IsTown && s != settlement, new SettlementDistanceComparer(settlement), new Func<Settlement, Settlement, float>(DistanceManager.getDistance));
		}

		// Token: 0x060001A8 RID: 424 RVA: 0x000095A9 File Offset: 0x000077A9
		public static List<Settlement> findClosestSettlements(Settlement settlement, int settlementCount)
		{
			return DistanceManager.findClosestSettlements<Settlement>(settlement, settlementCount, (Settlement s) => s.IsCastle || s.IsTown, new SettlementDistanceComparer(settlement), new Func<Settlement, Settlement, float>(DistanceManager.getDistance));
		}

		// Token: 0x060001A9 RID: 425 RVA: 0x000095E3 File Offset: 0x000077E3
		public static List<Settlement> findClosestSettlements(MobileParty party, int settlementCount)
		{
			return DistanceManager.findClosestSettlements<MobileParty>(party, settlementCount, (Settlement s) => s.IsCastle || s.IsTown, new SettlementToPartyDistanceComparer(party), new Func<MobileParty, Settlement, float>(DistanceManager.getDistance));
		}

		// Token: 0x060001AA RID: 426 RVA: 0x00009620 File Offset: 0x00007820
		public static Settlement findCastleOf(Settlement village)
		{
			return (from s in Settlement.FindAll((Settlement s) => s.BoundVillages != null)
			where s.BoundVillages.Contains(village.Village)
			select s).FirstOrDefault<Settlement>();
		}

		// Token: 0x060001AB RID: 427 RVA: 0x00009674 File Offset: 0x00007874
		private static float getDistance(Settlement fromSettlement, Settlement toSettlement)
		{
			return Campaign.Current.Models.MapDistanceModel.GetDistance(fromSettlement, toSettlement);
		}

		// Token: 0x060001AC RID: 428 RVA: 0x0000968C File Offset: 0x0000788C
		private static float getDistance(MobileParty fromParty, Settlement toSettlement)
		{
			return Campaign.Current.Models.MapDistanceModel.GetDistance(fromParty, toSettlement);
		}

		// Token: 0x060001AD RID: 429 RVA: 0x000096A4 File Offset: 0x000078A4
		internal static Settlement findClosestNeutralSettlement(MobileParty mobileParty)
		{
			return DistanceManager.findClosestNeutralSettlements(mobileParty).FirstOrDefault<Settlement>();
		}

		// Token: 0x060001AE RID: 430 RVA: 0x000096B4 File Offset: 0x000078B4
		internal static List<Settlement> findClosestNeutralSettlements(MobileParty mobileParty)
		{
			return DistanceManager.findClosestSettlements<MobileParty>(mobileParty, 1, (Settlement s) => (s.IsCastle || s.IsTown) && DistanceManager.settlementIsNeutral(s, mobileParty), new SettlementToPartyDistanceComparer(mobileParty), new Func<MobileParty, Settlement, float>(DistanceManager.getDistance));
		}

		// Token: 0x060001AF RID: 431 RVA: 0x000096FD File Offset: 0x000078FD
		private static bool settlementIsNeutral(Settlement settlement, MobileParty party)
		{
			return party.MapFaction != null && settlement.MapFaction != null && !party.MapFaction.IsAtWarWith(settlement.MapFaction);
		}

		// Token: 0x060001B0 RID: 432 RVA: 0x00009725 File Offset: 0x00007925
		public static float getDistance(MobileParty party, MobileParty otherParty)
		{
			return Campaign.Current.Models.MapDistanceModel.GetDistance(party, otherParty);
		}

		// Token: 0x060001B1 RID: 433 RVA: 0x00009740 File Offset: 0x00007940
		public static Distance getDistance(PartyBase partyBase1, PartyBase partyBase2)
		{
			if (partyBase1 == null || partyBase2 == null)
			{
				return DistanceManager.UnknownDistance;
			}
			MobileParty mobileParty = partyBase1.MobileParty;
			MobileParty mobileParty2 = partyBase2.MobileParty;
			if (mobileParty != null && mobileParty2 != null)
			{
				return new Distance(DistanceManager.getDistance(mobileParty, mobileParty2), true);
			}
			Settlement settlement = partyBase1.Settlement;
			Settlement settlement2 = partyBase2.Settlement;
			if (settlement != null && settlement2 != null)
			{
				return new Distance(DistanceManager.getDistance(settlement, settlement2), true);
			}
			if (mobileParty != null && settlement2 != null)
			{
				return new Distance(DistanceManager.getDistance(mobileParty, settlement2), true);
			}
			if (settlement != null && mobileParty2 != null)
			{
				return new Distance(DistanceManager.getDistance(mobileParty2, settlement), true);
			}
			return DistanceManager.UnknownDistance;
		}

		// Token: 0x060001B2 RID: 434 RVA: 0x000097CC File Offset: 0x000079CC
		public static Distance getDistance(MobileParty party, PartyBase partyBase)
		{
			if (party == null || partyBase == null)
			{
				return DistanceManager.UnknownDistance;
			}
			MobileParty mobileParty = partyBase.MobileParty;
			if (mobileParty != null)
			{
				return new Distance(DistanceManager.getDistance(party, mobileParty), true);
			}
			Settlement settlement = partyBase.Settlement;
			if (settlement != null)
			{
				return new Distance(DistanceManager.getDistance(party, settlement), true);
			}
			return DistanceManager.UnknownDistance;
		}

		// Token: 0x060001B3 RID: 435 RVA: 0x0000981C File Offset: 0x00007A1C
		public static Distance distanceToMainHero(PartyBase party)
		{
			if (party == null)
			{
				return DistanceManager.UnknownDistance;
			}
			if (Hero.MainHero.IsPrisoner)
			{
				PartyBase partyBelongedToAsPrisoner = Hero.MainHero.PartyBelongedToAsPrisoner;
				return DistanceManager.getDistance(party, partyBelongedToAsPrisoner);
			}
			if (Hero.MainHero.IsActive)
			{
				return DistanceManager.getDistance(MobileParty.MainParty, party);
			}
			return DistanceManager.UnknownDistance;
		}

		// Token: 0x0400008D RID: 141
		public static Distance UnknownDistance = new Distance(-1f, false);
	}
}
