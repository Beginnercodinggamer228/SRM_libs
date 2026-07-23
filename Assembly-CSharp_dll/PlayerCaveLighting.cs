using System;
using UnityEngine;

// Token: 0x020002AF RID: 687
public class PlayerCaveLighting : SRBehaviour, CaveTrigger.Listener
{
	// Token: 0x06000E9A RID: 3738 RVA: 0x0003B090 File Offset: 0x00039290
	public void Awake()
	{
		this.ambianceDir = SRSingleton<SceneContext>.Instance.AmbianceDirector;
	}

	// Token: 0x06000E9B RID: 3739 RVA: 0x0003B0A2 File Offset: 0x000392A2
	public void OnCaveEnter(GameObject gameObject, bool affectLighting, AmbianceDirector.Zone caveZone)
	{
		if (affectLighting)
		{
			this.ambianceDir.EnterCave(caveZone);
		}
	}

	// Token: 0x06000E9C RID: 3740 RVA: 0x0003B0B3 File Offset: 0x000392B3
	public void OnCaveExit(GameObject gameObject, bool affectLighting, AmbianceDirector.Zone caveZone)
	{
		if (affectLighting)
		{
			this.ambianceDir.ExitCave(caveZone);
		}
	}

	// Token: 0x04000DB5 RID: 3509
	private AmbianceDirector ambianceDir;
}
