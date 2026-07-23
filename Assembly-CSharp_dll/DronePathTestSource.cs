using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200015C RID: 348
public class DronePathTestSource : MonoBehaviour
{
	// Token: 0x0600077B RID: 1915 RVA: 0x00025758 File Offset: 0x00023958
	public void OnDrawGizmos()
	{
		DroneNetwork componentInParent = base.GetComponentInParent<DroneNetwork>();
		DronePathTestDest componentInChildren = componentInParent.GetComponentInChildren<DronePathTestDest>();
		Queue<Vector3> queue = componentInParent.GeneratePath(base.transform.position, componentInChildren.transform.position);
		if (queue == null)
		{
			return;
		}
		Gizmos.color = Color.green;
		Vector3 vector = queue.Dequeue();
		Gizmos.DrawLine(base.transform.position, vector);
		while (queue.Count > 0)
		{
			Vector3 vector2 = queue.Dequeue();
			Gizmos.DrawLine(vector, vector2);
			vector = vector2;
		}
	}
}
