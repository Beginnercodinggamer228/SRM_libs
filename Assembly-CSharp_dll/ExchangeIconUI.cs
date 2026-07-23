using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200057C RID: 1404
public class ExchangeIconUI : MonoBehaviour
{
	// Token: 0x06001D33 RID: 7475 RVA: 0x0006EF8C File Offset: 0x0006D18C
	public void Awake()
	{
		this.exchangeDir = SRSingleton<SceneContext>.Instance.ExchangeDirector;
		ExchangeDirector exchangeDirector = this.exchangeDir;
		exchangeDirector.onOfferChanged = (ExchangeDirector.OnOfferChanged)Delegate.Combine(exchangeDirector.onOfferChanged, new ExchangeDirector.OnOfferChanged(this.OnOfferChanged));
	}

	// Token: 0x06001D34 RID: 7476 RVA: 0x0006EFC5 File Offset: 0x0006D1C5
	public void Start()
	{
		this.OnOfferChanged();
	}

	// Token: 0x06001D35 RID: 7477 RVA: 0x0006EFCD File Offset: 0x0006D1CD
	public void OnDestroy()
	{
		ExchangeDirector exchangeDirector = this.exchangeDir;
		exchangeDirector.onOfferChanged = (ExchangeDirector.OnOfferChanged)Delegate.Remove(exchangeDirector.onOfferChanged, new ExchangeDirector.OnOfferChanged(this.OnOfferChanged));
	}

	// Token: 0x06001D36 RID: 7478 RVA: 0x0006EFF8 File Offset: 0x0006D1F8
	public void OnOfferChanged()
	{
		string offerRancherId = this.exchangeDir.GetOfferRancherId(this.offerType);
		if (offerRancherId != null)
		{
			this.img.sprite = this.GetRancherImage(offerRancherId);
			return;
		}
		if (this.exchangeDir.HasPendingOffers(this.offerType) || this.offerType != ExchangeDirector.OfferType.GENERAL)
		{
			this.img.sprite = this.pendingIcon;
			return;
		}
		this.img.sprite = this.defaultIcon;
	}

	// Token: 0x06001D37 RID: 7479 RVA: 0x0006F06B File Offset: 0x0006D26B
	private Sprite GetRancherImage(string rancherId)
	{
		return this.exchangeDir.GetRancherIcon(rancherId);
	}

	// Token: 0x04001C4A RID: 7242
	public Image img;

	// Token: 0x04001C4B RID: 7243
	public Sprite defaultIcon;

	// Token: 0x04001C4C RID: 7244
	public Sprite pendingIcon;

	// Token: 0x04001C4D RID: 7245
	public ExchangeDirector.OfferType offerType;

	// Token: 0x04001C4E RID: 7246
	private ExchangeDirector exchangeDir;
}
