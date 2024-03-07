using System;
using HarmonyLib;
using SSC.misc;
using SSC.party;
using SSC.settings;
using StoryMode.GameComponents.CampaignBehaviors;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace SSC.story
{
	// Token: 0x0200002E RID: 46
	[HarmonyPatch(typeof(TrainingFieldCampaignBehavior))]
	public static class StoryModePatches
	{
		// Token: 0x060001D4 RID: 468 RVA: 0x0000A41A File Offset: 0x0000861A
		[HarmonyPrefix]
		[HarmonyPatch(typeof(TrainingFieldCampaignBehavior), "RegisterEvents")]
		public static bool Prefix()
		{
			return false;
		}

		// Token: 0x060001D5 RID: 469 RVA: 0x0000A420 File Offset: 0x00008620
		public static bool OnSettlementLeft_Prefix(MobileParty party, Settlement settlement)
		{
			if (World.flagNotSet(Flags.PartyInitialized) && settlement.StringId == "tutorial_training_field" && party == MobileParty.MainParty)
			{
				CampaignEventDispatcher.Instance.RemoveListeners(Campaign.Current.GetCampaignBehavior<TutorialPhaseCampaignBehavior>());
				if (ModSettings.InitialCompanions > 0)
				{
					IntroductionStoryMode introductionStoryMode = new IntroductionStoryMode();
					introductionStoryMode.SetDialogs();
					introductionStoryMode.openConversation();
				}
			}
			return false;
		}
	}
}
