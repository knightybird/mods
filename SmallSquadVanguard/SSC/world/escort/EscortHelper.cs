using System;
using System.Linq;
using SSC.misc;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace SSC.world.escort
{
	// Token: 0x02000022 RID: 34
	internal class EscortHelper
	{
		// Token: 0x0600018A RID: 394 RVA: 0x00008CE0 File Offset: 0x00006EE0
		public MobileParty SpawnCaravan(Settlement origin)
		{
			ItemRoster itemRoster = new ItemRoster();
			foreach (string objectName in new string[]
			{
				"cotton",
				"velvet",
				"oil",
				"linen",
				"date_fruit"
			})
			{
				ItemObject @object = MBObjectManager.Instance.GetObject<ItemObject>(objectName);
				if (@object != null)
				{
					itemRoster.AddToCounts(@object, 7);
				}
			}
			string partyMountStringId;
			string partyHarnessStringId;
			PartyMisc.getAdditionalVisualsForParty(origin.Culture, out partyMountStringId, out partyHarnessStringId);
			TextObject name = new TextObject("Escorted caravan", null);
			MobileParty mobileParty = CustomPartyComponent.CreateQuestParty(origin.GatePosition, 0f, origin, name, origin.OwnerClan, TroopRoster.CreateDummyTroopRoster(), TroopRoster.CreateDummyTroopRoster(), origin.Owner, partyMountStringId, partyHarnessStringId, 4f, false);
			this.initializeCaravanOnCreation(mobileParty, origin, itemRoster, 10);
			Campaign.Current.VisualTrackerManager.RegisterObject(mobileParty);
			mobileParty.Ai.SetDoNotMakeNewDecisions(true);
			mobileParty.IgnoreForHours(24f);
			return mobileParty;
		}

		// Token: 0x0600018B RID: 395 RVA: 0x00008DE0 File Offset: 0x00006FE0
		private void initializeCaravanOnCreation(MobileParty mobileParty, Settlement settlement, ItemRoster caravanItems, int troopToBeGiven)
		{
			mobileParty.Aggressiveness = 0f;
			if (troopToBeGiven == 0)
			{
				float num;
				if (MBRandom.RandomFloat < 0.67f)
				{
					num = (1f - MBRandom.RandomFloat * MBRandom.RandomFloat) * 0.5f + 0.5f;
				}
				else
				{
					num = 1f;
				}
				int num2 = (int)((float)mobileParty.Party.PartySizeLimit * num);
				if (num2 >= 10)
				{
					num2--;
				}
				troopToBeGiven = num2;
			}
			PartyTemplateObject caravanPartyTemplate = settlement.Culture.CaravanPartyTemplate;
			mobileParty.InitializeMobilePartyAtPosition(caravanPartyTemplate, settlement.GatePosition, troopToBeGiven);
			CharacterObject character2 = CharacterObject.All.First((CharacterObject character) => character.Occupation == Occupation.CaravanGuard && character.IsInfantry && character.Level == 26 && character.Culture == mobileParty.Party.Owner.Culture);
			mobileParty.MemberRoster.AddToCounts(character2, 1, true, 0, 0, true, -1);
			mobileParty.Party.SetVisualAsDirty();
			mobileParty.InitializePartyTrade(10000);
			if (caravanItems != null)
			{
				mobileParty.ItemRoster.Add(caravanItems);
				return;
			}
			float num3 = 10000f;
			ItemObject itemObject = null;
			foreach (ItemObject itemObject2 in Items.All)
			{
				if (itemObject2.ItemCategory == DefaultItemCategories.PackAnimal && !itemObject2.NotMerchandise && (float)itemObject2.Value < num3)
				{
					itemObject = itemObject2;
					num3 = (float)itemObject2.Value;
				}
			}
			if (itemObject != null)
			{
				mobileParty.ItemRoster.Add(new ItemRosterElement(itemObject, (int)((float)mobileParty.MemberRoster.TotalManCount * 0.5f), null));
			}
		}
	}
}
