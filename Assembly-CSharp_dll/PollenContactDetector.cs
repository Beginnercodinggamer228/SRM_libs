using System;
using UnityEngine;

// Token: 0x02000420 RID: 1056
public class PollenContactDetector : MonoBehaviour
{
	// Token: 0x06001600 RID: 5632 RVA: 0x000555BA File Offset: 0x000537BA
	public void Awake()
	{
		this.destructor = base.GetComponentInParent<PollenCloudDestructor>();
	}

	// Token: 0x06001601 RID: 5633 RVA: 0x000555C8 File Offset: 0x000537C8
	public void OnTriggerEnter(Collider col)
	{
		if (!col.isTrigger && col.GetComponent<Rigidbody>() == null)
		{
			this.destructor.AddContact();
		}
	}

	// Token: 0x06001602 RID: 5634 RVA: 0x000555EB File Offset: 0x000537EB
	public void OnTriggerExit(Collider col)
	{
		if (!col.isTrigger && col.GetComponent<Rigidbody>() == null)
		{
			this.destructor.RemoveContact();
		}
	}

	// Token: 0x040014F8 RID: 5368
	private PollenCloudDestructor destructor;
}
