using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace SSC.party.roles
{
	// Token: 0x02000054 RID: 84
	public class EngineerRole : PartyRole
	{
		// Token: 0x06000281 RID: 641 RVA: 0x0000E3B0 File Offset: 0x0000C5B0
		public void EngineerReturns(Hero hero)
		{
			Hero hero2 = EngineerRole.bestEngineerInParty();
			if (hero == hero2)
			{
				return;
			}
			if (hero.GetSkillValue(DefaultSkills.Engineering) > hero2.GetSkillValue(DefaultSkills.Engineering))
			{
				MobileParty.MainParty.SetPartyEngineer(hero);
				MBInformationManager.AddQuickInformation(new TextObject(string.Format("{0} is now the party's engineer.", hero), null), this.notificationDuration, hero.CharacterObject, "");
			}
		}

		// Token: 0x06000282 RID: 642 RVA: 0x0000E414 File Offset: 0x0000C614
		private static Hero bestEngineerInParty()
		{
			Hero hero = Hero.MainHero;
			foreach (TroopRosterElement troopRosterElement in MobileParty.MainParty.MemberRoster.GetTroopRoster())
			{
				if (troopRosterElement.Character.IsHero && troopRosterElement.Character.HeroObject != Hero.MainHero)
				{
					Hero heroObject = troopRosterElement.Character.HeroObject;
					if (heroObject.GetSkillValue(DefaultSkills.Engineering) > hero.GetSkillValue(DefaultSkills.Engineering))
					{
						hero = heroObject;
					}
				}
			}
			return hero;
		}

		// Token: 0x06000283 RID: 643 RVA: 0x0000E4B8 File Offset: 0x0000C6B8
		protected override int compare(Hero hero1, Hero hero2)
		{
			return hero1.GetSkillValue(DefaultSkills.Engineering) - hero2.GetSkillValue(DefaultSkills.Engineering);
		}

		// Token: 0x06000284 RID: 644 RVA: 0x0000E4D1 File Offset: 0x0000C6D1
		protected override void setToRole(Hero hero)
		{
			MobileParty.MainParty.SetPartyEngineer(hero);
		}
	}
}
