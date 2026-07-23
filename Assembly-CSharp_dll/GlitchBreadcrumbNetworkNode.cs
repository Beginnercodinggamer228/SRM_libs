using System;
using UnityEngine;

// Token: 0x020004D5 RID: 1237
public class GlitchBreadcrumbNetworkNode : PathingNetworkNode
{
	// Token: 0x060019E0 RID: 6624 RVA: 0x00065139 File Offset: 0x00063339
	public void Awake()
	{
		if (this.onActiveFX != null)
		{
			this.onActiveFX.SetActive(false);
		}
	}

	// Token: 0x060019E1 RID: 6625 RVA: 0x00065155 File Offset: 0x00063355
	public void Activate(Vector3 nextPoint)
	{
		if (this.onActiveFX != null)
		{
			this.onActiveFX.SetActive(true);
			this.onActiveFX.transform.rotation = Quaternion.LookRotation(nextPoint - base.position);
		}
	}

	// Token: 0x060019E2 RID: 6626 RVA: 0x00065139 File Offset: 0x00063339
	public void Deactivate()
	{
		if (this.onActiveFX != null)
		{
			this.onActiveFX.SetActive(false);
		}
	}

	// Token: 0x04001982 RID: 6530
	[Tooltip("GameObject that is enabled while this breadcrumb is active. (optional)")]
	public GameObject onActiveFX;
}
