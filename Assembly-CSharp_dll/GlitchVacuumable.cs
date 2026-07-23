using System;
using UnityEngine;

// Token: 0x02000516 RID: 1302
public class GlitchVacuumable : Vacuumable
{
	// Token: 0x06001B2B RID: 6955 RVA: 0x00068632 File Offset: 0x00066832
	protected override void SetCaptive(Joint toJoint)
	{
		base.SetCaptive(toJoint);
		if (base.isCaptive())
		{
			this.body.velocity = Vector3.zero;
			this.body.angularVelocity = Vector3.zero;
		}
	}
}
