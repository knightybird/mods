using System;

namespace SSC
{
	// Token: 0x02000004 RID: 4
	public class FieldProxy<T>
	{
		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000016 RID: 22 RVA: 0x0000279F File Offset: 0x0000099F
		// (set) Token: 0x06000017 RID: 23 RVA: 0x000027AC File Offset: 0x000009AC
		public T Value
		{
			get
			{
				return this._get();
			}
			set
			{
				this._set(value);
			}
		}

		// Token: 0x06000018 RID: 24 RVA: 0x000027BA File Offset: 0x000009BA
		public FieldProxy(Func<T> getter, Action<T> setter)
		{
			this._set = setter;
			this._get = getter;
		}

		// Token: 0x06000019 RID: 25 RVA: 0x000027D0 File Offset: 0x000009D0
		public override string ToString()
		{
			T t = this._get();
			if (t == null)
			{
				return "null";
			}
			return t.ToString();
		}

		// Token: 0x04000006 RID: 6
		private Action<T> _set;

		// Token: 0x04000007 RID: 7
		private Func<T> _get;
	}
}
