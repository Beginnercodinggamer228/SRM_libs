using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x02000763 RID: 1891
[RequireComponent(typeof(CellDirector))]
[RequireComponent(typeof(DroneNetwork))]
[RequireComponent(typeof(Region))]
public class RanchCellFastForwarder : IdHandler, RanchCellModel.Participant
{
	// Token: 0x06002781 RID: 10113 RVA: 0x00095CBA File Offset: 0x00093EBA
	public void Awake()
	{
		this.timeDirector = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.network = base.GetComponent<DroneNetwork>();
		this.region = base.GetComponent<Region>();
		SRSingleton<SceneContext>.Instance.GameModel.RegisterRanchCell(base.id, this);
	}

	// Token: 0x06002782 RID: 10114 RVA: 0x00095CFA File Offset: 0x00093EFA
	public void Start()
	{
		this.region.onHibernationStateChanged += this.OnHibernationStateChanged;
		this.timeDirector.onFastForwardChanged += this.OnFastForwardChanged;
	}

	// Token: 0x06002783 RID: 10115 RVA: 0x00003296 File Offset: 0x00001496
	public void InitModel(RanchCellModel model)
	{
	}

	// Token: 0x06002784 RID: 10116 RVA: 0x00095D2A File Offset: 0x00093F2A
	public void SetModel(RanchCellModel model)
	{
		this.model = model;
	}

	// Token: 0x06002785 RID: 10117 RVA: 0x00095D34 File Offset: 0x00093F34
	public void Update()
	{
		if (this.model.sleepingTime != null)
		{
			double num = this.model.sleepingTime.Value + 14400.0;
			if (this.timeDirector.HasReached(num) || !this.timeDirector.IsFastForwarding())
			{
				num = Math.Min(num, this.timeDirector.WorldTime());
				this.FastForwardDrones(this.model.sleepingTime.Value, num);
				bool flag = this.timeDirector.IsFastForwarding() && this.AnyDronesActive(num);
				this.model.sleepingTime = (flag ? new double?(num) : null);
			}
		}
	}

	// Token: 0x06002786 RID: 10118 RVA: 0x00095DEC File Offset: 0x00093FEC
	public void OnDestroy()
	{
		if (this.timeDirector != null)
		{
			this.timeDirector.onFastForwardChanged -= this.OnFastForwardChanged;
			this.timeDirector = null;
		}
		if (this.region != null)
		{
			this.region.onHibernationStateChanged -= this.OnHibernationStateChanged;
			this.region = null;
		}
		if (SRSingleton<SceneContext>.Instance != null)
		{
			SRSingleton<SceneContext>.Instance.GameModel.UnregisterRanchCell(base.id);
		}
	}

	// Token: 0x06002787 RID: 10119 RVA: 0x00095E73 File Offset: 0x00094073
	protected override string IdPrefix()
	{
		return "ranch";
	}

	// Token: 0x06002788 RID: 10120 RVA: 0x00095E7A File Offset: 0x0009407A
	private void OnHibernationStateChanged(bool hibernating)
	{
		if (hibernating)
		{
			this.OnHibernation();
			return;
		}
		SRSingleton<GameContext>.Instance.StartCoroutine(this.OnFastForward());
	}

	// Token: 0x06002789 RID: 10121 RVA: 0x00095E97 File Offset: 0x00094097
	private void OnHibernation()
	{
		if (this.model.hibernationTime == null)
		{
			this.model.hibernationTime = new double?(this.timeDirector.WorldTime());
		}
	}

