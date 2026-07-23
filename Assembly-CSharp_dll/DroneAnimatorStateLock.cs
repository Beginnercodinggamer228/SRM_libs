using System;
using UnityEngine;

// Token: 0x0200012F RID: 303
public class DroneAnimatorStateLock : StateMachineBehaviour
{
	// Token: 0x06000696 RID: 1686 RVA: 0x0002324C File Offset: 0x0002144C
	public void Awake()
	{
		this.parameter = Animator.StringToHash("TRANSITION_LOCK");
	}

	// Token: 0x06000697 RID: 1687 RVA: 0x0002325E File Offset: 0x0002145E
	public override void OnStateEnter(Animator animator, AnimatorStateInfo state, int layerIndex)
	{
		animator.SetBool(this.parameter, false);
	}

	// Token: 0x06000698 RID: 1688 RVA: 0x0002326D File Offset: 0x0002146D
	public override void OnStateExit(Animator animator, AnimatorStateInfo state, int layerIndex)
	{
		animator.SetBool(this.parameter, true);
	}

	// Token: 0x0400062C RID: 1580
	private int parameter;
}
