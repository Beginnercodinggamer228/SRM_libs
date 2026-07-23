using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020005F9 RID: 1529
public class PurchaseUI_LoadingSlime : SRBehaviour
{
	// Token: 0x06002012 RID: 8210 RVA: 0x0007A74E File Offset: 0x0007894E
	public void Awake()
	{
		base.GetRequiredComponent<Image>().sprite = Randoms.SHARED.Pick<Sprite>(this.icons);
	}

	// Token: 0x04001F5E RID: 8030
	[Tooltip("List of potential loading slime icons.")]
	public Sprite[] icons;
}
