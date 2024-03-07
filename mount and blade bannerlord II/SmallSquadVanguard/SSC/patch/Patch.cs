using System;
using HarmonyLib;
using SandBox.CampaignBehaviors;
using SSC.party;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace SSC.patch
{
	// Token: 0x0200003D RID: 61
	[HarmonyPatch(typeof(DefaultNotificationsCampaignBehavior), "OnHeroGainedSkill")]
	internal class Patch
	{
		// Token: 0x06000207 RID: 519 RVA: 0x0000B5DC File Offset: 0x000097DC
		public static bool Prefix(Hero hero, SkillObject skill, ref bool shouldNotify)
		{
			if (Companion.isCustomCompanion(hero))
			{
				if (skill == DefaultSkills.Medicine)
				{
					shouldNotify = false;
				}
				else
				{
					int skillValue = hero.GetSkillValue(skill);
					shouldNotify = (skillValue % 2 == 0);
				}
			}
			return true;
		}
	}
}
