using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;

namespace SSC.conv
{
	// Token: 0x02000075 RID: 117
	public static class Conversation
	{
		// Token: 0x06000381 RID: 897 RVA: 0x00013A4E File Offset: 0x00011C4E
		public static bool isMainHeroInConversation()
		{
			return Hero.OneToOneConversationHero != null;
		}

		// Token: 0x06000382 RID: 898 RVA: 0x00013A58 File Offset: 0x00011C58
		public static void open(Hero hero)
		{
			CampaignMapConversation.OpenConversation(new ConversationCharacterData(CharacterObject.PlayerCharacter, null, true, true, false, false, false, false), new ConversationCharacterData(hero.CharacterObject, null, !hero.CharacterObject.HasMount(), true, false, false, false, false));
		}

		// Token: 0x06000383 RID: 899 RVA: 0x00013A9A File Offset: 0x00011C9A
		public static void LeaveConversation()
		{
			if (Campaign.Current != null && Campaign.Current.PlayerEncounter != null)
			{
				PlayerEncounter.LeaveEncounter = true;
				return;
			}
			Campaign campaign = Campaign.Current;
		}
	}
}
