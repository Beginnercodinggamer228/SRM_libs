using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x0200083E RID: 2110
public class vp_PoolManager : MonoBehaviour
{
	// Token: 0x170002B0 RID: 688
	// (get) Token: 0x06002C40 RID: 11328 RVA: 0x000A708C File Offset: 0x000A528C
	public static vp_PoolManager Instance
	{
		get
		{
			return vp_PoolManager.m_Instance;
		}
	}

	// Token: 0x06002C41 RID: 11329 RVA: 0x000A7093 File Offset: 0x000A5293
	protected virtual void Awake()
	{
		vp_PoolManager.m_Instance = this;
		this.m_Transform = base.transform;
	}

	// Token: 0x06002C42 RID: 11330 RVA: 0x000A70A8 File Offset: 0x000A52A8
	protected virtual void Start()
	{
		foreach (vp_PoolManager.vp_CustomPooledObject vp_CustomPooledObject in this.CustomPrefabs)
		{
			this.AddObjects(vp_CustomPooledObject.Prefab, Vector3.zero, Quaternion.identity, vp_CustomPooledObject.Buffer);
		}
	}

	// Token: 0x06002C43 RID: 11331 RVA: 0x000A7110 File Offset: 0x000A5310
	protected virtual void OnEnable()
	{
		vp_GlobalEventReturn<UnityEngine.Object, Vector3, Quaternion, UnityEngine.Object>.Register("vp_PoolManager Instantiate", new vp_GlobalCallbackReturn<UnityEngine.Object, Vector3, Quaternion, UnityEngine.Object>(this.InstantiateInternal));
		vp_GlobalEvent<UnityEngine.Object, float>.Register("vp_PoolManager Destroy", new vp_GlobalCallback<UnityEngine.Object, float>(this.DestroyInternal));
	}

	// Token: 0x06002C44 RID: 11332 RVA: 0x000A7140 File Offset: 0x000A5340
	protected virtual void OnDisable()
	{
		vp_GlobalEventReturn<UnityEngine.Object, Vector3, Quaternion, UnityEngine.Object>.Unregister("vp_PoolManager Instantiate", new vp_GlobalCallbackReturn<UnityEngine.Object, Vector3, Quaternion, UnityEngine.Object>(this.InstantiateInternal));
		vp_GlobalEvent<UnityEngine.Object, float>.Unregister("vp_PoolManager Destroy", new vp_GlobalCallback<UnityEngine.Object, float>(this.DestroyInternal));
	}

	// Token: 0x06002C45 RID: 11333 RVA: 0x000A7170 File Offset: 0x000A5370
	public virtual void AddObjects(UnityEngine.Object obj, Vector3 position, Quaternion rotation, int amount = 1)
	{
		if (obj == null)
		{
			return;
		}
		if (!this.m_AvailableObjects.ContainsKey(obj.name))
		{
			this.m_AvailableObjects.Add(obj.name, new List<UnityEngine.Object>());
			this.m_UsedObjects.Add(obj.name, new List<UnityEngine.Object>());
		}
		for (int i = 0; i < amount; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(obj, position, rotation) as GameObject;
			gameObject.name = obj.name;
			gameObject.transform.parent = this.m_Transform;
			vp_Utility.Activate(gameObject, false);
			this.m_AvailableObjects[obj.name].Add(gameObject);
		}
	}

