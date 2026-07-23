using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000165 RID: 357
public class DroneProgramDestinationCorral : DroneProgramDestination<DroneProgramDestinationCorral.Destination>
{
	// Token: 0x060007AD RID: 1965 RVA: 0x0002605D File Offset: 0x0002425D
	public override int GetAvailableSpace(Identifiable.Id id)
	{
		return this.GetDestinations(id, false).Aggregate(0, (int cd, DroneProgramDestinationCorral.Destination m) => cd + m.available);
	}

	// Token: 0x060007AE RID: 1966 RVA: 0x0002608C File Offset: 0x0002428C
	public override bool HasAvailableSpace(Identifiable.Id id)
	{
		return this.GetDestinations(id, false).Any((DroneProgramDestinationCorral.Destination m) => m.available > 0);
	}

	// Token: 0x060007AF RID: 1967 RVA: 0x000260BC File Offset: 0x000242BC
	protected override IEnumerable<DroneProgramDestinationCorral.Destination> GetDestinations(Identifiable.Id id, bool overflow)
	{
		return from m in this.drone.network.Plots
		where m.plot.typeId == LandPlot.Id.CORRAL
		select new DroneProgramDestinationCorral.Destination(m, id) into m
		where m.anyEat && m.available >= 5
		select m;
	}

	// Token: 0x060007B0 RID: 1968 RVA: 0x0002613F File Offset: 0x0002433F
	protected override IEnumerable<DroneProgramDestinationCorral.Destination> Prioritize(IEnumerable<DroneProgramDestinationCorral.Destination> destinations)
	{
		return destinations.OrderBy((DroneProgramDestinationCorral.Destination d) => d, DroneProgramDestinationCorral.Destination.Comparer.Default);
	}

	// Token: 0x060007B1 RID: 1969 RVA: 0x0002616B File Offset: 0x0002436B
	protected override bool CanCancel()
	{
		return base.CanCancel() || this.destination.CanCancel();
	}

	// Token: 0x060007B2 RID: 1970 RVA: 0x00026182 File Offset: 0x00024382
	protected override IEnumerable<DroneProgram.Orientation> GetTargetOrientations()
	{
		return DroneProgramDestinationCorral.GetTargetOrientations(this.destination);
	}

	// Token: 0x060007B3 RID: 1971 RVA: 0x0002618F File Offset: 0x0002438F
	protected static IEnumerable<DroneProgram.Orientation> GetTargetOrientations(DroneProgramDestinationCorral.Destination destination)
	{
		yield return new DroneProgram.Orientation(destination.metadata.plot.transform.position + Vector3.up * (float)(destination.metadata.plot.HasUpgrade(LandPlot.Upgrade.WALLS) ? 6 : 3), Quaternion.Euler(0f, (float)Randoms.SHARED.GetInRange(0, 360), 0f));
		yield break;
	}

	// Token: 0x060007B4 RID: 1972 RVA: 0x0002619F File Offset: 0x0002439F
	protected override Vector3 GetTargetPosition()
	{
		return this.destination.metadata.plot.transform.position;
	}

	// Token: 0x060007B5 RID: 1973 RVA: 0x000261BC File Offset: 0x000243BC
	protected override void OnFirstAction()
	{
		base.OnFirstAction();
		this.time = this.timeDirector.HoursFromNow(0.013333335f);
		this.dropCount = new DroneProgramDestinationCorral.Destination(this.destination.metadata, this.drone.ammo.GetSlotName()).available;
	}

	// Token: 0x060007B6 RID: 1974 RVA: 0x00026210 File Offset: 0x00024410
	protected override bool OnAction_Deposit(bool overflow)
	{
		if (this.dropCount > 0 || overflow)
		{
			this.dropCount -= (base.OnAction_DumpAmmo(ref this.time) ? 1 : 0);
		}
		return !overflow && this.dropCount <= 0;
	}

