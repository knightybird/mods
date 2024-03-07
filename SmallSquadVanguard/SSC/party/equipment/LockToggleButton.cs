using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace SSC.party.equipment
{
	// Token: 0x0200005A RID: 90
	internal class LockToggleButton : ButtonWidget
	{
		// Token: 0x060002A8 RID: 680 RVA: 0x0000EF84 File Offset: 0x0000D184
		public LockToggleButton(UIContext context) : base(context)
		{
			base.SuggestedWidth = 22f;
			base.SuggestedHeight = 22f;
			base.VerticalAlignment = VerticalAlignment.Top;
			base.WidthSizePolicy = SizePolicy.Fixed;
			base.HeightSizePolicy = SizePolicy.Fixed;
			base.Brush = context.BrushFactory.GetBrush("InventoryLockToggle");
		}
	}
}
