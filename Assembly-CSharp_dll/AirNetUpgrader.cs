using System;
using UnityEngine;

// Token: 0x020002F4 RID: 756
public class AirNetUpgrader : PlotUpgrader
{
	// Token: 0x0600102A RID: 4138 RVA: 0x00040EF8 File Offset: 0x0003F0F8
	public override void Apply(LandPlot.Upgrade upgrade)
	{
		if (upgrade == LandPlot.Upgrade.AIR_NET)
		{
			GameObject[] array = this.airNets;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(true);
			}
		}
	}

	// Token: 0x04000F02 RID: 3842
	[Tooltip("All the air net objects we need to activate/deactivate")]
	public GameObject[] airNets;
}
