using System;
using SSC.conv;
using SSC.misc;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace SSC.party.roles
{
	// Token: 0x02000055 RID: 85
	public class QuartermasterRole : PartyRole
	{
		// Token: 0x06000286 RID: 646 RVA: 0x0000E4E8 File Offset: 0x0000C6E8
		public void QuartermasterReturns(Hero hero, bool skipConversation)
		{
			if (hero == World.DesignatedQuartermaster.Value)
			{
				if (skipConversation || Hero.MainHero.IsPrisoner)
				{
					MobileParty.MainParty.SetPartyQuartermaster(hero);
					return;
				}
				QueuedAction queuedAction = new QueuedAction();
				queuedAction.Condition = (() => !Conversation.isMainHeroInConversation());
				queuedAction.Obsolete = (() => this.obsolete(hero));
				queuedAction.ActionToPerform = delegate()
				{
					this.setQuartermasterDialog(hero);
					Conversation.open(hero);
				};
				World.queueAction(queuedAction);
			}
		}

		// Token: 0x06000287 RID: 647 RVA: 0x0000E58E File Offset: 0x0000C78E
		private bool obsolete(Hero quartermaster)
		{
			return MobileParty.MainParty.EffectiveQuartermaster == quartermaster || World.DesignatedQuartermaster.Value != quartermaster;
		}

		// Token: 0x06000288 RID: 648 RVA: 0x0000E5B0 File Offset: 0x0000C7B0
		private void setQuartermasterDialog(Hero quartermaster)
		{
			this.dialogueBoundObject = new object();
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 9001).NpcLine(new TextObject("Hi, Captain, I have returned.[ib:normal2][if:convo_calm_friendly]", null), null, null).Condition(() => Hero.OneToOneConversationHero == quartermaster).PlayerLine(new TextObject("By the gods, it's good to see you, friend. We feared the worst.", null), null).NpcLine(new TextObject("Ha! Those buffoons couldn't even keep their prisoner keys in check. I half-considered giving them a lesson in proper prisoner management, but then I just left them to their misery. I trust you've kept our camp in order, Captain?[ib:normal][if:convo_calm_friendly]", null), null, null).PlayerLine(new TextObject("We've managed as best we could, but without you, it's been a struggle", null), null).NpcLine(new TextObject("Well, I'm back now. Let's take stock of our supplies and plan our next move. We've got a score to settle.[ib:normal2]", null), null, null).Consequence(new ConversationSentence.OnConsequenceDelegate(this.quartermaster_rejoins_consequence)).CloseDialog(), this.dialogueBoundObject);
		}

		// Token: 0x06000289 RID: 649 RVA: 0x0000E678 File Offset: 0x0000C878
		private void quartermaster_rejoins_consequence()
		{
			MobileParty.MainParty.SetPartyQuartermaster(World.DesignatedQuartermaster.Value);
			Campaign.Current.ConversationManager.RemoveRelatedLines(this.dialogueBoundObject);
			Message.display(new TextObject(string.Format("{0} is now the party's quartermaster.", World.DesignatedQuartermaster.Value), null).ToString(), Colors.Green);
		}

		// Token: 0x0600028A RID: 650 RVA: 0x0000E6D7 File Offset: 0x0000C8D7
		protected override int compare(Hero hero1, Hero hero2)
		{
			return hero1.GetSkillValue(DefaultSkills.Steward) - hero2.GetSkillValue(DefaultSkills.Steward);
		}

		// Token: 0x0600028B RID: 651 RVA: 0x0000E6F0 File Offset: 0x0000C8F0
		protected override void setToRole(Hero hero)
		{
			MobileParty.MainParty.SetPartyQuartermaster(hero);
		}
	}
}
