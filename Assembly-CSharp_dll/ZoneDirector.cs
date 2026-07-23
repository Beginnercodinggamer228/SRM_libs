using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x020007D0 RID: 2000
public class ZoneDirector : SRBehaviour, WorldModel.Participant
{
	// Token: 0x060029DD RID: 10717 RVA: 0x0009D1DC File Offset: 0x0009B3DC
	public static RegionRegistry.RegionSetId GetRegionSetId(ZoneDirector.Zone zone)
	{
		switch (zone)
		{
		case ZoneDirector.Zone.RANCH:
		case ZoneDirector.Zone.REEF:
		case ZoneDirector.Zone.QUARRY:
		case ZoneDirector.Zone.MOSS:
		case ZoneDirector.Zone.SEA:
		case ZoneDirector.Zone.RUINS:
		case ZoneDirector.Zone.RUINS_TRANSITION:
		case ZoneDirector.Zone.WILDS:
		case ZoneDirector.Zone.OGDEN_RANCH:
			return RegionRegistry.RegionSetId.HOME;
		case ZoneDirector.Zone.DESERT:
			return RegionRegistry.RegionSetId.DESERT;
		case ZoneDirector.Zone.VALLEY:
		case ZoneDirector.Zone.MOCHI_RANCH:
			return RegionRegistry.RegionSetId.VALLEY;
		case ZoneDirector.Zone.SLIMULATIONS:
			return RegionRegistry.RegionSetId.SLIMULATIONS;
		case ZoneDirector.Zone.VIKTOR_LAB:
			return RegionRegistry.RegionSetId.VIKTOR_LAB;
		}
		throw new ArgumentException(string.Format("Failed to get RegionSetId from Zone. [zone={0}]", zone));
	}

	// Token: 0x1700028E RID: 654
	// (get) Token: 0x060029DE RID: 10718 RVA: 0x0009D24C File Offset: 0x0009B44C
	public RegionRegistry.RegionSetId regionSetId
	{
		get
		{
			return ZoneDirector.GetRegionSetId(this.zone);
		}
	}

	// Token: 0x060029DF RID: 10719 RVA: 0x0009D25C File Offset: 0x0009B45C
	public void Awake()
	{
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.lookupDir = SRSingleton<GameContext>.Instance.LookupDirector;
		ZoneDirector.zones.Add(this.zone, this);
		SRSingleton<SceneContext>.Instance.GameModel.RegisterWorldParticipant(this);
	}

	// Token: 0x060029E0 RID: 10720 RVA: 0x0009D2AA File Offset: 0x0009B4AA
	public void OnDestroy()
	{
		ZoneDirector.zones.Remove(this.zone);
	}

	// Token: 0x060029E1 RID: 10721 RVA: 0x0009D2C0 File Offset: 0x0009B4C0
	public void Start()
	{
		foreach (ZoneDirector.AuxItemEntry auxItemEntry in this.auxItems)
		{
			this.auxItemDict[auxItemEntry.item] = auxItemEntry.weight;
		}
		if (SRSingleton<GameContext>.Instance.AutoSaveDirector.IsNewGame())
		{
			this.ResetCrates();
			this.ResetAuxItems();
			this.ResetGingerPatches();
		}
	}

	// Token: 0x060029E2 RID: 10722 RVA: 0x0009D320 File Offset: 0x0009B520
	public void Update()
	{
		if (this.timeDir.OnPassedHour(5f))
		{
			if (SRSingleton<LockOnDeath>.Instance != null && SRSingleton<LockOnDeath>.Instance.Locked())
			{
				this.ResetCrates();
				if (this.auxItemsFullClear)
				{
					this.ResetAuxItems();
				}
			}
			if (!this.auxItemsFullClear)
			{
				this.ResetAuxItems();
			}
			this.ResetGingerPatches();
		}
		if (this.timeDir.HasReached(this.nextKookadobaTime))
		{
			this.SpawnKookadoba();
			this.nextKookadobaTime = this.timeDir.HoursFromNow(ZoneDirector.KOOKADOBA_DELAY);
		}
	}

