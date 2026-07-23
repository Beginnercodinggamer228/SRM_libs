using System;
using UnityEngine;

// Token: 0x020003B3 RID: 947
public class DamagePlayerOnTouch_Trigger : RegisteredActorBehaviour, RegistryUpdateable
{
	// Token: 0x060013C3 RID: 5059 RVA: 0x0004CB4D File Offset: 0x0004AD4D
	public void Awake()
	{
		this.timeDirector = SRSingleton<SceneContext>.Instance.TimeDirector;
	}

	// Token: 0x060013C4 RID: 5060 RVA: 0x0004CB5F File Offset: 0x0004AD5F
	public void OnTriggerEnter(Collider collider)
	{
		if (this.damageGameObject == null && PhysicsUtil.IsPlayerMainCollider(collider))
		{
			this.damageGameObject = collider.gameObject;
		}
	}

	// Token: 0x060013C5 RID: 5061 RVA: 0x0004CB83 File Offset: 0x0004AD83
	public void OnTriggerExit(Collider collider)
	{
		if (this.damageGameObject == collider.gameObject)
		{
			this.damageGameObject = null;
		}
	}

	// Token: 0x060013C6 RID: 5062 RVA: 0x0004CBA0 File Offset: 0x0004ADA0
	public virtual void RegistryUpdate()
	{
		if (this.damageGameObject != null && this.timeDirector.HasReached(this.nextTime))
		{
			if (this.damageGameObject.GetInterfaceComponent<Damageable>().Damage(this.damagePerTick, base.gameObject))
			{
				DeathHandler.Kill(this.damageGameObject, DeathHandler.Source.SLIME_DAMAGE_PLAYER_ON_TOUCH, base.gameObject, "DamagePlayerOnTouch_Trigger.RegistryUpdate");
			}
			this.nextTime = this.timeDirector.HoursFromNow(this.cooldownPerTick * 0.016666668f);
		}
	}

	// Token: 0x04001283 RID: 4739
	[Tooltip("Amount of damage applied each tick.")]
	public int damagePerTick;

	// Token: 0x04001284 RID: 4740
	[Tooltip("Amount of time in between ticks. (in-game minutes)")]
	public float cooldownPerTick;

	// Token: 0x04001285 RID: 4741
	private TimeDirector timeDirector;

	// Token: 0x04001286 RID: 4742
	protected double nextTime;

	// Token: 0x04001287 RID: 4743
	protected GameObject damageGameObject;
}
