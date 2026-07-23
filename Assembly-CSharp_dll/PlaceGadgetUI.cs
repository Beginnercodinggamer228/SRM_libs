using System;
using System.Collections.Generic;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020005E4 RID: 1508
public class PlaceGadgetUI : BaseUI
{
	// Token: 0x06001FB3 RID: 8115 RVA: 0x00078AC9 File Offset: 0x00076CC9
	public void SetSite(GadgetSite site, GadgetSiteModel siteModel)
	{
		this.site = site;
		this.siteModel = siteModel;
		SRSingleton<SceneContext>.Instance.TutorialDirector.OnPlaceGadgetOpen();
		this.RebuildUI();
	}

	// Token: 0x06001FB4 RID: 8116 RVA: 0x00078AF0 File Offset: 0x00076CF0
	public void RebuildUI()
	{
		for (int i = 0; i < base.transform.childCount; i++)
		{
			Destroyer.Destroy(base.transform.GetChild(i).gameObject, "PlaceGadgetUI.RebuildUI");
		}
		GameObject gameObject = this.CreatePurchaseUI();
		this.purchaseUI = gameObject.GetComponent<PurchaseUI>();
		gameObject.transform.SetParent(base.transform, false);
		this.statusArea = gameObject.GetComponent<PurchaseUI>().statusArea;
	}

