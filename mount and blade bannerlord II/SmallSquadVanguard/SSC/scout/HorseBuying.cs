using System;
using SSC.misc;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace SSC.scout
{
	// Token: 0x02000032 RID: 50
	internal static class HorseBuying
	{
		// Token: 0x060001DF RID: 479 RVA: 0x0000A784 File Offset: 0x00008984
		public static void BuyHorses(MobileParty mobileParty, Town town, float budget, int maxPricePerHorse, int maxHorsesNeeded)
		{
			int num = 0;
			int num2 = 4;
			bool flag = false;
			for (int i = 0; i < num2; i++)
			{
				if (num >= maxHorsesNeeded)
				{
					Message.display(HorseBuying.leaderName(mobileParty) + " has enough horses.");
					break;
				}
				int num3 = HorseBuying.findBestHorseIndex(town, mobileParty, maxPricePerHorse);
				if (num3 == -1)
				{
					break;
				}
				flag = true;
				ItemRosterElement elementCopyAtIndex = town.Owner.ItemRoster.GetElementCopyAtIndex(num3);
				num += HorseBuying.buyHorses(mobileParty, town, budget, maxHorsesNeeded, num, elementCopyAtIndex);
			}
			if (!flag && num < maxHorsesNeeded)
			{
				Message.display(HorseBuying.leaderName(mobileParty) + ": There were no horses at 600 or less.");
			}
		}

		// Token: 0x060001E0 RID: 480 RVA: 0x0000A811 File Offset: 0x00008A11
		private static string leaderName(MobileParty party)
		{
			string text;
			if (party == null)
			{
				text = null;
			}
			else
			{
				Hero leaderHero = party.LeaderHero;
				if (leaderHero == null)
				{
					text = null;
				}
				else
				{
					TextObject name = leaderHero.Name;
					text = ((name != null) ? name.ToString() : null);
				}
			}
			return text ?? "";
		}

		// Token: 0x060001E1 RID: 481 RVA: 0x0000A840 File Offset: 0x00008A40
		private static int findBestHorseIndex(Town town, MobileParty party, int maxPricePerHorse)
		{
			int result = -1;
			int num = 100000;
			ItemRoster itemRoster = town.Owner.ItemRoster;
			for (int i = 0; i < itemRoster.Count; i++)
			{
				ItemObject itemAtIndex = itemRoster.GetItemAtIndex(i);
				if ((itemAtIndex.ItemCategory == DefaultItemCategories.Horse || itemAtIndex.ItemCategory == DefaultItemCategories.WarHorse) && itemAtIndex.IsMountable)
				{
					int itemPrice = town.GetItemPrice(itemRoster.GetElementCopyAtIndex(i).EquipmentElement, party, false);
					if (itemPrice < num && itemPrice <= maxPricePerHorse)
					{
						num = itemPrice;
						result = i;
					}
				}
			}
			return result;
		}

		// Token: 0x060001E2 RID: 482 RVA: 0x0000A8D0 File Offset: 0x00008AD0
		private static int buyHorses(MobileParty mobileParty, Town town, float budget, int maxHorsesNeeded, int totalHorsesBought, ItemRosterElement bestHorse)
		{
			int itemPrice = town.GetItemPrice(bestHorse.EquipmentElement, mobileParty, false);
			int num = bestHorse.Amount;
			if ((double)(num * itemPrice) > (double)budget)
			{
				num = MathF.Floor(budget / (float)itemPrice);
			}
			if (num + totalHorsesBought > maxHorsesNeeded)
			{
				num = maxHorsesNeeded - totalHorsesBought;
			}
			if (num > 0)
			{
				SellItemsAction.Apply(town.Owner, mobileParty.Party, bestHorse, num, town.Owner.Settlement);
				HorseBuying.displayMessage(HorseBuying.leaderName(mobileParty), bestHorse, num, (float)itemPrice);
				totalHorsesBought += num;
			}
			return totalHorsesBought;
		}

		// Token: 0x060001E3 RID: 483 RVA: 0x0000A950 File Offset: 0x00008B50
		private static void displayMessage(string hero, ItemRosterElement bestHorse, int availableHorses, float price)
		{
			ItemObject item = bestHorse.EquipmentElement.Item;
			string text;
			if (item == null)
			{
				text = null;
			}
			else
			{
				TextObject name = item.Name;
				text = ((name != null) ? name.ToString() : null);
			}
			string text2 = text ?? "";
			if (!hero.IsEmpty<char>() && !text2.IsEmpty<char>())
			{
				string text3 = "{GOLD_ICON}";
				string text4 = (availableHorses > 1) ? "each." : "";
				Message.display(new TextObject(string.Format("{0} bought {1} {2} at {3}{4}{5}", new object[]
				{
					hero,
					availableHorses,
					text2,
					price,
					text3,
					text4
				}), null).ToString());
			}
		}
	}
}
