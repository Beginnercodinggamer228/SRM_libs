using System;
using UnityEngine;

// Token: 0x020003EB RID: 1003
public class GordoEatTrigger : MonoBehaviour
{
	// Token: 0x06001500 RID: 5376 RVA: 0x00051AFC File Offset: 0x0004FCFC
	public void Awake()
	{
		this.eat = base.GetComponentInParent<GordoEat>();
	}

	// Token: 0x06001501 RID: 5377 RVA: 0x00051B0A File Offset: 0x0004FD0A
	public void OnTriggerEnter(Collider col)
	{
		if (!col.isTrigger)
		{
			this.eat.MaybeEat(col);
		}
	}

	// Token: 0x040013D7 RID: 5079
	private GordoEat eat;
}
