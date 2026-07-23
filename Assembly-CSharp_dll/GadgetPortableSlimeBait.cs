using System;
using UnityEngine;

// Token: 0x020001F2 RID: 498
public class GadgetPortableSlimeBait : SRBehaviour
{
	// Token: 0x06000A6F RID: 2671 RVA: 0x0002D249 File Offset: 0x0002B449
	public void OnHit(Transform onHitTransform)
	{
		if (Time.time >= this.nextHitTime)
		{
			SECTR_AudioSystem.Play(this.onHitCue, onHitTransform.position, false);
			this.nextHitTime = Time.time + 1f;
		}
	}

	// Token: 0x0400088A RID: 2186
	[Tooltip("SFX played when the slime bait is hit.")]
	public SECTR_AudioCue onHitCue;

	// Token: 0x0400088B RID: 2187
	private float nextHitTime;
}
