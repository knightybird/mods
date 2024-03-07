using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.MapNotificationTypes;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace SSC.debug
{
	// Token: 0x02000074 RID: 116
	public class TeleportationCampaignBehaviorMod : CampaignBehaviorBase, ITeleportationCampaignBehavior, ICampaignBehavior
	{
		// Token: 0x1700003E RID: 62
		// (get) Token: 0x0600036D RID: 877 RVA: 0x000131A2 File Offset: 0x000113A2
		private TextObject _partyLeaderChangeNotificationText
		{
			get
			{
				return new TextObject("{=QSaufZ9i}One of your parties has lost its leader. It will disband after a day has passed. You can assign a new clan member to lead it, if you wish to keep the party.", null);
			}
		}

		// Token: 0x0600036E RID: 878 RVA: 0x000131B0 File Offset: 0x000113B0
		public override void RegisterEvents()
		{
			CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(this.HourlyTick));
			CampaignEvents.DailyTickPartyEvent.AddNonSerializedListener(this, new Action<MobileParty>(this.DailyTickParty));
			CampaignEvents.HeroComesOfAgeEvent.AddNonSerializedListener(this, new Action<Hero>(this.OnHeroComesOfAge));
			CampaignEvents.MobilePartyDestroyed.AddNonSerializedListener(this, new Action<MobileParty, PartyBase>(this.OnMobilePartyDestroyed));
			CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, new Action<Settlement, bool, Hero, Hero, Hero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail>(this.OnSettlementOwnerChanged));
			CampaignEvents.HeroKilledEvent.AddNonSerializedListener(this, new Action<Hero, Hero, KillCharacterAction.KillCharacterActionDetail, bool>(this.OnHeroKilled));
			CampaignEvents.OnPartyDisbandStartedEvent.AddNonSerializedListener(this, new Action<MobileParty>(this.OnPartyDisbandStarted));
			CampaignEvents.OnGovernorChangedEvent.AddNonSerializedListener(this, new Action<Town, Hero, Hero>(this.OnGovernorChanged));
			CampaignEvents.OnHeroTeleportationRequestedEvent.AddNonSerializedListener(this, new Action<Hero, Settlement, MobileParty, TeleportHeroAction.TeleportationDetail>(this.OnHeroTeleportationRequested));
			CampaignEvents.OnPartyDisbandedEvent.AddNonSerializedListener(this, new Action<MobileParty, Settlement>(this.OnPartyDisbanded));
			CampaignEvents.OnClanLeaderChangedEvent.AddNonSerializedListener(this, new Action<Hero, Hero>(this.OnClanLeaderChanged));
		}

		// Token: 0x0600036F RID: 879 RVA: 0x000132BA File Offset: 0x000114BA
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<List<TeleportationCampaignBehaviorMod.TeleportationData>>("_teleportationList", ref this._teleportationList);
		}

		// Token: 0x06000370 RID: 880 RVA: 0x000132D0 File Offset: 0x000114D0
		public bool GetTargetOfTeleportingHero(Hero teleportingHero, out bool isGovernor, out bool isPartyLeader, out IMapPoint target)
		{
			isGovernor = false;
			isPartyLeader = false;
			target = null;
			for (int i = 0; i < this._teleportationList.Count; i++)
			{
				TeleportationCampaignBehaviorMod.TeleportationData teleportationData = this._teleportationList[i];
				if (teleportationData.TeleportingHero == teleportingHero)
				{
					if (teleportationData.TargetSettlement != null)
					{
						isGovernor = teleportationData.IsGovernor;
						target = teleportationData.TargetSettlement;
						return true;
					}
					if (teleportationData.TargetParty != null)
					{
						isPartyLeader = teleportationData.IsPartyLeader;
						target = teleportationData.TargetParty;
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06000371 RID: 881 RVA: 0x0001334C File Offset: 0x0001154C
		public CampaignTime GetHeroArrivalTimeToDestination(Hero teleportingHero)
		{
			TeleportationCampaignBehaviorMod.TeleportationData teleportationData = this._teleportationList.FirstOrDefaultQ((TeleportationCampaignBehaviorMod.TeleportationData x) => x.TeleportingHero == teleportingHero);
			if (teleportationData == null)
			{
				return CampaignTime.Never;
			}
			return teleportationData.TeleportationTime;
		}

		// Token: 0x06000372 RID: 882 RVA: 0x00013390 File Offset: 0x00011590
		private void HourlyTick()
		{
			for (int i = this._teleportationList.Count - 1; i >= 0; i--)
			{
				TeleportationCampaignBehaviorMod.TeleportationData teleportationData = this._teleportationList[i];
				if (teleportationData.TeleportationTime.IsPast && this.CanApplyImmediateTeleportation(teleportationData))
				{
					TeleportationCampaignBehaviorMod.TeleportationData data = teleportationData;
					this.RemoveTeleportationData(teleportationData, false, true);
					this.ApplyImmediateTeleport(data);
				}
			}
		}

		// Token: 0x06000373 RID: 883 RVA: 0x000133EC File Offset: 0x000115EC
		private void DailyTickParty(MobileParty mobileParty)
		{
			if (!mobileParty.IsActive || mobileParty.Army != null || mobileParty.MapEvent != null || mobileParty.LeaderHero == null || !mobileParty.LeaderHero.IsNoncombatant || mobileParty.ActualClan == null || mobileParty.ActualClan == Clan.PlayerClan || mobileParty.ActualClan.Leader == mobileParty.LeaderHero)
			{
				return;
			}
			MBList<Hero> mblist = mobileParty.ActualClan.Heroes.WhereQ((Hero h) => h.IsActive && h.IsCommander && h.PartyBelongedTo == null).ToMBList<Hero>();
			if (mblist.IsEmpty<Hero>())
			{
				return;
			}
			Hero leaderHero = mobileParty.LeaderHero;
			mobileParty.RemovePartyLeader();
			MakeHeroFugitiveAction.Apply(leaderHero);
			TeleportHeroAction.ApplyDelayedTeleportToPartyAsPartyLeader(mblist.GetRandomElementInefficiently<Hero>(), mobileParty);
		}

		// Token: 0x06000374 RID: 884 RVA: 0x000134AC File Offset: 0x000116AC
		private void OnHeroComesOfAge(Hero hero)
		{
			if (hero.Clan == Clan.PlayerClan || hero.IsNoncombatant)
			{
				return;
			}
			foreach (WarPartyComponent warPartyComponent in hero.Clan.WarPartyComponents)
			{
				MobileParty mobileParty = warPartyComponent.MobileParty;
				if (mobileParty != null && mobileParty.Army == null && mobileParty.MapEvent == null && mobileParty.LeaderHero != null && mobileParty.LeaderHero.IsNoncombatant)
				{
					Hero leaderHero = mobileParty.LeaderHero;
					mobileParty.RemovePartyLeader();
					MakeHeroFugitiveAction.Apply(leaderHero);
					TeleportHeroAction.ApplyDelayedTeleportToPartyAsPartyLeader(hero, warPartyComponent.Party.MobileParty);
					break;
				}
			}
		}

		// Token: 0x06000375 RID: 885 RVA: 0x00013568 File Offset: 0x00011768
		private void OnMobilePartyDestroyed(MobileParty mobileParty, PartyBase destroyerParty)
		{
			for (int i = this._teleportationList.Count - 1; i >= 0; i--)
			{
				TeleportationCampaignBehaviorMod.TeleportationData teleportationData = this._teleportationList[i];
				if (teleportationData.TargetParty != null && teleportationData.TargetParty == mobileParty)
				{
					this.RemoveTeleportationData(teleportationData, true, true);
				}
			}
		}

		// Token: 0x06000376 RID: 886 RVA: 0x000135B4 File Offset: 0x000117B4
		private void OnSettlementOwnerChanged(Settlement settlement, bool openToClaim, Hero newOwner, Hero oldOwner, Hero capturerHero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
		{
			for (int i = this._teleportationList.Count - 1; i >= 0; i--)
			{
				TeleportationCampaignBehaviorMod.TeleportationData teleportationData = this._teleportationList[i];
				if (teleportationData.TargetSettlement != null && teleportationData.TargetSettlement == settlement && newOwner.Clan != teleportationData.TeleportingHero.Clan)
				{
					this.RemoveTeleportationData(teleportationData, true, true);
				}
			}
		}

		// Token: 0x06000377 RID: 887 RVA: 0x00013614 File Offset: 0x00011814
		private void OnHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification = true)
		{
			for (int i = this._teleportationList.Count - 1; i >= 0; i--)
			{
				TeleportationCampaignBehaviorMod.TeleportationData teleportationData = this._teleportationList[i];
				if (teleportationData.TeleportingHero == victim)
				{
					this.RemoveTeleportationData(teleportationData, true, true);
				}
			}
		}

		// Token: 0x06000378 RID: 888 RVA: 0x00013658 File Offset: 0x00011858
		private void OnPartyDisbandStarted(MobileParty disbandParty)
		{
			if (disbandParty.ActualClan == Clan.PlayerClan && disbandParty.LeaderHero == null && (disbandParty.IsLordParty || disbandParty.IsCaravan))
			{
				Campaign.Current.CampaignInformationManager.NewMapNoticeAdded(new PartyLeaderChangeNotification(disbandParty, this._partyLeaderChangeNotificationText));
			}
			for (int i = this._teleportationList.Count - 1; i >= 0; i--)
			{
				TeleportationCampaignBehaviorMod.TeleportationData teleportationData = this._teleportationList[i];
				if (teleportationData.TargetParty != null && teleportationData.TargetParty == disbandParty)
				{
					this.RemoveTeleportationData(teleportationData, true, false);
				}
			}
		}

		// Token: 0x06000379 RID: 889 RVA: 0x000136E4 File Offset: 0x000118E4
		private void OnGovernorChanged(Town fortification, Hero oldGovernor, Hero newGovernor)
		{
			for (int i = this._teleportationList.Count - 1; i >= 0; i--)
			{
				TeleportationCampaignBehaviorMod.TeleportationData teleportationData = this._teleportationList[i];
				if (teleportationData.TeleportingHero != newGovernor && teleportationData.IsGovernor && teleportationData.TargetSettlement.Town == fortification)
				{
					teleportationData.IsGovernor = false;
				}
			}
		}

		// Token: 0x0600037A RID: 890 RVA: 0x0001373C File Offset: 0x0001193C
		private void OnHeroTeleportationRequested(Hero hero, Settlement targetSettlement, MobileParty targetParty, TeleportHeroAction.TeleportationDetail detail)
		{
			switch (detail)
			{
			case TeleportHeroAction.TeleportationDetail.ImmediateTeleportToSettlement:
				for (int i = this._teleportationList.Count - 1; i >= 0; i--)
				{
					TeleportationCampaignBehaviorMod.TeleportationData teleportationData = this._teleportationList[i];
					if (hero == teleportationData.TeleportingHero && teleportationData.TargetSettlement == targetSettlement)
					{
						this.RemoveTeleportationData(teleportationData, true, false);
					}
				}
				return;
			case TeleportHeroAction.TeleportationDetail.ImmediateTeleportToParty:
			case TeleportHeroAction.TeleportationDetail.ImmediateTeleportToPartyAsPartyLeader:
				break;
			case TeleportHeroAction.TeleportationDetail.DelayedTeleportToSettlement:
			case TeleportHeroAction.TeleportationDetail.DelayedTeleportToSettlementAsGovernor:
				this._teleportationList.Add(new TeleportationCampaignBehaviorMod.TeleportationData(hero, targetSettlement, detail == TeleportHeroAction.TeleportationDetail.DelayedTeleportToSettlementAsGovernor));
				return;
			case TeleportHeroAction.TeleportationDetail.DelayedTeleportToParty:
			case TeleportHeroAction.TeleportationDetail.DelayedTeleportToPartyAsPartyLeader:
				if (detail == TeleportHeroAction.TeleportationDetail.DelayedTeleportToPartyAsPartyLeader)
				{
					for (int j = this._teleportationList.Count - 1; j >= 0; j--)
					{
						TeleportationCampaignBehaviorMod.TeleportationData teleportationData2 = this._teleportationList[j];
						if (teleportationData2.TargetParty == targetParty && teleportationData2.IsPartyLeader)
						{
							this.RemoveTeleportationData(teleportationData2, true, false);
						}
					}
				}
				this._teleportationList.Add(new TeleportationCampaignBehaviorMod.TeleportationData(hero, targetParty, detail == TeleportHeroAction.TeleportationDetail.DelayedTeleportToPartyAsPartyLeader));
				break;
			default:
				return;
			}
		}

		// Token: 0x0600037B RID: 891 RVA: 0x00013824 File Offset: 0x00011A24
		private void OnPartyDisbanded(MobileParty disbandParty, Settlement relatedSettlement)
		{
			for (int i = this._teleportationList.Count - 1; i >= 0; i--)
			{
				TeleportationCampaignBehaviorMod.TeleportationData teleportationData = this._teleportationList[i];
				if (teleportationData.TargetParty != null && teleportationData.TargetParty == disbandParty)
				{
					this.RemoveTeleportationData(teleportationData, true, true);
				}
			}
		}

		// Token: 0x0600037C RID: 892 RVA: 0x00013870 File Offset: 0x00011A70
		private void OnClanLeaderChanged(Hero oldLeader, Hero newLeader)
		{
			for (int i = this._teleportationList.Count - 1; i >= 0; i--)
			{
				TeleportationCampaignBehaviorMod.TeleportationData teleportationData = this._teleportationList[i];
				if (teleportationData.TeleportingHero == newLeader && !teleportationData.IsPartyLeader)
				{
					this.RemoveTeleportationData(teleportationData, true, true);
				}
			}
		}

		// Token: 0x0600037D RID: 893 RVA: 0x000138BC File Offset: 0x00011ABC
		private void RemoveTeleportationData(TeleportationCampaignBehaviorMod.TeleportationData data, bool isCanceled, bool disbandTargetParty = true)
		{
			this._teleportationList.Remove(data);
			if (!isCanceled)
			{
				return;
			}
			if (data.TeleportingHero.IsTraveling && data.TeleportingHero.DeathMark == KillCharacterAction.KillCharacterActionDetail.None)
			{
				MakeHeroFugitiveAction.Apply(data.TeleportingHero);
			}
			if (data.TargetParty == null)
			{
				return;
			}
			if (data.TargetParty.ActualClan == Clan.PlayerClan)
			{
				CampaignEventDispatcher.Instance.OnPartyLeaderChangeOfferCanceled(data.TargetParty);
			}
			if (!disbandTargetParty || !data.TargetParty.IsActive || !data.IsPartyLeader)
			{
				return;
			}
			IDisbandPartyCampaignBehavior behavior = Campaign.Current.CampaignBehaviorManager.GetBehavior<IDisbandPartyCampaignBehavior>();
			if (behavior == null || behavior.IsPartyWaitingForDisband(data.TargetParty))
			{
				return;
			}
			DisbandPartyAction.StartDisband(data.TargetParty);
		}

		// Token: 0x0600037E RID: 894 RVA: 0x00013970 File Offset: 0x00011B70
		private bool CanApplyImmediateTeleportation(TeleportationCampaignBehaviorMod.TeleportationData data)
		{
			return (data.TargetSettlement != null && !data.TargetSettlement.IsUnderSiege && !data.TargetSettlement.IsUnderRaid) || (data.TargetParty != null && data.TargetParty.MapEvent == null);
		}

		// Token: 0x0600037F RID: 895 RVA: 0x000139B0 File Offset: 0x00011BB0
		private void ApplyImmediateTeleport(TeleportationCampaignBehaviorMod.TeleportationData data)
		{
			if (data.TargetSettlement != null)
			{
				if (data.IsGovernor)
				{
					data.TargetSettlement.Town.Governor = data.TeleportingHero;
					TeleportHeroAction.ApplyImmediateTeleportToSettlement(data.TeleportingHero, data.TargetSettlement);
					return;
				}
				TeleportHeroAction.ApplyImmediateTeleportToSettlement(data.TeleportingHero, data.TargetSettlement);
				return;
			}
			else
			{
				if (data.TargetParty == null)
				{
					return;
				}
				if (data.IsPartyLeader)
				{
					TeleportHeroAction.ApplyImmediateTeleportToPartyAsPartyLeader(data.TeleportingHero, data.TargetParty);
					return;
				}
				TeleportHeroAction.ApplyImmediateTeleportToParty(data.TeleportingHero, data.TargetParty);
				return;
			}
		}

		// Token: 0x040000FA RID: 250
		private List<TeleportationCampaignBehaviorMod.TeleportationData> _teleportationList = new List<TeleportationCampaignBehaviorMod.TeleportationData>();

		// Token: 0x020000B7 RID: 183
		internal class TeleportationData
		{
			// Token: 0x06000464 RID: 1124 RVA: 0x000148D0 File Offset: 0x00012AD0
			public TeleportationData(Hero teleportingHero, Settlement targetSettlement, bool isGovernor)
			{
				this.TeleportingHero = teleportingHero;
				this.TeleportationTime = CampaignTime.HoursFromNow(Campaign.Current.Models.DelayedTeleportationModel.GetTeleportationDelayAsHours(teleportingHero, targetSettlement.Party).ResultNumber);
				this.TargetSettlement = targetSettlement;
				this.IsGovernor = isGovernor;
				this.TargetParty = null;
				this.IsPartyLeader = false;
			}

			// Token: 0x06000465 RID: 1125 RVA: 0x00014934 File Offset: 0x00012B34
			public TeleportationData(Hero teleportingHero, MobileParty targetParty, bool isPartyLeader)
			{
				this.TeleportingHero = teleportingHero;
				this.TeleportationTime = CampaignTime.HoursFromNow(Campaign.Current.Models.DelayedTeleportationModel.GetTeleportationDelayAsHours(teleportingHero, targetParty.Party).ResultNumber);
				this.TargetParty = targetParty;
				this.IsPartyLeader = isPartyLeader;
				this.TargetSettlement = null;
				this.IsGovernor = false;
			}

			// Token: 0x06000466 RID: 1126 RVA: 0x00014998 File Offset: 0x00012B98
			internal static void AutoGeneratedStaticCollectObjectsTeleportationData(object o, List<object> collectedObjects)
			{
				((TeleportationCampaignBehaviorMod.TeleportationData)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			// Token: 0x06000467 RID: 1127 RVA: 0x000149A6 File Offset: 0x00012BA6
			protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				collectedObjects.Add(this.TeleportingHero);
				CampaignTime.AutoGeneratedStaticCollectObjectsCampaignTime(this.TeleportationTime, collectedObjects);
				collectedObjects.Add(this.TargetSettlement);
				collectedObjects.Add(this.TargetParty);
			}

			// Token: 0x06000468 RID: 1128 RVA: 0x000149DD File Offset: 0x00012BDD
			internal static object AutoGeneratedGetMemberValueTeleportingHero(object o)
			{
				return ((TeleportationCampaignBehaviorMod.TeleportationData)o).TeleportingHero;
			}

			// Token: 0x06000469 RID: 1129 RVA: 0x000149EA File Offset: 0x00012BEA
			internal static object AutoGeneratedGetMemberValueTeleportationTime(object o)
			{
				return ((TeleportationCampaignBehaviorMod.TeleportationData)o).TeleportationTime;
			}

			// Token: 0x0600046A RID: 1130 RVA: 0x000149FC File Offset: 0x00012BFC
			internal static object AutoGeneratedGetMemberValueTargetSettlement(object o)
			{
				return ((TeleportationCampaignBehaviorMod.TeleportationData)o).TargetSettlement;
			}

			// Token: 0x0600046B RID: 1131 RVA: 0x00014A09 File Offset: 0x00012C09
			internal static object AutoGeneratedGetMemberValueIsGovernor(object o)
			{
				return ((TeleportationCampaignBehaviorMod.TeleportationData)o).IsGovernor;
			}

			// Token: 0x0600046C RID: 1132 RVA: 0x00014A1B File Offset: 0x00012C1B
			internal static object AutoGeneratedGetMemberValueTargetParty(object o)
			{
				return ((TeleportationCampaignBehaviorMod.TeleportationData)o).TargetParty;
			}

			// Token: 0x0600046D RID: 1133 RVA: 0x00014A28 File Offset: 0x00012C28
			internal static object AutoGeneratedGetMemberValueIsPartyLeader(object o)
			{
				return ((TeleportationCampaignBehaviorMod.TeleportationData)o).IsPartyLeader;
			}

			// Token: 0x040001A5 RID: 421
			[SaveableField(1)]
			public Hero TeleportingHero;

			// Token: 0x040001A6 RID: 422
			[SaveableField(2)]
			public CampaignTime TeleportationTime;

			// Token: 0x040001A7 RID: 423
			[SaveableField(3)]
			public Settlement TargetSettlement;

			// Token: 0x040001A8 RID: 424
			[SaveableField(4)]
			public bool IsGovernor;

			// Token: 0x040001A9 RID: 425
			[SaveableField(5)]
			public MobileParty TargetParty;

			// Token: 0x040001AA RID: 426
			[SaveableField(6)]
			public bool IsPartyLeader;
		}
	}
}
