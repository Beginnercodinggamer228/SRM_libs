using System;
using UnityEngine;

// Token: 0x02000468 RID: 1128
public class SlimeDragUpdater : RegisteredActorBehaviour, RegistryFixedUpdateable
{
	// Token: 0x06001736 RID: 5942 RVA: 0x0005A02A File Offset: 0x0005822A
	public void Awake()
	{
		this.vacuumable = base.GetComponent<Vacuumable>();
		this.body = base.GetComponent<Rigidbody>();
	}

	// Token: 0x06001737 RID: 5943 RVA: 0x0005A044 File Offset: 0x00058244
	public void RegistryFixedUpdate()
	{
		if (this.vacuumable != null && this.vacuumable.isCaptive() && this.body != null)
		{
			this.body.drag = SlimeDragUpdater.DRAG_VAC;
			return;
		}
		if (this.body != null)
		{
			this.body.drag = SlimeDragUpdater.DRAG_NORM;
		}
	}

	// Token: 0x0400165F RID: 5727
	private static float DRAG_VAC = 0f;

	// Token: 0x04001660 RID: 5728
	private static float DRAG_NORM = 0.5f;

	// Token: 0x04001661 RID: 5729
	private Vacuumable vacuumable;

	// Token: 0x04001662 RID: 5730
	private Rigidbody body;
}
