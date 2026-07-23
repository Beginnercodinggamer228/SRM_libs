using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006CF RID: 1743
public class CaveLightController : MonoBehaviour
{
	// Token: 0x06002442 RID: 9282 RVA: 0x0008BE1F File Offset: 0x0008A01F
	public void Awake()
	{
		this.controlledLight = base.GetComponent<Light>();
		this.defaultIntensity = this.controlledLight.intensity;
	}

	// Token: 0x06002443 RID: 9283 RVA: 0x0008BE3E File Offset: 0x0008A03E
	public void SetTriggerness(CaveTrigger trigger, float triggerness)
	{
		this.triggernessVals[trigger] = triggerness;
	}

	// Token: 0x06002444 RID: 9284 RVA: 0x0008BE50 File Offset: 0x0008A050
	public void Update()
	{
		float num = 0f;
		foreach (KeyValuePair<CaveTrigger, float> keyValuePair in this.triggernessVals)
		{
			if (keyValuePair.Key != null && keyValuePair.Key.enabled)
			{
				num = Mathf.Max(num, keyValuePair.Value);
			}
		}
		this.controlledLight.intensity = this.defaultIntensity * num;
		this.controlledLight.enabled = (num > 0f);
	}

	// Token: 0x04002350 RID: 9040
	private Light controlledLight;

	// Token: 0x04002351 RID: 9041
	private float defaultIntensity;

	// Token: 0x04002352 RID: 9042
	private Dictionary<CaveTrigger, float> triggernessVals = new Dictionary<CaveTrigger, float>();
}
