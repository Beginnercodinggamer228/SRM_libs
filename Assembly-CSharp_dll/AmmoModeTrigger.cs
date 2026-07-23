using System;
using UnityEngine;

// Token: 0x020000D4 RID: 212
public class AmmoModeTrigger : MonoBehaviour
{
	// Token: 0x060004C0 RID: 1216 RVA: 0x0001E7D6 File Offset: 0x0001C9D6
	public void Awake()
	{
		this.playerState = SRSingleton<SceneContext>.Instance.PlayerState;
	}

	// Token: 0x060004C1 RID: 1217 RVA: 0x0001E7E8 File Offset: 0x0001C9E8
	public void OnTriggerEnter(Collider collider)
	{
		if (PhysicsUtil.IsPlayerMainCollider(collider))
		{
			this.playerState.SetAmmoMode(this.onEnter);
		}
	}

	// Token: 0x060004C2 RID: 1218 RVA: 0x0001E803 File Offset: 0x0001CA03
	public void OnTriggerExit(Collider collider)
	{
		if (PhysicsUtil.IsPlayerMainCollider(collider))
		{
			this.playerState.SetAmmoMode(this.onExit);
		}
	}

	// Token: 0x04000502 RID: 1282
	[Tooltip("Ammo mode to set on entering the trigger.")]
	public PlayerState.AmmoMode onEnter;

	// Token: 0x04000503 RID: 1283
	[Tooltip("Ammo mode to set on exiting the trigger.")]
	public PlayerState.AmmoMode onExit;

	// Token: 0x04000504 RID: 1284
	private PlayerState playerState;
}
