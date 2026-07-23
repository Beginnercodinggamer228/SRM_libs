using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Token: 0x02000574 RID: 1396
public class EndGameUI : BaseUI
{
	// Token: 0x06001D15 RID: 7445 RVA: 0x0006E620 File Offset: 0x0006C820
	public override void Awake()
	{
		base.Awake();
		this.takeScreenshotButton.gameObject.SetActive(true);
		int currency = SRSingleton<SceneContext>.Instance.PlayerState.GetCurrency();
		this.currencyText.text = currency.ToString();
		AchievementsDirector achievementsDirector = SRSingleton<SceneContext>.Instance.AchievementsDirector;
		this.deathsText.text = achievementsDirector.GetGameIntStat(AchievementsDirector.GameIntStat.DEATHS).ToString();
		this.spentText.text = achievementsDirector.GetGameIntStat(AchievementsDirector.GameIntStat.CURRENCY_SPENT).ToString();
		Dictionary<Identifiable.Id, int> gameIdDictStat = achievementsDirector.GetGameIdDictStat(AchievementsDirector.GameIdDictStat.PLORT_TYPES_SOLD);
		List<EndGameUI.PlortEntry> list = new List<EndGameUI.PlortEntry>();
		foreach (KeyValuePair<Identifiable.Id, int> keyValuePair in gameIdDictStat)
		{
			list.Add(new EndGameUI.PlortEntry(keyValuePair.Key, keyValuePair.Value));
		}
		list.Sort();
		list.Reverse();
		for (int i = 0; i < this.plortLines.Length; i++)
		{
			if (list.Count > i)
			{
				this.plortLines[i].Init(list[i].id, list[i].count, list[i].price);
			}
			else
			{
				this.plortLines[i].gameObject.SetActive(false);
			}
		}
		this.noPlortsText.gameObject.SetActive(list.Count == 0);
	}

	// Token: 0x06001D16 RID: 7446 RVA: 0x0006E798 File Offset: 0x0006C998
	public void OnScreenshot()
	{
		SRSingleton<GameContext>.Instance.TakeScreenshot();
	}

	// Token: 0x06001D17 RID: 7447 RVA: 0x0006E7A4 File Offset: 0x0006C9A4
	public void OnOK()
	{
		if (SRSingleton<GameContext>.Instance.AutoSaveDirector.SaveAllNow())
		{
			SRSingleton<SceneContext>.Instance.OnSessionEnded();
			SceneManager.LoadScene("MainMenu");
		}
	}

	// Token: 0x06001D18 RID: 7448 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
	protected override bool Closeable()
	{
		return false;
	}

	// Token: 0x04001C24 RID: 7204
	public TMP_Text currencyText;

	// Token: 0x04001C25 RID: 7205
	public TMP_Text deathsText;

	// Token: 0x04001C26 RID: 7206
	public TMP_Text spentText;

	// Token: 0x04001C27 RID: 7207
	public Button takeScreenshotButton;

	// Token: 0x04001C28 RID: 7208
	public EndGameUIPlortLine[] plortLines;

	// Token: 0x04001C29 RID: 7209
	public TMP_Text noPlortsText;

	// Token: 0x02000575 RID: 1397
	private class PlortEntry : IComparable<EndGameUI.PlortEntry>
	{
		// Token: 0x06001D1A RID: 7450 RVA: 0x0006E7CC File Offset: 0x0006C9CC
		public PlortEntry(Identifiable.Id id, int count)
		{
			this.id = id;
			this.count = count;
			this.price = count * SRSingleton<SceneContext>.Instance.EconomyDirector.GetCurrValue(id).Value;
		}

		// Token: 0x06001D1B RID: 7451 RVA: 0x0006E80D File Offset: 0x0006CA0D
		public int CompareTo(EndGameUI.PlortEntry that)
		{
			return this.price.CompareTo(that.price);
		}

		// Token: 0x04001C2A RID: 7210
		public Identifiable.Id id;

		// Token: 0x04001C2B RID: 7211
		public int count;

		// Token: 0x04001C2C RID: 7212
		public int price;
	}
}
