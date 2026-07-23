using System;
using UnityEngine;

// Token: 0x020000EA RID: 234
public class CreateObjectOnEnable : SRBehaviour
{
	// Token: 0x0600055F RID: 1375 RVA: 0x00020676 File Offset: 0x0001E876
	public void OnEnable()
	{
		UnityEngine.Object.Instantiate<GameObject>(this.toCreate, Vector3.zero, Quaternion.identity).transform.SetParent(this.attachToParent ? base.transform.parent : base.transform, false);
	}

	// Token: 0x04000560 RID: 1376
	public GameObject toCreate;

	// Token: 0x04000561 RID: 1377
	public bool attachToParent;
}
