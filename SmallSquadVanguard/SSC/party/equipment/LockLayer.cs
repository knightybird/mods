using System;
using SandBox.GauntletUI;
using SSC.settings;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.Library;
using TaleWorlds.ScreenSystem;

namespace SSC.party.equipment
{
	// Token: 0x02000059 RID: 89
	public static class LockLayer
	{
		// Token: 0x060002A5 RID: 677 RVA: 0x0000EEAC File Offset: 0x0000D0AC
		public static void pushScreen(ScreenBase pushedScreen)
		{
			if (!LockLayer.lockButtonsNeeded())
			{
				return;
			}
			GauntletInventoryScreen gauntletInventoryScreen = pushedScreen as GauntletInventoryScreen;
			if (gauntletInventoryScreen != null)
			{
				LockLayer.lockTogglesVM = new LockTogglesVM(gauntletInventoryScreen);
				LockLayer.layer = new GauntletLayer(150, "GauntletLayer", false);
				LockLayer.movie = LockLayer.layer.LoadMovie("LockToggleOverlay", LockLayer.lockTogglesVM);
				LockLayer.layer.InputRestrictions.SetInputRestrictions(true, InputUsageMask.All);
				gauntletInventoryScreen.AddLayer(LockLayer.layer);
			}
		}

		// Token: 0x060002A6 RID: 678 RVA: 0x0000EF20 File Offset: 0x0000D120
		public static void popScreen(ScreenBase poppedScreen)
		{
			if (!LockLayer.lockButtonsNeeded())
			{
				return;
			}
			if (poppedScreen is GauntletInventoryScreen && LockLayer.layer != null)
			{
				LockLayer.layer.ReleaseMovie(LockLayer.movie);
				LockLayer.lockTogglesVM.Dispose();
				LockLayer.lockTogglesVM = null;
				LockLayer.layer = null;
				LockLayer.movie = null;
			}
		}

		// Token: 0x060002A7 RID: 679 RVA: 0x0000EF6F File Offset: 0x0000D16F
		private static bool lockButtonsNeeded()
		{
			return ModSettings.ArmorAndWeaponLockButtons && !ModSettings.LockButtonEffectivelyDisabled();
		}

		// Token: 0x040000C7 RID: 199
		private static GauntletLayer layer;

		// Token: 0x040000C8 RID: 200
		private static IGauntletMovie movie;

		// Token: 0x040000C9 RID: 201
		private static LockTogglesVM lockTogglesVM;
	}
}
