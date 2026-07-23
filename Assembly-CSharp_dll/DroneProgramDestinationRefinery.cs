using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x0200017C RID: 380
public class DroneProgramDestinationRefinery : DroneProgramDestination<SiloCatcher>
{
	// Token: 0x0600081A RID: 2074 RVA: 0x00026E8B File Offset: 0x0002508B
	public override void Awake()
	{
		base.Awake();
		this.gadgetDirector = SRSingleton<SceneContext>.Instance.GadgetDirector;
	}

	// Token: 0x0600081B RID: 2075 RVA: 0x00026EA3 File Offset: 0x000250A3
	public override void Deselected()
	{
		base.Deselected();
		if (this.orientation != null)
		{
			this.orientation.Dispose();
			this.orientation = null;
		}
	}

	// Token: 0x0600081C RID: 2076 RVA: 0x00026EC5 File Offset: 0x000250C5
	public override int GetAvailableSpace(Identifiable.Id id)
	{
		if (!this.GetDestinations(id, false).Any<SiloCatcher>())
		{
			return 0;
		}
		return this.gadgetDirector.GetRefinerySpaceAvailable(id);
	}

	// Token: 0x0600081D RID: 2077 RVA: 0x00026EE4 File Offset: 0x000250E4
	public override bool HasAvailableSpace(Identifiable.Id id)
	{
		return this.GetDestinations(id, false).Any<SiloCatcher>() && this.gadgetDirector.HasRefinerySpaceAvailable(id, 1);
	}

	// Token: 0x0600081E RID: 2078 RVA: 0x00026F04 File Offset: 0x00025104
	protected override IEnumerable<SiloCatcher> Prioritize(IEnumerable<SiloCatcher> destinations)
	{
		return destinations.OrderBy((SiloCatcher d) => d, from m in new DroneProgramDestinationRefinery.Comparer()
		orderby (m.transform.position - this.drone.transform.position).sqrMagnitude
		select m);
	}

	// Token: 0x0600081F RID: 2079 RVA: 0x00026F41 File Offset: 0x00025141
	protected override IEnumerable<SiloCatcher> GetDestinations(Identifiable.Id id, bool overflow)
	{
		if (overflow || this.gadgetDirector.HasRefinerySpaceAvailable(id, 1))
		{
			foreach (SiloCatcher siloCatcher in this.drone.network.RefineryCatchers)
			{
				yield return siloCatcher;
			}
			IEnumerator<SiloCatcher> enumerator = null;
		}
		yield break;
		yield break;
	}

	// Token: 0x06000820 RID: 2080 RVA: 0x00026F5F File Offset: 0x0002515F
	protected override IEnumerable<DroneProgram.Orientation> GetTargetOrientations()
	{
		this.orientation = base.GetTargetOrientation_Catcher(this.destination.gameObject);
		yield return this.orientation.orientation;
		yield break;
	}

	// Token: 0x06000821 RID: 2081 RVA: 0x00026F6F File Offset: 0x0002516F
	protected override Vector3 GetTargetPosition()
	{
		return this.destination.transform.position;
	}

	// Token: 0x06000822 RID: 2082 RVA: 0x00026F84 File Offset: 0x00025184
	protected override bool OnAction_Deposit(bool overflow)
	{
		if (!this.timeDirector.HasReached(this.time))
		{
			return false;
		}
		if (this.gadgetDirector.AddToRefinery(this.drone.ammo.GetSlotName(), 1, overflow) > 0)
		{
			Identifiable.Id id = this.drone.ammo.Pop();
			this.time = this.timeDirector.HoursFromNow(0.0016666668f);
			return !overflow && !this.gadgetDirector.HasRefinerySpaceAvailable(id, 1);
		}
		return true;
	}

	// Token: 0x06000823 RID: 2083 RVA: 0x00027004 File Offset: 0x00025204
	public override DroneProgramDestination.FastForward_Response FastForward(Identifiable.Id id, bool overflow, double endTime, int maxFastForward)
	{
		maxFastForward = this.gadgetDirector.AddToRefinery(id, maxFastForward, overflow);
		return new DroneProgramDestination.FastForward_Response
		{
			deposits = maxFastForward
		};
	}

	// Token: 0x04000734 RID: 1844
	private GadgetDirector gadgetDirector;

	// Token: 0x04000735 RID: 1845
	private DroneSubbehaviour.CatcherOrientation orientation;

	// Token: 0x04000736 RID: 1846
	private double time;

	// Token: 0x0200017D RID: 381
	private class Comparer : SRComparer<SiloCatcher>
	{
	}
}
