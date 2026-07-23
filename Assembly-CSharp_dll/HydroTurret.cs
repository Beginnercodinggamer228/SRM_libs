using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x0200071F RID: 1823
public class HydroTurret : SRBehaviour
{
	// Token: 0x0600261D RID: 9757 RVA: 0x00091F98 File Offset: 0x00090198
	public void Awake()
	{
		this.region = base.GetComponentInParent<Region>();
		this.waiter = base.GetComponent<WaitForChargeup>();
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.tracker.SetFilter(new Predicate<GameObject>(this.IsTarr));
		this.liquidPrefab = SRSingleton<GameContext>.Instance.LookupDirector.GetPrefab(this.liquidId);
		foreach (HydroTurret hydroTurret in base.GetComponents<HydroTurret>())
		{
			if (hydroTurret != this)
			{
				this.otherTurrets.Add(hydroTurret);
			}
		}
	}

	// Token: 0x0600261E RID: 9758 RVA: 0x0009202D File Offset: 0x0009022D
	private void DelayForLocalShot()
	{
		this.nextShootTime = Math.Max(this.nextShootTime, this.timeDir.HoursFromNow(this.localShotDelay * 0.016666668f));
	}

	// Token: 0x0600261F RID: 9759 RVA: 0x00091E6D File Offset: 0x0009006D
	private bool IsTarr(GameObject gameObj)
	{
		return Identifiable.IsTarr(Identifiable.GetId(gameObj));
	}

