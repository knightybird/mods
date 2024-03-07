using System;
using System.Collections.Generic;
using System.Linq;
using SSC.world.escort;
using TaleWorlds.CampaignSystem;
using TaleWorlds.ObjectSystem;

namespace SSC.escort
{
	// Token: 0x02000073 RID: 115
	internal class TroopReader
	{
		// Token: 0x0600035C RID: 860 RVA: 0x00012BB0 File Offset: 0x00010DB0
		public static void fillTroops(List<string> cultureIds, List<CharacterObject> regulars, List<CharacterObject> bandits)
		{
			foreach (CharacterObject characterObject in TroopReader.allCultureTroops(cultureIds, false))
			{
				if (characterObject.Culture.IsBandit)
				{
					bandits.Add(characterObject);
				}
				else
				{
					regulars.Add(characterObject);
				}
			}
		}

		// Token: 0x0600035D RID: 861 RVA: 0x00012C1C File Offset: 0x00010E1C
		public static CultureObject findLootersCulture()
		{
			foreach (IFaction faction in Campaign.Current.Factions)
			{
				if (TroopReader.IsLooterFaction(faction))
				{
					return faction.Culture;
				}
			}
			return null;
		}

		// Token: 0x0600035E RID: 862 RVA: 0x00012C7C File Offset: 0x00010E7C
		private static bool IsLooterFaction(IFaction faction)
		{
			return !faction.Culture.CanHaveSettlement;
		}

		// Token: 0x0600035F RID: 863 RVA: 0x00012C8C File Offset: 0x00010E8C
		private static List<CharacterObject> allCultureTroops(List<string> cultureIds, bool banditsOnly)
		{
			return TroopReader.allCultureTroops(TroopReader.convertFromStrings(cultureIds), banditsOnly);
		}

		// Token: 0x06000360 RID: 864 RVA: 0x00012C9C File Offset: 0x00010E9C
		private static List<CultureObject> convertFromStrings(List<string> cultureIds)
		{
			List<CultureObject> list = new List<CultureObject>();
			foreach (string objectName in cultureIds)
			{
				CultureObject @object = MBObjectManager.Instance.GetObject<CultureObject>(objectName);
				if (@object != null)
				{
					list.Add(@object);
				}
			}
			return list;
		}

		// Token: 0x06000361 RID: 865 RVA: 0x00012D00 File Offset: 0x00010F00
		private static List<CharacterObject> allCultureTroops(List<CultureObject> cultures, bool banditsOnly)
		{
			HashSet<CharacterObject> hashSet = new HashSet<CharacterObject>();
			HashSet<CultureObject> hashSet2 = new HashSet<CultureObject>(cultures);
			TroopReader.addTroopsFromCultures(cultures, hashSet, banditsOnly);
			foreach (IFaction faction in Campaign.Current.Factions)
			{
				if (hashSet2.Contains(faction.Culture))
				{
					TroopReader.addUniqueUpgradeTargets(faction.BasicTroop, hashSet, banditsOnly);
				}
			}
			return (from troop in hashSet
			orderby troop.Tier
			select troop).ToList<CharacterObject>();
		}

		// Token: 0x06000362 RID: 866 RVA: 0x00012DA4 File Offset: 0x00010FA4
		public static List<CharacterObject> allBanditsFromCulture(CultureObject culture)
		{
			return TroopReader.allCultureTroops(new List<CultureObject>
			{
				culture
			}, true);
		}

		// Token: 0x06000363 RID: 867 RVA: 0x00012DB8 File Offset: 0x00010FB8
		public static List<CharacterObject> allCultureTroops(CultureObject culture)
		{
			HashSet<CharacterObject> hashSet = new HashSet<CharacterObject>();
			CharacterObject eliteBasicTroop = culture.EliteBasicTroop;
			TroopReader.addTroopsFromCultures(new List<CultureObject>
			{
				culture
			}, hashSet, false);
			foreach (IFaction faction in Campaign.Current.Factions)
			{
				if (faction.Culture == culture)
				{
					TroopReader.addUniqueUpgradeTargets(faction.BasicTroop, hashSet, false);
				}
			}
			return (from troop in hashSet
			orderby troop.Tier
			select troop).ToList<CharacterObject>();
		}

		// Token: 0x06000364 RID: 868 RVA: 0x00012E64 File Offset: 0x00011064
		public static CharacterObject findReplacementOrUseLooters(Troop troop, List<CharacterObject> lootersAsLastMeasure)
		{
			List<CultureObject> list = new List<CultureObject>();
			foreach (IFaction faction in Campaign.Current.Factions)
			{
				list.Add(faction.Culture);
			}
			CharacterObject characterObject = TroopReader.allCultureTroops(list, false).FirstOrDefault((CharacterObject character) => TroopReader.troopTypeOf(character) == troop.type && character.Tier == troop.tier);
			if (characterObject == null)
			{
				characterObject = lootersAsLastMeasure.FirstOrDefault((CharacterObject character) => character.Tier == troop.tier + 1);
			}
			if (characterObject == null)
			{
				characterObject = lootersAsLastMeasure.FirstOrDefault((CharacterObject character) => character.Tier <= troop.tier);
			}
			if (characterObject == null)
			{
				throw new Exception(string.Format("No replacement found for a {0} troop of tier {1}", troop.type, troop.tier));
			}
			return characterObject;
		}

