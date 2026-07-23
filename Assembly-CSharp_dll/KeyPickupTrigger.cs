using System;
using UnityEngine;

// Token: 0x02000726 RID: 1830
public class KeyPickupTrigger : SRBehaviour
{
	// Token: 0x06002636 RID: 9782 RVA: 0x0009252C File Offset: 0x0009072C
	public void OnTriggerEnter(Collider col)
	{
		if (col.gameObject == SRSingleton<SceneContext>.Instance.Player)
		{
			SRSingleton<SceneContext>.Instance.PlayerState.AddKey();
			if (this.pickupFX != null)
			{
				SRBehaviour.SpawnAndPlayFX(this.pickupFX, base.transform.position, base.transform.rotation);
			}
			Destroyer.DestroyActor(base.gameObject, "KeyPickupTrigger.OnTriggerEnter", false);
		}
	}

	// Token: 0x0400258C RID: 9612
	public GameObject pickupFX;
}
