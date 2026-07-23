using System;
using UnityEngine;

// Token: 0x02000871 RID: 2161
public class vp_HealthPickup : vp_Pickup
{
	// Token: 0x06002DB4 RID: 11700 RVA: 0x000AF1F4 File Offset: 0x000AD3F4
	protected override bool TryGive(vp_FPPlayerEventHandler player)
	{
		if (player.Health.Get() < 0f)
		{
			return false;
		}
		if (player.Health.Get() >= player.MaxHealth.Get())
		{
			return false;
		}
		player.Health.Set(Mathf.Min(player.MaxHealth.Get(), player.Health.Get() + this.Health));
		return true;
	}

	// Token: 0x04002BE0 RID: 11232
	public float Health = 1f;
}
