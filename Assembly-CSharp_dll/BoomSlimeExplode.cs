using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200039F RID: 927
public class BoomSlimeExplode : SlimeSubbehaviour, BoomMaterialAnimator.BoomMaterialInformer
{
	// Token: 0x06001355 RID: 4949 RVA: 0x0004B4E4 File Offset: 0x000496E4
	public override void Awake()
	{
		base.Awake();
		this.sfAnimator = base.GetComponent<SlimeFaceAnimator>();
		this.calmed = base.GetComponent<CalmedByWaterSpray>();
		this.slimeAppearanceApplicator = base.GetComponent<SlimeAppearanceApplicator>();
		if (this.slimeAppearanceApplicator.Appearance != null)
		{
			this.explodeFX = this.slimeAppearanceApplicator.Appearance.ExplosionAppearance.explodeFx;
		}
		this.slimeAppearanceApplicator.OnAppearanceChanged += delegate(SlimeAppearance appearance)
		{
			this.explodeFX = appearance.ExplosionAppearance.explodeFx;
		};
	}

	// Token: 0x06001356 RID: 4950 RVA: 0x0004B560 File Offset: 0x00049760
	public override void OnEnable()
	{
		base.OnEnable();
		if (Time.time + BoomSlimeExplode.BOOM_MIN_DELAY > this.nextPossibleExplode)
		{
			this.nextPossibleExplode = Math.Max(this.nextPossibleExplode, Time.time + Randoms.SHARED.GetFloat(BoomSlimeExplode.BOOM_MIN_DELAY));
		}
	}

	// Token: 0x06001357 RID: 4951 RVA: 0x0004B5AC File Offset: 0x000497AC
	public override void Start()
	{
		base.Start();
		this.nextExplodeDelayTime = this.BoomDelay();
		this.nextPossibleExplode = Time.time + this.nextExplodeDelayTime * Randoms.SHARED.GetInRange(0.25f, 1f);
		base.GetComponentsInChildren<ExplodeIndicatorMarker>(true)[0].SetActive(false);
	}

	// Token: 0x06001358 RID: 4952 RVA: 0x0004B601 File Offset: 0x00049801
	public override float Relevancy(bool isGrounded)
	{
		if (Time.fixedTime <= this.nextPossibleExplode || this.calmed.IsCalmed())
		{
			return 0f;
		}
		return 1f;
	}

	// Token: 0x06001359 RID: 4953 RVA: 0x00003296 File Offset: 0x00001496
	public override void Action()
	{
	}

	// Token: 0x0600135A RID: 4954 RVA: 0x0004B628 File Offset: 0x00049828
	public override void Selected()
	{
		base.StartCoroutine(this.DelayedExplosion());
	}

	// Token: 0x0600135B RID: 4955 RVA: 0x0004B637 File Offset: 0x00049837
	public void FixedUpdate()
	{
		if (this.calmed.IsCalmed())
		{
			this.nextPossibleExplode += Time.fixedDeltaTime;
		}
	}

	// Token: 0x0600135C RID: 4956 RVA: 0x0004B658 File Offset: 0x00049858
	private float BoomDelay()
	{
		return Mathf.Lerp(BoomSlimeExplode.BOOM_MIN_DELAY, BoomSlimeExplode.BOOM_MAX_DELAY, Mathf.Clamp(Randoms.SHARED.GetInRange(-0.1f, 0.1f) + (1f - this.emotions.GetCurr(SlimeEmotions.Emotion.AGITATION)), 0f, 1f));
	}

	// Token: 0x0600135D RID: 4957 RVA: 0x0004B6AA File Offset: 0x000498AA
	private IEnumerator DelayedExplosion()
	{
		this.state = BoomSlimeExplode.State.PREPARING;
		base.GetComponentsInChildren<ExplodeIndicatorMarker>(true)[0].SetActive(true);
		this.sfAnimator.SetTrigger("triggerGrimace");
		yield return new WaitForSeconds(BoomSlimeExplode.EXPLOSION_PREP_TIME);
		base.GetComponentsInChildren<ExplodeIndicatorMarker>(true)[0].SetActive(false);
		this.state = BoomSlimeExplode.State.EXPLODING;
		SRBehaviour.SpawnAndPlayFX(this.explodeFX, base.transform.position, base.transform.rotation);
		this.Explode();
		this.nextExplodeDelayTime = this.BoomDelay();
		this.nextPossibleExplode = Time.time + this.nextExplodeDelayTime;
		this.state = BoomSlimeExplode.State.RECOVERING;
		this.sfAnimator.SetTrigger("triggerFried");
		this.nextRecoverTime = Time.time + BoomSlimeExplode.EXPLOSION_RECOVERY_TIME;
		yield return new WaitForSeconds(BoomSlimeExplode.EXPLOSION_RECOVERY_TIME);
		this.state = BoomSlimeExplode.State.IDLE;
		yield break;
	}

