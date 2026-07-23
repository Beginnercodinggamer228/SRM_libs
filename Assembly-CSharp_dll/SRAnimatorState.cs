using System;
using UnityEngine;

// Token: 0x02000366 RID: 870
public abstract class SRAnimatorState<T> : StateMachineBehaviour where T : SRAnimator
{
	// Token: 0x06001205 RID: 4613 RVA: 0x00047A29 File Offset: 0x00045C29
	protected T GetAnimatorWrapper(Animator animator)
	{
		if (this.wrapper == null)
		{
			this.wrapper = animator.gameObject.GetComponent<T>();
		}
		return this.wrapper;
	}

	// Token: 0x0400111F RID: 4383
	private T wrapper;
}
