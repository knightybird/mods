using System;
using System.Collections.Generic;
using TaleWorlds.SaveSystem;

namespace SSC.party
{
	// Token: 0x02000041 RID: 65
	internal class ActionQueue
	{
		// Token: 0x06000227 RID: 551 RVA: 0x0000BED8 File Offset: 0x0000A0D8
		public void add(QueuedAction action)
		{
			if (!action.CheckOnNextTick() && action.Condition())
			{
				action.ActionToPerform();
				return;
			}
			if (!action.Obsolete())
			{
				this.actions.Add(action);
			}
		}

		// Token: 0x06000228 RID: 552 RVA: 0x0000BF24 File Offset: 0x0000A124
		public void execute()
		{
			for (int i = 0; i < this.actions.Count; i++)
			{
				if (this.actions[i].Condition())
				{
					this.actions[i].ActionToPerform();
					this.actions.RemoveAt(i);
					i--;
				}
				else if (this.actions[i].Obsolete())
				{
					this.actions.RemoveAt(i);
					i--;
				}
			}
		}

		// Token: 0x06000229 RID: 553 RVA: 0x0000BFAF File Offset: 0x0000A1AF
		public bool hasActions()
		{
			return this.actions.Count > 0;
		}

		// Token: 0x0600022A RID: 554 RVA: 0x0000BFBF File Offset: 0x0000A1BF
		public void clear()
		{
			this.actions.Clear();
		}

		// Token: 0x0600022B RID: 555 RVA: 0x0000BFCC File Offset: 0x0000A1CC
		public int count()
		{
			return this.actions.Count;
		}

		// Token: 0x040000B2 RID: 178
		[SaveableField(0)]
		private List<QueuedAction> actions = new List<QueuedAction>();
	}
}
