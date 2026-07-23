using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

// Token: 0x02000686 RID: 1670
public class MessageDirector : MonoBehaviour
{
	// Token: 0x06002287 RID: 8839 RVA: 0x000855E0 File Offset: 0x000837E0
	public static MessageDirector.Lang GetLang(string code)
	{
		string value = code.ToUpperInvariant();
		if (Enum.IsDefined(typeof(MessageDirector.Lang), value))
		{
			return (MessageDirector.Lang)Enum.Parse(typeof(MessageDirector.Lang), value);
		}
		return MessageDirector.Lang.EN;
	}

	// Token: 0x06002288 RID: 8840 RVA: 0x0008561D File Offset: 0x0008381D
	public static MessageDirector.Lang GetLang(CultureInfo culture)
	{
		return MessageDirector.GetLang(culture.TwoLetterISOLanguageName);
	}

	// Token: 0x06002289 RID: 8841 RVA: 0x0008562A File Offset: 0x0008382A
	public void Awake()
	{
		this.SetCulture(MessageDirector.GetLang(CultureInfo.CurrentCulture));
	}

	// Token: 0x0600228A RID: 8842 RVA: 0x0008563C File Offset: 0x0008383C
	public CultureInfo GetCulture()
	{
		return this.culture;
	}

	// Token: 0x0600228B RID: 8843 RVA: 0x00085644 File Offset: 0x00083844
	private void SetCulture(SystemLanguage systemLanguage)
	{
		Debug.Log("Setting Culture given SystemLanguage: " + systemLanguage);
		SystemLanguage systemLanguage2 = Application.systemLanguage;
		if (systemLanguage2 <= SystemLanguage.German)
		{
			if (systemLanguage2 != SystemLanguage.Chinese)
			{
				if (systemLanguage2 == SystemLanguage.French)
				{
					this.SetCulture(MessageDirector.Lang.FR);
					return;
				}
				if (systemLanguage2 != SystemLanguage.German)
				{
					goto IL_7F;
				}
				this.SetCulture(MessageDirector.Lang.DE);
				return;
			}
		}
		else if (systemLanguage2 <= SystemLanguage.Spanish)
		{
			if (systemLanguage2 == SystemLanguage.Russian)
			{
				this.SetCulture(MessageDirector.Lang.RU);
				return;
			}
			if (systemLanguage2 != SystemLanguage.Spanish)
			{
				goto IL_7F;
			}
			this.SetCulture(MessageDirector.Lang.ES);
			return;
		}
		else
		{
			if (systemLanguage2 == SystemLanguage.Swedish)
			{
				this.SetCulture(MessageDirector.Lang.SV);
				return;
			}
			if (systemLanguage2 - SystemLanguage.ChineseSimplified > 1)
			{
				goto IL_7F;
			}
		}
		this.SetCulture(MessageDirector.Lang.ZH);
		return;
		IL_7F:
		this.SetCulture(MessageDirector.Lang.EN);
	}

	// Token: 0x0600228C RID: 8844 RVA: 0x000856D7 File Offset: 0x000838D7
	private void SetCulture(CultureInfo culture)
	{
		this.SetCulture(culture, true);
	}

	// Token: 0x0600228D RID: 8845 RVA: 0x000856E1 File Offset: 0x000838E1
	public string GetCurrentLanguageCode()
	{
		return this.GetCulture().TwoLetterISOLanguageName;
	}

	// Token: 0x0600228E RID: 8846 RVA: 0x000856EE File Offset: 0x000838EE
	public void SetCulture(MessageDirector.Lang lang)
	{
		this.SetCulture(MessageDirector.GetCultureInfo(lang));
	}

	// Token: 0x0600228F RID: 8847 RVA: 0x000856FC File Offset: 0x000838FC
	public static CultureInfo GetCultureInfo(MessageDirector.Lang lang)
	{
		string text = lang.ToString();
		if (text == "ZH")
		{
			text = "ZH-HANS";
		}
		return CultureInfo.GetCultureInfo(text);
	}

	// Token: 0x06002290 RID: 8848 RVA: 0x00085730 File Offset: 0x00083930
	private void SetCulture(CultureInfo culture, bool updateGlobal)
	{
		if (this.culture == culture)
		{
			return;
		}
		this.culture = culture;
		this.cache.Clear();
		if (updateGlobal)
		{
			this.global = this.GetBundle("global");
		}
		Log.Info("", new object[]
		{
			"Culture",
			culture
		});
		if (this.bundlesListeners != null)
		{
			this.bundlesListeners(this);
		}
	}

	// Token: 0x06002291 RID: 8849 RVA: 0x0008579D File Offset: 0x0008399D
	public MessageDirector.Lang GetCultureLang()
	{
		return MessageDirector.GetLang(this.culture);
	}

	// Token: 0x06002292 RID: 8850 RVA: 0x000857AA File Offset: 0x000839AA
	public void RegisterBundlesListener(MessageDirector.BundlesListener avail)
	{
		this.bundlesListeners = (MessageDirector.BundlesListener)Delegate.Combine(this.bundlesListeners, avail);
		avail(this);
	}

