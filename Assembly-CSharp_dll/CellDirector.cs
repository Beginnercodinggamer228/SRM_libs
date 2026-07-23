using System;
using System.Collections.Generic;
using System.Linq;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x020006D2 RID: 1746
public class CellDirector : SRBehaviour
{
	// Token: 0x1700024F RID: 591
	// (get) Token: 0x0600244E RID: 9294 RVA: 0x0008C0D6 File Offset: 0x0008A2D6
	// (set) Token: 0x0600244F RID: 9295 RVA: 0x0008C0DE File Offset: 0x0008A2DE
	public Region region { get; private set; }

	// Token: 0x06002450 RID: 9296 RVA: 0x0008C0E8 File Offset: 0x0008A2E8
	public virtual void Awake()
	{
		CellDirector.allCellDirectors.Add(this);
		this.rand = new Randoms();
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.nextSpawn = this.timeDir.HoursFromNowOrStart(-this.avgSpawnTimeGameHours * (float)this.targetSlimeCount / (float)this.maxPerSpawn);
		this.region = base.GetComponent<Region>();
		this.zoneDirector = base.GetComponentInParent<ZoneDirector>();
	}

	// Token: 0x06002451 RID: 9297 RVA: 0x0008C15B File Offset: 0x0008A35B
	public void OnDestroy()
	{
		CellDirector.allCellDirectors.Remove(this);
	}

	// Token: 0x06002452 RID: 9298 RVA: 0x0008C169 File Offset: 0x0008A369
	public ZoneDirector.Zone GetZoneId()
	{
		if (this.zoneDirector != null)
		{
			return this.zoneDirector.zone;
		}
		return ZoneDirector.Zone.NONE;
	}

	// Token: 0x06002453 RID: 9299 RVA: 0x0008C186 File Offset: 0x0008A386
	public void Start()
	{
		this.player = SRSingleton<SceneContext>.Instance.Player;
	}

	// Token: 0x06002454 RID: 9300 RVA: 0x0008C198 File Offset: 0x0008A398
	public virtual void ForceCheckSpawn()
	{
		this.spawnThrottleTime = 0f;
		this.nextSpawn = 0.0;
	}

	// Token: 0x06002455 RID: 9301 RVA: 0x0008C1B4 File Offset: 0x0008A3B4
	public void Update()
	{
		if (Time.time >= this.spawnThrottleTime && !this.region.Hibernated)
		{
			this.UpdateToTime(this.timeDir.WorldTime());
			if (this.identifiableIndex.GetSlimes().Count<GameObjectActorModelIdentifiableIndex.Entry>() > this.cullSlimesLimit)
			{
				this.Despawn(this.identifiableIndex.GetSlimes(), this.identifiableIndex.GetSlimes().Count<GameObjectActorModelIdentifiableIndex.Entry>() - this.cullSlimesLimit);
			}
			if (this.identifiableIndex.GetAnimals().Count<GameObjectActorModelIdentifiableIndex.Entry>() > this.cullAnimalsLimit)
			{
				this.Despawn(this.identifiableIndex.GetAnimals(), this.identifiableIndex.GetAnimals().Count<GameObjectActorModelIdentifiableIndex.Entry>() - this.cullAnimalsLimit);
			}
			this.spawnThrottleTime = Time.time + 1f;
		}
	}

