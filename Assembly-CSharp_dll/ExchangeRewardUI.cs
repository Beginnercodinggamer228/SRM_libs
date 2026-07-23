using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000581 RID: 1409
public class ExchangeRewardUI : MonoBehaviour
{
	// Token: 0x06001D4B RID: 7499 RVA: 0x0006F4AA File Offset: 0x0006D6AA
	public void Awake()
	{
		this.exchangeDir = SRSingleton<SceneContext>.Instance.ExchangeDirector;
		ExchangeDirector exchangeDirector = this.exchangeDir;
		exchangeDirector.onOfferChanged = (ExchangeDirector.OnOfferChanged)Delegate.Combine(exchangeDirector.onOfferChanged, new ExchangeDirector.OnOfferChanged(this.OnOfferChanged));
	}

	// Token: 0x06001D4C RID: 7500 RVA: 0x0006F4E3 File Offset: 0x0006D6E3
	public void OnEnable()
	{
		this.slimeAppearanceDirector.onSlimeAppearanceChanged += this.OnSlimeAppearanceUpdated;
		this.OnOfferChanged();
	}

	// Token: 0x06001D4D RID: 7501 RVA: 0x0006F502 File Offset: 0x0006D702
	public void OnDisable()
	{
		this.slimeAppearanceDirector.onSlimeAppearanceChanged -= this.OnSlimeAppearanceUpdated;
	}

	// Token: 0x06001D4E RID: 7502 RVA: 0x0006F51B File Offset: 0x0006D71B
	public void OnSlimeAppearanceUpdated(SlimeDefinition slime, SlimeAppearance appearance)
	{
		this.OnOfferChanged();
	}

	// Token: 0x06001D4F RID: 7503 RVA: 0x0006F51B File Offset: 0x0006D71B
	public void Start()
	{
		this.OnOfferChanged();
	}

	// Token: 0x06001D50 RID: 7504 RVA: 0x0006F523 File Offset: 0x0006D723
	public void OnDestroy()
	{
		ExchangeDirector exchangeDirector = this.exchangeDir;
		exchangeDirector.onOfferChanged = (ExchangeDirector.OnOfferChanged)Delegate.Remove(exchangeDirector.onOfferChanged, new ExchangeDirector.OnOfferChanged(this.OnOfferChanged));
	}

	// Token: 0x06001D51 RID: 7505 RVA: 0x0006F54C File Offset: 0x0006D74C
	public void OnOfferChanged()
	{
		List<ExchangeDirector.ItemEntry> offerRewards = this.exchangeDir.GetOfferRewards(this.offerType);
		if (offerRewards == null)
		{
			if (this.exchangeDir.HasPendingOffers(this.offerType))
			{
				this.noRequestText.enabled = false;
				this.pendingRequestText.enabled = true;
			}
			else
			{
				this.noRequestText.enabled = true;
				this.pendingRequestText.enabled = false;
			}
			for (int i = 0; i < this.items.Length; i++)
			{
				this.items[i].SetEntry(null);
			}
			return;
		}
		this.noRequestText.enabled = false;
		this.pendingRequestText.enabled = false;
		for (int j = 0; j < this.items.Length; j++)
		{
			this.items[j].SetEntry((offerRewards.Count > j) ? offerRewards[j] : null);
		}
	}

	// Token: 0x04001C63 RID: 7267
	public Text noRequestText;

	// Token: 0x04001C64 RID: 7268
	public Text pendingRequestText;

	// Token: 0x04001C65 RID: 7269
	public ExchangeDirector.OfferType offerType;

	// Token: 0x04001C66 RID: 7270
	public ExchangeRewardItemEntryUI[] items;

	// Token: 0x04001C67 RID: 7271
	private ExchangeDirector exchangeDir;

	// Token: 0x04001C68 RID: 7272
	public SlimeAppearanceDirector slimeAppearanceDirector;
}
