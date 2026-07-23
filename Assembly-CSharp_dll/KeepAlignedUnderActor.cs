using System;
using UnityEngine;

// Token: 0x02000400 RID: 1024
public class KeepAlignedUnderActor : MonoBehaviour
{
	// Token: 0x06001564 RID: 5476 RVA: 0x0005337A File Offset: 0x0005157A
	public void Start()
	{
		this.body = base.GetComponent<Rigidbody>();
	}

	// Token: 0x06001565 RID: 5477 RVA: 0x00053388 File Offset: 0x00051588
	public void AlignWith(Transform alignWith)
	{
		this.alignWith = alignWith;
	}

	// Token: 0x06001566 RID: 5478 RVA: 0x00053391 File Offset: 0x00051591
	public void FixedUpdate()
	{
		if (this.alignWith == null)
		{
			Destroyer.Destroy(base.gameObject, "KeepAlignedUnderActor.FixedUpdate");
			return;
		}
		this.body.MovePosition(this.alignWith.position);
	}

	// Token: 0x0400145B RID: 5211
	private Transform alignWith;

	// Token: 0x0400145C RID: 5212
	private Rigidbody body;
}
