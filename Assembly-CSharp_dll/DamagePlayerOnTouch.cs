using System;
using UnityEngine;

// Token: 0x020003B2 RID: 946
public class DamagePlayerOnTouch : SRBehaviour, ControllerCollisionListener
{
	// Token: 0x060013BC RID: 5052 RVA: 0x0004CA74 File Offset: 0x0004AC74
	public void Awake()
	{
		this.ResetDamageAmnesty();
	}

	// Token: 0x060013BD RID: 5053 RVA: 0x0004CA7C File Offset: 0x0004AC7C
	public void ResetDamageAmnesty()
	{
		this.nextTime = Time.time + 0.1f;
	}

	// Token: 0x060013BE RID: 5054 RVA: 0x0004CA8F File Offset: 0x0004AC8F
	public void OnControllerCollision(GameObject gameObj)
	{
		if (Time.time >= this.nextTime)
		{
			this.TryToDamage(gameObj);
		}
	}

	// Token: 0x060013BF RID: 5055 RVA: 0x0004CAA5 File Offset: 0x0004ACA5
	public void OnCollisionEnter(Collision col)
	{
		if (Time.time >= this.nextTime && col.gameObject == SRSingleton<SceneContext>.Instance.Player)
		{
			this.TryToDamage(col.gameObject);
		}
	}

	// Token: 0x060013C0 RID: 5056 RVA: 0x0004CAD7 File Offset: 0x0004ACD7
	public void SetBlocked(bool blocked)
	{
		this.blocked = blocked;
	}

	// Token: 0x060013C1 RID: 5057 RVA: 0x0004CAE0 File Offset: 0x0004ACE0
	private void TryToDamage(GameObject gameObj)
	{
		if (!this.blocked && gameObj.GetInterfaceComponent<Damageable>().Damage(this.damagePerTouch, base.gameObject))
		{
			DeathHandler.Kill(gameObj, DeathHandler.Source.SLIME_DAMAGE_PLAYER_ON_TOUCH, base.gameObject, "DamagePlayerOnTouch.TryToDamage");
		}
		this.nextTime = Time.time + this.repeatTime;
	}

	// Token: 0x0400127E RID: 4734
	public int damagePerTouch = 10;

	// Token: 0x0400127F RID: 4735
	public float repeatTime = 1f;

	// Token: 0x04001280 RID: 4736
	private bool blocked;

	// Token: 0x04001281 RID: 4737
	private float nextTime;

	// Token: 0x04001282 RID: 4738
	private const float INIT_NO_DAMAGE_WINDOW = 0.1f;
}
