using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200054E RID: 1358
public class ColorShopUI : BaseUI
{
	// Token: 0x06001C50 RID: 7248 RVA: 0x0006BF08 File Offset: 0x0006A108
	public override void Awake()
	{
		base.Awake();
		SRSingleton<SceneContext>.Instance.PediaDirector.UnlockWithoutPopup(PediaDirector.Id.CHROMA);
		this.achieveDir = SRSingleton<SceneContext>.Instance.AchievementsDirector;
		this.BuildUI();
	}

	// Token: 0x06001C51 RID: 7249 RVA: 0x0006BF3C File Offset: 0x0006A13C
	public void BuildUI()
	{
		for (int i = 0; i < base.transform.childCount; i++)
		{
			Destroyer.Destroy(base.transform.GetChild(i).gameObject, "ColorShopUI.BuildUI");
		}
		GameObject gameObject = this.CreatePurchaseUI();
		gameObject.transform.SetParent(base.transform, false);
		this.purchaseUI = gameObject.GetComponent<PurchaseUI>();
		this.statusArea = this.purchaseUI.statusArea;
	}

	// Token: 0x06001C52 RID: 7250 RVA: 0x0006BFB0 File Offset: 0x0006A1B0
	protected GameObject CreatePurchaseUI()
	{
		RanchDirector ranchDirector = SRSingleton<SceneContext>.Instance.RanchDirector;
		List<PurchaseUI.Purchasable> list = new List<PurchaseUI.Purchasable>();
		List<RanchDirector.PaletteEntry> orderedPalettes = SRSingleton<SceneContext>.Instance.RanchDirector.GetOrderedPalettes();
		List<PurchaseUI.Category> list2 = new List<PurchaseUI.Category>();
		foreach (RanchDirector.PaletteType paletteType in this.paletteTypes)
		{
			PurchaseUI.Purchasable[] array2 = new PurchaseUI.Purchasable[orderedPalettes.Count];
			for (int j = 0; j < orderedPalettes.Count; j++)
			{
				RanchDirector.PaletteEntry entry = orderedPalettes[j];
				PurchaseUI.Purchasable purchasable = this.CreatePurchasable(ranchDirector, entry, paletteType);
				list.Add(purchasable);
				array2[j] = purchasable;
			}
			string name = Enum.GetName(typeof(RanchDirector.PaletteType), paletteType).ToLowerInvariant();
			list2.Add(new PurchaseUI.Category(name, array2));
		}
		GameObject gameObject;
		(gameObject = SRSingleton<GameContext>.Instance.UITemplates.CreatePurchaseUI(this.titleIcon, MessageUtil.Qualify("ui", "t.chroma_packs"), list.ToArray(), true, new PurchaseUI.OnClose(this.Close), true)).GetComponent<PurchaseUI>().SetCategories(list2);
		GameObject result;
		(result = gameObject).GetComponent<PurchaseUI>().SetPurchaseMsgs("b.select", "b.already_selected");
		return result;
	}

	// Token: 0x06001C53 RID: 7251 RVA: 0x0006C0DC File Offset: 0x0006A2DC
	private PurchaseUI.Purchasable CreatePurchasable(RanchDirector ranchDir, RanchDirector.PaletteEntry entry, RanchDirector.PaletteType paletteType)
	{
		string str = Enum.GetName(typeof(RanchDirector.Palette), entry.palette).ToLowerInvariant();
		RanchDirector.Palette finalPalette = entry.palette;
		return new PurchaseUI.Purchasable("m.palette.name." + str, entry.icon, entry.icon, "m.palette.desc", 0, null, delegate()
		{
			this.SelectPalette(paletteType, finalPalette);
		}, () => ranchDir.HasPalette(finalPalette), () => !ranchDir.IsSelectedPalette(finalPalette, paletteType), null, null, null, null, false);
	}

	// Token: 0x06001C54 RID: 7252 RVA: 0x0006C184 File Offset: 0x0006A384
	private void SelectPalette(RanchDirector.PaletteType paletteType, RanchDirector.Palette palette)
	{
		SRSingleton<SceneContext>.Instance.GameModel.GetRanchModel().SelectPalette(paletteType, palette);
		base.Play(this.selectCue);
		this.purchaseUI.PlayPurchaseFX();
		this.purchaseUI.Rebuild(true);
		this.achieveDir.AddToStat(AchievementsDirector.EnumStat.USE_CHROMAS, paletteType);
	}

	// Token: 0x06001C55 RID: 7253 RVA: 0x0006C1DC File Offset: 0x0006A3DC
	protected void PlayPurchaseCue()
	{
		base.Play(SRSingleton<GameContext>.Instance.UITemplates.purchaseCue);
	}

	// Token: 0x04001B55 RID: 6997
	public Sprite titleIcon;

	// Token: 0x04001B56 RID: 6998
	public SECTR_AudioCue selectCue;

	// Token: 0x04001B57 RID: 6999
	public RanchDirector.PaletteType[] paletteTypes;

	// Token: 0x04001B58 RID: 7000
	private PurchaseUI purchaseUI;

	// Token: 0x04001B59 RID: 7001
	private AchievementsDirector achieveDir;
}
