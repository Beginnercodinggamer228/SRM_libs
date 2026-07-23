using System;
using System.Collections.Generic;

// Token: 0x02000706 RID: 1798
public class FirestormController : SRBehaviour
{
	// Token: 0x0600258B RID: 9611 RVA: 0x000904E6 File Offset: 0x0008E6E6
	public void Awake()
	{
		this.columns = base.GetComponentsInChildren<FireColumn>(true);
	}

	// Token: 0x0600258C RID: 9612 RVA: 0x000904F5 File Offset: 0x0008E6F5
	public void AddColumnsToList(List<FireColumn> nearbyColumns)
	{
		nearbyColumns.AddRange(this.columns);
	}

	// Token: 0x0400248A RID: 9354
	private FireColumn[] columns;
}
