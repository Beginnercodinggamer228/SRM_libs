using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003BF RID: 959
public class DestroyOutsideHoursOfDay : SRBehaviour, CaveTrigger.Listener
{
	// Token: 0x06001409 RID: 5129 RVA: 0x0004D966 File Offset: 0x0004BB66
	public void Awake()
	{
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.StartShutdownClock();
	}

	// Token: 0x0600140A RID: 5130 RVA: 0x0004D97E File Offset: 0x0004BB7E
	public void OnEnable()
	{
		this.waitForPhysicsUpdate = true;
	}

	// Token: 0x0600140B RID: 5131 RVA: 0x0004D987 File Offset: 0x0004BB87
	public void FixedUpdate()
	{
		this.waitForPhysicsUpdate = false;
	}

	// Token: 0x0600140C RID: 5132 RVA: 0x0004D990 File Offset: 0x0004BB90
	public void Update()
	{
		if (this.waitForPhysicsUpdate)
		{
			return;
		}
		if (this.timeDir.HasReached(this.shutdownAt))
		{
			if (this.destroyFX != null)
			{
				SRBehaviour.SpawnAndPlayFX(this.destroyFX, base.transform.position, base.transform.rotation);
			}
			Destroyer.DestroyActor(base.gameObject, "DestroyOutsideHoursOfDay.Update", false);
		}
		if (this.caves.Count > 0)
		{
			UnityWorkarounds.SafeRemoveAllNulls<GameObject>(this.caves);
			if (this.caves.Count == 0)
			{
				this.StartShutdownClock();
			}
		}
	}

	// Token: 0x0600140D RID: 5133 RVA: 0x0004DA26 File Offset: 0x0004BC26
	public void OnCaveEnter(GameObject caveObj, bool affectLighting, AmbianceDirector.Zone caveZone)
	{
		if (this.caves.Count == 0)
		{
			this.StopShutdownClock();
		}
		this.caves.Add(caveObj);
	}

	// Token: 0x0600140E RID: 5134 RVA: 0x0004DA48 File Offset: 0x0004BC48
	public void OnCaveExit(GameObject caveObj, bool affectLighting, AmbianceDirector.Zone caveZone)
	{
		this.caves.Remove(caveObj);
		if (this.caves.Count == 0)
		{
			this.StartShutdownClock();
		}
	}

	// Token: 0x0600140F RID: 5135 RVA: 0x0004DA6C File Offset: 0x0004BC6C
	private void StartShutdownClock()
	{
		float num = this.timeDir.CurrHourOrStart();
		float inRange = Randoms.SHARED.GetInRange(this.minEndureHoursOutsideWindow, this.maxEndureHoursOutsideWindow);
		float num2 = num + inRange;
		if (num2 > 24f)
		{
			num2 %= 24f;
		}
		if (this.endHour >= this.startHour)
		{
			if ((num < this.startHour || num > this.endHour) && (num2 < this.startHour || num2 > this.endHour))
			{
				this.shutdownAt = this.timeDir.HoursFromNowOrStart(inRange);
				return;
			}
			this.shutdownAt = this.timeDir.GetNextHour(this.endHour) + (double)(inRange * 3600f);
			return;
		}
		else
		{
			if (num > this.endHour && num < this.startHour && num2 > this.endHour && num2 < this.startHour)
			{
				this.shutdownAt = this.timeDir.HoursFromNowOrStart(inRange);
				return;
			}
			this.shutdownAt = this.timeDir.GetNextHour(this.endHour) + (double)(inRange * 3600f);
			return;
		}
	}

	// Token: 0x06001410 RID: 5136 RVA: 0x0004DB6D File Offset: 0x0004BD6D
	private void StopShutdownClock()
	{
		this.shutdownAt = double.PositiveInfinity;
	}

	// Token: 0x040012BD RID: 4797
	public float startHour = 18f;

	// Token: 0x040012BE RID: 4798
	public float endHour = 6f;

	// Token: 0x040012BF RID: 4799
	public float minEndureHoursOutsideWindow = 0.5f;

	// Token: 0x040012C0 RID: 4800
	public float maxEndureHoursOutsideWindow = 1.5f;

	// Token: 0x040012C1 RID: 4801
	public bool cavesPreventShutdown = true;

	// Token: 0x040012C2 RID: 4802
	public GameObject destroyFX;

	// Token: 0x040012C3 RID: 4803
	private TimeDirector timeDir;

	// Token: 0x040012C4 RID: 4804
	private double shutdownAt;

	// Token: 0x040012C5 RID: 4805
	private HashSet<GameObject> caves = new HashSet<GameObject>();

	// Token: 0x040012C6 RID: 4806
	private bool waitForPhysicsUpdate;
}
