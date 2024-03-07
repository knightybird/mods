using System;
using System.Linq;
using Helpers;
using SSC.misc;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace SSC.world.escort
{
	// Token: 0x02000021 RID: 33
	public class EscortEvents
	{
		// Token: 0x06000171 RID: 369 RVA: 0x000082BC File Offset: 0x000064BC
		public EscortEvents(EscortData escort, EscortBattlesPerformance performance)
		{
			this.escort = escort;
			this.performance = performance;
			this.escortMenus = new EscortMenus(escort);
			this.escortMenus.onCaravanMenuOptionSelected += this.fillTheTownsListAndSetMenuTexts;
			this.escortMenus.onDestinationSelected += this.destinationSelected;
		}

		// Token: 0x06000172 RID: 370 RVA: 0x00008322 File Offset: 0x00006522
		public void onSessionLaunched(CampaignGameStarter starter)
		{
			this.escortMenus.AddGameMenus(starter);
			this.escortMenus.AddCaravanDialogs(starter);
		}

		// Token: 0x06000173 RID: 371 RVA: 0x0000833C File Offset: 0x0000653C
		internal void onGameLoadFinished()
		{
		}

		// Token: 0x06000174 RID: 372 RVA: 0x0000833E File Offset: 0x0000653E
		public void fillTheTownsListAndSetMenuTexts()
		{
			this.escort.closestTowns = DistanceManager.findClosestTowns(Settlement.CurrentSettlement, 6);
			this.escortMenus.setMenuEntriesText(this.escort.closestTowns);
		}

		// Token: 0x06000175 RID: 373 RVA: 0x0000836C File Offset: 0x0000656C
		public void destinationSelected(MenuCallbackArgs args, int townIndex)
		{
			this.escort.destination = this.escort.closestTowns[townIndex];
			this.escort.originTown = Settlement.CurrentSettlement;
			this.escort.caravan = this.helper.SpawnCaravan(this.escort.originTown);
			this.escort.banditsSpawned = false;
			this.escort.agreedPayment = this.escort.calculatePayment(this.escort.originTown, this.escort.destination);
			this.performance.newEscortTaskStarted();
			PlayerEncounter.LeaveSettlement();
			PlayerEncounter.Finish(true);
			this.escort.IsOngoing = true;
			Campaign.Current.SaveHandler.SignalAutoSave();
		}

		// Token: 0x06000176 RID: 374 RVA: 0x00008430 File Offset: 0x00006630
		public void onHourlyTick()
		{
			if (!this.escort.IsOngoing)
			{
				return;
			}
			if (this.escort.caravan.TargetSettlement == null)
			{
				this.setCaravanToDestination();
			}
			else if (this.escort.caravan.MapEvent == null)
			{
				this.adjustCaravanSpeed();
			}
			if (this.escort.caravan.TargetSettlement != null && !this.escort.banditsSpawned && this.escort.bandits == null)
			{
				this.escort.bandits = BanditPartyManager.SpawnBanditParty(this.escort, this.performance);
				BanditPartyManager.attackCaravan(this.escort.bandits, this.escort.caravan);
				this.escort.banditsSpawned = true;
			}
			if (this.escort.bandits != null && this.escort.bandits.IsActive)
			{
				Campaign.Current.Models.MapDistanceModel.GetDistance(this.escort.bandits, MobileParty.MainParty);
			}
			if (this.escort.caravan.TargetSettlement == null && this.escort.bandits != null && this.escort.bandits.IsActive)
			{
				BanditPartyManager.attackCaravan(this.escort.bandits, this.escort.caravan);
			}
		}

		// Token: 0x06000177 RID: 375 RVA: 0x00008584 File Offset: 0x00006784
		public void onSettlementEntered(MobileParty party, Settlement settlement, Hero hero)
		{
			if (!this.escort.IsOngoing)
			{
				return;
			}
			if (this.escort.caravan == null)
			{
				return;
			}
			if (party == this.escort.caravan && settlement == this.escort.caravan.TargetSettlement && settlement != this.escort.caravan.HomeSettlement)
			{
				if (settlement.GatePosition.NearlyEquals(MobileParty.MainParty.Position2D, MobileParty.MainParty.SeeingRange + 2f))
				{
					this.caravanReachesDestinationSuccessfuly(settlement);
					return;
				}
				this.escort.finalizeEscortTask();
			}
		}

		// Token: 0x06000178 RID: 376 RVA: 0x00008620 File Offset: 0x00006820
		private void caravanReachesDestinationSuccessfuly(Settlement settlement)
		{
			TextObject textObject = new TextObject("{=0wj3HIbh}Caravan entered {SETTLEMENT_LINK}.", null);
			textObject.SetTextVariable("SETTLEMENT_LINK", settlement.EncyclopediaLinkWithName);
			MBInformationManager.AddQuickInformation(textObject, 100, PartyBaseHelper.GetVisualPartyLeader(this.escort.caravan.Party), "");
			this.successConsequences();
		}

		// Token: 0x06000179 RID: 377 RVA: 0x00008671 File Offset: 0x00006871
		private void banditBattleEnded(MapEvent mapEvent)
		{
		}

		// Token: 0x0600017A RID: 378 RVA: 0x00008674 File Offset: 0x00006874
		public void onMapEventEnded(MapEvent mapEvent)
		{
			if (!this.escort.IsOngoing)
			{
				return;
			}
			if (this.escort.bandits != null && mapEvent.InvolvedParties.Contains(this.escort.bandits.Party))
			{
				this.banditBattleEnded(mapEvent);
			}
			if (this.escort.caravan == null || !mapEvent.InvolvedParties.Contains(this.escort.caravan.Party))
			{
				return;
			}
			if (mapEvent.HasWinner)
			{
				bool flag = this.escort.caravan.MapEventSide == MobileParty.MainParty.MapEventSide && mapEvent.IsPlayerMapEvent;
				bool flag2 = mapEvent.Winner == this.escort.caravan.MapEventSide;
				bool flag3 = mapEvent.InvolvedParties.Contains(PartyBase.MainParty);
				if (!flag2)
				{
					if (!flag3 || flag)
					{
						this.failConsequences();
					}
					return;
				}
				if (mapEvent.PlayerSide == BattleSideEnum.None)
				{
					this.performance.currentFormation.grow(5, true);
					this.removeBandits();
				}
				this.handleCaravanDeadOrTooSlow();
				return;
			}
			else
			{
				if (this.escort.caravan.MemberRoster.TotalManCount <= 0)
				{
					this.failConsequences();
					return;
				}
				if (this.escort.bandits != null && this.escort.caravan != null)
				{
					if (this.escort.caravan.TargetSettlement == null)
					{
						this.setCaravanToDestination();
					}
					BanditPartyManager.attackCaravan(this.escort.bandits, this.escort.caravan);
				}
				return;
			}
		}

		// Token: 0x0600017B RID: 379 RVA: 0x000087ED File Offset: 0x000069ED
		public void battleStarted(PartyBase attackerParty, PartyBase defenderParty, object subject, bool showNotification)
		{
		}

		// Token: 0x0600017C RID: 380 RVA: 0x000087F0 File Offset: 0x000069F0
		public void onPlayerBattleEnd(MapEvent mapEvent)
		{
			if (this.escort.bandits == null)
			{
				return;
			}
			if (!mapEvent.InvolvedParties.Contains(this.escort.bandits.Party))
			{
				return;
			}
			if (mapEvent.PlayerSide == BattleSideEnum.None)
			{
				return;
			}
			this.performance.battleFinished(mapEvent, MobileParty.MainParty, this.escort.bandits);
			if (mapEvent.WinningSide == mapEvent.PlayerSide)
			{
				this.removeBandits();
				return;
			}
			if (this.escort.bandits.IsActive)
			{
				BanditPartyManager.attackCaravan(this.escort.bandits, this.escort.caravan);
			}
		}

		// Token: 0x0600017D RID: 381 RVA: 0x00008891 File Offset: 0x00006A91
		private void startTracking(Settlement settlement)
		{
			if (settlement != null && !this.escort.isTracked(settlement))
			{
				Campaign.Current.VisualTrackerManager.RegisterObject(settlement);
			}
		}

		// Token: 0x0600017E RID: 382 RVA: 0x000088B4 File Offset: 0x00006AB4
		private void setCaravanToDestination()
		{
			SetPartyAiAction.GetActionForVisitingSettlement(this.escort.caravan, this.escort.destination);
			this.escort.caravan.Ai.SetDoNotMakeNewDecisions(true);
			TextObject textObject = new TextObject("{=OjI8uGFa}We are traveling to {SETTLEMENT_NAME}.", null);
			textObject.SetTextVariable("SETTLEMENT_NAME", this.escort.destination.Name);
			MBInformationManager.AddQuickInformation(textObject, 100, PartyBaseHelper.GetVisualPartyLeader(this.escort.caravan.Party), "");
			new TextObject("{=QDpfYm4c}The caravan is moving to {SETTLEMENT_NAME}.", null).SetTextVariable("SETTLEMENT_NAME", this.escort.destination.EncyclopediaLinkWithName);
			this.startTracking(this.escort.destination);
			SetPartyAiAction.GetActionForVisitingSettlement(MobileParty.MainParty, this.escort.destination);
		}

		// Token: 0x0600017F RID: 383 RVA: 0x00008988 File Offset: 0x00006B88
		private void adjustCaravanSpeed()
		{
			float speed = MobileParty.MainParty.Speed;
			float speed2 = this.escort.caravan.Speed;
			float num = speed - speed2;
			if (num > 1f)
			{
				PartyMisc.changeCustomPartyComponentSpeed(this.escort.caravan, 0.05f * (float)((int)(num / 0.05f)));
				return;
			}
			if (num < 0f)
			{
				PartyMisc.changeCustomPartyComponentSpeed(this.escort.caravan, -0.05f * (float)((int)(-num / 0.05f)));
			}
		}

		// Token: 0x06000180 RID: 384 RVA: 0x00008A04 File Offset: 0x00006C04
		private void CheckWarDeclaration()
		{
			if (this.escort.caravan != null && this.escort.caravan.MapFaction != null && this.escort.caravan.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction))
			{
				this.endTaskDueToWarDeclarationNotification();
				this.escort.finalizeEscortTask();
			}
		}

		// Token: 0x06000181 RID: 385 RVA: 0x00008A64 File Offset: 0x00006C64
		private bool IsCaravanInvolvedInBattle(IFaction faction1, IFaction faction2)
		{
			return (faction1 == this.escort.caravan.MapFaction || faction2 == this.escort.caravan.MapFaction) && PlayerEncounter.Battle != null && this.escort.caravan.MapEvent == PlayerEncounter.Battle;
		}

		// Token: 0x06000182 RID: 386 RVA: 0x00008AB7 File Offset: 0x00006CB7
		private bool IsCaravanAtWarWithMainHero()
		{
			return this.escort.caravan.ActualClan.IsAtWarWith(Hero.MainHero.MapFaction);
		}

		// Token: 0x06000183 RID: 387 RVA: 0x00008AD8 File Offset: 0x00006CD8
		private bool IsDestinationAtWarWithCaravan()
		{
			return this.escort.caravan != null && this.escort.destination.MapFaction.IsAtWarWith(this.escort.caravan.MapFaction) && this.escort.IsOngoing;
		}

		// Token: 0x06000184 RID: 388 RVA: 0x00008B26 File Offset: 0x00006D26
		private void EndTaskDueToWarDeclaration()
		{
			this.endTaskDueToWarDeclarationNotification();
			this.escort.finalizeEscortTask();
		}

		// Token: 0x06000185 RID: 389 RVA: 0x00008B39 File Offset: 0x00006D39
		private void endTaskDueToWarDeclarationNotification()
		{
			MBInformationManager.AddQuickInformation(new TextObject("Escort task ended due to declaration of war.", null), 100, PartyBaseHelper.GetVisualPartyLeader(this.escort.caravan.Party), "");
		}

		// Token: 0x06000186 RID: 390 RVA: 0x00008B68 File Offset: 0x00006D68
		private void removeBandits()
		{
			if (this.escort.bandits != null && this.escort.bandits.IsActive)
			{
				DestroyPartyAction.Apply(MobileParty.MainParty.Party, this.escort.bandits);
				this.escort.removeBandits();
			}
		}

		// Token: 0x06000187 RID: 391 RVA: 0x00008BBC File Offset: 0x00006DBC
		private void handleCaravanDeadOrTooSlow()
		{
			if (this.escort.caravan.MemberRoster.TotalManCount <= 0)
			{
				this.failConsequences();
				return;
			}
			if (this.escort.caravan.Speed < 2f)
			{
				this.escort.caravan.ItemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("sumpter_horse"), 5);
			}
		}

		// Token: 0x06000188 RID: 392 RVA: 0x00008C28 File Offset: 0x00006E28
		private void failConsequences()
		{
			MBInformationManager.AddQuickInformation(new TextObject("Escort mission failed.", null), 100, PartyBaseHelper.GetVisualPartyLeader(this.escort.caravan.Party), "");
			this.escort.originTown.Town.Prosperity -= 5f;
			this.escort.finalizeEscortTask();
		}

		// Token: 0x06000189 RID: 393 RVA: 0x00008C90 File Offset: 0x00006E90
		private void successConsequences()
		{
			GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, this.escort.agreedPayment, false);
			this.escort.originTown.Town.Prosperity += 5f;
			this.escort.finalizeEscortTask();
		}

		// Token: 0x04000082 RID: 130
		private EscortMenus escortMenus;

		// Token: 0x04000083 RID: 131
		private EscortData escort;

		// Token: 0x04000084 RID: 132
		private EscortBattlesPerformance performance;

		// Token: 0x04000085 RID: 133
		private EscortHelper helper = new EscortHelper();
	}
}
