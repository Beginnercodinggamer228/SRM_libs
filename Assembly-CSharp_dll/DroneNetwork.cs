using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x0200014C RID: 332
public class DroneNetwork : PathingNetwork
{
	// Token: 0x170000FA RID: 250
	// (get) Token: 0x06000733 RID: 1843 RVA: 0x00024E71 File Offset: 0x00023071
	public override Pather Pather
	{
		get
		{
			return this.pather;
		}
	}

	// Token: 0x06000734 RID: 1844 RVA: 0x00024E7C File Offset: 0x0002307C
	public void Register(LandPlot p)
	{
		List<DroneNetwork.LandPlotMetadata> list = this.plots;
		DroneNetwork.LandPlotMetadata landPlotMetadata = new DroneNetwork.LandPlotMetadata();
		landPlotMetadata.plot = p;
		landPlotMetadata.trackers = p.GetComponentsInChildren<TrackContainedIdentifiables>();
		landPlotMetadata.subnetwork = p.GetComponentInChildren<GardenDroneSubnetwork>();
		DroneNetwork.StorageMetadata[] feeders;
		if (p.typeId != LandPlot.Id.CORRAL)
		{
			feeders = new DroneNetwork.StorageMetadata[0];
		}
		else
		{
			feeders = DroneNetwork.GetStorageMetadata(from c in p.GetComponents<FeederUpgrader>()
			where c != null && c.feeder.activeInHierarchy
			select c.feeder.GetComponent<SiloStorage>()).ToArray<DroneNetwork.StorageMetadata>();
		}
		landPlotMetadata.feeders = feeders;
		DroneNetwork.StorageMetadata[] plortCollectors;
		if (p.typeId != LandPlot.Id.CORRAL)
		{
			plortCollectors = new DroneNetwork.StorageMetadata[0];
		}
		else
		{
			plortCollectors = DroneNetwork.GetStorageMetadata(from c in p.GetComponents<PlortCollectorUpgrader>()
			where c != null && c.collector.activeInHierarchy
			select c.collector.GetComponent<SiloStorage>()).ToArray<DroneNetwork.StorageMetadata>();
		}
		landPlotMetadata.plortCollectors = plortCollectors;
		DroneNetwork.StorageMetadata[] elderCollectors;
		if (p.typeId != LandPlot.Id.COOP)
		{
			elderCollectors = new DroneNetwork.StorageMetadata[0];
		}
		else
		{
			elderCollectors = DroneNetwork.GetStorageMetadata(from c in p.GetComponents<DeluxeCoopUpgrader>()
			where c != null && c.deluxeStuff.activeInHierarchy
			select c.deluxeStuff.GetComponentInChildren<SiloStorage>()).ToArray<DroneNetwork.StorageMetadata>();
		}
		landPlotMetadata.elderCollectors = elderCollectors;
		DroneNetwork.StorageMetadata[] silos;
		if (p.typeId != LandPlot.Id.SILO)
		{
			silos = new DroneNetwork.StorageMetadata[0];
		}
		else
		{
			silos = p.GetComponents<SiloStorage>().SelectMany((SiloStorage s) => s.GetComponentsInChildren<SiloStorageActivator>().SelectMany((SiloStorageActivator a) => from ui in a.siloSlotUIs
			select ui.slotIdx).Select(delegate(int i)
			{
				Func<SiloSlotUI, bool> <>9__11;
				return new DroneNetwork.StorageMetadata
				{
					index = i,
					storage = s,
					catcher = s.GetComponentsInChildren<SiloStorageActivator>().First(delegate(SiloStorageActivator a)
					{
						IEnumerable<SiloSlotUI> siloSlotUIs = a.siloSlotUIs;
						Func<SiloSlotUI, bool> predicate;
						if ((predicate = <>9__11) == null)
						{
							predicate = (<>9__11 = ((SiloSlotUI ui) => ui.slotIdx == i));
						}
						return siloSlotUIs.Any(predicate);
					}).siloCatcher
				};
			})).ToArray<DroneNetwork.StorageMetadata>();
		}
		landPlotMetadata.silos = silos;
		landPlotMetadata.gardens = ((p.typeId == LandPlot.Id.GARDEN) ? p.GetComponentsInChildren<GardenCatcher>() : new GardenCatcher[0]);
		landPlotMetadata.incinerators = ((p.typeId == LandPlot.Id.INCINERATOR) ? p.GetComponentsInChildren<Incinerate>() : new Incinerate[0]);
		list.Add(landPlotMetadata);
	}

	// Token: 0x06000735 RID: 1845 RVA: 0x00025088 File Offset: 0x00023288
	public bool Deregister(LandPlot deregister)
	{
		return this.plots.RemoveAll((DroneNetwork.LandPlotMetadata p) => p.plot == deregister) >= 1;
	}

