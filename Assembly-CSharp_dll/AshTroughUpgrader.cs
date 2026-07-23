using System;
using UnityEngine;

// Token: 0x020002F5 RID: 757
public class AshTroughUpgrader : PlotUpgrader
{
	// Token: 0x0600102C RID: 4140 RVA: 0x00040F30 File Offset: 0x0003F130
	public override void Apply(LandPlot.Upgrade upgrade)
	{
		if (upgrade == LandPlot.Upgrade.ASH_TROUGH)
		{
			this.ashTrough.SetActive(true);
		}
	}

	// Token: 0x04000F03 RID: 3843
	public GameObject ashTrough;
}