	// Token: 0x06002456 RID: 9302 RVA: 0x0008C284 File Offset: 0x0008A484
	protected virtual void UpdateToTime(double worldTime)
	{
		if (TimeUtil.HasReached(worldTime, this.nextSpawn))
		{
			if (this.spawners.Count > 0 && this.NeedsMoreSlimes() && this.CanSpawnSlimes())
			{
				Dictionary<DirectedSlimeSpawner, float> dictionary = new Dictionary<DirectedSlimeSpawner, float>();
				float num = 0f;
				foreach (DirectedSlimeSpawner directedSlimeSpawner in this.spawners)
				{
					if (directedSlimeSpawner.CanSpawn(null))
					{
						dictionary[directedSlimeSpawner] = directedSlimeSpawner.directedSpawnWeight;
						num += directedSlimeSpawner.directedSpawnWeight;
					}
				}
				if (dictionary.Count > 0 && num > 0f)
				{
					DirectedSlimeSpawner directedSlimeSpawner2 = this.rand.Pick<DirectedSlimeSpawner>(dictionary, null);
					float num2 = SRSingleton<SceneContext>.Instance.ModDirector.SlimeCountFactor();
					base.StartCoroutine(directedSlimeSpawner2.Spawn(Mathf.RoundToInt((float)this.rand.GetInRange(this.minPerSpawn, this.maxPerSpawn + 1) * num2), this.rand));
				}
			}
			else if (this.HasTooManySlimes())
			{
				this.Despawn(this.identifiableIndex.GetSlimes(), Mathf.CeilToInt(0.1f * (float)this.targetSlimeCount));
			}
			if (this.animalSpawners.Count > 0 && this.NeedsMoreAnimals())
			{
				List<DirectedAnimalSpawner> list = new List<DirectedAnimalSpawner>();
				foreach (DirectedAnimalSpawner directedAnimalSpawner in this.animalSpawners)
				{
					if (directedAnimalSpawner.CanSpawn(null))
					{
						list.Add(directedAnimalSpawner);
					}
				}
				if (list.Count > 0)
				{
					DirectedAnimalSpawner directedAnimalSpawner2 = this.rand.Pick<DirectedAnimalSpawner>(list, null);
					base.StartCoroutine(directedAnimalSpawner2.Spawn(this.rand.GetInRange(this.minAnimalPerSpawn, this.maxAnimalPerSpawn + 1), this.rand));
				}
			}
			else if (this.HasTooManyAnimals())
			{
				this.Despawn(this.identifiableIndex.GetAnimals(), Mathf.CeilToInt(0.1f * (float)this.targetAnimalCount));
			}
			this.nextSpawn += (double)(this.avgSpawnTimeGameHours * 3600f * this.rand.GetInRange(0.5f, 1.5f));
		}
	}

	// Token: 0x06002457 RID: 9303 RVA: 0x0008C4F8 File Offset: 0x0008A6F8
	private void Despawn(IEnumerable<GameObjectActorModelIdentifiableIndex.Entry> actorList, int count)
	{
		if (this.isRanch)
		{
			if (actorList.Any((GameObjectActorModelIdentifiableIndex.Entry e) => Identifiable.IsSlime(e.Id)))
			{
				Log.Error("CellDirector is despawning slimes on the ranch... " + new
				{
					gameObject = base.gameObject.name,
					cullSlimesLimit = this.cullSlimesLimit,
					targetSlimeCount = this.targetSlimeCount,
					despawnFactor = this.despawnFactor,
					slimesCount = this.identifiableIndex.GetSlimeCount(),
					hasTooManySlimes = this.HasTooManySlimes()
				}, Array.Empty<object>());
				SentrySdk.CaptureMessage("CellDirector is despawning slimes on the ranch!");
			}
		}
		Dictionary<GameObject, float> dictionary = new Dictionary<GameObject, float>();
		foreach (GameObjectActorModelIdentifiableIndex.Entry entry in actorList)
		{
			dictionary[entry.GameObject] = this.GetDespawnWeight(entry.GameObject);
		}
		for (int i = 0; i < count; i++)
		{
			GameObject gameObject = this.rand.Pick<GameObject>(dictionary, null);
			dictionary.Remove(gameObject);
			Destroyer.DestroyActor(gameObject, "CellDirector.Despawn", false);
		}
	}

	// Token: 0x06002458 RID: 9304 RVA: 0x0008C60C File Offset: 0x0008A80C
	private float GetDespawnWeight(GameObject actor)
	{
		float num = (this.player.transform.position - actor.transform.position).sqrMagnitude;
		Vacuumable component = actor.GetComponent<Vacuumable>();
		if (component != null)
		{
			num *= this.GetSizeMultiplier(component.size);
		}
		return num;
	}

	// Token: 0x06002459 RID: 9305 RVA: 0x0008C662 File Offset: 0x0008A862
	private float GetSizeMultiplier(Vacuumable.Size size)
	{
		switch (size)
		{
		case Vacuumable.Size.NORMAL:
			return 1f;
		case Vacuumable.Size.LARGE:
			return 0.5f;
		case Vacuumable.Size.GIANT:
			return 0.25f;
		default:
			return 1f;
		}
	}

