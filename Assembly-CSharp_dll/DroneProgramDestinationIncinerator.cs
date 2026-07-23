using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000174 RID: 372
public class DroneProgramDestinationIncinerator : DroneProgramDestination<Incinerate>
{
	// Token: 0x060007ED RID: 2029 RVA: 0x0002683C File Offset: 0x00024A3C
	public override int GetAvailableSpace(Identifiable.Id id)
	{
		return this.GetDestinations(id, false).Aggregate(0, (int agg, Incinerate i) => agg + i.GetAshSpace());
	}

	// Token: 0x060007EE RID: 2030 RVA: 0x0002686B File Offset: 0x00024A6B
	public override bool HasAvailableSpace(Identifiable.Id id)
	{
		return this.GetDestinations(id, false).Any((Incinerate i) => i.GetAshSpace() > 0);
	}

	// Token: 0x060007EF RID: 2031 RVA: 0x0002689C File Offset: 0x00024A9C
	protected override IEnumerable<Incinerate> GetDestinations(Identifiable.Id id, bool overflow)
	{
		return from i in this.drone.network.Plots.SelectMany((DroneNetwork.LandPlotMetadata i) => i.incinerators)
		where i.GetAshSpace() > 0
		select i;
	}

	// Token: 0x060007F0 RID: 2032 RVA: 0x00026904 File Offset: 0x00024B04
	protected override IEnumerable<Incinerate> Prioritize(IEnumerable<Incinerate> destinations)
	{
		return destinations.OrderBy((Incinerate d) => d, from i in new DroneProgramDestinationIncinerator.Comparer()
		orderby i.GetAshSpace() descending
		orderby (i.transform.position - this.drone.transform.position).sqrMagnitude
		select i);
	}

	// Token: 0x060007F1 RID: 2033 RVA: 0x00026970 File Offset: 0x00024B70
	protected override IEnumerable<DroneProgram.Orientation> GetTargetOrientations()
	{
		return base.GetTargetOrientations_Gather(this.destination.gameObject, new DroneSubbehaviour.GatherConfig
		{
			fallbackOffset = Vector3.forward,
			distanceHorizontal = 2.5f
		});
	}

	// Token: 0x060007F2 RID: 2034 RVA: 0x0002699E File Offset: 0x00024B9E
	protected override Vector3 GetTargetPosition()
	{
		return this.destination.transform.position;
	}

	// Token: 0x060007F3 RID: 2035 RVA: 0x000269B0 File Offset: 0x00024BB0
	protected override void OnFirstAction()
	{
		base.OnFirstAction();
		this.dropCount = this.destination.GetAshSpace();
	}

	// Token: 0x060007F4 RID: 2036 RVA: 0x000269CC File Offset: 0x00024BCC
	protected override bool OnAction_Deposit(bool overflow)
	{
		if ((this.dropCount > 0 || overflow) && this.timeDirector.HasReached(this.time))
		{
			Identifiable.Id slotName = this.drone.ammo.GetSlotName();
			this.time = this.timeDirector.HoursFromNow(0.0016666668f);
			this.drone.ammo.Decrement(slotName, 1);
			this.destination.ProcessIncinerateResults(slotName, 1, this.destination.transform.position + (this.drone.transform.position - this.destination.transform.position).normalized * PhysicsUtil.RadiusOfObject(this.destination.gameObject) * 0.25f, Quaternion.identity);
			this.dropCount--;
		}
		return !overflow && this.dropCount <= 0;
	}

	// Token: 0x060007F5 RID: 2037 RVA: 0x00026ACC File Offset: 0x00024CCC
	public override DroneProgramDestination.FastForward_Response FastForward(Identifiable.Id id, bool overflow, double endTime, int maxFastForward)
	{
		Incinerate incinerate = this.Prioritize(this.GetDestinations(id, overflow)).First<Incinerate>();
		maxFastForward = (overflow ? maxFastForward : Mathf.Min(maxFastForward, incinerate.GetAshSpace()));
		incinerate.ProcessIncinerateResults(id, maxFastForward);
		return new DroneProgramDestination.FastForward_Response
		{
			deposits = maxFastForward
		};
	}

	// Token: 0x04000721 RID: 1825
	private double time;

	// Token: 0x04000722 RID: 1826
	private int dropCount;

	// Token: 0x02000175 RID: 373
	private class Comparer : SRComparer<Incinerate>
	{
	}
}
