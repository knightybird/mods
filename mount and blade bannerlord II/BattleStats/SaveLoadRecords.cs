using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;

namespace BattleStats
{
	// Token: 0x02000007 RID: 7
	public static class SaveLoadRecords
	{
		// Token: 0x06000011 RID: 17 RVA: 0x000041C4 File Offset: 0x000023C4
		public static void SaveRecords()
		{
			TextObject name = Hero.MainHero.Name;
			string text = Path.Combine(SaveLoadRecords.folderPath, ((name != null) ? name.ToString() : null) + "_BattleStats.xml");
			SaveLoadRecords.file = text;
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(BattleStatsXml));
			XmlWriter xmlWriter;
			if (Directory.Exists(Path.Combine(new string[]
			{
				SaveLoadRecords.folderPath
			})))
			{
				Stream output = new FileStream(SaveLoadRecords.file, FileMode.Create);
				xmlWriter = XmlWriter.Create(output, new XmlWriterSettings
				{
					Indent = true,
					IndentChars = "\t",
					OmitXmlDeclaration = true
				});
			}
			else
			{
				Directory.CreateDirectory(Path.Combine(new string[]
				{
					SaveLoadRecords.folderPath
				}));
				Stream output2 = new FileStream(SaveLoadRecords.file, FileMode.Create);
				xmlWriter = XmlWriter.Create(output2, new XmlWriterSettings
				{
					Indent = true,
					IndentChars = "\t",
					OmitXmlDeclaration = true
				});
			}
			BattleStatsXml battleStatsXml = new BattleStatsXml
			{
				Clan = new List<HeroRecords>(),
				Army = new List<ArmyRecords>()
			};
			MenuSetup.SortRecordsByKills();
			foreach (HeroRecords item in MenuSetup.sortedHeroRecords)
			{
				battleStatsXml.Clan.Add(item);
			}
			foreach (ArmyRecords item2 in MenuSetup.sortedArmyRecords)
			{
				battleStatsXml.Army.Add(item2);
			}
			xmlSerializer.Serialize(xmlWriter, battleStatsXml);
			xmlWriter.Close();
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00004378 File Offset: 0x00002578
		private static void LoadRecords()
		{
			BattleStatsBehavior.heroRecords.Clear();
			BattleStatsBehavior.armyRecords.Clear();
			MenuSetup.sortedHeroRecords.Clear();
			MenuSetup.sortedArmyRecords.Clear();
			TextObject name = Hero.MainHero.Name;
			if (!Directory.Exists(Path.Combine(new string[]
			{
				SaveLoadRecords.folderPath
			})))
			{
				Directory.CreateDirectory(Path.Combine(new string[]
				{
					SaveLoadRecords.folderPath
				}));
			}
			if (!File.Exists(Path.Combine(SaveLoadRecords.folderPath, ((name != null) ? name.ToString() : null) + "_BattleStats.xml")))
			{
				return;
			}
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(BattleStatsXml));
			FileStream fileStream = new FileStream(Path.Combine(SaveLoadRecords.folderPath, ((name != null) ? name.ToString() : null) + "_BattleStats.xml"), FileMode.Open);
			BattleStatsXml battleStatsXml = (BattleStatsXml)xmlSerializer.Deserialize(fileStream);
			foreach (HeroRecords heroRecords in battleStatsXml.Clan)
			{
				if (!BattleStatsBehavior.heroRecords.ContainsKey(heroRecords.Name))
				{
					BattleStatsBehavior.heroRecords.Add(heroRecords.Name, heroRecords);
				}
			}
			foreach (ArmyRecords armyRecords in battleStatsXml.Army)
			{
				if (!BattleStatsBehavior.armyRecords.ContainsKey(armyRecords.Name))
				{
					BattleStatsBehavior.armyRecords.Add(armyRecords.Name, armyRecords);
				}
			}
			fileStream.Close();
			SaveLoadRecords.file = Path.Combine(SaveLoadRecords.folderPath, ((name != null) ? name.ToString() : null) + "_BattleStats.xml");
		}

		// Token: 0x06000013 RID: 19 RVA: 0x0000454C File Offset: 0x0000274C
		public static void LoadRecords(CampaignGameStarter cgs)
		{
			if (cgs == null)
			{
				throw new ArgumentNullException("cgs");
			}
			SaveLoadRecords.LoadRecords();
		}

		// Token: 0x04000019 RID: 25
		private static readonly string folderPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "SavedStats");

		// Token: 0x0400001A RID: 26
		public static string file;
	}
}
