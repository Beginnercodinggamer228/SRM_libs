using System;
using InControl;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200058B RID: 1419
public class GameCoreXboxStartScreen : MonoBehaviour
{
	// Token: 0x04001C7B RID: 7291
	public Text actionText;

	// Token: 0x04001C7C RID: 7292
	public Text gamerNameText;

	// Token: 0x04001C7D RID: 7293
	public GameObject happySlime;

	// Token: 0x04001C7E RID: 7294
	private GameCoreXboxStartScreen.EngagementScreenActions engagementActions;

	// Token: 0x0200058C RID: 1420
	private class EngagementScreenActions : PlayerActionSet
	{
		// Token: 0x06001D73 RID: 7539 RVA: 0x0006FDE5 File Offset: 0x0006DFE5
		public EngagementScreenActions()
		{
			this.Engage = base.CreatePlayerAction("Engage");
		}

		// Token: 0x04001C7F RID: 7295
		public PlayerAction Engage;
	}
}
