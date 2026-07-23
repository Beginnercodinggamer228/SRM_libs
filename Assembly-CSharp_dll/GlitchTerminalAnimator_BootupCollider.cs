using System;
using UnityEngine;

// Token: 0x02000507 RID: 1287
public class GlitchTerminalAnimator_BootupCollider : SRBehaviour
{
	// Token: 0x06001AF4 RID: 6900 RVA: 0x00067F5A File Offset: 0x0006615A
	public void Awake()
	{
		this.animator = base.GetRequiredComponentInParent<GlitchTerminalAnimator>(false);
	}

	// Token: 0x06001AF5 RID: 6901 RVA: 0x00067F6C File Offset: 0x0006616C
	public void OnTriggerEnter(Collider collider)
	{
		if (PhysicsUtil.IsPlayerMainCollider(collider) && this.animator.activator.GetLinkState() > GlitchTerminalActivator.LinkState.INACTIVE_PROGRESS)
		{
			this.animator.animator.SetBool("state_sleeping", false);
			Destroyer.Destroy(base.gameObject, "GlitchTerminalAnimator_BootupCollider.OnTriggerEnter");
		}
	}

	// Token: 0x04001A66 RID: 6758
	private GlitchTerminalAnimator animator;
}
