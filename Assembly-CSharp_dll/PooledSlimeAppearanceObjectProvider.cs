using System;
using UnityEngine;

// Token: 0x02000422 RID: 1058
public class PooledSlimeAppearanceObjectProvider : SlimeAppearanceObjectProvider
{
	// Token: 0x06001608 RID: 5640 RVA: 0x000556EC File Offset: 0x000538EC
	public PooledSlimeAppearanceObjectProvider(ObjectPool poolInstance)
	{
		this.pool = poolInstance;
	}

	// Token: 0x06001609 RID: 5641 RVA: 0x000556FB File Offset: 0x000538FB
	public SlimeAppearanceObject Get(SlimeAppearanceObject appearanceObjectPrefab, GameObject targetParent)
	{
		return this.pool.Spawn<SlimeAppearanceObject>(appearanceObjectPrefab, targetParent.transform, appearanceObjectPrefab.transform.position, appearanceObjectPrefab.transform.rotation);
	}

	// Token: 0x0600160A RID: 5642 RVA: 0x00055725 File Offset: 0x00053925
	public void Put(SlimeAppearanceObject appearanceObjectPrefab, SlimeAppearanceObject appearanceObject)
	{
		this.pool.Recycle<SlimeAppearanceObject>(appearanceObject);
	}

	// Token: 0x040014F9 RID: 5369
	private ObjectPool pool;
}
