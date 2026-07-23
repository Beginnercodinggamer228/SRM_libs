using System;
using UnityEngine;

// Token: 0x02000224 RID: 548
public class LandPlotLocation : IdHandler
{
	// Token: 0x06000BC4 RID: 3012 RVA: 0x00031593 File Offset: 0x0002F793
	public void Awake()
	{
		SRSingleton<SceneContext>.Instance.GameModel.RegisterLandPlot(base.id, base.gameObject);
	}

	// Token: 0x06000BC5 RID: 3013 RVA: 0x000315B0 File Offset: 0x0002F7B0
	public void OnDestroy()
	{
		if (SRSingleton<SceneContext>.Instance != null)
		{
			SRSingleton<SceneContext>.Instance.GameModel.UnregisterLandPlot(base.id);
		}
	}

	// Token: 0x06000BC6 RID: 3014 RVA: 0x000315D4 File Offset: 0x0002F7D4
	protected override string IdPrefix()
	{
		return "plot";
	}

	// Token: 0x06000BC7 RID: 3015 RVA: 0x000315DC File Offset: 0x0002F7DC
	public GameObject Replace(LandPlot oldLandPlot, GameObject replacementPrefab)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(replacementPrefab, oldLandPlot.transform.parent, false);
		gameObject.transform.position = oldLandPlot.transform.position;
		gameObject.transform.rotation = oldLandPlot.transform.rotation;
		Destroyer.Destroy(oldLandPlot.gameObject, "LandPlotUI.Replace");
		oldLandPlot.transform.parent = null;
		SRSingleton<SceneContext>.Instance.GameModel.UnregisterLandPlot(base.id);
		SRSingleton<SceneContext>.Instance.GameModel.RegisterLandPlot(base.id, base.gameObject);
		return gameObject;
	}
}
