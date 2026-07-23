using System;
using UnityEngine;

// Token: 0x020000EB RID: 235
public class CyclePrefabs : SRBehaviour
{
	// Token: 0x04000562 RID: 1378
	public CyclePrefabs.PrefabEntry[] prefabs;

	// Token: 0x020000EC RID: 236
	[Serializable]
	public struct PrefabEntry
	{
		// Token: 0x04000563 RID: 1379
		public GameObject prefab;

		// Token: 0x04000564 RID: 1380
		public float cameraDist;
	}
}
