using System;
using RichPresence;
using UnityEngine;

// Token: 0x020007D8 RID: 2008
public class XboxOneContext : MonoBehaviour, Handler
{
	// Token: 0x06002A0B RID: 10763 RVA: 0x0009DD4D File Offset: 0x0009BF4D
	public void SetRichPresence(MainMenuData data)
	{
		this.SetRichPresence("MainMenu");
	}

	// Token: 0x06002A0C RID: 10764 RVA: 0x0009DD5C File Offset: 0x0009BF5C
	public void SetRichPresence(InZoneData data)
	{
		string richPresence;
		if (Director.TryGetZoneId(data.zone, out richPresence))
		{
			this.SetRichPresence(richPresence);
		}
	}

	// Token: 0x06002A0D RID: 10765 RVA: 0x00003296 File Offset: 0x00001496
	private void SetRichPresence(string id)
	{
	}

	// Token: 0x04002939 RID: 10553
	public TextAsset xboxEvents;

	// Token: 0x0400293A RID: 10554
	public XboxEngagementPopupUI engagementPopupUIPrefab;

	// Token: 0x0400293B RID: 10555
	public XboxUserChangePopupUI userChangePopupUIPrefab;
}
