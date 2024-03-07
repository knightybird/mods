using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.Core;

namespace SSC.party
{
	// Token: 0x02000050 RID: 80
	internal class CharacterStatsModel : DefaultCharacterStatsModel
	{
		// Token: 0x06000268 RID: 616 RVA: 0x0000DB50 File Offset: 0x0000BD50
		public override ExplainedNumber MaxHitpoints(CharacterObject character, bool includeDescriptions = false)
		{
			if (character.HeroObject == null)
			{
				return base.MaxHitpoints(character, includeDescriptions);
			}
			ExplainedNumber result = base.MaxHitpoints(character, includeDescriptions);
			if (Companion.isCustomCompanion(character.HeroObject))
			{
				int skillValue = character.HeroObject.GetSkillValue(DefaultSkills.OneHanded);
				int skillValue2 = character.HeroObject.GetSkillValue(DefaultSkills.TwoHanded);
				int skillValue3 = character.HeroObject.GetSkillValue(DefaultSkills.Polearm);
				int num = Math.Max(skillValue, Math.Max(skillValue2, skillValue3));
				int num2 = (int)Math.Round((double)((float)num * 0.4f));
				SkillObject skillObject = null;
				if (num == skillValue)
				{
					skillObject = DefaultSkills.OneHanded;
				}
				else if (num == skillValue2)
				{
					skillObject = DefaultSkills.TwoHanded;
				}
				else if (num == skillValue3)
				{
					skillObject = DefaultSkills.Polearm;
				}
				result.Add((float)num2, skillObject.Name, null);
			}
			return result;
		}
	}
}
