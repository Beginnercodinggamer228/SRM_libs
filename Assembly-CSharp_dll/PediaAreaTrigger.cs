using System;
using UnityEngine;

// Token: 0x02000740 RID: 1856
public class PediaAreaTrigger : SRBehaviour
{
	// Token: 0x060026CF RID: 9935 RVA: 0x00093EF7 File Offset: 0x000920F7
	public void OnTriggerEnter(Collider col)
	{
		if (col.gameObject == SRSingleton<SceneContext>.Instance.Player && col is CharacterController)
		{
			SRSingleton<SceneContext>.Instance.PediaDirector.MaybeShowPopup(this.pediaId);
		}
	}

	// Token: 0x04002600 RID: 9728
	public PediaDirector.Id pediaId;
}
