using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000580 RID: 1408
public class ExchangeRewardItemEntryUI : MonoBehaviour
{
	// Token: 0x06001D47 RID: 7495 RVA: 0x0006F390 File Offset: 0x0006D590
	public void Awake()
	{
		this.lookupDir = SRSingleton<GameContext>.Instance.LookupDirector;
		this.exchangeDir = SRSingleton<SceneContext>.Instance.ExchangeDirector;
	}

	// Token: 0x06001D48 RID: 7496 RVA: 0x0006F3B4 File Offset: 0x0006D5B4
	public void SetEntry(ExchangeDirector.ItemEntry entry)
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
			this.amountText.text = this.GetCountDisplayForReward(entry.specReward);
			return;
		}
		this.icon.sprite = this.lookupDir.GetIcon(entry.id);
		this.amountText.text = entry.count.ToString();
	}

	// Token: 0x06001D49 RID: 7497 RVA: 0x0006F44C File Offset: 0x0006D64C
	private string GetCountDisplayForReward(ExchangeDirector.NonIdentReward specReward)
	{
		if (specReward - ExchangeDirector.NonIdentReward.NEWBUCKS_SMALL <= 4)
		{
			return ExchangeBreakOnImpact.GetNewbucksRewardValue(specReward).ToString();
		}
		if (specReward != ExchangeDirector.NonIdentReward.TIME_EXTENSION_12H)
		{
			return "1";
		}
		int value = Mathf.FloorToInt((float)ExchangeBreakOnImpact.GetTimeExtensionRewardValue(specReward) * 60f);
		return SRSingleton<SceneContext>.Instance.TimeDirector.FormatTimeMinutes(new int?(value));
	}

	// Token: 0x04001C5F RID: 7263
	public Image icon;

	// Token: 0x04001C60 RID: 7264
	public Text amountText;

	// Token: 0x04001C61 RID: 7265
	private LookupDirector lookupDir;

	// Token: 0x04001C62 RID: 7266
	private ExchangeDirector exchangeDir;
}
