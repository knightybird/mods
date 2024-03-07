using System;
using System.Collections.Generic;
using SSC.escort;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;

namespace SSC.world.escort
{
	// Token: 0x0200001A RID: 26
	public static class BanditTypes
	{
		// Token: 0x0600013C RID: 316 RVA: 0x00006B5C File Offset: 0x00004D5C
		public static List<TroopRosterElement> constructTroopList(List<Troop> troops, List<string> cultureIds)
		{
			List<CharacterObject> list = TroopReader.allBanditsFromCulture(TroopReader.findLootersCulture());
			List<TroopRosterElement> list2 = new List<TroopRosterElement>();
			List<CharacterObject> characters = new List<CharacterObject>();
			List<CharacterObject> list3 = new List<CharacterObject>();
			TroopReader.fillTroops(cultureIds, characters, list3);
			list3.AddRange(list);
			foreach (Troop troop in troops)
			{
				List<CharacterObject> collection = BanditTypes.findAllMatches(troop, list3);
				List<CharacterObject> collection2 = BanditTypes.findAllMatches(troop, characters);
				List<CharacterObject> list4 = new List<CharacterObject>();
				list4.AddRange(collection);
				list4.AddRange(collection);
				list4.AddRange(collection);
				list4.AddRange(collection2);
				if (list4.Count == 0)
				{
					CharacterObject character = TroopReader.findReplacementOrUseLooters(troop, list);
					TroopRosterElement item = new TroopRosterElement(character)
					{
						Number = troop.count
					};
					list2.Add(item);
				}
				else
				{
					for (int i = 0; i < troop.count; i++)
					{
						BanditTypes.addTroopRandomly(list4, list2);
					}
				}
			}
			return list2;
		}

		// Token: 0x0600013D RID: 317 RVA: 0x00006C70 File Offset: 0x00004E70
		private static void addTroopRandomly(List<CharacterObject> matches, List<TroopRosterElement> list)
		{
			int index = Manage.rand.Next(matches.Count);
			CharacterObject character = matches[index];
			TroopRosterElement item = new TroopRosterElement(character)
			{
				Number = 1
			};
			list.Add(item);
		}

		// Token: 0x0600013E RID: 318 RVA: 0x00006CB0 File Offset: 0x00004EB0
		public static int lowestTierByType(TroopType type)
		{
			if (!BanditTypes.initialized)
			{
				BanditTypes.initialize();
			}
			switch (type)
			{
			case TroopType.Melee:
				return BanditTypes.lowestMeleeTier;
			case TroopType.Ranged:
				return BanditTypes.lowestRangedTier;
			case TroopType.Mounted:
				return BanditTypes.lowestMountedTier;
			case TroopType.HorseArcher:
				return BanditTypes.lowestHorseArcherTier;
			default:
				return 0;
			}
		}

		// Token: 0x0600013F RID: 319 RVA: 0x00006CFC File Offset: 0x00004EFC
		private static void initialize()
		{
			BanditTypes.calculateLowestTiers();
		}

		// Token: 0x06000140 RID: 320 RVA: 0x00006D04 File Offset: 0x00004F04
		private static void calculateLowestTiers()
		{
			if (BanditTypes.initialized)
			{
				return;
			}
			BanditTypes.lowestMeleeTier = 100;
			BanditTypes.lowestRangedTier = 100;
			BanditTypes.lowestMountedTier = 100;
			BanditTypes.lowestHorseArcherTier = 100;
			foreach (string text in BanditTypes.bandits)
			{
				CharacterObject characterObject = BanditTypes.find(text);
				if (characterObject == null || characterObject.Age <= 0f)
				{
					throw new Exception("Uninitialized object for troop " + text);
				}
				TroopType troopType = TroopReader.troopTypeOf(characterObject);
				int tier = characterObject.Tier;
				switch (troopType)
				{
				case TroopType.Melee:
					if (tier < BanditTypes.lowestMeleeTier)
					{
						BanditTypes.lowestMeleeTier = tier;
					}
					break;
				case TroopType.Ranged:
					if (tier < BanditTypes.lowestRangedTier)
					{
						BanditTypes.lowestRangedTier = tier;
					}
					break;
				case TroopType.Mounted:
					if (tier < BanditTypes.lowestMountedTier)
					{
						BanditTypes.lowestMountedTier = tier;
					}
					break;
				case TroopType.HorseArcher:
					if (tier < BanditTypes.lowestHorseArcherTier)
					{
						BanditTypes.lowestHorseArcherTier = tier;
					}
					break;
				}
			}
			BanditTypes.initialized = true;
		}

