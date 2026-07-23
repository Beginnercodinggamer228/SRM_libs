using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000323 RID: 803
public class MiracleMix : MonoBehaviour
{
	// Token: 0x060010FD RID: 4349 RVA: 0x00043F2C File Offset: 0x0004212C
	public void OnTriggerEnter(Collider other)
	{
		if (other.isTrigger)
		{
			return;
		}
		Identifiable component = other.GetComponent<Identifiable>();
		if (component != null && this.IsPreservable(component))
		{
			ResourceCycle component2 = other.GetComponent<ResourceCycle>();
			this.preservedResources.Add(component2);
			component2.AttachPreservative(this);
		}
	}

	// Token: 0x060010FE RID: 4350 RVA: 0x00043F76 File Offset: 0x00042176
	public bool IsPreservable(Identifiable ident)
	{
		return Identifiable.VEGGIE_CLASS.Contains(ident.id) || Identifiable.FRUIT_CLASS.Contains(ident.id);
	}

	// Token: 0x060010FF RID: 4351 RVA: 0x00043F9C File Offset: 0x0004219C
	public void OnTriggerExit(Collider other)
	{
		if (other.isTrigger)
		{
			return;
		}
		Identifiable component = other.GetComponent<Identifiable>();
		if (component != null && this.IsPreservable(component))
		{
			ResourceCycle component2 = other.GetComponent<ResourceCycle>();
			this.RemoveResourceCycle(component2);
		}
	}

	// Token: 0x06001100 RID: 4352 RVA: 0x00043FD9 File Offset: 0x000421D9
	public void RemoveResourceCycle(ResourceCycle cycle)
	{
		this.preservedResources.Remove(cycle);
		cycle.DetachPreservative(this);
	}

	// Token: 0x06001101 RID: 4353 RVA: 0x00043FF0 File Offset: 0x000421F0
	public void OnDestroy()
	{
		foreach (ResourceCycle resourceCycle in this.preservedResources)
		{
			if (resourceCycle != null)
			{
				resourceCycle.DetachPreservative(this);
			}
		}
		this.preservedResources.Clear();
	}

	// Token: 0x06001102 RID: 4354 RVA: 0x00044058 File Offset: 0x00042258
	public float PreservativeRipenessModifier()
	{
		return this.ripenessModifier;
	}

	// Token: 0x04000FF0 RID: 4080
	public float ripenessModifier = -0.5f;

	// Token: 0x04000FF1 RID: 4081
	private HashSet<ResourceCycle> preservedResources = new HashSet<ResourceCycle>();
}
