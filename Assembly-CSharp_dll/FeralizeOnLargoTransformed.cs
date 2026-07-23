using System;
using UnityEngine;

// Token: 0x020003D4 RID: 980
public class FeralizeOnLargoTransformed : MonoBehaviour, OnTransformed
{
	// Token: 0x06001458 RID: 5208 RVA: 0x0004EA98 File Offset: 0x0004CC98
	public void OnTransformed()
	{
		Vacuumable component = base.GetComponent<Vacuumable>();
		SlimeFeral component2 = base.GetComponent<SlimeFeral>();
		if (component2 != null && component != null && component.size != Vacuumable.Size.NORMAL)
		{
			component2.SetFeral();
		}
	}
}
