using System;
using System.Collections.Generic;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000345 RID: 837
public class SlimeFeeder : SRBehaviour, LandPlotModel.Participant
{
	// Token: 0x06001192 RID: 4498 RVA: 0x000468BB File Offset: 0x00044ABB
	public void Awake()
	{
		this.storage = base.GetComponentInParent<SiloStorage>();
		this.region = base.GetComponentInParent<Region>();
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.SetFeederSpeedIcon(SlimeFeeder.FeedSpeed.Normal);
	}

	// Token: 0x06001193 RID: 4499 RVA: 0x000468EC File Offset: 0x00044AEC
	public void InitModel(LandPlotModel model)
	{
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
		model.nextFeedingTime = this.timeDir.HoursFromNowOrStart(this.CalcFeedingCycleHours(model.feederCycleSpeed));
	}

	// Token: 0x06001194 RID: 4500 RVA: 0x0004691B File Offset: 0x00044B1B
	public void SetModel(LandPlotModel model)
	{
		this.model = model;
		this.SetFeederSpeedIcon(model.feederCycleSpeed);
	}

	// Token: 0x06001195 RID: 4501 RVA: 0x00046930 File Offset: 0x00044B30
	public void Update()
	{
		this.UpdateToTime(this.timeDir.WorldTime());
		if (this.ShouldFeed())
		{
			this.ProcessFeedOperation(true);
		}
	}

	// Token: 0x06001196 RID: 4502 RVA: 0x00046954 File Offset: 0x00044B54
	public void UpdateToTime(double worldTime)
	{
		while (TimeUtil.HasReached(worldTime, this.model.nextFeedingTime))
		{
			this.model.remainingFeedOperations += this.itemsPerFeeding;
			this.model.nextFeedingTime += (double)(3600f * this.CalcFeedingCycleHours());
		}
	}

	// Token: 0x06001197 RID: 4503 RVA: 0x000469AD File Offset: 0x00044BAD
	private bool ShouldFeed()
	{
		return this.model.remainingFeedOperations > 0 && Time.time > this.nextEject && !this.region.Hibernated;
	}

	// Token: 0x06001198 RID: 4504 RVA: 0x000469DC File Offset: 0x00044BDC
	private void ProcessFeedOperation(bool ejectFood)
	{
		Ammo relevantAmmo = this.storage.GetRelevantAmmo();
		relevantAmmo.SetAmmoSlot(0);
		if (relevantAmmo.HasSelectedAmmo())
		{
			if (ejectFood)
			{
				this.EjectFood(relevantAmmo);
			}
			relevantAmmo.DecrementSelectedAmmo(1);
		}
		this.model.remainingFeedOperations = Math.Max(0, this.model.remainingFeedOperations - 1);
	}

	// Token: 0x06001199 RID: 4505 RVA: 0x00046A34 File Offset: 0x00044C34
	private void EjectFood(Ammo storageAmmo)
	{
		GameObject gameObject = SRBehaviour.InstantiateActor(storageAmmo.GetSelectedStored(), this.region.setId, base.transform.position + base.transform.forward * 0.5f, base.transform.rotation, false);
		Rigidbody component = gameObject.GetComponent<Rigidbody>();
		component.AddForce((base.transform.forward * 500f + UnityEngine.Random.insideUnitSphere * 400f) * component.mass);
		this.nextEject = Time.time + 0.5f;
		if (this.spawnFX != null)
		{
			SRBehaviour.SpawnAndPlayFX(this.spawnFX, gameObject.transform.position, base.transform.rotation);
		}
	}

	// Token: 0x0600119A RID: 4506 RVA: 0x00046B0B File Offset: 0x00044D0B
	public Identifiable.Id GetFoodId()
	{
		return this.storage.GetRelevantAmmo().GetSelectedId();
	}

	// Token: 0x0600119B RID: 4507 RVA: 0x00046B1D File Offset: 0x00044D1D
	public int GetFoodCount()
	{
		return this.storage.GetRelevantAmmo().GetSlotCount(0);
	}

	// Token: 0x0600119C RID: 4508 RVA: 0x00046B30 File Offset: 0x00044D30
	public int RemainingFeedOperationsFastForward()
	{
		return Mathf.Min(this.model.remainingFeedOperations, this.GetFoodCount());
	}

	// Token: 0x0600119D RID: 4509 RVA: 0x00046B48 File Offset: 0x00044D48
	public void ProcessFeedOperationFastForward()
	{
		this.ProcessFeedOperation(false);
	}

