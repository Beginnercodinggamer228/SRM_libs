using System;
using UnityEngine;

// Token: 0x020003BE RID: 958
public class DestroyOnWater : SRBehaviour, LiquidConsumer
{
	// Token: 0x06001405 RID: 5125 RVA: 0x0004D89D File Offset: 0x0004BA9D
	public void AddLiquid(Identifiable.Id liquidId, float units)
	{
		if (Identifiable.IsWater(liquidId))
		{
			this.DestroyWithFX();
		}
	}

	// Token: 0x06001406 RID: 5126 RVA: 0x0004D8B0 File Offset: 0x0004BAB0
	public void OnTriggerEnter(Collider col)
	{
		LiquidSource component = col.GetComponent<LiquidSource>();
		if (component != null && Identifiable.IsWater(component.liquidId))
		{
			this.DestroyWithFX();
			return;
		}
		DestroyOnTouching component2 = col.GetComponent<DestroyOnTouching>();
		if (component2 != null && component2.wateringUnits > 0f)
		{
			this.DestroyWithFX();
		}
	}

	// Token: 0x06001407 RID: 5127 RVA: 0x0004D904 File Offset: 0x0004BB04
	private void DestroyWithFX()
	{
		if (this.destroyFX != null)
		{
			SRBehaviour.SpawnAndPlayFX(this.destroyFX, base.transform.position, base.transform.rotation);
		}
		if (this.destroyAsActor)
		{
			Destroyer.DestroyActor(base.gameObject, "DestroyOnWater.DestroyWithFX", false);
			return;
		}
		base.RequestDestroy("DestroyOnWater.DestroyWithFX");
	}

	// Token: 0x040012BB RID: 4795
	public GameObject destroyFX;

	// Token: 0x040012BC RID: 4796
	public bool destroyAsActor;
}
