using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200004B RID: 75
[Serializable]
public sealed class ObjectPool
{
	// Token: 0x0600012E RID: 302 RVA: 0x0000A084 File Offset: 0x00008284
	public void CreateStartupPools()
	{
		if (!this.startupPoolsCreated)
		{
			this.startupPoolsCreated = true;
			ObjectPoolConfig.StartupPool[] startupPools = this.config.startupPools;
			if (startupPools != null && startupPools.Length != 0)
			{
				for (int i = 0; i < startupPools.Length; i++)
				{
					this.CreatePool(startupPools[i].prefab, startupPools[i].size, startupPools[i].maxSize);
				}
			}
		}
	}

	// Token: 0x0600012F RID: 303 RVA: 0x0000A0DF File Offset: 0x000082DF
	public void CreatePool<T>(T prefab, int initialPoolSize, int maxPoolSize) where T : Component
	{
		this.CreatePool(prefab.gameObject, initialPoolSize, maxPoolSize);
	}

	// Token: 0x06000130 RID: 304 RVA: 0x0000A0F4 File Offset: 0x000082F4
	public void CreatePool(GameObject prefab, int initialPoolSize, int maxPoolSize)
	{
		if (prefab != null && !this.pooledObjects.ContainsKey(prefab))
		{
			List<GameObject> list = new List<GameObject>();
			this.pooledObjects.Add(prefab, list);
			this.pooledObjectMaxInstances.Add(prefab, Math.Max(initialPoolSize, maxPoolSize));
			if (initialPoolSize > 0)
			{
				bool activeSelf = prefab.activeSelf;
				prefab.SetActive(false);
				Transform transform = this.poolRoot.transform;
				while (list.Count < initialPoolSize)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab);
					gameObject.transform.SetParent(transform, false);
					list.Add(gameObject);
				}
				prefab.SetActive(activeSelf);
			}
		}
	}

	// Token: 0x06000131 RID: 305 RVA: 0x0000A189 File Offset: 0x00008389
	public T Spawn<T>(T prefab, Transform parent, Vector3 position, Quaternion rotation) where T : Component
	{
		return this.Spawn(prefab.gameObject, parent, position, rotation).GetComponent<T>();
	}

	// Token: 0x06000132 RID: 306 RVA: 0x0000A1A5 File Offset: 0x000083A5
	public T Spawn<T>(T prefab, Vector3 position, Quaternion rotation) where T : Component
	{
		return this.Spawn(prefab.gameObject, null, position, rotation).GetComponent<T>();
	}

	// Token: 0x06000133 RID: 307 RVA: 0x0000A1C0 File Offset: 0x000083C0
	public T Spawn<T>(T prefab, Transform parent, Vector3 position) where T : Component
	{
		return this.Spawn(prefab.gameObject, parent, position, Quaternion.identity).GetComponent<T>();
	}

	// Token: 0x06000134 RID: 308 RVA: 0x0000A1DF File Offset: 0x000083DF
	public T Spawn<T>(T prefab, Vector3 position) where T : Component
	{
		return this.Spawn(prefab.gameObject, null, position, Quaternion.identity).GetComponent<T>();
	}

	// Token: 0x06000135 RID: 309 RVA: 0x0000A1FE File Offset: 0x000083FE
	public T Spawn<T>(T prefab, Transform parent) where T : Component
	{
		return this.Spawn(prefab.gameObject, parent, Vector3.zero, Quaternion.identity).GetComponent<T>();
	}

	// Token: 0x06000136 RID: 310 RVA: 0x0000A221 File Offset: 0x00008421
	public T Spawn<T>(T prefab) where T : Component
	{
		return this.Spawn(prefab.gameObject, null, Vector3.zero, Quaternion.identity).GetComponent<T>();
	}

	// Token: 0x06000137 RID: 311 RVA: 0x0000A244 File Offset: 0x00008444
	public GameObject Spawn(GameObject prefab, Transform parent, Vector3 position, Quaternion rotation)
	{
		List<GameObject> list;
		GameObject gameObject;
		if (this.pooledObjects.TryGetValue(prefab, out list))
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
					this.spawnedObjects.Add(gameObject, prefab);
					return gameObject;
				}
			}
			gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab);
			Transform transform2 = gameObject.transform;
			transform2.SetParent(parent, false);
			transform2.localPosition = position;
			transform2.localRotation = rotation;
			this.spawnedObjects.Add(gameObject, prefab);
			return gameObject;
		}
		gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab);
		Transform component = gameObject.GetComponent<Transform>();
		component.SetParent(parent, false);
		component.localPosition = position;
		component.localRotation = rotation;
		return gameObject;
	}

	// Token: 0x06000138 RID: 312 RVA: 0x0000A331 File Offset: 0x00008531
	public GameObject Spawn(GameObject prefab, Transform parent, Vector3 position)
	{
		return this.Spawn(prefab, parent, position, Quaternion.identity);
	}

	// Token: 0x06000139 RID: 313 RVA: 0x0000A341 File Offset: 0x00008541
	public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
	{
		return this.Spawn(prefab, null, position, rotation);
	}

	// Token: 0x0600013A RID: 314 RVA: 0x0000A34D File Offset: 0x0000854D
	public GameObject Spawn(GameObject prefab, Transform parent)
	{
		return this.Spawn(prefab, parent, Vector3.zero, Quaternion.identity);
	}

	// Token: 0x0600013B RID: 315 RVA: 0x0000A361 File Offset: 0x00008561
	public GameObject Spawn(GameObject prefab, Vector3 position)
	{
		return this.Spawn(prefab, null, position, Quaternion.identity);
	}

	// Token: 0x0600013C RID: 316 RVA: 0x0000A371 File Offset: 0x00008571
	public GameObject Spawn(GameObject prefab)
	{
		return this.Spawn(prefab, null, Vector3.zero, Quaternion.identity);
	}

	// Token: 0x0600013D RID: 317 RVA: 0x0000A385 File Offset: 0x00008585
	public void Recycle<T>(T obj) where T : Component
	{
		this.Recycle(obj.gameObject);
	}

	// Token: 0x0600013E RID: 318 RVA: 0x0000A398 File Offset: 0x00008598
	public void Recycle(GameObject obj)
	{
		GameObject prefab;
		if (this.spawnedObjects.TryGetValue(obj, out prefab) && !this.PoolHasMaxInstances(prefab))
		{
			this.Recycle(obj, prefab);
			return;
		}
		if (obj != null)
		{
			obj.transform.SetParent(null, false);
			Destroyer.Destroy(obj.gameObject, "ObjectPool.Recycle");
		}
	}

	// Token: 0x0600013F RID: 319 RVA: 0x0000A3ED File Offset: 0x000085ED
	public void RecycleAfterFrame(GameObject toRecycle)
	{
		SRSingleton<SceneContext>.Instance.StartCoroutine(this.RecycleAfterFrame_Coroutine(toRecycle));
	}

	// Token: 0x06000140 RID: 320 RVA: 0x0000A401 File Offset: 0x00008601
	private IEnumerator RecycleAfterFrame_Coroutine(GameObject toRecycle)
	{
		yield return new WaitForEndOfFrame();
		this.Recycle(toRecycle);
		yield break;
	}

	// Token: 0x06000141 RID: 321 RVA: 0x0000A417 File Offset: 0x00008617
	private bool PoolHasMaxInstances(GameObject prefab)
	{
		return this.pooledObjects[prefab].Count >= this.pooledObjectMaxInstances[prefab];
	}

	// Token: 0x06000142 RID: 322 RVA: 0x0000A43C File Offset: 0x0000863C
	private void Recycle(GameObject obj, GameObject prefab)
	{
		this.spawnedObjects.Remove(obj);
		if (obj != null)
		{
			this.pooledObjects[prefab].Add(obj);
			obj.transform.SetParent(this.poolRoot.transform, false);
			obj.SetActive(false);
		}
	}

	// Token: 0x06000143 RID: 323 RVA: 0x0000A48F File Offset: 0x0000868F
	public void RecycleAll<T>(T prefab) where T : Component
	{
		this.RecycleAll(prefab.gameObject);
	}

	// Token: 0x06000144 RID: 324 RVA: 0x0000A4A4 File Offset: 0x000086A4
	public void RecycleAll(GameObject prefab)
	{
		foreach (KeyValuePair<GameObject, GameObject> keyValuePair in this.spawnedObjects)
		{
			if (keyValuePair.Value == prefab)
			{
				ObjectPool.tempList.Add(keyValuePair.Key);
			}
		}
		for (int i = 0; i < ObjectPool.tempList.Count; i++)
		{
			this.Recycle(ObjectPool.tempList[i]);
		}
		ObjectPool.tempList.Clear();
	}

	// Token: 0x06000145 RID: 325 RVA: 0x0000A540 File Offset: 0x00008740
	public void RecycleAll()
	{
		ObjectPool.tempList.AddRange(this.spawnedObjects.Keys);
		for (int i = 0; i < ObjectPool.tempList.Count; i++)
		{
			this.Recycle(ObjectPool.tempList[i]);
		}
		ObjectPool.tempList.Clear();
	}

	// Token: 0x06000146 RID: 326 RVA: 0x0000A592 File Offset: 0x00008792
	public bool IsSpawned(GameObject obj)
	{
		return this.spawnedObjects.ContainsKey(obj);
	}

	// Token: 0x06000147 RID: 327 RVA: 0x0000A5A0 File Offset: 0x000087A0
	public int CountPooled<T>(T prefab) where T : Component
	{
		return this.CountPooled(prefab.gameObject);
	}

	// Token: 0x06000148 RID: 328 RVA: 0x0000A5B4 File Offset: 0x000087B4
	public int CountPooled(GameObject prefab)
	{
		List<GameObject> list;
		if (this.pooledObjects.TryGetValue(prefab, out list))
		{
			return list.Count;
		}
		return 0;
	}

	// Token: 0x06000149 RID: 329 RVA: 0x0000A5D9 File Offset: 0x000087D9
	public int CountSpawned<T>(T prefab) where T : Component
	{
		return this.CountSpawned(prefab.gameObject);
	}

	// Token: 0x0600014A RID: 330 RVA: 0x0000A5EC File Offset: 0x000087EC
	public int CountSpawned(GameObject prefab)
	{
		int num = 0;
		foreach (GameObject y in this.spawnedObjects.Values)
		{
			if (prefab == y)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x0600014B RID: 331 RVA: 0x0000A650 File Offset: 0x00008850
	public int CountAllPooled()
	{
		int num = 0;
		foreach (List<GameObject> list in this.pooledObjects.Values)
		{
			num += list.Count;
		}
		return num;
	}

	// Token: 0x0600014C RID: 332 RVA: 0x0000A6B0 File Offset: 0x000088B0
	public List<GameObject> GetPooled(GameObject prefab, List<GameObject> list, bool appendList)
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
		if (this.pooledObjects.TryGetValue(prefab, out collection))
		{
			list.AddRange(collection);
		}
		return list;
	}

	// Token: 0x0600014D RID: 333 RVA: 0x0000A6E8 File Offset: 0x000088E8
	public List<T> GetPooled<T>(T prefab, List<T> list, bool appendList) where T : Component
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
		if (this.pooledObjects.TryGetValue(prefab.gameObject, out list2))
		{
			for (int i = 0; i < list2.Count; i++)
			{
				list.Add(list2[i].GetComponent<T>());
			}
		}
		return list;
	}

	// Token: 0x0600014E RID: 334 RVA: 0x0000A748 File Offset: 0x00008948
	public List<GameObject> GetSpawned(GameObject prefab, List<GameObject> list, bool appendList)
	{
		if (list == null)
		{
			list = new List<GameObject>();
		}
		if (!appendList)
		{
			list.Clear();
		}
		foreach (KeyValuePair<GameObject, GameObject> keyValuePair in this.spawnedObjects)
		{
			if (keyValuePair.Value == prefab)
			{
				list.Add(keyValuePair.Key);
			}
		}
		return list;
	}

	// Token: 0x0600014F RID: 335 RVA: 0x0000A7C4 File Offset: 0x000089C4
	public List<T> GetSpawned<T>(T prefab, List<T> list, bool appendList) where T : Component
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
		foreach (KeyValuePair<GameObject, GameObject> keyValuePair in this.spawnedObjects)
		{
			if (keyValuePair.Value == gameObject)
			{
				list.Add(keyValuePair.Key.GetComponent<T>());
			}
		}
		return list;
	}

	// Token: 0x06000150 RID: 336 RVA: 0x0000A854 File Offset: 0x00008A54
	public void DestroyPooled(GameObject prefab)
	{
		List<GameObject> list;
		if (this.pooledObjects.TryGetValue(prefab, out list))
		{
			for (int i = 0; i < list.Count; i++)
			{
				Destroyer.Destroy(list[i], "ObjectPool.DestroyPooled");
			}
			list.Clear();
		}
	}

	// Token: 0x06000151 RID: 337 RVA: 0x0000A899 File Offset: 0x00008A99
	public void DestroyPooled<T>(T prefab) where T : Component
	{
		this.DestroyPooled(prefab.gameObject);
	}

	// Token: 0x06000152 RID: 338 RVA: 0x0000A8AC File Offset: 0x00008AAC
	public void DestroyAll(GameObject prefab)
	{
		this.RecycleAll(prefab);
		this.DestroyPooled(prefab);
	}

	// Token: 0x06000153 RID: 339 RVA: 0x0000A8BC File Offset: 0x00008ABC
	public void DestroyAll<T>(T prefab) where T : Component
	{
		this.DestroyAll(prefab.gameObject);
	}

	// Token: 0x06000154 RID: 340 RVA: 0x0000A8D0 File Offset: 0x00008AD0
	public GameObject Preload(Dictionary<GameObject, int> prefabs)
	{
		foreach (KeyValuePair<GameObject, int> keyValuePair in prefabs)
		{
			this.CreatePool(keyValuePair.Key, 0, keyValuePair.Value);
			int b;
			this.pooledObjectMaxInstances.TryGetValue(keyValuePair.Key, out b);
			this.pooledObjectMaxInstances[keyValuePair.Key] = Mathf.Max(keyValuePair.Value, b);
		}
		GameObject gameObject = new GameObject("ObjectPool.Preload");
		gameObject.AddComponent<ObjectPool.PreloadComponent>().Init(prefabs.Keys, this);
		gameObject.transform.SetParent(this.poolRoot.transform, false);
		return gameObject;
	}

	// Token: 0x0400017B RID: 379
	private static List<GameObject> tempList = new List<GameObject>();

	// Token: 0x0400017C RID: 380
	private Dictionary<GameObject, int> pooledObjectMaxInstances = new Dictionary<GameObject, int>();

	// Token: 0x0400017D RID: 381
	private Dictionary<GameObject, List<GameObject>> pooledObjects = new Dictionary<GameObject, List<GameObject>>();

	// Token: 0x0400017E RID: 382
	private Dictionary<GameObject, GameObject> spawnedObjects = new Dictionary<GameObject, GameObject>();

	// Token: 0x0400017F RID: 383
	public GameObject poolRoot;

	// Token: 0x04000180 RID: 384
	public ObjectPoolConfig config;

	// Token: 0x04000181 RID: 385
	private bool startupPoolsCreated;

	// Token: 0x0200004C RID: 76
	private class PreloadComponent : MonoBehaviour
	{
		// Token: 0x06000157 RID: 343 RVA: 0x0000A9C9 File Offset: 0x00008BC9
		public void Init(IEnumerable<GameObject> prefabs, ObjectPool targetPool)
		{
			this.pool = targetPool;
			this.prefabs = new List<GameObject>(prefabs);
			this.index = 0;
		}

		// Token: 0x06000158 RID: 344 RVA: 0x0000A9E8 File Offset: 0x00008BE8
		public void Update()
		{
			while (this.index < this.prefabs.Count)
			{
				GameObject gameObject = this.prefabs[this.index];
				if (!this.pool.PoolHasMaxInstances(gameObject))
				{
					this.pool.Recycle(UnityEngine.Object.Instantiate<GameObject>(gameObject), gameObject);
					return;
				}
				this.index++;
			}
			Destroyer.Destroy(base.gameObject, "PreloadComponent.Update");
		}

		// Token: 0x04000182 RID: 386
		public ObjectPool pool;

		// Token: 0x04000183 RID: 387
		private List<GameObject> prefabs;

		// Token: 0x04000184 RID: 388
		private int index;
	}
}
