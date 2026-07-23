using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020004A4 RID: 1188
public class StalkConsumable : FindConsumable, Collidable, RegistryUpdateable
{
	// Token: 0x060018C8 RID: 6344 RVA: 0x00060590 File Offset: 0x0005E790
	public override void Awake()
	{
		base.Awake();
		this.stealth = base.GetComponent<SlimeStealth>();
		this.animButtWiggleId = Animator.StringToHash("ButtWiggle");
		this.identifiable = base.GetComponent<Identifiable>();
	}

	// Token: 0x060018C9 RID: 6345 RVA: 0x000605C0 File Offset: 0x0005E7C0
	public override void Start()
	{
		base.Start();
		this.searchIds[Identifiable.Id.PLAYER] = new StalkConsumable.CalmableDriveCalculator(base.GetComponent<CalmedByWaterSpray>(), SlimeEmotions.Emotion.NONE, -0.1f, -1f);
	}

	// Token: 0x060018CA RID: 6346 RVA: 0x000605EB File Offset: 0x0005E7EB
	public override bool Forbids(SlimeSubbehaviour toMaybeForbid)
	{
		return toMaybeForbid is GotoConsumable;
	}

	// Token: 0x060018CB RID: 6347 RVA: 0x000605F8 File Offset: 0x0005E7F8
	public override float Relevancy(bool isGrounded)
	{
		if ((double)Time.time < this.nextStalkTime)
		{
			return 0f;
		}
		this.target = base.FindNearestConsumable(out this.currDrive);
		if (this.target == null)
		{
			return 0f;
		}
		if (!(this.target == null))
		{
			return this.currDrive * this.currDrive;
		}
		return 0f;
	}

	// Token: 0x060018CC RID: 6348 RVA: 0x00060660 File Offset: 0x0005E860
	protected override Dictionary<Identifiable.Id, DriveCalculator> GetSearchIds()
	{
		Dictionary<Identifiable.Id, DriveCalculator> searchIds = base.GetSearchIds();
		searchIds[Identifiable.Id.PLAYER] = new StalkConsumable.CalmableDriveCalculator(base.GetComponent<CalmedByWaterSpray>(), SlimeEmotions.Emotion.NONE, -0.1f, -1f);
		return searchIds;
	}

	// Token: 0x060018CD RID: 6349 RVA: 0x00060686 File Offset: 0x0005E886
	public override bool CanRethink()
	{
		return this.mode == StalkConsumable.Mode.NONE;
	}

	// Token: 0x060018CE RID: 6350 RVA: 0x00060691 File Offset: 0x0005E891
	public override void Selected()
	{
		this.SetStealth(true);
	}

	// Token: 0x060018CF RID: 6351 RVA: 0x0006069A File Offset: 0x0005E89A
	public override void Deselected()
	{
		base.Deselected();
		this.SetMode(StalkConsumable.Mode.NONE);
		this.SetStealth(false);
	}

	// Token: 0x060018D0 RID: 6352 RVA: 0x000606B0 File Offset: 0x0005E8B0
	public override void OnDisable()
	{
		base.OnDisable();
		this.target = null;
	}

