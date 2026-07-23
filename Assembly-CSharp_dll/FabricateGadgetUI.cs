using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000584 RID: 1412
public class FabricateGadgetUI : BaseUI
{
	// Token: 0x06001D59 RID: 7513 RVA: 0x0006F67B File Offset: 0x0006D87B
	public override void Awake()
	{
		base.Awake();
		this.BuildUI();
		SRSingleton<SceneContext>.Instance.TutorialDirector.OnFabricatorOpen();
	}

	// Token: 0x06001D5A RID: 7514 RVA: 0x0006F698 File Offset: 0x0006D898
	public void BuildUI()
	{
		if (this.purchaseUI != null && this.purchaseUI.gameObject != null)
		{
			Destroyer.Destroy(this.purchaseUI.gameObject, "FabricateGadgetUI.BuildUI");
		}
		GameObject gameObject = this.CreatePurchaseUI();
		gameObject.transform.SetParent(base.transform, false);
		this.purchaseUI = gameObject.GetComponent<PurchaseUI>();
		this.statusArea = this.purchaseUI.statusArea;
	}

	// Token: 0x06001D5B RID: 7515 RVA: 0x0006F714 File Offset: 0x0006D914
	protected GameObject CreatePurchaseUI()
	{
		this.categoryMap.Clear();
		GadgetDirector gadgetDir = SRSingleton<SceneContext>.Instance.GadgetDirector;
		List<PurchaseUI.Purchasable> list = new List<PurchaseUI.Purchasable>();
		Dictionary<PediaDirector.Id, List<PurchaseUI.Purchasable>> dictionary = new Dictionary<PediaDirector.Id, List<PurchaseUI.Purchasable>>();
		foreach (GadgetDefinition gadgetDefinition in SRSingleton<GameContext>.Instance.LookupDirector.GadgetDefinitions)
		{
			string str = Enum.GetName(typeof(Gadget.Id), gadgetDefinition.id).ToLowerInvariant();
			GadgetDefinition finalDefinition = gadgetDefinition;
			Gadget.Id finalId = gadgetDefinition.id;
			string descKey = "m.gadget.desc." + str;
			string text = "m.gadget.name." + str;
			PurchaseUI.Purchasable item = new PurchaseUI.Purchasable(text, gadgetDefinition.icon, gadgetDefinition.icon, descKey, 0, new PediaDirector.Id?(gadgetDefinition.pediaLink), delegate()
			{
				this.Fabricate(finalId);
			}, () => gadgetDir.HasBlueprint(finalId), () => gadgetDir.CanAddGadget(finalDefinition), null, null, () => gadgetDir.GetGadgetCount(finalId), gadgetDefinition.craftCosts, false);
			list.Add(item);
			this.categoryMap[text] = gadgetDefinition.pediaLink.ToString().ToLowerInvariant();
			List<PurchaseUI.Purchasable> list2 = dictionary.Get(gadgetDefinition.pediaLink);
			if (list2 == null)
			{
				list2 = new List<PurchaseUI.Purchasable>();
			}
			list2.Add(item);
			dictionary[gadgetDefinition.pediaLink] = list2;
		}
		GameObject gameObject = SRSingleton<GameContext>.Instance.UITemplates.CreatePurchaseUI(this.titleIcon, MessageUtil.Qualify("ui", "t.fabricate_gadget"), list.ToArray(), true, new PurchaseUI.OnClose(this.Close), false);
		List<PurchaseUI.Category> list3 = new List<PurchaseUI.Category>();
		foreach (PediaDirector.Id key in PediaUI.SCIENCE_ENTRIES)
		{
			if (dictionary.ContainsKey(key))
			{
				list3.Add(new PurchaseUI.Category(key.ToString().ToLowerInvariant(), dictionary[key].ToArray()));
			}
		}
		gameObject.GetComponent<PurchaseUI>().SetCategories(list3);
		gameObject.GetComponent<PurchaseUI>().SetPurchaseMsgs("b.fabricate", "b.sold_out");
		return gameObject;
	}

	// Token: 0x06001D5C RID: 7516 RVA: 0x0006F990 File Offset: 0x0006DB90
	public void Fabricate(Gadget.Id id)
	{
		GadgetDirector gadgetDirector = SRSingleton<SceneContext>.Instance.GadgetDirector;
		AchievementsDirector achievementsDirector = SRSingleton<SceneContext>.Instance.AchievementsDirector;
		GadgetDefinition gadgetDefinition = SRSingleton<GameContext>.Instance.LookupDirector.GetGadgetDefinition(id);
		if (!gadgetDirector.CanAddGadget(gadgetDefinition))
		{
			base.PlayErrorCue();
			base.Error("e.cannot_add_gagdget", false);
			return;
		}
		if (this.TrySpendResources(gadgetDefinition.craftCosts))
		{
			base.ClearStatus();
			this.PlayPurchaseCue();
			gadgetDirector.AddGadget(id);
			achievementsDirector.AddToStat(AchievementsDirector.GameIntStat.FABRICATED_GADGETS, 1);
			if (gadgetDefinition.buyInPairs)
			{
				gadgetDirector.AddGadget(id);
			}
			AnalyticsUtil.CustomEvent("Fabricate", new Dictionary<string, object>
			{
				{
					"id",
					id.ToString()
				}
			}, true);
			this.purchaseUI.PlayPurchaseFX();
			this.purchaseUI.Rebuild(false);
			return;
		}
		base.PlayErrorCue();
		base.Error("e.insuf_craft_resources", false);
	}

	// Token: 0x06001D5D RID: 7517 RVA: 0x0006FA6A File Offset: 0x0006DC6A
	private bool TrySpendResources(GadgetDefinition.CraftCost[] costs)
	{
		return SRSingleton<SceneContext>.Instance.GadgetDirector.TryToSpendFromRefinery(costs);
	}

	// Token: 0x06001D5E RID: 7518 RVA: 0x0006FA7C File Offset: 0x0006DC7C
	protected void PlayPurchaseCue()
	{
		base.Play(SRSingleton<GameContext>.Instance.UITemplates.purchaseBlueprintCue);
	}

	// Token: 0x04001C6C RID: 7276
	public Sprite titleIcon;

	// Token: 0x04001C6D RID: 7277
	private PurchaseUI purchaseUI;

	// Token: 0x04001C6E RID: 7278
	private Dictionary<string, string> categoryMap = new Dictionary<string, string>();

	// Token: 0x04001C6F RID: 7279
	private const string ERR_INSUF_CRAFT_RESOURCES = "e.insuf_craft_resources";

	// Token: 0x04001C70 RID: 7280
	private const string ERR_CANNOT_ADD_GADGET = "e.cannot_add_gagdget";
}
