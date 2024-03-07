using System;
using System.Collections.Generic;
using System.IO;
using SSC.settings;

namespace SSC
{
	// Token: 0x02000005 RID: 5
	internal static class FileConfigSettings
	{
		// Token: 0x0600001A RID: 26 RVA: 0x00002804 File Offset: 0x00000A04
		public static void readSettings()
		{
			Dictionary<string, bool> dictionary = new Dictionary<string, bool>
			{
				{
					FileConfigSettings.AUTO_EQUIP_ARMORS,
					true
				},
				{
					FileConfigSettings.AUTO_EQUIP_WEAPONS,
					true
				},
				{
					FileConfigSettings.AUTO_EQUIP_AMMO,
					true
				},
				{
					FileConfigSettings.AUTO_EQUIP_MOUNT,
					true
				}
			};
			try
			{
				string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), string.Format("Mount and Blade II Bannerlord\\Configs\\{0}", "SmallSquadVanguard"));
				if (Directory.Exists(text))
				{
					string path = "config.txt";
					if (File.Exists(Path.Combine(text, path)))
					{
						string[] array = File.ReadAllLines(Path.Combine(text, path));
						for (int i = 0; i < array.Length; i++)
						{
							Tuple<string, bool> tuple = FileConfigSettings.parse(array[i]);
							if (tuple != null)
							{
								dictionary[tuple.Item1] = tuple.Item2;
							}
						}
						FileConfigSettings.processSettings(dictionary);
					}
				}
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x0600001B RID: 27 RVA: 0x000028E0 File Offset: 0x00000AE0
		private static Tuple<string, bool> parse(string s)
		{
			string[] array = s.Split(new char[]
			{
				'='
			});
			if (array.Length != 2)
			{
				return null;
			}
			return new Tuple<string, bool>(array[0].Trim(), bool.Parse(array[1].Trim()));
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00002924 File Offset: 0x00000B24
		private static void processSettings(Dictionary<string, bool> map)
		{
			if (FileConfigSettings.isFalse(FileConfigSettings.AUTO_EQUIP_ARMORS, map))
			{
				ModSettings.AutoEquipArmorsDefault = false;
			}
			if (FileConfigSettings.isFalse(FileConfigSettings.AUTO_EQUIP_WEAPONS, map))
			{
				ModSettings.AutoEquipWeaponsDefault = false;
			}
			if (FileConfigSettings.isFalse(FileConfigSettings.AUTO_EQUIP_AMMO, map))
			{
				ModSettings.AutoEquipAmmoDefault = false;
			}
			if (FileConfigSettings.isFalse(FileConfigSettings.AUTO_EQUIP_MOUNT, map))
			{
				ModSettings.AutoEquipHorseDefault = false;
			}
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002980 File Offset: 0x00000B80
		private static bool isFalse(string setting, Dictionary<string, bool> map)
		{
			bool flag;
			return map.TryGetValue(setting, out flag) && !flag;
		}

		// Token: 0x04000008 RID: 8
		private static string AUTO_EQUIP_ARMORS = "auto_equip_armors";

		// Token: 0x04000009 RID: 9
		private static string AUTO_EQUIP_WEAPONS = "auto_equip_weapons";

		// Token: 0x0400000A RID: 10
		private static string AUTO_EQUIP_AMMO = "auto_equip_ammo";

		// Token: 0x0400000B RID: 11
		private static string AUTO_EQUIP_MOUNT = "auto_equip_mount";
	}
}