	// Token: 0x060018D1 RID: 6353 RVA: 0x000606C0 File Offset: 0x0005E8C0
	public override void Action()
	{
		if (this.target == null)
		{
			this.SetMode(StalkConsumable.Mode.NONE);
			this.endModeTime = 0.0;
			return;
		}
		if (this.mode == StalkConsumable.Mode.NONE)
		{
			bool flag = (SlimeSubbehaviour.GetGotoPos(this.target) - base.transform.position).sqrMagnitude < 64f;
			this.SetMode(flag ? StalkConsumable.Mode.PREP : StalkConsumable.Mode.APPROACH);
			this.endModeTime = (double)(Time.time + (flag ? 2f : 3f));
		}
		if (this.mode == StalkConsumable.Mode.APPROACH)
		{
			if (base.IsGrounded())
			{
				Vector3 vector = SlimeSubbehaviour.GetGotoPos(this.target) - base.transform.position;
				Vector3 normalized = vector.normalized;
				base.RotateTowards(normalized);
				Rigidbody component = base.GetComponent<Rigidbody>();
				float d = (float)((component.velocity.sqrMagnitude < 10f) ? 650 : 400);
				Vector3 normalized2 = (normalized * 400f + Vector3.up * d).normalized;
				component.AddForce(normalized2 * (250f * component.mass * this.pursuitSpeedFactor * Time.fixedDeltaTime));
				Vector3 position = base.transform.position + Vector3.down * (0.5f * base.transform.localScale.y);
				component.AddForceAtPosition(normalized2 * (450f * component.mass * Time.fixedDeltaTime), position);
				if (vector.sqrMagnitude < 64f)
				{
					this.SetMode(StalkConsumable.Mode.PREP);
					this.endModeTime = (double)(Time.fixedTime + 2f);
				}
			}
			if (this.mode == StalkConsumable.Mode.APPROACH && (double)Time.fixedTime > this.endModeTime)
			{
				this.SetMode(StalkConsumable.Mode.WAIT);
				this.endModeTime = (double)(Time.fixedTime + 3f);
				return;
			}
		}
		else if (this.mode == StalkConsumable.Mode.WAIT)
		{
			if ((double)Time.fixedTime > this.endModeTime)
			{
				this.SetMode(StalkConsumable.Mode.NONE);
				this.endModeTime = 0.0;
				return;
			}
		}
		else if (this.mode == StalkConsumable.Mode.PREP)
		{
			if ((double)Time.fixedTime > this.endModeTime)
			{
				if (this.doesParkour)
				{
					this.feinting = false;
					this.SetMode(StalkConsumable.Mode.FEINT);
					this.endModeTime = (double)(Time.time + 1f);
					return;
				}
				this.feinting = false;
				this.pounceFromPivot = false;
				this.SetMode(StalkConsumable.Mode.POUNCE);
				this.endModeTime = (double)(Time.time + 1f);
				return;
			}
		}
		else if (this.mode == StalkConsumable.Mode.POUNCE)
		{
			if (base.IsGrounded() || this.pounceFromPivot)
			{
				if (this.pounceFromPivot)
				{
					base.GetComponent<Rigidbody>().velocity = Vector3.zero;
				}
				Vector3 vector2 = SlimeSubbehaviour.GetGotoPos(this.target) - base.transform.position;
				float sqrMagnitude = vector2.sqrMagnitude;
				Vector3 normalized3 = vector2.normalized;
				this.LeapToward(sqrMagnitude, normalized3, normalized3);
				this.SetMode(StalkConsumable.Mode.NONE);
				this.endModeTime = 0.0;
				this.target = null;
				this.pouncing = true;
				this.pounceResetTime = SRSingleton<SceneContext>.Instance.TimeDirector.WorldTime() + 60.0;
				this.pounceFromPivot = false;
				this.nextStalkTime = (double)(Time.fixedTime + 15f);
			}
			if ((double)Time.fixedTime > this.endModeTime)
			{
				this.SetMode(StalkConsumable.Mode.NONE);
				this.endModeTime = 0.0;
				this.feinting = false;
				return;
			}
		}
		else if (this.mode == StalkConsumable.Mode.FEINT)
		{
			if (base.IsGrounded())
			{
				Vector3 vector3 = SlimeSubbehaviour.GetGotoPos(this.target) - base.transform.position;
				float sqrMagnitude2 = vector3.sqrMagnitude;
				Vector3 normalized4 = vector3.normalized;
				float angle = Randoms.SHARED.GetInRange(this.feintMinAngle, this.feintMaxAngle) * (float)(Randoms.SHARED.GetBoolean() ? -1 : 1);
				this.LeapToward(sqrMagnitude2 * Mathf.Pow(this.feintPowerMultiplier, 2f), Quaternion.AngleAxis(angle, Vector3.up) * normalized4, normalized4);
				this.feinting = true;
				this.SetMode(StalkConsumable.Mode.PIVOT);
				this.endModeTime = (double)(Time.time + 1.5f);
			}
			if ((double)Time.fixedTime > this.endModeTime)
			{
				this.SetMode(StalkConsumable.Mode.NONE);
				this.endModeTime = 0.0;
				this.feinting = false;
				return;
			}
		}
		else if (this.mode == StalkConsumable.Mode.PIVOT)
		{
			if (this.pivotNow)
			{
				this.pivotNow = false;
				this.pounceFromPivot = true;
				this.SetMode(StalkConsumable.Mode.POUNCE);
				this.endModeTime = (double)(Time.time + 1f);
			}
			if ((double)Time.fixedTime > this.endModeTime)
			{
				this.SetMode(StalkConsumable.Mode.NONE);
				this.endModeTime = 0.0;
				this.feinting = false;
			}
		}
	}

