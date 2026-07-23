using System;
using UnityEngine;

// Token: 0x02000636 RID: 1590
public class UIAudioSource : SRBehaviour
{
	// Token: 0x0600215C RID: 8540 RVA: 0x0007F84C File Offset: 0x0007DA4C
	public void OnEnable()
	{
		if (this.onEnable != null && (!this.skipOnEnableIfPaused || Time.timeScale > 0f))
		{
			SECTR_AudioSystem.Play(this.onEnable, SECTR_AudioSystem.Listener, Vector3.zero, false, null, false);
		}
	}

	// Token: 0x0600215D RID: 8541 RVA: 0x0007F89C File Offset: 0x0007DA9C
	public void PlayClick()
	{
		SECTR_AudioSystem.Play(SRSingleton<GameContext>.Instance.UITemplates.clickCue, SECTR_AudioSystem.Listener, Vector3.zero, false, null, false);
	}

	// Token: 0x0600215E RID: 8542 RVA: 0x0007F8D4 File Offset: 0x0007DAD4
	public void PlayPurchase()
	{
		SECTR_AudioSystem.Play(SRSingleton<GameContext>.Instance.UITemplates.purchaseCue, SECTR_AudioSystem.Listener, Vector3.zero, false, null, false);
	}

	// Token: 0x0600215F RID: 8543 RVA: 0x0007F90C File Offset: 0x0007DB0C
	public void PlayPurchaseExpansion()
	{
		SECTR_AudioSystem.Play(SRSingleton<GameContext>.Instance.UITemplates.purchaseExpansionCue, SECTR_AudioSystem.Listener, Vector3.zero, false, null, false);
	}

	// Token: 0x06002160 RID: 8544 RVA: 0x0007F944 File Offset: 0x0007DB44
	public void PlayPurchasePlot()
	{
		SECTR_AudioSystem.Play(SRSingleton<GameContext>.Instance.UITemplates.purchasePlotCue, SECTR_AudioSystem.Listener, Vector3.zero, false, null, false);
	}

	// Token: 0x06002161 RID: 8545 RVA: 0x0007F97C File Offset: 0x0007DB7C
	public void PlayPurchaseUpgrade()
	{
		SECTR_AudioSystem.Play(SRSingleton<GameContext>.Instance.UITemplates.purchaseUpgradeCue, SECTR_AudioSystem.Listener, Vector3.zero, false, null, false);
	}

	// Token: 0x06002162 RID: 8546 RVA: 0x0007F9B4 File Offset: 0x0007DBB4
	public void PlayPurchasePersonalUpgrade()
	{
		SECTR_AudioSystem.Play(SRSingleton<GameContext>.Instance.UITemplates.purchasePersonalUpgradeCue, SECTR_AudioSystem.Listener, Vector3.zero, false, null, false);
	}

	// Token: 0x040020AF RID: 8367
	public SECTR_AudioCue onEnable;

	// Token: 0x040020B0 RID: 8368
	public bool skipOnEnableIfPaused;
}
