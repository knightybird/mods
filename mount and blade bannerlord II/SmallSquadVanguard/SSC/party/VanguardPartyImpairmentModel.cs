using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace SSC.party
{
	// Token: 0x02000049 RID: 73
	internal class VanguardPartyImpairmentModel : DefaultPartyImpairmentModel
	{
		// Token: 0x06000249 RID: 585 RVA: 0x0000C803 File Offset: 0x0000AA03
		public override float GetDisorganizedStateDuration(MobileParty party)
		{
			if (party == MobileParty.MainParty)
			{
				return VanguardPartyImpairmentModel.duration(party, VanguardPartyImpairmentModel.getBaseDurationForMainParty(party));
			}
			if (World.ScoutMission_LedParty != null && party == World.ScoutMission_LedParty)
			{
				VanguardPartyImpairmentModel.duration(party, VanguardPartyImpairmentModel.getBaseDurationForLedParty(party));
			}
			return base.GetDisorganizedStateDuration(party);
		}

		// Token: 0x0600024A RID: 586 RVA: 0x0000C840 File Offset: 0x0000AA40
		private static float duration(MobileParty party, float baseDuration)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(baseDuration, false, null);
			if (party.MapEvent != null && (party.MapEvent.IsRaid || party.MapEvent.IsSiegeAssault > false) && party.HasPerk(DefaultPerks.Tactics.SwiftRegroup, false))
			{
				explainedNumber.AddFactor(DefaultPerks.Tactics.SwiftRegroup.PrimaryBonus, DefaultPerks.Tactics.SwiftRegroup.Description);
			}
			if (party.HasPerk(DefaultPerks.Scouting.Foragers, false))
			{
				explainedNumber.AddFactor(DefaultPerks.Scouting.Foragers.SecondaryBonus, DefaultPerks.Scouting.Foragers.Description);
			}
			return explainedNumber.ResultNumber;
		}

		// Token: 0x0600024B RID: 587 RVA: 0x0000C8DC File Offset: 0x0000AADC
		private static float getBaseDurationForLedParty(MobileParty party)
		{
			int numberOfAllMembers = party.Party.NumberOfAllMembers;
			int num;
			if (numberOfAllMembers <= 26)
			{
				num = 1;
			}
			else if (numberOfAllMembers <= 36)
			{
				num = 2;
			}
			else if (numberOfAllMembers <= 46)
			{
				num = 3;
			}
			else if (numberOfAllMembers <= 56)
			{
				num = 4;
			}
			else if (numberOfAllMembers <= 66)
			{
				num = 5;
			}
			else
			{
				num = 6;
			}
			if (num > 1)
			{
				Hero effectiveScout = MobileParty.MainParty.EffectiveScout;
				if (effectiveScout != null && effectiveScout.GetSkillValue(DefaultSkills.Scouting) >= 100)
				{
					num--;
				}
			}
			return (float)num;
		}

		// Token: 0x0600024C RID: 588 RVA: 0x0000C94C File Offset: 0x0000AB4C
		private static float getBaseDurationForMainParty(MobileParty party)
		{
			int numberOfAllMembers = party.Party.NumberOfAllMembers;
			int num;
			if (numberOfAllMembers <= 32)
			{
				num = 1;
			}
			else if (numberOfAllMembers <= 40)
			{
				num = 2;
			}
			else if (numberOfAllMembers <= 50)
			{
				num = 3;
			}
			else if (numberOfAllMembers <= 60)
			{
				num = 4;
			}
			else if (numberOfAllMembers <= 70)
			{
				num = 5;
			}
			else
			{
				num = 6;
			}
			Hero effectiveQuartermaster = MobileParty.MainParty.EffectiveQuartermaster;
			if (effectiveQuartermaster != null && effectiveQuartermaster.GetSkillValue(DefaultSkills.Steward) >= 100)
			{
				num--;
			}
			return (float)num;
		}
	}
}