	// Token: 0x060007B7 RID: 1975 RVA: 0x00026250 File Offset: 0x00024450
	public override DroneProgramDestination.FastForward_Response FastForward(Identifiable.Id id, bool overflow, double endTime, int maxFastForward)
	{
		DroneProgramDestinationCorral.Destination destination = this.Prioritize(this.GetDestinations(id, overflow)).First<DroneProgramDestinationCorral.Destination>();
		maxFastForward = (overflow ? maxFastForward : Mathf.Min(maxFastForward, destination.available));
		maxFastForward = RanchCellFastForwarder.FeedSlimes(destination.metadata, endTime, new RanchCellFastForwarder.FeedingSource[]
		{
			new RanchCellFastForwarder.FeedingSource.Basic(id, maxFastForward)
		});
		return new DroneProgramDestination.FastForward_Response
		{
			deposits = maxFastForward
		};
	}

	// Token: 0x17000109 RID: 265
	// (get) Token: 0x060007B8 RID: 1976 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
	protected override DroneAnimator.Id animation
	{
		get
		{
			return DroneAnimator.Id.IDLE;
		}
	}

	// Token: 0x1700010A RID: 266
	// (get) Token: 0x060007B9 RID: 1977 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
	protected override DroneAnimatorState.Id animationStateBegin
	{
		get
		{
			return DroneAnimatorState.Id.NONE;
		}
	}

	// Token: 0x1700010B RID: 267
	// (get) Token: 0x060007BA RID: 1978 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
	protected override DroneAnimatorState.Id animationStateEnd
	{
		get
		{
			return DroneAnimatorState.Id.NONE;
		}
	}

	// Token: 0x040006FD RID: 1789
	private double time;

	// Token: 0x040006FE RID: 1790
	private int dropCount;

	// Token: 0x040006FF RID: 1791
	private const float FOODS_PER_SLIME = 1.2f;

	// Token: 0x04000700 RID: 1792
	private const int MINIMUM_DELIVERY = 5;

	// Token: 0x02000166 RID: 358
	public class Destination
	{
		// Token: 0x060007BC RID: 1980 RVA: 0x000262BC File Offset: 0x000244BC
		public Destination(DroneNetwork.LandPlotMetadata metadata, Identifiable.Id id)
		{
			this.metadata = metadata;
			int num = 0;
			int num2 = 0;
			TrackContainedIdentifiables[] trackers = metadata.trackers;
			for (int i = 0; i < trackers.Length; i++)
			{
				foreach (KeyValuePair<Identifiable.Id, HashSet<Identifiable>> keyValuePair in trackers[i].GetAllTracked())
				{
					if (keyValuePair.Value.Any<Identifiable>())
					{
						if (Identifiable.IsSlime(keyValuePair.Key))
						{
							if (!this.anyEat || !this.anyFavorite)
							{
								SlimeEat component = keyValuePair.Value.First<Identifiable>().GetComponent<SlimeEat>();
								this.anyEat |= component.DoesEat(id);
								this.anyFavorite |= component.GetEatMapById(id).Any((SlimeDiet.EatMapEntry e) => e.isFavorite);
							}
							num += keyValuePair.Value.Count;
						}
						else if (Identifiable.IsFood(keyValuePair.Key))
						{
							num2 += keyValuePair.Value.Count;
						}
					}
				}
			}
			this.available = Mathf.Max(0, Mathf.Max(5, Mathf.CeilToInt((float)num * 1.2f)) - num2);
		}

		// Token: 0x060007BD RID: 1981 RVA: 0x00026420 File Offset: 0x00024620
		public bool CanCancel()
		{
			return this.metadata.plot == null;
		}

		// Token: 0x04000701 RID: 1793
		public readonly DroneNetwork.LandPlotMetadata metadata;

		// Token: 0x04000702 RID: 1794
		public readonly int available;

		// Token: 0x04000703 RID: 1795
		public readonly bool anyEat;

		// Token: 0x04000704 RID: 1796
		public readonly bool anyFavorite;

		// Token: 0x02000167 RID: 359
		public class Comparer : SRComparer<DroneProgramDestinationCorral.Destination>
		{
			// Token: 0x04000705 RID: 1797
			public new static Comparer<DroneProgramDestinationCorral.Destination> Default = from m in new DroneProgramDestinationCorral.Destination.Comparer()
			orderby m.anyFavorite descending
			orderby m.available
			select m;
		}
	}
}
