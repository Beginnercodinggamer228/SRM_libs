using System;
using UnityEngine;

// Token: 0x02000316 RID: 790
public class FeederUpgrader : PlotUpgrader
{
	// Token: 0x060010C7 RID: 4295 RVA: 0x00043431 File Offset: 0x00041631
	public override void Apply(LandPlot.Upgrade upgrade)
	{
		if (upgrade == LandPlot.Upgrade.FEEDER)
		{
			this.feeder.SetActive(true);
		}
	}

	// Token: 0x04000FAC RID: 4012
	public GameObject feeder;
}
