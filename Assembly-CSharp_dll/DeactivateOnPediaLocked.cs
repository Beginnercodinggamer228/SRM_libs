using System;
using UnityEngine;

// Token: 0x0200010E RID: 270
public class DeactivateOnPediaLocked : SRBehaviour
{
	// Token: 0x060005F3 RID: 1523 RVA: 0x00021ECE File Offset: 0x000200CE
	public void OnEnable()
	{
		if (!SRSingleton<SceneContext>.Instance.PediaDirector.IsUnlocked(this.id))
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x040005B9 RID: 1465
	[Tooltip("Pedia entry that is required to be unlocked.")]
	public PediaDirector.Id id;
}
