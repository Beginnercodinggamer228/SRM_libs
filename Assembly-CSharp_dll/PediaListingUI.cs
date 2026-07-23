using System;
using UnityEngine;

// Token: 0x020005DA RID: 1498
public class PediaListingUI : MonoBehaviour
{
	// Token: 0x06001F75 RID: 8053 RVA: 0x000778D0 File Offset: 0x00075AD0
	public void Start()
	{
		this.ui = base.GetComponentInParent<PediaUI>();
	}

	// Token: 0x06001F76 RID: 8054 RVA: 0x000778DE File Offset: 0x00075ADE
	public void OnClick()
	{
		this.ui.SelectEntry(this.id, false, this.id);
	}

	// Token: 0x04001E9E RID: 7838
	public PediaDirector.Id id;

	// Token: 0x04001E9F RID: 7839
	private PediaUI ui;
}
