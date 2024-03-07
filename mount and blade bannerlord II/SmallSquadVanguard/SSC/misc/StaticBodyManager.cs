using System;
using System.Reflection;
using System.Xml;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace SSC.misc
{
	// Token: 0x02000070 RID: 112
	public class StaticBodyManager
	{
		// Token: 0x06000355 RID: 853 RVA: 0x00012A68 File Offset: 0x00010C68
		public StaticBodyProperties fromKey(string key)
		{
			XmlElement xmlElement = new XmlDocument().CreateElement("elementName");
			xmlElement.SetAttribute("key", key);
			StaticBodyProperties result = default(StaticBodyProperties);
			StaticBodyProperties.FromXmlNode(xmlElement, out result);
			return result;
		}

		// Token: 0x06000356 RID: 854 RVA: 0x00012AA1 File Offset: 0x00010CA1
		public void setStaticBodyProperties(Hero hero, StaticBodyProperties sbp)
		{
			typeof(Hero).GetProperty("StaticBodyProperties", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(hero, sbp);
		}

		// Token: 0x06000357 RID: 855 RVA: 0x00012AC8 File Offset: 0x00010CC8
		public StaticBodyProperties getBlank()
		{
			return default(StaticBodyProperties);
		}

		// Token: 0x06000358 RID: 856 RVA: 0x00012AE0 File Offset: 0x00010CE0
		public ulong getKeyPart(StaticBodyProperties sbp, int index1to8)
		{
			switch (index1to8)
			{
			case 1:
				return sbp.KeyPart1;
			case 2:
				return sbp.KeyPart2;
			case 3:
				return sbp.KeyPart3;
			case 4:
				return sbp.KeyPart4;
			case 5:
				return sbp.KeyPart5;
			case 6:
				return sbp.KeyPart6;
			case 7:
				return sbp.KeyPart7;
			case 8:
				return sbp.KeyPart8;
			default:
				throw new Exception("Wrong key part index: " + index1to8.ToString());
			}
		}

		// Token: 0x040000F8 RID: 248
		public string caladog = "00054408800027875CA466A563442BD406899779A794573B86A7C473581AB487000546550E46C99D000000000000000000000000000000000000000000F03180";

		// Token: 0x040000F9 RID: 249
		public string derthert = "0000FC02C000220458C56743D5B5AB56396A8C717B876E66C7B287AB79C2A84B0000000305C96766000000000000000000000000000000000000000000F02000";
	}
}
