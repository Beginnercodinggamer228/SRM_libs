using System;
using UnityEngine;

// Token: 0x020002D8 RID: 728
public class SRFPCamera : vp_FPCamera
{
	// Token: 0x06000F89 RID: 3977 RVA: 0x0003D65F File Offset: 0x0003B85F
	protected override void Awake()
	{
		base.Awake();
		this.optionsDir = SRSingleton<GameContext>.Instance.OptionsDirector;
		base.GetComponent<Camera>().cullingMask &= -8193;
		this.defaultBobAmp = this.BobAmplitude;
	}

	// Token: 0x06000F8A RID: 3978 RVA: 0x0003D69A File Offset: 0x0003B89A
	protected override void UpdateBob()
	{
		if (!this.optionsDir.disableCameraBob)
		{
			this.BobAmplitude = this.defaultBobAmp;
		}
		else
		{
			this.BobAmplitude = this.NO_BOB;
		}
		base.UpdateBob();
	}

	// Token: 0x04000E54 RID: 3668
	private OptionsDirector optionsDir;

	// Token: 0x04000E55 RID: 3669
	private Vector4 defaultBobAmp;

	// Token: 0x04000E56 RID: 3670
	private Vector4 NO_BOB = new Vector4(0f, 0.001f, 0f, 0f);
}