	// Token: 0x0600135E RID: 4958 RVA: 0x0004B6BC File Offset: 0x000498BC
	private void Explode()
	{
		PhysicsUtil.Explode(base.gameObject, this.explodeRadius, this.explodePower, this.minPlayerDamage, this.maxPlayerDamage, false);
		if (base.gameObject.layer == LayerMask.NameToLayer("Launched"))
		{
			SRSingleton<SceneContext>.Instance.AchievementsDirector.AddToStat(AchievementsDirector.IntStat.LAUNCHED_BOOM_EXPLODE, 1);
		}
	}

	// Token: 0x0600135F RID: 4959 RVA: 0x0004B716 File Offset: 0x00049916
	public override void OnDisable()
	{
		base.OnDisable();
		this.state = BoomSlimeExplode.State.IDLE;
	}

	// Token: 0x06001360 RID: 4960 RVA: 0x0004B725 File Offset: 0x00049925
	public override bool CanRethink()
	{
		return this.state == BoomSlimeExplode.State.IDLE;
	}

	// Token: 0x06001361 RID: 4961 RVA: 0x0004B730 File Offset: 0x00049930
	public float GetReadiness()
	{
		return 1f - Mathf.Clamp((this.nextPossibleExplode - Time.time) / this.nextExplodeDelayTime, 0f, 1f);
	}

	// Token: 0x06001362 RID: 4962 RVA: 0x0004B75A File Offset: 0x0004995A
	public float GetRecoveriness()
	{
		if (this.state != BoomSlimeExplode.State.RECOVERING)
		{
			return 0f;
		}
		return Mathf.Clamp((this.nextRecoverTime - Time.time) / BoomSlimeExplode.EXPLOSION_RECOVERY_TIME, 0f, 1f);
	}

	// Token: 0x0400120E RID: 4622
	public float explodePower = 600f;

	// Token: 0x0400120F RID: 4623
	public float explodeRadius = 7f;

	// Token: 0x04001210 RID: 4624
	public float minPlayerDamage = 15f;

	// Token: 0x04001211 RID: 4625
	public float maxPlayerDamage = 45f;

	// Token: 0x04001212 RID: 4626
	private GameObject explodeFX;

	// Token: 0x04001213 RID: 4627
	private float nextPossibleExplode;

	// Token: 0x04001214 RID: 4628
	private float nextExplodeDelayTime = BoomSlimeExplode.BOOM_MAX_DELAY;

	// Token: 0x04001215 RID: 4629
	private float nextRecoverTime;

	// Token: 0x04001216 RID: 4630
	private SlimeFaceAnimator sfAnimator;

	// Token: 0x04001217 RID: 4631
	private CalmedByWaterSpray calmed;

	// Token: 0x04001218 RID: 4632
	private SlimeAppearanceApplicator slimeAppearanceApplicator;

	// Token: 0x04001219 RID: 4633
	public static float BOOM_MIN_DELAY = 10f;

	// Token: 0x0400121A RID: 4634
	public static float BOOM_MAX_DELAY = 45f;

	// Token: 0x0400121B RID: 4635
	public static float EXPLOSION_PREP_TIME = 1.5f;

	// Token: 0x0400121C RID: 4636
	public static float EXPLOSION_RECOVERY_TIME = 5f;

	// Token: 0x0400121D RID: 4637
	private BoomSlimeExplode.State state;

	// Token: 0x020003A0 RID: 928
	private enum State
	{
		// Token: 0x0400121F RID: 4639
		IDLE,
		// Token: 0x04001220 RID: 4640
		PREPARING,
		// Token: 0x04001221 RID: 4641
		EXPLODING,
		// Token: 0x04001222 RID: 4642
		RECOVERING
	}
}
