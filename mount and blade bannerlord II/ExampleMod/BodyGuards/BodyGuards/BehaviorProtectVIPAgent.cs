using System;
using TaleWorlds.MountAndBlade;

namespace Bodyguards
{
	// Token: 0x02000005 RID: 5
	internal class BehaviorProtectVIPAgent : BehaviorComponent
	{
		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000043 RID: 67 RVA: 0x00003314 File Offset: 0x00001514
		public override float NavmeshlessTargetPositionPenalty
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x06000044 RID: 68 RVA: 0x0000331B File Offset: 0x0000151B
		public BehaviorProtectVIPAgent(Formation formation) : base(formation)
		{
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00003330 File Offset: 0x00001530
		public bool IsAlive()
		{
			return this.VIP != null && this.VIP.IsActive();
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00003358 File Offset: 0x00001558
		public override void TickOccasionally()
		{
			bool flag = this.IsAlive();
			if (flag)
			{
				base.Formation.SetMovementOrder(MovementOrder.MovementOrderFollow(this.VIP));
				base.CurrentOrder = MovementOrder.MovementOrderFollow(this.VIP);
			}
		}

		// Token: 0x06000047 RID: 71 RVA: 0x0000339C File Offset: 0x0000159C
		protected override float GetAiWeight()
		{
			bool flag = this.IsAlive();
			float result;
			if (flag)
			{
				result = (this.DisableThisBehaviorManually ? 0f : 100f);
			}
			else
			{
				result = 0f;
			}
			return result;
		}

		// Token: 0x06000048 RID: 72 RVA: 0x000033D5 File Offset: 0x000015D5
		public override void ResetBehavior()
		{
			this.DisableThisBehaviorManually = false;
		}

		// Token: 0x04000013 RID: 19
		public bool DisableThisBehaviorManually = false;

		// Token: 0x04000014 RID: 20
		public Agent VIP;
	}
}
