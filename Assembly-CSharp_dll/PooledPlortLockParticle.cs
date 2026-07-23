using System;

// Token: 0x0200074D RID: 1869
public class PooledPlortLockParticle : PooledConfigurableSceneParticle
{
	// Token: 0x06002709 RID: 9993 RVA: 0x0009467C File Offset: 0x0009287C
	protected override void ConfigureParticles()
	{
		base.SetColors(null, this.coreStartColor);
		base.SetColors("Wisps", this.wispStartColor);
		base.SetColors("FX Bursts", this.burstStartColor);
	}

	// Token: 0x040026AB RID: 9899
	public PooledConfigurableSceneParticle.MinMaxGradientData coreStartColor;

	// Token: 0x040026AC RID: 9900
	public PooledConfigurableSceneParticle.MinMaxGradientData wispStartColor;

	// Token: 0x040026AD RID: 9901
	public PooledConfigurableSceneParticle.MinMaxGradientData burstStartColor;
}
