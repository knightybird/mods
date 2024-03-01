using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace BattleStats
{
	// Token: 0x02000005 RID: 5
	public static class MenuSetup
	{
		// Token: 0x06000009 RID: 9 RVA: 0x00002B24 File Offset: 0x00000D24
		public static void ShowMenu()
		{
			bool gameStarted = Campaign.Current.GameStarted;
			if (gameStarted && (Input.IsKeyDown(InputKey.LeftControl) || Input.IsKeyDown(InputKey.RightControl)) && Input.IsKeyPressed(MenuSetup.hotkey))
			{
				File.Delete(SaveLoadRecords.file);
				MenuSetup.sortedHeroRecords.Clear();
				MenuSetup.sortedArmyRecords.Clear();
				BattleStatsBehavior.heroRecords.Clear();
				BattleStatsBehavior.armyRecords.Clear();
				return;
			}
			if (gameStarted && Input.IsKeyPressed(MenuSetup.hotkey))
			{
				// MenuSetup.RemoveNonClanMembers();
				// MenuSetup.SortRecordsByKills();
				MenuSetup.SortRecordsByDeaths();
				MenuSetup.ShowMenuPage(1);
			}
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00002BB4 File Offset: 0x00000DB4
		private static bool IsClanMember(string name)
		{
			foreach (Hero hero in Clan.PlayerClan.Heroes)
			{
				if (name.Equals(hero.Name.ToString()) && hero.IsAlive)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002C28 File Offset: 0x00000E28
		private static void RemoveNonClanMembers()
		{
			if (!BattleStatsBehavior.heroRecords.IsEmpty<KeyValuePair<string, HeroRecords>>())
			{
				List<KeyValuePair<string, HeroRecords>> list = (from x in BattleStatsBehavior.heroRecords
				where !MenuSetup.IsClanMember(x.Key)
				select x).ToList<KeyValuePair<string, HeroRecords>>();
				foreach (KeyValuePair<string, HeroRecords> keyValuePair in list)
				{
					BattleStatsBehavior.heroRecords.Remove(keyValuePair.Key);
				}
			}
		}

		// Token: 0x0600000C RID: 12 RVA: 0x00002CBC File Offset: 0x00000EBC
		public static void SortRecordsByKills()
		{
			if (!BattleStatsBehavior.heroRecords.IsEmpty<KeyValuePair<string, HeroRecords>>())
			{
				Dictionary<string, HeroRecords>.ValueCollection values = BattleStatsBehavior.heroRecords.Values;
				MenuSetup.sortedHeroRecords = values.ToList<HeroRecords>();
				MenuSetup.sortedHeroRecords.Sort((HeroRecords x, HeroRecords y) => y.Kills.CompareTo(x.Kills));
			}
			if (!BattleStatsBehavior.armyRecords.IsEmpty<KeyValuePair<string, ArmyRecords>>())
			{
				MenuSetup.sortedArmyRecords = new List<ArmyRecords>();
				List<ArmyRecords> list = new List<ArmyRecords>();
				Dictionary<string, ArmyRecords>.ValueCollection values2 = BattleStatsBehavior.armyRecords.Values;
				list = values2.ToList<ArmyRecords>();
				list.Sort((ArmyRecords x, ArmyRecords y) => y.Kills.CompareTo(x.Kills));
				for (int i = 0; i < list.Count; i++)
				{
					if (!list.ElementAt(i).Name.Equals("Army Totals"))
					{
						MenuSetup.sortedArmyRecords.Add(list.ElementAt(i));
					}
				}
				MenuSetup.sortedArmyRecords.Add(list.Find((ArmyRecords x) => x.Name.Equals("Army Totals")));
			}
		}
		
		public static void SortRecordsByDeaths()
		{
			if (!BattleStatsBehavior.heroRecords.IsEmpty<KeyValuePair<string, HeroRecords>>())
			{
				Dictionary<string, HeroRecords>.ValueCollection values = BattleStatsBehavior.heroRecords.Values;
				MenuSetup.sortedHeroRecords = values.ToList<HeroRecords>();
				MenuSetup.sortedHeroRecords.Sort((HeroRecords x, HeroRecords y) => x.Dead.CompareTo(y.Dead));
			}
		}

		// Token: 0x0600000D RID: 13 RVA: 0x00002DD4 File Offset: 0x00000FD4
		private static void ShowMenuPage(int pageNum)
		{
			int num = MenuSetup.sortedHeroRecords.Count + MenuSetup.sortedArmyRecords.Count;
			if (MenuSetup.changeFormat)
			{
				if (MenuSetup.changeFormat)
				{
					switch (pageNum)
					{
					case 1:
						if (num > 8)
						{
							InformationManager.ShowInquiry(new InquiryData("Battle Stats", MenuSetup.StatsView(1), true, true, "Ok", "Next", null, delegate()
							{
								MenuSetup.ShowMenuPage(2);
							}, "", 0f, null, null, null), false, false);
							return;
						}
						InformationManager.ShowInquiry(new InquiryData("Battle Stats", MenuSetup.StatsView(1), true, false, "Ok", "", null, null, "", 0f, null, null, null), false, false);
						return;
					case 2:
						if (num > 16)
						{
							InformationManager.ShowInquiry(new InquiryData("Battle Stats", MenuSetup.StatsView(2), true, true, "Ok", "Next", null, delegate()
							{
								MenuSetup.ShowMenuPage(3);
							}, "", 0f, null, null, null), false, false);
							return;
						}
						InformationManager.ShowInquiry(new InquiryData("Battle Stats", MenuSetup.StatsView(2), true, true, "Ok", "Back", null, delegate()
						{
							MenuSetup.ShowMenuPage(1);
						}, "", 0f, null, null, null), false, false);
						return;
					case 3:
						if (num > 24)
						{
							InformationManager.ShowInquiry(new InquiryData("Battle Stats", MenuSetup.StatsView(3), true, true, "Ok", "Next", null, delegate()
							{
								MenuSetup.ShowMenuPage(4);
							}, "", 0f, null, null, null), false, false);
							return;
						}
						InformationManager.ShowInquiry(new InquiryData("Battle Stats", MenuSetup.StatsView(3), true, true, "Ok", "Back", null, delegate()
						{
							MenuSetup.ShowMenuPage(1);
						}, "", 0f, null, null, null), false, false);
						return;
					case 4:
						if (num > 32)
						{
							InformationManager.ShowInquiry(new InquiryData("Battle Stats", MenuSetup.StatsView(4), true, true, "Ok", "Next", null, delegate()
							{
								MenuSetup.ShowMenuPage(5);
							}, "", 0f, null, null, null), false, false);
							return;
						}
						InformationManager.ShowInquiry(new InquiryData("Battle Stats", MenuSetup.StatsView(4), true, true, "Ok", "Back", null, delegate()
						{
							MenuSetup.ShowMenuPage(1);
						}, "", 0f, null, null, null), false, false);
						return;
					case 5:
						InformationManager.ShowInquiry(new InquiryData("Battle Stats", MenuSetup.StatsView(5), true, true, "Ok", "Back", null, delegate()
						{
							MenuSetup.ShowMenuPage(1);
						}, "", 0f, null, null, null), false, false);
						break;
					default:
						return;
					}
				}
				return;
			}
			switch (pageNum)
			{
			case 1:
				if (num > 10)
				{
					InformationManager.ShowInquiry(new InquiryData("Battle Stats", MenuSetup.StatsView(1), true, true, "Ok", "Next", null, delegate()
					{
						MenuSetup.ShowMenuPage(2);
					}, "", 0f, null, null, null), false, false);
					return;
				}
				InformationManager.ShowInquiry(new InquiryData("Battle Stats", MenuSetup.StatsView(1), true, false, "Ok", "", null, null, "", 0f, null, null, null), false, false);
				return;
			case 2:
				if (num > 20)
				{
					InformationManager.ShowInquiry(new InquiryData("Battle Stats", MenuSetup.StatsView(2), true, true, "Ok", "Next", null, delegate()
					{
						MenuSetup.ShowMenuPage(3);
					}, "", 0f, null, null, null), false, false);
					return;
				}
				InformationManager.ShowInquiry(new InquiryData("Battle Stats", MenuSetup.StatsView(2), true, true, "Ok", "Back", null, delegate()
				{
					MenuSetup.ShowMenuPage(1);
				}, "", 0f, null, null, null), false, false);
				return;
			case 3:
				if (num > 30)
				{
					InformationManager.ShowInquiry(new InquiryData("Battle Stats", MenuSetup.StatsView(3), true, true, "Ok", "Next", null, delegate()
					{
						MenuSetup.ShowMenuPage(4);
					}, "", 0f, null, null, null), false, false);
					return;
				}
				InformationManager.ShowInquiry(new InquiryData("Battle Stats", MenuSetup.StatsView(3), true, true, "Ok", "Back", null, delegate()
				{
					MenuSetup.ShowMenuPage(1);
				}, "", 0f, null, null, null), false, false);
				return;
			case 4:
				if (num > 40)
				{
					InformationManager.ShowInquiry(new InquiryData("Battle Stats", MenuSetup.StatsView(4), true, true, "Ok", "Next", null, delegate()
					{
						MenuSetup.ShowMenuPage(5);
					}, "", 0f, null, null, null), false, false);
					return;
				}
				InformationManager.ShowInquiry(new InquiryData("Battle Stats", MenuSetup.StatsView(4), true, true, "Ok", "Back", null, delegate()
				{
					MenuSetup.ShowMenuPage(1);
				}, "", 0f, null, null, null), false, false);
				return;
			case 5:
				InformationManager.ShowInquiry(new InquiryData("Battle Stats", MenuSetup.StatsView(5), true, true, "Ok", "Back", null, delegate()
				{
					MenuSetup.ShowMenuPage(1);
				}, "", 0f, null, null, null), false, false);
				return;
			default:
				return;
			}
		}

		// Token: 0x0600000E RID: 14 RVA: 0x000033E4 File Offset: 0x000015E4
		private static string StatsView(int pageNum)
		{
			string text = string.Empty;
			int num = MenuSetup.sortedHeroRecords.Count + MenuSetup.sortedArmyRecords.Count;
			int count = MenuSetup.sortedHeroRecords.Count;
			bool flag = false;
			if (!MenuSetup.changeFormat)
			{
				switch (pageNum)
				{
				case 1:
				{
					int num2 = 0;
					while (num2 < 10 && num2 < count)
					{
						text = text + "[" + MenuSetup.sortedHeroRecords.ElementAt(num2).Name + "]\n";
						text = string.Concat(new string[]
						{
							text,
							"Kills: ",
							MenuSetup.sortedHeroRecords.ElementAt(num2).Kills.ToString().PadRight(8),
							// " PR: ",
							// MenuSetup.sortedHeroRecords.ElementAt(num2).PR.ToString().PadRight(8),
							// " K/B: ",
							// MenuSetup.sortedHeroRecords.ElementAt(num2).KB.ToString().PadRight(8),
							// " Wounds: ",
							// MenuSetup.sortedHeroRecords.ElementAt(num2).Wounds.ToString().PadRight(8),
							"  Injured: ",
							MenuSetup.sortedHeroRecords.ElementAt(num2).Wounds.ToString(),
							"  Dead: ",
							MenuSetup.sortedHeroRecords.ElementAt(num2).Dead.ToString(),
							"Battles: ",
							MenuSetup.sortedHeroRecords.ElementAt(num2).Battles.ToString(),
							"\n"
						});
						num2++;
					}
					if (num <= 10)
					{
						flag = true;
					}
					break;
				}
				case 2:
				{
					int num3 = 10;
					while (num3 < 20 && num3 < count)
					{
						text = text + "[" + MenuSetup.sortedHeroRecords.ElementAt(num3).Name + "]\n";
						text = string.Concat(new string[]
						{
							text,
							"Kills: ",
							MenuSetup.sortedHeroRecords.ElementAt(num3).Kills.ToString().PadRight(8),
							// "PR: ",
							// MenuSetup.sortedHeroRecords.ElementAt(num3).PR.ToString().PadRight(8),
							// "K/B: ",
							// MenuSetup.sortedHeroRecords.ElementAt(num3).KB.ToString().PadRight(8),
							// "Wounds: ",
							// MenuSetup.sortedHeroRecords.ElementAt(num3).Wounds.ToString().PadRight(8),
							"  Injured: ",
							MenuSetup.sortedHeroRecords.ElementAt(num3).Wounds.ToString(),
							"  Dead: ",
							MenuSetup.sortedHeroRecords.ElementAt(num3).Dead.ToString(),
							"Battles: ",
							MenuSetup.sortedHeroRecords.ElementAt(num3).Battles.ToString(),
							"\n"
						});
						num3++;
					}
					if (num <= 20)
					{
						flag = true;
					}
					break;
				}
				case 3:
				{
					int num4 = 20;
					while (num4 < 30 && num4 < count)
					{
						text = text + "[" + MenuSetup.sortedHeroRecords.ElementAt(num4).Name + "]\n";
						text = string.Concat(new string[]
						{
							text,
							"Kills: ",
							MenuSetup.sortedHeroRecords.ElementAt(num4).Kills.ToString().PadRight(8),
							// "PR: ",
							// MenuSetup.sortedHeroRecords.ElementAt(num4).PR.ToString().PadRight(8),
							// "K/B: ",
							// MenuSetup.sortedHeroRecords.ElementAt(num4).KB.ToString().PadRight(8),
							// "Wounds: ",
							// MenuSetup.sortedHeroRecords.ElementAt(num4).Wounds.ToString().PadRight(8),
							"  Injured: ",
							MenuSetup.sortedHeroRecords.ElementAt(num4).Wounds.ToString(),
							"  Dead: ",
							MenuSetup.sortedHeroRecords.ElementAt(num4).Dead.ToString(),
							"Battles: ",
							MenuSetup.sortedHeroRecords.ElementAt(num4).Battles.ToString(),
							"\n"
						});
						num4++;
					}
					if (num <= 30)
					{
						flag = true;
					}
					break;
				}
				case 4:
				{
					int num5 = 30;
					while (num5 < 40 && num5 < count)
					{
						text = text + "[" + MenuSetup.sortedHeroRecords.ElementAt(num5).Name + "]\n";
						text = string.Concat(new string[]
						{
							text,
							"Kills: ",
							MenuSetup.sortedHeroRecords.ElementAt(num5).Kills.ToString().PadRight(8),
							// "PR: ",
							// MenuSetup.sortedHeroRecords.ElementAt(num5).PR.ToString().PadRight(8),
							// "K/B: ",
							// MenuSetup.sortedHeroRecords.ElementAt(num5).KB.ToString().PadRight(8),
							// "Wounds: ",
							// MenuSetup.sortedHeroRecords.ElementAt(num5).Wounds.ToString().PadRight(8),
							"  Injured: ",
							MenuSetup.sortedHeroRecords.ElementAt(num5).Wounds.ToString(),
							"  Dead: ",
							MenuSetup.sortedHeroRecords.ElementAt(num5).Dead.ToString(),
							"Battles: ",
							MenuSetup.sortedHeroRecords.ElementAt(num5).Battles.ToString(),
							"\n"
						});
						num5++;
					}
					if (num <= 40)
					{
						flag = true;
					}
					break;
				}
				case 5:
					for (int i = 40; i < num; i++)
					{
						text = text + "[" + MenuSetup.sortedHeroRecords.ElementAt(i).Name + "]\n";
						text = string.Concat(new string[]
						{
							text,
							"Kills: ",
							MenuSetup.sortedHeroRecords.ElementAt(i).Kills.ToString().PadRight(8),
							// "PR: ",
							// MenuSetup.sortedHeroRecords.ElementAt(i).PR.ToString().PadRight(8),
							// "K/B: ",
							// MenuSetup.sortedHeroRecords.ElementAt(i).KB.ToString().PadRight(8),
							// "Wounds: ",
							// MenuSetup.sortedHeroRecords.ElementAt(i).Wounds.ToString().PadRight(8),
							"  Injured: ",
							MenuSetup.sortedHeroRecords.ElementAt(i).Wounds.ToString(),
							"  Dead: ",
							MenuSetup.sortedHeroRecords.ElementAt(i).Dead.ToString(),
							"Battles: ",
							MenuSetup.sortedHeroRecords.ElementAt(i).Battles.ToString(),
							"\n"
						});
					}
					flag = true;
					break;
				}
			}
			else if (MenuSetup.changeFormat)
			{
				switch (pageNum)
				{
				case 1:
				{
					int num6 = 0;
					while (num6 < 8 && num6 < count)
					{
						text = text + "[" + MenuSetup.sortedHeroRecords.ElementAt(num6).Name + "]\n";
						text = string.Concat(new string[]
						{
							text,
							"Kills: ",
							MenuSetup.sortedHeroRecords.ElementAt(num6).Kills.ToString(),
							// " PR: ",
							// MenuSetup.sortedHeroRecords.ElementAt(num6).PR.ToString(),
							// " K/B: ",
							// MenuSetup.sortedHeroRecords.ElementAt(num6).KB.ToString(),
							// " Wounds: ",
							// MenuSetup.sortedHeroRecords.ElementAt(num6).Wounds.ToString(),
							"  Injured: ",
							MenuSetup.sortedHeroRecords.ElementAt(num6).Wounds.ToString(),
							"  Dead: ",
							MenuSetup.sortedHeroRecords.ElementAt(num6).Dead.ToString(),
							" Battles: ",
							MenuSetup.sortedHeroRecords.ElementAt(num6).Battles.ToString(),
							"\n"
						});
						num6++;
					}
					if (num <= 8)
					{
						flag = true;
					}
					break;
				}
				case 2:
				{
					int num7 = 8;
					while (num7 < 16 && num7 < count)
					{
						text = text + "[" + MenuSetup.sortedHeroRecords.ElementAt(num7).Name + "]\n";
						text = string.Concat(new string[]
						{
							text,
							"Kills: ",
							MenuSetup.sortedHeroRecords.ElementAt(num7).Kills.ToString().PadRight(8),
							// " PR: ",
							// MenuSetup.sortedHeroRecords.ElementAt(num7).PR.ToString().PadRight(8),
							// " K/B: ",
							// MenuSetup.sortedHeroRecords.ElementAt(num7).KB.ToString().PadRight(8),
							// " Wounds: ",
							// MenuSetup.sortedHeroRecords.ElementAt(num7).Wounds.ToString().PadRight(8),
							"  Injured: ",
							MenuSetup.sortedHeroRecords.ElementAt(num7).Wounds.ToString(),
							"  Dead: ",
							MenuSetup.sortedHeroRecords.ElementAt(num7).Dead.ToString(),
							" Battles: ",
							MenuSetup.sortedHeroRecords.ElementAt(num7).Battles.ToString(),
							"\n"
						});
						num7++;
					}
					if (num <= 16)
					{
						flag = true;
					}
					break;
				}
				case 3:
				{
					int num8 = 16;
					while (num8 < 24 && num8 < count)
					{
						text = text + "[" + MenuSetup.sortedHeroRecords.ElementAt(num8).Name + "]\n";
						text = string.Concat(new string[]
						{
							text,
							"Kills: ",
							MenuSetup.sortedHeroRecords.ElementAt(num8).Kills.ToString(),
							// " PR: ",
							// MenuSetup.sortedHeroRecords.ElementAt(num8).PR.ToString(),
							// " K/B: ",
							// MenuSetup.sortedHeroRecords.ElementAt(num8).KB.ToString(),
							// " Wounds: ",
							// MenuSetup.sortedHeroRecords.ElementAt(num8).Wounds.ToString(),
							"  Injured: ",
							MenuSetup.sortedHeroRecords.ElementAt(num8).Wounds.ToString(),
							"  Dead: ",
							MenuSetup.sortedHeroRecords.ElementAt(num8).Dead.ToString(),
							" Battles: ",
							MenuSetup.sortedHeroRecords.ElementAt(num8).Battles.ToString(),
							"\n"
						});
						num8++;
					}
					if (num <= 24)
					{
						flag = true;
					}
					break;
				}
				case 4:
				{
					int num9 = 24;
					while (num9 < 32 && num9 < count)
					{
						text = text + "[" + MenuSetup.sortedHeroRecords.ElementAt(num9).Name + "]\n";
						text = string.Concat(new string[]
						{
							text,
							"Kills: ",
							MenuSetup.sortedHeroRecords.ElementAt(num9).Kills.ToString(),
							// " PR: ",
							// MenuSetup.sortedHeroRecords.ElementAt(num9).PR.ToString(),
							// " K/B: ",
							// MenuSetup.sortedHeroRecords.ElementAt(num9).KB.ToString(),
							// " Wounds: ",
							// MenuSetup.sortedHeroRecords.ElementAt(num9).Wounds.ToString(),
							"  Injured: ",
							MenuSetup.sortedHeroRecords.ElementAt(num9).Wounds.ToString(),
							"  Dead: ",
							MenuSetup.sortedHeroRecords.ElementAt(num9).Dead.ToString(),
							" Battles: ",
							MenuSetup.sortedHeroRecords.ElementAt(num9).Battles.ToString(),
							"\n"
						});
						num9++;
					}
					if (num <= 32)
					{
						flag = true;
					}
					break;
				}
				case 5:
					for (int j = 32; j < num; j++)
					{
						text = text + "[" + MenuSetup.sortedHeroRecords.ElementAt(j).Name + "]\n";
						text = string.Concat(new string[]
						{
							text,
							"Kills: ",
							MenuSetup.sortedHeroRecords.ElementAt(j).Kills.ToString(),
							// " PR: ",
							// MenuSetup.sortedHeroRecords.ElementAt(j).PR.ToString(),
							// " K/B: ",
							// MenuSetup.sortedHeroRecords.ElementAt(j).KB.ToString(),
							"  Injured: ",
							MenuSetup.sortedHeroRecords.ElementAt(j).Wounds.ToString(),
							"  Dead: ",
							MenuSetup.sortedHeroRecords.ElementAt(j).Dead.ToString(),
							" Battles: ",
							MenuSetup.sortedHeroRecords.ElementAt(j).Battles.ToString(),
							"\n"
						});
					}
					flag = true;
					break;
				}
			}
			if (flag && !MenuSetup.sortedArmyRecords.IsEmpty<ArmyRecords>())
			{
				foreach (ArmyRecords armyRecords in MenuSetup.sortedArmyRecords)
				{
					if (!armyRecords.Name.Equals("Army Totals"))
					{
						text = string.Concat(new string[]
						{
							text,
							"[",
							armyRecords.Name,
							"]\nK: ",
							armyRecords.Kills.ToString(),
							"  Injured: ",
							armyRecords.Wounded.ToString(),
							"  Dead: ",
							armyRecords.Casualties.ToString(),
							//"  PR: ",
							//armyRecords.PR.ToString(),
							//"  K/B: ",
							//armyRecords.KB.ToString(),
							//"  W/B: ",
							// armyRecords.WB.ToString(),
							//"  C/B: ",
							//armyRecords.CB.ToString(),
							"  B: ",
							armyRecords.Battles.ToString(),
							"\n"
						});
					}
					else if (armyRecords.Name.Equals("Army Totals"))
					{
						text = string.Concat(new string[]
						{
							text,
							"[",
							armyRecords.Name,
							"]\nK: ",
							armyRecords.Kills.ToString(),
							"  Injured: ",
							armyRecords.Wounded.ToString(),
							"  Dead: ",
							armyRecords.Casualties.ToString(),
							// "  PR: ",
							// armyRecords.PR.ToString(),
							// "  K/B: ",
							// armyRecords.KB.ToString(),
							// "  W/B: ",
							// armyRecords.WB.ToString(),
							// "  C/B: ",
							// armyRecords.CB.ToString(),
							// "  FK: ",
							// armyRecords.FK.ToString(),
							"  B: ",
							armyRecords.Battles.ToString(),
							"\n"
						});
					}
				}
			}
			return text;
		}

		// Token: 0x0400000B RID: 11
		public static List<HeroRecords> sortedHeroRecords = new List<HeroRecords>();

		// Token: 0x0400000C RID: 12
		public static List<ArmyRecords> sortedArmyRecords = new List<ArmyRecords>();

		// Token: 0x0400000D RID: 13
		public static bool changeFormat;

		// Token: 0x0400000E RID: 14
		public static InputKey hotkey = InputKey.SemiColon;
	}
}