	// Token: 0x06002293 RID: 8851 RVA: 0x000857CA File Offset: 0x000839CA
	public void UnregisterBundlesListener(MessageDirector.BundlesListener avail)
	{
		this.bundlesListeners = (MessageDirector.BundlesListener)Delegate.Remove(this.bundlesListeners, avail);
	}

	// Token: 0x06002294 RID: 8852 RVA: 0x000857E4 File Offset: 0x000839E4
	public MessageBundle GetBundle(string path)
	{
		if (this.cache.ContainsKey(path))
		{
			return this.cache[path];
		}
		ResourceBundle resourceBundle = this.LoadBundle(path);
		MessageBundle customBundle = null;
		if (resourceBundle != null)
		{
			string text = null;
			try
			{
				text = resourceBundle.GetString("msgbundle_class");
				if (text != null)
				{
					text = text.Trim();
					if (text != "")
					{
						customBundle = (Type.GetType(text).GetConstructor(new Type[0]).Invoke(new object[0]) as MessageBundle);
					}
				}
			}
			catch (Exception ex)
			{
				Log.Warning("Failure instantiating custom message bundle", new object[]
				{
					"mbclass",
					text,
					"error",
					ex
				});
			}
		}
		MessageBundle messageBundle = this.CreateBundle(path, resourceBundle, customBundle);
		this.cache[path] = messageBundle;
		return messageBundle;
	}

	// Token: 0x06002295 RID: 8853 RVA: 0x000858B8 File Offset: 0x00083AB8
	public string Get(string path, string key)
	{
		return this.GetBundle(path).Get(key);
	}

	// Token: 0x06002296 RID: 8854 RVA: 0x000858C7 File Offset: 0x00083AC7
	public string Get(string path, string key, params object[] args)
	{
		return this.GetBundle(path).Get(key, args);
	}

	// Token: 0x06002297 RID: 8855 RVA: 0x000858D7 File Offset: 0x00083AD7
	protected MessageBundle CreateBundle(string path, ResourceBundle rbundle, MessageBundle customBundle)
	{
		if (customBundle == null)
		{
			customBundle = new MessageBundle();
		}
		this.InitBundle(customBundle, path, rbundle);
		return customBundle;
	}

	// Token: 0x06002298 RID: 8856 RVA: 0x000858F0 File Offset: 0x00083AF0
	protected void InitBundle(MessageBundle bundle, string path, ResourceBundle rbundle)
	{
		MessageBundle bundle2 = this.global;
		if (rbundle != null)
		{
			string @string = rbundle.GetString("__parent");
			if (@string != null)
			{
				bundle2 = this.GetBundle(@string);
			}
		}
		bundle.Init(this, path, rbundle, bundle2);
	}

	// Token: 0x06002299 RID: 8857 RVA: 0x00085928 File Offset: 0x00083B28
	protected ResourceBundle LoadBundle(string path)
	{
		ResourceBundle result;
		try
		{
			result = ResourceBundle.GetBundle(this.msgPath, path, this.culture, this.fallbackLang);
		}
		catch (MissingResourceException ex)
		{
			Log.Warning("Unable to resolve resource bundle", new object[]
			{
				"path",
				path,
				"culture",
				this.culture,
				ex
			});
			result = null;
		}
		return result;
	}

	// Token: 0x04002244 RID: 8772
	private MessageDirector.BundlesListener bundlesListeners;

	// Token: 0x04002245 RID: 8773
	public const string GLOBAL_BUNDLE = "global";

	// Token: 0x04002246 RID: 8774
	public string msgPath = "I18n";

	// Token: 0x04002247 RID: 8775
	public MessageDirector.Lang defaultLang;

	// Token: 0x04002248 RID: 8776
	public string fallbackLang = "en";

	// Token: 0x04002249 RID: 8777
	private CultureInfo culture;

	// Token: 0x0400224A RID: 8778
	private Dictionary<string, MessageBundle> cache = new Dictionary<string, MessageBundle>();

	// Token: 0x0400224B RID: 8779
	private MessageBundle global;

	// Token: 0x0400224C RID: 8780
	private const string MBUNDLE_CLASS_KEY = "msgbundle_class";

	// Token: 0x02000687 RID: 1671
	public enum Lang
	{
		// Token: 0x0400224E RID: 8782
		EN,
		// Token: 0x0400224F RID: 8783
		DE,
		// Token: 0x04002250 RID: 8784
		ES,
		// Token: 0x04002251 RID: 8785
		FR,
		// Token: 0x04002252 RID: 8786
		RU,
		// Token: 0x04002253 RID: 8787
		SV,
		// Token: 0x04002254 RID: 8788
		ZH,
		// Token: 0x04002255 RID: 8789
		JA,
		// Token: 0x04002256 RID: 8790
		PT,
		// Token: 0x04002257 RID: 8791
		KO
	}

	// Token: 0x02000688 RID: 1672
	// (Invoke) Token: 0x0600229C RID: 8860
	public delegate void BundlesListener(MessageDirector msgDir);
}
