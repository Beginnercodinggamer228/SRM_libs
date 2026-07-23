using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000453 RID: 1107
public sealed class SlimeAppearanceObjectPool : MonoBehaviour, SlimeAppearanceObjectProvider
{
	// Token: 0x060016C4 RID: 5828 RVA: 0x00058999 File Offset: 0x00056B99
	private void Awake()
	{
		SlimeAppearanceObjectPool._instance = this;
		if (this.startupPoolMode == SlimeAppearanceObjectPool.StartupPoolMode.Awake)
		{
			SlimeAppearanceObjectPool.CreateStartupPools();
		}
	}

	// Token: 0x060016C5 RID: 5829 RVA: 0x000589AE File Offset: 0x00056BAE
	private void Start()
	{
		if (this.startupPoolMode == SlimeAppearanceObjectPool.StartupPoolMode.Start)
		{
			SlimeAppearanceObjectPool.CreateStartupPools();
		}
	}

	// Token: 0x060016C6 RID: 5830 RVA: 0x000589C0 File Offset: 0x00056BC0
	public static void CreateStartupPools()
	{
		if (!SlimeAppearanceObjectPool.instance.startupPoolsCreated)
		{
			SlimeAppearanceObjectPool.instance.startupPoolsCreated = true;
			SlimeAppearanceObjectPool.StartupPool[] array = SlimeAppearanceObjectPool.instance.startupPools;
			if (array != null && array.Length != 0)
			{
				for (int i = 0; i < array.Length; i++)
				{
					SlimeAppearanceObjectPool.CreatePool(array[i].prefab, array[i].size, array[i].maxSize);
				}
			}
		}
	}

	// Token: 0x060016C7 RID: 5831 RVA: 0x00058A21 File Offset: 0x00056C21
	public static void CreatePool<T>(T prefab, int initialPoolSize, int maxPoolSize) where T : Component
	{
		SlimeAppearanceObjectPool.CreatePool(prefab.gameObject, initialPoolSize, maxPoolSize);
	}

