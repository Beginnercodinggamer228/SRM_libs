using System;
using UnityEngine;

// Token: 0x020002FE RID: 766
public class DeluxeGardenUpgrader : PlotUpgrader
{
	// Token: 0x06001066 RID: 4198 RVA: 0x000417CC File Offset: 0x0003F9CC
	public override void Apply(LandPlot.Upgrade upgrade)
	{
		if (upgrade == LandPlot.Upgrade.DELUXE_GARDEN && this.deluxeStuff != null && !this.deluxeStuff.activeSelf)
		{
			this.deluxeStuff.SetActive(true);
			Identifiable.Id attachedCropId = base.GetComponent<LandPlot>().GetAttachedCropId();
			if (attachedCropId != Identifiable.Id.NONE)
			{
				SpawnResource componentInChildren = base.GetComponentInChildren<SpawnResource>();
				Destroyer.Destroy(componentInChildren.gameObject, "DeluxeGardenUpgrader.Apply");
				base.GetComponentInChildren<GardenCatcher>().Plant(attachedCropId, true).GetComponent<SpawnResource>().InitAsReplacement(componentInChildren);
			}
		}
	}

	// Token: 0x04000F26 RID: 3878
	public GameObject deluxeStuff;
}
