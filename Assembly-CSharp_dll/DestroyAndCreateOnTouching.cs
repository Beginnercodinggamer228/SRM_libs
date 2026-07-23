using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200011C RID: 284
public class DestroyAndCreateOnTouching : SRBehaviour
{
	// Token: 0x0600061C RID: 1564 RVA: 0x00022340 File Offset: 0x00020540
	public void OnCollisionEnter(Collision col)
	{
		if (!this.hasCollided)
		{
			SRBehaviour.InstantiateDynamic(this.prefab, base.transform.position, base.transform.rotation, false);
			base.StartCoroutine(this.DestroyAfterFrame());
			this.hasCollided = true;
		}
	}

	// Token: 0x0600061D RID: 1565 RVA: 0x0002238C File Offset: 0x0002058C
	private IEnumerator DestroyAfterFrame()
	{
		yield return new WaitForEndOfFrame();
		Destroyer.DestroyActor(base.gameObject, "DestroyAndCreateOnTouching.DestroyAfterFrame", false);
		yield break;
	}

	// Token: 0x040005E4 RID: 1508
	[Tooltip("Prefab to instantiate.")]
	public GameObject prefab;

	// Token: 0x040005E5 RID: 1509
	private bool hasCollided;
}
