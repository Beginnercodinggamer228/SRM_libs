using System;
using UnityEngine;

// Token: 0x02000348 RID: 840
public class SprinklerUpgrader : PlotUpgrader
{
	// Token: 0x060011A7 RID: 4519 RVA: 0x00046CA0 File Offset: 0x00044EA0
	public override void Apply(LandPlot.Upgrade upgrade)
	{
		if (upgrade == LandPlot.Upgrade.SPRINKLER)
		{
			this.sprinkler.SetActive(true);
		}
	}

	// Token: 0x040010E2 RID: 4322
	public GameObject sprinkler;
}
