using System;
using UnityEngine;

// Token: 0x02000635 RID: 1589
public class UIActivator : MonoBehaviour
{
	// Token: 0x06002159 RID: 8537 RVA: 0x00013CC5 File Offset: 0x00011EC5
	public virtual bool CanActivate()
	{
		return true;
	}

	// Token: 0x0600215A RID: 8538 RVA: 0x0007F7D4 File Offset: 0x0007D9D4
	public virtual GameObject Activate()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.uiPrefab);
		LandPlotUI component = gameObject.GetComponent<LandPlotUI>();
		if (component != null)
		{
			component.SetActivator(base.gameObject.GetComponentInParent<LandPlot>());
		}
		AccessDoorUI component2 = gameObject.GetComponent<AccessDoorUI>();
		if (component2 != null)
		{
			component2.SetAccessDoor(base.gameObject.GetComponentInParent<AccessDoor>());
		}
		LocationalUI component3 = gameObject.GetComponent<LocationalUI>();
		if (component3 != null)
		{
			component3.SetPosition(base.transform.position);
		}
		return gameObject;
	}

	// Token: 0x040020AD RID: 8365
	public GameObject uiPrefab;

	// Token: 0x040020AE RID: 8366
	public GameObject blockInExpoPrefab;
}