	// Token: 0x0600119E RID: 4510 RVA: 0x00046B51 File Offset: 0x00044D51
	public void SetFeederSpeed(SlimeFeeder.FeedSpeed speed)
	{
		this.model.feederCycleSpeed = speed;
		this.SetFeederSpeedIcon(speed);
	}

	// Token: 0x0600119F RID: 4511 RVA: 0x00046B68 File Offset: 0x00044D68
	private void SetFeederSpeedIcon(SlimeFeeder.FeedSpeed speed)
	{
		switch (speed)
		{
		case SlimeFeeder.FeedSpeed.Normal:
			this.feederIcon.sprite = this.normalIcon;
			return;
		case SlimeFeeder.FeedSpeed.Slow:
			this.feederIcon.sprite = this.slowIcon;
			return;
		case SlimeFeeder.FeedSpeed.Fast:
			this.feederIcon.sprite = this.fastIcon;
			return;
		default:
			return;
		}
	}

	// Token: 0x060011A0 RID: 4512 RVA: 0x00046BC0 File Offset: 0x00044DC0
	public void StepFeederSpeed()
	{
		switch (this.model.feederCycleSpeed)
		{
		case SlimeFeeder.FeedSpeed.Normal:
			this.SetFeederSpeed(SlimeFeeder.FeedSpeed.Fast);
			return;
		case SlimeFeeder.FeedSpeed.Slow:
			this.SetFeederSpeed(SlimeFeeder.FeedSpeed.Normal);
			return;
		case SlimeFeeder.FeedSpeed.Fast:
			this.SetFeederSpeed(SlimeFeeder.FeedSpeed.Slow);
			return;
		default:
			return;
		}
	}

	// Token: 0x060011A1 RID: 4513 RVA: 0x00046C03 File Offset: 0x00044E03
	public SlimeFeeder.FeedSpeed GetFeedingCycleSpeed()
	{
		return this.model.feederCycleSpeed;
	}

	// Token: 0x060011A2 RID: 4514 RVA: 0x00046C10 File Offset: 0x00044E10
	private float CalcFeedingCycleHours()
	{
		return this.CalcFeedingCycleHours(this.model.feederCycleSpeed);
	}

	// Token: 0x060011A3 RID: 4515 RVA: 0x00046C23 File Offset: 0x00044E23
	private float CalcFeedingCycleHours(SlimeFeeder.FeedSpeed speed)
	{
		return this.hoursByFeedSpeed[speed];
	}

	// Token: 0x040010CC RID: 4300
	public Dictionary<SlimeFeeder.FeedSpeed, float> hoursByFeedSpeed = new Dictionary<SlimeFeeder.FeedSpeed, float>
	{
		{
			SlimeFeeder.FeedSpeed.Slow,
			9f
		},
		{
			SlimeFeeder.FeedSpeed.Normal,
			6f
		},
		{
			SlimeFeeder.FeedSpeed.Fast,
			3f
		}
	};

	// Token: 0x040010CD RID: 4301
	public int itemsPerFeeding = 6;

	// Token: 0x040010CE RID: 4302
	public GameObject spawnFX;

	// Token: 0x040010CF RID: 4303
	public GameObject feederSpeedUI;

	// Token: 0x040010D0 RID: 4304
	public Image feederIcon;

	// Token: 0x040010D1 RID: 4305
	public Sprite slowIcon;

	// Token: 0x040010D2 RID: 4306
	public Sprite normalIcon;

	// Token: 0x040010D3 RID: 4307
	public Sprite fastIcon;

	// Token: 0x040010D4 RID: 4308
	private SiloStorage storage;

	// Token: 0x040010D5 RID: 4309
	private TimeDirector timeDir;

	// Token: 0x040010D6 RID: 4310
	private float nextEject;

	// Token: 0x040010D7 RID: 4311
	private Region region;

	// Token: 0x040010D8 RID: 4312
	private LandPlotModel model;

	// Token: 0x040010D9 RID: 4313
	private const float EJECT_DIST = 0.5f;

	// Token: 0x040010DA RID: 4314
	private const float EJECT_FORCE = 500f;

	// Token: 0x040010DB RID: 4315
	private const float EJECT_NOISE = 400f;

	// Token: 0x040010DC RID: 4316
	private const float EJECT_RATE = 0.5f;

	// Token: 0x02000346 RID: 838
	public enum FeedSpeed
	{
		// Token: 0x040010DE RID: 4318
		Normal,
		// Token: 0x040010DF RID: 4319
		Slow,
		// Token: 0x040010E0 RID: 4320
		Fast
	}
}