	// Token: 0x060018D2 RID: 6354 RVA: 0x00060BAC File Offset: 0x0005EDAC
	private void LeapToward(float distanceSquared, Vector3 directionToJump, Vector3 directionToFace)
	{
		base.RotateTowards(directionToFace);
		float num = 1.2f;
		float d = Mathf.Sqrt(Mathf.Sqrt(distanceSquared) * Physics.gravity.magnitude) * num;
		base.GetComponent<Rigidbody>().AddForce((directionToJump + Vector3.up).normalized * d, ForceMode.VelocityChange);
		this.slimeAudio.Play(this.slimeAudio.slimeSounds.jumpCue);
		this.slimeAudio.Play(this.slimeAudio.slimeSounds.voiceJumpCue);
	}

	// Token: 0x060018D3 RID: 6355 RVA: 0x00060C3D File Offset: 0x0005EE3D
	public void RegistryUpdate()
	{
		if (!this.hasStarted)
		{
			return;
		}
		if (base.IsGrounded() && SRSingleton<SceneContext>.Instance.TimeDirector.HasReached(this.pounceResetTime))
		{
			this.pouncing = false;
		}
	}

	// Token: 0x060018D4 RID: 6356 RVA: 0x00060C70 File Offset: 0x0005EE70
	public void ProcessCollisionEnter(Collision col)
	{
		if (Identifiable.BOOP_CLASS.Contains(this.identifiable.id) && this.pouncing && this.stealth == null && col.gameObject == SRSingleton<SceneContext>.Instance.Player)
		{
			Vector3 vector = col.gameObject.transform.InverseTransformPoint(col.contacts[0].point);
			if (vector.z > 0.2f && vector.y > 1f)
			{
				SRSingleton<SceneContext>.Instance.AchievementsDirector.AddToStat(AchievementsDirector.IntStat.TABBY_HEADBUTT, 1);
				return;
			}
		}
		else if (this.feinting)
		{
			this.pivotNow = true;
			this.feinting = false;
		}
	}

	// Token: 0x060018D5 RID: 6357 RVA: 0x00003296 File Offset: 0x00001496
	public void ProcessCollisionExit(Collision col)
	{
	}

	// Token: 0x060018D6 RID: 6358 RVA: 0x00060D28 File Offset: 0x0005EF28
	private void SetMode(StalkConsumable.Mode mode)
	{
		if (this.mode != mode)
		{
			this.mode = mode;
			base.GetComponentInChildren<Animator>().SetBool(this.animButtWiggleId, mode == StalkConsumable.Mode.PREP);
			if (mode == StalkConsumable.Mode.PREP)
			{
				this.slimeAudio.Play(this.slimeAudio.slimeSounds.wiggleCue);
			}
		}
	}

	// Token: 0x060018D7 RID: 6359 RVA: 0x00060D79 File Offset: 0x0005EF79
	private void SetStealth(bool isStealthed)
	{
		if (this.stealth != null)
		{
			this.stealth.SetStealth(isStealthed);
		}
	}

	// Token: 0x04001872 RID: 6258
	[Tooltip("Whether this should engage targets with parkour.")]
	public bool doesParkour;

	// Token: 0x04001873 RID: 6259
	[Tooltip("The power of the feint jump will be modified by this. A value of 1.0 is a normal power jump.")]
	public float feintPowerMultiplier = 1f;

	// Token: 0x04001874 RID: 6260
	[Tooltip("The minimum angle away from the player that the feint should be toward.")]
	public float feintMinAngle = 55f;