	// Token: 0x06000736 RID: 1846 RVA: 0x000250BF File Offset: 0x000232BF
	public void OnUpgradesChanged(LandPlot plot)
	{
		if (this.Deregister(plot))
		{
			this.Register(plot);
		}
	}

	// Token: 0x06000737 RID: 1847 RVA: 0x000250D4 File Offset: 0x000232D4
	public DroneNetwork.LandPlotMetadata GetContaining(Identifiable source)
	{
		return this.plots.FirstOrDefault((DroneNetwork.LandPlotMetadata m) => m.Contains(source));
	}

	// Token: 0x06000738 RID: 1848 RVA: 0x00025108 File Offset: 0x00023308
	private static IEnumerable<DroneNetwork.StorageMetadata> GetStorageMetadata(IEnumerable<SiloStorage> storages)
	{
		return (from s in storages
		select new
		{
			storage = s,
			ammo = s.GetRelevantAmmo()
		}).SelectMany(s => from i in Enumerable.Range(0, s.ammo.GetUsableSlotCount())
		select new DroneNetwork.StorageMetadata
		{
			index = i,
			storage = s.storage,
			catcher = s.storage.GetComponentsInChildren<SiloCatcher>().First((SiloCatcher c) => c.slotIdx == i)
		});
	}

	// Token: 0x170000FB RID: 251
	// (get) Token: 0x06000739 RID: 1849 RVA: 0x0002515E File Offset: 0x0002335E
	public IEnumerable<DroneNetwork.LandPlotMetadata> Plots
	{
		get
		{
			return this.plots;
		}
	}

	// Token: 0x170000FC RID: 252
	// (get) Token: 0x0600073A RID: 1850 RVA: 0x00025166 File Offset: 0x00023366
	public IEnumerable<Drone> Drones
	{
		get
		{
			return this.drones;
		}
	}

	// Token: 0x0600073B RID: 1851 RVA: 0x0002516E File Offset: 0x0002336E
	public void Register(Drone drone)
	{
		this.drones.Add(drone);
	}

	// Token: 0x0600073C RID: 1852 RVA: 0x0002517C File Offset: 0x0002337C
	public bool Deregister(Drone deregister)
	{
		return this.drones.RemoveAll((Drone d) => d == deregister) >= 1;
	}

	// Token: 0x170000FD RID: 253
	// (get) Token: 0x0600073D RID: 1853 RVA: 0x000251B3 File Offset: 0x000233B3
	public IEnumerable<SiloCatcher> RefineryCatchers
	{
		get
		{
			return this.refineryCatchers;
		}
	}

	// Token: 0x0600073E RID: 1854 RVA: 0x000251BB File Offset: 0x000233BB
	public void Register(SiloCatcher catcher)
	{
		if (catcher.type == SiloCatcher.Type.REFINERY)
		{
			this.refineryCatchers.Add(catcher);
		}
	}

	// Token: 0x0600073F RID: 1855 RVA: 0x000251D4 File Offset: 0x000233D4
	public bool Deregister(SiloCatcher catcher)
	{
		return catcher.type != SiloCatcher.Type.REFINERY || this.refineryCatchers.RemoveAll((SiloCatcher d) => d == catcher) >= 1;
	}

	// Token: 0x170000FE RID: 254
	// (get) Token: 0x06000740 RID: 1856 RVA: 0x0002521B File Offset: 0x0002341B
	public IEnumerable<ScorePlort> Markets
	{
		get
		{
			return this.markets;
		}
	}

	// Token: 0x06000741 RID: 1857 RVA: 0x00025223 File Offset: 0x00023423
	public void Register(ScorePlort market)
	{
		this.markets.Add(market);
	}

	// Token: 0x06000742 RID: 1858 RVA: 0x00025234 File Offset: 0x00023434
	public bool Deregister(ScorePlort market)
	{
		return this.markets.RemoveAll((ScorePlort d) => d == market) >= 1;
	}

	// Token: 0x06000743 RID: 1859 RVA: 0x0002526C File Offset: 0x0002346C
	public static bool IsResourceReady(GameObject go)
	{
		ResourceCycle component = go.GetComponent<ResourceCycle>();
		return component == null || component.GetState() == ResourceCycle.State.RIPE || component.GetState() == ResourceCycle.State.EDIBLE;
	}

	// Token: 0x06000744 RID: 1860 RVA: 0x000252A0 File Offset: 0x000234A0
	public static DroneNetwork Find(GameObject gameObject)
	{
		CellDirector componentInParent = gameObject.GetComponentInParent<CellDirector>();
		if (!(componentInParent == null))
		{
			return componentInParent.GetComponent<DroneNetwork>();
		}
		return null;
	}

	// Token: 0x040006B4 RID: 1716
	public const float PATHING_THROTTLE_HRS = 0.033333335f;

	// Token: 0x040006B5 RID: 1717
	[HideInInspector]
	public double pathingThrottleUntil;

	// Token: 0x040006B6 RID: 1718
	private const float MAX_CONNECTION_DIST = 40f;

