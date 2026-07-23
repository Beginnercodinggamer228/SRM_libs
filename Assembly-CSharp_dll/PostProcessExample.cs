using System;
using UnityEngine;

// Token: 0x0200000D RID: 13
[ExecuteInEditMode]
public class PostProcessExample : MonoBehaviour
{
	// Token: 0x06000040 RID: 64 RVA: 0x00003051 File Offset: 0x00001251
	private void Awake()
	{
		if (this.PostProcessMat == null)
		{
			base.enabled = false;
			return;
		}
		this.PostProcessMat.mainTexture = this.PostProcessMat.mainTexture;
	}

	// Token: 0x06000041 RID: 65 RVA: 0x0000307F File Offset: 0x0000127F
	private void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		Graphics.Blit(src, dest, this.PostProcessMat);
	}

	// Token: 0x04000029 RID: 41
	public Material PostProcessMat;
}
