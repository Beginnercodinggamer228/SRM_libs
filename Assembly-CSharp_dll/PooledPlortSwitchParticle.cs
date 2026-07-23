using System;

// Token: 0x0200074E RID: 1870
public class PooledPlortSwitchParticle : PooledConfigurableSceneParticle
{
	// Token: 0x0600270B RID: 9995 RVA: 0x000946B5 File Offset: 0x000928B5
	protected override void ConfigureParticles()
	{
		base.SetColors(null, this.coreStartColor);
		base.SetColors("FX Sparkle", this.sparkleStartColor);
		base.SetColors("Wisps", this.wispStartColor);
	}

	// Token: 0x040026AE RID: 9902
	public PooledConfigurableSceneParticle.MinMaxGradientData coreStartColor;

	// Token: 0x040026AF RID: 9903
	public PooledConfigurableSceneParticle.MinMaxGradientData sparkleStartColor;

	// Token: 0x040026B0 RID: 9904
	public PooledConfigurableSceneParticle.MinMaxGradientData wispStartColor;
}
