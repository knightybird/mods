using System;
using System.Collections.Generic;
using SSC.misc;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace SSC.party
{
	// Token: 0x0200004E RID: 78
	internal class IntroductionStoryMode
	{
		// Token: 0x06000260 RID: 608 RVA: 0x0000D7D4 File Offset: 0x0000B9D4
		public void SetDialogs()
		{
			bool isFemale = Hero.MainHero.IsFemale;
			string str = isFemale ? "my lady" : "sir";
			string str2 = isFemale ? "You must be a lady of stature, or something of the sort?" : "Are you perhaps a lord, or something of the sort?";
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 500).NpcLine(new TextObject("Excuse me, " + str + ", do you know if the military detachment is arriving soon?[ib:normal][if:convo_thinking]", null), null, null).Condition(new ConversationSentence.OnConditionDelegate(this.meeting_avigos)).PlayerLine(new TextObject("Um, I'm not aware of any such thing. What detachment? Are you soldiers?", null), null).NpcLine(new TextObject("We aren't yet. We came here to enlist. We thought soldiers would come here to train and that recruiters would be present. It seemed a safe assumption. But no one has shown up, and it's already been a week.[ib:normal]", null), null, null).PlayerLine(new TextObject("Why didn't you apply at Poros?", null), null).NpcLine(new TextObject("That fellow among us over there convinced us that garrison duty is dull and that active soldiers earn more. So, we decided to try our luck here instead.[ib:normal2]", null), null, null).PlayerLine(new TextObject("I see.", null), null).NpcLine(new TextObject("And now we’re running low on food. Er... I just thought that you might know if any recruiters are coming. Seeing you with a horse and all. " + str2 + "[ib:normal]", null), null, null).PlayerLine(new TextObject("Uh... something like that. Well, not quite. My brother and I came here from a distant land. We've had some... troubles.", null), null).NpcLine(new TextObject("Down on your luck, eh? Just like us. The training master told us we could buy food from a nearby village, but we don't have any money either.[ib:normal2]", null), null, null).PlayerLine(new TextObject("I have a mission though. My clan will rise, and certain people will face justice. In fact, why don't you all join me? I can pay you.", null), null).NpcLine(new TextObject("Really? Well, why not? It's not like we're in a position to be choosy. I'll go tell the men. I doubt any of them will want to wait here any longer.[ib:normal][if:convo_calm_friendly]", null), null, null).Consequence(new ConversationSentence.OnConsequenceDelegate(this.dialogConsequence)).CloseDialog(), this);
		}

		// Token: 0x06000261 RID: 609 RVA: 0x0000D927 File Offset: 0x0000BB27
		private void dialogConsequence()
		{
			PartyInitialization.AddCompanionsToParty(this.companions);
			World.setFlag(Flags.PartyInitialized);
			Campaign.Current.ConversationManager.RemoveRelatedLines(this);
		}

		// Token: 0x06000262 RID: 610 RVA: 0x0000D94B File Offset: 0x0000BB4B
		private bool meeting_avigos()
		{
			return true;
		}

		// Token: 0x06000263 RID: 611 RVA: 0x0000D950 File Offset: 0x0000BB50
		public void openConversation()
		{
			this.companions = PartyInitialization.CreateCompanionsToList();
			Hero hero = this.getCompanionByStringId("spc_companion_avigos", this.companions);
			if (hero == null)
			{
				return;
			}
			InformationManager.ShowInquiry(new InquiryData(new TextObject("", null).ToString(), new TextObject("As you exit the training field, your brother departs. You’re about to set off as well when a man approaches you. You notice a group of men camping at a distance behind him, just outside the training field.", null).ToString(), true, false, new TextObject("{=lmG7uRK2}Okay", null).ToString(), null, delegate()
			{
				CampaignMapConversation.OpenConversation(new ConversationCharacterData(CharacterObject.PlayerCharacter, null, true, true, false, false, false, false), new ConversationCharacterData(hero.CharacterObject, null, true, true, false, false, false, false));
			}, null, "", 0f, null, null, null), false, false);
		}

		// Token: 0x06000264 RID: 612 RVA: 0x0000D9E8 File Offset: 0x0000BBE8
		private Hero getCompanionByStringId(string id)
		{
			foreach (TroopRosterElement troopRosterElement in MobileParty.MainParty.MemberRoster.GetTroopRoster())
			{
				if (troopRosterElement.Character.IsHero)
				{
					CharacterObject originalCharacter = troopRosterElement.Character.OriginalCharacter;
					if (((originalCharacter != null) ? originalCharacter.StringId : null) == id)
					{
						return troopRosterElement.Character.HeroObject;
					}
				}
			}
			return null;
		}

		// Token: 0x06000265 RID: 613 RVA: 0x0000DA7C File Offset: 0x0000BC7C
		private Hero getCompanionByStringId(string id, List<Hero> list)
		{
			foreach (Hero hero in list)
			{
				CharacterObject originalCharacter = hero.CharacterObject.OriginalCharacter;
				if (((originalCharacter != null) ? originalCharacter.StringId : null) == id)
				{
					return hero;
				}
			}
			return null;
		}

		// Token: 0x040000C4 RID: 196
		private List<Hero> companions;
	}
}
