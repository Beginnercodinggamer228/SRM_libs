using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000558 RID: 1368
public class CoopUI : LandPlotUI
{
	// Token: 0x06001C73 RID: 7283 RVA: 0x0006C5F4 File Offset: 0x0006A7F4
	protected override GameObject CreatePurchaseUI()
	{
		PurchaseUI.Purchasable[] array = new PurchaseUI.Purchasable[5];
		array[0] = new PurchaseUI.Purchasable("m.upgrade.name.coop.walls", this.walls.icon, this.walls.img, "m.upgrade.desc.coop.walls", this.walls.cost, new PediaDirector.Id?(PediaDirector.Id.COOP), new UnityAction(this.UpgradeWalls), () => true, () => !this.activator.HasUpgrade(LandPlot.Upgrade.WALLS), null, null, null, null, false);
		array[1] = new PurchaseUI.Purchasable("m.upgrade.name.coop.feeder", this.feeder.icon, this.feeder.img, "m.upgrade.desc.coop.feeder", this.feeder.cost, new PediaDirector.Id?(PediaDirector.Id.COOP), new UnityAction(this.UpgradeFeeder), () => true, () => !this.activator.HasUpgrade(LandPlot.Upgrade.FEEDER), null, null, null, null, false);
		array[2] = new PurchaseUI.Purchasable("m.upgrade.name.coop.vitamizer", this.vitamizer.icon, this.vitamizer.img, "m.upgrade.desc.coop.vitamizer", this.vitamizer.cost, new PediaDirector.Id?(PediaDirector.Id.COOP), new UnityAction(this.UpgradeVitamizer), () => true, () => !this.activator.HasUpgrade(LandPlot.Upgrade.VITAMIZER), null, null, null, null, false);
		array[3] = new PurchaseUI.Purchasable("m.upgrade.name.coop.deluxe", this.deluxe.icon, this.deluxe.img, "m.upgrade.desc.coop.deluxe", this.deluxe.cost, new PediaDirector.Id?(PediaDirector.Id.COOP), new UnityAction(this.UpgradeDeluxe), () => SRSingleton<SceneContext>.Instance.ProgressDirector.GetProgress(ProgressDirector.ProgressType.MOCHI_REWARDS) >= 2, () => !this.activator.HasUpgrade(LandPlot.Upgrade.DELUXE_COOP), null, null, null, null, false);
		array[4] = new PurchaseUI.Purchasable(MessageUtil.Qualify("ui", "l.demolish_plot"), this.demolish.icon, this.demolish.img, MessageUtil.Qualify("ui", "m.desc.demolish_plot"), this.demolish.cost, null, new UnityAction(this.Demolish), () => true, () => true, "b.demolish", null, null, null, false);
		PurchaseUI.Purchasable[] purchasables = array;
		return SRSingleton<GameContext>.Instance.UITemplates.CreatePurchaseUI(this.titleIcon, "t.coop", purchasables, false, new PurchaseUI.OnClose(this.Close), false);
	}

	// Token: 0x06001C74 RID: 7284 RVA: 0x0006C8B9 File Offset: 0x0006AAB9
	public void UpgradeWalls()
	{
		base.Upgrade(LandPlot.Upgrade.WALLS, this.walls.cost);
	}

	// Token: 0x06001C75 RID: 7285 RVA: 0x0006C8CD File Offset: 0x0006AACD
	public void UpgradeFeeder()
	{
		base.Upgrade(LandPlot.Upgrade.FEEDER, this.feeder.cost);
	}

	// Token: 0x06001C76 RID: 7286 RVA: 0x0006C8E2 File Offset: 0x0006AAE2
	public void UpgradeVitamizer()
	{
		base.Upgrade(LandPlot.Upgrade.VITAMIZER, this.vitamizer.cost);
	}

	// Token: 0x06001C77 RID: 7287 RVA: 0x0006C8F7 File Offset: 0x0006AAF7
	public void UpgradeDeluxe()
	{
		base.Upgrade(LandPlot.Upgrade.DELUXE_COOP, this.deluxe.cost);
	}

	// Token: 0x06001C78 RID: 7288 RVA: 0x0006C90C File Offset: 0x0006AB0C
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

	// Token: 0x04001B7A RID: 7034
	public LandPlotUI.UpgradePurchaseItem walls;

	// Token: 0x04001B7B RID: 7035
	public LandPlotUI.UpgradePurchaseItem feeder;

	// Token: 0x04001B7C RID: 7036
	public LandPlotUI.UpgradePurchaseItem vitamizer;

	// Token: 0x04001B7D RID: 7037
	public LandPlotUI.UpgradePurchaseItem deluxe;

	// Token: 0x04001B7E RID: 7038
	public LandPlotUI.PlotPurchaseItem demolish;

	// Token: 0x04001B7F RID: 7039
	public Sprite titleIcon;
}
