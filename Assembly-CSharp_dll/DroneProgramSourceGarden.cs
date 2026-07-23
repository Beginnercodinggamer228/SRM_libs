using System;
using System.Collections.Generic;
using System.Linq;

// Token: 0x02000198 RID: 408
public class DroneProgramSourceGarden : DroneProgramSourceLandPlot
{
	// Token: 0x060008AF RID: 2223 RVA: 0x00028445 File Offset: 0x00026645
	protected override LandPlot.Id GetLandPlotID()
	{
		return LandPlot.Id.GARDEN;
	}

	// Token: 0x060008B0 RID: 2224 RVA: 0x00028448 File Offset: 0x00026648
	public override IEnumerable<DroneFastForwarder.GatherGroup> GetFastForwardGroups(double endTime)
	{
		return base.GetFastForwardGroups(endTime).Concat(from g in (from m in this.drone.network.Plots
		where m.plot.typeId == LandPlot.Id.GARDEN
		select m).SelectMany((DroneNetwork.LandPlotMetadata m) => m.plot.GetComponentsInChildren<SpawnResource>()).SelectMany((SpawnResource r) => r.GetFastForwardGroups(endTime))
		where this.predicate(g.id)
		select g);
	}

	// Token: 0x060008B1 RID: 2225 RVA: 0x000284F4 File Offset: 0x000266F4
	protected override float GetPickupRadius()
	{
		float num = base.GetPickupRadius();
		if (Identifiable.IsFruit(this.source.id))
		{
			ResourceCycle component = this.source.GetComponent<ResourceCycle>();
			if (component != null && component.GetState() == ResourceCycle.State.RIPE)
			{
				num *= 1.5f;
			}
		}
		return num;
	}
}
