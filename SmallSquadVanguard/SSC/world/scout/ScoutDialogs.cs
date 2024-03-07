using System;
using System.Collections.Generic;
using SSC.conv;
using SSC.misc;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;

namespace SSC.world.scout
{
	// Token: 0x02000015 RID: 21
	internal class ScoutDialogs
	{
		// Token: 0x060000E4 RID: 228 RVA: 0x00004DAD File Offset: 0x00002FAD
		public ScoutDialogs(ScoutMission scoutData)
		{
			this.scoutData = scoutData;
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x00004DBC File Offset: 0x00002FBC
		public void AddMenus(CampaignGameStarter starter)
		{
			starter.AddGameMenuOption("village", "scout_raid", "{scout_raid_this_village}", new GameMenuOption.OnConditionDelegate(this.game_menu_scout_raid), delegate(MenuCallbackArgs args)
			{
				if (this.scoutData.IsOngoing)
				{
					this.scoutData.Target = Settlement.CurrentSettlement;
				}
				PlayerEncounter.LeaveSettlement();
				PlayerEncounter.Finish(true);
				Campaign.Current.SaveHandler.SignalAutoSave();
			}, false, -1, true, null);
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x00004DFC File Offset: 0x00002FFC
		private bool game_menu_scout_raid(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.OrderTroopsToAttack;
			bool flag = this.scoutData.IsOngoing && Settlement.CurrentSettlement != null && Settlement.CurrentSettlement.IsVillage && this.scoutData.Follower != null && this.scoutData.Follower.isHostileTo(Settlement.CurrentSettlement) && this.scoutData.Leader != null;
			if (flag)
			{
				string str = this.scoutData.Leader.Name.ToString();
				MBTextManager.SetTextVariable("scout_raid_this_village", "Set as target for " + str, false);
			}
			return flag;
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x00004E95 File Offset: 0x00003095
		private ConversationSentence.OnConditionDelegate condition(Func<bool> condition)
		{
			return new ConversationSentence.OnConditionDelegate(condition.Invoke);
		}

		// Token: 0x060000E8 RID: 232 RVA: 0x00004EA3 File Offset: 0x000030A3
		private ConversationSentence.OnConsequenceDelegate consequence(Action consequence)
		{
			return new ConversationSentence.OnConsequenceDelegate(consequence.Invoke);
		}

		// Token: 0x060000E9 RID: 233 RVA: 0x00004EB1 File Offset: 0x000030B1
		private void takes_lead()
		{
			ScoutMission scoutMission = this.scoutData;
			PartyBase encounteredParty = PlayerEncounter.EncounteredParty;
			scoutMission.setFollowingParty((encounteredParty != null) ? encounteredParty.MobileParty : null);
		}

		// Token: 0x060000EA RID: 234 RVA: 0x00004ECF File Offset: 0x000030CF
		private void abandons_mission()
		{
			this.scoutData.abandonMission(Abandonment.Reason.PlayerDecision);
		}

		// Token: 0x060000EB RID: 235 RVA: 0x00004EDD File Offset: 0x000030DD
		private void leaves()
		{
			PlayerEncounter.LeaveEncounter = true;
		}

		// Token: 0x060000EC RID: 236 RVA: 0x00004EE8 File Offset: 0x000030E8
		public void AddDialogsSubordinates(CampaignGameStarter starter)
		{
			starter.AddPlayerLine("taking_the_lead_talk_1", "hero_main_options", "taking_the_lead_with_clan_member", "I'm taking the lead, follow my trail.", this.condition(() => this.take_the_lead_condition() && this.talkingToClanMember()), null, 120, null, null);
			starter.AddDialogLine("taking_the_lead_talk_2", "taking_the_lead_with_clan_member", "close_window", "As you command, we'll follow you.", this.condition(() => true), this.consequence(delegate
			{
				this.takes_lead();
				this.leaves();
			}), 100, null);
			starter.AddPlayerLine("release_command_talk", "hero_main_options", "release_command_with_clan_member", "You may proceed on your own now. You've done well. Take care.", this.condition(() => this.stop_leading_condition() && this.talkingToClanMember()), null, 120, null, null);
			starter.AddDialogLine("release_command_talk_2", "release_command_with_clan_member", "close_window", "Understood. We'll forge ahead.", this.condition(() => true), this.consequence(delegate
			{
				this.abandons_mission();
				this.leaves();
			}), 100, null);
			starter.AddPlayerLine("taking_the_lead_talk_3", "hero_main_options", "taking_the_lead_with_vassal", "I'm taking the lead, follow in my wake.", this.condition(() => this.take_the_lead_condition() && this.talkingToSubordinate() && !this.talkingToClanMember()), null, 100, null, null);
			starter.AddDialogLine("taking_the_lead_talk_4", "taking_the_lead_with_vassal", "close_window", "At your behest, my {?PLAYER.GENDER}lady{?}lord{\\?}. We shall follow your path.", this.condition(() => true), this.consequence(delegate
			{
				this.takes_lead();
				this.leaves();
			}), 100, null);
			starter.AddPlayerLine("release_command_talk", "hero_main_options", "release_command_vassal", "Our paths diverge here. Continue forth in my stead.", this.condition(() => this.stop_leading_condition() && this.talkingToSubordinate() && !this.talkingToClanMember()), null, 120, null, null);
			starter.AddDialogLine("release_command_talk_3", "release_command_vassal", "close_window", "As you wish, my {?PLAYER.GENDER}lady{?}lord{\\?}. We shall march forth in your honor.", this.condition(() => true), this.consequence(delegate
			{
				this.scoutData.abandonMission(Abandonment.Reason.PlayerDecision);
				PlayerEncounter.LeaveEncounter = true;
			}), 100, null);
		}

		// Token: 0x060000ED RID: 237 RVA: 0x00005114 File Offset: 0x00003314
		public void AddDialogs(CampaignGameStarter starter)
		{
			starter.AddPlayerLine("offer_scout_service_talk1", "hero_main_options", "scout_service_offered", "Would you enlist me as a scouting party? I can guide you on the path to glory and renown.", this.condition(() => this.take_the_lead_condition() && !this.talkingToClanMember() && !this.talkingToSubordinate()), this.consequence(delegate
			{
				this.scoutOfferMade = true;
			}), 100, null, null);
			starter.AddDialogLine("offer_scout_service_talk2_1", "scout_service_offered", "close_window", "Glory and renown, you say? Sounds enticing. And you seem capable. I agree.", this.condition(() => this.Response() == ResponseType.GladlyAccept && this.isMajorFaction()), new ConversationSentence.OnConsequenceDelegate(this.offerAccepted), 100, null);
			starter.AddDialogLine("offer_scout_service_talk2_2", "scout_service_offered", "close_window", "Glory and renown? I was thinking about plunder, but okay.", this.condition(() => this.Response() == ResponseType.GladlyAccept && !this.isMajorFaction()), new ConversationSentence.OnConsequenceDelegate(this.offerAccepted), 100, null);
			starter.AddDialogLine("offer_scout_service_talk3", "scout_service_offered", "close_window", "I’m uncertain about entrusting you with this… But I suppose we could give it a try.", this.condition(() => this.Response() == ResponseType.UnsureAccept), new ConversationSentence.OnConsequenceDelegate(this.offerAccepted), 100, null);
			starter.AddDialogLine("offer_scout_service_talk4", "scout_service_offered", "unsure_refuse", "A skilled scouting party is a valuable asset indeed. Yet, entrusting the vanguard to another is a decision not lightly made. I’m afraid I must decline your offer.", this.condition(() => this.Response() == ResponseType.UnsureRefuse), this.consequence(delegate
			{
				this.refusals++;
			}), 100, null);
			starter.AddPlayerLine("offer_scout_service_talk5", "unsure_refuse", "close_window", new TextObject("I see, farewell.", null).ToString(), this.condition(() => true), this.consequence(delegate
			{
				Conversation.LeaveConversation();
			}), 100, null, null);
			starter.AddPlayerLine("offer_scout_service_talk5", "unsure_refuse", "scout_what_I_lack", new TextObject("Might I ask you, why is it that every lord acknowledges the importance of a skilled scouting party, yet none seem willing to take a chance on one?", null).ToString(), this.condition(() => this.refusals > 2), null, 120, null, null);
			starter.AddDialogLine("offer_scout_service_talk6", "scout_what_I_lack", "scout_leads_the_way", "It’s not just a question of skill, but of trust. A scouting party leads the way, and there’s a fear they might lead us into peril.", this.condition(() => true), null, 100, null);
			starter.AddPlayerLine("offer_scout_service_talk6", "scout_leads_the_way", "the_path_of_war", "Ah.", () => true, null, 100, null, null);
			starter.AddDialogLine("offer_scout_service_talk7", "the_path_of_war", "close_window", "The path of war is fraught with unseen dangers, and a guide’s true worth is proven in adversity. Seek employment with smaller parties first. They often find themselves in need of all the help they can get.", this.condition(() => true), this.consequence(delegate
			{
				PlayerEncounter.LeaveEncounter = true;
			}), 100, null);
			starter.AddDialogLine("offer_scout_service_talk7", "scout_service_offered", "hero_main_options", "No.", this.condition(() => this.Response() == ResponseType.FlatlyRefuse), null, 100, null);
			starter.AddPlayerLine("should_buy_horses_1", "hero_main_options", "told_to_buy_horses", "You need to acquire some horses. Our speed is crucial.", this.condition(new Func<bool>(this.shouldTellThemToBuyHorses)), null, 100, null, null);
			starter.AddDialogLine("should_buy_horses_2", "told_to_buy_horses", "close_window", "Oh… You’re right, of course. They are expensive, but we will buy some at the next town.", this.condition(() => !this.talkingToClanMember()), this.consequence(delegate
			{
				World.setFlag(Flags.ScoutMission_BuyHorsesMode);
				PlayerEncounter.LeaveEncounter = true;
			}), 100, null);
			starter.AddDialogLine("should_buy_horses_3", "told_to_buy_horses", "close_window", "As you command. We will buy horses at the next town.", this.condition(() => this.talkingToClanMember()), this.consequence(delegate
			{
				World.setFlag(Flags.ScoutMission_BuyHorsesMode);
				PlayerEncounter.LeaveEncounter = true;
			}), 100, null);
		}

		// Token: 0x060000EE RID: 238 RVA: 0x00005504 File Offset: 0x00003704
		public void AddMoreDialogs(CampaignGameStarter starter)
		{
			starter.AddPlayerLine("scout_have_to_leave_1", "hero_main_options", "scout_have_to_leave_captain", "Hey Captain, it's been a blast scouting for you. But I've got this itch to see what else is out there. Mind if I take a break from our contract?", this.condition(() => this.stop_leading_condition() && !this.isMajorFaction() && !this.talkingToSubordinate()), null, 120, null, null);
			starter.AddDialogLine("scout_have_to_leave_2", "scout_have_to_leave_captain", "scout_friendly_captain", "You've done good work, and we'll miss having you around. But I get it, everyone needs a change of scenery now and then. Consider your contract on hold. Don't be a stranger, alright?", this.condition(() => this.release() == ScoutDialogs.Release.CaptainFriendly), null, 100, null);
			starter.AddPlayerLine("scout_have_to_leave_3", "scout_friendly_captain", "close_window", "Thanks, Captain. May our paths cross again. Until then, keep your blade sharp and your wits sharper.", null, this.consequence(delegate
			{
				this.abandons_mission();
				this.leaves();
			}), 100, null, null);
			starter.AddDialogLine("scout_have_to_leave_4", "scout_have_to_leave_captain", "close_window", "So you're just going to leave us high and dry, huh? Fine, go do your thing. But don't expect us to wait around for you.", this.condition(() => this.release() == ScoutDialogs.Release.Captain), this.consequence(delegate
			{
				this.abandons_mission();
				this.leaves();
			}), 100, null);
			starter.AddPlayerLine("scout_have_to_leave_5", "hero_main_options", "scout_have_to_leave_lord", "It’s been an honor serving under your banner, my Lord. But now, other tasks call for my attention. I'm afraid I have to leave.", this.condition(() => this.stop_leading_condition() && this.isMajorFaction() && !this.talkingToClanMember() && !this.talkingToSubordinate() && !this.heroIsFemale()), null, 120, null, null);
			starter.AddPlayerLine("scout_have_to_leave_5_1", "hero_main_options", "scout_have_to_leave_lord", "My Lady, it’s been an honor serving under your banner. But now, other tasks call for my attention. I'm afraid I have to leave.", this.condition(() => this.stop_leading_condition() && this.isMajorFaction() && !this.talkingToClanMember() && !this.talkingToSubordinate() && this.heroIsFemale()), null, 120, null, null);
			starter.AddDialogLine("scout_have_to_leave_6", "scout_have_to_leave_lord", "close_window", "Your service is invaluable, and your departure will leave a void. However, we all have our own journeys to embark on. Proceed with my goodwill.", this.condition(() => this.release() == ScoutDialogs.Release.LordFriendly), this.consequence(delegate
			{
				this.abandons_mission();
				this.leaves();
			}), 100, null);
			starter.AddDialogLine("scout_have_to_leave_7", "scout_have_to_leave_lord", "close_window", "Your service has been recognized. We all have our paths to tread. May fortune favor you more generously in your forthcoming ventures.", this.condition(() => this.release() == ScoutDialogs.Release.Lord), this.consequence(delegate
			{
				this.abandons_mission();
				this.leaves();
			}), 100, null);
			starter.AddDialogLine("scout_have_to_leave_8", "scout_have_to_leave_lord", "close_window", "Indeed, proceed as you deem fit. Doesn't feel like a big loss, to be honest.", this.condition(() => this.release() == ScoutDialogs.Release.LordIndifferent), this.consequence(delegate
			{
				this.abandons_mission();
				this.leaves();
			}), 100, null);
		}

		// Token: 0x060000EF RID: 239 RVA: 0x00005714 File Offset: 0x00003914
		private bool shouldTellThemToBuyHorses()
		{
			return this.scoutData.IsOngoing && this.scoutData.Follower != null && !World.isFlagSet(Flags.ScoutMission_BuyHorsesMode) && this.scoutData.Leader != null && this.scoutData.Leader == Hero.OneToOneConversationHero;
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x0000576C File Offset: 0x0000396C
		private bool hostileToPlayer(PartyBase partyBase)
		{
			if (partyBase == null)
			{
				return false;
			}
			IFaction mapFaction = partyBase.MapFaction;
			return mapFaction != null && mapFaction.IsAtWarWith(Hero.MainHero.MapFaction);
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x0000579C File Offset: 0x0000399C
		private bool isMajorFaction()
		{
			PartyBase encounteredParty = PlayerEncounter.EncounteredParty;
			if (encounteredParty == null)
			{
				return false;
			}
			MobileParty mobileParty = encounteredParty.MobileParty;
			if (mobileParty == null)
			{
				return false;
			}
			Clan actualClan = mobileParty.ActualClan;
			bool? flag = (actualClan != null) ? new bool?(actualClan.IsMinorFaction) : null;
			bool flag2 = false;
			return flag.GetValueOrDefault() == flag2 & flag != null;
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x000057F4 File Offset: 0x000039F4
		private bool talkingToClanMember()
		{
			PartyBase encounteredParty = PlayerEncounter.EncounteredParty;
			Hero oneToOneConversationHero = Hero.OneToOneConversationHero;
			Clan clan = (oneToOneConversationHero != null) ? oneToOneConversationHero.Clan : null;
			return ((encounteredParty != null) ? encounteredParty.MobileParty : null) != null && clan != null && clan == Hero.MainHero.Clan;
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x0000583C File Offset: 0x00003A3C
		private bool talkingToSubordinate()
		{
			PartyBase encounteredParty = PlayerEncounter.EncounteredParty;
			IFaction faction = (encounteredParty != null) ? encounteredParty.MapFaction : null;
			return ((encounteredParty != null) ? encounteredParty.MobileParty : null) != null && Hero.OneToOneConversationHero != null && faction != null && faction.IsKingdomFaction && faction.Leader == Hero.MainHero;
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x00005890 File Offset: 0x00003A90
		private bool heroIsFemale()
		{
			Hero oneToOneConversationHero = Hero.OneToOneConversationHero;
			return oneToOneConversationHero != null && oneToOneConversationHero.IsFemale;
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x000058B0 File Offset: 0x00003AB0
		private bool playerIsKing()
		{
			IFaction mapFaction = Hero.MainHero.MapFaction;
			return mapFaction != null && mapFaction.IsKingdomFaction && mapFaction.Leader == Hero.MainHero;
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x000058E4 File Offset: 0x00003AE4
		private ResponseType Response()
		{
			PartyBase encounteredParty = PlayerEncounter.EncounteredParty;
			if (encounteredParty == null)
			{
				return ResponseType.UnsureRefuse;
			}
			MobileParty mobileParty = encounteredParty.MobileParty;
			if (mobileParty == null)
			{
				return ResponseType.UnsureRefuse;
			}
			Hero leaderHero = encounteredParty.MobileParty.LeaderHero;
			if (leaderHero == null)
			{
				return ResponseType.UnsureRefuse;
			}
			float relationWithPlayer = leaderHero.GetRelationWithPlayer();
			float num = this.scoutData.Reputation + relationWithPlayer;
			if (num < -20f)
			{
				return ResponseType.FlatlyRefuse;
			}
			if (Hero.MainHero.Clan.Renown > 300f && num > 50f)
			{
				return ResponseType.GladlyAccept;
			}
			if (Hero.MainHero.Clan.Renown > 400f && num >= 0f)
			{
				return ResponseType.GladlyAccept;
			}
			int num2 = this.partySize(mobileParty);
			if (num > (float)(num2 + 20))
			{
				return ResponseType.GladlyAccept;
			}
			if (num > (float)num2)
			{
				return ResponseType.UnsureAccept;
			}
			if (mobileParty.MemberRoster.TotalManCount < 40)
			{
				return ResponseType.UnsureAccept;
			}
			return ResponseType.UnsureRefuse;
		}

		// Token: 0x060000F7 RID: 247 RVA: 0x000059AC File Offset: 0x00003BAC
		private int partySize(MobileParty party)
		{
			int num = party.MemberRoster.TotalManCount;
			if (party.Army != null)
			{
				foreach (MobileParty mobileParty in party.Army.Parties)
				{
					num += mobileParty.MemberRoster.TotalManCount;
				}
			}
			return num;
		}

		// Token: 0x060000F8 RID: 248 RVA: 0x00005A20 File Offset: 0x00003C20
		private void offerAccepted()
		{
			PlayerEncounter.LeaveEncounter = true;
			PartyBase encounteredParty = PlayerEncounter.EncounteredParty;
			if (encounteredParty == null)
			{
				return;
			}
			this.scoutData.setFollowingParty(encounteredParty.MobileParty);
		}

		// Token: 0x060000F9 RID: 249 RVA: 0x00005A50 File Offset: 0x00003C50
		private bool take_the_lead_condition()
		{
			PartyBase encounteredParty = PlayerEncounter.EncounteredParty;
			if (((encounteredParty != null) ? encounteredParty.MobileParty : null) == null || Hero.OneToOneConversationHero == null)
			{
				return false;
			}
			bool flag = encounteredParty.MobileParty == this.scoutData.MobileParty;
			bool flag2 = encounteredParty.MapFaction == Hero.MainHero.MapFaction;
			return !flag && flag2;
		}

		// Token: 0x060000FA RID: 250 RVA: 0x00005AA7 File Offset: 0x00003CA7
		internal void OnConversationEnded(IEnumerable<CharacterObject> enumerable)
		{
			if (this.scoutOfferMade)
			{
				this.scoutOfferMade = false;
			}
		}

		// Token: 0x060000FB RID: 251 RVA: 0x00005AB8 File Offset: 0x00003CB8
		private bool stop_leading_condition()
		{
			if (!this.scoutData.IsOngoing)
			{
				return false;
			}
			PartyBase encounteredParty = PlayerEncounter.EncounteredParty;
			return ((encounteredParty != null) ? encounteredParty.MobileParty : null) != null && Hero.OneToOneConversationHero != null && PlayerEncounter.EncounteredParty.MobileParty == this.scoutData.MobileParty;
		}

		// Token: 0x060000FC RID: 252 RVA: 0x00005B08 File Offset: 0x00003D08
		private ScoutDialogs.Release release()
		{
			float relationWithPlayer = this.scoutData.Leader.GetRelationWithPlayer();
			if (!this.isMajorFaction() && relationWithPlayer > 30f)
			{
				return ScoutDialogs.Release.CaptainFriendly;
			}
			if (!this.isMajorFaction())
			{
				return ScoutDialogs.Release.Captain;
			}
			if (relationWithPlayer > 40f && this.scoutData.Reputation > 80f)
			{
				return ScoutDialogs.Release.LordFriendly;
			}
			if (relationWithPlayer < 10f && !this.scoutData.Leader.IsFemale)
			{
				return ScoutDialogs.Release.LordIndifferent;
			}
			return ScoutDialogs.Release.Lord;
		}

		// Token: 0x04000055 RID: 85
		private ScoutMission scoutData;

		// Token: 0x04000056 RID: 86
		private bool scoutOfferMade;

		// Token: 0x04000057 RID: 87
		private int refusals;

		// Token: 0x0200007F RID: 127
		private enum ConversationHeroType
		{
			// Token: 0x04000107 RID: 263
			Vassal,
			// Token: 0x04000108 RID: 264
			ClanMember,
			// Token: 0x04000109 RID: 265
			MinorFactionHero,
			// Token: 0x0400010A RID: 266
			Lord,
			// Token: 0x0400010B RID: 267
			Lady,
			// Token: 0x0400010C RID: 268
			Neutral,
			// Token: 0x0400010D RID: 269
			Enemy,
			// Token: 0x0400010E RID: 270
			Unknown
		}

		// Token: 0x02000080 RID: 128
		private enum Release
		{
			// Token: 0x04000110 RID: 272
			CaptainFriendly,
			// Token: 0x04000111 RID: 273
			Captain,
			// Token: 0x04000112 RID: 274
			LordFriendly,
			// Token: 0x04000113 RID: 275
			Lord,
			// Token: 0x04000114 RID: 276
			LordIndifferent
		}
	}
}
