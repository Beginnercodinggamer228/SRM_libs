using System;
using UnityEngine;

// Token: 0x020004D6 RID: 1238
public class GlitchBreadcrumbNetworkPather : Pather
{
	// Token: 0x060019E4 RID: 6628 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
	protected override bool PathPredicate(Vector3 start, Vector3 end)
	{
		return false;
	}

	// Token: 0x060019E5 RID: 6629 RVA: 0x00013CC5 File Offset: 0x00011EC5
	protected override bool NearestAccessibleNodePredicate(Vector3 start, Vector3 end)
	{
		return true;
	}
}
