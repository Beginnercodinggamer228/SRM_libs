using System;
using System.Collections.Generic;
using System.Linq;

// Token: 0x02000181 RID: 385
public class DroneProgramDestinationSilo : DroneProgramDestinationSiloStorage<DroneNetwork.StorageMetadata>
{
	// Token: 0x0600083B RID: 2107 RVA: 0x000272F4 File Offset: 0x000254F4
	protected override IEnumerable<DroneNetwork.StorageMetadata> GetDestinations(Identifiable.Id id, bool overflow)
	{
		return from s in this.drone.network.Plots.SelectMany((DroneNetwork.LandPlotMetadata m) => m.silos)
		where s.storage.CanAccept(id, s.index, overflow)
		select s;
	}

	// Token: 0x0600083C RID: 2108 RVA: 0x0002735A File Offset: 0x0002555A
	protected override IEnumerable<DroneNetwork.StorageMetadata> Prioritize(IEnumerable<DroneNetwork.StorageMetadata> destinations)
	{
		return from s in destinations
		orderby s.count descending
		select s;
	}
}
