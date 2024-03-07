using System;
using System.Diagnostics;
using TaleWorlds.Library;

namespace SSC.misc
{
	// Token: 0x0200006E RID: 110
	public static class Message
	{
		// Token: 0x06000349 RID: 841 RVA: 0x00012814 File Offset: 0x00010A14
		public static void ShowDialog(string message)
		{
			InformationManager.ShowInquiry(new InquiryData(null, message, true, false, "OK", null, null, null, "", 0f, null, null, null), true, false);
		}

		// Token: 0x0600034A RID: 842 RVA: 0x00012846 File Offset: 0x00010A46
		public static void display(string message, Color color)
		{
			InformationManager.DisplayMessage(new InformationMessage(message, color));
		}

		// Token: 0x0600034B RID: 843 RVA: 0x00012854 File Offset: 0x00010A54
		public static void display(string message)
		{
			Message.display(message, Colors.White);
		}

		// Token: 0x0600034C RID: 844 RVA: 0x00012861 File Offset: 0x00010A61
		[Conditional("DEBUG")]
		public static void println(string msg)
		{
			StackFrame stackFrame = new StackTrace().GetFrames()[1];
			string name = stackFrame.GetMethod().DeclaringType.Name;
			string name2 = stackFrame.GetMethod().Name;
		}

		// Token: 0x0600034D RID: 845 RVA: 0x0001288B File Offset: 0x00010A8B
		[Conditional("DEBUG")]
		public static void debug(string msg)
		{
			StackFrame stackFrame = new StackTrace().GetFrames()[1];
			string name = stackFrame.GetMethod().DeclaringType.Name;
			string name2 = stackFrame.GetMethod().Name;
		}

		// Token: 0x0600034E RID: 846 RVA: 0x000128B8 File Offset: 0x00010AB8
		public static void println(object o)
		{
			if (o == null)
			{
				return;
			}
			try
			{
			}
			catch (Exception)
			{
			}
		}
	}
}