	// Token: 0x060029E3 RID: 10723 RVA: 0x0009D3B0 File Offset: 0x0009B5B0
	private void SpawnKookadoba()
	{
		KookadobaPatchNode kookadobaPatchNode = Randoms.SHARED.Pick<KookadobaPatchNode>(this.kookadobaPatches, null);
		if (kookadobaPatchNode != null)
		{
			kookadobaPatchNode.Grow();
		}
	}

	// Token: 0x060029E4 RID: 10724 RVA: 0x0009D3E0 File Offset: 0x0009B5E0
	public static HashSet<ZoneDirector.Zone> Zones(RegionMember member)
	{
		HashSet<ZoneDirector.Zone> hashSet = new HashSet<ZoneDirector.Zone>();
		foreach (Region region in member.regions)
		{
			ZoneDirector componentInParent = region.GetComponentInParent<ZoneDirector>();
			if (componentInParent != null)
			{
				hashSet.Add(componentInParent.zone);
			}
		}
		return hashSet;
	}

	// Token: 0x060029E5 RID: 10725 RVA: 0x0009D450 File Offset: 0x0009B650
	public static HashSet<ZoneDirector.Zone> Zones(GameObject gameObject)
	{
		RegionMember component = gameObject.GetComponent<RegionMember>();
		if (!(component != null))
		{
			return new HashSet<ZoneDirector.Zone>();
		}
		return ZoneDirector.Zones(component);
	}

	// Token: 0x060029E6 RID: 10726 RVA: 0x0009D479 File Offset: 0x0009B679
	public void Register(DirectedCrateSpawner spawner)
	{
		this.crateSpawners.Add(spawner);
	}

	// Token: 0x060029E7 RID: 10727 RVA: 0x0009D487 File Offset: 0x0009B687
	public void Register(DirectedAuxItemSpawner spawner)
	{
		this.auxItemSpawners.Add(spawner);
	}

	// Token: 0x060029E8 RID: 10728 RVA: 0x0009D495 File Offset: 0x0009B695
	public void Register(GingerPatchNode gingerPatch)
	{
		this.gingerPatches.Add(gingerPatch);
	}

	// Token: 0x060029E9 RID: 10729 RVA: 0x0009D4A3 File Offset: 0x0009B6A3
	public void Register(KookadobaPatchNode kookadobaPatch)
	{
		this.kookadobaPatches.Add(kookadobaPatch);
	}

	// Token: 0x060029EA RID: 10730 RVA: 0x0009D4B4 File Offset: 0x0009B6B4
	public void ResetCrates()
	{
		foreach (CellDirector cellDirector in base.GetComponentsInChildren<CellDirector>())
		{
			List<GameObject> list = new List<GameObject>();
			foreach (Identifiable.Id id in this.CRATE_TYPES_TO_CLEAR)
			{
				cellDirector.Get(id, ref list);
			}
			foreach (GameObject actorObj in list)
			{
				Destroyer.DestroyActor(actorObj, "ZoneDirector.ResetCrates", false);
			}
		}
		if (this.cratePrefab == null)
		{
			Log.Warning("Zone missing crate prefab: " + base.gameObject.name, Array.Empty<object>());
			return;
		}
		List<DirectedCrateSpawner> list2 = new List<DirectedCrateSpawner>(this.crateSpawners);
		int inRange = Randoms.SHARED.GetInRange(this.minCrates, this.maxCrates + 1);
		int num = 0;
		while (num < inRange && list2.Count > 0)
		{
			Randoms.SHARED.Pluck<DirectedCrateSpawner>(list2, null).Spawn(this.cratePrefab);
			num++;
		}
	}

