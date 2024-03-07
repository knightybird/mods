using System;
using System.Linq;
using SandBox.GauntletUI;
using SSC.party.companions;
using SSC.settings;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.Inventory;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.Library;

namespace SSC.party.equipment
{
	// Token: 0x0200005B RID: 91
	public class LockTogglesVM : ViewModel, IDisposable
	{
		// Token: 0x060002A9 RID: 681 RVA: 0x0000EFD9 File Offset: 0x0000D1D9
		public LockTogglesVM(GauntletInventoryScreen inventoryScreen)
		{
			this.inventoryScreen = inventoryScreen;
			this.inventoryViewModel = this.GetInventoryVM();
			this.inventoryViewModel.CharacterList.PropertyChangedWithValue += this.SelectedCharacterChanged;
			this.RefreshValues();
		}

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x060002AA RID: 682 RVA: 0x0000F016 File Offset: 0x0000D216
		private SelectorVM<InventoryCharacterSelectorItemVM> CharacterList
		{
			get
			{
				return this.inventoryViewModel.CharacterList;
			}
		}

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x060002AB RID: 683 RVA: 0x0000F023 File Offset: 0x0000D223
		private Hero CurrentHero
		{
			get
			{
				return this.CharacterList.SelectedItem.Hero;
			}
		}

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x060002AC RID: 684 RVA: 0x0000F035 File Offset: 0x0000D235
		[DataSourceProperty]
		public bool ArmorLock
		{
			get
			{
				return World.heroData.isFlagSet(this.CurrentHero, HeroFlags.ArmorLocked);
			}
		}

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x060002AD RID: 685 RVA: 0x0000F048 File Offset: 0x0000D248
		[DataSourceProperty]
		public bool ArmorsButtonVisible
		{
			get
			{
				return ModSettings.AutoEquipArmors && this.CurrentHero != Hero.MainHero;
			}
		}

		// Token: 0x060002AE RID: 686 RVA: 0x0000F063 File Offset: 0x0000D263
		public void ToggleArmorLock()
		{
			World.heroData.toggleFlag(this.CurrentHero, HeroFlags.ArmorLocked);
			base.OnPropertyChanged("ArmorLock");
		}

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x060002AF RID: 687 RVA: 0x0000F081 File Offset: 0x0000D281
		[DataSourceProperty]
		public bool WeaponLock
		{
			get
			{
				return World.heroData.isFlagSet(this.CurrentHero, HeroFlags.WeaponsLocked);
			}
		}

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x060002B0 RID: 688 RVA: 0x0000F094 File Offset: 0x0000D294
		[DataSourceProperty]
		public bool WeaponsButtonVisible
		{
			get
			{
				return ModSettings.AutoEquipWeapons && this.CurrentHero != Hero.MainHero;
			}
		}

		// Token: 0x060002B1 RID: 689 RVA: 0x0000F0AF File Offset: 0x0000D2AF
		public void ToggleWeaponLock()
		{
			World.heroData.toggleFlag(this.CurrentHero, HeroFlags.WeaponsLocked);
			base.OnPropertyChanged("WeaponLock");
		}

		// Token: 0x060002B2 RID: 690 RVA: 0x0000F0CD File Offset: 0x0000D2CD
		public override void RefreshValues()
		{
			base.RefreshValues();
			base.OnPropertyChanged("ArmorLock");
			base.OnPropertyChanged("WeaponLock");
			base.OnPropertyChanged("ArmorsButtonVisible");
			base.OnPropertyChanged("WeaponsButtonVisible");
		}

		// Token: 0x060002B3 RID: 691 RVA: 0x0000F101 File Offset: 0x0000D301
		private void SelectedCharacterChanged(object sender, PropertyChangedWithValueEventArgs e)
		{
			this.RefreshValues();
		}

		// Token: 0x060002B4 RID: 692 RVA: 0x0000F10C File Offset: 0x0000D30C
		private SPInventoryVM GetInventoryVM()
		{
			foreach (Tuple<IGauntletMovie, ViewModel> tuple in this.inventoryScreen.Layers.OfType<GauntletLayer>().SelectMany((GauntletLayer x) => x.MoviesAndDataSources))
			{
				SPInventoryVM spinventoryVM = tuple.Item2 as SPInventoryVM;
				if (spinventoryVM != null)
				{
					return spinventoryVM;
				}
			}
			return null;
		}

		// Token: 0x060002B5 RID: 693 RVA: 0x0000F194 File Offset: 0x0000D394
		public void Dispose()
		{
			this.inventoryViewModel.CharacterList.PropertyChangedWithValue -= this.SelectedCharacterChanged;
		}

		// Token: 0x040000CA RID: 202
		private readonly GauntletInventoryScreen inventoryScreen;

		// Token: 0x040000CB RID: 203
		private readonly SPInventoryVM inventoryViewModel;
	}
}
