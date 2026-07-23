using System;
using UnityEngine;

// Token: 0x02000069 RID: 105
public class CFX_Demo_RotateCamera : MonoBehaviour
{
	// Token: 0x060001D4 RID: 468 RVA: 0x0000E28E File Offset: 0x0000C48E
	private void Update()
	{
		if (CFX_Demo_RotateCamera.rotating)
		{
			base.transform.RotateAround(this.rotationCenter.position, Vector3.up, this.speed * Time.deltaTime);
		}
	}

	// Token: 0x04000233 RID: 563
	public static bool rotating = true;

	// Token: 0x04000234 RID: 564
	public float speed = 30f;

	// Token: 0x04000235 RID: 565
	public Transform rotationCenter;
}
