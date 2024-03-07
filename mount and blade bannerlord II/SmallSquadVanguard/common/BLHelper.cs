using System;
using System.Reflection;

namespace common
{
	// Token: 0x02000077 RID: 119
	public class BLHelper
	{
		// Token: 0x06000385 RID: 901 RVA: 0x00013AC4 File Offset: 0x00011CC4
		public static string modVersion()
		{
			Version version = Assembly.GetExecutingAssembly().GetName().Version;
			return string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);
		}

		// Token: 0x06000386 RID: 902 RVA: 0x00013B0C File Offset: 0x00011D0C
		public static string formatVersionString(string str)
		{
			string text = str;
			if (text.StartsWith("v"))
			{
				text = text.Substring(1);
			}
			int num = text.LastIndexOf('.');
			if (num > 0)
			{
				text = text.Substring(0, num);
			}
			return text;
		}
	}
}
