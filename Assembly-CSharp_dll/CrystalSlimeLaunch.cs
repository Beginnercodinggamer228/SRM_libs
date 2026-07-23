using System;
using System.Collections;
using UnityEngine;

// Token: 0x020003AE RID: 942
public class CrystalSlimeLaunch : SlimeSubbehaviour
{
	// Token: 0x060013A4 RID: 5028 RVA: 0x0004C374 File Offset: 0x0004A574
	public override void Awake()
	{
		base.Awake();
		this.timeDirector = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.animatorBase = base.GetComponentInChildren<Animator>();
		this.animatorFace = base.GetComponent<SlimeFaceAnimator>();
		this.audio = base.GetComponent<SlimeAudio>();
		this.calmed = base.GetComponent<CalmedByWaterSpray>();
		this.slimeAppearanceApplicator = base.GetComponent<SlimeAppearanceApplicator>();
		this.slimeAppearanceApplicator.OnAppearanceChanged += this.UpdateSpikeAppearance;
		if (this.slimeAppearanceApplicator.Appearance != null)
		{
			this.UpdateSpikeAppearance(this.slimeAppearanceApplicator.Appearance);
		}
	}

	// Token: 0x060013A5 RID: 5029 RVA: 0x0004C40E File Offset: 0x0004A60E
	public override void Start()
	{
		base.Start();
		this.ResetNextLaunchTime();
	}

	// Token: 0x060013A6 RID: 5030 RVA: 0x0004C41C File Offset: 0x0004A61C
	public override float Relevancy(bool isGrounded)
	{
		if (!isGrounded || !this.timeDirector.HasReached(this.nextLaunchTime) || this.calmed.IsCalmed())
		{
			return 0f;
		}
		this.rollDirection = base.transform.right;
		this.rollDirection.y = 0f;
		this.rollDirection.Normalize();
		this.rollForward = Vector3.Cross(this.rollDirection, Vector3.up);
		return 0.3f;
	}

	// Token: 0x060013A7 RID: 5031 RVA: 0x0004C49C File Offset: 0x0004A69C
	public override void Selected()
	{
		this.stateEndTime = this.timeDirector.HoursFromNow(0.016666668f);
		this.state = CrystalSlimeLaunch.State.PREPARING;
		this.animatorFace.SetTrigger("triggerAwe");
		this.audio.Play(this.audio.slimeSounds.rollCue);
	}

	// Token: 0x060013A8 RID: 5032 RVA: 0x0004C4F1 File Offset: 0x0004A6F1
	public override void Deselected()
	{
		base.Deselected();
		this.animatorBase.SetBool(CrystalSlimeLaunch.ANIMATION_ROCK_MODE, false);
		this.state = CrystalSlimeLaunch.State.IDLE;
	}

	// Token: 0x060013A9 RID: 5033 RVA: 0x0004C511 File Offset: 0x0004A711
	public override bool CanRethink()
	{
		return this.state == CrystalSlimeLaunch.State.IDLE;
	}

	// Token: 0x060013AA RID: 5034 RVA: 0x0004C51C File Offset: 0x0004A71C
	public override void Action()
	{
		if (this.calmed.IsCalmed())
		{
			this.plexer.ForceRethink();
			return;
		}
		if (this.timeDirector.HasReached(this.stateEndTime) && base.IsGrounded())
		{
			if (this.state == CrystalSlimeLaunch.State.PREPARING)
			{
				base.StartCoroutine(this.CreateSpikes());
				this.stateEndTime = this.timeDirector.HoursFromNow(0.0016666668f);
				this.state = CrystalSlimeLaunch.State.LAUNCHED;
			}
			else if (this.state == CrystalSlimeLaunch.State.LAUNCHED)
			{
				this.ResetNextLaunchTime();
				this.state = CrystalSlimeLaunch.State.IDLE;
				this.stateEndTime = 0.0;
			}
		}
		if (this.state == CrystalSlimeLaunch.State.LAUNCHED)
		{
			this.slimeBody.AddTorque(this.rollDirection * (1200f * this.slimeBody.mass * Time.fixedDeltaTime));
		}
	}

	// Token: 0x060013AB RID: 5035 RVA: 0x0004C5ED File Offset: 0x0004A7ED
	private void UpdateSpikeAppearance(SlimeAppearance appearance)
	{
		this.launchSpawnLarge = appearance.CrystalAppearance.largeCrystalPrefab;
		this.launchSpawnSmall = appearance.CrystalAppearance.smallCrystalPrefab;
	}

