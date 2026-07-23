using System;

namespace RichPresence
{
	// Token: 0x02000A29 RID: 2601
	public class NoopHandler : Handler
	{
		// Token: 0x060045F6 RID: 17910 RVA: 0x00003296 File Offset: 0x00001496
		public void SetRichPresence(MainMenuData data)
		{
		}

		// Token: 0x060045F7 RID: 17911 RVA: 0x00003296 File Offset: 0x00001496
		public void SetRichPresence(InZoneData data)
		{
		}

		// Token: 0x040033D3 RID: 13267
		public static NoopHandler Instance = new NoopHandler();
	}
}
