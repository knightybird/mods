using System;

namespace SSC.misc
{
	// Token: 0x0200006B RID: 107
	public class BitFlags
	{
		// Token: 0x06000336 RID: 822 RVA: 0x00012558 File Offset: 0x00010758
		public BitFlags()
		{
			this._flags = 0L;
		}

		// Token: 0x06000337 RID: 823 RVA: 0x00012568 File Offset: 0x00010768
		public BitFlags(long flags)
		{
			this._flags = flags;
		}

		// Token: 0x06000338 RID: 824 RVA: 0x00012577 File Offset: 0x00010777
		public void setFlag(Flags flag)
		{
			this._flags |= (long)flag;
		}

		// Token: 0x06000339 RID: 825 RVA: 0x00012587 File Offset: 0x00010787
		public void unsetFlag(Flags flag)
		{
			this._flags &= (long)(~(long)flag);
		}

		// Token: 0x0600033A RID: 826 RVA: 0x00012598 File Offset: 0x00010798
		public bool isFlagSet(Flags flag)
		{
			return (this._flags & (long)flag) != 0L;
		}

		// Token: 0x0600033B RID: 827 RVA: 0x000125A6 File Offset: 0x000107A6
		public long getFlags()
		{
			return this._flags;
		}

		// Token: 0x0600033C RID: 828 RVA: 0x000125AE File Offset: 0x000107AE
		public void overwriteFlags(long flags)
		{
			this._flags = flags;
		}

		// Token: 0x0600033D RID: 829 RVA: 0x000125B7 File Offset: 0x000107B7
		public void clearFlags()
		{
			this._flags = 0L;
		}

		// Token: 0x040000F5 RID: 245
		private long _flags;
	}
}
