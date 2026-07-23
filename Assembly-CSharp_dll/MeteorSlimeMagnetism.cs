using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200040A RID: 1034
public class MeteorSlimeMagnetism : SlimeSubbehaviour
{
	// Token: 0x0600159A RID: 5530 RVA: 0x00054177 File Offset: 0x00052377
	public override void Awake()
	{
		base.Awake();
		this.attracteeMask = LayerMask.GetMask(new string[]
		{
			"Actor",
			"ActorIgnorePlayer"
		});
		this.sfAnimator = base.GetComponent<SlimeFaceAnimator>();
	}

	// Token: 0x0600159B RID: 5531 RVA: 0x000541AC File Offset: 0x000523AC
	public override void Start()
	{
		base.Start();
		this.nextExplodeDelayTime = this.AttractDelay();
		this.nextPossibleAttract = Time.time + this.nextExplodeDelayTime * Randoms.SHARED.GetInRange(0.25f, 1f);
		base.GetComponentsInChildren<ExplodeIndicatorMarker>(true)[0].SetActive(false);
	}

	// Token: 0x0600159C RID: 5532 RVA: 0x00054201 File Offset: 0x00052401
	public override float Relevancy(bool isGrounded)
	{
		if (Time.time <= this.nextPossibleAttract)
		{
			return 0f;
		}
		return 1f;
	}

	// Token: 0x0600159D RID: 5533 RVA: 0x00003296 File Offset: 0x00001496
	public override void Action()
	{
	}

	// Token: 0x0600159E RID: 5534 RVA: 0x0005421B File Offset: 0x0005241B
	public override void Selected()
	{
		base.StartCoroutine(this.DelayedAttract());
	}

	// Token: 0x0600159F RID: 5535 RVA: 0x000526CC File Offset: 0x000508CC
	public override void Deselected()
	{
		base.Deselected();
	}

	// Token: 0x060015A0 RID: 5536 RVA: 0x0005422C File Offset: 0x0005242C
	private float AttractDelay()
	{
		return Mathf.Lerp(10f, 45f, Mathf.Clamp(Randoms.SHARED.GetInRange(-0.1f, 0.1f) + (1f - this.emotions.GetCurr(SlimeEmotions.Emotion.AGITATION)), 0f, 1f));
	}

	// Token: 0x060015A1 RID: 5537 RVA: 0x0005427E File Offset: 0x0005247E
	private IEnumerator DelayedAttract()
	{
		this.state = MeteorSlimeMagnetism.State.PREPARING;
		base.GetComponentsInChildren<ExplodeIndicatorMarker>(true)[0].SetActive(true);
		this.sfAnimator.SetTrigger("triggerGrimace");
		float originalMass = this.slimeBody.mass;
		this.slimeBody.mass *= 100f;
		yield return new WaitForSeconds(1.5f);
		base.GetComponentsInChildren<ExplodeIndicatorMarker>(true)[0].SetActive(false);
		this.state = MeteorSlimeMagnetism.State.ATTRACTING;
		this.FindAttractees();
		SRBehaviour.SpawnAndPlayFX(this.attractFX, base.transform.position, base.transform.rotation);
		this.nextExplodeDelayTime = this.AttractDelay();
		this.nextPossibleAttract = Time.time + this.nextExplodeDelayTime;
		this.state = MeteorSlimeMagnetism.State.RECOVERING;
		this.sfAnimator.SetTrigger("triggerFried");
		this.nextRecoverTime = Time.time + 5f;
		yield return new WaitForSeconds(5f);
		this.attractees.Clear();
		this.slimeBody.mass = originalMass;
		this.state = MeteorSlimeMagnetism.State.IDLE;
		yield break;
	}

	// Token: 0x060015A2 RID: 5538 RVA: 0x0005428D File Offset: 0x0005248D
	public void FixedUpdate()
	{
		if (this.state == MeteorSlimeMagnetism.State.RECOVERING || this.state == MeteorSlimeMagnetism.State.ATTRACTING)
		{
			this.Attract();
		}
	}

	// Token: 0x060015A3 RID: 5539 RVA: 0x000542A8 File Offset: 0x000524A8
	private void FindAttractees()
	{
		Vector3 position = base.transform.position;
		HashSet<GameObject> hashSet = new HashSet<GameObject>();
		foreach (Collider collider in Physics.OverlapSphere(position, this.attractRadius, this.attracteeMask, QueryTriggerInteraction.Ignore))
		{
			if (collider != null)
			{
				Rigidbody component = collider.GetComponent<Rigidbody>();
				GameObject gameObject = collider.gameObject;
				if (component != null && gameObject != base.gameObject && !hashSet.Contains(gameObject))
				{
					if (gameObject != SRSingleton<SceneContext>.Instance.Player)
					{
						this.attractees.Add(component);
					}
					hashSet.Add(gameObject);
				}
			}
		}
	}

	// Token: 0x060015A4 RID: 5540 RVA: 0x00054350 File Offset: 0x00052550
	private void Attract()
	{
		Vector3 position = base.transform.position;
		float num = this.attractPower * Time.fixedDeltaTime;
		for (int i = 0; i < this.attractees.Count; i++)
		{
			Rigidbody rigidbody = this.attractees[i];
			if (!(rigidbody == null))
			{
				rigidbody.AddExplosionForce(-num, position, this.attractRadius);
			}
		}
	}

