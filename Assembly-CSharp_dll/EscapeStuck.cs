using System;
using UnityEngine;

// Token: 0x020003C4 RID: 964
public class EscapeStuck : SlimeSubbehaviour
{
	// Token: 0x0600141D RID: 5149 RVA: 0x0004DD64 File Offset: 0x0004BF64
	public override void Awake()
	{
		base.Awake();
		this.slimeAudio = base.GetComponent<SlimeAudio>();
	}

	// Token: 0x0600141E RID: 5150 RVA: 0x0004DD78 File Offset: 0x0004BF78
	public override float Relevancy(bool isGrounded)
	{
		if (!isGrounded && this.slimeBody.velocity.sqrMagnitude < 0.0001f)
		{
			if (this.stuckSince == null)
			{
				this.stuckSince = new float?(Time.time);
			}
		}
		else
		{
			this.stuckSince = null;
		}
		if (this.stuckSince != null && Time.time - this.stuckSince.Value >= 2f)
		{
			return 1f;
		}
		return 0f;
	}

	// Token: 0x0600141F RID: 5151 RVA: 0x00003296 File Offset: 0x00001496
	public override void Selected()
	{
	}

	// Token: 0x06001420 RID: 5152 RVA: 0x0004DE00 File Offset: 0x0004C000
	public override void Action()
	{
		if (this.stuckSince != null)
		{
			float num = 0.5f * this.maxJump * this.slimeBody.mass;
			this.slimeBody.AddForce(UnityEngine.Random.Range(-num, num), this.verticalFactor * UnityEngine.Random.Range(num, this.maxJump), UnityEngine.Random.Range(-num, num), ForceMode.Impulse);
			this.slimeAudio.Play(this.slimeAudio.slimeSounds.jumpCue);
			this.slimeAudio.Play(this.slimeAudio.slimeSounds.voiceJumpCue);
			this.stuckSince = null;
		}
	}

	// Token: 0x040012D0 RID: 4816
	private float verticalFactor = 0.5f;

	// Token: 0x040012D1 RID: 4817
	private float maxJump = 4f;

	// Token: 0x040012D2 RID: 4818
	private float? stuckSince;

	// Token: 0x040012D3 RID: 4819
	private SlimeAudio slimeAudio;

	// Token: 0x040012D4 RID: 4820
	private const float MIN_TIME_TO_ACT = 2f;

	// Token: 0x040012D5 RID: 4821
	private const float STUCK_VEL = 0.01f;

	// Token: 0x040012D6 RID: 4822
	private const float STUCK_VEL_SQR = 0.0001f;
}