	// Token: 0x040006B7 RID: 1719
	private DronePather pather = new DronePather(40f);

	// Token: 0x040006B8 RID: 1720
	private List<DroneNetwork.LandPlotMetadata> plots = new List<DroneNetwork.LandPlotMetadata>();

	// Token: 0x040006B9 RID: 1721
	private List<Drone> drones = new List<Drone>();

	// Token: 0x040006BA RID: 1722
	private List<SiloCatcher> refineryCatchers = new List<SiloCatcher>();

	// Token: 0x040006BB RID: 1723
	private List<ScorePlort> markets = new List<ScorePlort>();

	// Token: 0x0200014D RID: 333
	public class LandPlotMetadata
	{
		// Token: 0x06000746 RID: 1862 RVA: 0x00025318 File Offset: 0x00023518
		public bool Contains(Identifiable identifiable)
		{
			return this.trackers.Any((TrackContainedIdentifiables t) => t.Contains(identifiable));
		}

		// Token: 0x040006BC RID: 1724
		public LandPlot plot;

		// Token: 0x040006BD RID: 1725
		public GardenCatcher[] gardens;

		// Token: 0x040006BE RID: 1726
		public DroneNetwork.StorageMetadata[] feeders;

		// Token: 0x040006BF RID: 1727
		public DroneNetwork.StorageMetadata[] silos;

		// Token: 0x040006C0 RID: 1728
		public DroneNetwork.StorageMetadata[] plortCollectors;

		// Token: 0x040006C1 RID: 1729
		public DroneNetwork.StorageMetadata[] elderCollectors;

		// Token: 0x040006C2 RID: 1730
		public TrackContainedIdentifiables[] trackers;

		// Token: 0x040006C3 RID: 1731
		public Incinerate[] incinerators;

		// Token: 0x040006C4 RID: 1732
		public GardenDroneSubnetwork subnetwork;
	}

	// Token: 0x0200014F RID: 335
	public class StorageMetadata
	{
		// Token: 0x0600074A RID: 1866 RVA: 0x000053FC File Offset: 0x000035FC
		public StorageMetadata()
		{
		}

		// Token: 0x0600074B RID: 1867 RVA: 0x00025357 File Offset: 0x00023557
		public StorageMetadata(DroneNetwork.StorageMetadata other)
		{
			this.storage = other.storage;
			this.catcher = other.catcher;
			this.index = other.index;
		}

		// Token: 0x0600074C RID: 1868 RVA: 0x00025383 File Offset: 0x00023583
		public bool CanCancel()
		{
			return this.storage == null || !this.storage.gameObject.activeInHierarchy;
		}

		// Token: 0x170000FF RID: 255
		// (get) Token: 0x0600074D RID: 1869 RVA: 0x000253A8 File Offset: 0x000235A8
		public Ammo ammo
		{
			get
			{
				return this.storage.GetRelevantAmmo();
			}
		}

		// Token: 0x17000100 RID: 256
		// (get) Token: 0x0600074E RID: 1870 RVA: 0x000253B5 File Offset: 0x000235B5
		public Identifiable.Id id
		{
			get
			{
				return this.ammo.GetSlotName(this.index);
			}
		}

		// Token: 0x17000101 RID: 257
		// (get) Token: 0x0600074F RID: 1871 RVA: 0x000253C8 File Offset: 0x000235C8
		public int count
		{
			get
			{
				return this.ammo.GetSlotCount(this.index);
			}
		}

		// Token: 0x17000102 RID: 258
		// (get) Token: 0x06000750 RID: 1872 RVA: 0x000253DB File Offset: 0x000235DB
		public int maxCount
		{
			get
			{
				return this.ammo.GetSlotMaxCount(this.index);
			}
		}

		// Token: 0x06000751 RID: 1873 RVA: 0x000253EE File Offset: 0x000235EE
		public int GetAvailableSpace(Identifiable.Id id)
		{
			if (this.id != id && this.id != Identifiable.Id.NONE)
			{
				return 0;
			}
			return this.maxCount - this.count;
		}

		// Token: 0x06000752 RID: 1874 RVA: 0x00025410 File Offset: 0x00023610
		public bool Increment(Identifiable.Id id, bool overflow, int count = 1)
		{
			return this.storage.MaybeAddIdentifiable(id, this.index, count, overflow);
		}

		// Token: 0x06000753 RID: 1875 RVA: 0x00025426 File Offset: 0x00023626
		public void Decrement(int count = 1)
		{
			this.ammo.Decrement(this.index, count);
		}

		// Token: 0x06000754 RID: 1876 RVA: 0x0002543A File Offset: 0x0002363A
		public bool IsFull()
		{
			return this.count >= this.maxCount;
		}

		// Token: 0x040006C6 RID: 1734
		public SiloStorage storage;

		// Token: 0x040006C7 RID: 1735
		public SiloCatcher catcher;

		// Token: 0x040006C8 RID: 1736
		public int index;
	}
}