	// Token: 0x0600245A RID: 9306 RVA: 0x0008C68F File Offset: 0x0008A88F
	public void Register(DirectedSlimeSpawner spawner)
	{
		if (spawner.allowDirectedSpawns)
		{
			this.spawners.Add(spawner);
		}
	}

	// Token: 0x0600245B RID: 9307 RVA: 0x0008C6A5 File Offset: 0x0008A8A5
	public void Register(DirectedAnimalSpawner spawner)
	{
		this.animalSpawners.Add(spawner);
	}

	// Token: 0x0600245C RID: 9308 RVA: 0x0008C6B4 File Offset: 0x0008A8B4
	public void Register(GameObject obj, ActorModel actorModel)
	{
		Identifiable.Id ident = actorModel.ident;
		this.identifiableIndex.Register(obj, actorModel);
		if (Identifiable.IsTarr(ident))
		{
			this.tarrSlimeCount++;
		}
		if (Identifiable.IsSlime(ident) && this.onSlimeAdded != null)
		{
			this.onSlimeAdded(ident);
		}
	}

	// Token: 0x0600245D RID: 9309 RVA: 0x0008C708 File Offset: 0x0008A908
	public void Unregister(GameObject obj, ActorModel actorModel)
	{
		Identifiable.Id ident = actorModel.ident;
		this.identifiableIndex.Deregister(obj, actorModel);
		if (Identifiable.IsTarr(ident))
		{
			this.tarrSlimeCount--;
		}
		if (Identifiable.IsSlime(ident) && this.onSlimeRemoved != null)
		{
			this.onSlimeRemoved(ident);
		}
	}

	// Token: 0x0600245E RID: 9310 RVA: 0x0008C75B File Offset: 0x0008A95B
	public void Get(Identifiable.Id id, ref List<GameObject> result)
	{
		result.AddRange(from entry in this.identifiableIndex.GetObjectsByIdentifiableId(id)
		select entry.GameObject);
	}

	// Token: 0x0600245F RID: 9311 RVA: 0x0008C794 File Offset: 0x0008A994
	public void Get(Identifiable.Id id, List<GameObject> result, HashSet<GameObject> toIgnore)
	{
		this.AddUniquesFromList(this.identifiableIndex.GetObjectsByIdentifiableId(id), result, toIgnore);
	}

	// Token: 0x06002460 RID: 9312 RVA: 0x0008C7AC File Offset: 0x0008A9AC
	public void Get(IEnumerable<Identifiable.Id> ids, List<GameObjectActorModelIdentifiableIndex.Entry> result, HashSet<GameObjectActorModelIdentifiableIndex.Entry> toIgnore)
	{
		foreach (Identifiable.Id id in ids)
		{
			this.AddUniquesFromList(this.identifiableIndex.GetObjectsByIdentifiableId(id), result, toIgnore);
		}
	}

	// Token: 0x06002461 RID: 9313 RVA: 0x0008C804 File Offset: 0x0008AA04
	public void Get(IEnumerable<Identifiable.Id> ids, List<GameObject> results)
	{
		CellDirector.selectedGameObjects.Clear();
		foreach (Identifiable.Id id in ids)
		{
			this.Get(id, results, CellDirector.selectedGameObjects);
		}
		CellDirector.selectedGameObjects.Clear();
	}

	// Token: 0x06002462 RID: 9314 RVA: 0x0008C868 File Offset: 0x0008AA68
	public void GetToys(IList<GameObjectActorModelIdentifiableIndex.Entry> results, HashSet<GameObjectActorModelIdentifiableIndex.Entry> toIgnore)
	{
		this.AddUniquesFromList(this.identifiableIndex.GetToys(), results, toIgnore);
	}

	// Token: 0x06002463 RID: 9315 RVA: 0x0008C87D File Offset: 0x0008AA7D
	public void GetLargos(IList<GameObjectActorModelIdentifiableIndex.Entry> results, HashSet<GameObjectActorModelIdentifiableIndex.Entry> toIgnore)
	{
		this.AddUniquesFromList(this.identifiableIndex.GetLargos(), results, toIgnore);
	}

