using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001C0 RID: 448
public class DroneSubbehaviourRest : DroneProgram
{
	// Token: 0x06000983 RID: 2435 RVA: 0x00013CC5 File Offset: 0x00011EC5
	public override bool Relevancy()
	{
		return true;
	}

	// Token: 0x06000984 RID: 2436 RVA: 0x0002A596 File Offset: 0x00028796
	public override void Selected()
	{
		base.Selected();
		this.rethinkTime = double.MaxValue;
	}

	// Token: 0x06000985 RID: 2437 RVA: 0x0002A5B0 File Offset: 0x000287B0
	public override void Deselected()
	{
		base.Deselected();
		this.drone.station.battery.onReset -= this.ForceRethink;
		this.drone.station.battery.onHasAnyChanged -= this.OnBatteryHasAnyChanged;
	}

	// Token: 0x06000986 RID: 2438 RVA: 0x0002A605 File Offset: 0x00028805
	public void ForceRethink()
	{
		this.rethinkTime = 0.0;
	}

	// Token: 0x06000987 RID: 2439 RVA: 0x0002A616 File Offset: 0x00028816
	protected override IEnumerable<DroneProgram.Orientation> GetTargetOrientations()
	{
		yield return this.drone.GetRestingOrientation();
		yield break;
	}

	// Token: 0x06000988 RID: 2440 RVA: 0x0002A626 File Offset: 0x00028826
	protected override Vector3 GetTargetPosition()
	{
		return this.drone.station.guideRest.position;
	}

	// Token: 0x06000989 RID: 2441 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
	protected override bool CanCancel()
	{
		return false;
	}

	// Token: 0x0600098A RID: 2442 RVA: 0x0002A640 File Offset: 0x00028840
	protected override void OnFirstAction()
	{
		base.OnFirstAction();
		this.rethinkTime = this.timeDirector.HoursFromNow(0.33333334f);
		this.drone.noClip.enabled = false;
		this.drone.onActiveCue.enabled = false;
		this.drone.station.battery.onReset += this.ForceRethink;
		this.drone.station.battery.onHasAnyChanged += this.OnBatteryHasAnyChanged;
		this.OnBatteryHasAnyChanged();
	}

	// Token: 0x0600098B RID: 2443 RVA: 0x0002A6D4 File Offset: 0x000288D4
	protected override bool OnAction()
	{
		if (this.timeDirector.HasReached(this.rethinkTime))
		{
			if (base.plexer.PickNextGatherBehaviour())
			{
				this.drone.onActiveCue.enabled = true;
				this.drone.station.animator.SetEnabled(true);
				return true;
			}
			this.rethinkTime = this.timeDirector.HoursFromNow(0.16666667f);
		}
		return false;
	}

	// Token: 0x0600098C RID: 2444 RVA: 0x0002A741 File Offset: 0x00028941
	private void OnBatteryHasAnyChanged()
	{
		this.drone.station.animator.SetEnabled(this.drone.station.battery.HasAny());
	}

	// Token: 0x1700012C RID: 300
	// (get) Token: 0x0600098D RID: 2445 RVA: 0x00028445 File Offset: 0x00026645
	protected override DroneAnimator.Id animation
	{
		get
		{
			return DroneAnimator.Id.REST;
		}
	}

	// Token: 0x1700012D RID: 301
	// (get) Token: 0x0600098E RID: 2446 RVA: 0x0002A76D File Offset: 0x0002896D
	protected override DroneAnimatorState.Id animationStateBegin
	{
		get
		{
			return DroneAnimatorState.Id.REST_BEGIN;
		}
	}

	// Token: 0x1700012E RID: 302
	// (get) Token: 0x0600098F RID: 2447 RVA: 0x0002A771 File Offset: 0x00028971
	protected override DroneAnimatorState.Id animationStateEnd
	{
		get
		{
			return DroneAnimatorState.Id.REST_END;
		}
	}

	// Token: 0x040007F5 RID: 2037
	private const float RETHINK_BASE_HOURS = 0.33333334f;

	// Token: 0x040007F6 RID: 2038
	private const float RETHINK_PERIOD_HOURS = 0.16666667f;

	// Token: 0x040007F7 RID: 2039
	private double rethinkTime;
}
