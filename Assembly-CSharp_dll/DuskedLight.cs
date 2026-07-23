using System;
using UnityEngine;

// Token: 0x020006ED RID: 1773
public class DuskedLight : MonoBehaviour
{
	// Token: 0x060024FE RID: 9470 RVA: 0x0008E208 File Offset: 0x0008C408
	public void Start()
	{
		SRSingleton<SceneContext>.Instance.AmbianceDirector.RegisterDuskedLight(base.GetComponent<Light>());
	}
}
