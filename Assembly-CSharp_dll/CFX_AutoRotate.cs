using System;
using UnityEngine;

// Token: 0x0200006B RID: 107
public class CFX_AutoRotate : MonoBehaviour
{
	// Token: 0x060001DC RID: 476 RVA: 0x0000E407 File Offset: 0x0000C607
	private void Update()
	{
		base.transform.Rotate(this.rotation * Time.deltaTime, this.space);
	}

	// Token: 0x0400023E RID: 574
	public Vector3 rotation;

	// Token: 0x0400023F RID: 575
	public Space space = Space.Self;
}
