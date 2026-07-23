using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020005EA RID: 1514
public class PondUI : LandPlotUI
{
	// Token: 0x06001FCD RID: 8141 RVA: 0x000793E8 File Offset: 0x000775E8
	protected override GameObject CreatePurchaseUI()
	{
		PurchaseUI.Purchasable[] array = new PurchaseUI.Purchasable[1];
		array[0] = new PurchaseUI.Purchasable(MessageUtil.Qualify("ui", "l.demolish_plot"), this.demolish.icon, this.demolish.img, MessageUtil.Qualify("ui", "m.desc.demolish_plot"), this.demolish.cost, null, new UnityAction(this.Demolish), () => true, () => true, "b.demolish", null, null, null, false);
		PurchaseUI.Purchasable[] purchasables = array;
		return SRSingleton<GameContext>.Instance.UITemplates.CreatePurchaseUI(this.titleIcon, "t.pond", purchasables, false, new PurchaseUI.OnClose(this.Close), false);
	}

	// Token: 0x06001FCE RID: 8142 RVA: 0x000794CC File Offset: 0x000776CC
	public void Demolish()
	{
		if (this.playerState.GetCurrency() >= this.demolish.cost)
		{
			this.playerState.SpendCurrency(this.demolish.cost, false);
			base.Replace(this.demolish.plotPrefab);
			base.PlayPurchaseCue();
			return;
		}
		base.Error("e.insuf_coins", false);
	}

	// Token: 0x04001EF8 RID: 7928
	public LandPlotUI.PlotPurchaseItem demolish;

	// Token: 0x04001EF9 RID: 7929
	public Sprite titleIcon;
}
