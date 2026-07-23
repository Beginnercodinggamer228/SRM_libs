using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x020001AA RID: 426
public abstract class DroneProgramSourceSiloStorage : DroneProgramSource<DroneNetwork.StorageMetadata>
{
	// Token: 0x060008F0 RID: 2288 RVA: 0x00028AE6 File Offset: 0x00026CE6
	public override void Deselected()
	{
		base.Deselected();
		if (this.orientation != null)
		{
			this.orientation.Dispose();
			this.orientation = null;
		}
	}

	// Token: 0x060008F1 RID: 2289 RVA: 0x00028B08 File Offset: 0x00026D08
	public override IEnumerable<DroneFastForwarder.GatherGroup> GetFastForwardGroups(double endTime)
	{
		return (from s in this.GetSources(this.predicate)
		select new DroneFastForwarder.GatherGroup.Storage(s)).Cast<DroneFastForwarder.GatherGroup>();
	}

	// Token: 0x060008F2 RID: 2290 RVA: 0x00028B3F File Offset: 0x00026D3F
	protected override bool CanCancel()
	{
		return this.source.CanCancel();
	}

	// Token: 0x060008F3 RID: 2291 RVA: 0x00028B4C File Offset: 0x00026D4C
	protected override IEnumerable<DroneProgram.Orientation> GetTargetOrientations(DroneNetwork.StorageMetadata source)
	{
		this.orientation = base.GetTargetOrientation_Catcher(source.catcher.gameObject);
		yield return this.orientation.orientation;
		yield break;
	}

	// Token: 0x060008F4 RID: 2292 RVA: 0x00028B63 File Offset: 0x00026D63
	protected override Vector3 GetTargetPosition(DroneNetwork.StorageMetadata source)
	{
		return source.catcher.transform.position;
	}

	// Token: 0x060008F5 RID: 2293 RVA: 0x00028B75 File Offset: 0x00026D75
	protected override GameObject GetTargetGameObject(DroneNetwork.StorageMetadata source)
	{
		return source.catcher.gameObject;
	}

	// Token: 0x060008F6 RID: 2294 RVA: 0x00028B84 File Offset: 0x00026D84
	protected override void OnFirstAction()
	{
		base.OnFirstAction();
		int num = this.drone.ammo.GetSlotMaxCount() - this.drone.ammo.GetSlotCount();
		int count = this.source.count;
		int availableDestinationSpace = base.GetAvailableDestinationSpace(this.source.id);
		this.remaining = Mathf.Min(new int[]
		{
			num,
			count,
			availableDestinationSpace
		});
	}

	// Token: 0x060008F7 RID: 2295 RVA: 0x00028BF4 File Offset: 0x00026DF4
	protected override bool OnAction()
	{
		if (!this.timeDirector.HasReached(this.time))
		{
			return false;
		}
		if (this.remaining > 0 && this.drone.ammo.MaybeAddToSlot(this.source.id))
		{
			this.time = this.timeDirector.HoursFromNow(0.0016666668f);
			this.source.ammo.Decrement(this.source.id, 1);
			int num = this.remaining - 1;
			this.remaining = num;
			return num <= 0;
		}
		return true;
	}

	// Token: 0x060008F8 RID: 2296 RVA: 0x00028C87 File Offset: 0x00026E87
	protected override void OnPathGenerationFailed()
	{
		base.OnPathGenerationFailed();
		if (this.orientation != null)
		{
			this.orientation.Dispose();
			this.orientation = null;
		}
	}

	// Token: 0x04000798 RID: 1944
	private int remaining;

	// Token: 0x04000799 RID: 1945
	private double time;

	// Token: 0x0400079A RID: 1946
	private DroneSubbehaviour.CatcherOrientation orientation;
}
