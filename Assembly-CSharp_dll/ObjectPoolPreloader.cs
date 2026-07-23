using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000052 RID: 82
public class ObjectPoolPreloader : MonoBehaviour
{
	// Token: 0x0600016C RID: 364 RVA: 0x0000AD64 File Offset: 0x00008F64
	public void Awake()
	{
		this.preloadsDict = this.preloads.ToDictionary((ObjectPoolPreloader.PreloadEntry p) => p.prefab, (ObjectPoolPreloader.PreloadEntry p) => p.count);
	}

	// Token: 0x0600016D RID: 365 RVA: 0x0000ADC0 File Offset: 0x00008FC0
	public void OnEnable()
	{
		Destroyer.Destroy(this.handler, "ObjectPoolPreloader.OnEnable");
		this.handler = SRSingleton<SceneContext>.Instance.fxPool.Preload(this.preloadsDict);
	}

	// Token: 0x0600016E RID: 366 RVA: 0x0000ADF0 File Offset: 0x00008FF0
	public void OnDisable()
	{
		Destroyer.Destroy(this.handler, "ObjectPoolPreloader.OnDisable");
		if (SRSingleton<SceneContext>.Instance != null && SRSingleton<SceneContext>.Instance.fxPool != null)
		{
			foreach (GameObject prefab in this.preloadsDict.Keys)
			{
				SRSingleton<SceneContext>.Instance.fxPool.DestroyPooled(prefab);
			}
		}
	}

	// Token: 0x04000199 RID: 409
	[Tooltip("List of prefab entries to preload.")]
	public List<ObjectPoolPreloader.PreloadEntry> preloads;

	// Token: 0x0400019A RID: 410
	private Dictionary<GameObject, int> preloadsDict;

	// Token: 0x0400019B RID: 411
	private GameObject handler;

	// Token: 0x02000053 RID: 83
	[Serializable]
	public class PreloadEntry
	{
		// Token: 0x0400019C RID: 412
		[Tooltip("Prefab to preload.")]
		public GameObject prefab;

		// Token: 0x0400019D RID: 413
		[Tooltip("Number of prefab instances to preload.")]
		public int count;
	}
}
