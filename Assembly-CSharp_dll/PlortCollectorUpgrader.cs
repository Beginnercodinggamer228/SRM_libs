using System;
using UnityEngine;

// Token: 0x0200032C RID: 812
public class PlortCollectorUpgrader : PlotUpgrader
{
	// Token: 0x06001121 RID: 4385 RVA: 0x00044C9D File Offset: 0x00042E9D
	public override void Apply(LandPlot.Upgrade upgrade)
	{
		if (upgrade == LandPlot.Upgrade.PLORT_COLLECTOR)
		{
			this.collector.SetActive(true);
		}
	}

	// Token: 0x0400101E RID: 4126
	[Tooltip("The collector object we need to activate/deactivate")]
	public GameObject collector;
}
