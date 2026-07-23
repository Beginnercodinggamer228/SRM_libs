using System;
using UnityEngine;

// Token: 0x020006F4 RID: 1780
public class EggActivator : SRBehaviour
{
	// Token: 0x0600251E RID: 9502 RVA: 0x0008E95F File Offset: 0x0008CB5F
	public void AddEgg()
	{
		this.endEgg = Time.time + this.eggPeriod;
		base.enabled = true;
		this.activateObj.SetActive(true);
	}

	// Token: 0x0600251F RID: 9503 RVA: 0x0008E986 File Offset: 0x0008CB86
	public void Update()
	{
		if (Time.time >= this.endEgg)
		{
			this.endEgg = 0f;
			base.enabled = false;
			this.activateObj.SetActive(false);
			Destroyer.Destroy(this, "EggActivator.Update");
		}
	}

	// Token: 0x040023FC RID: 9212
	public float eggPeriod = 3f;

	// Token: 0x040023FD RID: 9213
	public GameObject activateObj;

	// Token: 0x040023FE RID: 9214
	private float endEgg;
}
