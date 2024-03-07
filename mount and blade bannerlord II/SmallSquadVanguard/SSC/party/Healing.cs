using System;
using System.Collections.Generic;
using SSC.misc;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace SSC.party
{
	// Token: 0x0200004C RID: 76
	public class Healing
	{
		// Token: 0x06000255 RID: 597 RVA: 0x0000D22C File Offset: 0x0000B42C
		public void inSettlement(Settlement settlement)
		{
			if (settlement == null)
			{
				return;
			}
			if (MobileParty.MainParty.Party.IsStarving && !Hero.MainHero.IsPrisoner)
			{
				Message.display("The party is starving.");
				return;
			}
			if (settlement.MapFaction != null && settlement.MapFaction.IsAtWarWith(MobileParty.MainParty.MapFaction))
			{
				return;
			}
			this.healingWasDone = false;
			int num = 0;
			string str = null;
			if (settlement.IsVillage)
			{
				num += 2;
				str = settlement.Village.Name.ToString();
			}
			if (settlement.IsCastle)
			{
				num += 2;
				str = settlement.Name.ToString();
			}
			if (settlement.IsTown)
			{
				num += 3;
				str = settlement.Name.ToString();
			}
			int num2 = (int)CampaignTime.Now.ToHours;
			this.partyHeal(num);
			if (this.healingWasDone)
			{
				Message.display("The party is healing at " + str);
				return;
			}
			string str2 = string.Format("({0}% healed)", Healing.totalPercentHealed);
			if (settlement.IsUnderSiege || MobileParty.MainParty.MemberRoster.TotalHealthyCount < 20 || Healing.leastPercentageHitPoints < 50)
			{
				return;
			}
			if (num2 % 2 == 0)
			{
				if (Healing.leastPercentageHitPoints >= 50 && Healing.leastPercentageHitPoints <= 70)
				{
					Message.display("The party is bored " + str2 + ".");
					return;
				}
				if (Healing.leastPercentageHitPoints > 70)
				{
					Message.display("The party is raring to go " + str2 + ".");
				}
			}
		}

		// Token: 0x06000256 RID: 598 RVA: 0x0000D398 File Offset: 0x0000B598
		private void partyHeal(int bonus)
		{
			Hero effectiveSurgeon = MobileParty.MainParty.EffectiveSurgeon;
			Hero mainHero = Hero.MainHero;
			List<Hero> customCompanionsInPartyWithHitPointsAbove = this.getCustomCompanionsInPartyWithHitPointsAbove(0.6000000238418579);
			if (customCompanionsInPartyWithHitPointsAbove.Count <= 0)
			{
				Hero potentialMedic = null;
				Companion.forEachCustomCompanionInPartyDo(delegate(Hero companion)
				{
					if (potentialMedic == null || companion.HitPoints > potentialMedic.HitPoints)
					{
						potentialMedic = companion;
					}
				});
			}
			List<Hero> customCompanionsInPartyWithHitPointsBelow = Healing.getCustomCompanionsInPartyWithHitPointsBelow(0.6000000238418579);
			if ((float)mainHero.HitPoints > (float)mainHero.MaxHitPoints * 0.5f)
			{
				customCompanionsInPartyWithHitPointsAbove.Add(mainHero);
			}
			else
			{
				customCompanionsInPartyWithHitPointsBelow.Add(mainHero);
			}
			if (customCompanionsInPartyWithHitPointsBelow.Count <= 0)
			{
				return;
			}
			foreach (Hero medic in customCompanionsInPartyWithHitPointsAbove)
			{
				foreach (Hero patient in customCompanionsInPartyWithHitPointsBelow)
				{
					this.HealCompanion(medic, patient, effectiveSurgeon, bonus);
				}
			}
			Companion.updateAllCustomCompanions();
		}

		// Token: 0x06000257 RID: 599 RVA: 0x0000D4B0 File Offset: 0x0000B6B0
		private List<Hero> getCustomCompanionsInPartyWithHitPointsAbove(double percent)
		{
			List<Hero> list = new List<Hero>();
			Companion.forEachCustomCompanionInPartyDo(delegate(Hero companion)
			{
				if ((double)companion.HitPoints > (double)companion.MaxHitPoints * percent)
				{
					list.Add(companion);
				}
			});
			return list;
		}

		// Token: 0x06000258 RID: 600 RVA: 0x0000D4E0 File Offset: 0x0000B6E0
		private static List<Hero> getCustomCompanionsInPartyWithHitPointsBelow(double percent)
		{
			List<Hero> list = new List<Hero>();
			Healing.leastPercentageHitPoints = 100;
			int groupHitPoints = 0;
			int groupMaxHitPoints = 0;
			Companion.forEachCustomCompanionInPartyDo(delegate(Hero companion)
			{
				if ((double)companion.HitPoints < (double)companion.MaxHitPoints * percent)
				{
					list.Add(companion);
				}
				double num = (double)companion.HitPoints / (double)companion.MaxHitPoints * 100.0;
				if (num < (double)Healing.leastPercentageHitPoints)
				{
					Healing.leastPercentageHitPoints = (int)num;
				}
				groupHitPoints += companion.HitPoints;
				groupMaxHitPoints += companion.MaxHitPoints;
			});
			Healing.totalPercentHealed = (int)((double)groupHitPoints / (double)groupMaxHitPoints * 100.0);
			return list;
		}

		// Token: 0x06000259 RID: 601 RVA: 0x0000D550 File Offset: 0x0000B750
		private void HealCompanion(Hero medic, Hero patient, Hero surgeon, int bonus)
		{
			if (this.ints.Next(3) == 0)
			{
				int maxValue = 2 + bonus;
				int num = this.ints.Next(1, maxValue);
				patient.HitPoints += num;
				int num2 = this.xpForHealedHitPoint * num;
				if (medic.GetSkillValue(DefaultSkills.Medicine) < 15)
				{
					num2 *= 2;
				}
				medic.HeroDeveloper.AddSkillXp(DefaultSkills.Medicine, (float)num2, false, true);
				surgeon.HeroDeveloper.AddSkillXp(DefaultSkills.Medicine, 1f, false, false);
				this.healingWasDone = true;
			}
		}

		// Token: 0x040000BB RID: 187
		private Random ints = new Random();

		// Token: 0x040000BC RID: 188
		private int xpForHealedHitPoint = 6;

		// Token: 0x040000BD RID: 189
		private bool healingWasDone;

		// Token: 0x040000BE RID: 190
		private static int leastPercentageHitPoints;

		// Token: 0x040000BF RID: 191
		private static int totalPercentHealed;
	}
}