	// Token: 0x06001FB5 RID: 8117 RVA: 0x00078B64 File Offset: 0x00076D64
	protected GameObject CreatePurchaseUI()
	{
		GadgetDirector gadgetDir = SRSingleton<SceneContext>.Instance.GadgetDirector;
		List<PurchaseUI.Purchasable> list = new List<PurchaseUI.Purchasable>();
		Dictionary<PediaDirector.Id, List<PurchaseUI.Purchasable>> dictionary = new Dictionary<PediaDirector.Id, List<PurchaseUI.Purchasable>>();
		Gadget.Id attachedId = this.site.GetAttachedId();
		if (attachedId == Gadget.Id.NONE)
		{
			using (IEnumerator<GadgetDefinition> enumerator = SRSingleton<GameContext>.Instance.LookupDirector.GadgetDefinitions.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					GadgetDefinition gadgetDefinition = enumerator.Current;
					GadgetDirector.PlacementError placementError = gadgetDir.GetPlacementError(this.site, gadgetDefinition.id);
					string str = Enum.GetName(typeof(Gadget.Id), gadgetDefinition.id).ToLowerInvariant();
					Gadget.Id finalId = gadgetDefinition.id;
					string warning = (placementError != null) ? placementError.message : ((gadgetDefinition.destroyOnRemoval || Gadget.IsLinkDestroyerType(gadgetDefinition.id)) ? "w.gadget_install_permanent" : null);
					PurchaseUI.Purchasable item = new PurchaseUI.Purchasable("m.gadget.name." + str, gadgetDefinition.icon, gadgetDefinition.icon, "m.gadget.desc." + str, 0, new PediaDirector.Id?(gadgetDefinition.pediaLink), delegate()
					{
						this.Place(finalId);
					}, () => gadgetDir.GetGadgetCount(finalId) > 0, () => gadgetDir.CanPlaceGadget(this.site, finalId), (placementError != null) ? placementError.button : null, warning, () => gadgetDir.GetGadgetCount(finalId), null, false);
					list.Add(item);
					List<PurchaseUI.Purchasable> list2 = dictionary.Get(gadgetDefinition.pediaLink);
					if (list2 == null)
					{
						list2 = new List<PurchaseUI.Purchasable>();
					}
					list2.Add(item);
					dictionary[gadgetDefinition.pediaLink] = list2;
				}
				goto IL_462;
			}
		}
		if (this.site.DestroysLinkedPairOnRemoval())
		{
			list.Add(new PurchaseUI.Purchasable(MessageUtil.Qualify("ui", "l.demolish_linked_gadget"), this.demolish.icon, this.demolish.img, MessageUtil.Qualify("ui", "m.desc.demolish_linked_gadget"), this.demolish.cost, null, new UnityAction(this.DemolishPair), () => true, () => true, "b.demolish", this.site.DestroyingWillDestroyContents() ? "w.destroying_gadget_destroys_contents" : null, null, null, false));
		}
		else if (this.site.DestroysOnRemoval() || GordoSnare.HasSnaredGordo(this.site))
		{
			list.Add(new PurchaseUI.Purchasable(MessageUtil.Qualify("ui", "l.demolish_gadget"), this.demolish.icon, this.demolish.img, MessageUtil.Qualify("ui", "m.desc.demolish_gadget"), this.demolish.cost, null, new UnityAction(this.Demolish), () => true, () => true, "b.demolish", this.site.DestroyingWillDestroyContents() ? "w.destroying_gadget_destroys_contents" : null, null, null, false));
		}
		else
		{
			string warning2 = ((attachedId == Gadget.Id.DRONE || attachedId == Gadget.Id.DRONE_ADVANCED) && this.site.GetAttached().GetComponentInChildren<Drone>().ammo.Any()) ? "w.drone_reprogram_drops_ammo" : (this.site.DestroyingWillDestroyContents() ? "w.pick_up_gadget_destroys_contents" : null);
			list.Add(new PurchaseUI.Purchasable(MessageUtil.Qualify("ui", "l.pick_up_gadget"), this.pickUp.icon, this.pickUp.img, MessageUtil.Qualify("ui", "m.desc.pick_up_gadget"), this.pickUp.cost, null, new UnityAction(this.PickUp), () => true, () => true, "b.pick_up", warning2, null, null, false));
		}
		IL_462:
		GameObject gameObject = SRSingleton<GameContext>.Instance.UITemplates.CreatePurchaseUI(this.titleIcon, MessageUtil.Qualify("ui", "t.place_gadget"), list.ToArray(), true, new PurchaseUI.OnClose(this.Close), false);
		if (attachedId == Gadget.Id.NONE)
		{
			List<PurchaseUI.Category> list3 = new List<PurchaseUI.Category>();
			foreach (PediaDirector.Id key in PediaUI.SCIENCE_ENTRIES)
			{
				if (dictionary.ContainsKey(key))
				{
					list3.Add(new PurchaseUI.Category(key.ToString().ToLowerInvariant(), dictionary[key].ToArray()));
				}
			}
			gameObject.GetComponent<PurchaseUI>().SetCategories(list3);
			gameObject.GetComponent<PurchaseUI>().SetPurchaseMsgs("b.place", "b.place");
		}
		return gameObject;
	}

	// Token: 0x06001FB6 RID: 8118 RVA: 0x000790B4 File Offset: 0x000772B4
	private void Place(Gadget.Id id)
	{
		GameObject prefab = SRSingleton<GameContext>.Instance.LookupDirector.GetGadgetDefinition(id).prefab;
		SRSingleton<SceneContext>.Instance.GameModel.InstantiateGadget(prefab, this.siteModel);
		this.site.RotateToPlayer();
		this.PlayPurchaseCue();
		SRSingleton<SceneContext>.Instance.GadgetDirector.SpendGadget(id);
		this.Close();
		AnalyticsUtil.CustomEvent("PlaceGadget." + id.ToString(), new Dictionary<string, object>
		{
			{
				"GadgetSite.Position",
				AnalyticsUtil.GetEventData(this.site.transform.position)
			},
			{
				"GadgetSite.Id",
				this.site.id
			},
			{
				"Gadget.Id",
				id
			}
		}, true);
	}

	// Token: 0x06001FB7 RID: 8119 RVA: 0x00079180 File Offset: 0x00077380
	public void Demolish()
	{
		if (!this.site.HasAttached())
		{
			base.Error("e.cannot_destroy_gadget", false);
			return;
		}
		if ((!this.site.DestroysOnRemoval() && !GordoSnare.HasSnaredGordo(this.site)) || this.site.DestroysLinkedPairOnRemoval())
		{
			base.Error("e.cannot_destroy_gadget", false);
			return;
		}
		this.site.DestroyAttached();
		base.Play(SRSingleton<GameContext>.Instance.UITemplates.removeGadgetCue);
		this.RebuildUI();
		this.purchaseUI.PlayPurchaseFX();
	}

	// Token: 0x06001FB8 RID: 8120 RVA: 0x0007920C File Offset: 0x0007740C
	public void DemolishPair()
	{
		if (!this.site.HasAttached())
		{
			base.Error("e.cannot_destroy_gadget", false);
			return;
		}
		if (!this.site.DestroysLinkedPairOnRemoval())
		{
			base.Error("e.cannot_destroy_gadget", false);
			return;
		}
		this.site.DestroyAttachedWithPair();
		base.Play(SRSingleton<GameContext>.Instance.UITemplates.removeGadgetCue);
		this.RebuildUI();
		this.purchaseUI.PlayPurchaseFX();
	}

	// Token: 0x06001FB9 RID: 8121 RVA: 0x00079280 File Offset: 0x00077480
	public void PickUp()
	{
		if (!this.site.HasAttached())
		{
			base.Error("e.cannot_pickup_gadget", false);
			return;
		}
		if (this.site.DestroysOnRemoval() || this.site.DestroysLinkedPairOnRemoval())
		{
			base.Error("e.cannot_pickup_gadget", false);
			return;
		}
		Gadget.Id attachedId = this.site.GetAttachedId();
		this.site.DestroyAttached();
		base.Play(SRSingleton<GameContext>.Instance.UITemplates.removeGadgetCue);
		SRSingleton<SceneContext>.Instance.GadgetDirector.AddGadget(attachedId);
		this.RebuildUI();
		this.purchaseUI.PlayPurchaseFX();
	}

	// Token: 0x06001FBA RID: 8122 RVA: 0x0007931B File Offset: 0x0007751B
	protected void PlayPurchaseCue()
	{
		base.Play(SRSingleton<GameContext>.Instance.UITemplates.placeGadgetCue);
	}

	// Token: 0x04001EE0 RID: 7904
	public PlaceGadgetUI.PurchaseItem demolish;

	// Token: 0x04001EE1 RID: 7905
	public PlaceGadgetUI.PurchaseItem pickUp;

	// Token: 0x04001EE2 RID: 7906
	public Sprite titleIcon;

	// Token: 0x04001EE3 RID: 7907
	private GadgetSite site;

	// Token: 0x04001EE4 RID: 7908
	private GadgetSiteModel siteModel;

	// Token: 0x04001EE5 RID: 7909
	private PurchaseUI purchaseUI;

	// Token: 0x04001EE6 RID: 7910
	private const string ERR_CANNOT_PICKUP_GADGET = "e.cannot_pickup_gadget";

	// Token: 0x04001EE7 RID: 7911
	private const string ERR_CANNOT_DESTROY_GADGET = "e.cannot_destroy_gadget";

	// Token: 0x020005E5 RID: 1509
	[Serializable]
	public class PurchaseItem
	{
		// Token: 0x04001EE8 RID: 7912
		public Sprite icon;

		// Token: 0x04001EE9 RID: 7913
		public Sprite img;

		// Token: 0x04001EEA RID: 7914
		public int cost;
	}
}