	// Token: 0x060029EB RID: 10731 RVA: 0x0009D5DC File Offset: 0x0009B7DC
	public void ResetGingerPatches()
	{
		if (this.gingerPatches.Count == 0)
		{
			return;
		}
		if (this.currentGingerPatchNodes.Count > 0)
		{
			foreach (GingerPatchNode gingerPatchNode in this.currentGingerPatchNodes)
			{
				gingerPatchNode.HidePatchAndReset();
				this.model.currentGingerPatchIds.Remove(gingerPatchNode.id);
			}
			this.currentGingerPatchNodes.Clear();
		}
		List<GingerPatchNode> list = new List<GingerPatchNode>(this.gingerPatches);
		int inRange = Randoms.SHARED.GetInRange(Math.Max(this.MinGingerPatches, 0), Math.Min(this.MaxGingerPatches, list.Count));
		for (int i = 0; i < inRange; i++)
		{
			GingerPatchNode gingerPatchNode2 = Randoms.SHARED.Pluck<GingerPatchNode>(list, null);
			if (gingerPatchNode2 != null)
			{
				this.currentGingerPatchNodes.Add(gingerPatchNode2);
				this.model.currentGingerPatchIds.Add(gingerPatchNode2.id);
				gingerPatchNode2.Grow();
			}
		}
	}

	// Token: 0x060029EC RID: 10732 RVA: 0x0009D6F4 File Offset: 0x0009B8F4
	private void SetCurrentGingerPatch(List<string> gingerPatchIds)
	{
		this.currentGingerPatchNodes = (from id in gingerPatchIds
		select this.gingerPatches.FirstOrDefault((GingerPatchNode patch) => string.Compare(patch.id, id, false, CultureInfo.InvariantCulture) == 0) into patch
		where patch != null
		select patch).ToList<GingerPatchNode>();
	}

	// Token: 0x060029ED RID: 10733 RVA: 0x0009D742 File Offset: 0x0009B942
	public List<GingerPatchNode> GetCurrentGingerPatches()
	{
		return this.currentGingerPatchNodes;
	}

	// Token: 0x060029EE RID: 10734 RVA: 0x0009D74C File Offset: 0x0009B94C
	private void ResetAuxItems()
	{
		if ((this.auxItems == null || this.auxItems.Length == 0) && this.maxAuxItems > 0)
		{
			Log.Warning("Zone missing aux item prefab: " + base.gameObject.name, Array.Empty<object>());
			return;
		}
		if (this.maxAuxItems <= 0)
		{
			return;
		}
		if (this.auxItemsFullClear)
		{
			foreach (CellDirector cellDirector in base.GetComponentsInChildren<CellDirector>())
			{
				List<GameObject> list = new List<GameObject>();
				foreach (ZoneDirector.AuxItemEntry auxItemEntry in this.auxItems)
				{
					cellDirector.Get(auxItemEntry.item, ref list);
				}
				foreach (GameObject actorObj in list)
				{
					Destroyer.DestroyActor(actorObj, "ZoneDirector.ResetAuxItems", false);
				}
			}
		}
		else
		{
			foreach (DirectedAuxItemSpawner directedAuxItemSpawner in this.auxItemSpawners)
			{
				directedAuxItemSpawner.UnspawnIfPresent(this.auxItemDict.Keys);
			}
		}
		List<DirectedAuxItemSpawner> list2 = new List<DirectedAuxItemSpawner>(this.auxItemSpawners);
		int inRange = Randoms.SHARED.GetInRange(this.minAuxItems, this.maxAuxItems + 1);
		int num = 0;
		while (num < inRange && list2.Count > 0)
		{
			DirectedAuxItemSpawner directedAuxItemSpawner2 = Randoms.SHARED.Pluck<DirectedAuxItemSpawner>(list2, null);
			Identifiable.Id id = this.PickAuxItem();
			GameObject prefab = this.lookupDir.GetPrefab(id);
			directedAuxItemSpawner2.Spawn(prefab);
			num++;
		}
	}

