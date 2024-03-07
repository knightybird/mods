using System;
using System.Linq;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace SSC.world.castle_party
{
	// Token: 0x0200002B RID: 43
	internal static class CastlePartyHelper
	{
		// Token: 0x060001CA RID: 458 RVA: 0x00009F4C File Offset: 0x0000814C
		public static float calculateSpeed(MobileParty party)
		{
			float num = 6f;
			float num2 = 4.5f;
			float num3 = 0f;
			int num4 = 0;
			foreach (TroopRosterElement troopRosterElement in party.MemberRoster.GetTroopRoster())
			{
				int number = troopRosterElement.Number;
				num4 += number;
				if (troopRosterElement.Character.IsMounted)
				{
					num3 += (float)number * num;
				}
				else
				{
					num3 += (float)number * num2;
				}
			}
			Settlement homeSettlement = party.HomeSettlement;
			float num5 = (homeSettlement.Culture.StringId == "aserai" || homeSettlement.Culture.StringId == "khuzait") ? 1.1f : 1f;
			return num3 / (float)num4 * num5;
		}

		// Token: 0x060001CB RID: 459 RVA: 0x0000A030 File Offset: 0x00008230
		internal static void loadHalfTheTroops(Settlement castle, MobileParty party)
		{
			MobileParty mobileParty = castle.Parties.FirstOrDefault((MobileParty p) => p.IsGarrison);
			if (mobileParty == null)
			{
				return;
			}
			MBList<TroopRosterElement> troopRoster = mobileParty.MemberRoster.GetTroopRoster();
			troopRoster.Sort(delegate(TroopRosterElement a, TroopRosterElement b)
			{
				if (a.Character.IsMounted && !b.Character.IsMounted)
				{
					return -1;
				}
				if (!a.Character.IsMounted && b.Character.IsMounted)
				{
					return 1;
				}
				if (a.Character.Tier > b.Character.Tier)
				{
					return -1;
				}
				if (a.Character.Tier < b.Character.Tier)
				{
					return 1;
				}
				return 0;
			});
			int num = mobileParty.Party.NumberOfAllMembers / 2;
			foreach (TroopRosterElement troopRosterElement in troopRoster)
			{
				if (num == 0)
				{
					break;
				}
				int num2 = Math.Min(troopRosterElement.Number, num);
				mobileParty.MemberRoster.RemoveTroop(troopRosterElement.Character, num2, default(UniqueTroopDescriptor), 0);
				party.MemberRoster.AddToCounts(troopRosterElement.Character, num2, false, 0, 0, true, -1);
				num -= num2;
			}
		}

		// Token: 0x060001CC RID: 460 RVA: 0x0000A134 File Offset: 0x00008334
		internal static void loadSmallTroops(Settlement castle, MobileParty party)
		{
			MobileParty mobileParty = castle.Parties.FirstOrDefault((MobileParty p) => p.IsGarrison);
			if (mobileParty == null)
			{
				return;
			}
			MBList<TroopRosterElement> troopRoster = mobileParty.MemberRoster.GetTroopRoster();
			troopRoster.Sort(delegate(TroopRosterElement a, TroopRosterElement b)
			{
				if (a.Character.IsMounted && !b.Character.IsMounted)
				{
					return -1;
				}
				if (!a.Character.IsMounted && b.Character.IsMounted)
				{
					return 1;
				}
				if (a.Character.Tier > b.Character.Tier)
				{
					return -1;
				}
				if (a.Character.Tier < b.Character.Tier)
				{
					return 1;
				}
				return 0;
			});
			foreach (TroopRosterElement troopRosterElement in troopRoster)
			{
				if (troopRosterElement.Number < 10)
				{
					mobileParty.MemberRoster.RemoveTroop(troopRosterElement.Character, troopRosterElement.Number, default(UniqueTroopDescriptor), 0);
					party.MemberRoster.AddToCounts(troopRosterElement.Character, troopRosterElement.Number, false, 0, 0, true, -1);
				}
			}
		}

		// Token: 0x060001CD RID: 461 RVA: 0x0000A224 File Offset: 0x00008424
		public static int tierCount(MobileParty party)
		{
			int num = 0;
			foreach (TroopRosterElement troopRosterElement in party.MemberRoster.GetTroopRoster())
			{
				num += troopRosterElement.Character.Tier * troopRosterElement.Number;
			}
			return num;
		}

		// Token: 0x060001CE RID: 462 RVA: 0x0000A290 File Offset: 0x00008490
		internal static void loadTroops(Settlement castle, MobileParty punishers, int tierCount)
		{
			MobileParty mobileParty = castle.Parties.FirstOrDefault((MobileParty p) => p.IsGarrison);
			if (mobileParty == null)
			{
				return;
			}
			MBList<TroopRosterElement> troopRoster = mobileParty.MemberRoster.GetTroopRoster();
			troopRoster.Sort(delegate(TroopRosterElement a, TroopRosterElement b)
			{
				if (a.Character.IsMounted && !b.Character.IsMounted)
				{
					return -1;
				}
				if (!a.Character.IsMounted && b.Character.IsMounted)
				{
					return 1;
				}
				if (a.Character.Tier > b.Character.Tier)
				{
					return -1;
				}
				if (a.Character.Tier < b.Character.Tier)
				{
					return 1;
				}
				return 0;
			});
			int num = tierCount;
			foreach (TroopRosterElement troopRosterElement in troopRoster)
			{
				if (num <= 0)
				{
					break;
				}
				int num2 = Math.Min(troopRosterElement.Number, num / troopRosterElement.Character.Tier);
				mobileParty.MemberRoster.RemoveTroop(troopRosterElement.Character, num2, default(UniqueTroopDescriptor), 0);
				punishers.MemberRoster.AddToCounts(troopRosterElement.Character, num2, false, 0, 0, true, -1);
				num -= num2 * troopRosterElement.Character.Tier;
			}
		}

		// Token: 0x060001CF RID: 463 RVA: 0x0000A3A0 File Offset: 0x000085A0
		internal static bool isGarrisonStrongEnough(Settlement castle, MobileParty offenders)
		{
			MobileParty mobileParty = castle.Parties.FirstOrDefault((MobileParty p) => p.IsGarrison);
			if (mobileParty == null)
			{
				return false;
			}
			int num = CastlePartyHelper.tierCount(mobileParty);
			int num2 = CastlePartyHelper.tierCount(offenders);
			return num >= num2 * 2;
		}
	}
}
