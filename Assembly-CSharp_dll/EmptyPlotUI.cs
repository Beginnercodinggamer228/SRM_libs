using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000570 RID: 1392
public class EmptyPlotUI : LandPlotUI
{
	// Token: 0x06001CF9 RID: 7417 RVA: 0x0006E178 File Offset: 0x0006C378
	protected override GameObject CreatePurchaseUI()
	{
		PurchaseUI.Purchasable[] array = new PurchaseUI.Purchasable[6];
		array[0] = new PurchaseUI.Purchasable("t.corral", this.corral.icon, this.corral.img, "m.intro.corral", this.corral.cost, new PediaDirector.Id?(PediaDirector.Id.CORRAL), new UnityAction(this.BuyCorral), () => true, () => true, null, null, null, null, false);
		array[1] = new PurchaseUI.Purchasable("t.garden", this.garden.icon, this.garden.img, "m.intro.garden", this.garden.cost, new PediaDirector.Id?(PediaDirector.Id.GARDEN), new UnityAction(this.BuyGarden), () => true, () => true, null, null, null, null, false);
		array[2] = new PurchaseUI.Purchasable("t.coop", this.coop.icon, this.coop.img, "m.intro.coop", this.coop.cost, new PediaDirector.Id?(PediaDirector.Id.COOP), new UnityAction(this.BuyCoop), () => true, () => true, null, null, null, null, false);
		array[3] = new PurchaseUI.Purchasable("t.silo", this.silo.icon, this.silo.img, "m.intro.silo", this.silo.cost, new PediaDirector.Id?(PediaDirector.Id.SILO), new UnityAction(this.BuySilo), () => true, () => true, null, null, null, null, false);
		array[4] = new PurchaseUI.Purchasable("t.incinerator", this.incinerator.icon, this.incinerator.img, "m.intro.incinerator", this.incinerator.cost, new PediaDirector.Id?(PediaDirector.Id.INCINERATOR), new UnityAction(this.BuyIncinerator), () => true, () => true, null, null, null, null, false);
		array[5] = new PurchaseUI.Purchasable("t.pond", this.pond.icon, this.pond.img, "m.intro.pond", this.pond.cost, new PediaDirector.Id?(PediaDirector.Id.POND), new UnityAction(this.BuyPond), () => true, () => true, null, null, null, null, false);
		PurchaseUI.Purchasable[] purchasables = array;
		return SRSingleton<GameContext>.Instance.UITemplates.CreatePurchaseUI(this.titleIcon, MessageUtil.Qualify("ui", "t.empty_plot"), purchasables, false, new PurchaseUI.OnClose(this.Close), false);
	}

	// Token: 0x06001CFA RID: 7418 RVA: 0x0006E508 File Offset: 0x0006C708
	public void BuyCorral()
	{
		base.BuyPlot(this.corral);
	}

	// Token: 0x06001CFB RID: 7419 RVA: 0x0006E517 File Offset: 0x0006C717
	public void BuyGarden()
	{
		if (base.BuyPlot(this.garden))
		{
			SRSingleton<SceneContext>.Instance.TutorialDirector.MaybeShowPopup(TutorialDirector.Id.GARDEN);
		}
	}

	// Token: 0x06001CFC RID: 7420 RVA: 0x0006E537 File Offset: 0x0006C737
	public void BuyCoop()
	{
		base.BuyPlot(this.coop);
	}

	// Token: 0x06001CFD RID: 7421 RVA: 0x0006E546 File Offset: 0x0006C746
	public void BuySilo()
	{
		base.BuyPlot(this.silo);
	}

	// Token: 0x06001CFE RID: 7422 RVA: 0x0006E555 File Offset: 0x0006C755
	public void BuyIncinerator()
	{
		base.BuyPlot(this.incinerator);
	}

	// Token: 0x06001CFF RID: 7423 RVA: 0x0006E564 File Offset: 0x0006C764
	public void BuyPond()
	{
		base.BuyPlot(this.pond);
	}

	// Token: 0x04001C0C RID: 7180
	[Tooltip("The icon we show next to the overall title for the UI")]
	public Sprite titleIcon;

	// Token: 0x04001C0D RID: 7181
	[Tooltip("Specifies our info for the corral item")]
	public LandPlotUI.PlotPurchaseItem corral;

	// Token: 0x04001C0E RID: 7182
	[Tooltip("Specifies our info for the garden item")]
	public LandPlotUI.PlotPurchaseItem garden;

	// Token: 0x04001C0F RID: 7183
	[Tooltip("Specifies our info for the coop item")]
	public LandPlotUI.PlotPurchaseItem coop;

	// Token: 0x04001C10 RID: 7184
	[Tooltip("Specifies our info for the silo item")]
	public LandPlotUI.PlotPurchaseItem silo;

	// Token: 0x04001C11 RID: 7185
	[Tooltip("Specifies our info for the incinerator item")]
	public LandPlotUI.PlotPurchaseItem incinerator;

	// Token: 0x04001C12 RID: 7186
	[Tooltip("Specifies our info for the pond item")]
	public LandPlotUI.PlotPurchaseItem pond;
}
