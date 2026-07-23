using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000070 RID: 112
public class CFX_SpawnSystem : MonoBehaviour
{
	// Token: 0x060001ED RID: 493 RVA: 0x0000E64C File Offset: 0x0000C84C
	public static GameObject GetNextObject(GameObject sourceObj, bool activateObject = true)
	{
		int instanceID = sourceObj.GetInstanceID();
		if (!CFX_SpawnSystem.instance.poolCursors.ContainsKey(instanceID))
		{
			Debug.LogError(string.Concat(new object[]
			{
				"[CFX_SpawnSystem.GetNextPoolObject()] Object hasn't been preloaded: ",
				sourceObj.name,
				" (ID:",
				instanceID,
				")"
			}));
			return null;
		}
		int index = CFX_SpawnSystem.instance.poolCursors[instanceID];
		Dictionary<int, int> dictionary = CFX_SpawnSystem.instance.poolCursors;
		int key = instanceID;
		int num = dictionary[key];
		dictionary[key] = num + 1;
		if (CFX_SpawnSystem.instance.poolCursors[instanceID] >= CFX_SpawnSystem.instance.instantiatedObjects[instanceID].Count)
		{
			CFX_SpawnSystem.instance.poolCursors[instanceID] = 0;
		}
		GameObject gameObject = CFX_SpawnSystem.instance.instantiatedObjects[instanceID][index];
		if (activateObject)
		{
			gameObject.SetActive(true);
		}
		return gameObject;
	}

	// Token: 0x060001EE RID: 494 RVA: 0x0000E738 File Offset: 0x0000C938
	public static void PreloadObject(GameObject sourceObj, int poolSize = 1)
	{
		CFX_SpawnSystem.instance.addObjectToPool(sourceObj, poolSize);
	}

	// Token: 0x060001EF RID: 495 RVA: 0x0000E746 File Offset: 0x0000C946
	public static void UnloadObjects(GameObject sourceObj)
	{
		CFX_SpawnSystem.instance.removeObjectsFromPool(sourceObj);
	}

	// Token: 0x17000023 RID: 35
	// (get) Token: 0x060001F0 RID: 496 RVA: 0x0000E753 File Offset: 0x0000C953
	public static bool AllObjectsLoaded
	{
		get
		{
			return CFX_SpawnSystem.instance.allObjectsLoaded;
		}
	}

	// Token: 0x060001F1 RID: 497 RVA: 0x0000E760 File Offset: 0x0000C960
	private void addObjectToPool(GameObject sourceObject, int number)
	{
		int instanceID = sourceObject.GetInstanceID();
		if (!this.instantiatedObjects.ContainsKey(instanceID))
		{
			this.instantiatedObjects.Add(instanceID, new List<GameObject>());
			this.poolCursors.Add(instanceID, 0);
		}
		for (int i = 0; i < number; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(sourceObject);
			gameObject.SetActive(false);
			CFX_AutoDestructShuriken[] componentsInChildren = gameObject.GetComponentsInChildren<CFX_AutoDestructShuriken>(true);
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				componentsInChildren[j].OnlyDeactivate = true;
			}
			CFX_LightIntensityFade[] componentsInChildren2 = gameObject.GetComponentsInChildren<CFX_LightIntensityFade>(true);
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				componentsInChildren2[j].autodestruct = false;
			}
			this.instantiatedObjects[instanceID].Add(gameObject);
			if (this.hideObjectsInHierarchy)
			{
				gameObject.hideFlags = HideFlags.HideInHierarchy;
			}
		}
	}

	// Token: 0x060001F2 RID: 498 RVA: 0x0000E82C File Offset: 0x0000CA2C
	private void removeObjectsFromPool(GameObject sourceObject)
	{
		int instanceID = sourceObject.GetInstanceID();
		if (!this.instantiatedObjects.ContainsKey(instanceID))
		{
			Debug.LogWarning(string.Concat(new object[]
			{
				"[CFX_SpawnSystem.removeObjectsFromPool()] There aren't any preloaded object for: ",
				sourceObject.name,
				" (ID:",
				instanceID,
				")"
			}));
			return;
		}
		for (int i = this.instantiatedObjects[instanceID].Count - 1; i >= 0; i--)
		{
			UnityEngine.Object @object = this.instantiatedObjects[instanceID][i];
			this.instantiatedObjects[instanceID].RemoveAt(i);
			Destroyer.Destroy(@object, "CFX_SpawnSystem.removeObjectsFromPool");
		}
		this.instantiatedObjects.Remove(instanceID);
		this.poolCursors.Remove(instanceID);
	}

	// Token: 0x060001F3 RID: 499 RVA: 0x0000E8EF File Offset: 0x0000CAEF
	private void Awake()
	{
		if (CFX_SpawnSystem.instance != null)
		{
			Debug.LogWarning("CFX_SpawnSystem: There should only be one instance of CFX_SpawnSystem per Scene!");
		}
		CFX_SpawnSystem.instance = this;
	}

	// Token: 0x060001F4 RID: 500 RVA: 0x0000E910 File Offset: 0x0000CB10
	private void Start()
	{
		this.allObjectsLoaded = false;
		for (int i = 0; i < this.objectsToPreload.Length; i++)
		{
			CFX_SpawnSystem.PreloadObject(this.objectsToPreload[i], this.objectsToPreloadTimes[i]);
		}
		this.allObjectsLoaded = true;
	}

	// Token: 0x0400024B RID: 587
	private static CFX_SpawnSystem instance;

	// Token: 0x0400024C RID: 588
	public GameObject[] objectsToPreload = new GameObject[0];

	// Token: 0x0400024D RID: 589
	public int[] objectsToPreloadTimes = new int[0];

	// Token: 0x0400024E RID: 590
	public bool hideObjectsInHierarchy;

	// Token: 0x0400024F RID: 591
	private bool allObjectsLoaded;

	// Token: 0x04000250 RID: 592
	private Dictionary<int, List<GameObject>> instantiatedObjects = new Dictionary<int, List<GameObject>>();

	// Token: 0x04000251 RID: 593
	private Dictionary<int, int> poolCursors = new Dictionary<int, int>();
}
