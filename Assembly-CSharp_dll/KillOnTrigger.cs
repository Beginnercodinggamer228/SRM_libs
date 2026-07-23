using System;
using UnityEngine;

// Token: 0x02000223 RID: 547
public class KillOnTrigger : SRBehaviour
{
	// Token: 0x06000BC2 RID: 3010 RVA: 0x000314BC File Offset: 0x0002F6BC
	private void OnTriggerEnter(Collider collider)
	{
		Rigidbody component = collider.GetComponent<Rigidbody>();
		if (!collider.isTrigger && component != null && (!component.isKinematic || PhysicsUtil.IsPlayerMainCollider(collider)))
		{
			Debug.Log("Fallthrough destroying: " + collider.gameObject.name);
			DeathHandler.Kill(collider.gameObject, DeathHandler.Source.KILL_ON_TRIGGER, base.gameObject, "KillOnTrigger.OnTriggerEnter");
			if (PhysicsUtil.IsPlayerMainCollider(collider) && this.playerKillFx != null)
			{
				SRBehaviour.SpawnAndPlayFX(this.playerKillFx, collider.gameObject.transform.position, Quaternion.identity);
				return;
			}
			if (this.killFX != null)
			{
				SRBehaviour.SpawnAndPlayFX(this.killFX, collider.gameObject.transform.position, Quaternion.identity);
			}
		}
	}

	// Token: 0x04000AB0 RID: 2736
	public GameObject killFX;

	// Token: 0x04000AB1 RID: 2737
	public GameObject playerKillFx;
}
