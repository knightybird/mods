using System;
using System.Linq;
using SSC.misc;
using SSC.world.escort;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace SSC.world.castle_party
{
	// Token: 0x0200002A RID: 42
	public class CastleParty
	{
		// Token: 0x1700002F RID: 47
		// (get) Token: 0x060001BA RID: 442 RVA: 0x00009940 File Offset: 0x00007B40
		public bool IsOngoing
		{
			get
			{
				return this.isOngoing;
			}
		}

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x060001BB RID: 443 RVA: 0x00009948 File Offset: 0x00007B48
		public MobileParty Party
		{
			get
			{
				return this.party;
			}
		}

		// Token: 0x060001BC RID: 444 RVA: 0x00009950 File Offset: 0x00007B50
		public void sendPunishmentPartyFromNearbyCastle(Settlement village, MobileParty offendingParty)
		{
			if (this.isOngoing)
			{
				return;
			}
			if (village == null)
			{
				return;
			}
			this.offender = offendingParty;
			Settlement settlement = DistanceManager.findCastleOf(village);
			if (settlement == null)
			{
				return;
			}
			if (settlement.IsUnderSiege)
			{
				return;
			}
			if (CastlePartyHelper.isGarrisonStrongEnough(settlement, this.offender))
			{
				int num = CastlePartyHelper.tierCount(offendingParty);
				num = (int)Math.Ceiling((double)((float)num * 1.1f));
				if (num < 10)
				{
					num = 10;
				}
				bool isPatrol = num <= 15;
				MobileParty mobileParty = this.spawnCastleParty(settlement, isPatrol);
				CastlePartyHelper.loadTroops(settlement, mobileParty, num + World.PlayerStrength.Value);
				SetPartyAiAction.GetActionForEngagingParty(mobileParty, this.offender);
				PartyMisc.setCustomPartyComponentSpeed(this.party, CastlePartyHelper.calculateSpeed(this.party));
				this.isOngoing = true;
			}
		}

		// Token: 0x060001BD RID: 445 RVA: 0x00009A00 File Offset: 0x00007C00
		private MobileParty spawnCastleParty(Settlement castle, bool isPatrol)
		{
			if (castle == null)
			{
				throw new ArgumentNullException("castle");
			}
			string arg = isPatrol ? "Patrol" : "Detachment";
			TextObject name = new TextObject(string.Format("{0} {1}", castle.Name, arg), null);
			this.party = CustomPartyComponent.CreateQuestParty(castle.GatePosition, 0f, castle, name, castle.OwnerClan, TroopRoster.CreateDummyTroopRoster(), TroopRoster.CreateDummyTroopRoster(), castle.Owner, "", "", 4f, false);
			ItemObject @object = MBObjectManager.Instance.GetObject<ItemObject>("grain");
			if (@object != null)
			{
				this.party.ItemRoster.AddToCounts(@object, 20);
			}
			this.party.Ai.SetDoNotMakeNewDecisions(true);
			return this.party;
		}

		// Token: 0x060001BE RID: 446 RVA: 0x00009AC0 File Offset: 0x00007CC0
		internal void OnHourlyTick()
		{
			if (!this.isOngoing)
			{
				return;
			}
			if (this.needsToReturnHome())
			{
				this.goHome();
				return;
			}
			if (this.offender == null)
			{
				SetPartyAiAction.GetActionForVisitingSettlement(this.party, this.party.HomeSettlement);
			}
			else
			{
				SetPartyAiAction.GetActionForEngagingParty(this.party, this.offender);
			}
			if (this.party.TargetSettlement != null)
			{
				this.party.TargetSettlement.Name.ToString();
				return;
			}
			if (this.party.TargetParty != null)
			{
				this.party.TargetParty.Name.ToString();
			}
		}

		// Token: 0x060001BF RID: 447 RVA: 0x00009B60 File Offset: 0x00007D60
		private bool needsToReturnHome()
		{
			Settlement settlement = DistanceManager.findClosestSettlements(this.party, 1).FirstOrDefault<Settlement>();
			return settlement != null && settlement != this.party.HomeSettlement && settlement.MapFaction.IsAtWarWith(this.party.MapFaction);
		}

		// Token: 0x060001C0 RID: 448 RVA: 0x00009BAB File Offset: 0x00007DAB
		private void endTracking()
		{
			this.isOngoing = false;
			this.party = null;
			this.offender = null;
		}

		// Token: 0x060001C1 RID: 449 RVA: 0x00009BC4 File Offset: 0x00007DC4
		internal void OnMapEventEnded(MapEvent mapEvent)
		{
			if (!this.isOngoing)
			{
				return;
			}
			if (!mapEvent.InvolvedParties.Any((PartyBase p) => p == this.party.Party))
			{
				return;
			}
			if (this.party.MemberRoster.TotalManCount == 0)
			{
				this.updatePlayerStrength(mapEvent);
				this.endTracking();
				return;
			}
			if (PartyMisc.HasPartyLost(this.party, mapEvent))
			{
				this.updatePlayerStrength(mapEvent);
				this.goHome();
				return;
			}
			if (this.offender == null)
			{
				this.goHome();
				return;
			}
			if (this.offender.MemberRoster.TotalManCount == 0)
			{
				this.goHome();
				return;
			}
			SetPartyAiAction.GetActionForEngagingParty(this.party, this.offender);
		}

		// Token: 0x060001C2 RID: 450 RVA: 0x00009C6C File Offset: 0x00007E6C
		private void updatePlayerStrength(MapEvent mapEvent)
		{
			if (mapEvent.InvolvedParties.Any((PartyBase p) => p == MobileParty.MainParty.Party))
			{
				int totalHealthyCount = MobileParty.MainParty.MemberRoster.TotalHealthyCount;
				if (totalHealthyCount <= 5)
				{
					return;
				}
				int num = Math.Min(totalHealthyCount - 5, 16);
				World.PlayerStrength.Value += num;
			}
		}

		// Token: 0x060001C3 RID: 451 RVA: 0x00009CD7 File Offset: 0x00007ED7
		private void goHome()
		{
			if (this.party == null)
			{
				return;
			}
			this.offender = null;
			this.party.Ai.SetDoNotMakeNewDecisions(true);
			SetPartyAiAction.GetActionForVisitingSettlement(this.party, this.party.HomeSettlement);
		}

		// Token: 0x060001C4 RID: 452 RVA: 0x00009D10 File Offset: 0x00007F10
		internal void OnSettlementEntered(MobileParty enteringParty, Settlement castle, Hero hero)
		{
			if (!this.isOngoing)
			{
				return;
			}
			if (enteringParty != this.party)
			{
				return;
			}
			Settlement homeSettlement = this.party.HomeSettlement;
			MobileParty mobileParty = castle.Parties.FirstOrDefault((MobileParty p) => p.IsGarrison);
			if (mobileParty == null)
			{
				return;
			}
			foreach (TroopRosterElement troopRosterElement in this.party.MemberRoster.GetTroopRoster())
			{
				this.party.MemberRoster.RemoveTroop(troopRosterElement.Character, troopRosterElement.Number, default(UniqueTroopDescriptor), 0);
				mobileParty.MemberRoster.AddToCounts(troopRosterElement.Character, troopRosterElement.Number, false, 0, 0, true, -1);
			}
			this.movePrisonersToCastleDungeon(this.party, castle);
			DestroyPartyAction.Apply(this.party.Party, this.party);
			this.endTracking();
		}

		// Token: 0x060001C5 RID: 453 RVA: 0x00009E24 File Offset: 0x00008024
		private void movePrisonersToCastleDungeon(MobileParty party, Settlement castle)
		{
			if (party.PrisonRoster.TotalManCount == 0)
			{
				return;
			}
			TroopRoster prisonRoster = party.PrisonRoster;
			TroopRoster prisonRoster2 = castle.Party.PrisonRoster;
			for (int i = prisonRoster.Count - 1; i >= 0; i--)
			{
				TroopRosterElement elementCopyAtIndex = prisonRoster.GetElementCopyAtIndex(i);
				if (elementCopyAtIndex.Character.IsHero)
				{
					EnterSettlementAction.ApplyForPrisoner(elementCopyAtIndex.Character.HeroObject, castle);
				}
				prisonRoster2.AddToCounts(elementCopyAtIndex.Character, elementCopyAtIndex.Number, false, 0, 0, true, -1);
				prisonRoster.RemoveTroop(elementCopyAtIndex.Character, elementCopyAtIndex.Number, default(UniqueTroopDescriptor), 0);
			}
		}

		// Token: 0x060001C6 RID: 454 RVA: 0x00009EC4 File Offset: 0x000080C4
		public void AddDialogs(CampaignGameStarter starter)
		{
			starter.AddDialogLine("castle_detachment_start_id", "start", "close_window", "Just surrender.", new ConversationSentence.OnConditionDelegate(this.CastleDetachmentTalkOnCondition), null, 100, null);
		}

		// Token: 0x060001C7 RID: 455 RVA: 0x00009EFC File Offset: 0x000080FC
		private bool CastleDetachmentTalkOnCondition()
		{
			if (!this.isOngoing)
			{
				return false;
			}
			PartyBase encounteredParty = PlayerEncounter.EncounteredParty;
			return encounteredParty != null && encounteredParty.MobileParty != null && encounteredParty.MobileParty == this.party;
		}

		// Token: 0x0400008F RID: 143
		[SaveableField(1)]
		private bool isOngoing;

		// Token: 0x04000090 RID: 144
		[SaveableField(2)]
		private MobileParty party;

		// Token: 0x04000091 RID: 145
		[SaveableField(3)]
		private MobileParty offender;

		// Token: 0x04000092 RID: 146
		private const int balanceThreshold = 5;
	}
}
