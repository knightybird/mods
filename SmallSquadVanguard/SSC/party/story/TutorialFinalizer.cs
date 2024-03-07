using System;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.SceneInformationPopupTypes;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace SSC.party.story
{
	// Token: 0x02000053 RID: 83
	public static class TutorialFinalizer
	{
		// Token: 0x0600027D RID: 637 RVA: 0x0000E2A4 File Offset: 0x0000C4A4
		public static void finalizeTutorial()
		{
			MBInformationManager.ShowSceneNotification(new FindingFirstBannerPieceSceneNotificationItem(Hero.MainHero, new Action(TutorialFinalizer.ClandAndBannerSelection)));
		}

		// Token: 0x0600027E RID: 638 RVA: 0x0000E2C4 File Offset: 0x0000C4C4
		private static void ClandAndBannerSelection()
		{
			InformationManager.ShowTextInquiry(new TextInquiryData(new TextObject("{=JJiKk4ow}Select your family name: ", null).ToString(), string.Empty, true, false, GameTexts.FindText("str_done", null).ToString(), null, new Action<string>(TutorialFinalizer.OnChangeClanNameDone), null, false, new Func<string, Tuple<bool, string>>(FactionHelper.IsClanNameApplicable), "", Clan.PlayerClan.Name.ToString()), false, false);
		}

		// Token: 0x0600027F RID: 639 RVA: 0x0000E334 File Offset: 0x0000C534
		private static void OnChangeClanNameDone(string newClanName)
		{
			TextObject textObject = GameTexts.FindText("str_generic_clan_name", null);
			textObject.SetTextVariable("CLAN_NAME", new TextObject(newClanName, null));
			Clan.PlayerClan.InitializeClan(textObject, textObject, Clan.PlayerClan.Culture, Clan.PlayerClan.Banner, default(Vec2), false);
			TutorialFinalizer.OpenBannerSelectionScreen();
		}

		// Token: 0x06000280 RID: 640 RVA: 0x0000E38F File Offset: 0x0000C58F
		private static void OpenBannerSelectionScreen()
		{
			Game.Current.GameStateManager.PushState(Game.Current.GameStateManager.CreateState<BannerEditorState>(), 0);
		}
	}
}
