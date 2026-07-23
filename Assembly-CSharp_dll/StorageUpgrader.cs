using System;
using UnityEngine;

// Token: 0x02000349 RID: 841
public class StorageUpgrader : PlotUpgrader
{
	// Token: 0x060011A9 RID: 4521 RVA: 0x00046CB4 File Offset: 0x00044EB4
	public override void Apply(LandPlot.Upgrade upgrade)
	{
		if (upgrade == LandPlot.Upgrade.STORAGE2)
		{
			this.storageAdd2.SetActive(true);
			this.storageOnly1.SetActive(false);
			this.storageOnly2.SetActive(true);
			this.storageOnly3And4.SetActive(false);
			return;
		}
		if (upgrade == LandPlot.Upgrade.STORAGE3)
		{
			this.storageAdd3.SetActive(true);
			this.storageOnly1.SetActive(false);
			this.storageOnly2.SetActive(false);
			this.storageOnly3And4.SetActive(true);
			return;
		}
		if (upgrade == LandPlot.Upgrade.STORAGE4)
		{
			this.storageAdd4.SetActive(true);
			this.storageOnly1.SetActive(false);
			this.storageOnly2.SetActive(false);
			this.storageOnly3And4.SetActive(true);
		}
	}

	// Token: 0x040010E3 RID: 4323
	public GameObject storageAdd2;

	// Token: 0x040010E4 RID: 4324
	public GameObject storageAdd3;

	// Token: 0x040010E5 RID: 4325
	public GameObject storageAdd4;

	// Token: 0x040010E6 RID: 4326
	public GameObject storageOnly1;

	// Token: 0x040010E7 RID: 4327
	public GameObject storageOnly2;

	// Token: 0x040010E8 RID: 4328
	public GameObject storageOnly3And4;
}
