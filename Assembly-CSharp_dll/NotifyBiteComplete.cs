using System;
using UnityEngine;

// Token: 0x02000410 RID: 1040
public class NotifyBiteComplete : MonoBehaviour
{
	// Token: 0x060015BE RID: 5566 RVA: 0x000548AC File Offset: 0x00052AAC
	public void Awake()
	{
		this.chomper = base.GetComponentInParent<Chomper>();
	}

	// Token: 0x060015BF RID: 5567 RVA: 0x000548BA File Offset: 0x00052ABA
	public void DisableBiteModel()
	{
		this.chomper.BiteComplete();
	}

	// Token: 0x040014BA RID: 5306
	private Chomper chomper;
}
