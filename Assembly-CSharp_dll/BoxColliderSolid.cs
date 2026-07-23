using System;
using UnityEngine;

// Token: 0x020000E3 RID: 227
[RequireComponent(typeof(BoxCollider))]
public class BoxColliderSolid : MonoBehaviour
{
	// Token: 0x0600054E RID: 1358 RVA: 0x000204DC File Offset: 0x0001E6DC
	public void OnDrawGizmos()
	{
		BoxCollider component = base.GetComponent<BoxCollider>();
		Gizmos.color = this.color;
		Gizmos.DrawCube(base.transform.TransformPoint(component.center), base.transform.TransformVector(component.size));
	}

	// Token: 0x04000551 RID: 1361
	public Color color = new Color(1f, 1f, 1f, 0.8f);
}
