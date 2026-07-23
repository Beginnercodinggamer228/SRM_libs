using System;
using UnityEngine;

// Token: 0x020007C9 RID: 1993
public class UnlockedOnGameMode : MonoBehaviour
{
	// Token: 0x060029BA RID: 10682 RVA: 0x00051FBD File Offset: 0x000501BD
	public bool IsUnlockedFor(PlayerState.GameMode currGameMode)
	{
		return currGameMode == PlayerState.GameMode.TIME_LIMIT_V2;
	}
}