	// Token: 0x04001875 RID: 6261
	[Tooltip("The maximum angle away from the player that the feint should be toward.")]
	public float feintMaxAngle = 80f;

	// Token: 0x04001876 RID: 6262
	private GameObject target;

	// Token: 0x04001877 RID: 6263
	private float currDrive;

	// Token: 0x04001878 RID: 6264
	private bool pouncing;

	// Token: 0x04001879 RID: 6265
	private bool feinting;

	// Token: 0x0400187A RID: 6266
	private double pounceResetTime;

	// Token: 0x0400187B RID: 6267
	private bool pounceFromPivot;

	// Token: 0x0400187C RID: 6268
	private StalkConsumable.Mode mode;

	// Token: 0x0400187D RID: 6269
	private double endModeTime;

	// Token: 0x0400187E RID: 6270
	private double nextStalkTime;

	// Token: 0x0400187F RID: 6271
	private double initStealthUntil;

	// Token: 0x04001880 RID: 6272
	private const float POUNCE_DIST = 8f;

	// Token: 0x04001881 RID: 6273
	private const float POUNCE_DIST_SQR = 64f;

	// Token: 0x04001882 RID: 6274
	private const float APPROACH_TIME = 3f;

	// Token: 0x04001883 RID: 6275
	private const float WAIT_TIME = 3f;

	// Token: 0x04001884 RID: 6276
	private const float PREP_TIME = 2f;

	// Token: 0x04001885 RID: 6277
	private const float POUNCE_TIME = 1f;

	// Token: 0x04001886 RID: 6278
	private const float FEINT_TIME = 1f;

	// Token: 0x04001887 RID: 6279
	private const float PIVOT_TIME = 1.5f;

	// Token: 0x04001888 RID: 6280
	private const float POUNCE_RESET_TIME = 15f;

	// Token: 0x04001889 RID: 6281
	private const float PLAYER_EAT_EXTRA_DRIVE = -0.1f;

	// Token: 0x0400188A RID: 6282
	private const float CALMED_PLAYER_EAT_EXTRA_DRIVE = -1f;

	// Token: 0x0400188B RID: 6283
	private int animButtWiggleId;

	// Token: 0x0400188C RID: 6284
	private SlimeStealth stealth;

	// Token: 0x0400188D RID: 6285
	private bool pivotNow;

	// Token: 0x0400188E RID: 6286
	private Identifiable identifiable;

	// Token: 0x020004A5 RID: 1189
	private enum Mode
	{
		// Token: 0x04001890 RID: 6288
		NONE,
		// Token: 0x04001891 RID: 6289
		APPROACH,
		// Token: 0x04001892 RID: 6290
		WAIT,
		// Token: 0x04001893 RID: 6291
		PREP,
		// Token: 0x04001894 RID: 6292
		POUNCE,
		// Token: 0x04001895 RID: 6293
		FEINT,
		// Token: 0x04001896 RID: 6294
		PIVOT
	}

	// Token: 0x020004A6 RID: 1190
	private class CalmableDriveCalculator : DriveCalculator
	{
		// Token: 0x060018D9 RID: 6361 RVA: 0x00060DBE File Offset: 0x0005EFBE
		public CalmableDriveCalculator(CalmedByWaterSpray calmed, SlimeEmotions.Emotion emotion, float normalBonus, float calmedBonus) : base(emotion, normalBonus, 0f)
		{
			this.calmedExtraDrive = calmedBonus;
			this.calmed = calmed;
		}

		// Token: 0x060018DA RID: 6362 RVA: 0x00060DDC File Offset: 0x0005EFDC
		public override float Drive(SlimeEmotions emotions, Identifiable.Id id)
		{
			return Mathf.Max(0f, emotions.GetCurr(this.emotion) + (this.calmed.IsCalmed() ? this.calmedExtraDrive : this.extraDrive));
		}

		// Token: 0x04001897 RID: 6295
		private CalmedByWaterSpray calmed;

		// Token: 0x04001898 RID: 6296
		private float calmedExtraDrive;
	}
}
