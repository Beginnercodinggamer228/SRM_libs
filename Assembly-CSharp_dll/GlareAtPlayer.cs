using System;
using UnityEngine;

// Token: 0x020003E0 RID: 992
public class GlareAtPlayer : SlimeSubbehaviour
{
	// Token: 0x060014B0 RID: 5296 RVA: 0x0005071F File Offset: 0x0004E91F
	public override void Awake()
	{
		base.Awake();
		this.feral = base.GetComponent<SlimeFeral>();
	}

	// Token: 0x060014B1 RID: 5297 RVA: 0x00050734 File Offset: 0x0004E934
	public override float Relevancy(bool isGrounded)
	{
		if (isGrounded && this.feral.IsFeral() && Time.time >= this.nextGlareTime)
		{
			float sqrMagnitude = (SlimeSubbehaviour.GetGotoPos(SRSingleton<SceneContext>.Instance.Player) - base.transform.position).sqrMagnitude;
			if (sqrMagnitude <= this.maxGlareDistance && sqrMagnitude >= this.minGlareDistance)
			{
				return Randoms.SHARED.GetInRange(0.3f, 1f);
			}
		}
		return 0f;
	}

	// Token: 0x060014B2 RID: 5298 RVA: 0x000507B2 File Offset: 0x0004E9B2
	public override bool CanRethink()
	{
		return !this.isGlaring;
	}

	// Token: 0x060014B3 RID: 5299 RVA: 0x000507BD File Offset: 0x0004E9BD
	public override void Selected()
	{
		this.isGlaring = true;
		this.stopGlareTime = Time.time + this.glareTime;
	}

	// Token: 0x060014B4 RID: 5300 RVA: 0x000507D8 File Offset: 0x0004E9D8
	public override void Action()
	{
		if (this.isGlaring && Time.time >= this.stopGlareTime)
		{
			this.isGlaring = false;
			this.nextGlareTime = Time.time + this.minGlareDelay;
			return;
		}
		Vector3 vector = SRSingleton<SceneContext>.Instance.Player.transform.TransformPoint(new Vector3(0f, 0f, 2f)) - base.transform.position;
		float sqrMagnitude = vector.sqrMagnitude;
		Vector3 normalized = vector.normalized;
		base.RotateTowards(normalized, 1f, 5f);
	}

	// Token: 0x04001388 RID: 5000
	public float glareTime = 5f;

	// Token: 0x04001389 RID: 5001
	public float minGlareDelay = 5f;

	// Token: 0x0400138A RID: 5002
	public float minGlareDistance;

	// Token: 0x0400138B RID: 5003
	public float maxGlareDistance = 20f;

	// Token: 0x0400138C RID: 5004
	private bool isGlaring;

	// Token: 0x0400138D RID: 5005
	private SlimeFeral feral;

	// Token: 0x0400138E RID: 5006
	private float nextGlareTime;

	// Token: 0x0400138F RID: 5007
	private float stopGlareTime;
}
