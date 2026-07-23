using System;
using UnityEngine;

// Token: 0x0200034F RID: 847
public class WallUpgrader : PlotUpgrader
{
	// Token: 0x060011BC RID: 4540 RVA: 0x00046F23 File Offset: 0x00045123
	public override void Apply(LandPlot.Upgrade upgrade)
	{
		if (upgrade == LandPlot.Upgrade.WALLS)
		{
			this.standardWalls.SetActive(false);
			this.upgradeWalls.SetActive(true);
		}
	}

	// Token: 0x040010F5 RID: 4341
	public GameObject standardWalls;

	// Token: 0x040010F6 RID: 4342
	public GameObject upgradeWalls;
}
