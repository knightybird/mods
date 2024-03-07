using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Overlay;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;

namespace SSC.world.escort
{
	// Token: 0x02000023 RID: 35
	public class EscortMenus
	{
		// Token: 0x14000004 RID: 4
		// (add) Token: 0x0600018D RID: 397 RVA: 0x00008FA4 File Offset: 0x000071A4
		// (remove) Token: 0x0600018E RID: 398 RVA: 0x00008FDC File Offset: 0x000071DC
		public event Action onCaravanMenuOptionSelected;

		// Token: 0x14000005 RID: 5
		// (add) Token: 0x0600018F RID: 399 RVA: 0x00009014 File Offset: 0x00007214
		// (remove) Token: 0x06000190 RID: 400 RVA: 0x0000904C File Offset: 0x0000724C
		public event Action<MenuCallbackArgs, int> onDestinationSelected;

		// Token: 0x06000191 RID: 401 RVA: 0x00009081 File Offset: 0x00007281
		public EscortMenus(EscortData escortData)
		{
			this.escortData = escortData;
		}

		// Token: 0x06000192 RID: 402 RVA: 0x00009090 File Offset: 0x00007290
		public void AddGameMenus(CampaignGameStarter starter)
		{
			this.AddLookForCaravanMenuOption(starter);
			this.AddCaravanMenuEntries(starter, 6);
			this.AddLeaveOption(starter);
		}

		// Token: 0x06000193 RID: 403 RVA: 0x000090A8 File Offset: 0x000072A8
		public void setMenuEntriesText(List<Settlement> closestTowns)
		{
			closestTowns.Sort(delegate(Settlement a, Settlement b)
			{
				float distance = EscortData.getDistance(Settlement.CurrentSettlement, a);
				float distance2 = EscortData.getDistance(Settlement.CurrentSettlement, b);
				return distance.CompareTo(distance2);
			});
			string arg = "{GOLD_ICON}";
			for (int i = 0; i < closestTowns.Count; i++)
			{
				int num = this.escortData.calculatePayment(Settlement.CurrentSettlement, closestTowns[i]);
				EscortData.getDistance(Settlement.CurrentSettlement, closestTowns[i]);
				MBTextManager.SetTextVariable(string.Format("CARAVAN_{0}", i), string.Format("{0}    {1}{2}", closestTowns[i].Name, num, arg), false);
			}
		}

		// Token: 0x06000194 RID: 404 RVA: 0x00009150 File Offset: 0x00007350
		private void AddLookForCaravanMenuOption(CampaignGameStarter starter)
		{
			starter.AddGameMenuOption("town", "caravan_escort", "Look for a caravan to escort", new GameMenuOption.OnConditionDelegate(this.game_menu_caravan_escort), delegate(MenuCallbackArgs args)
			{
				Action action = this.onCaravanMenuOptionSelected;
				if (action != null)
				{
					action();
				}
				GameMenu.SwitchToMenu("available_caravans");
			}, false, 0, true, null);
			starter.AddGameMenu("available_caravans", "As you stroll through the bustling marketplace, you encounter several merchants who are in need of an escort.", delegate(MenuCallbackArgs args)
			{
			}, GameOverlays.MenuOverlayType.SettlementWithBoth, GameMenu.MenuFlags.None, null);
		}

		// Token: 0x06000195 RID: 405 RVA: 0x000091C0 File Offset: 0x000073C0
		private void AddCaravanMenuEntries(CampaignGameStarter starter, int townCount)
		{
			for (int i = 0; i < townCount; i++)
			{
				int index = i;
				string str = string.Format("CARAVAN_{0}", i);
				string text = "{" + str + "}";
				starter.AddGameMenuOption("available_caravans", string.Format("caravan_escort_{0}_id", i), text ?? "", new GameMenuOption.OnConditionDelegate(this.game_menu_caravan), delegate(MenuCallbackArgs args)
				{
					this.onDestinationSelected(args, index);
				}, false, -1, true, null);
			}
		}

		// Token: 0x06000196 RID: 406 RVA: 0x00009250 File Offset: 0x00007450
		private void AddLeaveOption(CampaignGameStarter starter)
		{
			starter.AddGameMenuOption("available_caravans", "caravan_escort_leave_id", "Cancel", new GameMenuOption.OnConditionDelegate(this.game_menu_caravan_leave), delegate(MenuCallbackArgs args)
			{
				GameMenu.SwitchToMenu("town");
			}, true, -1, true, null);
		}

		// Token: 0x06000197 RID: 407 RVA: 0x000092A1 File Offset: 0x000074A1
		private bool game_menu_caravan(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Continue;
			return true;
		}

		// Token: 0x06000198 RID: 408 RVA: 0x000092AC File Offset: 0x000074AC
		private bool game_menu_caravan_escort(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.DefendAction;
			return !Settlement.CurrentSettlement.IsUnderSiege && !this.escortData.IsOngoing;
		}

		// Token: 0x06000199 RID: 409 RVA: 0x000092D2 File Offset: 0x000074D2
		private bool game_menu_caravan_leave(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Leave;
			return true;
		}

		// Token: 0x0600019A RID: 410 RVA: 0x000092E0 File Offset: 0x000074E0
		public void AddCaravanDialogs(CampaignGameStarter starter)
		{
			string id = "escort_caravan_greeting";
			TextObject textObject = new TextObject("Well met, captain! We are making good progress on our journey.", null);
			starter.AddDialogLine(id, "start", "greeted_by_caravan_party", textObject.ToString(), new ConversationSentence.OnConditionDelegate(this.CaravanTalkOnCondition), null, 100, null);
			starter.AddPlayerLine("escort_caravan_talk_leave", "greeted_by_caravan_party", "close_window", new TextObject("Just checking in. Keep moving, stay safe.", null).ToString(), null, new ConversationSentence.OnConsequenceDelegate(this.leave_on_consequence), 100, null, null);
		}

		// Token: 0x0600019B RID: 411 RVA: 0x0000935E File Offset: 0x0000755E
		private void leave_on_consequence()
		{
			PlayerEncounter.LeaveEncounter = true;
		}

		// Token: 0x0600019C RID: 412 RVA: 0x00009368 File Offset: 0x00007568
		private bool CaravanTalkOnCondition()
		{
			if (!this.escortData.IsOngoing)
			{
				return false;
			}
			PartyBase encounteredParty = PlayerEncounter.EncounteredParty;
			return encounteredParty != null && encounteredParty.MobileParty != null && encounteredParty.MobileParty == this.escortData.caravan;
		}

		// Token: 0x04000086 RID: 134
		public EscortData escortData;
	}
}
