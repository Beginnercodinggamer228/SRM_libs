using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200048F RID: 1167
public class SlimeRandomMove : SlimeSubbehaviour
{
	// Token: 0x06001842 RID: 6210 RVA: 0x0005DE4C File Offset: 0x0005C04C
	static SlimeRandomMove()
	{
		SlimeRandomMove.MODE_WEIGHTS[SlimeRandomMove.Mode.IDLE] = 0.2f;
		SlimeRandomMove.MODE_WEIGHTS[SlimeRandomMove.Mode.SCOOT] = 0.3f;
		SlimeRandomMove.MODE_WEIGHTS[SlimeRandomMove.Mode.JUMP] = 0.5f;
	}

	// Token: 0x06001843 RID: 6211 RVA: 0x0005DE88 File Offset: 0x0005C088
	public override void Awake()
	{
		base.Awake();
		this.slimeAudio = base.GetComponent<SlimeAudio>();
	}

	// Token: 0x06001844 RID: 6212 RVA: 0x0004BA2F File Offset: 0x00049C2F
	public override void Start()
	{
		base.Start();
	}

	// Token: 0x06001845 RID: 6213 RVA: 0x0004BA37 File Offset: 0x00049C37
	public override float Relevancy(bool isGrounded)
	{
		return 0.2f;
	}

	// Token: 0x06001846 RID: 6214 RVA: 0x00003296 File Offset: 0x00001496
	public override void Selected()
	{
	}

	// Token: 0x06001847 RID: 6215 RVA: 0x0005DE9C File Offset: 0x0005C09C
	public override void Action()
	{
		if (!base.IsGrounded())
		{
			return;
		}
		if (Time.fixedTime > this.modeChangeTime)
		{
			this.mode = Randoms.SHARED.Pick<SlimeRandomMove.Mode>(SlimeRandomMove.MODE_WEIGHTS, SlimeRandomMove.Mode.IDLE);
			this.modeChangeTime = Time.time + 10f;
			float f = Mathf.Atan2(base.transform.forward.z, base.transform.forward.x) + Randoms.SHARED.GetInRange(-0.5f, 0.5f);
			this.scootDir = new Vector3(Mathf.Cos(f), 0f, Mathf.Sin(f));
		}
		switch (this.mode)
		{
		case SlimeRandomMove.Mode.IDLE:
			break;
		case SlimeRandomMove.Mode.SCOOT:
		{
			base.RotateTowards(this.scootDir, 1f, 1f);
			float num = this.ScootCycleSpeed();
			this.slimeBody.AddForce(base.transform.forward * (150f * this.slimeBody.mass * this.scootSpeedFactor * Time.fixedDeltaTime * num));
			Vector3 position = base.transform.position + Vector3.down * (0.5f * base.transform.localScale.y);
			this.slimeBody.AddForceAtPosition(base.transform.forward * (270f * this.slimeBody.mass * Time.fixedDeltaTime * num), position);
			break;
		}
		case SlimeRandomMove.Mode.JUMP:
			if (Time.time > this.nextJumpTime && this.slimeBody.velocity.sqrMagnitude <= 25f && base.IsGrounded())
			{
				float num2 = 0.5f * this.maxJump * this.slimeBody.mass;
				this.slimeBody.AddForce(Randoms.SHARED.GetInRange(-num2, num2), this.verticalFactor * Randoms.SHARED.GetInRange(num2, this.maxJump * this.slimeBody.mass), Randoms.SHARED.GetInRange(-num2, num2), ForceMode.Impulse);
				this.slimeAudio.Play(this.slimeAudio.slimeSounds.jumpCue);
				this.slimeAudio.Play(this.slimeAudio.slimeSounds.voiceJumpCue);
				this.nextJumpTime = Time.fixedTime + 1f;
				return;
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06001848 RID: 6216 RVA: 0x0005E0FF File Offset: 0x0005C2FF
	protected float ScootCycleSpeed()
	{
		return Mathf.Sin(Time.fixedTime * 6.2831855f) + 1f;
	}

	// Token: 0x040017B5 RID: 6069
	public float verticalFactor = 1f;

	// Token: 0x040017B6 RID: 6070
	public float scootSpeedFactor = 1f;

	// Token: 0x040017B7 RID: 6071
	private float maxJump = 6f;

	// Token: 0x040017B8 RID: 6072
	private SlimeAudio slimeAudio;

	// Token: 0x040017B9 RID: 6073
	private float nextJumpTime;

	// Token: 0x040017BA RID: 6074
	private SlimeRandomMove.Mode mode;

	// Token: 0x040017BB RID: 6075
	private float modeChangeTime;

	// Token: 0x040017BC RID: 6076
	private Vector3 scootDir;

	// Token: 0x040017BD RID: 6077
	private const float TIME_BETWEEN_JUMPS = 1f;

	// Token: 0x040017BE RID: 6078
	private const float MODE_CHANGE_LENGTH = 10f;

	// Token: 0x040017BF RID: 6079
	private const float SCOOT_CYCLE_TIME = 1f;

	// Token: 0x040017C0 RID: 6080
	private const float SCOOT_CYCLE_FACTOR = 6.2831855f;

	// Token: 0x040017C1 RID: 6081
	private static Dictionary<SlimeRandomMove.Mode, float> MODE_WEIGHTS = new Dictionary<SlimeRandomMove.Mode, float>();

	// Token: 0x040017C2 RID: 6082
	private const float MAX_VEL_TO_BOUNCE = 5f;

	// Token: 0x040017C3 RID: 6083
	private const float SQR_MAX_VEL_TO_BOUNCE = 25f;

	// Token: 0x02000490 RID: 1168
	private enum Mode
	{
		// Token: 0x040017C5 RID: 6085
		IDLE,
		// Token: 0x040017C6 RID: 6086
		SCOOT,
		// Token: 0x040017C7 RID: 6087
		JUMP
	}
}
