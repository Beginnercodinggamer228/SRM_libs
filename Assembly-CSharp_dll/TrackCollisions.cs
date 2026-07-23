using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002DB RID: 731
public class TrackCollisions : SRBehaviour
{
	// Token: 0x06000FA1 RID: 4001 RVA: 0x0003D96C File Offset: 0x0003BB6C
	protected virtual void OnTriggerEnter(Collider other)
	{
		if (!this.currColliders.ContainsKey(other.gameObject))
		{
			this.currColliders[other.gameObject] = new List<Collider>();
		}
		this.currColliders[other.gameObject].Add(other);
		this.gameObjSet.Add(other.gameObject);
	}

	// Token: 0x06000FA2 RID: 4002 RVA: 0x0003D9CC File Offset: 0x0003BBCC
	protected virtual void OnTriggerExit(Collider other)
	{
		if (this.currColliders.ContainsKey(other.gameObject))
		{
			this.currColliders[other.gameObject].Remove(other);
			if (this.currColliders[other.gameObject].Count == 0)
			{
				this.currColliders.Remove(other.gameObject);
				this.gameObjSet.Remove(other.gameObject);
			}
		}
	}

	// Token: 0x06000FA3 RID: 4003 RVA: 0x0003DA40 File Offset: 0x0003BC40
	public HashSet<GameObject> CurrColliders()
	{
		foreach (KeyValuePair<GameObject, List<Collider>> keyValuePair in this.currColliders)
		{
			foreach (Collider collider in keyValuePair.Value)
			{
				if (!TrackCollisions.RemovePredicate(collider))
				{
					this.local_collidersToKeep.Add(collider);
				}
			}
			if (this.local_collidersToKeep.Count == 0)
			{
				this.local_gameObjsToRemove.Add(keyValuePair.Key);
			}
			else
			{
				keyValuePair.Value.Clear();
				foreach (Collider item in this.local_collidersToKeep)
				{
					keyValuePair.Value.Add(item);
				}
			}
			this.local_collidersToKeep.Clear();
		}
		foreach (GameObject gameObject in this.local_gameObjsToRemove)
		{
			this.gameObjSet.Remove(gameObject);
			this.currColliders.Remove(gameObject);
		}
		this.local_gameObjsToRemove.Clear();
		foreach (GameObject gameObject2 in this.gameObjSet)
		{
			if (TrackCollisions.RemovePredicate(gameObject2))
			{
				this.local_gameObjsToRemove.Add(gameObject2);
			}
		}
		foreach (GameObject item2 in this.local_gameObjsToRemove)
		{
			this.gameObjSet.Remove(item2);
		}
		this.local_gameObjsToRemove.Clear();
		return this.gameObjSet;
	}

	// Token: 0x06000FA4 RID: 4004 RVA: 0x0003DC78 File Offset: 0x0003BE78
	private static bool RemovePredicate(Collider collider)
	{
		return collider == null || !collider.enabled || TrackCollisions.RemovePredicate(collider.gameObject);
	}

	// Token: 0x06000FA5 RID: 4005 RVA: 0x0003DC98 File Offset: 0x0003BE98
	private static bool RemovePredicate(GameObject go)
	{
		return go == null || !go.activeInHierarchy;
	}

	// Token: 0x04000E62 RID: 3682
	private Dictionary<GameObject, List<Collider>> currColliders = new Dictionary<GameObject, List<Collider>>();

	// Token: 0x04000E63 RID: 3683
	private HashSet<GameObject> gameObjSet = new HashSet<GameObject>();

	// Token: 0x04000E64 RID: 3684
	private List<GameObject> local_gameObjsToRemove = new List<GameObject>(50);

	// Token: 0x04000E65 RID: 3685
	private List<Collider> local_collidersToKeep = new List<Collider>(50);

	// Token: 0x04000E66 RID: 3686
	private HashSet<GameObject> emptySet = new HashSet<GameObject>();
}