		// Token: 0x06000141 RID: 321 RVA: 0x00006E18 File Offset: 0x00005018
		private static bool isValid(CharacterObject ch)
		{
			return ch != null && ch.Age > 0f;
		}

		// Token: 0x06000142 RID: 322 RVA: 0x00006E30 File Offset: 0x00005030
		private static List<CharacterObject> findAllMatches(Troop troop, List<string> cultureIds, List<string> troopIds)
		{
			List<CharacterObject> list = new List<CharacterObject>();
			foreach (string stringId in troopIds)
			{
				CharacterObject characterObject = BanditTypes.find(stringId);
				if (BanditTypes.isValid(characterObject) && BanditTypes.matches(troop, characterObject, cultureIds))
				{
					list.Add(characterObject);
				}
			}
			return list;
		}

		// Token: 0x06000143 RID: 323 RVA: 0x00006E9C File Offset: 0x0000509C
		private static List<CharacterObject> findAllMatches(Troop troop, List<CharacterObject> characters)
		{
			List<CharacterObject> list = new List<CharacterObject>();
			foreach (CharacterObject characterObject in characters)
			{
				if (TroopReader.troopTypeOf(characterObject) == troop.type && characterObject.Tier == troop.tier)
				{
					list.Add(characterObject);
				}
			}
			return list;
		}

		// Token: 0x06000144 RID: 324 RVA: 0x00006F10 File Offset: 0x00005110
		private static bool matches(Troop troop, CharacterObject obj, List<string> cultureIds)
		{
			return BanditTypes.isValid(obj) && TroopReader.troopTypeOf(obj) == troop.type && obj.Tier == troop.tier && cultureIds.Contains(obj.Culture.StringId);
		}

		// Token: 0x06000145 RID: 325 RVA: 0x00006F5D File Offset: 0x0000515D
		private static CharacterObject find(string stringId)
		{
			CharacterObject characterObject = CharacterObject.Find(stringId);
			BanditTypes.differingCategories(characterObject);
			return characterObject;
		}

