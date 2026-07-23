using System;
using UnityEngine;

// Token: 0x020005E2 RID: 1506
public class PersonalUpgradeUI : BaseUI
{
	// Token: 0x06001FA9 RID: 8105 RVA: 0x00078726 File Offset: 0x00076926
	public override void Awake()
	{
		base.Awake();
		this.playerState = SRSingleton<SceneContext>.Instance.PlayerState;
		this.lookupDir = SRSingleton<GameContext>.Instance.LookupDirector;
		this.RebuildUI();
	}

	// Token: 0x06001FAA RID: 8106 RVA: 0x00078754 File Offset: 0x00076954
	public void RebuildUI()
	{
		for (int i = 0; i < base.transform.childCount; i++)
		{
			Destroyer.Destroy(base.transform.GetChild(i).gameObject, "PersonalUpgradeUI.RebuildUI");
		}
		GameObject gameObject = this.CreatePurchaseUI();
		this.purchaseUI = gameObject.GetComponent<PurchaseUI>();
		gameObject.transform.SetParent(base.transform, false);
		this.statusArea = gameObject.GetComponent<PurchaseUI>().statusArea;
	}

	// Token: 0x06001FAB RID: 8107 RVA: 0x000787C8 File Offset: 0x000769C8
	protected GameObject CreatePurchaseUI()
	{
		PurchaseUI.Purchasable[] purchasables = new PurchaseUI.Purchasable[]
		{
			this.CreateUpgradePurchasable(PlayerState.Upgrade.LIQUID_SLOT),
			this.CreateUpgradePurchasable(PlayerState.Upgrade.JETPACK),
			this.CreateUpgradePurchasable(PlayerState.Upgrade.JETPACK_EFFICIENCY),
			this.CreateUpgradePurchasable(PlayerState.Upgrade.RUN_EFFICIENCY),
			this.CreateUpgradePurchasable(PlayerState.Upgrade.RUN_EFFICIENCY_2),
			this.CreateUpgradePurchasable(PlayerState.Upgrade.AIR_BURST),
			this.CreateUpgradePurchasable(PlayerState.Upgrade.HEALTH_1),
			this.CreateUpgradePurchasable(PlayerState.Upgrade.HEALTH_2),
			this.CreateUpgradePurchasable(PlayerState.Upgrade.HEALTH_3),
			this.CreateUpgradePurchasable(PlayerState.Upgrade.HEALTH_4),
			this.CreateUpgradePurchasable(PlayerState.Upgrade.ENERGY_1),
			this.CreateUpgradePurchasable(PlayerState.Upgrade.ENERGY_2),
			this.CreateUpgradePurchasable(PlayerState.Upgrade.ENERGY_3),
			this.CreateUpgradePurchasable(PlayerState.Upgrade.AMMO_1),
			this.CreateUpgradePurchasable(PlayerState.Upgrade.AMMO_2),
			this.CreateUpgradePurchasable(PlayerState.Upgrade.AMMO_3),
			this.CreateUpgradePurchasable(PlayerState.Upgrade.AMMO_4),
			this.CreateUpgradePurchasable(PlayerState.Upgrade.TREASURE_CRACKER_1),
			this.CreateUpgradePurchasable(PlayerState.Upgrade.TREASURE_CRACKER_2),
			this.CreateUpgradePurchasable(PlayerState.Upgrade.TREASURE_CRACKER_3),
			this.CreateUpgradePurchasable(PlayerState.Upgrade.GOLDEN_SURESHOT),
			this.CreateUpgradePurchasable(PlayerState.Upgrade.SPARE_KEY)
		};
		return SRSingleton<GameContext>.Instance.UITemplates.CreatePurchaseUI(this.titleIcon, MessageUtil.Qualify("ui", "t.personal_upgrades"), purchasables, false, new PurchaseUI.OnClose(this.Close), false);
	}

	// Token: 0x06001FAC RID: 8108 RVA: 0x00078908 File Offset: 0x00076B08
	private PurchaseUI.Purchasable CreateUpgradePurchasable(PlayerState.Upgrade upgrade)
	{
		UpgradeDefinition item = this.lookupDir.GetUpgradeDefinition(upgrade);
		string str = upgrade.ToString().ToLowerInvariant();
		return new PurchaseUI.Purchasable("m.upgrade.name.personal." + str, item.Icon, item.Icon, "m.upgrade.desc.personal." + str, item.Cost, null, delegate()
		{
			this.Upgrade(upgrade, item.Cost);
		}, () => this.playerState.HasOrCanGetUpgrade(upgrade), () => !this.playerState.HasUpgrade(upgrade), null, null, null, null, false);
	}

	// Token: 0x06001FAD RID: 8109 RVA: 0x000789C8 File Offset: 0x00076BC8
	protected void Upgrade(PlayerState.Upgrade upgrade, int cost)
	{
		if (this.playerState.HasUpgrade(upgrade))
		{
			base.PlayErrorCue();
			base.Error("e.already_has_personal_upgrade", false);
			return;
		}
		if (!this.playerState.CanGetUpgrade(upgrade))
		{
			base.PlayErrorCue();
			base.Error("e.ineligible_for_personal_upgrade", false);
			return;
		}
		if (this.playerState.GetCurrency() >= cost)
		{
			base.Play(SRSingleton<GameContext>.Instance.UITemplates.purchasePersonalUpgradeCue);
			this.playerState.SpendCurrency(cost, false);
			this.playerState.AddUpgrade(upgrade, true);
			this.RebuildUI();
			this.purchaseUI.PlayPurchaseFX();
			return;
		}
		base.PlayErrorCue();
		base.Error("e.insuf_coins", false);
	}

	// Token: 0x04001ED7 RID: 7895
	public Sprite titleIcon;

	// Token: 0x04001ED8 RID: 7896
	private PlayerState playerState;

	// Token: 0x04001ED9 RID: 7897
	private LookupDirector lookupDir;

	// Token: 0x04001EDA RID: 7898
	private PurchaseUI purchaseUI;

	// Token: 0x04001EDB RID: 7899
	private const string ALREADY_HAS_UPGRADE = "e.already_has_personal_upgrade";

	// Token: 0x04001EDC RID: 7900
	private const string INELIGIBLE_FOR_UPGRADE = "e.ineligible_for_personal_upgrade";
}
