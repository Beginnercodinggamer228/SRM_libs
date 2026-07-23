using System;
using UnityEngine;

// Token: 0x0200029B RID: 667
public class FallDamager : SRBehaviour, EventHandlerRegistrable
{
	// Token: 0x06000E0E RID: 3598 RVA: 0x000391A2 File Offset: 0x000373A2
	public void Awake()
	{
		this.playerEvents = base.GetComponentInChildren<vp_FPPlayerEventHandler>();
		this.damageable = base.GetInterfaceComponent<Damageable>();
	}

	// Token: 0x06000E0F RID: 3599 RVA: 0x000391BC File Offset: 0x000373BC
	protected virtual void OnEnable()
	{
		if (this.playerEvents != null)
		{
			this.Register(this.playerEvents);
		}
	}

	// Token: 0x06000E10 RID: 3600 RVA: 0x000391D8 File Offset: 0x000373D8
	protected virtual void OnDisable()
	{
		if (this.playerEvents != null)
		{
			this.Unregister(this.playerEvents);
		}
	}

	// Token: 0x06000E11 RID: 3601 RVA: 0x000391F4 File Offset: 0x000373F4
	public virtual void OnMessage_FallImpact(float val)
	{
		if (val > this.minImpactForDamage)
		{
			float f = (val - this.minImpactForDamage) * this.damagePerImpact;
			if (this.damageable.Damage(Mathf.RoundToInt(f), null))
			{
				DeathHandler.Kill(base.gameObject, DeathHandler.Source.FALL_DAMAGE, null, "FallDamager.OnMessage_FallImpact");
			}
		}
	}

	// Token: 0x06000E12 RID: 3602 RVA: 0x00039241 File Offset: 0x00037441
	public void Register(vp_EventHandler eventHandler)
	{
		eventHandler.RegisterMessage<float>("FallImpact", new vp_Message<float>.Sender<float>(this.OnMessage_FallImpact));
	}

	// Token: 0x06000E13 RID: 3603 RVA: 0x0003925B File Offset: 0x0003745B
	public void Unregister(vp_EventHandler eventHandler)
	{
		eventHandler.UnregisterMessage<float>("FallImpact", new vp_Message<float>.Sender<float>(this.OnMessage_FallImpact));
	}

	// Token: 0x04000D40 RID: 3392
	public float minImpactForDamage = 0.2f;

	// Token: 0x04000D41 RID: 3393
	public float damagePerImpact = 300f;

	// Token: 0x04000D42 RID: 3394
	private vp_FPPlayerEventHandler playerEvents;

	// Token: 0x04000D43 RID: 3395
	private Damageable damageable;
}
