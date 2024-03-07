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
	// Token: 0x02000056 RID: 86
	public class SurgeonRole : PartyRole
	{
		// Token: 0x0600028D RID: 653 RVA: 0x0000E708 File Offset: 0x0000C908
		public void SurgeonReturns(Hero hero, bool skipConversation)
		{
			if (hero == World.DesignatedSurgeon.Value)
			{
				if (skipConversation || Hero.MainHero.IsPrisoner)
				{
					MobileParty.MainParty.SetPartySurgeon(hero);
					return;
				}
				QueuedAction queuedAction = new QueuedAction();
				queuedAction.Condition = (() => !Conversation.isMainHeroInConversation());
				queuedAction.Obsolete = (() => this.obsolete(hero));
				queuedAction.ActionToPerform = delegate()
				{
					this.setSurgeonDialog(hero);
					Conversation.open(hero);
				};
				World.queueAction(queuedAction);
			}
		}

		// Token: 0x0600028E RID: 654 RVA: 0x0000E7AE File Offset: 0x0000C9AE
		private bool obsolete(Hero surgeon)
		{
			return MobileParty.MainParty.EffectiveSurgeon == surgeon || World.DesignatedSurgeon.Value != surgeon;
		}

		// Token: 0x0600028F RID: 655 RVA: 0x0000E7D0 File Offset: 0x0000C9D0
		private void setSurgeonDialog(Hero surgeon)
		{
			this.dialogueBoundObject = new object();
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 9000).NpcLine(new TextObject("Hi, Captain, I have returned. [ib:normal2][if:convo_calm_friendly]", null), null, null).Condition(() => Hero.OneToOneConversationHero == surgeon).PlayerLine(new TextObject(string.Format("{0}! By the gods, you are a sight for sore eyes. We feared the worst.", surgeon), null), null).NpcLine(new TextObject("Fear not, for I am made of sterner stuff. It takes more than a prison cell to keep me from my duties.[ib:normal][if:convo_calm_friendly]", null), null, null).PlayerLine(new TextObject("We are in dire need of your skills. Some of us are still nursing our battle wounds.", null), null).NpcLine(new TextObject("Rest assured, I shall tend to everyone. No one will be left to suffer.[ib:normal]", null), null, null).Consequence(new ConversationSentence.OnConsequenceDelegate(this.surgeon_rejoins_consequence)).CloseDialog(), this.dialogueBoundObject);
		}

		// Token: 0x06000290 RID: 656 RVA: 0x0000E8A4 File Offset: 0x0000CAA4
		private void surgeon_rejoins_consequence()
		{
			MobileParty.MainParty.SetPartySurgeon(World.DesignatedSurgeon.Value);
			Campaign.Current.ConversationManager.RemoveRelatedLines(this.dialogueBoundObject);
			Message.display(new TextObject(string.Format("{0} is now the party's surgeon.", World.DesignatedSurgeon.Value), null).ToString(), Colors.Green);
			Message.println(Campaign.Current.ConversationManager.DetailedDebugLog);
		}

		// Token: 0x06000291 RID: 657 RVA: 0x0000E917 File Offset: 0x0000CB17
		protected override int compare(Hero hero1, Hero hero2)
		{
			return hero1.GetSkillValue(DefaultSkills.Medicine) - hero2.GetSkillValue(DefaultSkills.Medicine);
		}

		// Token: 0x06000292 RID: 658 RVA: 0x0000E930 File Offset: 0x0000CB30
		protected override void setToRole(Hero hero)
		{
			MobileParty.MainParty.SetPartySurgeon(hero);
		}
	}
}
