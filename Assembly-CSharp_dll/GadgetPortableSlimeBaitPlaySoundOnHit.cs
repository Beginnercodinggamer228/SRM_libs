using System;
using UnityEngine;

// Token: 0x020001F4 RID: 500
public class GadgetPortableSlimeBaitPlaySoundOnHit : SRBehaviour, ControllerCollisionListener
{
	// Token: 0x06000A75 RID: 2677 RVA: 0x0002D348 File Offset: 0x0002B548
	public void Awake()
	{
		this.parent = base.GetRequiredComponentInParent<GadgetPortableSlimeBait>(false);
	}

	// Token: 0x06000A76 RID: 2678 RVA: 0x0002D357 File Offset: 0x0002B557
	public void OnCollisionEnter(Collision collision)
	{
		this.parent.OnHit(base.transform);
	}

	// Token: 0x06000A77 RID: 2679 RVA: 0x0002D357 File Offset: 0x0002B557
	public void OnControllerCollision(GameObject gameObject)
	{
		this.parent.OnHit(base.transform);
	}

	// Token: 0x0400088D RID: 2189
	private GadgetPortableSlimeBait parent;
}
