using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006D7 RID: 1751
public class ClearAreaFeralsOnHit : MonoBehaviour
{
	// Token: 0x06002490 RID: 9360 RVA: 0x0008D05B File Offset: 0x0008B25B
	public void Awake()
	{
		this.waiter = base.GetComponentInParent<WaitForChargeup>();
	}

	// Token: 0x06002491 RID: 9361 RVA: 0x0008D069 File Offset: 0x0008B269
	public void OnCollisionEnter(Collision col)
	{
		this.MaybeHandleCollision();
	}

	// Token: 0x06002492 RID: 9362 RVA: 0x0008D069 File Offset: 0x0008B269
	public void OnControllerCollision(GameObject gameObj)
	{
		this.MaybeHandleCollision();
	}

	// Token: 0x06002493 RID: 9363 RVA: 0x0008D071 File Offset: 0x0008B271
	private void MaybeHandleCollision()
	{
		if (this.waiter.IsWaiting())
		{
			return;
		}
		if (Time.time >= this.nextTime)
		{
			this.HandleCollision();
			this.nextTime = Time.time + this.minTimeBetween;
		}
	}

	// Token: 0x06002494 RID: 9364 RVA: 0x0008D0A8 File Offset: 0x0008B2A8
	private void HandleCollision()
	{
		SphereOverlapTrigger.CreateGameObject(base.transform.position, this.radius, delegate(IEnumerable<Collider> colliders)
		{
			foreach (Collider collider in colliders)
			{
				SlimeFeral component = collider.GetComponent<SlimeFeral>();
				if (component != null)
				{
					component.ClearFeral(true);
				}
			}
		}, 0);
		if (this.hitCue != null)
		{
			SECTR_AudioSystem.Play(this.hitCue, base.transform.position, false);
		}
	}

	// Token: 0x06002495 RID: 9365 RVA: 0x0008D112 File Offset: 0x0008B312
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere(base.transform.position, this.radius);
	}

	// Token: 0x04002387 RID: 9095
	public float radius = 20f;

	// Token: 0x04002388 RID: 9096
	public float minTimeBetween = 1f;

	// Token: 0x04002389 RID: 9097
	public SECTR_AudioCue hitCue;

	// Token: 0x0400238A RID: 9098
	private float nextTime;

	// Token: 0x0400238B RID: 9099
	private WaitForChargeup waiter;
}
