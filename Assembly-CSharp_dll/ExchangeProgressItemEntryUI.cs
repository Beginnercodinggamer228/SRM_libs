using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200057E RID: 1406
public class ExchangeProgressItemEntryUI : MonoBehaviour
{
	// Token: 0x06001D3C RID: 7484 RVA: 0x0006F133 File Offset: 0x0006D333
	public void Awake()
	{
		this.lookupDir = SRSingleton<GameContext>.Instance.LookupDirector;
		this.exchangeDir = SRSingleton<SceneContext>.Instance.ExchangeDirector;
		this.uiBundle = SRSingleton<GameContext>.Instance.MessageDirector.GetBundle("ui");
	}

	// Token: 0x06001D3D RID: 7485 RVA: 0x0006F170 File Offset: 0x0006D370
	public void SetEntry(ExchangeDirector.RequestedItemEntry entry)
	{
		if (entry == null)
		{
			base.gameObject.SetActive(false);
			return;
		}
		base.gameObject.SetActive(true);
		if (entry.specReward != ExchangeDirector.NonIdentReward.NONE)
		{
			this.icon.sprite = this.exchangeDir.GetSpecRewardIcon(entry.specReward);
		}
		else
		{
			this.icon.sprite = this.lookupDir.GetIcon(entry.id);
		}
		this.progressText.text = this.uiBundle.Get("l.exchange_progress", new object[]
		{
			entry.progress,
			entry.count
		});
	}

	// Token: 0x04001C54 RID: 7252
	public Image icon;

	// Token: 0x04001C55 RID: 7253
	public Text progressText;

	// Token: 0x04001C56 RID: 7254
	private LookupDirector lookupDir;

	// Token: 0x04001C57 RID: 7255
	private ExchangeDirector exchangeDir;

	// Token: 0x04001C58 RID: 7256
	private MessageBundle uiBundle;
}
