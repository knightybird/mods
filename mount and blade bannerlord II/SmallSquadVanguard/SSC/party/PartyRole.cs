using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;

namespace SSC.party
{
	// Token: 0x02000046 RID: 70
	public abstract class PartyRole
	{
		// Token: 0x06000239 RID: 569
		protected abstract int compare(Hero hero1, Hero hero2);

		// Token: 0x0600023A RID: 570
		protected abstract void setToRole(Hero hero);

		// Token: 0x0600023B RID: 571 RVA: 0x0000C39C File Offset: 0x0000A59C
		public void assignRoleToTheBestHeroInParty()
		{
			this.setToRole(this.bestCurrentlyInParty());
		}

		// Token: 0x0600023C RID: 572 RVA: 0x0000C3AC File Offset: 0x0000A5AC
		protected Hero bestCurrentlyInParty()
		{
			Hero hero = Hero.MainHero;
			foreach (TroopRosterElement troopRosterElement in MobileParty.MainParty.MemberRoster.GetTroopRoster())
			{
				if (troopRosterElement.Character.IsHero && troopRosterElement.Character.HeroObject != Hero.MainHero)
				{
					Hero heroObject = troopRosterElement.Character.HeroObject;
					if (this.compare(heroObject, hero) > 0)
					{
						hero = heroObject;
					}
				}
			}
			return hero;
		}

		// Token: 0x040000B7 RID: 183
		protected object dialogueBoundObject;

		// Token: 0x040000B8 RID: 184
		protected int notificationDuration = 3000;
	}
}
