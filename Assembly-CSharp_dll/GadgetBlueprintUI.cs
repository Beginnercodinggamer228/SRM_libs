using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000587 RID: 1415
public class GadgetBlueprintUI : BaseUI
{
	// Token: 0x06001D66 RID: 7526 RVA: 0x0006FB06 File Offset: 0x0006DD06
	public override void Awake()
	{
		base.Awake();
		this.RebuildUI();
		SRSingleton<SceneContext>.Instance.TutorialDirector.OnBuilderShopOpen();
	}

	// Token: 0x06001D67 RID: 7527 RVA: 0x0006FB24 File Offset: 0x0006DD24
	public void RebuildUI()
	{
		for (int i = 0; i < base.transform.childCount; i++)
		{
			Destroyer.Destroy(base.transform.GetChild(i).gameObject, "GadgetBlueprintUI.RebuildUI");
		}
		GameObject gameObject = this.CreatePurchaseUI();
		gameObject.transform.SetParent(base.transform, false);
		this.statusArea = gameObject.GetComponent<PurchaseUI>().statusArea;
	}

	// Token: 0x06001D68 RID: 7528 RVA: 0x0006FB8C File Offset: 0x0006DD8C
	protected GameObject CreatePurchaseUI()
	{
		GadgetDirector gadgetDir = SRSingleton<SceneContext>.Instance.GadgetDirector;
		List<PurchaseUI.Purchasable> list = new List<PurchaseUI.Purchasable>();
		using (IEnumerator<GadgetDefinition> enumerator = SRSingleton<GameContext>.Instance.LookupDirector.GadgetDefinitions.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				GadgetDefinition entry = enumerator.Current;
				string arg = Enum.GetName(typeof(Gadget.Id), entry.id).ToLowerInvariant();
				List<PurchaseUI.Purchasable> list2 = list;
				string nameKey = string.Format("m.gadget.name.{0}", arg);
				string descKey = string.Format("m.gadget.desc.{0}", arg);
				list2.Add(new PurchaseUI.Purchasable(nameKey, entry.icon, entry.icon, descKey, entry.blueprintCost, new PediaDirector.Id?(entry.pediaLink), delegate()
				{
					this.BuyBlueprint(entry.id);
				}, () => gadgetDir.HasBlueprint(entry.id) || gadgetDir.IsBlueprintUnlocked(entry.id), () => !gadgetDir.HasBlueprint(entry.id), null, null, null, null, false));
			}
		}
		return SRSingleton<GameContext>.Instance.UITemplates.CreatePurchaseUI(this.titleIcon, MessageUtil.Qualify("ui", "t.purchase_blueprint"), list.ToArray(), false, new PurchaseUI.OnClose(this.Close), false);
	}

	// Token: 0x06001D69 RID: 7529 RVA: 0x0006FCF8 File Offset: 0x0006DEF8
	public void BuyBlueprint(Gadget.Id id)
	{
		PlayerState playerState = SRSingleton<SceneContext>.Instance.PlayerState;
		GadgetDefinition gadgetDefinition = SRSingleton<GameContext>.Instance.LookupDirector.GetGadgetDefinition(id);
		if (playerState.GetCurrency() >= gadgetDefinition.blueprintCost)
		{
			playerState.SpendCurrency(gadgetDefinition.blueprintCost, false);
			SRSingleton<SceneContext>.Instance.GadgetDirector.AddBlueprint(id);
			this.PlayPurchaseCue();
			this.Close();
			return;
		}
		base.PlayErrorCue();
		base.Error("e.insuf_coins", false);
	}

	// Token: 0x06001D6A RID: 7530 RVA: 0x0006FA7C File Offset: 0x0006DC7C
	protected void PlayPurchaseCue()
	{
		base.Play(SRSingleton<GameContext>.Instance.UITemplates.purchaseBlueprintCue);
	}

	// Token: 0x04001C76 RID: 7286
	public Sprite titleIcon;
}
