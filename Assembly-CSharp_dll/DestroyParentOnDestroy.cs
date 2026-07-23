using System;
using UnityEngine;

// Token: 0x0200011E RID: 286
public class DestroyParentOnDestroy : MonoBehaviour
{
	// Token: 0x06000625 RID: 1573 RVA: 0x0002240C File Offset: 0x0002060C
	public void OnDestroy()
	{
		Destroyer.Destroy(base.transform.parent.gameObject, "DestroyParentOnDestroy.OnDestroy");
	}
}
