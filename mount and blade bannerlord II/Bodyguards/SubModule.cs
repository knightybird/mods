using System;
using System.Collections.Generic;
using Bannerlord.UIExtenderEx;
using Bodyguards.UIEX;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace Bodyguards
{
	// Token: 0x02000008 RID: 8
	public class SubModule : MBSubModuleBase
	{
		// Token: 0x06000052 RID: 82 RVA: 0x000034D0 File Offset: 0x000016D0
		public override void OnMissionBehaviorInitialize(Mission mission)
		{
			bool flag = mission == null;
			if (!flag)
			{
				base.OnMissionBehaviorInitialize(mission);
				mission.AddMissionBehavior(new AddBodyguardsMissionBehavior());
			}
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00003500 File Offset: 0x00001700
		protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
		{
			bool flag = !(game.GameType is Campaign);
			if (!flag)
			{
				CampaignGameStarter campaignGameStarter = (CampaignGameStarter)gameStarterObject;
				campaignGameStarter.AddBehavior(new BodyguardsSaveDataBehavior());
				bool flag2 = !this._UIEXInitialized && AccessTools.TypeByName("UIExtender") != null;
				if (flag2)
				{
					UIExtender uiextender = new UIExtender("carbon.bodyguards");
					uiextender.Register(new List<Type>
					{
						typeof(EncyclopediaHeroPagePrefabExtension),
						typeof(EncyclopediaHeroPageVMMixin)
					});
					uiextender.Enable();
					this._UIEXInitialized = true;
				}
			}
		}

		// Token: 0x06000054 RID: 84 RVA: 0x000035AC File Offset: 0x000017AC
		public override void BeginGameStart(Game game)
		{
			bool flag = ((game != null) ? game.GameTextManager : null) != null;
			if (flag)
			{
				string name = typeof(BehaviorProtectVIPAgent).Name;
				List<GameTextManager.ChoiceTag> list = new List<GameTextManager.ChoiceTag>();
				list.Add(new GameTextManager.ChoiceTag(name, 1));
				GameText gameText = game.GameTextManager.GetGameText("str_formation_ai_behavior_text");
				gameText.AddVariationWithId(name, new TextObject("{=BGOAh7sOTJY}Bodyguards are protecting their General.", null), list);
			}
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00003618 File Offset: 0x00001818
		protected override void OnBeforeInitialModuleScreenSetAsRoot()
		{
			new Harmony("carbon.bodyguards").PatchAll();
		}

		// Token: 0x04000016 RID: 22
		private bool _UIEXInitialized = false;
	}
}
