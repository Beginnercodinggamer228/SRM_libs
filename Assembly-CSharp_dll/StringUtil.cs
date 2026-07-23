using System;
using System.Linq;

// Token: 0x020006A9 RID: 1705
public class StringUtil
{
	// Token: 0x06002397 RID: 9111 RVA: 0x00089F3C File Offset: 0x0008813C
	public static string ToString(object arg)
	{
		if (arg is string[])
		{
			return string.Join(",", (string[])arg);
		}
		if (arg is object[])
		{
			return string.Join(",", (from XlateText in (object[])arg
			select XlateText.ToString()).ToArray<string>());
		}
		return Convert.ToString(arg);
	}

	// Token: 0x06002398 RID: 9112 RVA: 0x00089FAC File Offset: 0x000881AC
	public static string Pad(int val, int numDigits)
	{
		string text = string.Concat(val);
		while (text.Length < numDigits)
		{
			text = "0" + text;
		}
		return text;
	}
}
