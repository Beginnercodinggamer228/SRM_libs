using System;
using UnityEngine;

// Token: 0x02000322 RID: 802
public class MineralSoilUpgrader : PlotUpgrader
{
	// Token: 0x060010FB RID: 4347 RVA: 0x00043F17 File Offset: 0x00042117
	public override void Apply(LandPlot.Upgrade upgrade)
	{
		if (upgrade == LandPlot.Upgrade.SOIL)
		{
			this.soil.SetActive(true);
		}
	}

	// Token: 0x04000FEF RID: 4079
	public GameObject soil;
}
