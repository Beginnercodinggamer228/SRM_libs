using System;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x0200034E RID: 846
public class WakeUpDestination : SRBehaviour
{
	// Token: 0x060011B8 RID: 4536 RVA: 0x00046ECB File Offset: 0x000450CB
	public void Awake()
	{
		this.sceneContext = SRSingleton<SceneContext>.Instance;
		this.sceneContext.Register(this);
	}

	// Token: 0x060011B9 RID: 4537 RVA: 0x00046EE4 File Offset: 0x000450E4
	public void OnDestroy()
	{
		this.sceneContext.Deregister(this);
	}

	// Token: 0x060011BA RID: 4538 RVA: 0x00046EF2 File Offset: 0x000450F2
	public RegionRegistry.RegionSetId GetRegionSetId()
	{
		if (this.regionSetId == null)
		{
			this.regionSetId = new RegionRegistry.RegionSetId?(base.GetRequiredComponentInParent<Region>(false).setId);
		}
		return this.regionSetId.Value;
	}

	// Token: 0x040010F2 RID: 4338
	[Tooltip("Region associated with the WakeUpDestination. (unique)")]
	public RegionRegistry.RegionSetId deathRegionSetId;

	// Token: 0x040010F3 RID: 4339
	private SceneContext sceneContext;

	// Token: 0x040010F4 RID: 4340
	private RegionRegistry.RegionSetId? regionSetId;
}
