using System;
using UnityEngine;

// Token: 0x0200040E RID: 1038
public class MosaicAttractor : Attractor
{
	// Token: 0x060015B5 RID: 5557 RVA: 0x00054720 File Offset: 0x00052920
	public void Start()
	{
		base.SetAweFactor(1f);
	}

	// Token: 0x060015B6 RID: 5558 RVA: 0x0005472D File Offset: 0x0005292D
	public override void OnTriggerEnter(Collider col)
	{
		if (col.GetComponentInChildren<MosaicAttractor>() == null)
		{
			base.OnTriggerEnter(col);
		}
	}

	// Token: 0x060015B7 RID: 5559 RVA: 0x00013CC5 File Offset: 0x00011EC5
	public override bool CauseMoveTowards()
	{
		return true;
	}
}
