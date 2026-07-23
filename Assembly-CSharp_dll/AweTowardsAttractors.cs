using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000394 RID: 916
public class AweTowardsAttractors : SlimeSubbehaviour
{
	// Token: 0x0600131E RID: 4894 RVA: 0x0004ABC2 File Offset: 0x00048DC2
	public override void Awake()
	{
		base.Awake();
		this.startTime = Time.time;
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.sfAnimator = base.GetComponent<SlimeFaceAnimator>();
	}

	// Token: 0x0600131F RID: 4895 RVA: 0x0004ABF4 File Offset: 0x00048DF4
	public override float Relevancy(bool isGrounded)
	{
		if (this.attractors.Count == 0 || !isGrounded || !this.timeDir.HasReached(this.nextActivationTime))
		{
			return 0f;
		}
		this.target = Randoms.SHARED.Pick<Attractor>(this.attractors, null);
		if (this.target == null)
		{
			this.attractors.Remove(this.target);
			this.target = null;
			return 0f;
		}
		if (!(this.target == null))
		{
			return Randoms.SHARED.GetInRange(0.1f, 1f) * this.target.AweFactor(base.gameObject);
		}
		return 0f;
	}

	// Token: 0x06001320 RID: 4896 RVA: 0x0004ACA8 File Offset: 0x00048EA8
	public override void Action()
	{
		if (this.target != null)
		{
			base.RotateTowards(SlimeSubbehaviour.GetGotoPos(this.target.gameObject) - base.transform.position, this.facingSpeed, this.facingStability);
			if (this.target.CauseMoveTowards())
			{
				this.ScootTowards(this.target.transform.position);
			}
		}
	}

	// Token: 0x06001321 RID: 4897 RVA: 0x0004AD18 File Offset: 0x00048F18
	private void ScootTowards(Vector3 targetPos)
	{
		Vector3 normalized = (targetPos - base.transform.position).normalized;
		float num = this.ScootCycleSpeed();
		this.slimeBody.AddForce(normalized * (150f * this.slimeBody.mass * 0.5f * Time.fixedDeltaTime * num));
		Vector3 position = base.transform.position + Vector3.down * (0.5f * base.transform.localScale.y);
		this.slimeBody.AddForceAtPosition(normalized * (270f * this.slimeBody.mass * Time.fixedDeltaTime * num), position);
	}

	// Token: 0x06001322 RID: 4898 RVA: 0x0004ADD2 File Offset: 0x00048FD2
	protected float ScootCycleSpeed()
	{
		return Mathf.Sin((Time.time - this.startTime) * 6.2831855f) + 1f;
	}

	// Token: 0x06001323 RID: 4899 RVA: 0x0004ADF1 File Offset: 0x00048FF1
	public override void Selected()
	{
		this.sfAnimator.SetTrigger("triggerLongAwe");
		this.nextActivationTime = this.timeDir.HoursFromNow(1f);
		this.endTime = Time.time + 3f;
	}

	// Token: 0x06001324 RID: 4900 RVA: 0x0004AE2A File Offset: 0x0004902A
	public override bool CanRethink()
	{
		return Time.time >= this.endTime;
	}

	// Token: 0x06001325 RID: 4901 RVA: 0x0004AE3C File Offset: 0x0004903C
	public void RegisterAttractor(Attractor attractor)
	{
		this.attractors.Add(attractor);
	}

	// Token: 0x06001326 RID: 4902 RVA: 0x0004AE4A File Offset: 0x0004904A
	public void UnregisterAttractor(Attractor attractor)
	{
		this.attractors.Remove(attractor);
	}

	// Token: 0x040011ED RID: 4589
	public float facingStability = 1f;

	// Token: 0x040011EE RID: 4590
	public float facingSpeed = 5f;

	// Token: 0x040011EF RID: 4591
	private Attractor target;

	// Token: 0x040011F0 RID: 4592
	private List<Attractor> attractors = new List<Attractor>();

	// Token: 0x040011F1 RID: 4593
	private TimeDirector timeDir;

	// Token: 0x040011F2 RID: 4594
	private SlimeFaceAnimator sfAnimator;

	// Token: 0x040011F3 RID: 4595
	private double nextActivationTime;

	// Token: 0x040011F4 RID: 4596
	private float startTime;

	// Token: 0x040011F5 RID: 4597
	private float endTime;

	// Token: 0x040011F6 RID: 4598
	private const float SCOOT_CYCLE_TIME = 1f;

	// Token: 0x040011F7 RID: 4599
	private const float SCOOT_CYCLE_FACTOR = 6.2831855f;

	// Token: 0x040011F8 RID: 4600
	private const float SCOOT_SPEED_FACTOR = 0.5f;
}
