using System;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x020004F2 RID: 1266
public class GlitchRegionHelper_Viktor : SRSingleton<GlitchRegionHelper_Viktor>, AmbianceDirector.TimeOfDay
{
	// Token: 0x06001A80 RID: 6784 RVA: 0x00066B04 File Offset: 0x00064D04
	public override void Awake()
	{
		base.Awake();
		SRSingleton<SceneContext>.Instance.RegionRegistry.ManageWithRegionSet(base.gameObject, RegionRegistry.RegionSetId.VIKTOR_LAB);
	}

	// Token: 0x06001A81 RID: 6785 RVA: 0x00066B22 File Offset: 0x00064D22
	public override void OnDestroy()
	{
		base.OnDestroy();
		if (SRSingleton<SceneContext>.Instance != null && SRSingleton<SceneContext>.Instance.RegionRegistry != null)
		{
			SRSingleton<SceneContext>.Instance.RegionRegistry.ReleaseFromRegionSet(base.gameObject, RegionRegistry.RegionSetId.VIKTOR_LAB);
		}
	}

	// Token: 0x06001A82 RID: 6786 RVA: 0x000665F9 File Offset: 0x000647F9
	public void OnEnable()
	{
		SRSingleton<SceneContext>.Instance.AmbianceDirector.Register(this);
	}

	// Token: 0x06001A83 RID: 6787 RVA: 0x0006660B File Offset: 0x0006480B
	public void OnDisable()
	{
		if (SRSingleton<SceneContext>.Instance != null && SRSingleton<SceneContext>.Instance.AmbianceDirector != null)
		{
			SRSingleton<SceneContext>.Instance.AmbianceDirector.Deregister(this);
		}
	}

	// Token: 0x06001A84 RID: 6788 RVA: 0x000669C6 File Offset: 0x00064BC6
	public float GetCurrentDayFraction_Position()
	{
		return SRSingleton<SceneContext>.Instance.MetadataDirector.Glitch.ambianceTimeOfDay;
	}

	// Token: 0x06001A85 RID: 6789 RVA: 0x00066B5F File Offset: 0x00064D5F
	public float GetCurrentDayFraction_Color()
	{
		return SRSingleton<SceneContext>.Instance.TimeDirector.CurrDayFraction();
	}

	// Token: 0x04001A17 RID: 6679
	[Tooltip("Reference to the GlitchTerminalActivator in the scene.")]
	public GlitchTerminalActivator activator;
}