	// Token: 0x06002620 RID: 9760 RVA: 0x00092058 File Offset: 0x00090258
	public void Update()
	{
		if (this.waiter.IsWaiting())
		{
			return;
		}
		if (this.timeDir.HasReached(this.nextShootTime))
		{
			HashSet<GameObject> hashSet = this.tracker.CurrColliders();
			if (hashSet.Count > 0)
			{
				if (this.currTarget == null || !hashSet.Contains(this.currTarget))
				{
					this.currTarget = Randoms.SHARED.Pick<GameObject>(hashSet, null);
				}
				HydroTurret.ShootResult shootResult = this.TryToShootAt(this.currTarget);
				if (shootResult == HydroTurret.ShootResult.SHOT)
				{
					this.currTarget = null;
					this.nextShootTime = this.timeDir.HoursFromNow(this.shootDelay * 0.016666668f);
					using (List<HydroTurret>.Enumerator enumerator = this.otherTurrets.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							HydroTurret hydroTurret = enumerator.Current;
							hydroTurret.DelayForLocalShot();
						}
						return;
					}
				}
				if (shootResult == HydroTurret.ShootResult.FAIL)
				{
					this.nextShootTime = this.timeDir.HoursFromNow(this.retargetDelay * 0.016666668f);
					return;
				}
			}
			else
			{
				this.nextShootTime = this.timeDir.HoursFromNow(this.retargetDelay * 0.016666668f);
			}
		}
	}

	// Token: 0x06002621 RID: 9761 RVA: 0x00092188 File Offset: 0x00090388
	private HydroTurret.ShootResult TryToShootAt(GameObject target)
	{
		Vector3 vector = target.transform.position - this.toRotate.position;
		float num = 25f;
		float num2 = Mathf.Abs(Physics.gravity.y);
		Vector3 vector2 = vector;
		vector2.y = 0f;
		float magnitude = vector2.magnitude;
		float y = vector.y;
		float num3 = num * num;
		float num4 = num3 * num3 - num2 * (num2 * magnitude * magnitude + 2f * y * num3);
		if (num4 < 0f)
		{
			return HydroTurret.ShootResult.FAIL;
		}
		float y2 = num * num - Mathf.Sqrt(num4);
		float num5 = num2 * magnitude;
		vector2.Normalize();
		Vector3 vector3 = new Vector3(vector2.x * num5, y2, vector2.z * num5).normalized * num;
		if (this.WouldHitWall(vector3, Mathf.Min(vector.magnitude, 5f)))
		{
			return HydroTurret.ShootResult.FAIL;
		}
		if (!this.RotateTowards(vector3))
		{
			return HydroTurret.ShootResult.PENDING;
		}
		GameObject gameObject = SRBehaviour.InstantiateActor(this.liquidPrefab, this.region.setId, this.spawnLoc.position, this.spawnLoc.rotation, false);
		gameObject.GetComponent<Rigidbody>().velocity = vector3;
		float x = gameObject.transform.localScale.x;
		float fromValue = x * 0.2f;
		gameObject.transform.DOScale(x, 0.1f).From(fromValue, true).SetEase(Ease.Linear);
		if (this.shootCue != null)
		{
			SECTR_AudioSystem.Play(this.shootCue, this.spawnLoc.position, false);
		}
		return HydroTurret.ShootResult.SHOT;
	}

	// Token: 0x06002622 RID: 9762 RVA: 0x00092320 File Offset: 0x00090520
	private bool RotateTowards(Vector3 dir)
	{
		Quaternion quaternion = Quaternion.LookRotation(dir, Vector3.up);
		this.toRotate.rotation = Quaternion.RotateTowards(this.toRotate.rotation, quaternion, 180f * Time.deltaTime);
		return Quaternion.Angle(this.toRotate.rotation, quaternion) < Mathf.Epsilon;
	}

	// Token: 0x06002623 RID: 9763 RVA: 0x00092378 File Offset: 0x00090578
	private bool WouldHitWall(Vector3 dir, float maxDist)
	{
		LayerMask mask = 268435457;
		return Physics.Raycast(this.toRotate.position, dir, maxDist, mask, QueryTriggerInteraction.Ignore);
	}

	// Token: 0x04002570 RID: 9584
	public Identifiable.Id liquidId = Identifiable.Id.WATER_LIQUID;

	// Token: 0x04002571 RID: 9585
	public Transform spawnLoc;

	// Token: 0x04002572 RID: 9586
	public FilteredTrackCollisions tracker;

	// Token: 0x04002573 RID: 9587
	public Transform toRotate;

	// Token: 0x04002574 RID: 9588
	public SECTR_AudioCue shootCue;

	// Token: 0x04002575 RID: 9589
	public SECTR_AudioCue rotateCue;

	// Token: 0x04002576 RID: 9590
	[Tooltip("Delay in game mins between shots")]
	public float shootDelay = 2f;

	// Token: 0x04002577 RID: 9591
	[Tooltip("Delay in game mins before we can shoot if we need to retarget.")]
	public float retargetDelay = 0.5f;

	// Token: 0x04002578 RID: 9592
	[Tooltip("Delay in game mins before we can shoot if another turret on our gadget shot.")]
	public float localShotDelay = 0.2f;

	// Token: 0x04002579 RID: 9593
	private TimeDirector timeDir;

	// Token: 0x0400257A RID: 9594
	private Region region;

	// Token: 0x0400257B RID: 9595
	private GameObject liquidPrefab;

	// Token: 0x0400257C RID: 9596
	private double nextShootTime;

	// Token: 0x0400257D RID: 9597
	private GameObject currTarget;

	// Token: 0x0400257E RID: 9598
	private WaitForChargeup waiter;

	// Token: 0x0400257F RID: 9599
	private List<HydroTurret> otherTurrets = new List<HydroTurret>();

	// Token: 0x04002580 RID: 9600
	private const float SHOOT_SCALE_UP_TIME = 0.1f;

	// Token: 0x04002581 RID: 9601
	private const float SHOOT_SCALE_FACTOR = 0.2f;

	// Token: 0x04002582 RID: 9602
	private const float MAX_ROT_PER_SEC = 180f;

	// Token: 0x04002583 RID: 9603
	private const float MAX_WALL_CHECK_DIST = 5f;

	// Token: 0x02000720 RID: 1824
	private enum ShootResult
	{
		// Token: 0x04002585 RID: 9605
		PENDING,
		// Token: 0x04002586 RID: 9606
		SHOT,
		// Token: 0x04002587 RID: 9607
		FAIL
	}
}
