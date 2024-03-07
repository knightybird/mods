using System;
using SSC.conv;
using SSC.misc;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace SSC.party
{
	// Token: 0x02000047 RID: 71
	public class ScoutRole : PartyRole
	{
		// Token: 0x0600023E RID: 574 RVA: 0x0000C454 File Offset: 0x0000A654
		public void ScoutReturns(Hero hero, bool skipConversation)
		{
			if (hero == World.DesignatedScout.Value)
			{
				if (skipConversation || Hero.MainHero.IsPrisoner)
				{
					MobileParty.MainParty.SetPartyScout(hero);
					return;
				}
				QueuedAction queuedAction = new QueuedAction();
				queuedAction.Condition = (() => !Conversation.isMainHeroInConversation());
				queuedAction.Obsolete = (() => this.obsolete(hero));
				queuedAction.ActionToPerform = delegate()
				{
					this.setScoutDialog(hero);
					Conversation.open(hero);
				};
				World.queueAction(queuedAction);
			}
		}

		// Token: 0x0600023F RID: 575 RVA: 0x0000C4FA File Offset: 0x0000A6FA
		private bool obsolete(Hero scout)
		{
			return MobileParty.MainParty.EffectiveScout == scout || World.DesignatedScout.Value != scout;
		}

		// Token: 0x06000240 RID: 576 RVA: 0x0000C51C File Offset: 0x0000A71C
		private void setScoutDialog(Hero scout)
		{
			this.dialogueBoundObject = new object();
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 1000010).NpcLine(new TextObject("Hi, Captain, I have returned.[ib:normal][if:convo_calm_friendly]", null), null, null).Condition(() => ScoutRole.scout_rejoins(scout)).PlayerLine(new TextObject("Hail, friend! It's good to see you in one piece! We feared the worst.", null), null).NpcLine(new TextObject("Aye, it was a close call. But the shadows are a scout's best friend.[ib:normal]", null), null, null).PlayerLine(new TextObject(string.Format("We've been at a disadvantage without our scout. Are you ready to take up that role, {0}?", scout.Name), null), null).NpcLine(new TextObject("Aye, Captain. I'm ready to serve as the party's eyes and ears.[ib:normal][if:convo_calm_friendly]", null), null, null).Consequence(new ConversationSentence.OnConsequenceDelegate(this.scout_rejoins_consequence)).CloseDialog(), this.dialogueBoundObject);
		}

		// Token: 0x06000241 RID: 577 RVA: 0x0000C5F4 File Offset: 0x0000A7F4
		private static bool scout_rejoins(Hero hero)
		{
			return Hero.OneToOneConversationHero == hero;
		}

		// Token: 0x06000242 RID: 578 RVA: 0x0000C600 File Offset: 0x0000A800
		private void scout_rejoins_consequence()
		{
			MobileParty.MainParty.SetPartyScout(World.DesignatedScout.Value);
			Campaign.Current.ConversationManager.RemoveRelatedLines(this.dialogueBoundObject);
			Message.display(new TextObject(string.Format("{0} is now the party's scout.", World.DesignatedScout.Value), null).ToString(), Colors.Green);
		}

		// Token: 0x06000243 RID: 579 RVA: 0x0000C660 File Offset: 0x0000A860
		private static Hero bestScoutInParty()
		{
			Hero hero = Hero.MainHero;
			foreach (TroopRosterElement troopRosterElement in MobileParty.MainParty.MemberRoster.GetTroopRoster())
			{
				if (troopRosterElement.Character.IsHero && troopRosterElement.Character.HeroObject != Hero.MainHero)
				{
					Hero heroObject = troopRosterElement.Character.HeroObject;
					if (heroObject.GetSkillValue(DefaultSkills.Scouting) > hero.GetSkillValue(DefaultSkills.Scouting))
					{
						hero = heroObject;
					}
				}
			}
			return hero;
		}

		// Token: 0x06000244 RID: 580 RVA: 0x0000C704 File Offset: 0x0000A904
		protected override void setToRole(Hero hero)
		{
			MobileParty.MainParty.SetPartyScout(hero);
		}

		// Token: 0x06000245 RID: 581 RVA: 0x0000C711 File Offset: 0x0000A911
		protected override int compare(Hero hero1, Hero hero2)
		{
			return hero1.GetSkillValue(DefaultSkills.Scouting) - hero2.GetSkillValue(DefaultSkills.Scouting);
		}
	}
}
