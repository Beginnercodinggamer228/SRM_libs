using System;
using UnityEngine;

// Token: 0x0200034D RID: 845
public class VitamizerUpgrader : PlotUpgrader
{
	// Token: 0x060011B6 RID: 4534 RVA: 0x00046EB8 File Offset: 0x000450B8
	public override void Apply(LandPlot.Upgrade upgrade)
	{
		if (upgrade == LandPlot.Upgrade.VITAMIZER)
		{
			this.vitamizer.SetActive(true);
		}
	}

	// Token: 0x040010F1 RID: 4337
	public GameObject vitamizer;
}
