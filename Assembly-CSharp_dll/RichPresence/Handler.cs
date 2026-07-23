using System;

namespace RichPresence
{
	// Token: 0x02000A28 RID: 2600
	public interface Handler
	{
		// Token: 0x060045F4 RID: 17908
		void SetRichPresence(MainMenuData data);

		// Token: 0x060045F5 RID: 17909
		void SetRichPresence(InZoneData data);
	}
}
