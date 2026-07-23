using System;
using UnityEngine;

// Token: 0x020007CF RID: 1999
public class ZoneActivator : MonoBehaviour, TechActivator
{
	// Token: 0x060029DA RID: 10714 RVA: 0x0009D1BF File Offset: 0x0009B3BF
	public void Activate()
	{
		this.zone.SetOperating(!this.zone.GetOperating());
	}

	// Token: 0x060029DB RID: 10715 RVA: 0x00025E60 File Offset: 0x00024060
	public GameObject GetCustomGuiPrefab()
	{
		return null;
	}

	// Token: 0x04002906 RID: 10502
	public ReverseGravityZone zone;
}
