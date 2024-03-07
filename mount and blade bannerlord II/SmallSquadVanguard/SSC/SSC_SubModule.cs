using System;
using System.Collections.Generic;
using common;
using SSC.party;
using SSC.party.companions;
using SSC.patch;
using SSC.settings;
using SSC.world;
using SSC.world.castle_party;
using SSC.world.escort;
using SSC.world.scout;
using StoryMode;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SSC
{
	// Token: 0x02000007 RID: 7
	public class SSC_SubModule : MBSubModuleBase
	{
		// Token: 0x06000023 RID: 35 RVA: 0x00002A07 File Offset: 0x00000C07
		protected override void OnSubModuleLoad()
		{
			base.OnSubModuleLoad();
			FileConfigSettings.readSettings();
			MCM_Manager.LoadMCMSettings();
			this.MCMInstalled = MCM_Manager.IsMCMActive();
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00002A24 File Offset: 0x00000C24
		protected override void InitializeGameStarter(Game game, IGameStarter starterObject)
		{
			if (starterObject.GetType() == typeof(CampaignGameStarter))
			{
				CampaignGameStarter gameStarter = (CampaignGameStarter)starterObject;
				this.addBehaviors(game, gameStarter);
				HarmonyLibrary.printHarmonyInfo();
				HarmonyLibrary.applyRegularPatches();
				HarmonyLibrary.patchInfo();
			}
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00002A68 File Offset: 0x00000C68
		protected override void OnBeforeInitialModuleScreenSetAsRoot()
		{
			base.OnBeforeInitialModuleScreenSetAsRoot();
			try
			{
				if (!this.MCMInstalled)
				{
					this.MCMInstalled = MCM_Manager.IsMCMActive();
				}
				string text = BLHelper.formatVersionString(ApplicationVersion.FromParametersFile(null).ToString());
				if (text != this.supportedGameVersion && text != this.previousVersionAlsoSupported)
				{
					InformationManager.DisplayMessage(new InformationMessage(string.Concat(new string[]
					{
						World.MOD_TITLE,
						" supports Bannerlord version ",
						this.supportedGameVersion,
						" but you are running ",
						text,
						"!"
					}), Colors.Red));
				}
				else
				{
					IEnumerable<char> source = this.MCMInstalled ? "" : "(MCMv5 not found, using default settings)";
					InformationManager.DisplayMessage(new InformationMessage(World.MOD_TITLE + " " + BLHelper.modVersion(), Colors.Green));
					source.IsEmpty<char>();
				}
			}
			catch (Exception ex)
			{
				InformationManager.DisplayMessage(new InformationMessage("Failed to load " + World.MOD_TITLE + "!", Colors.Red));
				InformationManager.DisplayMessage(new InformationMessage(string.Format("{0}: {1}", ex.GetType(), ex.Message), Colors.Yellow));
			}
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00002BA8 File Offset: 0x00000DA8
		public override void OnMissionBehaviorInitialize(Mission mission)
		{
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00002BAC File Offset: 0x00000DAC
		private void addBehaviors(Game game, CampaignGameStarter gameStarter)
		{
			if (game.GameType is CampaignStoryMode)
			{
				gameStarter.AddBehavior(new VanguardTrainingFieldCampaignBehavior());
			}
			gameStarter.AddModel(new BanditsModel());
			gameStarter.AddModel(new CharacterStatsModel());
			gameStarter.AddModel(new VanguardDelayedTeleportationModel());
			gameStarter.AddModel(new VanguardPartyImpairmentModel());
			gameStarter.AddBehavior(new ActionQueueBehavior());
			gameStarter.AddBehavior(new PartyEvents());
			gameStarter.AddBehavior(new EscortBehavior());
			gameStarter.AddBehavior(new ScoutBehavior());
			gameStarter.AddBehavior(new CastlePartyBehavior());
			gameStarter.AddBehavior(new HideoutGrowthBehavior());
			gameStarter.AddBehavior(new CompanionRolesCampaignBehaviorMod());
		}

		// Token: 0x04000010 RID: 16
		private bool MCMInstalled;

		// Token: 0x04000011 RID: 17
		private string supportedGameVersion = "1.2.9";

		// Token: 0x04000012 RID: 18
		private string previousVersionAlsoSupported = "1.2.8";
	}
}