	// Token: 0x060016C8 RID: 5832 RVA: 0x00058A38 File Offset: 0x00056C38
	public static void CreatePool(GameObject prefab, int initialPoolSize, int maxPoolSize)
	{
		if (prefab != null && !SlimeAppearanceObjectPool.instance.pooledObjects.ContainsKey(prefab))
		{
			List<GameObject> list = new List<GameObject>();
			SlimeAppearanceObjectPool.instance.pooledObjects.Add(prefab, list);
			SlimeAppearanceObjectPool.instance.pooledObjectMaxInstances.Add(prefab, Math.Max(initialPoolSize, maxPoolSize));
			if (initialPoolSize > 0)
			{
				bool activeSelf = prefab.activeSelf;
				prefab.SetActive(false);
				Transform transform = SlimeAppearanceObjectPool.instance.transform;
				while (list.Count < initialPoolSize)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab);
					gameObject.transform.parent = transform;
					list.Add(gameObject);
				}
				prefab.SetActive(activeSelf);
			}
		}
	}

	// Token: 0x060016C9 RID: 5833 RVA: 0x00058ADA File Offset: 0x00056CDA
	public static T Spawn<T>(T prefab, Transform parent, Vector3 position, Quaternion rotation) where T : Component
	{
		return SlimeAppearanceObjectPool.Spawn(prefab.gameObject, parent, position, rotation).GetComponent<T>();
	}

	// Token: 0x060016CA RID: 5834 RVA: 0x00058AF4 File Offset: 0x00056CF4
	public static T Spawn<T>(T prefab, Vector3 position, Quaternion rotation) where T : Component
	{
		return SlimeAppearanceObjectPool.Spawn(prefab.gameObject, null, position, rotation).GetComponent<T>();
	}

	// Token: 0x060016CB RID: 5835 RVA: 0x00058B0E File Offset: 0x00056D0E
	public static T Spawn<T>(T prefab, Transform parent, Vector3 position) where T : Component
	{
		return SlimeAppearanceObjectPool.Spawn(prefab.gameObject, parent, position, Quaternion.identity).GetComponent<T>();
	}

	// Token: 0x060016CC RID: 5836 RVA: 0x00058B2C File Offset: 0x00056D2C
	public static T Spawn<T>(T prefab, Vector3 position) where T : Component
	{
		return SlimeAppearanceObjectPool.Spawn(prefab.gameObject, null, position, Quaternion.identity).GetComponent<T>();
	}

	// Token: 0x060016CD RID: 5837 RVA: 0x00058B4A File Offset: 0x00056D4A
	public static T Spawn<T>(T prefab, Transform parent) where T : Component
	{
		return SlimeAppearanceObjectPool.Spawn(prefab.gameObject, parent, Vector3.zero, Quaternion.identity).GetComponent<T>();
	}

	// Token: 0x060016CE RID: 5838 RVA: 0x00058B6C File Offset: 0x00056D6C
	public static T Spawn<T>(T prefab) where T : Component
	{
		return SlimeAppearanceObjectPool.Spawn(prefab.gameObject, null, Vector3.zero, Quaternion.identity).GetComponent<T>();
	}

	// Token: 0x060016CF RID: 5839 RVA: 0x00058B90 File Offset: 0x00056D90
	public static GameObject Spawn(GameObject prefab, Transform parent, Vector3 position, Quaternion rotation)
	{
		List<GameObject> list;
		GameObject gameObject;
		if (SlimeAppearanceObjectPool.instance.pooledObjects.TryGetValue(prefab, out list))
		{
			gameObject = null;
			if (list.Count > 0)
			{
				while (gameObject == null && list.Count > 0)
				{
					gameObject = list[list.Count - 1];
					list.RemoveAt(list.Count - 1);
				}
				if (gameObject != null)
				{
					Transform transform = gameObject.transform;
					transform.SetParent(parent, false);
					transform.localPosition = position;
					transform.localRotation = rotation;
					gameObject.SetActive(true);
					SlimeAppearanceObjectPool.instance.spawnedObjects.Add(gameObject, prefab);
					return gameObject;
				}
			}
			gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab);
			Transform transform2 = gameObject.transform;
			transform2.SetParent(parent, false);
			transform2.localPosition = position;
			transform2.localRotation = rotation;
			SlimeAppearanceObjectPool.instance.spawnedObjects.Add(gameObject, prefab);
			return gameObject;
		}
		gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab);
		Transform component = gameObject.GetComponent<Transform>();
		component.SetParent(parent, false);
		component.localPosition = position;
		component.localRotation = rotation;
		return gameObject;
	}

	// Token: 0x060016D0 RID: 5840 RVA: 0x00058C86 File Offset: 0x00056E86
	public static GameObject Spawn(GameObject prefab, Transform parent, Vector3 position)
	{
		return SlimeAppearanceObjectPool.Spawn(prefab, parent, position, Quaternion.identity);
	}

	// Token: 0x060016D1 RID: 5841 RVA: 0x00058C95 File Offset: 0x00056E95
	public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
	{
		return SlimeAppearanceObjectPool.Spawn(prefab, null, position, rotation);
	}

	// Token: 0x060016D2 RID: 5842 RVA: 0x00058CA0 File Offset: 0x00056EA0
	public static GameObject Spawn(GameObject prefab, Transform parent)
	{
		return SlimeAppearanceObjectPool.Spawn(prefab, parent, Vector3.zero, Quaternion.identity);
	}

	// Token: 0x060016D3 RID: 5843 RVA: 0x00058CB3 File Offset: 0x00056EB3
	public static GameObject Spawn(GameObject prefab, Vector3 position)
	{
		return SlimeAppearanceObjectPool.Spawn(prefab, null, position, Quaternion.identity);
	}

	// Token: 0x060016D4 RID: 5844 RVA: 0x00058CC2 File Offset: 0x00056EC2
	public static GameObject Spawn(GameObject prefab)
	{
		return SlimeAppearanceObjectPool.Spawn(prefab, null, Vector3.zero, Quaternion.identity);
	}

	// Token: 0x060016D5 RID: 5845 RVA: 0x00058CD5 File Offset: 0x00056ED5
	public static void Recycle<T>(T obj) where T : Component
	{
		SlimeAppearanceObjectPool.Recycle(obj.gameObject);
	}

	// Token: 0x060016D6 RID: 5846 RVA: 0x00058CE8 File Offset: 0x00056EE8
	public static void Recycle(GameObject obj)
	{
		GameObject prefab;
		if (SlimeAppearanceObjectPool.instance.spawnedObjects.TryGetValue(obj, out prefab) && !SlimeAppearanceObjectPool.PoolHasMaxInstances(prefab))
		{
			SlimeAppearanceObjectPool.Recycle(obj, prefab);
			return;
		}
		obj.transform.parent = null;
		Destroyer.Destroy(obj.gameObject, "SlimeAppearanceObjectPool.Recycle");
	}

	// Token: 0x060016D7 RID: 5847 RVA: 0x00058D35 File Offset: 0x00056F35
	private static bool PoolHasMaxInstances(GameObject prefab)
	{
		return SlimeAppearanceObjectPool.instance.pooledObjects[prefab].Count >= SlimeAppearanceObjectPool.instance.pooledObjectMaxInstances[prefab];
	}

	// Token: 0x060016D8 RID: 5848 RVA: 0x00058D64 File Offset: 0x00056F64
	private static void Recycle(GameObject obj, GameObject prefab)
	{
		SlimeAppearanceObjectPool.instance.pooledObjects[prefab].Add(obj);
		SlimeAppearanceObjectPool.instance.spawnedObjects.Remove(obj);
		obj.transform.SetParent(SlimeAppearanceObjectPool.instance.transform, false);
		obj.SetActive(false);
	}

	// Token: 0x060016D9 RID: 5849 RVA: 0x00058DB5 File Offset: 0x00056FB5
	public static void RecycleAll<T>(T prefab) where T : Component
	{
		SlimeAppearanceObjectPool.RecycleAll(prefab.gameObject);
	}

	// Token: 0x060016DA RID: 5850 RVA: 0x00058DC8 File Offset: 0x00056FC8
	public static void RecycleAll(GameObject prefab)
	{
		foreach (KeyValuePair<GameObject, GameObject> keyValuePair in SlimeAppearanceObjectPool.instance.spawnedObjects)
		{
			if (keyValuePair.Value == prefab)
			{
				SlimeAppearanceObjectPool.tempList.Add(keyValuePair.Key);
			}
		}
		for (int i = 0; i < SlimeAppearanceObjectPool.tempList.Count; i++)
		{
			SlimeAppearanceObjectPool.Recycle(SlimeAppearanceObjectPool.tempList[i]);
		}
		SlimeAppearanceObjectPool.tempList.Clear();
	}

	// Token: 0x060016DB RID: 5851 RVA: 0x00058E68 File Offset: 0x00057068
	public static void RecycleAll()
	{
		SlimeAppearanceObjectPool.tempList.AddRange(SlimeAppearanceObjectPool.instance.spawnedObjects.Keys);
		for (int i = 0; i < SlimeAppearanceObjectPool.tempList.Count; i++)
		{
			SlimeAppearanceObjectPool.Recycle(SlimeAppearanceObjectPool.tempList[i]);
		}
		SlimeAppearanceObjectPool.tempList.Clear();
	}

	// Token: 0x060016DC RID: 5852 RVA: 0x00058EBD File Offset: 0x000570BD
	public static bool IsSpawned(GameObject obj)
	{
		return SlimeAppearanceObjectPool.instance.spawnedObjects.ContainsKey(obj);
	}

	// Token: 0x060016DD RID: 5853 RVA: 0x00058ECF File Offset: 0x000570CF
	public static int CountPooled<T>(T prefab) where T : Component
	{
		return SlimeAppearanceObjectPool.CountPooled(prefab.gameObject);
	}

	// Token: 0x060016DE RID: 5854 RVA: 0x00058EE4 File Offset: 0x000570E4
	public static int CountPooled(GameObject prefab)
	{
		List<GameObject> list;
		if (SlimeAppearanceObjectPool.instance.pooledObjects.TryGetValue(prefab, out list))
		{
			return list.Count;
		}
		return 0;
	}

	// Token: 0x060016DF RID: 5855 RVA: 0x00058F0D File Offset: 0x0005710D
	public static int CountSpawned<T>(T prefab) where T : Component
	{
		return SlimeAppearanceObjectPool.CountSpawned(prefab.gameObject);
	}

	// Token: 0x060016E0 RID: 5856 RVA: 0x00058F20 File Offset: 0x00057120
	public static int CountSpawned(GameObject prefab)
	{
		int num = 0;
		foreach (GameObject y in SlimeAppearanceObjectPool.instance.spawnedObjects.Values)
		{
			if (prefab == y)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x060016E1 RID: 5857 RVA: 0x00058F88 File Offset: 0x00057188
	public static int CountAllPooled()
	{
		int num = 0;
		foreach (List<GameObject> list in SlimeAppearanceObjectPool.instance.pooledObjects.Values)
		{
			num += list.Count;
		}
		return num;
	}

	// Token: 0x060016E2 RID: 5858 RVA: 0x00058FEC File Offset: 0x000571EC
	public static List<GameObject> GetPooled(GameObject prefab, List<GameObject> list, bool appendList)
	{
		if (list == null)
		{
			list = new List<GameObject>();
		}
		if (!appendList)
		{
			list.Clear();
		}
		List<GameObject> collection;
		if (SlimeAppearanceObjectPool.instance.pooledObjects.TryGetValue(prefab, out collection))
		{
			list.AddRange(collection);
		}
		return list;
	}

	// Token: 0x060016E3 RID: 5859 RVA: 0x00059028 File Offset: 0x00057228
	public static List<T> GetPooled<T>(T prefab, List<T> list, bool appendList) where T : Component
	{
		if (list == null)
		{
			list = new List<T>();
		}
		if (!appendList)
		{
			list.Clear();
		}
		List<GameObject> list2;
		if (SlimeAppearanceObjectPool.instance.pooledObjects.TryGetValue(prefab.gameObject, out list2))
		{
			for (int i = 0; i < list2.Count; i++)
			{
				list.Add(list2[i].GetComponent<T>());
			}
		}
		return list;
	}

	// Token: 0x060016E4 RID: 5860 RVA: 0x0005908C File Offset: 0x0005728C
	public static List<GameObject> GetSpawned(GameObject prefab, List<GameObject> list, bool appendList)
	{
		if (list == null)
		{
			list = new List<GameObject>();
		}
		if (!appendList)
		{
			list.Clear();
		}
		foreach (KeyValuePair<GameObject, GameObject> keyValuePair in SlimeAppearanceObjectPool.instance.spawnedObjects)
		{
			if (keyValuePair.Value == prefab)
			{
				list.Add(keyValuePair.Key);
			}
		}
		return list;
	}

	// Token: 0x060016E5 RID: 5861 RVA: 0x0005910C File Offset: 0x0005730C
	public static List<T> GetSpawned<T>(T prefab, List<T> list, bool appendList) where T : Component
	{
		if (list == null)
		{
			list = new List<T>();
		}
		if (!appendList)
		{
			list.Clear();
		}
		GameObject gameObject = prefab.gameObject;
		foreach (KeyValuePair<GameObject, GameObject> keyValuePair in SlimeAppearanceObjectPool.instance.spawnedObjects)
		{
			if (keyValuePair.Value == gameObject)
			{
				list.Add(keyValuePair.Key.GetComponent<T>());
			}
		}
		return list;
	}

	// Token: 0x060016E6 RID: 5862 RVA: 0x000591A0 File Offset: 0x000573A0
	public static void DestroyPooled(GameObject prefab)
	{
		List<GameObject> list;
		if (SlimeAppearanceObjectPool.instance.pooledObjects.TryGetValue(prefab, out list))
		{
			for (int i = 0; i < list.Count; i++)
			{
				Destroyer.Destroy(list[i], "ObjectPool.DestroyPooled");
			}
			list.Clear();
		}
	}

	// Token: 0x060016E7 RID: 5863 RVA: 0x000591E9 File Offset: 0x000573E9
	public static void DestroyPooled<T>(T prefab) where T : Component
	{
		SlimeAppearanceObjectPool.DestroyPooled(prefab.gameObject);
	}

	// Token: 0x060016E8 RID: 5864 RVA: 0x000591FB File Offset: 0x000573FB
	public static void DestroyAll(GameObject prefab)
	{
		SlimeAppearanceObjectPool.RecycleAll(prefab);
		SlimeAppearanceObjectPool.DestroyPooled(prefab);
	}

	// Token: 0x060016E9 RID: 5865 RVA: 0x00059209 File Offset: 0x00057409
	public static void DestroyAll<T>(T prefab) where T : Component
	{
		SlimeAppearanceObjectPool.DestroyAll(prefab.gameObject);
	}

	// Token: 0x170001CD RID: 461
	// (get) Token: 0x060016EA RID: 5866 RVA: 0x0005921C File Offset: 0x0005741C
	public static SlimeAppearanceObjectPool instance
	{
		get
		{
			if (SlimeAppearanceObjectPool._instance != null)
			{
				return SlimeAppearanceObjectPool._instance;
			}
			SlimeAppearanceObjectPool._instance = UnityEngine.Object.FindObjectOfType<SlimeAppearanceObjectPool>();
			if (SlimeAppearanceObjectPool._instance != null)
			{
				return SlimeAppearanceObjectPool._instance;
			}
			SlimeAppearanceObjectPool._instance = new GameObject("SlimeAppearanceObjectPool")
			{
				transform = 
				{
					localPosition = Vector3.zero,
					localRotation = Quaternion.identity,
					localScale = Vector3.one
				}
			}.AddComponent<SlimeAppearanceObjectPool>();
			return SlimeAppearanceObjectPool._instance;
		}
	}

	// Token: 0x060016EB RID: 5867 RVA: 0x000592A4 File Offset: 0x000574A4
	public List<string> CheckPooledConfiguration()
	{
		List<string> list = new List<string>();
		if (this.startupPools == null || this.startupPools.Length == 0)
		{
			list.Add("No pools are configured");
			return list;
		}
		for (int i = 0; i < this.startupPools.Length; i++)
		{
			SlimeAppearanceObjectPool.StartupPool startupPool = this.startupPools[i];
			if (startupPool == null)
			{
				list.Add(string.Format("Pool {0} is null.", i));
			}
			else if (startupPool.prefab == null)
			{
				list.Add(string.Format("Pool {0} has a null prefab.", i));
			}
			else if (startupPool.size == 0)
			{
				list.Add(string.Format("Pool {0} has a pool count of zero.", i));
			}
		}
		return list;
	}

	// Token: 0x060016EC RID: 5868 RVA: 0x00059354 File Offset: 0x00057554
	public static GameObject Preload(Dictionary<GameObject, int> prefabs)
	{
		foreach (KeyValuePair<GameObject, int> keyValuePair in prefabs)
		{
			SlimeAppearanceObjectPool.CreatePool(keyValuePair.Key, 0, keyValuePair.Value);
			int b;
			SlimeAppearanceObjectPool.instance.pooledObjectMaxInstances.TryGetValue(keyValuePair.Key, out b);
			SlimeAppearanceObjectPool.instance.pooledObjectMaxInstances[keyValuePair.Key] = Mathf.Max(keyValuePair.Value, b);
		}
		GameObject gameObject = new GameObject("SlimeAppearanceObjectPool.Preload");
		gameObject.AddComponent<SlimeAppearanceObjectPool.PreloadComponent>().Init(prefabs.Keys);
		gameObject.transform.SetParent(SlimeAppearanceObjectPool.instance.transform, false);
		return gameObject;
	}

	// Token: 0x060016ED RID: 5869 RVA: 0x0005941C File Offset: 0x0005761C
	public SlimeAppearanceObject Get(SlimeAppearanceObject appearanceObjectPrefab, GameObject targetParent)
	{
		return SlimeAppearanceObjectPool.Spawn<SlimeAppearanceObject>(appearanceObjectPrefab, targetParent.transform, appearanceObjectPrefab.transform.position, appearanceObjectPrefab.transform.rotation);
	}

	// Token: 0x060016EE RID: 5870 RVA: 0x00059440 File Offset: 0x00057640
	public void Put(SlimeAppearanceObject appearanceObjectPrefab, SlimeAppearanceObject appearanceObject)
	{
		SlimeAppearanceObjectPool.Recycle<SlimeAppearanceObject>(appearanceObject);
	}

	// Token: 0x04001617 RID: 5655
	private static SlimeAppearanceObjectPool _instance;

	// Token: 0x04001618 RID: 5656
	private static List<GameObject> tempList = new List<GameObject>();

	// Token: 0x04001619 RID: 5657
	private Dictionary<GameObject, int> pooledObjectMaxInstances = new Dictionary<GameObject, int>();

	// Token: 0x0400161A RID: 5658
	private Dictionary<GameObject, List<GameObject>> pooledObjects = new Dictionary<GameObject, List<GameObject>>();

	// Token: 0x0400161B RID: 5659
	private Dictionary<GameObject, GameObject> spawnedObjects = new Dictionary<GameObject, GameObject>();

	// Token: 0x0400161C RID: 5660
	public SlimeAppearanceObjectPool.StartupPoolMode startupPoolMode;

	// Token: 0x0400161D RID: 5661
	public SlimeAppearanceObjectPool.StartupPool[] startupPools;

	// Token: 0x0400161E RID: 5662
	private bool startupPoolsCreated;

	// Token: 0x02000454 RID: 1108
	public enum StartupPoolMode
	{
		// Token: 0x04001620 RID: 5664
		Awake,
		// Token: 0x04001621 RID: 5665
		Start,
		// Token: 0x04001622 RID: 5666
		CallManually
	}

	// Token: 0x02000455 RID: 1109
	[Serializable]
	public class StartupPool
	{
		// Token: 0x04001623 RID: 5667
		public int size;

		// Token: 0x04001624 RID: 5668
		public GameObject prefab;

		// Token: 0x04001625 RID: 5669
		public int maxSize;

		// Token: 0x04001626 RID: 5670
		public bool doesNotSelfDestruct;
	}

	// Token: 0x02000456 RID: 1110
	private class PreloadComponent : MonoBehaviour
	{
		// Token: 0x060016F2 RID: 5874 RVA: 0x0005947D File Offset: 0x0005767D
		public void Init(IEnumerable<GameObject> prefabs)
		{
			this.prefabs = new List<GameObject>(prefabs);
			this.index = 0;
		}

		// Token: 0x060016F3 RID: 5875 RVA: 0x00059494 File Offset: 0x00057694
		public void Update()
		{
			while (this.index < this.prefabs.Count)
			{
				GameObject gameObject = this.prefabs[this.index];
				if (!SlimeAppearanceObjectPool.PoolHasMaxInstances(gameObject))
				{
					SlimeAppearanceObjectPool.Recycle(UnityEngine.Object.Instantiate<GameObject>(gameObject), gameObject);
					return;
				}
				this.index++;
			}
			Destroyer.Destroy(base.gameObject, "PreloadComponent.Update");
		}

		// Token: 0x04001627 RID: 5671
		private List<GameObject> prefabs;

		// Token: 0x04001628 RID: 5672
		private int index;
	}
}
