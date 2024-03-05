using System;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.ViewModels;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Pages;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Bodyguards.UIEX
{
	// Token: 0x02000009 RID: 9
	[ViewModelMixin("RefreshValues")]
	internal sealed class EncyclopediaHeroPageVMMixin : BaseViewModelMixin<EncyclopediaHeroPageVM>
	{
		// Token: 0x06000057 RID: 87 RVA: 0x0000363C File Offset: 0x0000183C
		public EncyclopediaHeroPageVMMixin(EncyclopediaHeroPageVM vm) : base(vm)
		{
			this._hero = (vm.Obj as Hero);
			this._vm = vm;
			this._vm.RefreshValues();
			this.OnRefresh();
        }

        // Token: 0x06000058 RID: 88 RVA: 0x000036A9 File Offset: 0x000018A9
        public override void OnRefresh()
		{
			base.OnPropertyChanged("CanBeBodyguard");
			base.OnPropertyChanged("ToggleBodyguardActionName");
			base.OnPropertyChanged("ToggleBodyguardHint");
            base.OnPropertyChanged("ToggleBodyguardActionName2");
            base.OnPropertyChanged("ToggleBodyguardHint2");
        }

        // Token: 0x06000059 RID: 89 RVA: 0x000036D0 File Offset: 0x000018D0
        [DataSourceMethod]
		public void ToggleBodyguard()
		{
			bool flag = this._behavior == null;
			if (!flag)
			{
				bool flag2 = this._behavior.IsBodyguard(this._hero);
				if (flag2)
				{
					this._behavior.RemoveBodyguard(this._hero);
				}
				else
				{
					this._behavior.AddBodyguard(this._hero);
				}
				this.OnRefresh();
			}
		}

        [DataSourceMethod]
        public void ToggleBodyguard2()
        {
            bool flag = this._behavior == null;
            if (!flag)
            {
                bool flag2 = this._behavior.IsBodyguard2(this._hero);
                if (flag2)
                {
                    this._behavior.RemoveBodyguard2(this._hero);
                }
                else
                {
                    this._behavior.AddBodyguard2(this._hero);
                }
                this.OnRefresh();
            }
        }

        // Token: 0x17000021 RID: 33
        // (get) Token: 0x0600005A RID: 90 RVA: 0x00003734 File Offset: 0x00001934
        [DataSourceProperty]
		public bool CanBeBodyguard
		{
			get
			{
				return this._settings.companionGuardMode && !this._hero.Equals(Hero.MainHero) && Clan.PlayerClan.Heroes.Contains(this._hero) && this._hero.IsAlive && !this._hero.IsChild;
			}
		}

        // Token: 0x17000022 RID: 34
        // (get) Token: 0x0600005B RID: 91 RVA: 0x00003795 File Offset: 0x00001995
        [DataSourceProperty]
		public string ToggleBodyguardActionName
		{
			get
			{
				return this._behavior.IsBodyguard(this._hero) ? new TextObject("{=BGQq8pl36of}Remove from Bodyguards", null).ToString() : new TextObject("{=BG3HYFbmIzB}Add to Bodyguards", null).ToString();
			}
		}

        [DataSourceProperty]
        public string ToggleBodyguardActionName2
        {
            get
            {
                return this._behavior.IsBodyguard2(this._hero) ? new TextObject("Remove from Bodyguards2", null).ToString() : new TextObject("Add to Bodyguards2", null).ToString();
            }
        }

        // Token: 0x17000023 RID: 35
        // (get) Token: 0x0600005C RID: 92 RVA: 0x000037CC File Offset: 0x000019CC
        [DataSourceProperty]
		public HintViewModel ToggleBodyguardHint
		{
			get
			{
				return new HintViewModel(this._hintText.SetTextVariable("ADDING", this._behavior.IsBodyguard(this._hero) ? 0 : 1).SetTextVariable("HERONAME", this._hero.Name.ToString()), null);
			}
		}

        [DataSourceProperty]
        public HintViewModel ToggleBodyguardHint2
        {
            get
            {
                return new HintViewModel(this._hintText.SetTextVariable("ADDING", this._behavior.IsBodyguard2(this._hero) ? 0 : 1).SetTextVariable("HERONAME", this._hero.Name.ToString()), null);
            }
        }

        // Token: 0x04000017 RID: 23
        private EncyclopediaHeroPageVM _vm;

		// Token: 0x04000018 RID: 24
		private readonly Hero _hero;

		// Token: 0x04000019 RID: 25
		private BodyguardsSettings _settings = new BodyguardsSettings();

		// Token: 0x0400001A RID: 26
		private readonly BodyguardsSaveDataBehavior _behavior = Campaign.Current.GetCampaignBehavior<BodyguardsSaveDataBehavior>();

		// Token: 0x0400001B RID: 27
		private readonly TextObject _hintText = new TextObject("{=BGA8o3aiJKF}{ADDING?}Add{?}Remove{\\?} {HERONAME} {?ADDING}to{?}from{\\?} your bodyguard detail", null);
	}
}
