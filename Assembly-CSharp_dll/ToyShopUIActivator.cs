using System;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x02000631 RID: 1585
public class ToyShopUIActivator : UIActivator
{
	// Token: 0x0600213E RID: 8510 RVA: 0x0007F28F File Offset: 0x0007D48F
	public override GameObject Activate()
	{
		GameObject gameObject = base.Activate();
		ToyShopUI component = gameObject.GetComponent<ToyShopUI>();
		component.ejectionPoint = this.ejector;
		component.regionSetId = base.GetComponentInParent<Region>().setId;
		return gameObject;
	}

	// Token: 0x04002096 RID: 8342
	public GameObject ejector;
}
