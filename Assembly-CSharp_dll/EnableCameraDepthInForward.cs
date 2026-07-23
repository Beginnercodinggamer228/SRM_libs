using System;
using UnityEngine;

// Token: 0x020007E2 RID: 2018
[RequireComponent(typeof(Camera))]
public class EnableCameraDepthInForward : MonoBehaviour
{
	// Token: 0x06002A44 RID: 10820 RVA: 0x0009E63F File Offset: 0x0009C83F
	private void Start()
	{
		this.Set();
	}

	// Token: 0x06002A45 RID: 10821 RVA: 0x0009E647 File Offset: 0x0009C847
	private void Set()
	{
		if (base.GetComponent<Camera>().depthTextureMode == DepthTextureMode.None)
		{
			base.GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
		}
	}
}
