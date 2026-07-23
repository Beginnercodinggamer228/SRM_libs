using System;
using UnityEngine;

// Token: 0x020002F1 RID: 753
public class ActivateOnInstrumentsUnlocked : MonoBehaviour
{
	// Token: 0x0600101B RID: 4123 RVA: 0x00040B89 File Offset: 0x0003ED89
	public void Start()
	{
		SRSingleton<SceneContext>.Instance.InstrumentDirector.onInstrumentUnlocked += this.OnInstrumentUnlocked;
		this.OnInstrumentUnlocked();
	}

	// Token: 0x0600101C RID: 4124 RVA: 0x00040BAC File Offset: 0x0003EDAC
	public void OnDestroy()
	{
		if (SRSingleton<SceneContext>.Instance != null && SRSingleton<SceneContext>.Instance.InstrumentDirector != null)
		{
			SRSingleton<SceneContext>.Instance.InstrumentDirector.onInstrumentUnlocked -= this.OnInstrumentUnlocked;
		}
	}

	// Token: 0x0600101D RID: 4125 RVA: 0x00040BE8 File Offset: 0x0003EDE8
	private void OnInstrumentUnlocked()
	{
		base.gameObject.SetActive(SRSingleton<SceneContext>.Instance.InstrumentDirector.GetUnlockedInstruments().Count > 1);
	}
}
