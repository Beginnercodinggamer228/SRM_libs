using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x0200018D RID: 397
public class DroneProgramSourceCoop : DroneProgramSourceLandPlot
{
	// Token: 0x06000877 RID: 2167 RVA: 0x00025EA6 File Offset: 0x000240A6
	protected override LandPlot.Id GetLandPlotID()
	{
		return LandPlot.Id.COOP;
	}

	// Token: 0x06000878 RID: 2168 RVA: 0x000279C0 File Offset: 0x00025BC0
	protected override IEnumerable<Identifiable> GetSources(Predicate<Identifiable.Id> predicate, DroneNetwork.LandPlotMetadata metadata)
	{
		TrackContainedIdentifiables container = metadata.trackers.First<TrackContainedIdentifiables>();
		return from s in (from pair in container.GetAllTracked()
		where predicate(pair.Key)
		select pair).SelectMany((KeyValuePair<Identifiable.Id, HashSet<Identifiable>> pair) => pair.Value.Skip(DroneProgramSourceCoop.GetSkipCount(pair.Key, container)))
		where this.SourcePredicate(metadata, s)
		select s;
	}

	// Token: 0x06000879 RID: 2169 RVA: 0x00027A3C File Offset: 0x00025C3C
	protected override int GetMaxPickup(Identifiable.Id id)
	{
		int b = int.MaxValue;
		if (this.currentLandPlot != null && Identifiable.MEAT_CLASS.Contains(id))
		{
			TrackContainedIdentifiables trackContainedIdentifiables = this.currentLandPlot.trackers.First<TrackContainedIdentifiables>();
			b = trackContainedIdentifiables.Count(id) - DroneProgramSourceCoop.GetSkipCount(id, trackContainedIdentifiables);
		}
		return Mathf.Min(base.GetMaxPickup(id), b);
	}

	// Token: 0x0600087A RID: 2170 RVA: 0x00027A94 File Offset: 0x00025C94
	private static int GetSkipCount(Identifiable.Id id, TrackContainedIdentifiables container)
	{
		if (id <= Identifiable.Id.ROOSTER)
		{
			if (id == Identifiable.Id.HEN)
			{
				int num = container.Count(Identifiable.Id.PAINTED_HEN);
				int num2 = container.Count(Identifiable.Id.BRIAR_HEN);
				int num3 = container.Count(Identifiable.Id.STONY_HEN);
				return Mathf.Max(0, 4 - num - num2 - num3);
			}
			if (id == Identifiable.Id.ROOSTER)
			{
				return 2;
			}
		}
		else
		{
			if (id == Identifiable.Id.STONY_HEN)
			{
				int num4 = container.Count(Identifiable.Id.PAINTED_HEN);
				int num5 = container.Count(Identifiable.Id.BRIAR_HEN);
				return Math.Max(1, 4 - num4 - num5);
			}
			if (id == Identifiable.Id.BRIAR_HEN)
			{
				int num6 = container.Count(Identifiable.Id.PAINTED_HEN);
				int num7 = Mathf.Min(1, container.Count(Identifiable.Id.STONY_HEN));
				return Math.Max(1, 4 - num6 - num7);
			}
			if (id == Identifiable.Id.PAINTED_HEN)
			{
				int num8 = Mathf.Min(1, container.Count(Identifiable.Id.BRIAR_HEN));
				int num9 = Mathf.Min(1, container.Count(Identifiable.Id.STONY_HEN));
				return Math.Max(1, 4 - num8 - num9);
			}
		}
		return 0;
	}

	// Token: 0x0400075E RID: 1886
	private const int SKIP_ROOSTER = 2;

	// Token: 0x0400075F RID: 1887
	private const int SKIP_HEN_RARE = 1;

	// Token: 0x04000760 RID: 1888
	private const int SKIP_HEN = 4;
}