	// Token: 0x0600278A RID: 10122 RVA: 0x00095EC6 File Offset: 0x000940C6
	private IEnumerator OnFastForward()
	{
		if (this.model.hibernationTime != null)
		{
			double startTime = this.model.hibernationTime.Value;
			double endTime = this.timeDirector.WorldTime();
			this.model.hibernationTime = null;
			if (endTime - startTime >= 7200.0)
			{
				try
				{
					DroneFastForwarder.FastForward_Pre(this);
					while (startTime < endTime)
					{
						double chunkEndTime = this.AnyDronesActive(startTime) ? Math.Min(endTime, startTime + 14400.0) : endTime;
						this.FastForwardCorrals(startTime, chunkEndTime);
						this.FastForwardGardens(startTime, chunkEndTime);
						this.FastForwardPonds(startTime, chunkEndTime);
						yield return new WaitForFixedUpdate();
						this.FastForwardDrones(startTime, chunkEndTime);
						startTime = chunkEndTime;
					}
				}
				finally
				{
					DroneFastForwarder.FastForward_Post(this);
				}
			}
		}
		yield break;
		yield break;
	}

	// Token: 0x0600278B RID: 10123 RVA: 0x00095ED8 File Offset: 0x000940D8
	private void FastForwardCorrals(double startTime, double endTime)
	{
		foreach (DroneNetwork.LandPlotMetadata landPlotMetadata in this.network.Plots)
		{
			if (landPlotMetadata.plot.typeId == LandPlot.Id.CORRAL)
			{
				RanchCellFastForwarder.FeedSlimes(landPlotMetadata, endTime, new RanchCellFastForwarder.FeedingSource[]
				{
					new RanchCellFastForwarder.FeedingSource.AutoFeeder(landPlotMetadata, endTime),
					new RanchCellFastForwarder.FeedingSource.Dynamic(landPlotMetadata.trackers.First<TrackContainedIdentifiables>())
				});
			}
		}
	}

	// Token: 0x0600278C RID: 10124 RVA: 0x00095F5C File Offset: 0x0009415C
	private void FastForwardPonds(double startTime, double endTime)
	{
		foreach (DroneNetwork.LandPlotMetadata landPlotMetadata in this.network.Plots)
		{
			if (landPlotMetadata.plot.typeId == LandPlot.Id.POND)
			{
				LiquidSource componentInChildren = landPlotMetadata.plot.GetComponentInChildren<LiquidSource>();
				RanchCellFastForwarder.FeedSlimes(landPlotMetadata, endTime, new RanchCellFastForwarder.FeedingSource[]
				{
					new RanchCellFastForwarder.FeedingSource.LiquidSource(componentInChildren)
				});
			}
		}
	}

	// Token: 0x0600278D RID: 10125 RVA: 0x00095FD8 File Offset: 0x000941D8
	public static int FeedSlimes(DroneNetwork.LandPlotMetadata metadata, double endTime, params RanchCellFastForwarder.FeedingSource[] sources)
	{
		int num = 0;
		RanchCellFastForwarder.HUNGRY_SLIMES.Clear();
		RanchCellFastForwarder.PRODUCED.Clear();
		RanchCellFastForwarder.COLLECTED.Clear();
		if (sources.Any((RanchCellFastForwarder.FeedingSource s) => s.ids.Any<Identifiable.Id>()))
		{
			metadata.trackers.First<TrackContainedIdentifiables>().GetTrackedItemsOfClass(Identifiable.EATERS_CLASS, RanchCellFastForwarder.HUNGRY_SLIMES);
			while (RanchCellFastForwarder.HUNGRY_SLIMES.Any<GameObject>())
			{
				if (!sources.Any((RanchCellFastForwarder.FeedingSource s) => s.ids.Any<Identifiable.Id>()))
				{
					break;
				}
				GameObject gameObject = Randoms.SHARED.Pluck<GameObject>(RanchCellFastForwarder.HUNGRY_SLIMES, null);
				gameObject.GetComponent<SlimeEmotions>().UpdateToTime(endTime);
				if (RanchCellFastForwarder.FeedSlime(metadata, gameObject, sources))
				{
					RanchCellFastForwarder.HUNGRY_SLIMES.Add(gameObject);
					num++;
				}
			}
			RanchCellFastForwarder.HUNGRY_SLIMES.Clear();
			RanchCellFastForwarder.PRODUCED.Clear();
			RanchCellFastForwarder.COLLECTED.Clear();
		}
		return num;
	}

