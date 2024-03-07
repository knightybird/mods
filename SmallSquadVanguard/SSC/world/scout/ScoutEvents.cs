using System;
using System.Linq;
using SSC.misc;
using SSC.scout;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace SSC.world.scout
{
	// Token: 0x02000016 RID: 22
	internal class ScoutEvents
	{
		// Token: 0x0600011E RID: 286 RVA: 0x00005DDD File Offset: 0x00003FDD
		public ScoutEvents(ScoutMission scoutMission)
		{
			this.scoutMission = scoutMission;
		}

		// Token: 0x0600011F RID: 287 RVA: 0x00005DEC File Offset: 0x00003FEC
		internal void OnHourlyTick()
		{
			if (!this.scoutMission.IsOngoing)
			{
				return;
			}
			this.updateRenown();
			MobileParty mainParty = MobileParty.MainParty;
			Settlement targetSettlement = mainParty.TargetSettlement;
			if (this.scoutMission.Target != null)
			{
				this.addMilitiaToVillage(this.scoutMission.Target);
				this.scoutMission.Follower.Raid(this.scoutMission.Target);
				return;
			}
			if (mainParty.BesiegedSettlement != null)
			{
				this.scoutMission.Follower.Besiege(mainParty.BesiegedSettlement);
				return;
			}
			if (mainParty.CurrentSettlement != null)
			{
				if (mainParty.CurrentSettlement.IsUnderSiege)
				{
					this.scoutMission.Follower.Hold();
					this.scoutMission.Follower.DecideForYourself();
					return;
				}
				this.scoutMission.Follower.Visit(mainParty.CurrentSettlement);
				return;
			}
			else
			{
				if (Settlement.CurrentSettlement != null && Settlement.CurrentSettlement.IsVillage)
				{
					Settlement currentSettlement = Settlement.CurrentSettlement;
					this.scoutMission.Follower.RaidIfHostile(currentSettlement);
					return;
				}
				if (this.needsToEngageTarget(mainParty.TargetParty))
				{
					this.scoutMission.Follower.Engage(mainParty.TargetParty);
					return;
				}
				if (mainParty.TargetParty == this.scoutMission.MobileParty)
				{
					this.scoutMission.Follower.Hold();
					return;
				}
				if (this.needsToVisitTarget(mainParty.TargetSettlement))
				{
					this.scoutMission.Follower.Visit(mainParty.TargetSettlement);
					return;
				}
				if (this.needsToVisitVillage(mainParty.TargetSettlement))
				{
					Settlement targetSettlement2 = mainParty.TargetSettlement;
					this.scoutMission.Follower.Visit(targetSettlement2);
					return;
				}
				if (mainParty.CurrentSettlement == null && mainParty.BesiegedSettlement == null)
				{
					this.scoutMission.Follower.Follow();
				}
				return;
			}
		}

		// Token: 0x06000120 RID: 288 RVA: 0x00005FA0 File Offset: 0x000041A0
		private bool needsToVisitTarget(Settlement playerTarget)
		{
			return playerTarget != null && playerTarget.MapFaction != null && (playerTarget.IsTown || playerTarget.IsCastle) && !playerTarget.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction) && !this.scoutMission.Follower.isHostileTo(playerTarget) && !playerTarget.IsUnderSiege;
		}

		// Token: 0x06000121 RID: 289 RVA: 0x00006000 File Offset: 0x00004200
		private bool needsToVisitVillage(Settlement playerTarget)
		{
			return playerTarget != null && playerTarget.MapFaction != null && playerTarget.IsVillage && !playerTarget.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction) && !this.scoutMission.Follower.isHostileTo(playerTarget) && !playerTarget.IsRaided;
		}

		// Token: 0x06000122 RID: 290 RVA: 0x00006058 File Offset: 0x00004258
		private bool needsToEngageTarget(MobileParty party)
		{
			if (party != null && party.MapFaction != null)
			{
				MobileParty mainParty = MobileParty.MainParty;
				if (((mainParty != null) ? mainParty.MapFaction : null) != null && this.scoutMission.Follower.isHostileTo(party))
				{
					return MobileParty.MainParty.MapFaction.IsAtWarWith(party.MapFaction);
				}
			}
			return false;
		}

		// Token: 0x06000123 RID: 291 RVA: 0x000060AD File Offset: 0x000042AD
		private void addMilitiaToVillage(Settlement village)
		{
			if (village.IsVillage && village.Militia <= 1.5f)
			{
				village.Militia = 2f;
			}
		}

		// Token: 0x06000124 RID: 292 RVA: 0x000060CF File Offset: 0x000042CF
		private void updateRenown()
		{
			this.lastRenown = this.scoutMission.MobileParty.ActualClan.Renown;
		}

		// Token: 0x06000125 RID: 293 RVA: 0x000060EC File Offset: 0x000042EC
		internal void OnMapEventEnded(MapEvent mapEvent)
		{
			if (!this.scoutMission.IsOngoing)
			{
				return;
			}
			MobileParty follower = this.scoutMission.MobileParty;
			if (!mapEvent.InvolvedParties.Any((PartyBase party) => party == follower.Party))
			{
				return;
			}
			if (mapEvent.MapEventSettlement == this.scoutMission.Target)
			{
				this.scoutMission.Target = null;
			}
			Hero leader = this.scoutMission.Leader;
			PartyMisc.HasPartyLost(follower, mapEvent);
			if (PartyMisc.HasPartyWon(follower, mapEvent))
			{
				this.followerWonInBattle(this.lastRenown, follower, mapEvent);
			}
		}

		// Token: 0x06000126 RID: 294 RVA: 0x00006194 File Offset: 0x00004394
		private void followerWonInBattle(float lastRenown, MobileParty follower, MapEvent mapEvent)
		{
			if (lastRenown != 0f)
			{
				int renownGain = (int)Math.Ceiling((double)(follower.ActualClan.Renown - lastRenown));
				if (mapEvent.IsRaid)
				{
					ScoutRewards.followerGainsRenown(renownGain, ScoutRewards.Reason.Raid, this.scoutMission);
					Settlement mapEventSettlement = mapEvent.MapEventSettlement;
					if (mapEventSettlement != null && mapEventSettlement.IsVillage)
					{
						World.CastleParty.sendPunishmentPartyFromNearbyCastle(mapEventSettlement, follower);
						return;
					}
				}
				else
				{
					if (mapEvent.IsSiegeAssault)
					{
						ScoutRewards.followerGainsRenown(renownGain, ScoutRewards.Reason.SiegeAssault, this.scoutMission);
						return;
					}
					if (mapEvent.IsFieldBattle)
					{
						ScoutRewards.followerGainsRenown(renownGain, ScoutRewards.Reason.FieldBattle, this.scoutMission);
						return;
					}
					if (this.isWinAgainstBanditsOnly(mapEvent))
					{
						ScoutRewards.followerGainsRenown(renownGain, ScoutRewards.Reason.BanditsEncounter, this.scoutMission);
					}
				}
			}
		}

		// Token: 0x06000127 RID: 295 RVA: 0x00006238 File Offset: 0x00004438
		private bool isWinAgainstBanditsOnly(MapEvent mapEvent)
		{
			if (mapEvent == null || mapEvent.InvolvedParties == null)
			{
				return false;
			}
			foreach (PartyBase partyBase in mapEvent.InvolvedParties)
			{
				if (partyBase == null || partyBase.MobileParty == null)
				{
					return false;
				}
				if (!partyBase.MobileParty.IsBandit)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000128 RID: 296 RVA: 0x000062B0 File Offset: 0x000044B0
		internal void OnMakePeace(IFaction faction1, IFaction faction2, MakePeaceAction.MakePeaceDetail detail)
		{
			if (!this.scoutMission.IsOngoing)
			{
				return;
			}
			IFaction kingdom = this.scoutMission.MobileParty.ActualClan.Kingdom;
			if (faction1 != kingdom && faction2 != kingdom)
			{
				return;
			}
			foreach (Kingdom kingdom2 in Kingdom.All)
			{
				if (kingdom2 != kingdom && kingdom2.IsAtWarWith(kingdom))
				{
					break;
				}
			}
		}

		// Token: 0x06000129 RID: 297 RVA: 0x00006340 File Offset: 0x00004540
		internal void OnHeroPrisonerTaken(PartyBase partyBase, Hero hero)
		{
			if (!this.scoutMission.IsOngoing)
			{
				return;
			}
			if (hero == Hero.MainHero)
			{
				this.scoutMission.abandonMission(Abandonment.Reason.PlayerCaptured);
			}
			if (hero == this.scoutMission.Leader)
			{
				this.scoutMission.abandonMission(Abandonment.Reason.Captured);
			}
		}

		// Token: 0x0600012A RID: 298 RVA: 0x0000637E File Offset: 0x0000457E
		internal void OnHeroKilled(Hero victim, Hero hero2, KillCharacterAction.KillCharacterActionDetail detail, bool arg4)
		{
			if (!this.scoutMission.IsOngoing)
			{
				return;
			}
			if (victim == Hero.MainHero)
			{
				this.scoutMission.abandonMission(Abandonment.Reason.PlayerKilled);
			}
			if (victim == this.scoutMission.Leader)
			{
				this.scoutMission.abandonMission(Abandonment.Reason.PartyLeaderKilled);
			}
		}

		// Token: 0x0600012B RID: 299 RVA: 0x000063BC File Offset: 0x000045BC
		internal void sessionLaunched()
		{
			if (this.scoutMission.IsOngoing)
			{
				this.updateRenown();
			}
		}

		// Token: 0x0600012C RID: 300 RVA: 0x000063D4 File Offset: 0x000045D4
		internal void OnSettlementEntered(MobileParty party, Settlement settlement, Hero hero)
		{
			if (!this.scoutMission.IsOngoing)
			{
				return;
			}
			if (party == MobileParty.MainParty && (settlement.IsCastle || settlement.IsTown))
			{
				this.scoutMission.Follower.Visit(settlement);
			}
			if (settlement.IsTown && party == this.scoutMission.MobileParty && World.isFlagSet(Flags.ScoutMission_BuyHorsesMode))
			{
				this.townEnteredCheckForHorses(settlement, this.howManyHorsesAreNeeded(party));
			}
		}

		// Token: 0x0600012D RID: 301 RVA: 0x00006448 File Offset: 0x00004648
		internal void OnWarDeclared(IFaction faction1, IFaction faction2, DeclareWarAction.DeclareWarDetail detail)
		{
			if (!this.scoutMission.IsOngoing)
			{
				return;
			}
			MobileParty mobileParty = this.scoutMission.MobileParty;
			IFaction faction3 = (mobileParty != null) ? mobileParty.MapFaction : null;
			if (faction3 == null)
			{
				return;
			}
			if (faction1 != faction3 && faction2 != faction3)
			{
				return;
			}
			if (Hero.MainHero.MapFaction.IsAtWarWith(faction3))
			{
				this.scoutMission.abandonMission(Abandonment.Reason.FactionsAtWar);
			}
		}

		// Token: 0x0600012E RID: 302 RVA: 0x000064A6 File Offset: 0x000046A6
		internal void OnArmyDispersed(Army army, Army.ArmyDispersionReason reason, bool arg3)
		{
			if (!this.scoutMission.IsOngoing)
			{
				return;
			}
			if (army.LeaderParty == this.scoutMission.MobileParty)
			{
				this.scoutMission.abandonMission(Abandonment.Reason.ArmyDisbanded);
			}
		}

		// Token: 0x0600012F RID: 303 RVA: 0x000064D8 File Offset: 0x000046D8
		private int howManyHorsesAreNeeded(MobileParty party)
		{
			int totalManCount = party.MemberRoster.TotalManCount;
			int numberOfMounts = party.ItemRoster.NumberOfMounts;
			int num = (from troop in party.MemberRoster.GetTroopRoster()
			where troop.Character.IsMounted
			select troop).Sum((TroopRosterElement troop) => troop.Number);
			return totalManCount - num - numberOfMounts;
		}

		// Token: 0x06000130 RID: 304 RVA: 0x00006554 File Offset: 0x00004754
		private void townEnteredCheckForHorses(Settlement settlement, int maxHorsesNeeded)
		{
			MobileParty mobileParty = this.scoutMission.MobileParty;
			if (mobileParty != null && mobileParty.MapFaction != null && !mobileParty.MapFaction.IsAtWarWith(settlement.MapFaction) && mobileParty != MobileParty.MainParty && mobileParty.IsLordParty && mobileParty.LeaderHero != null && !mobileParty.IsDisbanding)
			{
				int num = MathF.Min(100000, mobileParty.LeaderHero.Gold);
				int numberOfMounts = mobileParty.Party.NumberOfMounts;
				if (numberOfMounts > mobileParty.Party.NumberOfRegularMembers)
				{
					return;
				}
				Town town = settlement.Town;
				int itemCountOfCategory = town.MarketData.GetItemCountOfCategory(DefaultItemCategories.Horse);
				if (itemCountOfCategory == 0)
				{
					itemCountOfCategory = town.MarketData.GetItemCountOfCategory(DefaultItemCategories.WarHorse);
				}
				if (itemCountOfCategory == 0)
				{
					return;
				}
				float num2 = 600f;
				float num3 = (float)(num / 2);
				if ((double)num3 > (double)(mobileParty.Party.NumberOfRegularMembers - numberOfMounts) * (double)num2)
				{
					num3 = (float)(mobileParty.Party.NumberOfRegularMembers - numberOfMounts) * num2;
				}
				if (mobileParty.Party.NumberOfRegularMembers - numberOfMounts > maxHorsesNeeded)
				{
					num3 = (float)maxHorsesNeeded * num2;
				}
				HorseBuying.BuyHorses(mobileParty, town, num3, 600, maxHorsesNeeded);
			}
		}

		// Token: 0x04000058 RID: 88
		private float lastRenown;

		// Token: 0x04000059 RID: 89
		private readonly ScoutMission scoutMission;
	}
}
