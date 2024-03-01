using System;
using System.Collections.Generic;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.ViewModelCollection.Scoreboard;

namespace BattleStats
{
	// Token: 0x02000002 RID: 2
	[HarmonyPatch(typeof(ScoreboardBaseVM))]
	[HarmonyPatch("UpdateQuitText")]
	public class BattleStatsBehavior
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002048 File Offset: 0x00000248
		[HarmonyPostfix]
		public static void PostFix(ScoreboardBaseVM __instance)
		{
			bool isOver = __instance.IsOver;
			if (isOver)
			{
				BattleStatsBehavior.GetStatsFromBattle(__instance);
			}
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002068 File Offset: 0x00000268
		private static void GetStatsFromBattle(ScoreboardBaseVM scoreboard)
		{
			List<SPScoreboardUnitVM> list = new List<SPScoreboardUnitVM>();
			List<SPScoreboardUnitVM> list2 = new List<SPScoreboardUnitVM>();
			List<SPScoreboardUnitVM> list3 = new List<SPScoreboardUnitVM>();
			List<SPScoreboardUnitVM> list4 = new List<SPScoreboardUnitVM>();
			List<SPScoreboardUnitVM> list5 = new List<SPScoreboardUnitVM>();
			Dictionary<string, List<SPScoreboardUnitVM>> dictionary = new Dictionary<string, List<SPScoreboardUnitVM>>();
			int enemyKills = 0;
			int num = 0;
			if (PartyBase.MainParty.Side.ToString().Equals("Attacker"))
			{
				foreach (SPScoreboardPartyVM spscoreboardPartyVM in scoreboard.Attackers.Parties)
				{
					if (spscoreboardPartyVM.BattleCombatant.Banner == Hero.MainHero.ClanBanner)
					{
						using (IEnumerator<SPScoreboardUnitVM> enumerator2 = spscoreboardPartyVM.Members.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								SPScoreboardUnitVM spscoreboardUnitVM = enumerator2.Current;
								if (spscoreboardUnitVM.IsHero)
								{
									list.Add(spscoreboardUnitVM);
								}
								else if (spscoreboardUnitVM.Character.IsMounted && spscoreboardUnitVM.Character.IsRanged)
								{
									list5.Add(spscoreboardUnitVM);
								}
								else if (spscoreboardUnitVM.Character.IsMounted && !spscoreboardUnitVM.Character.IsRanged)
								{
									list4.Add(spscoreboardUnitVM);
								}
								else if (spscoreboardUnitVM.Character.IsRanged && !spscoreboardUnitVM.Character.IsMounted)
								{
									list3.Add(spscoreboardUnitVM);
								}
								else if (spscoreboardUnitVM.Character.IsInfantry)
								{
									list2.Add(spscoreboardUnitVM);
								}
							}
							continue;
						}
					}
					num += spscoreboardPartyVM.Score.Dead + spscoreboardPartyVM.Score.Wounded;
				}
				enemyKills = scoreboard.Defenders.Score.Kill;
			}
			else if (PartyBase.MainParty.Side.ToString().Equals("Defender"))
			{
				foreach (SPScoreboardPartyVM spscoreboardPartyVM2 in scoreboard.Defenders.Parties)
				{
					if (spscoreboardPartyVM2.BattleCombatant.Banner == Hero.MainHero.ClanBanner)
					{
						using (IEnumerator<SPScoreboardUnitVM> enumerator4 = spscoreboardPartyVM2.Members.GetEnumerator())
						{
							while (enumerator4.MoveNext())
							{
								SPScoreboardUnitVM spscoreboardUnitVM2 = enumerator4.Current;
								if (spscoreboardUnitVM2.IsHero)
								{
									list.Add(spscoreboardUnitVM2);
								}
								else if (spscoreboardUnitVM2.Character.IsMounted && spscoreboardUnitVM2.Character.IsRanged)
								{
									list5.Add(spscoreboardUnitVM2);
								}
								else if (spscoreboardUnitVM2.Character.IsMounted && !spscoreboardUnitVM2.Character.IsRanged)
								{
									list4.Add(spscoreboardUnitVM2);
								}
								else if (spscoreboardUnitVM2.Character.IsRanged && !spscoreboardUnitVM2.Character.IsMounted)
								{
									list3.Add(spscoreboardUnitVM2);
								}
								else if (spscoreboardUnitVM2.Character.IsInfantry)
								{
									list2.Add(spscoreboardUnitVM2);
								}
							}
							continue;
						}
					}
					num += spscoreboardPartyVM2.Score.Dead + spscoreboardPartyVM2.Score.Wounded;
				}
				enemyKills = scoreboard.Attackers.Score.Kill;
			}
			if (!list2.IsEmpty<SPScoreboardUnitVM>())
			{
				dictionary.Add("Infantry", list2);
			}
			if (!list3.IsEmpty<SPScoreboardUnitVM>())
			{
				dictionary.Add("Ranged", list3);
			}
			if (!list4.IsEmpty<SPScoreboardUnitVM>())
			{
				dictionary.Add("Cavalry", list4);
			}
			if (!list5.IsEmpty<SPScoreboardUnitVM>())
			{
				dictionary.Add("Horse Archers", list5);
			}
			if (!list.IsEmpty<SPScoreboardUnitVM>())
			{
				BattleStatsBehavior.UpdateHeroRecords(list);
			}
			if (!dictionary.IsEmpty<KeyValuePair<string, List<SPScoreboardUnitVM>>>())
			{
				BattleStatsBehavior.UpdateArmyRecords(dictionary, enemyKills, num);
			}
		}

