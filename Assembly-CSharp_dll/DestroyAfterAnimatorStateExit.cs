using System;
using UnityEngine;

// Token: 0x02000115 RID: 277
public class DestroyAfterAnimatorStateExit : StateMachineBehaviour
{
	// Token: 0x060005FB RID: 1531 RVA: 0x00021F5D File Offset: 0x0002015D
	public override void OnStateExit(Animator animator, AnimatorStateInfo state, int layerIndex)
	{
		base.OnStateExit(animator, state, layerIndex);
		Destroyer.Destroy(animator.gameObject, "DestroyAfterAnimatorStateExit.OnStateExit");
	}
}
