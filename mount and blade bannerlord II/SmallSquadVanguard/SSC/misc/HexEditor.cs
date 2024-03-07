using System;
using System.Text;
using TaleWorlds.Core;

namespace SSC.misc
{
	// Token: 0x0200006D RID: 109
	public class HexEditor
	{
		// Token: 0x06000343 RID: 835 RVA: 0x00012720 File Offset: 0x00010920
		public HexEditor(StaticBodyProperties sbp)
		{
			string value = this.strip(sbp.ToString());
			this.hex.Append(value);
		}

		// Token: 0x06000344 RID: 836 RVA: 0x00012760 File Offset: 0x00010960
		public StaticBodyProperties getResult()
		{
			string key = this.hex.ToString();
			return Manage.SBP.fromKey(key);
		}

		// Token: 0x06000345 RID: 837 RVA: 0x00012784 File Offset: 0x00010984
		public void randomSoldierHeight()
		{
			this.changeHexDigit(121, 15);
			this.changeHexDigit(122, 15);
		}

		// Token: 0x06000346 RID: 838 RVA: 0x0001279A File Offset: 0x0001099A
		public void removeBeards()
		{
			this.changeHexDigit(13, 0);
			this.changeHexDigit(14, 0);
		}

		// Token: 0x06000347 RID: 839 RVA: 0x000127B0 File Offset: 0x000109B0
		public void changeHexDigit(int hexIndex, int hexValue)
		{
			string text = hexValue.ToString("X");
			this.hex[hexIndex] = text[0];
		}

		// Token: 0x06000348 RID: 840 RVA: 0x000127E0 File Offset: 0x000109E0
		private string strip(string keyToStringValue)
		{
			int num = keyToStringValue.IndexOf("\"") + 1;
			int num2 = keyToStringValue.LastIndexOf("\"");
			return keyToStringValue.Substring(num, num2 - num);
		}

		// Token: 0x040000F6 RID: 246
		private StringBuilder hex = new StringBuilder();
	}
}
