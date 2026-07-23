using System;
using UnityEngine;

// Token: 0x020002BC RID: 700
public class PlayerIgniteReact : MonoBehaviour, Ignitable
{
	// Token: 0x06000ECE RID: 3790 RVA: 0x0003BA7E File Offset: 0x00039C7E
	public void Awake()
	{
		this.damageable = base.GetComponent<PlayerDamageable>();
	}

	// Token: 0x06000ECF RID: 3791 RVA: 0x0003BA8C File Offset: 0x00039C8C
	public void Ignite(GameObject igniter)
	{
		if ((double)Time.time >= this.nextTime)
		{
			this.TryToDamage(igniter);
		}
	}

	// Token: 0x06000ED0 RID: 3792 RVA: 0x0003BAA3 File Offset: 0x00039CA3
	private void TryToDamage(GameObject igniter)
	{
		if (this.damageable.Damage(this.damagePerIgnite, base.gameObject))
		{
			DeathHandler.Kill(base.gameObject, DeathHandler.Source.SLIME_IGNITE, igniter, "PlayerIgniteReact.TryToDamage");
		}
		this.nextTime = (double)(Time.time + this.repeatTime);
	}

	// Token: 0x04000DE3 RID: 3555
	public int damagePerIgnite = 10;

	// Token: 0x04000DE4 RID: 3556
	public float repeatTime = 1f;

	// Token: 0x04000DE5 RID: 3557
	private PlayerDamageable damageable;

	// Token: 0x04000DE6 RID: 3558
	private double nextTime;
}
