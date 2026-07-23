using System;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x02000731 RID: 1841
public class ManageWithRegionSet : MonoBehaviour
{
	// Token: 0x06002679 RID: 9849 RVA: 0x00093150 File Offset: 0x00091350
	public void Awake()
	{
		SRSingleton<SceneContext>.Instance.RegionRegistry.ManageWithRegionSet(base.gameObject, this.setId);
	}

	// Token: 0x0600267A RID: 9850 RVA: 0x0009316D File Offset: 0x0009136D
	public void OnDestroy()
	{
		if (SRSingleton<SceneContext>.Instance != null && SRSingleton<SceneContext>.Instance.RegionRegistry != null)
		{
			SRSingleton<SceneContext>.Instance.RegionRegistry.ReleaseFromRegionSet(base.gameObject, this.setId);
		}
	}

	// Token: 0x040025BC RID: 9660
	public RegionRegistry.RegionSetId setId;
}
