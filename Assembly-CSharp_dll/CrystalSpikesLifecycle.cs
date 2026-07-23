using System;
using UnityEngine;

// Token: 0x020003B1 RID: 945
public class CrystalSpikesLifecycle : SRBehaviour, LiquidConsumer
{
	// Token: 0x060013B7 RID: 5047 RVA: 0x0004C8D0 File Offset: 0x0004AAD0
	public void Awake()
	{
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.destroyAt = this.timeDir.HoursFromNowOrStart(this.lifetime);
		if (this.spawnFX != null)
		{
			SRBehaviour.SpawnAndPlayFX(this.spawnFX, base.transform.position, base.transform.rotation);
		}
	}

	// Token: 0x060013B8 RID: 5048 RVA: 0x0004C934 File Offset: 0x0004AB34
	public void Update()
	{
		if (this.timeDir.HasReached(this.destroyAt))
		{
			bool flag = this.timeDir.HasReached(this.destroyAt + 3600.0);
			if (this.destroyFX != null && !flag)
			{
				SRBehaviour.SpawnAndPlayFX(this.destroyFX, base.transform.position, base.transform.rotation);
			}
			Destroyer.Destroy(base.gameObject, "CrystalSpikesLifecycle.Update");
		}
	}

	// Token: 0x060013B9 RID: 5049 RVA: 0x0004C9B4 File Offset: 0x0004ABB4
	public void OnTriggerEnter(Collider col)
	{
		if (!col.isTrigger)
		{
			Identifiable component = col.gameObject.GetComponent<Identifiable>();
			if (component != null && component.id == Identifiable.Id.PLAYER && col.gameObject.GetComponent<Damageable>().Damage(this.damagePerHit, base.gameObject))
			{
				DeathHandler.Kill(col.gameObject, DeathHandler.Source.SLIME_CRYSTAL_SPIKES, base.gameObject, "CrystalSpikesLifecycle.OnTriggerEnter");
			}
		}
	}

	// Token: 0x060013BA RID: 5050 RVA: 0x0004CA1D File Offset: 0x0004AC1D
	public void AddLiquid(Identifiable.Id liquidId, float units)
	{
		if (Identifiable.IsWater(liquidId))
		{
			SRBehaviour.SpawnAndPlayFX(this.destroyFX, base.transform.position, base.transform.rotation);
			Destroyer.Destroy(base.gameObject, "CrystalSpikesLifecycle.AddLiquid");
		}
	}

	// Token: 0x04001278 RID: 4728
	[Tooltip("Lifetime of spikes in hours")]
	public float lifetime = 0.5f;

	// Token: 0x04001279 RID: 4729
	public int damagePerHit = 10;

	// Token: 0x0400127A RID: 4730
	public GameObject spawnFX;

	// Token: 0x0400127B RID: 4731
	public GameObject destroyFX;

	// Token: 0x0400127C RID: 4732
	private double destroyAt;

	// Token: 0x0400127D RID: 4733
	private TimeDirector timeDir;
}
