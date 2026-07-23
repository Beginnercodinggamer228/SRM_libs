using System;
using UnityEngine;

// Token: 0x020006F9 RID: 1785
public class ExplodingFireBall : FireBall
{
	// Token: 0x06002532 RID: 9522 RVA: 0x0008ECBD File Offset: 0x0008CEBD
	protected override void OnExpire()
	{
		this.Explode();
	}

	// Token: 0x06002533 RID: 9523 RVA: 0x0008ECC8 File Offset: 0x0008CEC8
	public void Explode()
	{
		if (!this.defused)
		{
			PhysicsUtil.Explode(base.gameObject, this.explodeRadius, this.explodePower, this.minPlayerDamage, this.maxPlayerDamage, true);
			if (this.explodeFX != null)
			{
				SRBehaviour.SpawnAndPlayFX(this.explodeFX, base.transform.position, base.transform.rotation);
			}
		}
	}

	// Token: 0x0400240D RID: 9229
	public float explodePower = 600f;

	// Token: 0x0400240E RID: 9230
	public float explodeRadius = 7f;

	// Token: 0x0400240F RID: 9231
	public float minPlayerDamage = 15f;

	// Token: 0x04002410 RID: 9232
	public float maxPlayerDamage = 45f;

	// Token: 0x04002411 RID: 9233
	public GameObject explodeFX;
}