		// Token: 0x06000003 RID: 3 RVA: 0x00002488 File Offset: 0x00000688
		private static void UpdateHeroRecords(List<SPScoreboardUnitVM> clanHeros)
		{
			foreach (SPScoreboardUnitVM spscoreboardUnitVM in clanHeros)
			{
				string nameText = spscoreboardUnitVM.Score.NameText;
				HeroRecords heroRecords = new HeroRecords
				{
					Name = nameText,
					Kills = spscoreboardUnitVM.Score.Kill,
					PR = spscoreboardUnitVM.Score.Kill,
					KB = (double)spscoreboardUnitVM.Score.Kill,
					Wounds = spscoreboardUnitVM.Score.Wounded,
					Dead = spscoreboardUnitVM.Score.Dead,
					Battles = 1
				};
				if (!BattleStatsBehavior.heroRecords.IsEmpty<KeyValuePair<string, HeroRecords>>())
				{
					if (BattleStatsBehavior.heroRecords.ContainsKey(nameText))
					{
						BattleStatsBehavior.heroRecords[nameText].Kills += heroRecords.Kills;
						if (BattleStatsBehavior.heroRecords[nameText].PR < heroRecords.Kills)
						{
							BattleStatsBehavior.heroRecords[nameText].PR = heroRecords.Kills;
						}
						BattleStatsBehavior.heroRecords[nameText].Wounds += heroRecords.Wounds;
						BattleStatsBehavior.heroRecords[nameText].Battles++;
						BattleStatsBehavior.heroRecords[nameText].KB = Math.Round((double)BattleStatsBehavior.heroRecords[nameText].Kills / (double)BattleStatsBehavior.heroRecords[nameText].Battles, 2);
					}
					else
					{
						BattleStatsBehavior.heroRecords.Add(nameText, heroRecords);
					}
				}
				else
				{
					BattleStatsBehavior.heroRecords.Add(nameText, heroRecords);
				}
			}
		}

