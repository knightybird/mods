using System;
using System.IO;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade;

namespace BattleStats
{
	// Token: 0x02000008 RID: 8
	public class SubModule : MBSubModuleBase
	{
		// Token: 0x06000015 RID: 21 RVA: 0x00004584 File Offset: 0x00002784
		protected override void OnSubModuleLoad()
		{
			SubModule.LoadConfig();
			Harmony harmony = new Harmony("onez.battlestats");
			harmony.PatchAll(Assembly.GetExecutingAssembly());
		}

		// Token: 0x06000016 RID: 22 RVA: 0x000045AC File Offset: 0x000027AC
		protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
		{
			if (game.GameType is Campaign)
			{
				CampaignEvents.OnBeforeSaveEvent.AddNonSerializedListener(this, new Action(SaveLoadRecords.SaveRecords));
				CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(SaveLoadRecords.LoadRecords));
			}
		}

		// Token: 0x06000017 RID: 23 RVA: 0x000045EC File Offset: 0x000027EC
		private static void LoadConfig()
		{
			StreamReader streamReader = new StreamReader(SubModule.ConfigFile);
			string text;
			while ((text = streamReader.ReadLine()) != null)
			{
				int num = text.IndexOf('=');
				if (num != -1)
				{
					string text2 = text.Substring(0, num - 1);
					string value = text.Substring(num + 2);
					InputKey inputKey;
					if (text2.ToLower().Equals("hotkey") && Enum.TryParse<InputKey>(value, true, out inputKey) && (Enum.IsDefined(typeof(InputKey), inputKey) | inputKey.ToString().Contains(",")))
					{
						MenuSetup.hotkey = inputKey;
					}
				}
				if (text.ToLower().Equals("changetextformat = true"))
				{
					MenuSetup.changeFormat = true;
				}
			}
			streamReader.Close();
		}

		// Token: 0x06000018 RID: 24 RVA: 0x000046AC File Offset: 0x000028AC
		protected override void OnApplicationTick(float dt)
		{
			base.OnApplicationTick(dt);
			bool flag = Campaign.Current != null;
			if (flag)
			{
				MenuSetup.ShowMenu();
			}
		}

		// Token: 0x0400001B RID: 27
		public static readonly string ConfigFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "config.txt");
	}
}
