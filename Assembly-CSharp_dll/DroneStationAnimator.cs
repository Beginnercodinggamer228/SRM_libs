using System;
using UnityEngine;

// Token: 0x020001AE RID: 430
public class DroneStationAnimator : SRAnimator
{
	// Token: 0x0600090D RID: 2317 RVA: 0x00028E04 File Offset: 0x00027004
	public void SetEnabled(bool enabled)
	{
		base.animator.SetBool(DroneStationAnimator.STATION_ENABLED, enabled);
	}

	// Token: 0x040007A7 RID: 1959
	private static readonly int STATION_ENABLED = Animator.StringToHash("STATION_ENABLED");
}
