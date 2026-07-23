using System;
using UnityEngine;

// Token: 0x0200006C RID: 108
public class CFX_AutodestructWhenNoChildren : MonoBehaviour
{
	// Token: 0x060001DE RID: 478 RVA: 0x0000E439 File Offset: 0x0000C639
	private void Update()
	{
		if (base.transform.childCount == 0)
		{
			Destroyer.Destroy(base.gameObject, "CFX_AutodestructWhenNoChildren.Update");
		}
	}
}