	// Token: 0x060029EF RID: 10735 RVA: 0x0009D8F8 File Offset: 0x0009BAF8
	public Identifiable.Id PickAuxItem()
	{
		return Randoms.SHARED.Pick<Identifiable.Id>(this.auxItemDict, Identifiable.Id.NONE);
	}

	// Token: 0x060029F0 RID: 10736 RVA: 0x0009D90B File Offset: 0x0009BB0B
	public ICollection<Identifiable.Id> GetAllAuxItems()
	{
		return this.auxItemDict.Keys;
	}

	// Token: 0x060029F1 RID: 10737 RVA: 0x0009D918 File Offset: 0x0009BB18
	public static bool HasAccessToZone(ZoneDirector.Zone zone)
	{
		switch (zone)
		{
		case ZoneDirector.Zone.RANCH:
		case ZoneDirector.Zone.REEF:
		case ZoneDirector.Zone.SEA:
			return true;
		case ZoneDirector.Zone.QUARRY:
			return ZoneDirector.HasProgressForZone(ProgressDirector.ProgressType.UNLOCK_QUARRY);
		case ZoneDirector.Zone.MOSS:
			return ZoneDirector.HasProgressForZone(ProgressDirector.ProgressType.UNLOCK_MOSS);
		case ZoneDirector.Zone.DESERT:
			return ZoneDirector.HasProgressForZone(ProgressDirector.ProgressType.UNLOCK_DESERT);
		case ZoneDirector.Zone.RUINS:
			return ZoneDirector.HasProgressForZone(ProgressDirector.ProgressType.UNLOCK_RUINS);
		case ZoneDirector.Zone.RUINS_TRANSITION:
			return ZoneDirector.HasProgressForZone(ProgressDirector.ProgressType.UNLOCK_MOSS) || ZoneDirector.HasProgressForZone(ProgressDirector.ProgressType.UNLOCK_QUARRY);
		case ZoneDirector.Zone.WILDS:
			return ZoneDirector.HasProgressForZone(ProgressDirector.ProgressType.UNLOCK_WILDS);
		case ZoneDirector.Zone.OGDEN_RANCH:
			return ZoneDirector.HasProgressForZone(ProgressDirector.ProgressType.UNLOCK_OGDEN_MISSIONS);
		case ZoneDirector.Zone.VALLEY:
			return ZoneDirector.HasProgressForZone(ProgressDirector.ProgressType.UNLOCK_VALLEY);
		case ZoneDirector.Zone.MOCHI_RANCH:
			return ZoneDirector.HasProgressForZone(ProgressDirector.ProgressType.UNLOCK_MOCHI_MISSIONS);
		case ZoneDirector.Zone.SLIMULATIONS:
			return ZoneDirector.HasProgressForZone(ProgressDirector.ProgressType.UNLOCK_SLIMULATIONS);
		case ZoneDirector.Zone.VIKTOR_LAB:
			return ZoneDirector.HasProgressForZone(ProgressDirector.ProgressType.UNLOCK_VIKTOR_MISSIONS);
		}
		return false;
	}

	// Token: 0x060029F2 RID: 10738 RVA: 0x0009D9D7 File Offset: 0x0009BBD7
	private static bool HasProgressForZone(ProgressDirector.ProgressType progressType)
	{
		return SRSingleton<SceneContext>.Instance.ProgressDirector.HasProgress(progressType);
	}

	// Token: 0x060029F3 RID: 10739 RVA: 0x0009D9EC File Offset: 0x0009BBEC
	public Sprite GetZoneIcon()
	{
		if (ZoneDirector.zonePediaIdLookup.ContainsKey(this.zone))
		{
			PediaDirector.IdEntry idEntry = SRSingleton<SceneContext>.Instance.PediaDirector.Get(ZoneDirector.zonePediaIdLookup[this.zone]);
			if (idEntry != null)
			{
				return idEntry.icon;
			}
		}
		return null;
	}

