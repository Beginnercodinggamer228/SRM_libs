using System;
using UnityEngine;

// Token: 0x02000302 RID: 770
public class ExchangeChatActivator : MonoBehaviour, TechActivator
{
	// Token: 0x0600107A RID: 4218 RVA: 0x00041DFF File Offset: 0x0003FFFF
	public void Awake()
	{
		this.exchangeDir = SRSingleton<SceneContext>.Instance.ExchangeDirector;
	}

	// Token: 0x0600107B RID: 4219 RVA: 0x00041E14 File Offset: 0x00040014
	public void Activate()
	{
		foreach (ExchangeDirector.OfferType offerType in this.offerTypes)
		{
			if (offerType == ExchangeDirector.OfferType.GENERAL && this.exchangeDir.TryToAcceptNewOffer())
			{
				return;
			}
			if (this.exchangeDir.MaybeStartNext(offerType) || this.exchangeDir.CreateRancherChatUI(offerType, false))
			{
				break;
			}
		}
	}

	// Token: 0x0600107C RID: 4220 RVA: 0x00041E68 File Offset: 0x00040068
	public GameObject GetCustomGuiPrefab()
	{
		bool flag = true;
		foreach (ExchangeDirector.OfferType offerType in this.offerTypes)
		{
			if (!this.exchangeDir.IsOffline(offerType))
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			return this.offlineGuiPrefab;
		}
		return null;
	}

	// Token: 0x04000F40 RID: 3904
	public ExchangeDirector.OfferType[] offerTypes;

	// Token: 0x04000F41 RID: 3905
	public GameObject offlineGuiPrefab;

	// Token: 0x04000F42 RID: 3906
	private ExchangeDirector exchangeDir;
}
