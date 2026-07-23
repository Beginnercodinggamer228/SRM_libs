using System;
using UnityEngine;

// Token: 0x020003D2 RID: 978
public class FeralSlimeButtstomp : SlimeSubbehaviour, Collidable
{
	// Token: 0x0600144C RID: 5196 RVA: 0x0004E6E7 File Offset: 0x0004C8E7
	public override void Awake()
	{
		base.Awake();
		this.feral = base.GetComponent<SlimeFeral>();
		this.slimeAudio = base.GetComponent<SlimeAudio>();
		this.theGameModeConfig = SRSingleton<SceneContext>.Instance.GameModeConfig;
	}

	// Token: 0x0600144D RID: 5197 RVA: 0x0004E718 File Offset: 0x0004C918
	public override float Relevancy(bool isGrounded)
	{
		if (isGrounded && this.feral.IsFeral() && !this.theGameModeConfig.GetModeSettings().preventHostiles && Time.time >= this.nextStompTime)
		{
			float sqrMagnitude = (SlimeSubbehaviour.GetGotoPos(SRSingleton<SceneContext>.Instance.Player) - base.transform.position).sqrMagnitude;
			if (sqrMagnitude <= 400f && sqrMagnitude >= 25f)
			{
				return Randoms.SHARED.GetInRange(0.3f, 1f);
			}
		}
		return 0f;
	}

	// Token: 0x0600144E RID: 5198 RVA: 0x0004E7A6 File Offset: 0x0004C9A6
	public override bool CanRethink()
	{
		return this.mode == FeralSlimeButtstomp.Mode.WAITING || this.mode == FeralSlimeButtstomp.Mode.LANDED;
	}

	// Token: 0x0600144F RID: 5199 RVA: 0x0004E7BB File Offset: 0x0004C9BB
	public override void Selected()
	{
		this.mode = FeralSlimeButtstomp.Mode.WAITING;
	}

	// Token: 0x06001450 RID: 5200 RVA: 0x0004E7C4 File Offset: 0x0004C9C4
	public override void Action()
	{
		switch (this.mode)
		{
		case FeralSlimeButtstomp.Mode.WAITING:
			this.LaunchAction();
			return;
		case FeralSlimeButtstomp.Mode.MIDAIR:
			this.MidairAction();
			return;
		case FeralSlimeButtstomp.Mode.WAIT_FOR_GROUND_IMPACT:
			if (this.plexer.IsFloating())
			{
				this.mode = FeralSlimeButtstomp.Mode.STOMPING;
				return;
			}
			break;
		case FeralSlimeButtstomp.Mode.STOMPING:
			this.StompingAction();
			break;
		case FeralSlimeButtstomp.Mode.LANDED:
			break;
		default:
			return;
		}
	}

	// Token: 0x06001451 RID: 5201 RVA: 0x0004E81C File Offset: 0x0004CA1C
	private void LaunchAction()
	{
		Vector3 vector = SRSingleton<SceneContext>.Instance.Player.transform.TransformPoint(new Vector3(0f, 0f, 2f)) - base.transform.position;
		float sqrMagnitude = vector.sqrMagnitude;
		Vector3 normalized = vector.normalized;
		base.RotateTowards(normalized, 1f, 5f);
		float num = 1.2f;
		float num2 = 1.4f;
		float d = Mathf.Sqrt(Mathf.Sqrt(sqrMagnitude) * Physics.gravity.magnitude) * num * num2;
		this.slimeBody.AddForce((normalized + Vector3.up).normalized * d, ForceMode.VelocityChange);
		this.slimeAudio.Play(this.slimeAudio.slimeSounds.jumpCue);
		this.slimeAudio.Play(this.slimeAudio.slimeSounds.voiceJumpCue);
		this.slimeAudio.Play(this.slimeAudio.slimeSounds.stompJumpCue);
		this.mode = FeralSlimeButtstomp.Mode.MIDAIR;
	}

