using System;
using Assets.Script.Util.Extensions;
using UnityEngine;

// Token: 0x020004AA RID: 1194
public class TarrSpawnFX : MonoBehaviour
{
	// Token: 0x060018E8 RID: 6376 RVA: 0x00061085 File Offset: 0x0005F285
	public void Awake()
	{
		this.aggregator = base.gameObject.GetRequiredComponentInChildren(false);
	}

	// Token: 0x060018E9 RID: 6377 RVA: 0x00061099 File Offset: 0x0005F299
	public void Start()
	{
		this.aggregator.OnSpawnBubbles += this.OnSpawnBubbles;
	}

	// Token: 0x060018EA RID: 6378 RVA: 0x000610B2 File Offset: 0x0005F2B2
	public void OnSpawnBubbles()
	{
		SRBehaviour.SpawnAndPlayFX(this.SpawnFX, base.gameObject);
	}

	// Token: 0x060018EB RID: 6379 RVA: 0x000610C6 File Offset: 0x0005F2C6
	public void Destroy()
	{
		if (this.aggregator != null)
		{
			this.aggregator.OnSpawnBubbles -= this.OnSpawnBubbles;
		}
	}

	// Token: 0x040018A2 RID: 6306
	public GameObject SpawnFX;

	// Token: 0x040018A3 RID: 6307
	private BiteEventAggregator aggregator;
}