		// Token: 0x06000004 RID: 4 RVA: 0x0000263C File Offset: 0x0000083C
		private static void UpdateArmyRecords(Dictionary<string, List<SPScoreboardUnitVM>> formations, int enemyKills, int allyCasualties)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			foreach (KeyValuePair<string, List<SPScoreboardUnitVM>> keyValuePair in formations)
			{
				string key = keyValuePair.Key;
				int num4 = 0;
				int num5 = 0;
				int num6 = 0;
				foreach (SPScoreboardUnitVM spscoreboardUnitVM in keyValuePair.Value)
				{
					num4 += spscoreboardUnitVM.Score.Kill;
					num5 += spscoreboardUnitVM.Score.Wounded;
					num6 += spscoreboardUnitVM.Score.Dead;
				}
				num += num4;
				num2 += num5;
				num3 += num6;
				ArmyRecords value = new ArmyRecords
				{
					Name = key,
					Kills = num4,
					PR = num4,
					KB = (double)num4,
					Wounded = num5,
					Casualties = num6,
					WB = (double)num5,
					CB = (double)num6,
					Battles = 1
				};
				if (!BattleStatsBehavior.armyRecords.IsEmpty<KeyValuePair<string, ArmyRecords>>())
				{
					if (BattleStatsBehavior.armyRecords.ContainsKey(key) && !key.Equals("Army Totals"))
					{
						BattleStatsBehavior.armyRecords[key].Kills += num4;
						if (BattleStatsBehavior.armyRecords[key].PR < num4)
						{
							BattleStatsBehavior.armyRecords[key].PR = num4;
						}
						BattleStatsBehavior.armyRecords[key].Wounded += num5;
						BattleStatsBehavior.armyRecords[key].Casualties += num6;
						BattleStatsBehavior.armyRecords[key].Battles++;
						BattleStatsBehavior.armyRecords[key].KB = Math.Round((double)BattleStatsBehavior.armyRecords[key].Kills / (double)BattleStatsBehavior.armyRecords[key].Battles, 2);
						BattleStatsBehavior.armyRecords[key].WB = Math.Round((double)BattleStatsBehavior.armyRecords[key].Wounded / (double)BattleStatsBehavior.armyRecords[key].Battles, 2);
						BattleStatsBehavior.armyRecords[key].CB = Math.Round((double)BattleStatsBehavior.armyRecords[key].Casualties / (double)BattleStatsBehavior.armyRecords[key].Battles, 2);
					}
					else
					{
						BattleStatsBehavior.armyRecords.Add(key, value);
					}
				}
				else
				{
					BattleStatsBehavior.armyRecords.Add(key, value);
				}
			}
			if (BattleStatsBehavior.armyRecords.ContainsKey("Army Totals"))
			{
				BattleStatsBehavior.armyRecords["Army Totals"].Battles++;
			}
			else
			{
				ArmyRecords value2 = new ArmyRecords
				{
					Name = "Army Totals",
					Battles = 1
				};
				BattleStatsBehavior.armyRecords.Add("Army Totals", value2);
			}
			BattleStatsBehavior.armyRecords["Army Totals"].Kills += num;
			if (BattleStatsBehavior.armyRecords["Army Totals"].PR < num)
			{
				BattleStatsBehavior.armyRecords["Army Totals"].PR = num;
			}
			BattleStatsBehavior.armyRecords["Army Totals"].Wounded += num2;
			BattleStatsBehavior.armyRecords["Army Totals"].Casualties += num3;
			BattleStatsBehavior.armyRecords["Army Totals"].KB = Math.Round((double)BattleStatsBehavior.armyRecords["Army Totals"].Kills / (double)BattleStatsBehavior.armyRecords["Army Totals"].Battles, 2);
			BattleStatsBehavior.armyRecords["Army Totals"].WB = Math.Round((double)BattleStatsBehavior.armyRecords["Army Totals"].Wounded / (double)BattleStatsBehavior.armyRecords["Army Totals"].Battles, 2);
			BattleStatsBehavior.armyRecords["Army Totals"].CB = Math.Round((double)BattleStatsBehavior.armyRecords["Army Totals"].Casualties / (double)BattleStatsBehavior.armyRecords["Army Totals"].Battles, 2);
			if (num3 + num2 + allyCasualties > enemyKills)
			{
				int num7 = num3 + num2 - (enemyKills - allyCasualties);
				BattleStatsBehavior.armyRecords["Army Totals"].FK += num7;
			}
		}

		// Token: 0x04000001 RID: 1
		public static Dictionary<string, HeroRecords> heroRecords = new Dictionary<string, HeroRecords>();

		// Token: 0x04000002 RID: 2
		public static Dictionary<string, ArmyRecords> armyRecords = new Dictionary<string, ArmyRecords>();
	}
}
