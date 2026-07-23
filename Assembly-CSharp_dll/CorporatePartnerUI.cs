using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200055A RID: 1370
public class CorporatePartnerUI : BaseUI
{
	// Token: 0x06001C86 RID: 7302 RVA: 0x0006C9E4 File Offset: 0x0006ABE4
	public override void Awake()
	{
		base.Awake();
		this.RebuildUI();
	}

	// Token: 0x06001C87 RID: 7303 RVA: 0x0006C9F2 File Offset: 0x0006ABF2
	public void OnEnable()
	{
		base.Play(this.openCue);
	}

	// Token: 0x06001C88 RID: 7304 RVA: 0x0006CA00 File Offset: 0x0006AC00
	public void OnDisable()
	{
		base.Play(this.closeCue);
	}

	// Token: 0x06001C89 RID: 7305 RVA: 0x0006CA10 File Offset: 0x0006AC10
	public void RebuildUI()
	{
		ProgressDirector progressDirector = SRSingleton<SceneContext>.Instance.ProgressDirector;
		this.currencyText.text = SRSingleton<SceneContext>.Instance.PlayerState.GetCurrency().ToString();
		int progress = progressDirector.GetProgress(ProgressDirector.ProgressType.CORPORATE_PARTNER);
		int num = progress + 1;
		bool flag = progress > 0;
		bool flag2 = num <= this.ranks.Length;
		this.noCurrRankPlaceholder.SetActive(!flag);
		this.currRankPanel.SetActive(flag);
		if (flag)
		{
			this.currRankNumberText.text = progress.ToString();
			this.currRankTitleText.text = this.uiBundle.Get("m.partner_rank." + progress);
			this.currRankRibbonImage.sprite = this.ranks[progress - 1].rewardBanner;
		}
		this.noNextRankPlaceholder.SetActive(!flag2);
		this.nextRankPanel.SetActive(flag2);
		this.nextRankRibbon.SetActive(flag2);
		if (flag2)
		{
			this.nextRankTitleText.text = this.uiBundle.Get("m.partner_rank." + num);
			this.nextRankRibbonImage.sprite = this.ranks[num - 1].rewardBanner;
			this.nextRankNumberText.text = num.ToString();
			int num2 = this.ranks[num - 1].rewardIcons.Length;
			for (int i = 0; i < 3; i++)
			{
				if (i <= num2 - 1)
				{
					this.EnableReward(num, i);
				}
				else
				{
					this.DisableReward(i);
				}
			}
			this.costText.text = this.ranks[num - 1].cost.ToString();
		}
	}

	// Token: 0x06001C8A RID: 7306 RVA: 0x0006CBBC File Offset: 0x0006ADBC
	private void EnableReward(int rank, int rewardIndex)
	{
		this.rewardObjects[rewardIndex].SetActive(true);
		this.rewardIcons[rewardIndex].sprite = this.ranks[rank - 1].rewardIcons[rewardIndex];
		this.rewardTitles[rewardIndex].text = this.uiBundle.Get(string.Format("m.partner_rank.{0}.reward.{1}", rank, rewardIndex + 1));
	}

	// Token: 0x06001C8B RID: 7307 RVA: 0x0006CC25 File Offset: 0x0006AE25
	private void DisableReward(int rewardIndex)
	{
		this.rewardObjects[rewardIndex].SetActive(false);
	}

	// Token: 0x06001C8C RID: 7308 RVA: 0x0006CC38 File Offset: 0x0006AE38
	private void BuyLevel(ProgressDirector progressDir, int level, int cost)
	{
		PlayerState playerState = SRSingleton<SceneContext>.Instance.PlayerState;
		int progress = progressDir.GetProgress(ProgressDirector.ProgressType.CORPORATE_PARTNER);
		if (progress >= level)
		{
			base.PlayErrorCue();
			base.Error("e.already_has_corp_level", false);
			return;
		}
		if (progress < level - 1)
		{
			base.PlayErrorCue();
			base.Error("e.ineligible_for_corp_level", false);
			return;
		}
		if (playerState.GetCurrency() >= cost)
		{
			this.PlayPurchaseCue();
			playerState.SpendCurrency(cost, false);
			progressDir.AddProgress(ProgressDirector.ProgressType.CORPORATE_PARTNER);
			this.RebuildUI();
			this.PlayPurchaseFX();
			if (level >= 5)
			{
				SRSingleton<SceneContext>.Instance.PediaDirector.UnlockWithoutPopup(PediaDirector.Id.CHROMA);
			}
			if (level >= 8)
			{
				SRSingleton<SceneContext>.Instance.PediaDirector.UnlockWithoutPopup(PediaDirector.Id.SLIME_TOYS);
				return;
			}
		}
		else
		{
			base.PlayErrorCue();
			base.Error("e.insuf_coins", false);
		}
	}