	// Token: 0x06001452 RID: 5202 RVA: 0x0004E92C File Offset: 0x0004CB2C
	private void MidairAction()
	{
		if (this.slimeBody.velocity.y <= 0f)
		{
			float magnitude = this.slimeBody.velocity.magnitude;
			this.slimeBody.velocity = new Vector3(0f, -magnitude, 0f);
			this.mode = FeralSlimeButtstomp.Mode.WAIT_FOR_GROUND_IMPACT;
		}
	}

	// Token: 0x06001453 RID: 5203 RVA: 0x0004E988 File Offset: 0x0004CB88
	private void StompingAction()
	{
		if (this.stompFX != null)
		{
			SRBehaviour.SpawnAndPlayFX(this.stompFX, base.transform.position, base.transform.rotation);
		}
		this.slimeAudio.Play(this.slimeAudio.slimeSounds.stompLandCue);
		this.Explode();
		this.mode = FeralSlimeButtstomp.Mode.LANDED;
		this.nextStompTime = Time.time + 5f;
	}

	// Token: 0x06001454 RID: 5204 RVA: 0x0004E9FE File Offset: 0x0004CBFE
	private void Explode()
	{
		PhysicsUtil.Explode(base.gameObject, this.explodeRadius, this.explodePower, this.minPlayerDamage, this.maxPlayerDamage, false);
	}

	// Token: 0x06001455 RID: 5205 RVA: 0x0004EA24 File Offset: 0x0004CC24
	public void ProcessCollisionEnter(Collision col)
	{
		if (this.mode == FeralSlimeButtstomp.Mode.WAIT_FOR_GROUND_IMPACT && base.transform.position.y - col.contacts[0].point.y >= 0.5f)
		{
			this.mode = FeralSlimeButtstomp.Mode.STOMPING;
		}
	}

	// Token: 0x06001456 RID: 5206 RVA: 0x00003296 File Offset: 0x00001496
	public void ProcessCollisionExit(Collision col)
	{
	}

	// Token: 0x0400130F RID: 4879
	public GameObject stompFX;

	// Token: 0x04001310 RID: 4880
	public float explodePower = 600f;

	// Token: 0x04001311 RID: 4881
	public float explodeRadius = 7f;

	// Token: 0x04001312 RID: 4882
	public float minPlayerDamage = 15f;

	// Token: 0x04001313 RID: 4883
	public float maxPlayerDamage = 45f;

	// Token: 0x04001314 RID: 4884
	private FeralSlimeButtstomp.Mode mode;

	// Token: 0x04001315 RID: 4885
	private float nextStompTime;

	// Token: 0x04001316 RID: 4886
	private SlimeFeral feral;

	// Token: 0x04001317 RID: 4887
	private SlimeAudio slimeAudio;

	// Token: 0x04001318 RID: 4888
	private GameModeConfig theGameModeConfig;

	// Token: 0x04001319 RID: 4889
	private const float MAX_DIST = 20f;

	// Token: 0x0400131A RID: 4890
	private const float MAX_DIST_SQR = 400f;

	// Token: 0x0400131B RID: 4891
	private const float MIN_DIST = 5f;

	// Token: 0x0400131C RID: 4892
	private const float MIN_DIST_SQR = 25f;

	// Token: 0x0400131D RID: 4893
	private const float STOMP_RESET_TIME = 5f;

	// Token: 0x0400131E RID: 4894
	private const float PLAYER_FORCE_FACTOR = 0.001f;

	// Token: 0x0400131F RID: 4895
	private const float UNDERNEATH_THRESHOLD = 0.5f;

	// Token: 0x020003D3 RID: 979
	private enum Mode
	{
		// Token: 0x04001321 RID: 4897
		WAITING,
		// Token: 0x04001322 RID: 4898
		MIDAIR,
		// Token: 0x04001323 RID: 4899
		WAIT_FOR_GROUND_IMPACT,
		// Token: 0x04001324 RID: 4900
		STOMPING,
		// Token: 0x04001325 RID: 4901
		LANDED
	}
}
