using System;
using UnityEngine;

// Token: 0x020000E6 RID: 230
public class CameraFacingBillboard : MonoBehaviour
{
	// Token: 0x06000553 RID: 1363 RVA: 0x0002055C File Offset: 0x0001E75C
	public void Awake()
	{
		this.FaceCamera();
	}

	// Token: 0x06000554 RID: 1364 RVA: 0x0002055C File Offset: 0x0001E75C
	public void OnRenderObject()
	{
		this.FaceCamera();
	}

	// Token: 0x06000555 RID: 1365 RVA: 0x00020564 File Offset: 0x0001E764
	private void FaceCamera()
	{
		if (this.mainCamera == null)
		{
			this.mainCamera = Camera.main;
		}
		base.transform.LookAt(this.mainCamera.transform);
	}

	// Token: 0x0400055E RID: 1374
	private Camera mainCamera;
}
