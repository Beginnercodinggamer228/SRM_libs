using System;
using UnityEngine;

// Token: 0x020003DB RID: 987
public class FollowWaypoints : SlimeSubbehaviour
{
	// Token: 0x06001484 RID: 5252 RVA: 0x0004F751 File Offset: 0x0004D951
	public override void Awake()
	{
		base.Awake();
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
	}

	// Token: 0x06001485 RID: 5253 RVA: 0x0004F76C File Offset: 0x0004D96C
	public override float Relevancy(bool isGrounded)
	{
		if (!this.timeDir.HasReached(this.disableUntil))
		{
			return 0f;
		}
		if (this.nextWaypoint == null)
		{
			this.nextWaypoint = RaceWaypoint.GetNearest(base.transform.position, 225f);
			if (this.nextWaypoint != null)
			{
				this.resetAfter = this.timeDir.HoursFromNow(0.16666667f);
			}
		}
		if (!(this.nextWaypoint == null))
		{
			return 0.8f;
		}
		return 0f;
	}

	// Token: 0x06001486 RID: 5254 RVA: 0x00003296 File Offset: 0x00001496
	public override void Selected()
	{
	}

	// Token: 0x06001487 RID: 5255 RVA: 0x0004F7F8 File Offset: 0x0004D9F8
	public override void Deselected()
	{
		base.Deselected();
		this.nextWaypoint = null;
		this.resetAfter = double.PositiveInfinity;
		this.disableUntil = 0.0;
		this.rotateTime = 0.0;
	}

	// Token: 0x06001488 RID: 5256 RVA: 0x0004F834 File Offset: 0x0004DA34
	public override void Action()
	{
		if (this.timeDir.HasReached(this.resetAfter))
		{
			this.disableUntil = this.timeDir.HoursFromNow(0.16666667f);
			this.nextWaypoint = null;
			this.resetAfter = double.PositiveInfinity;
			return;
		}
		if (this.nextWaypoint != null)
		{
			if (this.nextWaypoint.HasHitCheckpoint(base.transform.position))
			{
				this.nextWaypoint = this.nextWaypoint.GetNext();
				if (this.nextWaypoint != null)
				{
					this.resetAfter = this.timeDir.HoursFromNow(0.16666667f);
				}
			}
			if (this.nextWaypoint != null)
			{
				Vector3 normalized = (this.nextWaypoint.transform.position - base.transform.position).normalized;
				if (this.timeDir.HasReached(this.rotateTime))
				{
					base.RotateTowards(normalized, this.facingSpeed, this.facingStability);
					this.rotateTime = this.timeDir.HoursFromNow(this.rotationDelay * 0.00027777778f);
				}
				if (base.IsGrounded())
				{
					this.MoveTowards(normalized, Mathf.Min(this.timeDir.HasReached(this.slowUntil) ? 1f : this.slowSpeedLimit, this.nextWaypoint.approachForceFactor));
				}
			}
		}
	}

	// Token: 0x06001489 RID: 5257 RVA: 0x0004F99C File Offset: 0x0004DB9C
	public void ApplySlow(float durationGameHrs)
	{
		if (this.timeDir.HasReached(this.slowUntil))
		{
			if (this.slimeBody.velocity.sqrMagnitude > this.slowSpeedInstantMaxVelocity * this.slowSpeedInstantMaxVelocity)
			{
				this.slimeBody.velocity = this.slimeBody.velocity.normalized * this.slowSpeedInstantMaxVelocity;
			}
			this.slimeBody.velocity = this.slimeBody.velocity * this.slowSpeedInstantFactor;
		}
		this.slowUntil = this.timeDir.HoursFromNow(durationGameHrs);
	}

	// Token: 0x0600148A RID: 5258 RVA: 0x0004FA3C File Offset: 0x0004DC3C
	private void MoveTowards(Vector3 direction, float approachForceFactor)
	{
		this.slimeBody.AddForce(direction * (this.straightlineForceFactor * approachForceFactor * 80f * this.slimeBody.mass * Time.fixedDeltaTime));
		Vector3 position = base.transform.position + Vector3.down * (0.5f * base.transform.localScale.y);
		this.slimeBody.AddForceAtPosition(direction * (this.straightlineForceFactor * approachForceFactor * 240f * this.slimeBody.mass * Time.fixedDeltaTime), position);
	}

	// Token: 0x0400134B RID: 4939
	public float straightlineForceFactor = 1f;

	// Token: 0x0400134C RID: 4940
	public float facingStability = 1f;

	// Token: 0x0400134D RID: 4941
	public float facingSpeed = 5f;

	// Token: 0x0400134E RID: 4942
	public float slowSpeedLimit = 0.35f;

	// Token: 0x0400134F RID: 4943
	[Tooltip("Factor multiplied instantly to the slime's velocity when slow is applied.")]
	public float slowSpeedInstantFactor = 0.6f;

	// Token: 0x04001350 RID: 4944
	[Tooltip("Maximum slime starting velocity when the slow is applied.")]
	public float slowSpeedInstantMaxVelocity = 18f;

	// Token: 0x04001351 RID: 4945
	[Tooltip("Delay, in game seconds, between rotation changes. Helps reduce jitter.")]
	public float rotationDelay = 10f;

	// Token: 0x04001352 RID: 4946
	private RaceWaypoint nextWaypoint;

	// Token: 0x04001353 RID: 4947
	private double resetAfter = double.PositiveInfinity;

	// Token: 0x04001354 RID: 4948
	private double disableUntil;

	// Token: 0x04001355 RID: 4949
	private double slowUntil;

	// Token: 0x04001356 RID: 4950
	private double rotateTime;

	// Token: 0x04001357 RID: 4951
	private TimeDirector timeDir;

	// Token: 0x04001358 RID: 4952
	private const float TRY_TO_FOLLOW_TIME = 0.16666667f;

	// Token: 0x04001359 RID: 4953
	private const float RESET_DISABLE_TIME = 0.16666667f;
}
