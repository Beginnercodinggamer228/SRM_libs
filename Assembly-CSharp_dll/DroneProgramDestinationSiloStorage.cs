using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000184 RID: 388
public abstract class DroneProgramDestinationSiloStorage<T> : DroneProgramDestination<T> where T : DroneNetwork.StorageMetadata
{
	// Token: 0x06000844 RID: 2116 RVA: 0x000273BC File Offset: 0x000255BC
	public override void Deselected()
	{
		base.Deselected();
		if (this.orientation != null)
		{
			this.orientation.Dispose();
			this.orientation = null;
		}
	}

	// Token: 0x06000845 RID: 2117 RVA: 0x000273E0 File Offset: 0x000255E0
	public override int GetAvailableSpace(Identifiable.Id id)
	{
		return this.GetDestinations(id, false).Cast<DroneNetwork.StorageMetadata>().Aggregate(0, (int accum, DroneNetwork.StorageMetadata d) => accum + d.GetAvailableSpace(id));
	}

	// Token: 0x06000846 RID: 2118 RVA: 0x0002741E File Offset: 0x0002561E
	public override bool HasAvailableSpace(Identifiable.Id id)
	{
		return this.GetDestinations(id, false).Any<T>();
	}

	// Token: 0x06000847 RID: 2119 RVA: 0x0002742D File Offset: 0x0002562D
	protected override bool CanCancel()
	{
		return base.CanCancel() || this.destination.CanCancel();
	}

	// Token: 0x06000848 RID: 2120 RVA: 0x00027449 File Offset: 0x00025649
	protected override IEnumerable<DroneProgram.Orientation> GetTargetOrientations()
	{
		this.orientation = base.GetTargetOrientation_Catcher(this.destination.catcher.gameObject);
		yield return this.orientation.orientation;
		yield break;
	}

	// Token: 0x06000849 RID: 2121 RVA: 0x00027459 File Offset: 0x00025659
	protected override Vector3 GetTargetPosition()
	{
		return this.destination.catcher.transform.position;
	}

	// Token: 0x0600084A RID: 2122 RVA: 0x00027478 File Offset: 0x00025678
	protected override bool OnAction_Deposit(bool overflow)
	{
		if (!this.timeDirector.HasReached(this.time))
		{
			return false;
		}
		Identifiable.Id slotName = this.drone.ammo.GetSlotName();
		if (this.destination.Increment(slotName, overflow, 1))
		{
			this.drone.ammo.Decrement(slotName, 1);
			this.time = this.timeDirector.HoursFromNow(0.0016666668f);
			return !overflow && this.destination.IsFull();
		}
		return true;
	}

	// Token: 0x0600084B RID: 2123 RVA: 0x00027500 File Offset: 0x00025700
	public override DroneProgramDestination.FastForward_Response FastForward(Identifiable.Id id, bool overflow, double endTime, int maxFastForward)
	{
		DroneNetwork.StorageMetadata storageMetadata = this.Prioritize(this.GetDestinations(id, overflow)).First<T>();
		maxFastForward = (overflow ? maxFastForward : Mathf.Min(maxFastForward, storageMetadata.GetAvailableSpace(id)));
		storageMetadata.Increment(id, overflow, maxFastForward);
		return new DroneProgramDestination.FastForward_Response
		{
			deposits = maxFastForward
		};
	}

	// Token: 0x0400074B RID: 1867
	private double time;

	// Token: 0x0400074C RID: 1868
	private DroneSubbehaviour.CatcherOrientation orientation;
}
