using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace SSC.party
{
	// Token: 0x02000048 RID: 72
	internal class VanguardDelayedTeleportationModel : DefaultDelayedTeleportationModel
	{
		// Token: 0x06000247 RID: 583 RVA: 0x0000C734 File Offset: 0x0000A934
		public override ExplainedNumber GetTeleportationDelayAsHours(Hero teleportingHero, PartyBase target)
		{
			if (!Companion.isCustomCompanion(teleportingHero))
			{
				return base.GetTeleportationDelayAsHours(teleportingHero, target);
			}
			float num = 300f;
			IMapPoint mapPoint = teleportingHero.GetMapPoint();
			if (mapPoint != null)
			{
				if (target.IsSettlement)
				{
					if (teleportingHero.CurrentSettlement != null && teleportingHero.CurrentSettlement == target.Settlement)
					{
						num = 0f;
					}
					else
					{
						Campaign.Current.Models.MapDistanceModel.GetDistance(mapPoint, target.Settlement, 300f, out num);
					}
				}
				else if (target.IsMobile)
				{
					Campaign.Current.Models.MapDistanceModel.GetDistance(mapPoint, target.MobileParty, 300f, out num);
				}
			}
			double num2 = (double)MBRandom.RandomFloat + 0.1;
			return new ExplainedNumber(num * this.DefaultTeleportationSpeed * (float)num2, false, null);
		}
	}
}
