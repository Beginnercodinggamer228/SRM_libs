using System;
using UnityEngine;

// Token: 0x020001DA RID: 474
public class EchoNoteGordoAnimatorState : SRAnimatorState<EchoNoteGordoAnimator>
{
	// Token: 0x060009FD RID: 2557 RVA: 0x0002C1C9 File Offset: 0x0002A3C9
	public override void OnStateEnter(Animator animator, AnimatorStateInfo state, int layerIndex)
	{
		base.OnStateEnter(animator, state, layerIndex);
		base.GetAnimatorWrapper(animator).parent.OnAnimationEvent_StateEnter(this.id);
	}

	// Token: 0x060009FE RID: 2558 RVA: 0x0002C1EB File Offset: 0x0002A3EB
	public override void OnStateExit(Animator animator, AnimatorStateInfo state, int layerIndex)
	{
		base.OnStateExit(animator, state, layerIndex);
		base.GetAnimatorWrapper(animator).parent.OnAnimationEvent_StateExit(this.id);
	}

	// Token: 0x04000843 RID: 2115
	[Tooltip("Animation state identifier.")]
	public EchoNoteGordoAnimatorState.Id id;

	// Token: 0x020001DB RID: 475
	public enum Id
	{
		// Token: 0x04000845 RID: 2117
		NONE,
		// Token: 0x04000846 RID: 2118
		PRE_ACTIVATION,
		// Token: 0x04000847 RID: 2119
		ACTIVATION
	}
}