	// Token: 0x06002464 RID: 9316 RVA: 0x0008C892 File Offset: 0x0008AA92
	public void GetSlimes(List<GameObject> result, HashSet<GameObject> toIgnore)
	{
		this.AddUniquesFromList(this.identifiableIndex.GetSlimes(), result, toIgnore);
	}

	// Token: 0x06002465 RID: 9317 RVA: 0x0008C8A8 File Offset: 0x0008AAA8
	private void AddUniquesFromList(IList<GameObjectActorModelIdentifiableIndex.Entry> sourceList, IList<GameObjectActorModelIdentifiableIndex.Entry> targetList, HashSet<GameObjectActorModelIdentifiableIndex.Entry> targetListLookup)
	{
		for (int i = 0; i < sourceList.Count; i++)
		{
			GameObjectActorModelIdentifiableIndex.Entry item = sourceList[i];
			if (!targetListLookup.Contains(item))
			{
				targetList.Add(item);
				targetListLookup.Add(item);
			}
		}
	}

	// Token: 0x06002466 RID: 9318 RVA: 0x0008C8E8 File Offset: 0x0008AAE8
	private void AddUniquesFromList(IList<GameObjectActorModelIdentifiableIndex.Entry> sourceList, List<GameObject> targetList, HashSet<GameObject> targetListLookup)
	{
		for (int i = 0; i < sourceList.Count; i++)
		{
			GameObject gameObject = sourceList[i].GameObject;
			if (!targetListLookup.Contains(gameObject))
			{
				targetList.Add(gameObject);
				targetListLookup.Add(gameObject);
			}
		}
	}

	// Token: 0x06002467 RID: 9319 RVA: 0x0008C930 File Offset: 0x0008AB30
	public static void Get(Identifiable.Id id, RegionMember nearMember, List<GameObject> results)
	{
		CellDirector.selectedGameObjects.Clear();
		for (int i = 0; i < nearMember.regions.Count; i++)
		{
			nearMember.regions[i].cellDir.Get(id, results, CellDirector.selectedGameObjects);
		}
		CellDirector.selectedGameObjects.Clear();
	}

	// Token: 0x06002468 RID: 9320 RVA: 0x0008C984 File Offset: 0x0008AB84
	public static void Get(IEnumerable<Identifiable.Id> ids, RegionMember nearMember, List<GameObjectActorModelIdentifiableIndex.Entry> results)
	{
		CellDirector.selectedGameObjectEntries.Clear();
		for (int i = 0; i < nearMember.regions.Count; i++)
		{
			nearMember.regions[i].cellDir.Get(ids, results, CellDirector.selectedGameObjectEntries);
		}
		CellDirector.selectedGameObjectEntries.Clear();
	}

	// Token: 0x06002469 RID: 9321 RVA: 0x0008C9D8 File Offset: 0x0008ABD8
	public static void GetToysNearMember(RegionMember nearMember, IList<GameObjectActorModelIdentifiableIndex.Entry> results)
	{
		CellDirector.selectedGameObjectEntries.Clear();
		for (int i = 0; i < nearMember.regions.Count; i++)
		{
			nearMember.regions[i].cellDir.GetToys(results, CellDirector.selectedGameObjectEntries);
		}
		CellDirector.selectedGameObjectEntries.Clear();
	}

	// Token: 0x0600246A RID: 9322 RVA: 0x0008CA2C File Offset: 0x0008AC2C
	public static void GetLargosNearMember(RegionMember nearMember, IList<GameObjectActorModelIdentifiableIndex.Entry> results)
	{
		CellDirector.selectedGameObjectEntries.Clear();
		for (int i = 0; i < nearMember.regions.Count; i++)
		{
			nearMember.regions[i].cellDir.GetLargos(results, CellDirector.selectedGameObjectEntries);
		}
		CellDirector.selectedGameObjectEntries.Clear();
	}

	// Token: 0x0600246B RID: 9323 RVA: 0x0008CA80 File Offset: 0x0008AC80
	public static void GetSlimes(RegionMember nearMember, List<GameObject> results)
	{
		CellDirector.selectedGameObjects.Clear();
		for (int i = 0; i < nearMember.regions.Count; i++)
		{
			nearMember.regions[i].cellDir.GetSlimes(results, CellDirector.selectedGameObjects);
		}
		CellDirector.selectedGameObjects.Clear();
	}