	// Token: 0x06002C46 RID: 11334 RVA: 0x000A721C File Offset: 0x000A541C
	protected virtual UnityEngine.Object InstantiateInternal(UnityEngine.Object original, Vector3 position, Quaternion rotation)
	{
		if (this.IgnoredPrefabs.FirstOrDefault((GameObject obj) => obj.name == original.name || obj.name == original.name + "(Clone)") != null)
		{
			return UnityEngine.Object.Instantiate(original, position, rotation);
		}
		List<UnityEngine.Object> list = null;
		List<UnityEngine.Object> list2 = null;
		if (this.m_AvailableObjects.TryGetValue(original.name, out list))
		{
			GameObject gameObject;
			for (;;)
			{
				this.m_UsedObjects.TryGetValue(original.name, out list2);
				int num = list.Count + list2.Count;
				if (this.CustomPrefabs.FirstOrDefault((vp_PoolManager.vp_CustomPooledObject obj) => obj.Prefab.name == original.name) == null && num < this.MaxAmount && list.Count == 0)
				{
					this.AddObjects(original, position, rotation, 1);
				}
				if (list.Count == 0)
				{
					gameObject = (list2.FirstOrDefault<UnityEngine.Object>() as GameObject);
					if (gameObject == null)
					{
						list2.Remove(gameObject);
					}
					else
					{
						vp_Utility.Activate(gameObject, false);
						list2.Remove(gameObject);
						list.Add(gameObject);
					}
				}
				else
				{
					gameObject = (list.FirstOrDefault<UnityEngine.Object>() as GameObject);
					if (!(gameObject == null))
					{
						break;
					}
					list.Remove(gameObject);
				}
			}
			gameObject.transform.position = position;
			gameObject.transform.rotation = rotation;
			list.Remove(gameObject);
			list2.Add(gameObject);
			vp_Utility.Activate(gameObject, true);
			return gameObject;
		}
		this.AddObjects(original, position, rotation, 1);
		return this.InstantiateInternal(original, position, rotation);
	}

	// Token: 0x06002C47 RID: 11335 RVA: 0x000A739C File Offset: 0x000A559C
	protected virtual void DestroyInternal(UnityEngine.Object obj, float t)
	{
		if (obj == null)
		{
			return;
		}
		if (this.IgnoredPrefabs.FirstOrDefault((GameObject o) => o.name == obj.name || o.name == obj.name + "(Clone)") != null || (!this.m_AvailableObjects.ContainsKey(obj.name) && !this.PoolOnDestroy))
		{
			Destroyer.Destroy(obj, t, "vp_PoolManager.DestroyInternal", false, false);
			return;
		}
		if (t != 0f)
		{
			vp_Timer.In(t, delegate()
			{
				this.DestroyInternal(obj, 0f);
			}, null);
			return;
		}
		if (!this.m_AvailableObjects.ContainsKey(obj.name))
		{
			this.AddObjects(obj, Vector3.zero, Quaternion.identity, 1);
			return;
		}
		List<UnityEngine.Object> list = null;
		List<UnityEngine.Object> list2 = null;
		this.m_AvailableObjects.TryGetValue(obj.name, out list);
		this.m_UsedObjects.TryGetValue(obj.name, out list2);
		GameObject gameObject = list2.FirstOrDefault((UnityEngine.Object o) => o.GetInstanceID() == obj.GetInstanceID()) as GameObject;
		if (gameObject == null)
		{
			return;
		}
		gameObject.transform.parent = this.m_Transform;
		vp_Utility.Activate(gameObject, false);
		list2.Remove(gameObject);
		list.Add(gameObject);
	}

	// Token: 0x04002A73 RID: 10867
	public int MaxAmount = 25;

	// Token: 0x04002A74 RID: 10868
	public bool PoolOnDestroy = true;

	// Token: 0x04002A75 RID: 10869
	public List<GameObject> IgnoredPrefabs = new List<GameObject>();

	// Token: 0x04002A76 RID: 10870
	public List<vp_PoolManager.vp_CustomPooledObject> CustomPrefabs = new List<vp_PoolManager.vp_CustomPooledObject>();

	// Token: 0x04002A77 RID: 10871
	protected Transform m_Transform;

	// Token: 0x04002A78 RID: 10872
	protected Dictionary<string, List<UnityEngine.Object>> m_AvailableObjects = new Dictionary<string, List<UnityEngine.Object>>();

	// Token: 0x04002A79 RID: 10873
	protected Dictionary<string, List<UnityEngine.Object>> m_UsedObjects = new Dictionary<string, List<UnityEngine.Object>>();

	// Token: 0x04002A7A RID: 10874
	protected static vp_PoolManager m_Instance;

	// Token: 0x0200083F RID: 2111
	[Serializable]
	public class vp_CustomPooledObject
	{
		// Token: 0x04002A7B RID: 10875
		public GameObject Prefab;

		// Token: 0x04002A7C RID: 10876
		public int Buffer = 15;

		// Token: 0x04002A7D RID: 10877
		public int MaxAmount = 25;
	}
}
