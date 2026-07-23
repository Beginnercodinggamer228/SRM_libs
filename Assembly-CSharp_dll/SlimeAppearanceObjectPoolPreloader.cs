using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000457 RID: 1111
public class SlimeAppearanceObjectPoolPreloader : MonoBehaviour
{
	// Token: 0x060016F5 RID: 5877 RVA: 0x000594FC File Offset: 0x000576FC
	public void Awake()
	{
		this.preloadsDict = this.preloads.ToDictionary((SlimeAppearanceObjectPoolPreloader.PreloadEntry p) => p.prefab, (SlimeAppearanceObjectPoolPreloader.PreloadEntry p) => p.count);
	}

	// Token: 0x060016F6 RID: 5878 RVA: 0x00059558 File Offset: 0x00057758
	public void OnEnable()
	{
		Destroyer.Destroy(this.handler, "ObjectPoolPreloader.OnEnable");
		this.handler = SlimeAppearanceObjectPool.Preload(this.preloadsDict);
	}

	// Token: 0x060016F7 RID: 5879 RVA: 0x0005957C File Offset: 0x0005777C
	public void OnDisable()
	{
		Destroyer.Destroy(this.handler, "ObjectPoolPreloader.OnDisable");
		foreach (GameObject prefab in this.preloadsDict.Keys)
		{
			SlimeAppearanceObjectPool.DestroyPooled(prefab);
		}
	}

	// Token: 0x04001629 RID: 5673
	[Tooltip("List of prefab entries to preload.")]
	public List<SlimeAppearanceObjectPoolPreloader.PreloadEntry> preloads;

	// Token: 0x0400162A RID: 5674
	private Dictionary<GameObject, int> preloadsDict;

	// Token: 0x0400162B RID: 5675
	private GameObject handler;

	// Token: 0x02000458 RID: 1112
	[Serializable]
	public class PreloadEntry
	{
		// Token: 0x0400162C RID: 5676
		[Tooltip("Prefab to preload.")]
		public GameObject prefab;

		// Token: 0x0400162D RID: 5677
		[Tooltip("Number of prefab instances to preload.")]
		public int count;
	}
}