	// Token: 0x06001C8D RID: 7309 RVA: 0x0006CD00 File Offset: 0x0006AF00
	public void Pedia()
	{
		if (!this.waitForPedia)
		{
			this.waitForPedia = true;
			PediaUI component = SRSingleton<SceneContext>.Instance.PediaDirector.ShowPedia(PediaDirector.Id.PARTNER).GetComponent<PediaUI>();
			component.onDestroy = (BaseUI.OnDestroyDelegate)Delegate.Combine(component.onDestroy, new BaseUI.OnDestroyDelegate(delegate()
			{
				this.waitForPedia = false;
			}));
		}
	}

	// Token: 0x06001C8E RID: 7310 RVA: 0x0006CD58 File Offset: 0x0006AF58
	public void Purchase()
	{
		if (!this.waitForPedia)
		{
			ProgressDirector progressDirector = SRSingleton<SceneContext>.Instance.ProgressDirector;
			int num = progressDirector.GetProgress(ProgressDirector.ProgressType.CORPORATE_PARTNER) + 1;
			int cost = this.ranks[num - 1].cost;
			this.BuyLevel(progressDirector, num, cost);
			SRSingleton<SceneContext>.Instance.AchievementsDirector.AddToStat(AchievementsDirector.IntStat.REWARD_LEVELS, 1);
			AnalyticsUtil.CustomEvent("ReachedRewardsLevel", new Dictionary<string, object>
			{
				{
					"level",
					num
				}
			}, true);
			SRSingleton<GameContext>.Instance.AutoSaveDirector.SaveAllNow();
		}
	}

	// Token: 0x06001C8F RID: 7311 RVA: 0x0006CDE2 File Offset: 0x0006AFE2
	public void PlayPurchaseFX()
	{
		if (this.purchaseFX != null)
		{
			UnityEngine.Object.Instantiate<GameObject>(this.purchaseFX).transform.SetParent(this.purchaseBtn.transform, false);
		}
	}

	// Token: 0x06001C90 RID: 7312 RVA: 0x0006CE13 File Offset: 0x0006B013
	protected void PlayPurchaseCue()
	{
		base.Play(this.rewardsPurchaseCue);
	}

	// Token: 0x04001B87 RID: 7047
	public CorporatePartnerUI.RankEntry[] ranks;

	// Token: 0x04001B88 RID: 7048
	public Sprite titleIcon;

	// Token: 0x04001B89 RID: 7049
	public int numLevels = 10;

	// Token: 0x04001B8A RID: 7050
	public TMP_Text costText;

	// Token: 0x04001B8B RID: 7051
	public Button learnMoreBtn;

	// Token: 0x04001B8C RID: 7052
	public Button purchaseBtn;

	// Token: 0x04001B8D RID: 7053
	public GameObject currRankPanel;

	// Token: 0x04001B8E RID: 7054
	public GameObject noCurrRankPlaceholder;

	// Token: 0x04001B8F RID: 7055
	public GameObject nextRankPanel;

	// Token: 0x04001B90 RID: 7056
	public GameObject nextRankRibbon;

	// Token: 0x04001B91 RID: 7057
	public GameObject noNextRankPlaceholder;

	// Token: 0x04001B92 RID: 7058
	public GameObject[] rewardObjects;

	// Token: 0x04001B93 RID: 7059
	public Image[] rewardIcons;

	// Token: 0x04001B94 RID: 7060
	public TMP_Text[] rewardTitles;

	// Token: 0x04001B95 RID: 7061
	public Image currRankRibbonImage;

	// Token: 0x04001B96 RID: 7062
	public TMP_Text currRankNumberText;

	// Token: 0x04001B97 RID: 7063
	public TMP_Text currRankTitleText;

	// Token: 0x04001B98 RID: 7064
	public Image nextRankRibbonImage;

	// Token: 0x04001B99 RID: 7065
	public TMP_Text nextRankNumberText;

	// Token: 0x04001B9A RID: 7066
	public TMP_Text nextRankTitleText;

	// Token: 0x04001B9B RID: 7067
	public TMP_Text currencyText;

	// Token: 0x04001B9C RID: 7068
	public GameObject purchaseFX;

	// Token: 0x04001B9D RID: 7069
	public SECTR_AudioCue rewardsPurchaseCue;

	// Token: 0x04001B9E RID: 7070
	public SECTR_AudioCue openCue;

	// Token: 0x04001B9F RID: 7071
	public SECTR_AudioCue closeCue;

	// Token: 0x04001BA0 RID: 7072
	private bool waitForPedia;

	// Token: 0x04001BA1 RID: 7073
	private const string ALREADY_HAS_LEVEL = "e.already_has_corp_level";

	// Token: 0x04001BA2 RID: 7074
	private const string INELIGIBLE_FOR_LEVEL = "e.ineligible_for_corp_level";

	// Token: 0x04001BA3 RID: 7075
	private const int CHROMA_LEVEL = 5;

	// Token: 0x04001BA4 RID: 7076
	private const int SLIME_TOYS_LEVEL = 8;

	// Token: 0x0200055B RID: 1371
	[Serializable]
	public class RankEntry
	{
		// Token: 0x04001BA5 RID: 7077
		public int cost;

		// Token: 0x04001BA6 RID: 7078
		public Sprite[] rewardIcons;

		// Token: 0x04001BA7 RID: 7079
		public Sprite rewardBanner;
	}
}
