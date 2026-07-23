using System;
using UnityEngine;

// Token: 0x020000E9 RID: 233
public class ControllerCollisionForwarder : SRBehaviour
{
	// Token: 0x0600055D RID: 1373 RVA: 0x0002061C File Offset: 0x0001E81C
	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if (hit.gameObject != null)
		{
			Component[] components = hit.gameObject.GetComponents(typeof(ControllerCollisionListener));
			for (int i = 0; i < components.Length; i++)
			{
				((ControllerCollisionListener)components[i]).OnControllerCollision(base.gameObject);
			}
		}
	}
}
