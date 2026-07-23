using System;
using UnityEngine;

// Token: 0x02000007 RID: 7
public class BeatrixRandom : StateMachineBehaviour
{
	// Token: 0x06000027 RID: 39 RVA: 0x00002A88 File Offset: 0x00000C88
	public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
	{
		animator.SetInteger("IdleIndex", UnityEngine.Random.Range(0, 8));
	}
}
