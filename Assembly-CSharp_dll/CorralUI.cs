using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200055C RID: 1372
public class CorralUI : LandPlotUI
{
	// Token: 0x06001C94 RID: 7316 RVA: 0x0006CE3C File Offset: 0x0006B03C
	protected override GameObject CreatePurchaseUI()
	{
		PurchaseUI.Purchasable[] array = new PurchaseUI.Purchasable[7];
		array[0] = new PurchaseUI.Purchasable("m.upgrade.name.corral.walls", this.walls.icon, this.walls.img, "m.upgrade.desc.corral.walls", this.walls.cost, new PediaDirector.Id?(PediaDirector.Id.CORRAL), new UnityAction(this.UpgradeWalls), () => true, () => !this.activator.HasUpgrade(LandPlot.Upgrade.WALLS), null, null, null, null, false);
		array[1] = new PurchaseUI.Purchasable("m.upgrade.name.corral.music_box", this.musicBox.icon, this.musicBox.img, "m.upgrade.desc.corral.music_box", this.musicBox.cost, new PediaDirector.Id?(PediaDirector.Id.CORRAL), new UnityAction(this.UpgradeMusicBox), () => true, () => !this.activator.HasUpgrade(LandPlot.Upgrade.MUSIC_BOX), null, null, null, null, false);
		array[2] = new PurchaseUI.Purchasable("m.upgrade.name.corral.air_net", this.airNet.icon, this.airNet.img, "m.upgrade.desc.corral.air_net", this.airNet.cost, new PediaDirector.Id?(PediaDirector.Id.CORRAL), new UnityAction(this.UpgradeAirNet), () => true, () => !this.activator.HasUpgrade(LandPlot.Upgrade.AIR_NET), null, null, null, null, false);
		array[3] = new PurchaseUI.Purchasable("m.upgrade.name.corral.solar_shield", this.solarShield.icon, this.solarShield.img, "m.upgrade.desc.corral.solar_shield", this.solarShield.cost, new PediaDirector.Id?(PediaDirector.Id.CORRAL), new UnityAction(this.UpgradeSolarShield), () => true, () => !this.activator.HasUpgrade(LandPlot.Upgrade.SOLAR_SHIELD), null, null, null, null, false);
		array[4] = new PurchaseUI.Purchasable("m.upgrade.name.corral.plort_collector", this.plortCollector.icon, this.plortCollector.img, "m.upgrade.desc.corral.plort_collector", this.plortCollector.cost, new PediaDirector.Id?(PediaDirector.Id.CORRAL), new UnityAction(this.UpgradePlortCollector), () => true, () => !this.activator.HasUpgrade(LandPlot.Upgrade.PLORT_COLLECTOR), null, null, null, null, false);
		array[5] = new PurchaseUI.Purchasable("m.upgrade.name.corral.feeder", this.feeder.icon, this.feeder.img, "m.upgrade.desc.corral.feeder", this.feeder.cost, new PediaDirector.Id?(PediaDirector.Id.CORRAL), new UnityAction(this.UpgradeFeeder), () => true, () => !this.activator.HasUpgrade(LandPlot.Upgrade.FEEDER), null, null, null, null, false);
		array[6] = new PurchaseUI.Purchasable(MessageUtil.Qualify("ui", "l.demolish_plot"), this.demolish.icon, this.demolish.img, MessageUtil.Qualify("ui", "m.desc.demolish_plot"), this.demolish.cost, null, new UnityAction(this.Demolish), () => true, () => this.AllowDemolish(), "b.demolish", this.AllowDemolish() ? null : "w.cannot_demolish_corral_tutorial", null, null, false);
		PurchaseUI.Purchasable[] purchasables = array;
		return SRSingleton<GameContext>.Instance.UITemplates.CreatePurchaseUI(this.titleIcon, "t.corral", purchasables, false, new PurchaseUI.OnClose(this.Close), false);
	}

	// Token: 0x06001C95 RID: 7317 RVA: 0x0006D1EF File Offset: 0x0006B3EF
	private bool AllowDemolish()
	{
		TutorialDirector tutorialDirector = SRSingleton<SceneContext>.Instance.TutorialDirector;
		OptionsDirector optionsDirector = SRSingleton<GameContext>.Instance.OptionsDirector;
		return tutorialDirector.IsCompletedOrDisabled(TutorialDirector.Id.SHOOTING) || SRSingleton<SceneContext>.Instance.GameModeConfig.GetModeSettings().assumeExperiencedUser;
	}

	// Token: 0x06001C96 RID: 7318 RVA: 0x0006D224 File Offset: 0x0006B424
	public void UpgradeWalls()
	{
		base.Upgrade(LandPlot.Upgrade.WALLS, this.walls.cost);
	}

	// Token: 0x06001C97 RID: 7319 RVA: 0x0006D238 File Offset: 0x0006B438
	public void UpgradeMusicBox()
	{
		base.Upgrade(LandPlot.Upgrade.MUSIC_BOX, this.musicBox.cost);
	}

	// Token: 0x06001C98 RID: 7320 RVA: 0x0006D24C File Offset: 0x0006B44C
	public void UpgradeAirNet()
	{
		base.Upgrade(LandPlot.Upgrade.AIR_NET, this.airNet.cost);
	}

	// Token: 0x06001C99 RID: 7321 RVA: 0x0006D261 File Offset: 0x0006B461
	public void UpgradeSolarShield()
	{
		base.Upgrade(LandPlot.Upgrade.SOLAR_SHIELD, this.solarShield.cost);
	}

	// Token: 0x06001C9A RID: 7322 RVA: 0x0006D276 File Offset: 0x0006B476
	public void UpgradePlortCollector()
	{
		base.Upgrade(LandPlot.Upgrade.PLORT_COLLECTOR, this.plortCollector.cost);
	}

	// Token: 0x06001C9B RID: 7323 RVA: 0x0006D28B File Offset: 0x0006B48B
	public void UpgradeFeeder()
	{
		base.Upgrade(LandPlot.Upgrade.FEEDER, this.feeder.cost);
	}

	// Token: 0x06001C9C RID: 7324 RVA: 0x0006D2A0 File Offset: 0x0006B4A0
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

	// Token: 0x04001BA8 RID: 7080
	public LandPlotUI.UpgradePurchaseItem walls;

	// Token: 0x04001BA9 RID: 7081
	public LandPlotUI.UpgradePurchaseItem musicBox;

	// Token: 0x04001BAA RID: 7082
	public LandPlotUI.UpgradePurchaseItem airNet;

	// Token: 0x04001BAB RID: 7083
	public LandPlotUI.UpgradePurchaseItem solarShield;

	// Token: 0x04001BAC RID: 7084
	public LandPlotUI.UpgradePurchaseItem plortCollector;

	// Token: 0x04001BAD RID: 7085
	public LandPlotUI.UpgradePurchaseItem feeder;

	// Token: 0x04001BAE RID: 7086
	public LandPlotUI.PlotPurchaseItem demolish;

	// Token: 0x04001BAF RID: 7087
	public Sprite titleIcon;
}
