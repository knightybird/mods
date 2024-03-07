using System;

namespace SSC.misc.ExtensionMethods
{
	// Token: 0x02000072 RID: 114
	public static class MyExtensions
	{
		// Token: 0x0600035B RID: 859 RVA: 0x00012B93 File Offset: 0x00010D93
		public static int WordCount(this string str)
		{
			return str.Split(new char[]
			{
				' ',
				'.',
				'?'
			}, StringSplitOptions.RemoveEmptyEntries).Length;
		}
	}
}
