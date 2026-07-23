using System;
using UnityEngine;

// Token: 0x020005A3 RID: 1443
public abstract class LandPlotUI : BaseUI
{
	// Token: 0x06001DFA RID: 7674 RVA: 0x00072032 File Offset: 0x00070232
	public override void Awake()
	{
		base.Awake();
		this.playerState = SRSingleton<SceneContext>.Instance.PlayerState;
	}

	// Token: 0x06001DFB RID: 7675
	protected abstract GameObject CreatePurchaseUI();

	// Token: 0x06001DFC RID: 7676 RVA: 0x0007204A File Offset: 0x0007024A
	public virtual void SetActivator(LandPlot activator)
	{
		this.activator = activator;
		this.RebuildUI();
	}

	// Token: 0x06001DFD RID: 7677 RVA: 0x0007205C File Offset: 0x0007025C
	public void RebuildUI()
	{
		for (int i = 0; i < base.transform.childCount; i++)
		{
			Destroyer.Destroy(base.transform.GetChild(i).gameObject, "LandPlotUI.RebuildUI");
		}
		GameObject gameObject = this.CreatePurchaseUI();
		this.purchaseUI = gameObject.GetComponent<PurchaseUI>();
		gameObject.transform.SetParent(base.transform, false);
		this.statusArea = gameObject.GetComponent<PurchaseUI>().statusArea;
	}

	// Token: 0x06001DFE RID: 7678 RVA: 0x000720D0 File Offset: 0x000702D0
	protected GameObject Replace(GameObject replacementPrefab)
	{
		GameObject result = this.activator.transform.parent.GetComponent<LandPlotLocation>().Replace(this.activator, replacementPrefab);
		this.Close();
		return result;
	}

	// Token: 0x06001DFF RID: 7679 RVA: 0x000720FC File Offset: 0x000702FC
	protected void Upgrade(LandPlot.Upgrade upgrade, int cost)
	{
		if (this.activator.HasUpgrade(upgrade))
		{
			base.Error("e.already_has_upgrade", false);
			return;
		}
		if (this.playerState.GetCurrency() >= cost)
		{
			this.playerState.SpendCurrency(cost, false);
			this.activator.AddUpgrade(upgrade);
			this.PlayPurchaseUpgradeCue();
			this.RebuildUI();
			this.purchaseUI.PlayPurchaseFX();
			return;
		}
		base.PlayErrorCue();
		base.Error("e.insuf_coins", false);
	}

	// Token: 0x06001E00 RID: 7680 RVA: 0x00072178 File Offset: 0x00070378
	protected bool BuyPlot(LandPlotUI.PlotPurchaseItem plot)
	{
		if (this.playerState.GetCurrency() >= plot.cost)
		{
			this.playerState.SpendCurrency(plot.cost, false);
			this.PlayPurchaseCue();
			this.Replace(plot.plotPrefab);
			return true;
		}
		base.PlayErrorCue();
		base.Error("e.insuf_coins", false);
		return false;
	}

	// Token: 0x06001E01 RID: 7681 RVA: 0x000721D2 File Offset: 0x000703D2
	protected void PlayPurchaseUpgradeCue()
	{
		base.Play(SRSingleton<GameContext>.Instance.UITemplates.purchaseUpgradeCue);
	}

	// Token: 0x06001E02 RID: 7682 RVA: 0x000721E9 File Offset: 0x000703E9
	protected void PlayPurchaseCue()
	{
		base.Play(SRSingleton<GameContext>.Instance.UITemplates.purchasePlotCue);
	}

	// Token: 0x04001D1B RID: 7451
	protected LandPlot activator;

	// Token: 0x04001D1C RID: 7452
	protected PlayerState playerState;

	// Token: 0x04001D1D RID: 7453
	private PurchaseUI purchaseUI;

	// Token: 0x04001D1E RID: 7454
	private const string ERR_ALREADY_HAS_UPGRADE = "e.already_has_upgrade";

	// Token: 0x020005A4 RID: 1444
	[Serializable]
	public class PurchaseItem
	{
		// Token: 0x04001D1F RID: 7455
		public Sprite icon;

		// Token: 0x04001D20 RID: 7456
		public Sprite img;

		// Token: 0x04001D21 RID: 7457
		public int cost;
	}

	// Token: 0x020005A5 RID: 1445
	[Serializable]
	public class PlotPurchaseItem : LandPlotUI.PurchaseItem
	{
		// Token: 0x04001D22 RID: 7458
		public GameObject plotPrefab;
	}

	// Token: 0x020005A6 RID: 1446
	[Serializable]
	public class UpgradePurchaseItem : LandPlotUI.PurchaseItem
	{
		// Token: 0x04001D23 RID: 7459
		public LandPlot.Upgrade upgrade;
	}
}
