using System;
using System.Collections.Generic;
using System.Linq;

// Token: 0x0200016D RID: 365
public class DroneProgramDestinationFeeder : DroneProgramDestinationSiloStorage<DroneProgramDestinationFeeder.Destination>
{
	// Token: 0x060007D8 RID: 2008 RVA: 0x00026614 File Offset: 0x00024814
	protected override IEnumerable<DroneProgramDestinationFeeder.Destination> GetDestinations(Identifiable.Id id, bool overflow)
	{
		Func<DroneNetwork.StorageMetadata, bool> <>9__2;
		return from m in this.drone.network.Plots.SelectMany(delegate(DroneNetwork.LandPlotMetadata m)
		{
			IEnumerable<DroneNetwork.StorageMetadata> feeders = m.feeders;
			Func<DroneNetwork.StorageMetadata, bool> predicate;
			if ((predicate = <>9__2) == null)
			{
				predicate = (<>9__2 = ((DroneNetwork.StorageMetadata s) => s.storage.CanAccept(id, s.index, overflow)));
			}
			return from s in feeders.Where(predicate)
			select new DroneProgramDestinationFeeder.Destination(s, m, id);
		})
		where m.seeded || m.corral.anyEat
		select m;
	}

	// Token: 0x060007D9 RID: 2009 RVA: 0x0002667A File Offset: 0x0002487A
	protected override IEnumerable<DroneProgramDestinationFeeder.Destination> Prioritize(IEnumerable<DroneProgramDestinationFeeder.Destination> destinations)
	{
		return destinations.OrderBy((DroneProgramDestinationFeeder.Destination d) => d, DroneProgramDestinationFeeder.Destination.Comparer.Default);
	}

	// Token: 0x0200016E RID: 366
	public class Destination : DroneNetwork.StorageMetadata
	{
		// Token: 0x060007DB RID: 2011 RVA: 0x000266AE File Offset: 0x000248AE
		public Destination(DroneNetwork.StorageMetadata storage, DroneNetwork.LandPlotMetadata metadata, Identifiable.Id id) : base(storage)
		{
			this.corral = new DroneProgramDestinationCorral.Destination(metadata, id);
			this.seeded = (storage.id == id);
		}

		// Token: 0x04000715 RID: 1813
		public readonly DroneProgramDestinationCorral.Destination corral;

		// Token: 0x04000716 RID: 1814
		public readonly bool seeded;

		// Token: 0x0200016F RID: 367
		public class Comparer : SRComparer<DroneProgramDestinationFeeder.Destination>
		{
			// Token: 0x04000717 RID: 1815
			public new static Comparer<DroneProgramDestinationFeeder.Destination> Default = from m in new DroneProgramDestinationFeeder.Destination.Comparer()
			orderby m.seeded descending
			orderby m.count
			orderby m.corral.anyFavorite descending
			orderby m.corral.available
			select m;
		}
	}
}
