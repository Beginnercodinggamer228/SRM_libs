using System;
using UnityEngine.UI;

// Token: 0x0200054B RID: 1355
public class CannotStoreGifUI : SRBehaviour
{
	// Token: 0x06001C43 RID: 7235 RVA: 0x0006BCF8 File Offset: 0x00069EF8
	public void Awake()
	{
		this.bufferForGifToggle.isOn = SRSingleton<GameContext>.Instance.OptionsDirector.bufferForGif;
	}

	// Token: 0x06001C44 RID: 7236 RVA: 0x0006BD14 File Offset: 0x00069F14
	public void OK()
	{
		Destroyer.Destroy(base.gameObject, "CannotStoreGifUI.OK");
	}

	// Token: 0x06001C45 RID: 7237 RVA: 0x0006BD26 File Offset: 0x00069F26
	public void ToggleBufferForGif()
	{
		SRSingleton<GameContext>.Instance.OptionsDirector.bufferForGif = this.bufferForGifToggle.isOn;
		SRSingleton<GameContext>.Instance.AutoSaveDirector.SaveProfile();
	}

	// Token: 0x04001B4B RID: 6987
	public Toggle bufferForGifToggle;
}
