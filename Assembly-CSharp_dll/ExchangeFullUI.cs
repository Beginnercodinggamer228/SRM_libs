using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200057B RID: 1403
public class ExchangeFullUI : BaseUI
{
	// Token: 0x06001D2D RID: 7469 RVA: 0x0006ED14 File Offset: 0x0006CF14
	public override void Awake()
	{
		base.Awake();
		this.exchangeDir = SRSingleton<SceneContext>.Instance.ExchangeDirector;
		ExchangeDirector exchangeDirector = this.exchangeDir;
		exchangeDirector.onOfferChanged = (ExchangeDirector.OnOfferChanged)Delegate.Combine(exchangeDirector.onOfferChanged, new ExchangeDirector.OnOfferChanged(this.OnOfferChanged));
		this.exchangeBundle = SRSingleton<GameContext>.Instance.MessageDirector.GetBundle("exchange");
	}

	// Token: 0x06001D2E RID: 7470 RVA: 0x0006ED78 File Offset: 0x0006CF78
	public void Start()
	{
		this.OnOfferChanged();
	}

	// Token: 0x06001D2F RID: 7471 RVA: 0x0006ED80 File Offset: 0x0006CF80
	public override void OnDestroy()
	{
		base.OnDestroy();
		ExchangeDirector exchangeDirector = this.exchangeDir;
		exchangeDirector.onOfferChanged = (ExchangeDirector.OnOfferChanged)Delegate.Remove(exchangeDirector.onOfferChanged, new ExchangeDirector.OnOfferChanged(this.OnOfferChanged));
	}

	// Token: 0x06001D30 RID: 7472 RVA: 0x0006EDB0 File Offset: 0x0006CFB0
	public void OnOfferChanged()
	{
		List<ExchangeDirector.RequestedItemEntry> offerRequests = this.exchangeDir.GetOfferRequests(ExchangeDirector.OfferType.GENERAL);
		List<ExchangeDirector.ItemEntry> offerRewards = this.exchangeDir.GetOfferRewards(ExchangeDirector.OfferType.GENERAL);
		if (offerRequests == null || offerRewards == null)
		{
			this.noRequestPanel.SetActive(true);
			this.mainOfferPanel.SetActive(false);
			for (int i = 0; i < this.requestItems.Length; i++)
			{
				this.requestItems[i].SetEntry(null);
			}
			for (int j = 0; j < this.rewardItems.Length; j++)
			{
				this.rewardItems[j].SetEntry(null);
			}
		}
		else
		{
			this.noRequestPanel.SetActive(false);
			this.mainOfferPanel.SetActive(true);
			for (int k = 0; k < this.requestItems.Length; k++)
			{
				this.requestItems[k].SetEntry((offerRequests.Count > k) ? offerRequests[k] : null);
			}
			for (int l = 0; l < this.rewardItems.Length; l++)
			{
				this.rewardItems[l].SetEntry((offerRewards.Count > l) ? offerRewards[l] : null);
			}
		}
		string offerRancherId = this.exchangeDir.GetOfferRancherId(ExchangeDirector.OfferType.GENERAL);
		string offerId = this.exchangeDir.GetOfferId(ExchangeDirector.OfferType.GENERAL);
		if (offerId != null && offerRancherId != null)
		{
			this.rancherText.text = this.exchangeBundle.Get("m.rancher." + offerRancherId);
			this.flavorText.text = this.exchangeBundle.Get(offerId);
			Sprite rancherImage = this.GetRancherImage(offerRancherId);
			if (rancherImage != null)
			{
				this.rancherImg.sprite = rancherImage;
				return;
			}
		}
		else
		{
			this.rancherText.text = "";
			this.flavorText.text = "";
		}
	}

	// Token: 0x06001D31 RID: 7473 RVA: 0x0006EF6B File Offset: 0x0006D16B
	private Sprite GetRancherImage(string rancherId)
	{
		return Resources.Load("Exchange/Ranchers/" + rancherId, typeof(Sprite)) as Sprite;
	}

	// Token: 0x04001C41 RID: 7233
	[Tooltip("The individual request UI elements we are managing.")]
	public ExchangeItemEntryUI[] requestItems;

	// Token: 0x04001C42 RID: 7234
	[Tooltip("the individual reward UI elements we are managing.")]
	public ExchangeItemEntryUI[] rewardItems;

	// Token: 0x04001C43 RID: 7235
	[Tooltip("The panel we will enable when we have no offer.")]
	public GameObject noRequestPanel;

	// Token: 0x04001C44 RID: 7236
	[Tooltip("The panel we will enable when we have an offer.")]
	public GameObject mainOfferPanel;

	// Token: 0x04001C45 RID: 7237
	[Tooltip("The text which shows the Rancher's name.")]
	public TMP_Text rancherText;

	// Token: 0x04001C46 RID: 7238
	[Tooltip("The image which shows the Rancher's face.")]
	public Image rancherImg;

	// Token: 0x04001C47 RID: 7239
	[Tooltip("The flavor text which goes with the offer.")]
	public TMP_Text flavorText;

	// Token: 0x04001C48 RID: 7240
	private ExchangeDirector exchangeDir;

	// Token: 0x04001C49 RID: 7241
	private MessageBundle exchangeBundle;
}
