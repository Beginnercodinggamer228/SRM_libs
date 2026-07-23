using System;
using UnityEngine;

// Token: 0x020002FD RID: 765
public class DeluxeCoopUpgrader : PlotUpgrader
{
	// Token: 0x06001064 RID: 4196 RVA: 0x00041774 File Offset: 0x0003F974
	public override void Apply(LandPlot.Upgrade upgrade)
	{
		if (upgrade == LandPlot.Upgrade.DELUXE_COOP)
		{
			if (this.deluxeStuff != null && !this.deluxeStuff.activeSelf)
			{
				this.deluxeStuff.SetActive(true);
			}
			CoopRegion[] array = this.coopRegions;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetDeluxe();
			}
		}
	}

	// Token: 0x04000F24 RID: 3876
	public GameObject deluxeStuff;

	// Token: 0x04000F25 RID: 3877
	public CoopRegion[] coopRegions;
}
