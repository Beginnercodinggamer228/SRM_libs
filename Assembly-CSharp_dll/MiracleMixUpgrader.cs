using System;
using UnityEngine;

// Token: 0x02000324 RID: 804
public class MiracleMixUpgrader : PlotUpgrader
{
	// Token: 0x06001104 RID: 4356 RVA: 0x0004407E File Offset: 0x0004227E
	public override void Apply(LandPlot.Upgrade upgrade)
	{
		if (upgrade == LandPlot.Upgrade.MIRACLE_MIX)
		{
			this.miracleMix.SetActive(true);
			this.normSoil.SetActive(false);
		}
	}

	// Token: 0x04000FF2 RID: 4082
	public GameObject miracleMix;

	// Token: 0x04000FF3 RID: 4083
	public GameObject normSoil;
}
