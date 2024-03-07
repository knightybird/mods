using System;
using SSC.party.roles;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;

namespace SSC.party
{
	// Token: 0x0200004D RID: 77
	internal class PartyRoles
	{
		// Token: 0x0600025B RID: 603 RVA: 0x0000D5F4 File Offset: 0x0000B7F4
		public static void Report(Hero hero, bool skipConversation)
		{
			if (hero == null)
			{
				return;
			}
			if (hero == Hero.MainHero)
			{
				return;
			}
			PartyRoles.scout.Value.ScoutReturns(hero, skipConversation);
			PartyRoles.surgeon.Value.SurgeonReturns(hero, skipConversation);
			PartyRoles.quarterMaster.Value.QuartermasterReturns(hero, skipConversation);
			PartyRoles.engineer.Value.EngineerReturns(hero);
		}

		// Token: 0x0600025C RID: 604 RVA: 0x0000D651 File Offset: 0x0000B851
		private static void AssignRolesToTheBestHeroesCurrentlyInParty()
		{
			PartyRoles.scout.Value.assignRoleToTheBestHeroInParty();
			PartyRoles.surgeon.Value.assignRoleToTheBestHeroInParty();
			PartyRoles.quarterMaster.Value.assignRoleToTheBestHeroInParty();
			PartyRoles.engineer.Value.assignRoleToTheBestHeroInParty();
		}

		// Token: 0x0600025D RID: 605 RVA: 0x0000D690 File Offset: 0x0000B890
		public static void designateHeroesIfNeeded()
		{
			if (World.DesignatedScout.Value == null)
			{
				Hero effectiveScout = MobileParty.MainParty.EffectiveScout;
				if (effectiveScout != null && effectiveScout != Hero.MainHero)
				{
					World.DesignatedScout.Value = effectiveScout;
				}
			}
			if (World.DesignatedSurgeon.Value == null)
			{
				Hero effectiveSurgeon = MobileParty.MainParty.EffectiveSurgeon;
				if (effectiveSurgeon != null && effectiveSurgeon != Hero.MainHero)
				{
					World.DesignatedSurgeon.Value = effectiveSurgeon;
				}
			}
			if (World.DesignatedQuartermaster.Value == null)
			{
				Hero effectiveQuartermaster = MobileParty.MainParty.EffectiveQuartermaster;
				if (effectiveQuartermaster != null && effectiveQuartermaster != Hero.MainHero)
				{
					World.DesignatedQuartermaster.Value = effectiveQuartermaster;
				}
			}
			if (World.DesignatedEngineer.Value == null)
			{
				Hero effectiveEngineer = MobileParty.MainParty.EffectiveEngineer;
				if (effectiveEngineer != null && effectiveEngineer != Hero.MainHero)
				{
					World.DesignatedEngineer.Value = effectiveEngineer;
				}
			}
		}

		// Token: 0x040000C0 RID: 192
		private static Lazy<ScoutRole> scout = new Lazy<ScoutRole>(() => new ScoutRole());

		// Token: 0x040000C1 RID: 193
		private static Lazy<SurgeonRole> surgeon = new Lazy<SurgeonRole>(() => new SurgeonRole());

		// Token: 0x040000C2 RID: 194
		private static Lazy<QuartermasterRole> quarterMaster = new Lazy<QuartermasterRole>(() => new QuartermasterRole());

		// Token: 0x040000C3 RID: 195
		private static Lazy<EngineerRole> engineer = new Lazy<EngineerRole>(() => new EngineerRole());
	}
}