	// Token: 0x0600278E RID: 10126 RVA: 0x000960D4 File Offset: 0x000942D4
	private static bool FeedSlime(DroneNetwork.LandPlotMetadata metadata, GameObject slime, RanchCellFastForwarder.FeedingSource[] sources)
	{
		Identifiable.Id id = Identifiable.GetId(slime);
		if (id == Identifiable.Id.PUDDLE_SLIME)
		{
			return RanchCellFastForwarder.FeedSlime(metadata, slime.GetComponent<SlimeEatWater>(), sources);
		}
		if (id != Identifiable.Id.FIRE_SLIME)
		{
			return RanchCellFastForwarder.FeedSlime(metadata, slime.GetComponent<SlimeEat>(), sources);
		}
		return RanchCellFastForwarder.FeedSlime(metadata, slime.GetComponent<SlimeEatAsh>(), sources);
	}

	// Token: 0x0600278F RID: 10127 RVA: 0x00096120 File Offset: 0x00094320
	private static bool FeedSlime(DroneNetwork.LandPlotMetadata metadata, SlimeEat eat, RanchCellFastForwarder.FeedingSource[] sources)
	{
		if (eat.WantsToEat())
		{
			PlortCollector componentInChildren = metadata.plot.GetComponentInChildren<PlortCollector>();
			foreach (RanchCellFastForwarder.FeedingSource feedingSource in sources)
			{
				foreach (Identifiable.Id id in feedingSource.ids)
				{
					if (eat.WillNowEat(id) && feedingSource.Selected(id))
					{
						RanchCellFastForwarder.PRODUCED = eat.GetProducedIds(id, RanchCellFastForwarder.PRODUCED);
						RanchCellFastForwarder.COLLECTED.Clear();
						if (componentInChildren != null)
						{
							componentInChildren.FastForward(RanchCellFastForwarder.PRODUCED, RanchCellFastForwarder.COLLECTED);
						}
						eat.EatImmediate(feedingSource.GetTarget(id), id, RanchCellFastForwarder.PRODUCED, RanchCellFastForwarder.COLLECTED, true);
						return true;
					}
				}
			}
		}
		return false;
	}

