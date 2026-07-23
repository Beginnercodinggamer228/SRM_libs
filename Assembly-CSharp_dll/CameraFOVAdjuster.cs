using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000294 RID: 660
public class CameraFOVAdjuster : SRBehaviour
{
	// Token: 0x06000DE4 RID: 3556 RVA: 0x000388C7 File Offset: 0x00036AC7
	public void Awake()
	{
		CameraFOVAdjuster.Instances.Add(this);
		this.ownCamera = base.GetRequiredComponent<Camera>();
	}

	// Token: 0x06000DE5 RID: 3557 RVA: 0x000388E0 File Offset: 0x00036AE0
	public void OnDestroy()
	{
		CameraFOVAdjuster.Instances.Remove(this);
	}

	// Token: 0x06000DE6 RID: 3558 RVA: 0x000388EE File Offset: 0x00036AEE
	public void SetFOV(float fov)
	{
		this.ownCamera.fieldOfView = fov;
	}

	// Token: 0x04000D17 RID: 3351
	public static List<CameraFOVAdjuster> Instances = new List<CameraFOVAdjuster>();

	// Token: 0x04000D18 RID: 3352
	private Camera ownCamera;
}
