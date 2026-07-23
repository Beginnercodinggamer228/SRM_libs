using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200071E RID: 1822
public class HydroShower : SRBehaviour, GadgetInteractor
{
	// Token: 0x06002616 RID: 9750 RVA: 0x00091D41 File Offset: 0x0008FF41
	public void Awake()
	{
		this.waiter = base.GetComponentInParent<WaitForChargeup>();
		this.tracker.SetFilter(new Predicate<GameObject>(this.IsTarr));
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
	}

	// Token: 0x06002617 RID: 9751 RVA: 0x00091D78 File Offset: 0x0008FF78
	public void Update()
	{
		if (this.waiter.IsWaiting())
		{
			return;
		}
		if (this.timeDir.HasReached(this.waterUntil))
		{
			this.showerFX.SetActive(false);
			this.nextWaterPulse = double.PositiveInfinity;
			return;
		}
		if (this.timeDir.HasReached(this.nextWaterPulse))
		{
			this.DoWaterPulse();
			this.nextWaterPulse = this.timeDir.HoursFromNow(this.pulseDelay * 0.016666668f);
		}
		this.showerFX.SetActive(true);
	}

	// Token: 0x06002618 RID: 9752 RVA: 0x00091E04 File Offset: 0x00090004
	public void OnInteract()
	{
		if (this.timeDir.HasReached(this.nextShower))
		{
			this.waterUntil = this.timeDir.HoursFromNow(this.waterDuration * 0.016666668f);
			this.nextWaterPulse = 0.0;
			this.nextShower = this.timeDir.HoursFromNow(this.timeBetweenShowers * 0.016666668f);
		}
	}

	// Token: 0x06002619 RID: 9753 RVA: 0x00013CC5 File Offset: 0x00011EC5
	public bool CanInteract()
	{
		return true;
	}

	// Token: 0x0600261A RID: 9754 RVA: 0x00091E6D File Offset: 0x0009006D
	private bool IsTarr(GameObject gameObj)
	{
		return Identifiable.IsTarr(Identifiable.GetId(gameObj));
	}

	// Token: 0x0600261B RID: 9755 RVA: 0x00091E7C File Offset: 0x0009007C
	private void DoWaterPulse()
	{
		HashSet<GameObject> hashSet = this.tracker.CurrColliders();
		if (hashSet.Count > 0)
		{
			HashSet<LiquidConsumer> hashSet2 = new HashSet<LiquidConsumer>();
			foreach (GameObject gameObject in hashSet)
			{
				if (gameObject != null)
				{
					foreach (LiquidConsumer item in gameObject.GetComponentsInParent<LiquidConsumer>())
					{
						hashSet2.Add(item);
					}
				}
			}
			foreach (LiquidConsumer liquidConsumer in hashSet2)
			{
				liquidConsumer.AddLiquid(this.liquidId, this.wateringUnitsPerPulse);
			}
		}
	}

	// Token: 0x04002564 RID: 9572
	public Identifiable.Id liquidId = Identifiable.Id.WATER_LIQUID;

	// Token: 0x04002565 RID: 9573
	public FilteredTrackCollisions tracker;

	// Token: 0x04002566 RID: 9574
	public GameObject showerFX;

	// Token: 0x04002567 RID: 9575
	[Tooltip("How long we should keep the shower going in game-minutes")]
	public float waterDuration = 5f;

	// Token: 0x04002568 RID: 9576
	[Tooltip("How long between activations in game-minutes")]
	public float timeBetweenShowers = 60f;

	// Token: 0x04002569 RID: 9577
	public float wateringUnitsPerPulse = 0.5f;

	// Token: 0x0400256A RID: 9578
	[Tooltip("How long between pulses in game-minutes")]
	public float pulseDelay = 0.5f;

	// Token: 0x0400256B RID: 9579
	private double waterUntil;

	// Token: 0x0400256C RID: 9580
	private double nextWaterPulse;

	// Token: 0x0400256D RID: 9581
	private double nextShower;

	// Token: 0x0400256E RID: 9582
	private TimeDirector timeDir;

	// Token: 0x0400256F RID: 9583
	private WaitForChargeup waiter;
}
