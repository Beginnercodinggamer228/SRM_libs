using System;
using UnityEngine;

// Token: 0x0200029C RID: 668
public class FilteredTrackCollisions : TrackCollisions
{
	// Token: 0x06000E15 RID: 3605 RVA: 0x00039293 File Offset: 0x00037493
	public void SetFilter(Predicate<GameObject> filter)
	{
		this.filter = filter;
	}

	// Token: 0x06000E16 RID: 3606 RVA: 0x0003929C File Offset: 0x0003749C
	protected override void OnTriggerEnter(Collider other)
	{
		if (this.filter == null || this.filter(other.gameObject))
		{
			base.OnTriggerEnter(other);
		}
	}

	// Token: 0x06000E17 RID: 3607 RVA: 0x000392C0 File Offset: 0x000374C0
	protected override void OnTriggerExit(Collider other)
	{
		if (this.filter == null || this.filter(other.gameObject))
		{
			base.OnTriggerExit(other);
		}
	}

	// Token: 0x04000D44 RID: 3396
	private Predicate<GameObject> filter;
}
