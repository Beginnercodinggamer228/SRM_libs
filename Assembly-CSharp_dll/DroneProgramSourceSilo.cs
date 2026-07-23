using System;
using System.Collections.Generic;
using System.Linq;

// Token: 0x020001A7 RID: 423
public class DroneProgramSourceSilo : DroneProgramSourceSiloStorage
{
	// Token: 0x060008E8 RID: 2280 RVA: 0x00028A44 File Offset: 0x00026C44
	protected override IEnumerable<DroneNetwork.StorageMetadata> GetSources(Predicate<Identifiable.Id> predicate)
	{
		return from s in this.drone.network.Plots.SelectMany((DroneNetwork.LandPlotMetadata m) => m.silos)
		where predicate(s.id)
		orderby s.count
		select s;
	}
}
