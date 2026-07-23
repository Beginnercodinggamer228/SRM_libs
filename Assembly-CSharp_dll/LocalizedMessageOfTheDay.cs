using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200003E RID: 62
public class LocalizedMessageOfTheDay : MessageOfTheDay
{
	// Token: 0x060000FF RID: 255 RVA: 0x00009B33 File Offset: 0x00007D33
	public LocalizedMessageOfTheDay(string id, Sprite sprite, string defaultLanguageCode)
	{
		this.id = id;
		this.sprite = sprite;
		this.defaultLanguageCode = defaultLanguageCode;
	}

	// Token: 0x06000100 RID: 256 RVA: 0x00009B5C File Offset: 0x00007D5C
	public void AddEntry(string languageCode, string announcementText, string titleText, string bodyText, string buttonText, string url)
	{
		this.localizedEntries.Add(languageCode, new LocalizedMessageOfTheDay.LocalizedEntry
		{
			announcementText = announcementText,
			titleText = titleText,
			bodyText = bodyText,
			buttonText = buttonText,
			url = url
		});
	}

	// Token: 0x06000101 RID: 257 RVA: 0x00009BA9 File Offset: 0x00007DA9
	public override string GetAnnouncementText(string languageCode)
	{
		return this.GetEntryText(languageCode, (LocalizedMessageOfTheDay.LocalizedEntry entry) => entry.announcementText);
	}

	// Token: 0x06000102 RID: 258 RVA: 0x00009BD1 File Offset: 0x00007DD1
	public override string GetTitleText(string languageCode)
	{
		return this.GetEntryText(languageCode, (LocalizedMessageOfTheDay.LocalizedEntry entry) => entry.titleText);
	}

	// Token: 0x06000103 RID: 259 RVA: 0x00009BF9 File Offset: 0x00007DF9
	public override string GetBodyText(string languageCode)
	{
		return this.GetEntryText(languageCode, (LocalizedMessageOfTheDay.LocalizedEntry entry) => entry.bodyText);
	}

	// Token: 0x06000104 RID: 260 RVA: 0x00009C21 File Offset: 0x00007E21
	public override string GetButtonText(string languageCode)
	{
		return this.GetEntryText(languageCode, (LocalizedMessageOfTheDay.LocalizedEntry entry) => entry.buttonText);
	}

	// Token: 0x06000105 RID: 261 RVA: 0x00009C4C File Offset: 0x00007E4C
	private string GetEntryText(string languageCode, Func<LocalizedMessageOfTheDay.LocalizedEntry, string> extractor)
	{
		LocalizedMessageOfTheDay.LocalizedEntry arg;
		if (!this.TryGetLocalizedValue(languageCode, out arg))
		{
			return "";
		}
		return extractor(arg);
	}

	// Token: 0x06000106 RID: 262 RVA: 0x00009C71 File Offset: 0x00007E71
	private bool TryGetLocalizedValue(string languageCode, out LocalizedMessageOfTheDay.LocalizedEntry entry)
	{
		return this.localizedEntries.TryGetValue(languageCode, out entry) || this.localizedEntries.TryGetValue(this.defaultLanguageCode, out entry);
	}

	// Token: 0x06000107 RID: 263 RVA: 0x00009C98 File Offset: 0x00007E98
	public override string GetUrl(string languageCode)
	{
		LocalizedMessageOfTheDay.LocalizedEntry localizedEntry;
		if (this.localizedEntries.TryGetValue(languageCode, out localizedEntry))
		{
			return localizedEntry.url;
		}
		if (this.localizedEntries.TryGetValue(this.defaultLanguageCode, out localizedEntry))
		{
			return localizedEntry.url;
		}
		return "";
	}

	// Token: 0x0400015C RID: 348
	private Dictionary<string, LocalizedMessageOfTheDay.LocalizedEntry> localizedEntries = new Dictionary<string, LocalizedMessageOfTheDay.LocalizedEntry>();

	// Token: 0x0400015D RID: 349
	private string defaultLanguageCode;

	// Token: 0x0200003F RID: 63
	private struct LocalizedEntry
	{
		// Token: 0x0400015E RID: 350
		public string announcementText;

		// Token: 0x0400015F RID: 351
		public string titleText;

		// Token: 0x04000160 RID: 352
		public string bodyText;

		// Token: 0x04000161 RID: 353
		public string buttonText;

		// Token: 0x04000162 RID: 354
		public string url;
	}
}
