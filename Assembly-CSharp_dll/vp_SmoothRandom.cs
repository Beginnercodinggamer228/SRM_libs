using System;
using UnityEngine;

// Token: 0x0200082F RID: 2095
public class vp_SmoothRandom
{
	// Token: 0x06002BE8 RID: 11240 RVA: 0x000A537C File Offset: 0x000A357C
	public static Vector3 GetVector3(float speed)
	{
		float x = Time.time * 0.01f * speed;
		return new Vector3(vp_SmoothRandom.Get().HybridMultifractal(x, 15.73f, 0.58f), vp_SmoothRandom.Get().HybridMultifractal(x, 63.94f, 0.58f), vp_SmoothRandom.Get().HybridMultifractal(x, 0.2f, 0.58f));
	}

	// Token: 0x06002BE9 RID: 11241 RVA: 0x000A53DC File Offset: 0x000A35DC
	public static Vector3 GetVector3Centered(float speed)
	{
		float x = Time.time * 0.01f * speed;
		float x2 = (Time.time - 1f) * 0.01f * speed;
		Vector3 a = new Vector3(vp_SmoothRandom.Get().HybridMultifractal(x, 15.73f, 0.58f), vp_SmoothRandom.Get().HybridMultifractal(x, 63.94f, 0.58f), vp_SmoothRandom.Get().HybridMultifractal(x, 0.2f, 0.58f));
		Vector3 b = new Vector3(vp_SmoothRandom.Get().HybridMultifractal(x2, 15.73f, 0.58f), vp_SmoothRandom.Get().HybridMultifractal(x2, 63.94f, 0.58f), vp_SmoothRandom.Get().HybridMultifractal(x2, 0.2f, 0.58f));
		return a - b;
	}

	// Token: 0x06002BEA RID: 11242 RVA: 0x000A549C File Offset: 0x000A369C
	public static float Get(float speed)
	{
		float num = Time.time * 0.01f * speed;
		return vp_SmoothRandom.Get().HybridMultifractal(num * 0.01f, 15.7f, 0.65f);
	}

	// Token: 0x06002BEB RID: 11243 RVA: 0x000A54D2 File Offset: 0x000A36D2
	private static vp_FractalNoise Get()
	{
		if (vp_SmoothRandom.s_Noise == null)
		{
			vp_SmoothRandom.s_Noise = new vp_FractalNoise(1.27f, 2.04f, 8.36f);
		}
		return vp_SmoothRandom.s_Noise;
	}

	// Token: 0x04002A22 RID: 10786
	private static vp_FractalNoise s_Noise;
}
