using System;
using UnityEngine;

// Token: 0x0200003B RID: 59
[Serializable]
public abstract class MessageOfTheDay
{
	// Token: 0x060000EA RID: 234 RVA: 0x00009A7C File Offset: 0x00007C7C
	public virtual string GetId()
	{
		return this.id;
	}

	// Token: 0x060000EB RID: 235 RVA: 0x00009A84 File Offset: 0x00007C84
	public Sprite GetSprite()
	{
		return this.sprite;
	}

	// Token: 0x060000EC RID: 236
	public abstract string GetUrl(string lang);

	// Token: 0x060000ED RID: 237
	public abstract string GetAnnouncementText(string lang);

	// Token: 0x060000EE RID: 238
	public abstract string GetTitleText(string lang);

	// Token: 0x060000EF RID: 239
	public abstract string GetBodyText(string lang);

	// Token: 0x060000F0 RID: 240
	public abstract string GetButtonText(string lang);

	// Token: 0x04000153 RID: 339
	public string id;

	// Token: 0x04000154 RID: 340
	public Sprite sprite;
}
