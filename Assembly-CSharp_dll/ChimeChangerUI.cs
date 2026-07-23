using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200054C RID: 1356
public class ChimeChangerUI : MonoBehaviour
{
	// Token: 0x06001C47 RID: 7239 RVA: 0x0006BD52 File Offset: 0x00069F52
	public void Start()
	{
		this.ShowInstrument(SRSingleton<SceneContext>.Instance.InstrumentDirector.currentInstrument);
		SRSingleton<SceneContext>.Instance.InstrumentDirector.onInstrumentChanged += this.ShowInstrument;
	}

	// Token: 0x06001C48 RID: 7240 RVA: 0x0006BD84 File Offset: 0x00069F84
	public void ShowInstrument(EchoNoteGameMetadata instrument)
	{
		this.iconImage.sprite = instrument.icon;
	}

	// Token: 0x06001C49 RID: 7241 RVA: 0x0006BD97 File Offset: 0x00069F97
	private void OnDestroy()
	{
		if (SRSingleton<SceneContext>.Instance != null && SRSingleton<SceneContext>.Instance.InstrumentDirector != null)
		{
			SRSingleton<SceneContext>.Instance.InstrumentDirector.onInstrumentChanged -= this.ShowInstrument;
		}
	}

	// Token: 0x04001B4C RID: 6988
	public Image iconImage;
}