		// Token: 0x06000365 RID: 869 RVA: 0x00012F48 File Offset: 0x00011148
		public static TroopType troopTypeOf(CharacterObject ch)
		{
			if (ch == null)
			{
				return TroopType.None;
			}
			if (!ch.IsRanged && !ch.IsMounted)
			{
				return TroopType.Melee;
			}
			if (ch.IsRanged && !ch.IsMounted)
			{
				return TroopType.Ranged;
			}
			if (ch.IsMounted && !ch.IsRanged)
			{
				return TroopType.Mounted;
			}
			if (ch.IsMounted && ch.IsRanged)
			{
				return TroopType.HorseArcher;
			}
			return TroopType.None;
		}

		// Token: 0x06000366 RID: 870 RVA: 0x00012FA4 File Offset: 0x000111A4
		private static void addTroopsFromCultures(List<CultureObject> cultures, HashSet<CharacterObject> uniqueTroops, bool banditsOnly)
		{
			foreach (CultureObject cultureObject in cultures)
			{
				TroopReader.addUniqueUpgradeTargets(cultureObject.EliteBasicTroop, uniqueTroops, banditsOnly);
				TroopReader.addUniqueUpgradeTargets(cultureObject.MilitiaArcher, uniqueTroops, banditsOnly);
				TroopReader.addUniqueUpgradeTargets(cultureObject.MilitiaVeteranArcher, uniqueTroops, banditsOnly);
				TroopReader.addUniqueUpgradeTargets(cultureObject.RangedMilitiaTroop, uniqueTroops, banditsOnly);
				TroopReader.addUniqueUpgradeTargets(cultureObject.MilitiaSpearman, uniqueTroops, banditsOnly);
				TroopReader.addUniqueUpgradeTargets(cultureObject.MilitiaVeteranSpearman, uniqueTroops, banditsOnly);
				TroopReader.addUniqueUpgradeTargets(cultureObject.MeleeMilitiaTroop, uniqueTroops, banditsOnly);
				TroopReader.addUniqueUpgradeTargets(cultureObject.MeleeEliteMilitiaTroop, uniqueTroops, banditsOnly);
				TroopReader.addUniqueUpgradeTargets(cultureObject.RangedEliteMilitiaTroop, uniqueTroops, banditsOnly);
				uniqueTroops.UnionWith(cultureObject.BasicMercenaryTroops);
			}
		}

		// Token: 0x06000367 RID: 871 RVA: 0x00013074 File Offset: 0x00011274
		private static List<CharacterObject> banditsOnly(List<CharacterObject> troops)
		{
			return troops.Where(delegate(CharacterObject troop)
			{
				CultureObject culture = troop.Culture;
				return culture != null && culture.IsBandit;
			}).ToList<CharacterObject>();
		}

		// Token: 0x06000368 RID: 872 RVA: 0x000130A0 File Offset: 0x000112A0
		private static void list(List<CharacterObject> troops, string message)
		{
			foreach (CharacterObject characterObject in troops)
			{
			}
		}

		// Token: 0x06000369 RID: 873 RVA: 0x000130E8 File Offset: 0x000112E8
		private static void allUpgradeTargets(CharacterObject troop, List<CharacterObject> allTroopTypes, bool banditsOnly)
		{
			if (troop == null)
			{
				return;
			}
			allTroopTypes.Add(troop);
			foreach (CharacterObject characterObject in troop.UpgradeTargets)
			{
				if ((!banditsOnly || characterObject.Culture.IsBandit) && !allTroopTypes.Contains(characterObject))
				{
					TroopReader.allUpgradeTargets(characterObject, allTroopTypes, banditsOnly);
				}
			}
		}

		// Token: 0x0600036A RID: 874 RVA: 0x0001313C File Offset: 0x0001133C
		private static void listUpgradeTargets(CharacterObject troop, ref int count)
		{
			if (troop == null)
			{
				return;
			}
			CharacterObject[] upgradeTargets = troop.UpgradeTargets;
			for (int i = 0; i < upgradeTargets.Length; i++)
			{
				TroopReader.listUpgradeTargets(upgradeTargets[i], ref count);
				count++;
			}
		}

		// Token: 0x0600036B RID: 875 RVA: 0x00013174 File Offset: 0x00011374
		private static void addUniqueUpgradeTargets(CharacterObject troop, HashSet<CharacterObject> uniqueTroops, bool banditsOnly)
		{
			if (troop == null)
			{
				return;
			}
			List<CharacterObject> list = new List<CharacterObject>();
			TroopReader.allUpgradeTargets(troop, list, banditsOnly);
			uniqueTroops.UnionWith(list);
		}
	}
}
