using System;
using UnityEngine;

// Token: 0x02000116 RID: 278
public class DestroyAfterSeconds : SRBehaviour
{
	// Token: 0x060005FD RID: 1533 RVA: 0x00021F78 File Offset: 0x00020178
	public void Awake()
	{
		this.awakeTime = Time.time;
	}

	// Token: 0x060005FE RID: 1534 RVA: 0x00021F85 File Offset: 0x00020185
	public void Update()
	{
		if (Time.time >= this.awakeTime + this.time)
		{
			Destroyer.DestroyActor(base.gameObject, "DestroyAfterSeconds.Update", false);
		}
	}

	// Token: 0x040005CE RID: 1486
	public float time;

	// Token: 0x040005CF RID: 1487
	private float awakeTime;
}
