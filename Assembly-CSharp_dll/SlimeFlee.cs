using System;
using UnityEngine;

// Token: 0x02000489 RID: 1161
public class SlimeFlee : SlimeSubbehaviour
{
	// Token: 0x170001D8 RID: 472
	// (get) Token: 0x0600181C RID: 6172 RVA: 0x0005D7EF File Offset: 0x0005B9EF
	// (set) Token: 0x0600181D RID: 6173 RVA: 0x0005D7F7 File Offset: 0x0005B9F7
	private protected Vector3? fleeDir { protected get; private set; }

	// Token: 0x0600181E RID: 6174 RVA: 0x0005D800 File Offset: 0x0005BA00
	public override void Awake()
	{
		base.Awake();
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
	}

	// Token: 0x0600181F RID: 6175 RVA: 0x0005D818 File Offset: 0x0005BA18
	public void StartFleeing(GameObject fleeFrom)
	{
		this.SetFleeDirection(base.transform.position - SlimeSubbehaviour.GetGotoPos(fleeFrom));
		this.plexer.ForceRethink();
	}

	// Token: 0x06001820 RID: 6176 RVA: 0x0005D841 File Offset: 0x0005BA41
	protected void SetFleeDirection(Vector3 direction)
	{
		direction.y = 0f;
		this.fleeDir = new Vector3?(direction.normalized);
	}

	// Token: 0x06001821 RID: 6177 RVA: 0x0005D864 File Offset: 0x0005BA64
	public bool IsFleeing()
	{
		return this.fleeDir != null;
	}

	// Token: 0x06001822 RID: 6178 RVA: 0x0005D880 File Offset: 0x0005BA80
	public override float Relevancy(bool isGrounded)
	{
		if (this.fleeDir != null)
		{
			return 1f;
		}
		return 0f;
	}

	// Token: 0x06001823 RID: 6179 RVA: 0x0005D8A8 File Offset: 0x0005BAA8
	public override void Selected()
	{
		if (this.fleeCue != null)
		{
			SlimeAudio component = base.GetComponent<SlimeAudio>();
			if (component != null)
			{
				component.Play(this.fleeCue);
			}
		}
	}

	// Token: 0x06001824 RID: 6180 RVA: 0x0005D8E0 File Offset: 0x0005BAE0
	public override void Action()
	{
		if (this.fleeDir == null)
		{
			return;
		}
		if (this.plexer.IsBlocked(null, this.fleeDir.Value, 0, false))
		{
			SRBehaviour.SpawnAndPlayFX(this.disappearFX, base.transform.position, base.transform.rotation);
			Destroyer.DestroyActor(base.gameObject, "SlimeFlee.Action", false);
			return;
		}
		this.MoveTowards(this.fleeDir.Value);
	}

	// Token: 0x06001825 RID: 6181 RVA: 0x0005D964 File Offset: 0x0005BB64
	protected void MoveTowards(Vector3 dirToTarget)
	{
		base.RotateTowards(dirToTarget, this.facingSpeed, this.facingStability);
		this.slimeBody.AddForce(dirToTarget * (300f * this.slimeBody.mass * this.fleeSpeedFactor * Time.fixedDeltaTime));
		Vector3 position = base.transform.position + Vector3.down * (0.5f * base.transform.localScale.y);
		this.slimeBody.AddForceAtPosition(dirToTarget * (540f * this.slimeBody.mass * Time.fixedDeltaTime), position);
	}

	// Token: 0x04001786 RID: 6022
	public GameObject disappearFX;

	// Token: 0x04001787 RID: 6023
	public float facingStability = 1f;

	// Token: 0x04001788 RID: 6024
	public float facingSpeed = 5f;

	// Token: 0x04001789 RID: 6025
	public float fleeSpeedFactor = 1f;

	// Token: 0x0400178A RID: 6026
	public SECTR_AudioCue fleeCue;

	// Token: 0x0400178C RID: 6028
	protected TimeDirector timeDir;
}
