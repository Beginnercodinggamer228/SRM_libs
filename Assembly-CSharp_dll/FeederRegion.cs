using System;
using UnityEngine;

// Token: 0x02000315 RID: 789
public class FeederRegion : SRBehaviour
{
	// Token: 0x060010C4 RID: 4292 RVA: 0x000433DC File Offset: 0x000415DC
	public void OnTriggerEnter(Collider collider)
	{
		TransformAfterTime component = collider.gameObject.GetComponent<TransformAfterTime>();
		if (component != null)
		{
			component.AddFeeder(this);
		}
	}

	// Token: 0x060010C5 RID: 4293 RVA: 0x00043408 File Offset: 0x00041608
	public void OnTriggerExit(Collider collider)
	{
		TransformAfterTime component = collider.gameObject.GetComponent<TransformAfterTime>();
		if (component != null)
		{
			component.RemoveFeeder(this);
		}
	}

	// Token: 0x04000FAB RID: 4011
	public const float GROWTH_FACTOR = 2f;
}
