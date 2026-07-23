using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200057F RID: 1407
public class ExchangeRequestUI : MonoBehaviour
{
	// Token: 0x06001D3F RID: 7487 RVA: 0x0006F218 File Offset: 0x0006D418
	public void Awake()
	{
		this.exchangeDir = SRSingleton<SceneContext>.Instance.ExchangeDirector;
		ExchangeDirector exchangeDirector = this.exchangeDir;
		exchangeDirector.onOfferChanged = (ExchangeDirector.OnOfferChanged)Delegate.Combine(exchangeDirector.onOfferChanged, new ExchangeDirector.OnOfferChanged(this.OnOfferChanged));
	}

	// Token: 0x06001D40 RID: 7488 RVA: 0x0006F251 File Offset: 0x0006D451
	public void OnEnable()
	{
		this.slimeAppearanceDirector.onSlimeAppearanceChanged += this.OnSlimeAppearanceUpdated;
		this.OnOfferChanged();
	}

	// Token: 0x06001D41 RID: 7489 RVA: 0x0006F270 File Offset: 0x0006D470
	public void OnDisable()
	{
		this.slimeAppearanceDirector.onSlimeAppearanceChanged -= this.OnSlimeAppearanceUpdated;
	}

	// Token: 0x06001D42 RID: 7490 RVA: 0x0006F289 File Offset: 0x0006D489
	public void OnSlimeAppearanceUpdated(SlimeDefinition slime, SlimeAppearance appearance)
	{
		this.OnOfferChanged();
	}

	// Token: 0x06001D43 RID: 7491 RVA: 0x0006F289 File Offset: 0x0006D489
	public void Start()
	{
		this.OnOfferChanged();
	}

	// Token: 0x06001D44 RID: 7492 RVA: 0x0006F291 File Offset: 0x0006D491
	public void OnDestroy()
	{
		ExchangeDirector exchangeDirector = this.exchangeDir;
		exchangeDirector.onOfferChanged = (ExchangeDirector.OnOfferChanged)Delegate.Remove(exchangeDirector.onOfferChanged, new ExchangeDirector.OnOfferChanged(this.OnOfferChanged));
	}

	// Token: 0x06001D45 RID: 7493 RVA: 0x0006F2BC File Offset: 0x0006D4BC
	public void OnOfferChanged()
	{
		List<ExchangeDirector.RequestedItemEntry> offerRequests = this.exchangeDir.GetOfferRequests(this.offerType);
		if (offerRequests == null)
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
			this.items[j].SetEntry((offerRequests.Count > j) ? offerRequests[j] : null);
		}
	}

	// Token: 0x04001C59 RID: 7257
	public Text noRequestText;

	// Token: 0x04001C5A RID: 7258
	public Text pendingRequestText;

	// Token: 0x04001C5B RID: 7259
	public ExchangeDirector.OfferType offerType;

	// Token: 0x04001C5C RID: 7260
	[SerializeField]
	private ExchangeProgressItemEntryUI[] items;

	// Token: 0x04001C5D RID: 7261
	public SlimeAppearanceDirector slimeAppearanceDirector;

	// Token: 0x04001C5E RID: 7262
	private ExchangeDirector exchangeDir;
}