	// Token: 0x060029F4 RID: 10740 RVA: 0x0009DA38 File Offset: 0x0009BC38
	public static Sprite GetZoneIcon(GameObject gameObject)
	{
		Sprite result = null;
		ZoneDirector componentInParent = gameObject.GetComponentInParent<ZoneDirector>();
		if (componentInParent != null)
		{
			result = componentInParent.GetZoneIcon();
		}
		return result;
	}

	// Token: 0x060029F5 RID: 10741 RVA: 0x00003296 File Offset: 0x00001496
	public void InitModel(WorldModel model)
	{
	}

	// Token: 0x060029F6 RID: 10742 RVA: 0x0009DA5F File Offset: 0x0009BC5F
	public void SetModel(WorldModel model)
	{
		this.model = model;
		if (this.gingerPatches.Count > 0 && model.currentGingerPatchIds.Count > 0)
		{
			this.SetCurrentGingerPatch(model.currentGingerPatchIds);
		}
	}

	// Token: 0x04002907 RID: 10503
	public ZoneDirector.Zone zone;

	// Token: 0x04002908 RID: 10504
	public GameObject cratePrefab;

	// Token: 0x04002909 RID: 10505
	public int minCrates = 3;

	// Token: 0x0400290A RID: 10506
	public int maxCrates = 5;

	// Token: 0x0400290B RID: 10507
	public int minAuxItems;

	// Token: 0x0400290C RID: 10508
	public int maxAuxItems;

	// Token: 0x0400290D RID: 10509
	public bool auxItemsFullClear;

	// Token: 0x0400290E RID: 10510
	public ZoneDirector.AuxItemEntry[] auxItems;

	// Token: 0x0400290F RID: 10511
	private WorldModel model;

	// Token: 0x04002910 RID: 10512
	public static Dictionary<ZoneDirector.Zone, ZoneDirector> zones = new Dictionary<ZoneDirector.Zone, ZoneDirector>(ZoneDirector.zoneComparer);

	// Token: 0x04002911 RID: 10513
	public static ZoneDirector.ZoneComparer zoneComparer = new ZoneDirector.ZoneComparer();

	// Token: 0x04002912 RID: 10514
	public int MinGingerPatches;

	// Token: 0x04002913 RID: 10515
	public int MaxGingerPatches;

	// Token: 0x04002914 RID: 10516
	private TimeDirector timeDir;

	// Token: 0x04002915 RID: 10517
	private LookupDirector lookupDir;

	// Token: 0x04002916 RID: 10518
	private List<DirectedCrateSpawner> crateSpawners = new List<DirectedCrateSpawner>();

	// Token: 0x04002917 RID: 10519
	private List<DirectedAuxItemSpawner> auxItemSpawners = new List<DirectedAuxItemSpawner>();

	// Token: 0x04002918 RID: 10520
	private List<GingerPatchNode> gingerPatches = new List<GingerPatchNode>();

	// Token: 0x04002919 RID: 10521
	private List<GingerPatchNode> currentGingerPatchNodes = new List<GingerPatchNode>();

	// Token: 0x0400291A RID: 10522
	private List<KookadobaPatchNode> kookadobaPatches = new List<KookadobaPatchNode>();

	// Token: 0x0400291B RID: 10523
	private double nextKookadobaTime;

	// Token: 0x0400291C RID: 10524
	private static float KOOKADOBA_DELAY = 0.5f;

	// Token: 0x0400291D RID: 10525
	private readonly Identifiable.Id[] CRATE_TYPES_TO_CLEAR = new Identifiable.Id[]
	{
		Identifiable.Id.CRATE_REEF_01,
		Identifiable.Id.CRATE_QUARRY_01,
		Identifiable.Id.CRATE_MOSS_01,
		Identifiable.Id.CRATE_DESERT_01,
		Identifiable.Id.CRATE_RUINS_01,
		Identifiable.Id.CRATE_WILDS_01,
		Identifiable.Id.CRATE_PARTY_01
	};

	// Token: 0x0400291E RID: 10526
	private readonly Dictionary<Identifiable.Id, float> auxItemDict = new Dictionary<Identifiable.Id, float>(Identifiable.idComparer);

