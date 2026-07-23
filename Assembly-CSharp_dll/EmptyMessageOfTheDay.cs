using System;

// Token: 0x0200003C RID: 60
public class EmptyMessageOfTheDay : MessageOfTheDay
{
	// Token: 0x060000F2 RID: 242 RVA: 0x00009A8C File Offset: 0x00007C8C
	private EmptyMessageOfTheDay()
	{
	}

	// Token: 0x060000F3 RID: 243 RVA: 0x00009A94 File Offset: 0x00007C94
	public override string GetUrl(string lang)
	{
		return "";
	}

	// Token: 0x060000F4 RID: 244 RVA: 0x00009A94 File Offset: 0x00007C94
	public override string GetAnnouncementText(string lang)
	{
		return "";
	}

	// Token: 0x060000F5 RID: 245 RVA: 0x00009A94 File Offset: 0x00007C94
	public override string GetTitleText(string lang)
	{
		return "";
	}

	// Token: 0x060000F6 RID: 246 RVA: 0x00009A94 File Offset: 0x00007C94
	public override string GetBodyText(string lang)
	{
		return "";
	}

	// Token: 0x060000F7 RID: 247 RVA: 0x00009A94 File Offset: 0x00007C94
	public override string GetButtonText(string lang)
	{
		return "";
	}

	// Token: 0x04000155 RID: 341
	public static EmptyMessageOfTheDay Default = new EmptyMessageOfTheDay();
}
