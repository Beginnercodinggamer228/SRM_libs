using System;
using UnityEngine;

// Token: 0x02000494 RID: 1172
public abstract class SlimeSubbehaviour : CollidableActorBehaviour
{
	// Token: 0x0600185A RID: 6234 RVA: 0x0005E466 File Offset: 0x0005C666
	public override void Awake()
	{
		base.Awake();
		this.plexer = base.GetComponent<SlimeSubbehaviourPlexer>();
		this.emotions = base.GetComponent<SlimeEmotions>();
		this.vacuumable = base.GetComponent<Vacuumable>();
		this.slimeBody = base.GetComponent<Rigidbody>();
	}

	// Token: 0x0600185B RID: 6235
	public abstract float Relevancy(bool isGrounded);

	// Token: 0x0600185C RID: 6236
	public abstract void Action();

	// Token: 0x0600185D RID: 6237
	public abstract void Selected();

	// Token: 0x0600185E RID: 6238 RVA: 0x00003296 File Offset: 0x00001496
	public virtual void Deselected()
	{
	}

	// Token: 0x0600185F RID: 6239 RVA: 0x00013CC5 File Offset: 0x00011EC5
	public virtual bool CanRethink()
	{
		return true;
	}

	// Token: 0x06001860 RID: 6240 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
	public virtual bool Forbids(SlimeSubbehaviour toMaybeForbid)
	{
		return false;
	}

	// Token: 0x06001861 RID: 6241 RVA: 0x0005E49E File Offset: 0x0005C69E
	protected bool IsFloating()
	{
		return this.plexer != null && this.plexer.IsFloating();
	}

	// Token: 0x06001862 RID: 6242 RVA: 0x0005E4BB File Offset: 0x0005C6BB
	protected bool IsGrounded()
	{
		return this.plexer != null && this.plexer.IsGrounded();
	}

	// Token: 0x06001863 RID: 6243 RVA: 0x0005E4D8 File Offset: 0x0005C6D8
	protected bool IsNearGrounded(float dist)
	{
		return this.plexer != null && this.plexer.IsNearGrounded(dist);
	}

	// Token: 0x06001864 RID: 6244 RVA: 0x0005E4F6 File Offset: 0x0005C6F6
	protected bool IsBlocked(GameObject obj, int layersToIgnore = 0, bool forceCheckFullDist = false)
	{
		return this.plexer != null && this.plexer.IsBlocked(obj, layersToIgnore, forceCheckFullDist);
	}

	// Token: 0x06001865 RID: 6245 RVA: 0x0005E518 File Offset: 0x0005C718
	protected bool IsCaptive()
	{
		bool result = false;
		if (this.vacuumable != null)
		{
			result = this.vacuumable.isCaptive();
		}
		return result;
	}

	// Token: 0x06001866 RID: 6246 RVA: 0x0005E544 File Offset: 0x0005C744
	protected void RotateTowards(Vector3 dirToTarget, float facingSpeed, float facingStability)
	{
		Vector3 angularVelocity = this.slimeBody.angularVelocity;
		Vector3 a = Vector3.Cross(Quaternion.AngleAxis(angularVelocity.magnitude * 57.29578f * facingStability * 0.1f / facingSpeed, angularVelocity) * base.transform.forward, dirToTarget);
		this.slimeBody.AddTorque(a * (facingSpeed * facingSpeed) * this.slimeBody.mass);
	}

	// Token: 0x06001867 RID: 6247 RVA: 0x0005E5B5 File Offset: 0x0005C7B5
	public static Vector3 GetGotoPos(GameObject obj)
	{
		if (!(obj == SRSingleton<SceneContext>.Instance.Player))
		{
			return obj.transform.position;
		}
		return obj.transform.position + Vector3.up;
	}

	// Token: 0x040017F0 RID: 6128
	protected SlimeEmotions emotions;

	// Token: 0x040017F1 RID: 6129
	protected Vacuumable vacuumable;

	// Token: 0x040017F2 RID: 6130
	protected SlimeSubbehaviourPlexer plexer;

	// Token: 0x040017F3 RID: 6131
	protected Rigidbody slimeBody;

	// Token: 0x040017F4 RID: 6132
	private const float STABILIZE_FACTOR = 0.1f;
}
