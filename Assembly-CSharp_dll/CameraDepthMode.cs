using System;
using Assets.Script.Util.Extensions;
using UnityEngine;

// Token: 0x020000E5 RID: 229
public class CameraDepthMode : MonoBehaviour
{
	// Token: 0x06000551 RID: 1361 RVA: 0x00020549 File Offset: 0x0001E749
	private void Start()
	{
		this.GetRequiredComponent<Camera>().depthTextureMode = this.depth;
	}

	// Token: 0x0400055D RID: 1373
	public DepthTextureMode depth;
}
