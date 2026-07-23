using System;
using UnityEngine;

// Token: 0x02000512 RID: 1298
public class GlitchTerminalAnimator_PlayerState : SRAnimatorState<GlitchTerminalAnimator_Player>
{
	// Token: 0x06001B25 RID: 6949 RVA: 0x0006852A File Offset: 0x0006672A
	public override void OnStateExit(Animator animator, AnimatorStateInfo state, int layerIndex)
	{
		base.OnStateExit(animator, state, layerIndex);
		base.GetAnimatorWrapper(animator).OnStateExit(this.id);
	}

	// Token: 0x04001A8B RID: 6795
	[Tooltip("State identifier.")]
	public GlitchTerminalAnimator_PlayerState.Id id;

	// Token: 0x02000513 RID: 1299
	public enum Id
	{
		// Token: 0x04001A8D RID: 6797
		NONE,
		// Token: 0x04001A8E RID: 6798
		ENTERING,
		// Token: 0x04001A8F RID: 6799
		EXITING
	}
}