	// Token: 0x060013AC RID: 5036 RVA: 0x0004C611 File Offset: 0x0004A811
	private IEnumerator CreateSpikes()
	{
		this.animatorBase.SetBool(CrystalSlimeLaunch.ANIMATION_ROCK_MODE, true);
		this.slimeBody.AddForce(Vector3.up * 200f + this.rollForward * 40f * this.slimeBody.mass);
		this.CreateSpikes(this.launchSpawnLarge, base.transform.position);
		yield return new WaitForSeconds(0.2f);
		int inRange = Randoms.SHARED.GetInRange(Mathf.CeilToInt(4f * this.slimeBody.mass), Mathf.CeilToInt(7f * this.slimeBody.mass));
		float num = Randoms.SHARED.GetFloat(6.2831855f);
		int i = 0;
		while (i < inRange)
		{
			this.CreateSpikes(this.launchSpawnSmall, base.transform.position + new Vector3(Mathf.Cos(num), 0f, Mathf.Sin(num)) * 1.5f);
			i++;
			num += 6.2831855f / (float)inRange;
		}
		yield break;
	}

	// Token: 0x060013AD RID: 5037 RVA: 0x0004C620 File Offset: 0x0004A820
	private bool CreateSpikes(GameObject prefab, Vector3 position)
	{
		RaycastHit? raycastHit = null;
		foreach (RaycastHit value in Physics.RaycastAll(new Ray(position, Vector3.down), 2f, 2129921, QueryTriggerInteraction.Ignore))
		{
			if (value.rigidbody == null && (raycastHit == null || value.distance < raycastHit.Value.distance))
			{
				raycastHit = new RaycastHit?(value);
			}
		}
		if (raycastHit != null)
		{
			SRBehaviour.InstantiateDynamic(prefab, raycastHit.Value.point, Quaternion.Euler(0f, Randoms.SHARED.GetFloat(360f), 0f), false);
			return true;
		}
		return false;
	}

	// Token: 0x060013AE RID: 5038 RVA: 0x0004C6E4 File Offset: 0x0004A8E4
	private void ResetNextLaunchTime()
	{
		this.nextLaunchTime = this.timeDirector.HoursFromNow(Mathf.Lerp(0.050000004f, 0.25f, Mathf.Clamp(Randoms.SHARED.GetInRange(-0.1f, 0.1f) + (1f - this.emotions.GetCurr(SlimeEmotions.Emotion.AGITATION)), 0f, 1f)));
	}

	// Token: 0x0400125B RID: 4699
	private static readonly int ANIMATION_ROCK_MODE = Animator.StringToHash("RockMode");

	// Token: 0x0400125C RID: 4700
	private const float LAUNCH_MIN_HOURS = 0.050000004f;

	// Token: 0x0400125D RID: 4701
	private const float LAUNCH_MAX_HOURS = 0.25f;

	// Token: 0x0400125E RID: 4702
	private const float MIN_LAUNCH_HOURS = 0.0016666668f;

	// Token: 0x0400125F RID: 4703
	private const float PREP_HOURS = 0.016666668f;

	// Token: 0x04001260 RID: 4704
	private const float LAUNCH_FORCE = 200f;

	// Token: 0x04001261 RID: 4705
	private const float LAUNCH_FORCE_FORWARD = 40f;

	// Token: 0x04001262 RID: 4706
	private const float SPIKES_SPREAD = 1.5f;

	// Token: 0x04001263 RID: 4707
	private const int LAYER_MASK = 2129921;

	// Token: 0x04001264 RID: 4708
	private GameObject launchSpawnLarge;

	// Token: 0x04001265 RID: 4709
	private GameObject launchSpawnSmall;

	// Token: 0x04001266 RID: 4710
	private TimeDirector timeDirector;

	// Token: 0x04001267 RID: 4711
	private Animator animatorBase;

	// Token: 0x04001268 RID: 4712
	private SlimeFaceAnimator animatorFace;

	// Token: 0x04001269 RID: 4713
	private CalmedByWaterSpray calmed;

	// Token: 0x0400126A RID: 4714
	private SlimeAppearanceApplicator slimeAppearanceApplicator;

	// Token: 0x0400126B RID: 4715
	private SlimeAudio audio;

	// Token: 0x0400126C RID: 4716
	private Vector3 rollDirection;

	// Token: 0x0400126D RID: 4717
	private Vector3 rollForward;

	// Token: 0x0400126E RID: 4718
	private double nextLaunchTime;

	// Token: 0x0400126F RID: 4719
	private CrystalSlimeLaunch.State state;

	// Token: 0x04001270 RID: 4720
	private double stateEndTime;

	// Token: 0x020003AF RID: 943
	private enum State
	{
		// Token: 0x04001272 RID: 4722
		IDLE,
		// Token: 0x04001273 RID: 4723
		PREPARING,
		// Token: 0x04001274 RID: 4724
		LAUNCHED
	}
}
