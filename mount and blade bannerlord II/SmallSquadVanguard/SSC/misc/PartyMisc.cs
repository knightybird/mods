using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.Core;

namespace SSC.misc
{
	// Token: 0x0200006C RID: 108
	internal static class PartyMisc
	{
		// Token: 0x0600033E RID: 830 RVA: 0x000125C4 File Offset: 0x000107C4
		public static void getAdditionalVisualsForParty(CultureObject culture, out string mountStringId, out string harnessStringId)
		{
			if (culture.StringId == "aserai" || culture.StringId == "khuzait")
			{
				mountStringId = "camel";
				harnessStringId = ((MBRandom.RandomFloat > 0.5f) ? "camel_saddle_a" : "camel_saddle_b");
				return;
			}
			mountStringId = "mule";
			harnessStringId = ((MBRandom.RandomFloat > 0.5f) ? "mule_load_a" : ((MBRandom.RandomFloat > 0.5f) ? "mule_load_b" : "mule_load_c"));
		}

		// Token: 0x0600033F RID: 831 RVA: 0x0001264B File Offset: 0x0001084B
		public static void changeCustomPartyComponentSpeed(MobileParty party, float speedChange)
		{
			CustomPartyComponent customPartyComponent = party.PartyComponent as CustomPartyComponent;
			customPartyComponent.SetBaseSpeed(customPartyComponent.BaseSpeed + speedChange);
		}

		// Token: 0x06000340 RID: 832 RVA: 0x00012665 File Offset: 0x00010865
		public static void setCustomPartyComponentSpeed(MobileParty party, float speed)
		{
			(party.PartyComponent as CustomPartyComponent).SetBaseSpeed(speed);
		}

		// Token: 0x06000341 RID: 833 RVA: 0x00012678 File Offset: 0x00010878
		public static bool HasPartyWon(MobileParty party, MapEvent mapEvent)
		{
			if (mapEvent.DefeatedSide == BattleSideEnum.None)
			{
				return false;
			}
			BattleSideEnum battleSideEnum = mapEvent.AttackerSide.Parties.Any((MapEventParty p) => p.Party == party.Party) ? BattleSideEnum.Attacker : BattleSideEnum.Defender;
			return mapEvent.DefeatedSide != battleSideEnum;
		}

		// Token: 0x06000342 RID: 834 RVA: 0x000126CC File Offset: 0x000108CC
		public static bool HasPartyLost(MobileParty party, MapEvent mapEvent)
		{
			if (mapEvent.DefeatedSide == BattleSideEnum.None)
			{
				return false;
			}
			BattleSideEnum battleSideEnum = mapEvent.AttackerSide.Parties.Any((MapEventParty p) => p.Party == party.Party) ? BattleSideEnum.Attacker : BattleSideEnum.Defender;
			return mapEvent.DefeatedSide == battleSideEnum;
		}
	}
}
