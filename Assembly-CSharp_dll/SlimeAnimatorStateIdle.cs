using System;
using UnityEngine;

// Token: 0x020004BE RID: 1214
public class SlimeAnimatorStateIdle : StateMachineBehaviour
{
	// Token: 0x170001DE RID: 478
	// (get) Token: 0x06001969 RID: 6505 RVA: 0x00062EDC File Offset: 0x000610DC
	public bool IsCurrentState
	{
		get
		{
			bool? flag = this.isCurrentState;
			bool flag2 = true;
			return flag.GetValueOrDefault() == flag2 & flag != null;
		}
	}

	// Token: 0x170001DF RID: 479
	// (get) Token: 0x0600196A RID: 6506 RVA: 0x00062F04 File Offset: 0x00061104
	public bool IsInitialized
	{
		get
		{
			return this.isCurrentState != null;
		}
	}

	// Token: 0x0600196B RID: 6507 RVA: 0x00062F11 File Offset: 0x00061111
	public override void OnStateEnter(Animator animator, AnimatorStateInfo state, int layerIndex)
	{
		base.OnStateEnter(animator, state, layerIndex);
		this.isCurrentState = new bool?(true);
	}

	// Token: 0x0600196C RID: 6508 RVA: 0x00062F28 File Offset: 0x00061128
	public override void OnStateExit(Animator animator, AnimatorStateInfo state, int layerIndex)
	{
		base.OnStateExit(animator, state, layerIndex);
		this.isCurrentState = new bool?(false);
	}

	// Token: 0x04001914 RID: 6420
	private bool? isCurrentState;
}
