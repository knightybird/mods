using System;
using TaleWorlds.Core;

namespace SSC.misc
{
	// Token: 0x0200006F RID: 111
	public class SBPEditor
	{
		// Token: 0x0600034F RID: 847 RVA: 0x000128E0 File Offset: 0x00010AE0
		public SBPEditor(StaticBodyProperties sbp)
		{
			this.bytes = this.getByteArray(sbp);
		}

		// Token: 0x06000350 RID: 848 RVA: 0x000128F8 File Offset: 0x00010AF8
		public StaticBodyProperties getResult()
		{
			ulong[] array = this.backToUlongs(this.bytes);
			return new StaticBodyProperties(array[0], array[1], array[2], array[3], array[4], array[5], array[6], array[7]);
		}

		// Token: 0x06000351 RID: 849 RVA: 0x0001292F File Offset: 0x00010B2F
		public void changeHexDigit(int hexIndex, int hexValue)
		{
			this.changeHexDigit(this.bytes, hexIndex, hexValue);
		}

		// Token: 0x06000352 RID: 850 RVA: 0x00012940 File Offset: 0x00010B40
		private void changeHexDigit(byte[] bytes, int hexIndex, int hexValue)
		{
			int num = hexIndex / 2;
			int num2 = hexIndex % 2;
			byte b = (byte)(15 << num2 * 4);
			int num3 = num;
			bytes[num3] &= ~b;
			int num4 = num;
			bytes[num4] |= (byte)(hexValue << num2 * 4);
		}

		// Token: 0x06000353 RID: 851 RVA: 0x00012984 File Offset: 0x00010B84
		private byte[] getByteArray(StaticBodyProperties sbp)
		{
			ulong[] array = new ulong[]
			{
				Manage.SBP.getKeyPart(sbp, 1),
				Manage.SBP.getKeyPart(sbp, 2),
				Manage.SBP.getKeyPart(sbp, 3),
				Manage.SBP.getKeyPart(sbp, 4),
				Manage.SBP.getKeyPart(sbp, 5),
				Manage.SBP.getKeyPart(sbp, 6),
				Manage.SBP.getKeyPart(sbp, 7),
				Manage.SBP.getKeyPart(sbp, 8)
			};
			byte[] array2 = new byte[64];
			for (int i = 0; i < 8; i++)
			{
				Array.Copy(BitConverter.GetBytes(array[i]), 0, array2, i * 8, 8);
			}
			return array2;
		}

		// Token: 0x06000354 RID: 852 RVA: 0x00012A38 File Offset: 0x00010C38
		private ulong[] backToUlongs(byte[] bytes)
		{
			ulong[] array = new ulong[8];
			for (int i = 0; i < 8; i++)
			{
				array[i] = BitConverter.ToUInt64(bytes, i * 8);
			}
			return array;
		}

		// Token: 0x040000F7 RID: 247
		private byte[] bytes;
	}
}