		// Token: 0x06000146 RID: 326 RVA: 0x00006F6C File Offset: 0x0000516C
		private static bool differingCategories(CharacterObject character)
		{
			Dictionary<ItemCategory, float> dictionary = new Dictionary<ItemCategory, float>();
			foreach (ItemCategory key in ItemCategories.All)
			{
				dictionary[key] = 0f;
			}
			Equipment equipment = character.Equipment;
			for (int i = 0; i < 12; i++)
			{
				EquipmentElement equipmentElement = equipment[i];
				if (equipmentElement.Item != null)
				{
					ItemCategory itemCategory = equipmentElement.Item.GetItemCategory();
					if (!dictionary.ContainsKey(itemCategory))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0400005B RID: 91
		private static readonly List<string> bandits = new List<string>
		{
			"desert_bandits_bandit",
			"desert_bandits_boss",
			"desert_bandits_chief",
			"desert_bandits_raider",
			"forest_bandits_bandit",
			"forest_bandits_boss",
			"forest_bandits_chief",
			"forest_bandits_raider",
			"looter",
			"deserter",
			"mounted_pillager",
			"mounted_ransacker",
			"mountain_bandits_bandit",
			"mountain_bandits_chief",
			"mountain_bandits_raider",
			"sea_raiders_bandit",
			"sea_raiders_chief",
			"sea_raiders_raider",
			"steppe_bandits_bandit",
			"battanian_raider",
			"sturgian_brigand",
			"sturgian_hardened_brigand",
			"sturgian_horse_raider",
			"khuzait_nomad",
			"khuzait_tribal_warrior",
			"khuzait_raider"
		};

		// Token: 0x0400005C RID: 92
		private static readonly List<string> regulars = new List<string>
		{
			"battanian_volunteer",
			"battanian_clanwarrior",
			"battanian_trained_warrior",
			"battanian_picked_warrior",
			"battanian_oathsworn",
			"battanian_scout",
			"battanian_mounted_skirmisher",
			"battanian_horseman",
			"battanian_woodrunner",
			"battanian_skirmisher",
			"battanian_wildling",
			"battanian_falxman",
			"battanian_veteran_skirmisher",
			"battanian_veteran_falxman",
			"battanian_highborn_youth",
			"battanian_highborn_warrior",
			"battanian_hero",
			"battanian_fian",
			"battanian_fian_champion",
			"wolfskins_tier_1",
			"wolfskins_tier_2",
			"wolfskins_tier_3",
			"battanian_militia_archer",
			"battanian_militia_veteran_archer",
			"battanian_militia_spearman",
			"battanian_militia_veteran_spearman",
			"vlandian_recruit",
			"vlandian_footman",
			"vlandian_spearman",
			"vlandian_billman",
			"vlandian_voulgier",
			"vlandian_pikeman",
			"vlandian_infantry",
			"vlandian_swordsman",
			"vlandian_sergeant",
			"vlandian_light_cavalry",
			"vlandian_cavalry",
			"vlandian_vanguard",
			"vlandian_levy_crossbowman",
			"vlandian_crossbowman",
			"vlandian_hardened_crossbowman",
			"vlandian_sharpshooter",
			"vlandian_squire",
			"vlandian_gallant",
			"vlandian_knight",
			"vlandian_champion",
			"vlandian_banner_knight",
			"vlandian_militia_archer",
			"vlandian_militia_veteran_archer",
			"vlandian_militia_spearman",
			"vlandian_militia_veteran_spearman",
			"imperial_recruit",
			"imperial_infantryman",
			"imperial_vigla_recruit",
			"imperial_equite",
			"imperial_heavy_horseman",
			"imperial_cataphract",
			"imperial_elite_cataphract",
			"imperial_trained_infantryman",
			"imperial_veteran_infantryman",
			"imperial_legionary",
			"imperial_archer",
			"bucellarii",
			"imperial_trained_archer",
			"imperial_veteran_archer",
			"imperial_palatine_guard",
			"imperial_menavliaton",
			"imperial_elite_menavliaton",
			"imperial_crossbowman",
			"imperial_sergeant_crossbowman",
			"eleftheroi_tier_1",
			"eleftheroi_tier_2",
			"eleftheroi_tier_3",
			"imperial_militia_archer",
			"imperial_militia_veteran_archer",
			"imperial_militia_spearman",
			"imperial_militia_veteran_spearman",
			"sturgian_recruit",
			"sturgian_warrior",
			"sturgian_soldier",
			"sturgian_shock_troop",
			"sturgian_veteran_warrior",
			"sturgian_warrior_son",
			"varyag",
			"varyag_veteran",
			"druzhinnik",
			"druzhinnik_champion",
			"sturgian_woodsman",
			"sturgian_hunter",
			"sturgian_archer",
			"sturgian_veteran_bowman",
			"sturgian_berzerker",
			"sturgian_ulfhednar",
			"sturgian_spearman",
			"lakepike_tier_1",
			"lakepike_tier_2",
			"lakepike_tier_3",
			"forest_people_tier_1",
			"forest_people_tier_2",
			"forest_people_tier_3",
			"sturgian_militia_archer",
			"sturgian_militia_veteran_archer",
			"sturgian_militia_spearman",
			"sturgian_militia_veteran_spearman",
			"aserai_recruit",
			"aserai_tribesman",
			"aserai_footman",
			"aserai_skirmisher",
			"aserai_archer",
			"aserai_master_archer",
			"aserai_infantry",
			"aserai_veteran_infantry",
			"aserai_mameluke_soldier",
			"aserai_mameluke_regular",
			"aserai_mameluke_cavalry",
			"aserai_mameluke_heavy_cavalry",
			"aserai_mameluke_axeman",
			"aserai_mameluke_guard",
			"mamluke_palace_guard",
			"aserai_youth",
			"aserai_tribal_horseman",
			"aserai_faris",
			"aserai_veteran_faris",
			"aserai_vanguard_faris",
			"aserai_militia_archer",
			"aserai_militia_veteran_archer",
			"aserai_militia_spearman",
			"aserai_militia_veteran_spearman",
			"aserai_tribal_horseman",
			"khuzait_footman",
			"khuzait_noble_son",
			"khuzait_hunter",
			"khuzait_spearman",
			"khuzait_horseman",
			"khuzait_qanqli",
			"khuzait_archer",
			"khuzait_spear_infantry",
			"khuzait_horse_archer",
			"khuzait_lancer",
			"khuzait_torguud",
			"khuzait_marksman",
			"khuzait_darkhan",
			"khuzait_heavy_horse_archer",
			"khuzait_heavy_lancer",
			"khuzait_kheshig",
			"khuzait_khans_guard",
			"khuzait_militia_archer",
			"khuzait_militia_veteran_archer",
			"khuzait_militia_spearman",
			"khuzait_militia_veteran_spearman"
		};

		// Token: 0x0400005D RID: 93
		private static bool initialized = false;

		// Token: 0x0400005E RID: 94
		private static int lowestMeleeTier = 0;

		// Token: 0x0400005F RID: 95
		private static int lowestRangedTier = 0;

		// Token: 0x04000060 RID: 96
		private static int lowestMountedTier = 0;

		// Token: 0x04000061 RID: 97
		private static int lowestHorseArcherTier = 0;
	}
}
