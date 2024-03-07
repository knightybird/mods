using System;
using System.IO;
using System.Reflection;

namespace SSC.settings
{
	// Token: 0x0200002F RID: 47
	public static class MCM_Manager
	{
		// Token: 0x060001D6 RID: 470 RVA: 0x0000A480 File Offset: 0x00008680
		public static bool IsMCMInstalled_NotWorking()
		{
			try
			{
				Assembly.Load("MCMv5");
			}
			catch (FileNotFoundException)
			{
				return false;
			}
			catch (Exception)
			{
				return false;
			}
			return true;
		}

		// Token: 0x060001D7 RID: 471 RVA: 0x0000A4C4 File Offset: 0x000086C4
		public static bool IsMCMActive()
		{
			bool result;
			try
			{
				MCM_Manager.LoadMCMSettings();
				foreach (Type type in MCM_Manager.mcmSettings.GetTypes())
				{
				}
				Type type2 = MCM_Manager.mcmSettings.GetType("SSC.settings.GlobalPoint");
				if (type2 == null)
				{
					throw new Exception("Type SSC.settings.GlobalPoint not found in assembly");
				}
				MethodInfo method = type2.GetMethod("IsMCMActive");
				if (method == null)
				{
					throw new Exception("Method IsMCMActive not found in type SSC.settings.GlobalPoint");
				}
				result = (bool)method.Invoke(null, null);
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		// Token: 0x060001D8 RID: 472 RVA: 0x0000A55C File Offset: 0x0000875C
		public static void LoadMCMSettings()
		{
			if (MCM_Manager.mcmSettings != null)
			{
				return;
			}
			MCM_Manager.mcmSettings = Assembly.LoadFrom(Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath), "MCMSettings.dll"));
		}

		// Token: 0x04000093 RID: 147
		private static Assembly mcmSettings;
	}
}
