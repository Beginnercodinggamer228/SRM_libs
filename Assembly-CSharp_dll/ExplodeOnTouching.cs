using System;
using UnityEngine;

// Token: 0x020006F8 RID: 1784
public class ExplodeOnTouching : SRBehaviour
{
	// Token: 0x0600252F RID: 9519 RVA: 0x0008EBE0 File Offset: 0x0008CDE0
	public void OnCollisionEnter(Collision col)
	{
		DestroyOnTouching component = col.gameObject.GetComponent<DestroyOnTouching>();
		if (component == null || component.wateringUnits <= 0f)
		{
			this.Explode();
		}
	}

	// Token: 0x06002530 RID: 9520 RVA: 0x0008EC18 File Offset: 0x0008CE18
	public void Explode()
	{
		PhysicsUtil.Explode(base.gameObject, this.explodeRadius, this.explodePower, this.minPlayerDamage, this.maxPlayerDamage, this.ignites);
		if (this.explodeFX != null)
		{
			SRBehaviour.SpawnAndPlayFX(this.explodeFX, base.transform.position, base.transform.rotation);
		}
		base.RequestDestroy("ExplodeOnTouching.Explode");
	}

	// Token: 0x04002407 RID: 9223
	public float explodePower = 600f;

	// Token: 0x04002408 RID: 9224
	public float explodeRadius = 7f;

	// Token: 0x04002409 RID: 9225
	public float minPlayerDamage = 15f;

	// Token: 0x0400240A RID: 9226
	public float maxPlayerDamage = 45f;

	// Token: 0x0400240B RID: 9227
	public bool ignites;

	// Token: 0x0400240C RID: 9228
	public GameObject explodeFX;
}
