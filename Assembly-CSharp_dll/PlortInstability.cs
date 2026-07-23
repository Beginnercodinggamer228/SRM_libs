using System;
using UnityEngine;

// Token: 0x0200041B RID: 1051
public class PlortInstability : SRBehaviour
{
	// Token: 0x060015EE RID: 5614 RVA: 0x0005514C File Offset: 0x0005334C
	public void Awake()
	{
		if (!SRSingleton<SceneContext>.Instance.ModDirector.PlortsUnstable())
		{
			Destroyer.Destroy(this, "PlortInstability.Awake");
		}
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.destroyTime = this.timeDir.HoursFromNowOrStart(this.lifetimeHours);
	}

	// Token: 0x060015EF RID: 5615 RVA: 0x0005519C File Offset: 0x0005339C
	public void Update()
	{
		if (this.timeDir.HasReached(this.destroyTime))
		{
			UnityEngine.Object.Instantiate<GameObject>(this.explodeFX, base.transform.position, base.transform.rotation);
			Destroyer.DestroyActor(base.gameObject, "PlortInstability.Update", false);
			PhysicsUtil.Explode(base.gameObject, this.explodeRadius, this.explodePower, this.minPlayerDamage, this.maxPlayerDamage, false);
		}
	}

	// Token: 0x040014DA RID: 5338
	public float lifetimeHours = 0.5f;

	// Token: 0x040014DB RID: 5339
	public float explodePower = 400f;

	// Token: 0x040014DC RID: 5340
	public float explodeRadius = 7f;

	// Token: 0x040014DD RID: 5341
	public float minPlayerDamage = 10f;

	// Token: 0x040014DE RID: 5342
	public float maxPlayerDamage = 30f;

	// Token: 0x040014DF RID: 5343
	public GameObject explodeFX;

	// Token: 0x040014E0 RID: 5344
	private double destroyTime;

	// Token: 0x040014E1 RID: 5345
	private TimeDirector timeDir;
}
