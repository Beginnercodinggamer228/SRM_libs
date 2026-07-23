using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000593 RID: 1427
public class GardenUI : LandPlotUI
{
	// Token: 0x06001DA1 RID: 7585 RVA: 0x00070E84 File Offset: 0x0006F084
	protected override GameObject CreatePurchaseUI()
	{
		PurchaseUI.Purchasable[] array = new PurchaseUI.Purchasable[7];
		array[0] = new PurchaseUI.Purchasable("m.upgrade.name.garden.soil", this.soil.icon, this.soil.img, "m.upgrade.desc.garden.soil", this.soil.cost, new PediaDirector.Id?(PediaDirector.Id.GARDEN), new UnityAction(this.UpgradeSoil), () => true, () => !this.activator.HasUpgrade(LandPlot.Upgrade.SOIL), null, null, null, null, false);
		array[1] = new PurchaseUI.Purchasable("m.upgrade.name.garden.sprinkler", this.sprinkler.icon, this.sprinkler.img, "m.upgrade.desc.garden.sprinkler", this.sprinkler.cost, new PediaDirector.Id?(PediaDirector.Id.GARDEN), new UnityAction(this.UpgradeSprinkler), () => true, () => !this.activator.HasUpgrade(LandPlot.Upgrade.SPRINKLER), null, null, null, null, false);
		array[2] = new PurchaseUI.Purchasable("m.upgrade.name.garden.scareslime", this.scareslime.icon, this.scareslime.img, "m.upgrade.desc.garden.scareslime", this.scareslime.cost, new PediaDirector.Id?(PediaDirector.Id.GARDEN), new UnityAction(this.UpgradeScareslime), () => true, () => !this.activator.HasUpgrade(LandPlot.Upgrade.SCARESLIME), null, null, null, null, false);
		array[3] = new PurchaseUI.Purchasable("m.upgrade.name.garden.miracle_mix", this.miracleMix.icon, this.miracleMix.img, "m.upgrade.desc.garden.miracle_mix", this.miracleMix.cost, new PediaDirector.Id?(PediaDirector.Id.GARDEN), new UnityAction(this.UpgradeMiracleMix), () => SRSingleton<SceneContext>.Instance.ProgressDirector.GetProgress(ProgressDirector.ProgressType.OGDEN_REWARDS) >= 1, () => !this.activator.HasUpgrade(LandPlot.Upgrade.MIRACLE_MIX), null, null, null, null, false);
		array[4] = new PurchaseUI.Purchasable("m.upgrade.name.garden.deluxe", this.deluxe.icon, this.deluxe.img, "m.upgrade.desc.garden.deluxe", this.deluxe.cost, new PediaDirector.Id?(PediaDirector.Id.GARDEN), new UnityAction(this.UpgradeDeluxe), () => SRSingleton<SceneContext>.Instance.ProgressDirector.GetProgress(ProgressDirector.ProgressType.OGDEN_REWARDS) >= 2, () => !this.activator.HasUpgrade(LandPlot.Upgrade.DELUXE_GARDEN), null, null, null, null, false);
		array[5] = new PurchaseUI.Purchasable(MessageUtil.Qualify("ui", "b.clear_crop"), this.clearCrop.icon, this.clearCrop.img, MessageUtil.Qualify("ui", "m.desc.clear_crop"), this.clearCrop.cost, null, new UnityAction(this.ClearCrop), () => this.activator.HasAttached(), () => true, null, null, null, null, false);
		array[6] = new PurchaseUI.Purchasable(MessageUtil.Qualify("ui", "l.demolish_plot"), this.demolish.icon, this.demolish.img, MessageUtil.Qualify("ui", "m.desc.demolish_plot"), this.demolish.cost, null, new UnityAction(this.Demolish), () => true, () => true, "b.demolish", null, null, null, false);
		PurchaseUI.Purchasable[] purchasables = array;
		return SRSingleton<GameContext>.Instance.UITemplates.CreatePurchaseUI(this.titleIcon, "t.garden", purchasables, false, new PurchaseUI.OnClose(this.Close), false);
	}

	// Token: 0x06001DA2 RID: 7586 RVA: 0x0007124E File Offset: 0x0006F44E
	public void UpgradeSoil()
	{
		base.Upgrade(LandPlot.Upgrade.SOIL, this.soil.cost);
	}

	// Token: 0x06001DA3 RID: 7587 RVA: 0x00071262 File Offset: 0x0006F462
	public void UpgradeSprinkler()
	{
		base.Upgrade(LandPlot.Upgrade.SPRINKLER, this.sprinkler.cost);
	}

	// Token: 0x06001DA4 RID: 7588 RVA: 0x00071276 File Offset: 0x0006F476
	public void UpgradeScareslime()
	{
		base.Upgrade(LandPlot.Upgrade.SCARESLIME, this.scareslime.cost);
	}

	// Token: 0x06001DA5 RID: 7589 RVA: 0x0007128A File Offset: 0x0006F48A
	public void UpgradeMiracleMix()
	{
		base.Upgrade(LandPlot.Upgrade.MIRACLE_MIX, this.miracleMix.cost);
	}

	// Token: 0x06001DA6 RID: 7590 RVA: 0x0007129F File Offset: 0x0006F49F
	public void UpgradeDeluxe()
	{
		base.Upgrade(LandPlot.Upgrade.DELUXE_GARDEN, this.deluxe.cost);
	}

	// Token: 0x06001DA7 RID: 7591 RVA: 0x000712B4 File Offset: 0x0006F4B4
	public void ClearCrop()
	{
		if (this.playerState.GetCurrency() >= this.clearCrop.cost)
		{
			this.playerState.SpendCurrency(this.clearCrop.cost, false);
			this.activator.DestroyAttached();
			base.PlayPurchaseCue();
			this.Close();
			return;
		}
		base.Error("e.insuf_coins", false);
	}

	// Token: 0x06001DA8 RID: 7592 RVA: 0x00071314 File Offset: 0x0006F514
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

	// Token: 0x04001CBD RID: 7357
	public LandPlotUI.UpgradePurchaseItem soil;

	// Token: 0x04001CBE RID: 7358
	public LandPlotUI.UpgradePurchaseItem sprinkler;

	// Token: 0x04001CBF RID: 7359
	public LandPlotUI.UpgradePurchaseItem scareslime;

	// Token: 0x04001CC0 RID: 7360
	public LandPlotUI.UpgradePurchaseItem miracleMix;

	// Token: 0x04001CC1 RID: 7361
	public LandPlotUI.UpgradePurchaseItem deluxe;

	// Token: 0x04001CC2 RID: 7362
	public LandPlotUI.PurchaseItem clearCrop;

	// Token: 0x04001CC3 RID: 7363
	public LandPlotUI.PlotPurchaseItem demolish;

	// Token: 0x04001CC4 RID: 7364
	public Sprite titleIcon;

	// Token: 0x04001CC5 RID: 7365
	public GameObject plantButtonPanelObject;
}
