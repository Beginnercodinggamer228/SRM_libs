using System;
using System.Collections.Generic;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x020006CD RID: 1741
public abstract class BreakOnImpactBase : SRBehaviour
{
	// Token: 0x0600243C RID: 9276 RVA: 0x0008BC9F File Offset: 0x00089E9F
	public virtual void Awake()
	{
		this.body = base.GetComponent<Rigidbody>();
	}

	// Token: 0x0600243D RID: 9277 RVA: 0x0008BCB0 File Offset: 0x00089EB0
	public void OnCollisionEnter(Collision col)
	{
		if (!col.collider.isTrigger && !this.body.isKinematic)
		{
			float num = 0f;
			foreach (ContactPoint contactPoint in col.contacts)
			{
				num = Mathf.Max(num, Vector3.Dot(contactPoint.normal, col.relativeVelocity));
			}
			if (num > 14f)
			{
				this.BreakOpen();
			}
		}
	}

	// Token: 0x0600243E RID: 9278 RVA: 0x0008BD24 File Offset: 0x00089F24
	private void BreakOpen()
	{
		if (this.breaking)
		{
			return;
		}
		this.breaking = true;
		SRBehaviour.SpawnAndPlayFX(this.breakFX, base.gameObject.transform.position, base.gameObject.transform.rotation);
		Destroyer.DestroyActor(base.gameObject, "BreakOnImpact.BreakOpen", false);
		RegionRegistry.RegionSetId setId = base.GetComponent<RegionMember>().setId;
		foreach (GameObject original in this.GetRewardPrefabs())
		{
			Vector3 position = base.transform.position + UnityEngine.Random.insideUnitSphere;
			Rigidbody component = SRBehaviour.InstantiateActor(original, setId, position, Quaternion.identity, true).GetComponent<Rigidbody>();
			if (component != null)
			{
				component.AddTorque(UnityEngine.Random.insideUnitSphere);
				component.AddForce(UnityEngine.Random.insideUnitSphere);
			}
		}
	}

	// Token: 0x0600243F RID: 9279
	protected abstract IEnumerable<GameObject> GetRewardPrefabs();

	// Token: 0x0400234A RID: 9034
	public GameObject breakFX;

	// Token: 0x0400234B RID: 9035
	private const float COLLISION_THRESHOLD = 14f;

	// Token: 0x0400234C RID: 9036
	private Rigidbody body;

	// Token: 0x0400234D RID: 9037
	private bool breaking;
}
