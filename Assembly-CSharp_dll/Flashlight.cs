using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200029D RID: 669
public class Flashlight : MonoBehaviour
{
	// Token: 0x06000E19 RID: 3609 RVA: 0x000392EC File Offset: 0x000374EC
	public void Awake()
	{
		this.activateLight = base.GetComponent<Light>();
	}

	// Token: 0x06000E1A RID: 3610 RVA: 0x000392FC File Offset: 0x000374FC
	public void Update()
	{
		if (Time.timeScale == 0f)
		{
			return;
		}
		if (SRInput.Actions.light.WasPressed)
		{
			this.activateLight.enabled = !this.activateLight.enabled;
			AnalyticsUtil.CustomEvent("FlashlightToggled", new Dictionary<string, object>
			{
				{
					"FlashlightState",
					this.activateLight.enabled
				}
			}, true);
			SECTR_AudioSystem.Play(this.activateLight.enabled ? this.flashlightOn : this.flashlightOff, Vector3.zero, false);
		}
	}

	// Token: 0x04000D45 RID: 3397
	public SECTR_AudioCue flashlightOn;

	// Token: 0x04000D46 RID: 3398
	public SECTR_AudioCue flashlightOff;

	// Token: 0x04000D47 RID: 3399
	private Light activateLight;
}
