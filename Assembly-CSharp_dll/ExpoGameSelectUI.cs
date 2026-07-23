using System;
using UnityEngine;

// Token: 0x02000582 RID: 1410
public class ExpoGameSelectUI : BaseUI
{
	// Token: 0x06001D53 RID: 7507 RVA: 0x0006F620 File Offset: 0x0006D820
	public void LoadGame(TextAsset asset)
	{
		SRSingleton<GameContext>.Instance.AutoSaveDirector.BeginLoad("", asset.name, delegate
		{
		});
	}

	// Token: 0x06001D54 RID: 7508 RVA: 0x0006F65B File Offset: 0x0006D85B
	public override void Close()
	{
		UnityEngine.Object.Instantiate<GameObject>(this.mainMenuUIPrefab);
		base.Close();
	}

	// Token: 0x04001C69 RID: 7273
	public GameObject mainMenuUIPrefab;
}
