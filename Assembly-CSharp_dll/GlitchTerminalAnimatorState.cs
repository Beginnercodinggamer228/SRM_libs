using System;
using UnityEngine;

// Token: 0x02000505 RID: 1285
public class GlitchTerminalAnimatorState : SRAnimatorState<GlitchTerminalAnimator>
{
	// Token: 0x06001AF2 RID: 6898 RVA: 0x00067F35 File Offset: 0x00066135
	public override void OnStateEnter(Animator animator, AnimatorStateInfo state, int layerIndex)
	{
		base.OnStateEnter(animator, state, layerIndex);
		base.GetAnimatorWrapper(animator).OnStateEnter(this.id);
	}

	// Token: 0x04001A60 RID: 6752
	[Tooltip("State identifier.")]
	public GlitchTerminalAnimatorState.Id id;

	// Token: 0x02000506 RID: 1286
	public enum Id
	{
		// Token: 0x04001A62 RID: 6754
		NONE,
		// Token: 0x04001A63 RID: 6755
		SLEEP,
		// Token: 0x04001A64 RID: 6756
		BOOT_UP,
		// Token: 0x04001A65 RID: 6757
		IDLE
	}
}
