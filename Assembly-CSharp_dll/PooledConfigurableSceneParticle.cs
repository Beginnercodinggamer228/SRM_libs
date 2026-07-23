using System;
using UnityEngine;

// Token: 0x0200074B RID: 1867
public abstract class PooledConfigurableSceneParticle : PooledSceneParticle
{
	// Token: 0x06002703 RID: 9987 RVA: 0x0009455B File Offset: 0x0009275B
	protected override void InitParticle()
	{
		base.InitParticle();
		if (this.particle != null)
		{
			this.ConfigureParticles();
		}
	}

	// Token: 0x06002704 RID: 9988
	protected abstract void ConfigureParticles();

	// Token: 0x06002705 RID: 9989 RVA: 0x00094578 File Offset: 0x00092778
	protected void SetColors(string relObjPath, PooledConfigurableSceneParticle.MinMaxGradientData colorData)
	{
		Transform transform = (relObjPath == null) ? this.particle.transform : this.particle.transform.Find(relObjPath);
		ParticleSystem particleSystem = (transform == null) ? null : transform.GetComponent<ParticleSystem>();
		if (particleSystem != null)
		{
			PooledConfigurableSceneParticle.SetStartColors(particleSystem, colorData);
		}
	}

	// Token: 0x06002706 RID: 9990 RVA: 0x000945CC File Offset: 0x000927CC
	private static void SetStartColors(ParticleSystem part, PooledConfigurableSceneParticle.MinMaxGradientData colorData)
	{
		ParticleSystem.MainModule main = part.main;
		ParticleSystem.MinMaxGradient startColor = null;
		switch (colorData.mode)
		{
		case ParticleSystemGradientMode.Color:
			startColor = new ParticleSystem.MinMaxGradient(colorData.color);
			break;
		case ParticleSystemGradientMode.Gradient:
			startColor = new ParticleSystem.MinMaxGradient(colorData.gradient);
			break;
		case ParticleSystemGradientMode.TwoColors:
			startColor = new ParticleSystem.MinMaxGradient(colorData.colorMin, colorData.colorMax);
			break;
		case ParticleSystemGradientMode.TwoGradients:
			startColor = new ParticleSystem.MinMaxGradient(colorData.gradientMin, colorData.gradientMax);
			break;
		case ParticleSystemGradientMode.RandomColor:
			startColor = new ParticleSystem.MinMaxGradient(colorData.gradient);
			break;
		}
		startColor.mode = colorData.mode;
		main.startColor = startColor;
	}

	// Token: 0x0200074C RID: 1868
	[Serializable]
	public class MinMaxGradientData
	{
		// Token: 0x040026A4 RID: 9892
		public ParticleSystemGradientMode mode;

		// Token: 0x040026A5 RID: 9893
		public Color color;

		// Token: 0x040026A6 RID: 9894
		public Gradient gradient;

		// Token: 0x040026A7 RID: 9895
		public Color colorMin;

		// Token: 0x040026A8 RID: 9896
		public Color colorMax;

		// Token: 0x040026A9 RID: 9897
		public Color gradientMin;

		// Token: 0x040026AA RID: 9898
		public Color gradientMax;
	}
}
