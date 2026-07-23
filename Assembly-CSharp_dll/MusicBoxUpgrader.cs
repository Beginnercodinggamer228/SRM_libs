using System;
using UnityEngine;

// Token: 0x02000326 RID: 806
public class MusicBoxUpgrader : PlotUpgrader
{
	// Token: 0x0600110A RID: 4362 RVA: 0x000441B4 File Offset: 0x000423B4
	public override void Apply(LandPlot.Upgrade upgrade)
	{
		if (upgrade == LandPlot.Upgrade.MUSIC_BOX)
		{
			this.musicBox.SetActive(true);
		}
	}

	// Token: 0x04000FF7 RID: 4087
	public GameObject musicBox;
}