	// Token: 0x0600246C RID: 9324 RVA: 0x0008CAD4 File Offset: 0x0008ACD4
	public static void UnregisterFromAll(RegionMember member, GameObject gameObj, ActorModel actorModel)
	{
		foreach (Region region in member.regions)
		{
			region.cellDir.Unregister(gameObj, actorModel);
		}
	}

	// Token: 0x0600246D RID: 9325 RVA: 0x0008CB2C File Offset: 0x0008AD2C
	public static bool IsOnRanch(RegionMember member)
	{
		if (member.regions.Count == 0)
		{
			return false;
		}
		using (List<Region>.Enumerator enumerator = member.regions.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (!enumerator.Current.cellDir.isRanch)
				{
					return false;
				}
			}
		}
		return true;
	}

	// Token: 0x0600246E RID: 9326 RVA: 0x0008CB9C File Offset: 0x0008AD9C
	public static bool IsOnHomeRanch(RegionMember member)
	{
		if (member.regions.Count == 0)
		{
			return false;
		}
		using (List<Region>.Enumerator enumerator = member.regions.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (!enumerator.Current.cellDir.isHomeRanch)
				{
					return false;
				}
			}
		}
		return true;
	}

	// Token: 0x0600246F RID: 9327 RVA: 0x0008CC0C File Offset: 0x0008AE0C
	public static bool IsInWilds(RegionMember member)
	{
		if (member.regions.Count == 0)
		{
			return false;
		}
		using (List<Region>.Enumerator enumerator = member.regions.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (!enumerator.Current.cellDir.isWilds)
				{
					return false;
				}
			}
		}
		return true;
	}

	// Token: 0x06002470 RID: 9328 RVA: 0x0008CC7C File Offset: 0x0008AE7C
	public static IEnumerable<GameObjectActorModelIdentifiableIndex.Entry> GetAllRegistered()
	{
		foreach (CellDirector cellDirector in CellDirector.allCellDirectors)
		{
			foreach (GameObjectActorModelIdentifiableIndex.Entry entry in cellDirector.identifiableIndex.GetAllRegistered())
			{
				yield return entry;
			}
			IEnumerator<GameObjectActorModelIdentifiableIndex.Entry> enumerator2 = null;
		}
		List<CellDirector>.Enumerator enumerator = default(List<CellDirector>.Enumerator);
		yield break;
		yield break;
	}

	// Token: 0x06002471 RID: 9329 RVA: 0x0008CC85 File Offset: 0x0008AE85
	protected virtual bool CanSpawnSlimes()
	{
		return this.tarrSlimeCount <= 0;
	}

	// Token: 0x06002472 RID: 9330 RVA: 0x0008CC93 File Offset: 0x0008AE93
	private bool NeedsMoreSlimes()
	{
		return this.identifiableIndex.GetSlimeCount() < this.targetSlimeCount;
	}

	// Token: 0x06002473 RID: 9331 RVA: 0x0008CCA8 File Offset: 0x0008AEA8
	private bool NeedsMoreAnimals()
	{
		return this.GetNonignoredAnimalCount() < this.targetAnimalCount;
	}

	// Token: 0x06002474 RID: 9332 RVA: 0x0008CCB8 File Offset: 0x0008AEB8
	private bool HasTooManySlimes()
	{
		return (float)this.identifiableIndex.GetSlimeCount() > (float)this.targetSlimeCount * this.despawnFactor;
	}

	// Token: 0x06002475 RID: 9333 RVA: 0x0008CCD6 File Offset: 0x0008AED6
	private bool HasTooManyAnimals()
	{
		return (float)this.GetNonignoredAnimalCount() > (float)this.targetAnimalCount * this.despawnFactor;
	}

	// Token: 0x06002476 RID: 9334 RVA: 0x0008CCF0 File Offset: 0x0008AEF0
	private int GetNonignoredAnimalCount()
	{
		if (this.ignoreCoopCorralAnimals)
		{
			int num = 0;
			foreach (GameObject gameObject in from entry in this.identifiableIndex.GetAnimals()
			select entry.GameObject)
			{
				Vector3 position = gameObject.transform.position;
				if (!CoopRegion.IsWithin(position) && !CorralRegion.IsWithin(position))
				{
					num++;
				}
			}
			return num;
		}
		return this.identifiableIndex.GetAnimalCount();
	}

	// Token: 0x04002359 RID: 9049
	public const string RANCH_HOME = "cellRanch_Home";

	// Token: 0x0400235A RID: 9050
	public const string RANCH_LAB = "cellRanch_Lab";

	// Token: 0x0400235B RID: 9051
	public int cullSlimesLimit = 250;

	// Token: 0x0400235C RID: 9052
	public int cullAnimalsLimit = 50;

	// Token: 0x0400235D RID: 9053
	public int targetSlimeCount = 100;

	// Token: 0x0400235E RID: 9054
	public int targetAnimalCount = 20;

	// Token: 0x0400235F RID: 9055
	public int minPerSpawn = 3;

	// Token: 0x04002360 RID: 9056
	public int maxPerSpawn = 5;

	// Token: 0x04002361 RID: 9057
	public int minAnimalPerSpawn = 3;

	// Token: 0x04002362 RID: 9058
	public int maxAnimalPerSpawn = 5;

	// Token: 0x04002363 RID: 9059
	public float despawnFactor = 1.2f;

	// Token: 0x04002364 RID: 9060
	public float avgSpawnTimeGameHours = 2f;

	// Token: 0x04002365 RID: 9061
	public bool isRanch;

	// Token: 0x04002366 RID: 9062
	public bool isHomeRanch;

	// Token: 0x04002367 RID: 9063
	public bool isWilds;

	// Token: 0x04002368 RID: 9064
	public AmbianceDirector.Zone ambianceZone;

	// Token: 0x04002369 RID: 9065
	public bool ignoreCoopCorralAnimals;

	// Token: 0x0400236A RID: 9066
	public CellDirector.OnSlimeAdded onSlimeAdded;

	// Token: 0x0400236B RID: 9067
	public CellDirector.OnSlimeRemoved onSlimeRemoved;

	// Token: 0x0400236C RID: 9068
	public bool notShownOnMap;

	// Token: 0x0400236D RID: 9069
	private List<DirectedSlimeSpawner> spawners = new List<DirectedSlimeSpawner>();

	// Token: 0x0400236E RID: 9070
	private List<DirectedAnimalSpawner> animalSpawners = new List<DirectedAnimalSpawner>();

	// Token: 0x0400236F RID: 9071
	protected Randoms rand;

	// Token: 0x04002370 RID: 9072
	public GameObjectActorModelIdentifiableIndex identifiableIndex = new GameObjectActorModelIdentifiableIndex();

	// Token: 0x04002372 RID: 9074
	protected int tarrSlimeCount;

	// Token: 0x04002373 RID: 9075
	private double nextSpawn = double.PositiveInfinity;

	// Token: 0x04002374 RID: 9076
	private TimeDirector timeDir;

	// Token: 0x04002375 RID: 9077
	private GameObject player;

	// Token: 0x04002376 RID: 9078
	private ZoneDirector zoneDirector;

	// Token: 0x04002377 RID: 9079
	private float spawnThrottleTime;

	// Token: 0x04002378 RID: 9080
	private const float SPAWN_THROTTLE_DELAY = 1f;

	// Token: 0x04002379 RID: 9081
	private const float PCT_OF_TARGET_TO_DESPAWN = 0.1f;

	// Token: 0x0400237A RID: 9082
	private const float PROB_DESTROY_ON_SLEEP = 0.5f;

	// Token: 0x0400237B RID: 9083
	private static List<CellDirector> allCellDirectors = new List<CellDirector>();

	// Token: 0x0400237C RID: 9084
	private static HashSet<GameObject> selectedGameObjects = new HashSet<GameObject>();

	// Token: 0x0400237D RID: 9085
	private static HashSet<GameObjectActorModelIdentifiableIndex.Entry> selectedGameObjectEntries = new HashSet<GameObjectActorModelIdentifiableIndex.Entry>();

	// Token: 0x020006D3 RID: 1747
	// (Invoke) Token: 0x0600247A RID: 9338
	public delegate void OnSlimeAdded(Identifiable.Id slimeId);

	// Token: 0x020006D4 RID: 1748
	// (Invoke) Token: 0x0600247E RID: 9342
	public delegate void OnSlimeRemoved(Identifiable.Id slimeId);
}
