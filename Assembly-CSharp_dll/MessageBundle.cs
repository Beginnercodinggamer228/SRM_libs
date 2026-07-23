using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000685 RID: 1669
public class MessageBundle
{
	// Token: 0x0600227A RID: 8826 RVA: 0x0008517D File Offset: 0x0008337D
	public void Init(MessageDirector msgDir, string path, ResourceBundle bundle, MessageBundle parent)
	{
		this.msgDir = msgDir;
		this.path = path;
		this.bundle = bundle;
		this.parent = parent;
	}

	// Token: 0x0600227B RID: 8827 RVA: 0x0008519C File Offset: 0x0008339C
	public string Get(string key)
	{
		if (MessageUtil.IsTainted(key))
		{
			return MessageUtil.Untaint(key);
		}
		string resourceString = this.GetResourceString(key);
		if (resourceString == null)
		{
			return key;
		}
		return resourceString;
	}

	// Token: 0x0600227C RID: 8828 RVA: 0x000851C8 File Offset: 0x000833C8
	public void GetAll(string prefix, ICollection<string> messages, bool includeParent)
	{
		foreach (string text in this.bundle.GetKeys())
		{
			if (text.StartsWith(prefix))
			{
				messages.Add(this.Get(text));
			}
		}
		if (includeParent && this.parent != null)
		{
			this.parent.GetAll(prefix, messages, includeParent);
		}
	}

	// Token: 0x0600227D RID: 8829 RVA: 0x00085244 File Offset: 0x00083444
	public void GetAllKeys(string prefix, ICollection<string> keys, bool includeParent)
	{
		foreach (string text in this.bundle.GetKeys())
		{
			if (text.StartsWith(prefix))
			{
				keys.Add(text);
			}
		}
		if (includeParent && this.parent != null)
		{
			this.parent.GetAllKeys(prefix, keys, includeParent);
		}
	}

	// Token: 0x0600227E RID: 8830 RVA: 0x000852B8 File Offset: 0x000834B8
	public bool Exists(string key)
	{
		return this.GetResourceString(key, false) != null;
	}

	// Token: 0x0600227F RID: 8831 RVA: 0x000852C5 File Offset: 0x000834C5
	public string GetResourceString(string key)
	{
		return this.GetResourceString(key, true);
	}

	// Token: 0x06002280 RID: 8832 RVA: 0x000852D0 File Offset: 0x000834D0
	public string GetResourceString(string key, bool reportMissing)
	{
		string text = null;
		if (this.bundle != null)
		{
			text = this.bundle.GetString(key);
		}
		if (text != null)
		{
			return text;
		}
		if (this.parent != null)
		{
			string resourceString = this.parent.GetResourceString(key, false);
			if (resourceString != null)
			{
				return resourceString;
			}
		}
		if (reportMissing)
		{
			Log.Warning("Missing translation message", new object[]
			{
				"bundle",
				this.path,
				"key",
				key
			});
		}
		return null;
	}

	// Token: 0x06002281 RID: 8833 RVA: 0x00085348 File Offset: 0x00083548
	public string Get(string key, bool reportMissing, params object[] args)
	{
		if (key.StartsWith("%"))
		{
			return this.msgDir.GetBundle(MessageUtil.GetBundle(key)).Get(MessageUtil.GetUnqualifiedKey(key), args);
		}
		string suffix = this.GetSuffix(args);
		string resourceString = this.GetResourceString(key + suffix, false);
		if (resourceString == null)
		{
			if (suffix != "")
			{
				resourceString = this.GetResourceString(key, false);
			}
			if (resourceString == null)
			{
				if (reportMissing)
				{
					Log.Warning("Missing translation message", new object[]
					{
						"bundle",
						this.path,
						"key",
						key
					});
				}
				return key + StringUtil.ToString(args);
			}
		}
		string result;
		try
		{
			result = string.Format(resourceString, args);
		}
		catch (ArgumentException ex)
		{
			Log.Warning("Translation error: '" + ex.Message + "'", new object[]
			{
				"bundle",
				this.path,
				"key",
				key,
				"msg",
				resourceString,
				"args",
				args,
				ex
			});
			result = resourceString + StringUtil.ToString(args);
		}
		return result;
	}

	// Token: 0x06002282 RID: 8834 RVA: 0x00085478 File Offset: 0x00083678
	public string Get(string key, params object[] args)
	{
		return this.Get(key, true, args);
	}

	// Token: 0x06002283 RID: 8835 RVA: 0x00085484 File Offset: 0x00083684
	public string Get(string key, params string[] args)
	{
		return this.Get(key, args);
	}

	// Token: 0x06002284 RID: 8836 RVA: 0x0008549C File Offset: 0x0008369C
	public string GetSuffix(object[] args)
	{
		if (args.Length != 0 && args[0] != null)
		{
			try
			{
				int num = 0;
				if (args[0] is int)
				{
					num = (int)args[0];
				}
				else if (!int.TryParse(Convert.ToString(args[0]), out num))
				{
					return "";
				}
				if (num == 0)
				{
					return ".0";
				}
				if (num != 1)
				{
					return ".n";
				}
				return ".1";
			}
			catch (FormatException)
			{
				Debug.LogWarning("Format Exception in GetSuffix");
			}
		}
		return "";
	}

	// Token: 0x06002285 RID: 8837 RVA: 0x00085528 File Offset: 0x00083728
	public string Xlate(string compoundKey)
	{
		if (compoundKey.StartsWith("%"))
		{
			return this.msgDir.GetBundle(MessageUtil.GetBundle(compoundKey)).Xlate(MessageUtil.GetUnqualifiedKey(compoundKey));
		}
		int num = compoundKey.IndexOf('|');
		if (num == -1)
		{
			return this.Get(compoundKey);
		}
		string text = compoundKey.Substring(0, num);
		string[] array = compoundKey.Substring(num + 1).Split(new char[]
		{
			'|'
		});
		for (int i = 0; i < array.Length; i++)
		{
			if (MessageUtil.IsTainted(array[i]))
			{
				array[i] = MessageUtil.Unescape(MessageUtil.Untaint(array[i]));
			}
			else
			{
				array[i] = this.Xlate(MessageUtil.Unescape(array[i]));
			}
		}
		string key = text;
		object[] args = array;
		return this.Get(key, args);
	}

	// Token: 0x04002240 RID: 8768
	private MessageDirector msgDir;

	// Token: 0x04002241 RID: 8769
	private string path;

	// Token: 0x04002242 RID: 8770
	private ResourceBundle bundle;

	// Token: 0x04002243 RID: 8771
	private MessageBundle parent;
}
