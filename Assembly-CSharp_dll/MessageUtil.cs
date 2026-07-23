using System;
using System.Text;
using System.Text.RegularExpressions;

// Token: 0x02000689 RID: 1673
public class MessageUtil
{
	// Token: 0x0600229F RID: 8863 RVA: 0x000859C1 File Offset: 0x00083BC1
	public static string Taint(object text)
	{
		return "~" + text;
	}

	// Token: 0x060022A0 RID: 8864 RVA: 0x000859CE File Offset: 0x00083BCE
	public static bool IsTainted(string text)
	{
		return text != null && text.StartsWith("~");
	}

	// Token: 0x060022A1 RID: 8865 RVA: 0x000859E0 File Offset: 0x00083BE0
	public static string Untaint(string text)
	{
		if (!MessageUtil.IsTainted(text))
		{
			return text;
		}
		return text.Substring("~".Length);
	}

	// Token: 0x060022A2 RID: 8866 RVA: 0x000859FC File Offset: 0x00083BFC
	public static string Compose(string key, params object[] args)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(key);
		stringBuilder.Append('|');
		for (int i = 0; i < args.Length; i++)
		{
			if (i > 0)
			{
				stringBuilder.Append('|');
			}
			string text = (args[i] == null) ? "" : Convert.ToString(args[i]);
			int length = text.Length;
			for (int j = 0; j < length; j++)
			{
				char c = text[j];
				if (c == '|')
				{
					stringBuilder.Append("\\!");
				}
				else if (c == '\\')
				{
					stringBuilder.Append("\\\\");
				}
				else
				{
					stringBuilder.Append(c);
				}
			}
		}
		return stringBuilder.ToString();
	}

	// Token: 0x060022A3 RID: 8867 RVA: 0x00085AA8 File Offset: 0x00083CA8
	public static string Compose(string key, params string[] args)
	{
		return MessageUtil.Compose(key, args);
	}

	// Token: 0x060022A4 RID: 8868 RVA: 0x00085AC0 File Offset: 0x00083CC0
	public static string Unescape(string value)
	{
		if (value.IndexOf('\\') == -1)
		{
			return value;
		}
		StringBuilder stringBuilder = new StringBuilder();
		int length = value.Length;
		for (int i = 0; i < length; i++)
		{
			char c = value[i];
			if (c != '\\' || i == length - 1)
			{
				stringBuilder.Append(c);
			}
			else
			{
				c = value[++i];
				stringBuilder.Append((c == '!') ? '|' : c);
			}
		}
		return stringBuilder.ToString();
	}

	// Token: 0x060022A5 RID: 8869 RVA: 0x00085B34 File Offset: 0x00083D34
	public static string Tcompose(string key, params object[] args)
	{
		int num = args.Length;
		string[] array = new string[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = MessageUtil.Taint(args[i]);
		}
		object[] args2 = array;
		return MessageUtil.Compose(key, args2);
	}

	// Token: 0x060022A6 RID: 8870 RVA: 0x00085B6C File Offset: 0x00083D6C
	public static string Tcompose(string key, params string[] args)
	{
		int i = 0;
		int num = args.Length;
		while (i < num)
		{
			args[i] = MessageUtil.Taint(args[i]);
			i++;
		}
		return MessageUtil.Compose(key, args);
	}

	// Token: 0x060022A7 RID: 8871 RVA: 0x00085B9C File Offset: 0x00083D9C
	public static string[] decompose(string compoundKey)
	{
		string[] array = Regex.Split(compoundKey, "\\|");
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = MessageUtil.Unescape(array[i]);
		}
		return array;
	}

	// Token: 0x060022A8 RID: 8872 RVA: 0x00085BD0 File Offset: 0x00083DD0
	public static string Qualify(string bundle, string key)
	{
		if (bundle.IndexOf("%") != -1 || bundle.IndexOf(":") != -1)
		{
			throw new ArgumentException(string.Concat(new string[]
			{
				"Message bundle may not contain '%' or ':' [bundle=",
				bundle,
				", key=",
				key,
				"]"
			}));
		}
		return "%" + bundle + ":" + key;
	}

	// Token: 0x060022A9 RID: 8873 RVA: 0x00085C3C File Offset: 0x00083E3C
	public static string GetBundle(string qualifiedKey)
	{
		if (!qualifiedKey.StartsWith("%"))
		{
			throw new ArgumentException(qualifiedKey + " is not a fully qualified message key.");
		}
		int num = qualifiedKey.IndexOf(":");
		if (num == -1)
		{
			throw new ArgumentException(qualifiedKey + " is not a valid fully qualified key.");
		}
		return qualifiedKey.Substring("%".Length, num - "%".Length);
	}

	// Token: 0x060022AA RID: 8874 RVA: 0x00085CA4 File Offset: 0x00083EA4
	public static string GetUnqualifiedKey(string qualifiedKey)
	{
		if (!qualifiedKey.StartsWith("%"))
		{
			throw new ArgumentException(qualifiedKey + " is not a fully qualified message key.");
		}
		int num = qualifiedKey.IndexOf(":");
		if (num == -1)
		{
			throw new ArgumentException(qualifiedKey + " is not a valid fully qualified key.");
		}
		return qualifiedKey.Substring(num + 1);
	}

	// Token: 0x04002258 RID: 8792
	public const string QUAL_PREFIX = "%";

	// Token: 0x04002259 RID: 8793
	public const string QUAL_SEP = ":";

	// Token: 0x0400225A RID: 8794
	private const string TAINT_CHAR = "~";
}
