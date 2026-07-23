using System;
using RichPresence;
using UnityEngine;

// Token: 0x02000269 RID: 617
public class GameCoreXboxContext : MonoBehaviour, Handler
{
	// Token: 0x06000CFE RID: 3326 RVA: 0x00003296 File Offset: 0x00001496
	public void SetRichPresence(MainMenuData data)
	{
	}

	// Token: 0x06000CFF RID: 3327 RVA: 0x00003296 File Offset: 0x00001496
	public void SetRichPresence(InZoneData data)
	{
	}

	// Token: 0x04000C35 RID: 3125
	public GameCoreEngagementPopupUI engagementPopupUIPrefab;

	// Token: 0x04000C36 RID: 3126
	public XboxUserChangePopupUI userChangePopupUIPrefab;
}
