using System;
using UnityEngine;

// Token: 0x0200054A RID: 1354
[RequireComponent(typeof(Camera))]
public class CameraFog : MonoBehaviour
{
	// Token: 0x06001C40 RID: 7232 RVA: 0x0006BBF4 File Offset: 0x00069DF4
	private void OnPreRender()
	{
		this.revertFogState = RenderSettings.fog;
		this.revertFogColor = RenderSettings.fogColor;
		this.revertFogDensity = RenderSettings.fogDensity;
		this.revertFogMode = RenderSettings.fogMode;
		this.revertFogStart = RenderSettings.fogStartDistance;
		this.revertFogEnd = RenderSettings.fogEndDistance;
		RenderSettings.fog = true;
		RenderSettings.fogColor = this.fogColor;
		RenderSettings.fogDensity = this.fogDensity;
		RenderSettings.fogMode = this.fogMode;
		RenderSettings.fogStartDistance = this.fogStartDistance;
		RenderSettings.fogEndDistance = this.fogEndDistance;
	}

	// Token: 0x06001C41 RID: 7233 RVA: 0x0006BC80 File Offset: 0x00069E80
	private void OnPostRender()
	{
		RenderSettings.fog = this.revertFogState;
		RenderSettings.fogColor = this.revertFogColor;
		RenderSettings.fogDensity = this.revertFogDensity;
		RenderSettings.fogMode = this.revertFogMode;
		RenderSettings.fogStartDistance = this.revertFogStart;
		RenderSettings.fogEndDistance = this.revertFogEnd;
	}

	// Token: 0x04001B40 RID: 6976
	public Color fogColor;

	// Token: 0x04001B41 RID: 6977
	public FogMode fogMode;

	// Token: 0x04001B42 RID: 6978
	[Header("Linear Mode Properties")]
	public float fogEndDistance = 6f;

	// Token: 0x04001B43 RID: 6979
	public float fogStartDistance = 3f;

	// Token: 0x04001B44 RID: 6980
	[Header("Exponential Mode Properties")]
	public float fogDensity = 100f;

	// Token: 0x04001B45 RID: 6981
	private bool revertFogState;

	// Token: 0x04001B46 RID: 6982
	private Color revertFogColor;

	// Token: 0x04001B47 RID: 6983
	private float revertFogDensity;

	// Token: 0x04001B48 RID: 6984
	private FogMode revertFogMode;

	// Token: 0x04001B49 RID: 6985
	private float revertFogStart;

	// Token: 0x04001B4A RID: 6986
	private float revertFogEnd;
}
