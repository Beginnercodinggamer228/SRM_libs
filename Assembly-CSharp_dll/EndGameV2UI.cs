using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000577 RID: 1399
public class EndGameV2UI : EndGameUI
{
	// Token: 0x06001D1E RID: 7454 RVA: 0x0006E894 File Offset: 0x0006CA94
	public override void Awake()
	{
		base.Awake();
		SRSingleton<GameContext>.Instance.MusicDirector.SetRushCreditsMode(true);
		AchievementsDirector achievementsDirector = SRSingleton<SceneContext>.Instance.AchievementsDirector;
		Dictionary<Identifiable.Id, int> gameIdDictStat = achievementsDirector.GetGameIdDictStat(AchievementsDirector.GameIdDictStat.PLORT_TYPES_SOLD);
		this.InitPlortBonuses(gameIdDictStat);
		int currency = SRSingleton<SceneContext>.Instance.PlayerState.GetCurrency();
		float scoreMultiplier = EndGameV2UI.GetScoreMultiplier(gameIdDictStat);
		int num = Mathf.CeilToInt((float)currency * scoreMultiplier);
		this.currencyText.text = string.Format("{0}", currency);
		this.plortBonusText.text = this.uiBundle.Get("m.percentage", new object[]
		{
			Mathf.Round((scoreMultiplier - 1f) * 100f)
		});
		this.scoreText.text = string.Format("{0}", num);
		AnalyticsUtil.CustomEvent("TimeLimitV2GameEnd", new Dictionary<string, object>
		{
			{
				"currency",
				currency
			},
			{
				"multiplier",
				scoreMultiplier
			},
			{
				"score",
				num
			}
		}, true);
		achievementsDirector.MaybeUpdateMaxStat(AchievementsDirector.IntStat.TIME_LIMIT_V2_CURRENCY, num);
	}

	// Token: 0x06001D1F RID: 7455 RVA: 0x0006E9B3 File Offset: 0x0006CBB3
	public override void OnDestroy()
	{
		base.OnDestroy();
		if (SRSingleton<GameContext>.Instance != null)
		{
			SRSingleton<GameContext>.Instance.MusicDirector.SetRushCreditsMode(false);
		}
	}

	// Token: 0x06001D20 RID: 7456 RVA: 0x0006E9D8 File Offset: 0x0006CBD8
	private void InitPlortBonuses(IEnumerable<KeyValuePair<Identifiable.Id, int>> plorts)
	{
		Image[] componentsInChildren = this.plortBonusLines.GetComponentsInChildren<Image>(true);
		List<Identifiable.Id> list = new List<Identifiable.Id>();
		foreach (KeyValuePair<Identifiable.Id, int> keyValuePair in plorts)
		{
			if (keyValuePair.Value >= 25)
			{
				list.Add(keyValuePair.Key);
			}
		}
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Image image = componentsInChildren[i];
			image.gameObject.SetActive(i < list.Count);
			if (image.gameObject.activeSelf)
			{
				image.sprite = SRSingleton<GameContext>.Instance.LookupDirector.GetIcon(list[i]);
			}
		}
		this.plortBonusNoneText.gameObject.SetActive(list.Count == 0);
	}

	// Token: 0x06001D21 RID: 7457 RVA: 0x0006EAB8 File Offset: 0x0006CCB8
	private static float GetScoreMultiplier(IEnumerable<KeyValuePair<Identifiable.Id, int>> plorts)
	{
		float num = 1f;
		foreach (KeyValuePair<Identifiable.Id, int> keyValuePair in plorts)
		{
			num += ((keyValuePair.Value >= 25) ? GameModeSettings.GetScoreMultiplier(keyValuePair.Key) : 0f);
		}
		return num;
	}

	// Token: 0x04001C30 RID: 7216
	[Tooltip("Text displaying the plort bonus percentage.")]
	public TMP_Text plortBonusText;

	// Token: 0x04001C31 RID: 7217
	[Tooltip("Text displaying the score.")]
	public TMP_Text scoreText;

	// Token: 0x04001C32 RID: 7218
	[Tooltip("Parent GameObject containing the plort bonus images.")]
	public GameObject plortBonusLines;

	// Token: 0x04001C33 RID: 7219
	[Tooltip("Text displayed if there are no plort bonuses.")]
	public TMP_Text plortBonusNoneText;
}
