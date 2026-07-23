using System;
using UnityEngine;

// Token: 0x02000347 RID: 839
public class SolarShieldUpgrader : PlotUpgrader
{
	// Token: 0x060011A5 RID: 4517 RVA: 0x00046C70 File Offset: 0x00044E70
	public override void Apply(LandPlot.Upgrade upgrade)
	{
		if (upgrade == LandPlot.Upgrade.SOLAR_SHIELD)
		{
			GameObject[] array = this.shields;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(true);
			}
		}
	}

	// Token: 0x040010E1 RID: 4321
	[Tooltip("All the solar shield objects we need to activate/deactivate")]
	public GameObject[] shields;
}
