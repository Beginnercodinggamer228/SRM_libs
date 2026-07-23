using System;
using System.Collections.Generic;
using System.Linq;

// Token: 0x02000194 RID: 404
public class DroneProgramSourceElderCollector : DroneProgramSourceSiloStorage
{
	// Token: 0x060008A4 RID: 2212 RVA: 0x0002833C File Offset: 0x0002653C
	protected override IEnumerable<DroneNetwork.StorageMetadata> GetSources(Predicate<Identifiable.Id> predicate)
	{
		return from s in this.drone.network.Plots.SelectMany((DroneNetwork.LandPlotMetadata m) => m.elderCollectors)
		where predicate(s.id)
		orderby s.count descending
		select s;
	}
}
