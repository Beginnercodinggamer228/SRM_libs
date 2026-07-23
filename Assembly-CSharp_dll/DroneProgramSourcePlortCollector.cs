using System;
using System.Collections.Generic;
using System.Linq;

// Token: 0x020001A3 RID: 419
public class DroneProgramSourcePlortCollector : DroneProgramSourceSiloStorage
{
	// Token: 0x060008DD RID: 2269 RVA: 0x00028994 File Offset: 0x00026B94
	protected override IEnumerable<DroneNetwork.StorageMetadata> GetSources(Predicate<Identifiable.Id> predicate)
	{
		return from s in this.drone.network.Plots.SelectMany((DroneNetwork.LandPlotMetadata m) => m.plortCollectors)
		where predicate(s.id)
		orderby s.count descending
		select s;
	}
}
