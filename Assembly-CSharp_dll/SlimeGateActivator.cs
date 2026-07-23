using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200077F RID: 1919
public class SlimeGateActivator : MonoBehaviour
{
	// Token: 0x06002824 RID: 10276 RVA: 0x00098148 File Offset: 0x00096348
	public void Activate()
	{
		if (SRSingleton<SceneContext>.Instance.PlayerState.SpendKey())
		{
			this.gateDoor.CurrState = AccessDoor.State.OPEN;
			SRSingleton<SceneContext>.Instance.AchievementsDirector.AddToStat(AchievementsDirector.IntStat.OPENED_SLIME_GATES, 1);
			AnalyticsUtil.CustomEvent("GateOpened", new Dictionary<string, object>
			{
				{
					"name",
					base.name
				}
			}, true);
			SRSingleton<GameContext>.Instance.AutoSaveDirector.SaveAllNow();
		}
	}

	// Token: 0x040027A9 RID: 10153
	public AccessDoor gateDoor;
}
