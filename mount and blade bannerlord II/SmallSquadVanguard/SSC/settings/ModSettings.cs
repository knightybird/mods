using System;
using System.Text;

namespace SSC.settings
{
	// Token: 0x02000031 RID: 49
	public static class ModSettings
	{
		// Token: 0x17000031 RID: 49
		// (get) Token: 0x060001DA RID: 474 RVA: 0x0000A5AB File Offset: 0x000087AB
		public static bool AutoEquipmentDisabled
		{
			get
			{
				return !ModSettings.AutoEquipArmors && !ModSettings.AutoEquipWeapons && !ModSettings.AutoEquipAmmo && !ModSettings.AutoEquipHorse;
			}
		}

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x060001DB RID: 475 RVA: 0x0000A5CC File Offset: 0x000087CC
		public static bool AllAutoEquipmentIsEnabled
		{
			get
			{
				return ModSettings.AutoEquipArmors && ModSettings.AutoEquipWeapons && ModSettings.AutoEquipAmmo && ModSettings.AutoEquipHorse;
			}
		}

		// Token: 0x060001DC RID: 476 RVA: 0x0000A5EA File Offset: 0x000087EA
		public static bool LockButtonEffectivelyDisabled()
		{
			return !ModSettings.AutoEquipWeapons || !ModSettings.AutoEquipArmors || !ModSettings.AutoEquipAmmo;
		}

		// Token: 0x060001DD RID: 477 RVA: 0x0000A604 File Offset: 0x00008804
		public static bool printOut()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("LargeSquadMode: " + ModSettings.LargeSquadMode.ToString() + "\n");
			stringBuilder.Append("InitialCompanions: " + ModSettings.InitialCompanions.ToString() + "\n");
			stringBuilder.Append("AutoEquipWeapons: " + ModSettings.AutoEquipWeapons.ToString() + "\n");
			stringBuilder.Append("AutoEquipArmors: " + ModSettings.AutoEquipArmors.ToString() + "\n");
			stringBuilder.Append("AutoEquipAmmo: " + ModSettings.AutoEquipAmmo.ToString() + "\n");
			stringBuilder.Append("AutoEquipHorse: " + ModSettings.AutoEquipHorse.ToString() + "\n");
			stringBuilder.Append("ArmorAndWeaponLockButtons: " + ModSettings.ArmorAndWeaponLockButtons.ToString() + "\n");
			return true;
		}

		// Token: 0x04000094 RID: 148
		public static bool LargeSquadModeDefault = false;

		// Token: 0x04000095 RID: 149
		public static int InitialCompanionsDefault = World.SMALL_SQUAD_SIZE - 1;

		// Token: 0x04000096 RID: 150
		public static bool AutoEquipWeaponsDefault = true;

		// Token: 0x04000097 RID: 151
		public static bool AutoEquipArmorsDefault = true;

		// Token: 0x04000098 RID: 152
		public static bool AutoEquipAmmoDefault = true;

		// Token: 0x04000099 RID: 153
		public static bool AutoEquipHorseDefault = true;

		// Token: 0x0400009A RID: 154
		public static bool ArmorAndWeaponLockButtonsDefault = true;

		// Token: 0x0400009B RID: 155
		public static bool LargeSquadMode = ModSettings.LargeSquadModeDefault;

		// Token: 0x0400009C RID: 156
		public static int InitialCompanions = ModSettings.InitialCompanionsDefault;

		// Token: 0x0400009D RID: 157
		public static bool AutoEquipWeapons = ModSettings.AutoEquipWeaponsDefault;

		// Token: 0x0400009E RID: 158
		public static bool AutoEquipArmors = ModSettings.AutoEquipArmorsDefault;

		// Token: 0x0400009F RID: 159
		public static bool AutoEquipAmmo = ModSettings.AutoEquipAmmoDefault;

		// Token: 0x040000A0 RID: 160
		public static bool AutoEquipHorse = ModSettings.AutoEquipHorseDefault;

		// Token: 0x040000A1 RID: 161
		public static bool ArmorAndWeaponLockButtons = ModSettings.ArmorAndWeaponLockButtonsDefault;

		// Token: 0x040000A2 RID: 162
		public static bool TestMode2 = false;
	}
}
