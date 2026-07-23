using System;
using UnityEngine;

// Token: 0x020003F2 RID: 1010
public class GordoNearBurstOnGameMode : MonoBehaviour
{
	// Token: 0x06001520 RID: 5408 RVA: 0x00051FBD File Offset: 0x000501BD
	public bool NearBurstForGameMode(PlayerState.GameMode currGameMode)
	{
		return currGameMode == PlayerState.GameMode.TIME_LIMIT_V2;
	}

	// Token: 0x040013F5 RID: 5109
	[Tooltip("Number of eaten counts remaining on the gordo.")]
	public uint remaining = 1U;
}
