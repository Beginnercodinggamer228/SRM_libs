using System;
using UnityEngine;

// Token: 0x020003A3 RID: 931
public class ChickenRandomMove : SlimeSubbehaviour, RegistryUpdateable
{
	// Token: 0x06001370 RID: 4976 RVA: 0x0004B9D4 File Offset: 0x00049BD4
	public override void Awake()
	{
		base.Awake();
		this.slimeAudio = base.GetComponent<SlimeAudio>();
		this.animator = base.GetComponentInChildren<Animator>();
		this.animGroundedId = Animator.StringToHash("grounded");
		this.animWalkId = Animator.StringToHash("walk");
		this.animPeckId = Animator.StringToHash("peck");
	}

	// Token: 0x06001371 RID: 4977 RVA: 0x0004BA2F File Offset: 0x00049C2F
	public override void Start()
	{
		base.Start();
	}

	// Token: 0x06001372 RID: 4978 RVA: 0x0004BA37 File Offset: 0x00049C37
	public override float Relevancy(bool isGrounded)
	{
		return 0.2f;
	}

	// Token: 0x06001373 RID: 4979 RVA: 0x0004BA3E File Offset: 0x00049C3E
	public override void Selected()
	{
		this.SelectMode();
	}

	// Token: 0x06001374 RID: 4980 RVA: 0x0004BA46 File Offset: 0x00049C46
	private void SelectMode()
	{
		this.mode = (Randoms.SHARED.GetProbability(0.5f) ? ChickenRandomMove.Mode.JUMP : (Randoms.SHARED.GetChance(2) ? ChickenRandomMove.Mode.PECK : ChickenRandomMove.Mode.WALK));
		this.nextModeChoice = Time.time + 1f;
	}

	// Token: 0x06001375 RID: 4981 RVA: 0x0004BA84 File Offset: 0x00049C84
	public override void Action()
	{
		if (Time.fixedTime >= this.nextModeChoice)
		{
			this.SelectMode();
		}
		if (base.IsGrounded())
		{
			if (this.mode == ChickenRandomMove.Mode.JUMP)
			{
				if (this.slimeBody.velocity.sqrMagnitude <= 0.010000001f)
				{
					float min = 0.5f * this.maxJump * this.slimeBody.mass;
					this.slimeBody.AddForce(0f, UnityEngine.Random.Range(min, this.maxJump), 0f, ForceMode.Impulse);
					this.slimeBody.AddTorque(0f, UnityEngine.Random.Range(-0.2f, 0.2f), 0f, ForceMode.Impulse);
					this.slimeAudio.Play(this.slimeAudio.slimeSounds.jumpCue);
					this.slimeAudio.Play(this.slimeAudio.slimeSounds.voiceJumpCue);
					this.mode = ChickenRandomMove.Mode.WAIT;
				}
			}
			else if (this.mode != ChickenRandomMove.Mode.PECK && this.mode == ChickenRandomMove.Mode.WALK)
			{
				float num = 1f;
				this.slimeBody.AddForce(base.transform.forward * (this.walkForwardForce * this.slimeBody.mass * num * Time.fixedDeltaTime), ForceMode.Impulse);
				Vector3 position = base.transform.position + Vector3.down * (0.5f * base.transform.localScale.y);
				this.slimeBody.AddForceAtPosition(base.transform.forward * (2f * this.walkForwardForce * this.slimeBody.mass * num * Time.fixedDeltaTime), position, ForceMode.Impulse);
			}
		}
		this.animator.SetBool(this.animWalkId, this.mode == ChickenRandomMove.Mode.WALK);
		this.animator.SetBool(this.animPeckId, this.mode == ChickenRandomMove.Mode.PECK);
	}

	// Token: 0x06001376 RID: 4982 RVA: 0x0004BC70 File Offset: 0x00049E70
	public void RegistryUpdate()
	{
		bool flag = base.IsGrounded();
		if (this.animator.GetBool(this.animGroundedId) && !flag)
		{
			this.slimeAudio.Play(this.flapCue);
		}
		this.animator.SetBool(this.animGroundedId, flag);
	}

	// Token: 0x0400122B RID: 4651
	public float maxJump = 1f;

	// Token: 0x0400122C RID: 4652
	public float walkForwardForce = 0.05f;

	// Token: 0x0400122D RID: 4653
	public SECTR_AudioCue flapCue;

	// Token: 0x0400122E RID: 4654
	private const float JUMP_PROB = 0.5f;

	// Token: 0x0400122F RID: 4655
	private const float JUMP_TORQUE = 0.2f;

	// Token: 0x04001230 RID: 4656
	private SlimeAudio slimeAudio;

	// Token: 0x04001231 RID: 4657
	private ChickenRandomMove.Mode mode;

	// Token: 0x04001232 RID: 4658
	private float nextModeChoice;

	// Token: 0x04001233 RID: 4659
	private Animator animator;

	// Token: 0x04001234 RID: 4660
	private int animGroundedId;

	// Token: 0x04001235 RID: 4661
	private int animWalkId;

	// Token: 0x04001236 RID: 4662
	private int animPeckId;

	// Token: 0x04001237 RID: 4663
	private const float MAX_VEL_TO_BOUNCE = 0.1f;

	// Token: 0x04001238 RID: 4664
	private const float SQR_MAX_VEL_TO_BOUNCE = 0.010000001f;

	// Token: 0x020003A4 RID: 932
	private enum Mode
	{
		// Token: 0x0400123A RID: 4666
		JUMP,
		// Token: 0x0400123B RID: 4667
		PECK,
		// Token: 0x0400123C RID: 4668
		WALK,
		// Token: 0x0400123D RID: 4669
		WAIT
	}
}
