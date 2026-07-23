using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000618 RID: 1560
public class SiloUI : LandPlotUI
{
	// Token: 0x060020B2 RID: 8370 RVA: 0x0007CEE4 File Offset: 0x0007B0E4
	protected override GameObject CreatePurchaseUI()
	{
		PurchaseUI.Purchasable[] array = new PurchaseUI.Purchasable[4];
		array[0] = new PurchaseUI.Purchasable("m.upgrade.name.silo.storage2", this.storage2.icon, this.storage2.img, "m.upgrade.desc.silo.storage2", this.storage2.cost, new PediaDirector.Id?(PediaDirector.Id.SILO), new UnityAction(this.UpgradeStorage2), () => !this.activator.HasUpgrade(LandPlot.Upgrade.STORAGE2), () => true, null, null, null, null, false);
		array[1] = new PurchaseUI.Purchasable("m.upgrade.name.silo.storage2", this.storage3.icon, this.storage3.img, "m.upgrade.desc.silo.storage2", this.storage3.cost, new PediaDirector.Id?(PediaDirector.Id.SILO), new UnityAction(this.UpgradeStorage3), () => this.activator.HasUpgrade(LandPlot.Upgrade.STORAGE2) && !this.activator.HasUpgrade(LandPlot.Upgrade.STORAGE3), () => true, null, null, null, null, false);
		array[2] = new PurchaseUI.Purchasable("m.upgrade.name.silo.storage2", this.storage4.icon, this.storage4.img, "m.upgrade.desc.silo.storage2", this.storage4.cost, new PediaDirector.Id?(PediaDirector.Id.SILO), new UnityAction(this.UpgradeStorage4), () => this.activator.HasUpgrade(LandPlot.Upgrade.STORAGE3), () => !this.activator.HasUpgrade(LandPlot.Upgrade.STORAGE4), null, null, null, null, false);
		array[3] = new PurchaseUI.Purchasable(MessageUtil.Qualify("ui", "l.demolish_plot"), this.demolish.icon, this.demolish.img, MessageUtil.Qualify("ui", "m.desc.demolish_plot"), this.demolish.cost, null, new UnityAction(this.Demolish), () => true, () => true, "b.demolish", this.activator.GetComponent<SiloStorage>().GetRelevantAmmo().IsEmpty() ? null : "w.destroying_silo_destroys_contents", null, null, true);
		PurchaseUI.Purchasable[] purchasables = array;
		return SRSingleton<GameContext>.Instance.UITemplates.CreatePurchaseUI(this.titleIcon, "t.silo", purchasables, false, new PurchaseUI.OnClose(this.Close), false);
	}

	// Token: 0x060020B3 RID: 8371 RVA: 0x0007D13B File Offset: 0x0007B33B
	public void UpgradeStorage2()
	{
		base.Upgrade(LandPlot.Upgrade.STORAGE2, this.storage2.cost);
	}

	// Token: 0x060020B4 RID: 8372 RVA: 0x0007D14F File Offset: 0x0007B34F
	public void UpgradeStorage3()
	{
		base.Upgrade(LandPlot.Upgrade.STORAGE3, this.storage3.cost);
	}

	// Token: 0x060020B5 RID: 8373 RVA: 0x0007D163 File Offset: 0x0007B363
	public void UpgradeStorage4()
	{
		base.Upgrade(LandPlot.Upgrade.STORAGE4, this.storage4.cost);
	}

	// Token: 0x060020B6 RID: 8374 RVA: 0x0007D178 File Offset: 0x0007B378
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

	// Token: 0x04002008 RID: 8200
	public LandPlotUI.UpgradePurchaseItem storage2;

	// Token: 0x04002009 RID: 8201
	public LandPlotUI.UpgradePurchaseItem storage3;

	// Token: 0x0400200A RID: 8202
	public LandPlotUI.UpgradePurchaseItem storage4;

	// Token: 0x0400200B RID: 8203
	public LandPlotUI.PlotPurchaseItem demolish;

	// Token: 0x0400200C RID: 8204
	public Sprite titleIcon;
}
