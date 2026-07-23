using System;
using System.Collections.Generic;
using DLCPackage;

// Token: 0x0200003D RID: 61
[Serializable]
public class BundledMessageOfTheDay : MessageOfTheDay
{
	// Token: 0x060000F9 RID: 249 RVA: 0x00009AA7 File Offset: 0x00007CA7
	public override string GetAnnouncementText(string lang)
	{
		return SRSingleton<GameContext>.Instance.MessageDirector.GetBundle("ui").Get(this.announcementTranslationId);
	}

	// Token: 0x060000FA RID: 250 RVA: 0x00009AC8 File Offset: 0x00007CC8
	public override string GetTitleText(string lang)
	{
		return SRSingleton<GameContext>.Instance.MessageDirector.GetBundle("ui").Get(this.titleTranslationId);
	}

	// Token: 0x060000FB RID: 251 RVA: 0x00009AE9 File Offset: 0x00007CE9
	public override string GetBodyText(string lang)
	{
		return SRSingleton<GameContext>.Instance.MessageDirector.GetBundle("ui").Get(this.bodyTranslationId);
	}

	// Token: 0x060000FC RID: 252 RVA: 0x00009B0A File Offset: 0x00007D0A
	public override string GetButtonText(string lang)
	{
		return SRSingleton<GameContext>.Instance.MessageDirector.GetBundle("ui").Get(this.buttonTranslationId);
	}

	// Token: 0x060000FD RID: 253 RVA: 0x00009B2B File Offset: 0x00007D2B
	public override string GetUrl(string lang)
	{
		return this.url;
	}

	// Token: 0x04000156 RID: 342
	public string url;

	// Token: 0x04000157 RID: 343
	public string announcementTranslationId;

	// Token: 0x04000158 RID: 344
	public string titleTranslationId;

	// Token: 0x04000159 RID: 345
	public string bodyTranslationId;

	// Token: 0x0400015A RID: 346
	public string buttonTranslationId;

	// Token: 0x0400015B RID: 347
	public List<Id> showForAvailableDLCPackages;
}
