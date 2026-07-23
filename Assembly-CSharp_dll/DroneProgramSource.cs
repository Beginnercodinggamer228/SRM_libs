using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000187 RID: 391
public abstract class DroneProgramSource : DroneProgram
{
	// Token: 0x06000857 RID: 2135
	public abstract IEnumerable<DroneFastForwarder.GatherGroup> GetFastForwardGroups(double endTime);

	// Token: 0x17000116 RID: 278
	// (get) Token: 0x06000858 RID: 2136 RVA: 0x0002764B File Offset: 0x0002584B
	protected override DroneAnimator.Id animation
	{
		get
		{
			return DroneAnimator.Id.GATHER;
		}
	}

	// Token: 0x17000117 RID: 279
	// (get) Token: 0x06000859 RID: 2137 RVA: 0x0002764E File Offset: 0x0002584E
	protected override DroneAnimatorState.Id animationStateBegin
	{
		get
		{
			return DroneAnimatorState.Id.GATHER_BEGIN;
		}
	}

	// Token: 0x17000118 RID: 280
	// (get) Token: 0x0600085A RID: 2138 RVA: 0x00027652 File Offset: 0x00025852
	protected override DroneAnimatorState.Id animationStateEnd
	{
		get
		{
			return DroneAnimatorState.Id.GATHER_END;
		}
	}

	// Token: 0x0600085B RID: 2139 RVA: 0x00027658 File Offset: 0x00025858
	protected int GetAvailableDestinationSpace(Identifiable.Id id)
	{
		return this.destinations.Aggregate(0, (int c, DroneProgramDestination d) => c + d.GetAvailableSpace(id));
	}

	// Token: 0x0600085C RID: 2140 RVA: 0x0002768C File Offset: 0x0002588C
	protected bool HasAvailableDestinationSpace(Identifiable.Id id)
	{
		return this.destinations.Any((DroneProgramDestination d) => d.HasAvailableSpace(id));
	}

	// Token: 0x0600085D RID: 2141 RVA: 0x000276C0 File Offset: 0x000258C0
	protected bool HasAvailableDestinationSpace(Identifiable.Id id, int minimum)
	{
		foreach (DroneProgramDestination droneProgramDestination in this.destinations)
		{
			int availableSpace = droneProgramDestination.GetAvailableSpace(id);
			if (availableSpace >= minimum)
			{
				return true;
			}
			minimum -= availableSpace;
		}
		return false;
	}

	// Token: 0x04000752 RID: 1874
	public static HashSet<GameObject> BLACKLIST = new HashSet<GameObject>();

	// Token: 0x04000753 RID: 1875
	public Predicate<Identifiable.Id> predicate = (Identifiable.Id id) => false;

	// Token: 0x04000754 RID: 1876
	public IEnumerable<DroneProgramDestination> destinations = Enumerable.Empty<DroneProgramDestination>();
}