	// Token: 0x060015A5 RID: 5541 RVA: 0x000543B4 File Offset: 0x000525B4
	private void ApplyLowGravChargesNearby()
	{
		Vector3 position = base.transform.position;
		HashSet<GameObject> hashSet = new HashSet<GameObject>();
		foreach (Collider collider in Physics.OverlapSphere(position, 2f))
		{
			if (collider != null && !collider.isTrigger)
			{
				UnityEngine.Object component = collider.GetComponent<Rigidbody>();
				GameObject gameObject = collider.gameObject;
				if (component != null && gameObject != base.gameObject && !hashSet.Contains(gameObject))
				{
					if (gameObject != SRSingleton<SceneContext>.Instance.Player)
					{
						gameObject.AddComponent<MeteorSlimeMagnetism.LowGravity>().SetLowGrav(0.2f, this.lowGravFX);
					}
					hashSet.Add(gameObject);
				}
			}
		}
	}

	// Token: 0x060015A6 RID: 5542 RVA: 0x00054465 File Offset: 0x00052665
	public override bool CanRethink()
	{
		return this.state == MeteorSlimeMagnetism.State.IDLE;
	}

	// Token: 0x060015A7 RID: 5543 RVA: 0x00054470 File Offset: 0x00052670
	public float GetReadiness()
	{
		return 1f - Mathf.Clamp((this.nextPossibleAttract - Time.time) / this.nextExplodeDelayTime, 0f, 1f);
	}

	// Token: 0x060015A8 RID: 5544 RVA: 0x0005449A File Offset: 0x0005269A
	public float GetRecoveriness()
	{
		if (this.state != MeteorSlimeMagnetism.State.RECOVERING)
		{
			return 0f;
		}
		return Mathf.Clamp((this.nextRecoverTime - Time.time) / 5f, 0f, 1f);
	}

	// Token: 0x04001496 RID: 5270
	public float attractPower = 600f;

	// Token: 0x04001497 RID: 5271
	public float attractRadius = 12f;

	// Token: 0x04001498 RID: 5272
	public GameObject attractFX;

	// Token: 0x04001499 RID: 5273
	public GameObject lowGravFX;

	// Token: 0x0400149A RID: 5274
	private float nextPossibleAttract;

	// Token: 0x0400149B RID: 5275
	private float nextExplodeDelayTime = 45f;

	// Token: 0x0400149C RID: 5276
	private float nextRecoverTime;

	// Token: 0x0400149D RID: 5277
	private SlimeFaceAnimator sfAnimator;

	// Token: 0x0400149E RID: 5278
	private List<Rigidbody> attractees = new List<Rigidbody>();

	// Token: 0x0400149F RID: 5279
	private const float ATTRACT_MIN_DELAY = 10f;

	// Token: 0x040014A0 RID: 5280
	private const float ATTRACT_MAX_DELAY = 45f;

	// Token: 0x040014A1 RID: 5281
	private const float ATTRACT_PREP_TIME = 1.5f;

	// Token: 0x040014A2 RID: 5282
	private const float ATTRACT_RECOVERY_TIME = 5f;

	// Token: 0x040014A3 RID: 5283
	private const float LOW_GRAV_TIME = 10f;

	// Token: 0x040014A4 RID: 5284
	private const float LOW_GRAV_FACTOR = 0.2f;

	// Token: 0x040014A5 RID: 5285
	private const float LOW_GRAV_RAD = 2f;

	// Token: 0x040014A6 RID: 5286
	private const float MAGNETIC_MASS_FACTOR = 100f;

	// Token: 0x040014A7 RID: 5287
	private int attracteeMask;

	// Token: 0x040014A8 RID: 5288
	private MeteorSlimeMagnetism.State state;

	// Token: 0x0200040B RID: 1035
	private enum State
	{
		// Token: 0x040014AA RID: 5290
		IDLE,
		// Token: 0x040014AB RID: 5291
		PREPARING,
		// Token: 0x040014AC RID: 5292
		ATTRACTING,
		// Token: 0x040014AD RID: 5293
		RECOVERING
	}

	// Token: 0x0200040C RID: 1036
	private class LowGravity : SRBehaviour
	{
		// Token: 0x060015AA RID: 5546 RVA: 0x00054500 File Offset: 0x00052700
		public void Awake()
		{
			this.body = base.GetComponent<Rigidbody>();
			this.deathTime = Time.time + 10f;
		}

		// Token: 0x060015AB RID: 5547 RVA: 0x0005451F File Offset: 0x0005271F
		public void SetLowGrav(float factor, GameObject fxPrefab)
		{
			this.antiGrav = Physics.gravity * (-1f + factor);
			this.lowGravFX = UnityEngine.Object.Instantiate<GameObject>(fxPrefab);
			this.lowGravFX.transform.SetParent(base.transform, false);
		}

		// Token: 0x060015AC RID: 5548 RVA: 0x0005455B File Offset: 0x0005275B
		public void OnDestroy()
		{
			Destroyer.Destroy(this.lowGravFX, "MeteorSlimeMagnetism.OnDestroy");
		}

		// Token: 0x060015AD RID: 5549 RVA: 0x0005456D File Offset: 0x0005276D
		public void FixedUpdate()
		{
			if (Time.fixedTime >= this.deathTime)
			{
				Destroyer.Destroy(this, "MeteorSlimeMagnetism.FixedUpdate");
				return;
			}
			this.body.AddForce(this.antiGrav, ForceMode.Acceleration);
		}

		// Token: 0x040014AE RID: 5294
		private Rigidbody body;

		// Token: 0x040014AF RID: 5295
		private Vector3 antiGrav;

		// Token: 0x040014B0 RID: 5296
		private float deathTime;

		// Token: 0x040014B1 RID: 5297
		private GameObject lowGravFX;
	}
}
