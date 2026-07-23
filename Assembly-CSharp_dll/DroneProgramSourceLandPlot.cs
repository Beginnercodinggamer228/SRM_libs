using System;
using System.Collections.Generic;
using System.Linq;

// Token: 0x0200019C RID: 412
public abstract class DroneProgramSourceLandPlot : DroneProgramSourceDynamic
{
	// Token: 0x060008BD RID: 2237 RVA: 0x00028593 File Offset: 0x00026793
	public override void Awake()
	{
		base.Awake();
		base.plexer.onSubbehaviourSelected += this.OnDroneSubbehaviourSelected;
	}

	// Token: 0x060008BE RID: 2238 RVA: 0x000285B2 File Offset: 0x000267B2
	public override void OnDestroy()
	{
		base.OnDestroy();
		base.plexer.onSubbehaviourSelected -= this.OnDroneSubbehaviourSelected;
	}

	// Token: 0x060008BF RID: 2239 RVA: 0x000285D4 File Offset: 0x000267D4
	public override bool Relevancy()
	{
		if (base.Relevancy())
		{
			this.currentLandPlot = this.drone.network.GetContaining(this.source);
			if (this.currentLandPlot != null)
			{
				this.grayListHashCode = new int?(this.currentLandPlot.GetHashCode());
				DroneProgramSourceLandPlot.GRAYLIST.Increment(this.grayListHashCode.Value);
			}
			return true;
		}
		return false;
	}

	// Token: 0x060008C0 RID: 2240 RVA: 0x0002863C File Offset: 0x0002683C
	public override void Deselected()
	{
		base.Deselected();
		if (this.grayListHashCode != null)
		{
			DroneProgramSourceLandPlot.GRAYLIST.Decrement(this.grayListHashCode.Value);
			this.grayListHashCode = null;
		}
	}

	// Token: 0x060008C1 RID: 2241 RVA: 0x00028673 File Offset: 0x00026873
	protected override bool CanCancel()
	{
		return base.CanCancel() || this.currentLandPlot == null || !this.currentLandPlot.Contains(this.source);
	}

	// Token: 0x060008C2 RID: 2242 RVA: 0x0002869C File Offset: 0x0002689C
	protected override IEnumerable<Identifiable> GetSources(Predicate<Identifiable.Id> predicate)
	{
		return (from m in this.drone.network.Plots
		where m.plot.typeId == this.GetLandPlotID()
		select new DroneProgramSourceLandPlot.Intermediate
		{
			metadata = m,
			sources = this.GetSources(predicate, m)
		}).OrderBy((DroneProgramSourceLandPlot.Intermediate o) => o, from o in new DroneProgramSourceLandPlot.Intermediate.Comparer()
		orderby o.metadata == this.currentLandPlot descending
		orderby DroneProgramSourceLandPlot.GRAYLIST.ContainsKey(o.metadata.GetHashCode())
		orderby o.sources.Count<Identifiable>() descending
		select o).SelectMany((DroneProgramSourceLandPlot.Intermediate o) => o.sources);
	}

	// Token: 0x060008C3 RID: 2243 RVA: 0x00028798 File Offset: 0x00026998
	protected virtual IEnumerable<Identifiable> GetSources(Predicate<Identifiable.Id> predicate, DroneNetwork.LandPlotMetadata metadata)
	{
		Func<KeyValuePair<Identifiable.Id, HashSet<Identifiable>>, bool> <>9__1;
		Func<Identifiable, bool> <>9__3;
		return metadata.trackers.SelectMany(delegate(TrackContainedIdentifiables tracker)
		{
			IEnumerable<KeyValuePair<Identifiable.Id, HashSet<Identifiable>>> allTracked = tracker.GetAllTracked();
			Func<KeyValuePair<Identifiable.Id, HashSet<Identifiable>>, bool> predicate2;
			if ((predicate2 = <>9__1) == null)
			{
				predicate2 = (<>9__1 = ((KeyValuePair<Identifiable.Id, HashSet<Identifiable>> kv) => predicate(kv.Key)));
			}
			IEnumerable<Identifiable> source = allTracked.Where(predicate2).SelectMany((KeyValuePair<Identifiable.Id, HashSet<Identifiable>> kv) => kv.Value);
			Func<Identifiable, bool> predicate3;
			if ((predicate3 = <>9__3) == null)
			{
				predicate3 = (<>9__3 = ((Identifiable id) => this.SourcePredicate(metadata, id)));
			}
			return source.Where(predicate3);
		});
	}

	// Token: 0x060008C4 RID: 2244 RVA: 0x000287DC File Offset: 0x000269DC
	protected override bool SourcePredicate(DroneNetwork.LandPlotMetadata metadata, Identifiable source)
	{
		return base.SourcePredicate(metadata, source) && metadata != null && metadata.plot.typeId == this.GetLandPlotID();
	}

	// Token: 0x060008C5 RID: 2245 RVA: 0x00028800 File Offset: 0x00026A00
	private void OnDroneSubbehaviourSelected(DroneSubbehaviour subbehaviour)
	{
		if (subbehaviour != this && !(subbehaviour is DroneSubbehaviourIdle))
		{
			this.currentLandPlot = null;
		}
	}

	// Token: 0x060008C6 RID: 2246 RVA: 0x0002881A File Offset: 0x00026A1A
	protected override GardenDroneSubnetwork GetSubnetwork()
	{
		if (this.currentLandPlot == null)
		{
			return null;
		}
		return this.currentLandPlot.subnetwork;
	}

	// Token: 0x060008C7 RID: 2247
	protected abstract LandPlot.Id GetLandPlotID();

	// Token: 0x0400077E RID: 1918
	protected DroneNetwork.LandPlotMetadata currentLandPlot;

	// Token: 0x0400077F RID: 1919
	private static ReferenceCount<int> GRAYLIST = new ReferenceCount<int>();

	// Token: 0x04000780 RID: 1920
	private int? grayListHashCode;

	// Token: 0x0200019D RID: 413
	private class Intermediate
	{
		// Token: 0x04000781 RID: 1921
		public DroneNetwork.LandPlotMetadata metadata;

		// Token: 0x04000782 RID: 1922
		public IEnumerable<Identifiable> sources;

		// Token: 0x0200019E RID: 414
		public class Comparer : SRComparer<DroneProgramSourceLandPlot.Intermediate>
		{
		}
	}
}
