using System;
using System.Collections.Generic;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x0200062E RID: 1582
public class ToyShopUI : BaseUI
{
	// Token: 0x06002131 RID: 8497 RVA: 0x0007EF14 File Offset: 0x0007D114
	public override void Awake()
	{
		base.Awake();
		this.playerState = SRSingleton<SceneContext>.Instance.PlayerState;
		this.lookupDir = SRSingleton<GameContext>.Instance.LookupDirector;
		this.achieveDir = SRSingleton<SceneContext>.Instance.AchievementsDirector;
		this.toyDirector = SRSingleton<GameContext>.Instance.ToyDirector;
		SRSingleton<SceneContext>.Instance.PediaDirector.UnlockWithoutPopup(PediaDirector.Id.SLIME_TOYS);
		this.RebuildUI();
	}

	// Token: 0x06002132 RID: 8498 RVA: 0x0007EF84 File Offset: 0x0007D184
	public void RebuildUI()
	{
		for (int i = 0; i < base.transform.childCount; i++)
		{
			Destroyer.Destroy(base.transform.GetChild(i).gameObject, "ToyShopUI.RebuildUI");
		}
		GameObject gameObject = this.CreatePurchaseUI();
		this.purchaseUI = gameObject.GetComponent<PurchaseUI>();
		gameObject.transform.SetParent(base.transform, false);
		this.statusArea = gameObject.GetComponent<PurchaseUI>().statusArea;
	}

	// Token: 0x06002133 RID: 8499 RVA: 0x0007EFF8 File Offset: 0x0007D1F8
	protected GameObject CreatePurchaseUI()
	{
		List<PurchaseUI.Purchasable> list = new List<PurchaseUI.Purchasable>();
		foreach (Identifiable.Id toyId in this.toyDirector.GetPurchaseableToys())
		{
			list.Add(this.CreatePurchasableToy(toyId));
		}
		return SRSingleton<GameContext>.Instance.UITemplates.CreatePurchaseUI(this.titleIcon, MessageUtil.Qualify("ui", "t.slime_toys"), list.ToArray(), false, new PurchaseUI.OnClose(this.Close), false);
	}

	// Token: 0x06002134 RID: 8500 RVA: 0x0007F090 File Offset: 0x0007D290
	private PurchaseUI.Purchasable CreatePurchasableToy(Identifiable.Id toyId)
	{
		ToyDefinition toy = this.lookupDir.GetToyDefinition(toyId);
		return new PurchaseUI.Purchasable(string.Format("m.toy.name.{0}", toy.NameKey), toy.Icon, toy.Icon, string.Format("m.toy.desc.{0}", toy.NameKey), toy.Cost, null, delegate()
		{
			this.BuyToy(toyId, toy.Cost);
		}, () => true, () => true, null, null, null, null, false);
	}

	// Token: 0x06002135 RID: 8501 RVA: 0x0007F170 File Offset: 0x0007D370
	protected void BuyToy(Identifiable.Id toyId, int cost)
	{
		if (this.playerState.GetCurrency() >= cost)
		{
			base.Play(SRSingleton<GameContext>.Instance.UITemplates.purchasePersonalUpgradeCue);
			this.playerState.SpendCurrency(cost, false);
			this.InstantiateToy(toyId);
			this.purchaseUI.PlayPurchaseFX();
			this.achieveDir.AddToStat(AchievementsDirector.EnumStat.SLIME_TOYS_BOUGHT, toyId);
			this.Close();
			return;
		}
		base.PlayErrorCue();
		base.Error("e.insuf_coins", false);
	}

	// Token: 0x06002136 RID: 8502 RVA: 0x0007F1EC File Offset: 0x0007D3EC
	private void InstantiateToy(Identifiable.Id toyId)
	{
		if (this.ejectionPoint != null)
		{
			Rigidbody component = SRBehaviour.InstantiateActor(this.lookupDir.GetPrefab(toyId), this.regionSetId, this.ejectionPoint.transform.position, this.ejectionPoint.transform.rotation, false).GetComponent<Rigidbody>();
			component.isKinematic = false;
			component.AddForce(base.transform.forward * 25f);
		}
	}

	// Token: 0x04002087 RID: 8327
	public Sprite titleIcon;

	// Token: 0x04002088 RID: 8328
	private PlayerState playerState;

	// Token: 0x04002089 RID: 8329
	private LookupDirector lookupDir;

	// Token: 0x0400208A RID: 8330
	private PurchaseUI purchaseUI;

	// Token: 0x0400208B RID: 8331
	private AchievementsDirector achieveDir;

	// Token: 0x0400208C RID: 8332
	private ToyDirector toyDirector;

	// Token: 0x0400208D RID: 8333
	private const float EJECT_FORCE = 25f;

	// Token: 0x0400208E RID: 8334
	[HideInInspector]
	public GameObject ejectionPoint;

	// Token: 0x0400208F RID: 8335
	[HideInInspector]
	public RegionRegistry.RegionSetId regionSetId;
}
