using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200059C RID: 1436
public class IncineratorUI : LandPlotUI
{
	// Token: 0x06001DD6 RID: 7638 RVA: 0x00071AC8 File Offset: 0x0006FCC8
	protected override GameObject CreatePurchaseUI()
	{
		PurchaseUI.Purchasable[] array = new PurchaseUI.Purchasable[2];
		array[0] = new PurchaseUI.Purchasable("m.upgrade.name.incinerator.ash_trough", this.ashTrough.icon, this.ashTrough.img, "m.upgrade.desc.incinerator.ash_trough", this.ashTrough.cost, new PediaDirector.Id?(PediaDirector.Id.CORRAL), new UnityAction(this.UpgradeAshTrough), () => true, () => !this.activator.HasUpgrade(LandPlot.Upgrade.ASH_TROUGH), null, null, null, null, false);
		array[1] = new PurchaseUI.Purchasable(MessageUtil.Qualify("ui", "l.demolish_plot"), this.demolish.icon, this.demolish.img, MessageUtil.Qualify("ui", "m.desc.demolish_plot"), this.demolish.cost, null, new UnityAction(this.Demolish), () => true, () => true, "b.demolish", null, null, null, false);
		PurchaseUI.Purchasable[] purchasables = array;
		return SRSingleton<GameContext>.Instance.UITemplates.CreatePurchaseUI(this.titleIcon, "t.incinerator", purchasables, false, new PurchaseUI.OnClose(this.Close), false);
	}

	// Token: 0x06001DD7 RID: 7639 RVA: 0x00071C22 File Offset: 0x0006FE22
	public void UpgradeAshTrough()
	{
		base.Upgrade(LandPlot.Upgrade.ASH_TROUGH, this.ashTrough.cost);
	}

	// Token: 0x06001DD8 RID: 7640 RVA: 0x00071C38 File Offset: 0x0006FE38
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

	// Token: 0x04001D00 RID: 7424
	public LandPlotUI.UpgradePurchaseItem ashTrough;

	// Token: 0x04001D01 RID: 7425
	public LandPlotUI.PlotPurchaseItem demolish;

	// Token: 0x04001D02 RID: 7426
	public Sprite titleIcon;
}
