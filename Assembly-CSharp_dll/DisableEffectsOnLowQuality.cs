using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

// Token: 0x02000299 RID: 665
public class DisableEffectsOnLowQuality : MonoBehaviour
{
	// Token: 0x06000DF6 RID: 3574 RVA: 0x00038B2C File Offset: 0x00036D2C
	public void Awake()
	{
		this.CheckQuality();
	}

	// Token: 0x06000DF7 RID: 3575 RVA: 0x00038B2C File Offset: 0x00036D2C
	public void Update()
	{
		this.CheckQuality();
	}

	// Token: 0x06000DF8 RID: 3576 RVA: 0x00038B34 File Offset: 0x00036D34
	private void CheckQuality()
	{
		SSAOPro component = base.GetComponent<SSAOPro>();
		if (component.enabled != SRQualitySettings.AmbientOcclusion)
		{
			component.enabled = SRQualitySettings.AmbientOcclusion;
		}
		Bloom component2 = base.GetComponent<Bloom>();
		if (component2.enabled != SRQualitySettings.Bloom)
		{
			component2.enabled = SRQualitySettings.Bloom;
		}
		DepthTextureMode depthTextureMode = SRQualitySettings.GetDepthTextureMode();
		if (depthTextureMode != this.lastDepthMode)
		{
			base.GetComponent<Camera>().depthTextureMode = depthTextureMode;
			this.lastDepthMode = depthTextureMode;
		}
	}

	// Token: 0x04000D26 RID: 3366
	private DepthTextureMode lastDepthMode;
}
