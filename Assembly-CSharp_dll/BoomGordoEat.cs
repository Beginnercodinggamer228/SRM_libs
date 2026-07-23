using System;
using UnityEngine;

// Token: 0x0200039C RID: 924
public class BoomGordoEat : GordoEat, BoomMaterialAnimator.BoomMaterialInformer
{
	// Token: 0x0600134A RID: 4938 RVA: 0x0004B2AF File Offset: 0x000494AF
	protected override void WillStartBurst()
	{
		base.WillStartBurst();
		base.GetComponentsInChildren<ExplodeIndicatorMarker>(true)[0].SetActive(true);
	}

	// Token: 0x0600134B RID: 4939 RVA: 0x0004B2C8 File Offset: 0x000494C8
	protected override void DidCompleteBurst()
	{
		base.DidCompleteBurst();
		PhysicsUtil.Explode(base.gameObject, this.explodeRadius, this.explodePower, this.minPlayerDamage, this.maxPlayerDamage, base.gameObject);
		base.GetComponentsInChildren<ExplodeIndicatorMarker>(true)[0].SetActive(false);
	}

	// Token: 0x0600134C RID: 4940 RVA: 0x0004B318 File Offset: 0x00049518
	public float GetReadiness()
	{
		return Mathf.Lerp(0.2f, 1f, base.GetPercentageFed());
	}

	// Token: 0x0600134D RID: 4941 RVA: 0x0004B32F File Offset: 0x0004952F
	public float GetRecoveriness()
	{
		return 0f;
	}

	// Token: 0x04001204 RID: 4612
	public float explodePower = 600f;

	// Token: 0x04001205 RID: 4613
	public float explodeRadius = 7f;

	// Token: 0x04001206 RID: 4614
	public float minPlayerDamage = 15f;

	// Token: 0x04001207 RID: 4615
	public float maxPlayerDamage = 45f;
}