	// Token: 0x0400291F RID: 10527
	public static Dictionary<ZoneDirector.Zone, PediaDirector.Id> zonePediaIdLookup = new Dictionary<ZoneDirector.Zone, PediaDirector.Id>
	{
		{
			ZoneDirector.Zone.RANCH,
			PediaDirector.Id.THE_RANCH
		},
		{
			ZoneDirector.Zone.REEF,
			PediaDirector.Id.REEF
		},
		{
			ZoneDirector.Zone.QUARRY,
			PediaDirector.Id.QUARRY
		},
		{
			ZoneDirector.Zone.MOSS,
			PediaDirector.Id.MOSS
		},
		{
			ZoneDirector.Zone.DESERT,
			PediaDirector.Id.DESERT
		},
		{
			ZoneDirector.Zone.RUINS,
			PediaDirector.Id.RUINS
		},
		{
			ZoneDirector.Zone.RUINS_TRANSITION,
			PediaDirector.Id.RUINS
		},
		{
			ZoneDirector.Zone.SEA,
			PediaDirector.Id.SEA
		},
		{
			ZoneDirector.Zone.WILDS,
			PediaDirector.Id.WILDS
		},
		{
			ZoneDirector.Zone.OGDEN_RANCH,
			PediaDirector.Id.OGDEN_RETREAT
		},
		{
			ZoneDirector.Zone.MOCHI_RANCH,
			PediaDirector.Id.MOCHI_MANOR
		},
		{
			ZoneDirector.Zone.VALLEY,
			PediaDirector.Id.VALLEY
		},
		{
			ZoneDirector.Zone.SLIMULATIONS,
			PediaDirector.Id.SLIMULATIONS_WORLD
		},
		{
			ZoneDirector.Zone.VIKTOR_LAB,
			PediaDirector.Id.VIKTOR_LAB
		}
	};

	// Token: 0x020007D1 RID: 2001
	public enum Zone
	{
		// Token: 0x04002921 RID: 10529
		NONE = -1,
		// Token: 0x04002922 RID: 10530
		RANCH,
		// Token: 0x04002923 RID: 10531
		REEF,
		// Token: 0x04002924 RID: 10532
		QUARRY,
		// Token: 0x04002925 RID: 10533
		MOSS,
		// Token: 0x04002926 RID: 10534
		DESERT,
		// Token: 0x04002927 RID: 10535
		SEA,
		// Token: 0x04002928 RID: 10536
		RUINS = 7,
		// Token: 0x04002929 RID: 10537
		RUINS_TRANSITION,
		// Token: 0x0400292A RID: 10538
		WILDS,
		// Token: 0x0400292B RID: 10539
		OGDEN_RANCH,
		// Token: 0x0400292C RID: 10540
		VALLEY,
		// Token: 0x0400292D RID: 10541
		MOCHI_RANCH,
		// Token: 0x0400292E RID: 10542
		SLIMULATIONS,
		// Token: 0x0400292F RID: 10543
		VIKTOR_LAB
	}

	// Token: 0x020007D2 RID: 2002
	[Serializable]
	public class AuxItemEntry
	{
		// Token: 0x04002930 RID: 10544
		public Identifiable.Id item;

		// Token: 0x04002931 RID: 10545
		public float weight;
	}

	// Token: 0x020007D3 RID: 2003
	public class ZoneComparer : IEqualityComparer<ZoneDirector.Zone>
	{
		// Token: 0x060029FB RID: 10747 RVA: 0x00017781 File Offset: 0x00015981
		public bool Equals(ZoneDirector.Zone x, ZoneDirector.Zone y)
		{
			return x == y;
		}

		// Token: 0x060029FC RID: 10748 RVA: 0x00017787 File Offset: 0x00015987
		public int GetHashCode(ZoneDirector.Zone obj)
		{
			return (int)obj;
		}
	}
}