	// Token: 0x06002790 RID: 10128 RVA: 0x0009620C File Offset: 0x0009440C
	private static bool FeedSlime(DroneNetwork.LandPlotMetadata metadata, SlimeEatWater eat, RanchCellFastForwarder.FeedingSource[] sources)
	{
		foreach (RanchCellFastForwarder.FeedingSource feedingSource in sources)
		{
			foreach (Identifiable.Id id in feedingSource.ids)
			{
				if (eat.WillNowEat(id) && feedingSource.Selected(id))
				{
					RanchCellFastForwarder.PRODUCED = eat.GetProducedIds(id, RanchCellFastForwarder.PRODUCED);
					RanchCellFastForwarder.COLLECTED.Clear();
					eat.EatImmediate(feedingSource.GetTarget(id), id, RanchCellFastForwarder.PRODUCED, RanchCellFastForwarder.COLLECTED, true);
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06002791 RID: 10129 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
	private static bool FeedSlime(DroneNetwork.LandPlotMetadata metadata, SlimeEatAsh eat, RanchCellFastForwarder.FeedingSource[] sources)
	{
		return false;
	}

	// Token: 0x06002792 RID: 10130 RVA: 0x000962C4 File Offset: 0x000944C4
	private void FastForwardGardens(double startTime, double endTime)
	{
		foreach (DroneNetwork.LandPlotMetadata landPlotMetadata in this.network.Plots)
		{
			if (landPlotMetadata.plot.typeId == LandPlot.Id.GARDEN)
			{
				SpawnResource componentInChildren = landPlotMetadata.plot.GetComponentInChildren<SpawnResource>();
				if (componentInChildren != null)
				{
					componentInChildren.FastForward(startTime, endTime);
				}
			}
		}
	}

	// Token: 0x06002793 RID: 10131 RVA: 0x0009633C File Offset: 0x0009453C
	private void FastForwardDrones(double startTime, double endTime)
	{
		foreach (Drone drone in this.network.Drones)
		{
			DroneFastForwarder.FastForward(drone, startTime, endTime);
		}
	}

	// Token: 0x06002794 RID: 10132 RVA: 0x00096390 File Offset: 0x00094590
	private bool AnyDronesActive(double time)
	{
		return this.network.Drones.Any((Drone d) => d.station.battery.Time > time);
	}

	// Token: 0x06002795 RID: 10133 RVA: 0x000963C8 File Offset: 0x000945C8
	private void OnFastForwardChanged(bool isFastForwarding)
	{
		if (isFastForwarding)
		{
			if (!this.region.Hibernated && this.AnyDronesActive(this.timeDirector.WorldTime()))
			{
				DroneFastForwarder.FastForward_Pre(this);
				this.model.sleepingTime = new double?(this.timeDirector.WorldTime());
				return;
			}
		}
		else
		{
			SRSingleton<SceneContext>.Instance.StartCoroutine(this.OnFastForwardChangedCoroutine());
		}
	}

	// Token: 0x06002796 RID: 10134 RVA: 0x0009642B File Offset: 0x0009462B
	private IEnumerator OnFastForwardChangedCoroutine()
	{
		yield return new WaitForEndOfFrame();
		DroneFastForwarder.FastForward_Post(this);
		yield break;
	}

	// Token: 0x04002739 RID: 10041
	private RanchCellModel model;

	// Token: 0x0400273A RID: 10042
	private static List<GameObject> HUNGRY_SLIMES = new List<GameObject>();

	// Token: 0x0400273B RID: 10043
	private static List<Identifiable.Id> PRODUCED = new List<Identifiable.Id>();

	// Token: 0x0400273C RID: 10044
	private static List<Identifiable.Id> COLLECTED = new List<Identifiable.Id>();

	// Token: 0x0400273D RID: 10045
	private const double FASTFORWARD_MIN_HOURS = 2.0;

	// Token: 0x0400273E RID: 10046
	private const double FASTFORWARD_MIN_SECS = 7200.0;

	// Token: 0x0400273F RID: 10047
	private const double FASTFORWARD_CHUNK_HOURS = 4.0;

	// Token: 0x04002740 RID: 10048
	private const double FASTFORWARD_CHUNK_SECS = 14400.0;

	// Token: 0x04002741 RID: 10049
	private DroneNetwork network;

	// Token: 0x04002742 RID: 10050
	private Region region;

	// Token: 0x04002743 RID: 10051
	private TimeDirector timeDirector;

	// Token: 0x02000764 RID: 1892
	public abstract class FeedingSource
	{
		// Token: 0x17000270 RID: 624
		// (get) Token: 0x06002799 RID: 10137
		public abstract IEnumerable<Identifiable.Id> ids { get; }

		// Token: 0x0600279A RID: 10138 RVA: 0x00013CC5 File Offset: 0x00011EC5
		public virtual bool Selected(Identifiable.Id id)
		{
			return true;
		}

		// Token: 0x0600279B RID: 10139 RVA: 0x00025E60 File Offset: 0x00024060
		public virtual GameObject GetTarget(Identifiable.Id id)
		{
			return null;
		}

		// Token: 0x02000765 RID: 1893
		public class Basic : RanchCellFastForwarder.FeedingSource
		{
			// Token: 0x17000271 RID: 625
			// (get) Token: 0x0600279D RID: 10141 RVA: 0x0009645A File Offset: 0x0009465A
			public override IEnumerable<Identifiable.Id> ids
			{
				get
				{
					if (this.remaining > 0)
					{
						yield return this.id;
					}
					yield break;
				}
			}

			// Token: 0x0600279E RID: 10142 RVA: 0x0009646A File Offset: 0x0009466A
			public Basic()
			{
			}

			// Token: 0x0600279F RID: 10143 RVA: 0x00096472 File Offset: 0x00094672
			public Basic(Identifiable.Id id, int remaining)
			{
				this.id = id;
				this.remaining = remaining;
			}

			// Token: 0x060027A0 RID: 10144 RVA: 0x00096488 File Offset: 0x00094688
			public override bool Selected(Identifiable.Id id)
			{
				if (base.Selected(id))
				{
					this.remaining--;
					return true;
				}
				return false;
			}

			// Token: 0x04002744 RID: 10052
			protected Identifiable.Id id;

			// Token: 0x04002745 RID: 10053
			protected int remaining;
		}

		// Token: 0x02000767 RID: 1895
		public class AutoFeeder : RanchCellFastForwarder.FeedingSource.Basic
		{
			// Token: 0x060027A9 RID: 10153 RVA: 0x00096574 File Offset: 0x00094774
			public AutoFeeder(DroneNetwork.LandPlotMetadata metadata, double endTime)
			{
				this.feeder = metadata.plot.GetComponentInChildren<SlimeFeeder>();
				if (this.feeder != null)
				{
					this.feeder.UpdateToTime(endTime);
					this.id = this.feeder.GetFoodId();
					this.remaining = this.feeder.RemainingFeedOperationsFastForward();
				}
			}

			// Token: 0x060027AA RID: 10154 RVA: 0x000965D4 File Offset: 0x000947D4
			public override bool Selected(Identifiable.Id id)
			{
				if (base.Selected(id))
				{
					this.feeder.ProcessFeedOperationFastForward();
					return true;
				}
				return false;
			}

			// Token: 0x0400274A RID: 10058
			private SlimeFeeder feeder;
		}

		// Token: 0x02000768 RID: 1896
		public class Dynamic : RanchCellFastForwarder.FeedingSource
		{
			// Token: 0x17000274 RID: 628
			// (get) Token: 0x060027AB RID: 10155 RVA: 0x000965ED File Offset: 0x000947ED
			public override IEnumerable<Identifiable.Id> ids
			{
				get
				{
					return from id in this.container.GetTrackedIdentifiableTypes()
					where Identifiable.IsFood(id)
					select id;
				}
			}

			// Token: 0x060027AC RID: 10156 RVA: 0x0009661E File Offset: 0x0009481E
			public Dynamic(TrackContainedIdentifiables container)
			{
				this.container = container;
			}

			// Token: 0x060027AD RID: 10157 RVA: 0x0009662D File Offset: 0x0009482D
			public override GameObject GetTarget(Identifiable.Id id)
			{
				return this.container.RemoveTrackedObject(id).gameObject;
			}

			// Token: 0x0400274B RID: 10059
			private TrackContainedIdentifiables container;
		}

		// Token: 0x0200076A RID: 1898
		public class LiquidSource : RanchCellFastForwarder.FeedingSource
		{
			// Token: 0x17000275 RID: 629
			// (get) Token: 0x060027B1 RID: 10161 RVA: 0x00096654 File Offset: 0x00094854
			public override IEnumerable<Identifiable.Id> ids
			{
				get
				{
					yield return this.source.liquidId;
					yield break;
				}
			}

			// Token: 0x060027B2 RID: 10162 RVA: 0x00096664 File Offset: 0x00094864
			public LiquidSource(global::LiquidSource source)
			{
				this.source = source;
			}

			// Token: 0x060027B3 RID: 10163 RVA: 0x00096673 File Offset: 0x00094873
			public override bool Selected(Identifiable.Id id)
			{
				if (this.source.Available())
				{
					this.source.ConsumeLiquid();
					return true;
				}
				return false;
			}

			// Token: 0x060027B4 RID: 10164 RVA: 0x00096690 File Offset: 0x00094890
			public override GameObject GetTarget(Identifiable.Id id)
			{
				return this.source.gameObject;
			}

			// Token: 0x0400274E RID: 10062
			private global::LiquidSource source;
		}
	}
}
