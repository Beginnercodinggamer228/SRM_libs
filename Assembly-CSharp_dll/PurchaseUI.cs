using System;
using System.Collections.Generic;
using Assets.Script.Util.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x020005F1 RID: 1521
public class PurchaseUI : BaseUI
{
	// Token: 0x06001FE6 RID: 8166 RVA: 0x00079898 File Offset: 0x00077A98
	public override void Awake()
	{
		base.Awake();
		this.pediaBundle = SRSingleton<GameContext>.Instance.MessageDirector.GetBundle("pedia");
		this.actorBundle = SRSingleton<GameContext>.Instance.MessageDirector.GetBundle("actor");
		this.actionPanel.SetActive(false);
		this.placeholderPanel.SetActive(true);
	}

	// Token: 0x06001FE7 RID: 8167 RVA: 0x000798F7 File Offset: 0x00077AF7
	public void Init(Sprite icon, string titleKey, PurchaseUI.OnClose onClose)
	{
		this.titleImg.sprite = icon;
		this.titleText.text = this.pediaBundle.Xlate(titleKey);
		this.onClose = onClose;
		this.toggleMap.Clear();
	}

	// Token: 0x06001FE8 RID: 8168 RVA: 0x0007992E File Offset: 0x00077B2E
	public void PlayPurchaseFX()
	{
		if (this.purchaseFX != null)
		{
			UnityEngine.Object.Instantiate<GameObject>(this.purchaseFX).transform.SetParent(this.purchaseBtn.transform, false);
		}
	}

	// Token: 0x06001FE9 RID: 8169 RVA: 0x0007995F File Offset: 0x00077B5F
	public void SetPurchaseMsgs(string availMsg, string unavailMsg)
	{
		this.purchaseMsg = availMsg;
		this.purchaseUnavailMsg = unavailMsg;
		if (this.selected != null)
		{
			this.Select(this.selected);
		}
	}

	// Token: 0x06001FEA RID: 8170 RVA: 0x00079984 File Offset: 0x00077B84
	public void AddButton(PurchaseUI.Purchasable purchasable, bool unavailInMainList)
	{
		GameObject gameObject = this.CreateButton(purchasable);
		if (purchasable.avail() || unavailInMainList)
		{
			gameObject.transform.SetParent(this.buttonListPanel.transform, false);
		}
		else
		{
			gameObject.transform.SetParent(this.unavailButtonListPanel.transform, false);
		}
		gameObject.SetActive(purchasable.unlocked());
		gameObject.GetRequiredComponent<Toggle>().group = this.buttonListPanel.GetRequiredComponentInParent(true);
		this.buttonDict[purchasable] = gameObject;
	}

	// Token: 0x06001FEB RID: 8171 RVA: 0x00079A10 File Offset: 0x00077C10
	public void ClearButtons()
	{
		foreach (GameObject instance in this.buttonDict.Values)
		{
			Destroyer.Destroy(instance, "PurchaseUI.ClearButtons");
		}
	}

	// Token: 0x06001FEC RID: 8172 RVA: 0x00079A6C File Offset: 0x00077C6C
	public void Rebuild(bool unavailInMainList)
	{
		foreach (KeyValuePair<PurchaseUI.Purchasable, GameObject> keyValuePair in this.buttonDict)
		{
			this.UpdateButton(keyValuePair.Key, keyValuePair.Value);
			keyValuePair.Value.SetActive(keyValuePair.Key.unlocked());
			Transform transform = (keyValuePair.Key.avail() || unavailInMainList) ? this.buttonListPanel.transform : this.unavailButtonListPanel.transform;
			if (keyValuePair.Value.transform.parent != transform)
			{
				keyValuePair.Value.transform.SetParent(transform, false);
			}
		}
		if (this.selectedCategory != null)
		{
			this.ActivateCategory(this.selectedCategory);
		}
		if (this.buttonDict.ContainsKey(this.selected) && (this.selected.avail() || unavailInMainList))
		{
			this.buttonDict[this.selected].GetComponent<Toggle>().Select();
			return;
		}
		this.SelectFirst();
	}

	// Token: 0x06001FED RID: 8173 RVA: 0x00079BA8 File Offset: 0x00077DA8
	public void HideSelectionPanel()
	{
		this.selectionPanel.SetActive(false);
	}

	// Token: 0x06001FEE RID: 8174 RVA: 0x00079BB8 File Offset: 0x00077DB8
	private void SetCosts(GadgetDefinition.CraftCost[] costs)
	{
		this.costsPanel.SetActive(true);
		this.ClearCostListPanel(true);
		foreach (GadgetDefinition.CraftCost cost in costs)
		{
			this.CreateCost(cost).transform.SetParent(this.costListPanel.transform, false);
		}
	}

	// Token: 0x06001FEF RID: 8175 RVA: 0x00079C0C File Offset: 0x00077E0C
	private void ClearCostListPanel(bool isRequiresTextEnabled)
	{
		this.costListPanel.transform.GetChild(0).gameObject.SetActive(isRequiresTextEnabled);
		for (int i = 1; i < this.costListPanel.transform.childCount; i++)
		{
			Destroyer.Destroy(this.costListPanel.transform.GetChild(i).gameObject, "PurchaseUI.SetCosts");
		}
	}

	// Token: 0x06001FF0 RID: 8176 RVA: 0x00079C70 File Offset: 0x00077E70
	public void HideNubuckCost()
	{
		this.hideNubuckCosts = true;
	}

	// Token: 0x06001FF1 RID: 8177 RVA: 0x00079C7C File Offset: 0x00077E7C
	private GameObject CreateButton(PurchaseUI.Purchasable purchasable)
	{
		GameObject buttonObj = UnityEngine.Object.Instantiate<GameObject>(this.buttonListItemPrefab);
		this.UpdateButton(purchasable, buttonObj);
		Toggle component = buttonObj.GetComponent<Toggle>();
		component.onValueChanged.AddListener(delegate(bool isOn)
		{
			if (isOn)
			{
				this.Select(purchasable);
			}
		});
		OnSelectDelegator.Create(buttonObj, delegate
		{
			buttonObj.GetComponent<Toggle>().isOn = true;
		});
		this.toggleMap[component] = purchasable;
		return buttonObj;
	}

	// Token: 0x06001FF2 RID: 8178 RVA: 0x00079D14 File Offset: 0x00077F14
	private void UpdateButton(PurchaseUI.Purchasable purchasable, GameObject buttonObj)
	{
		MeshToggleButtonStyler component = buttonObj.GetComponent<MeshToggleButtonStyler>();
		TMP_Text component2 = buttonObj.transform.Find("Content/Name").gameObject.GetComponent<TMP_Text>();
		Image component3 = buttonObj.transform.Find("Content/Icon").gameObject.GetComponent<Image>();
		TMP_Text component4 = buttonObj.transform.Find("Content/Count").gameObject.GetComponent<TMP_Text>();
		component2.text = this.pediaBundle.Xlate(purchasable.nameKey);
		component3.sprite = purchasable.icon;
		int num = (purchasable.currCount == null) ? -1 : purchasable.currCount();
		component4.text = this.uiBundle.Xlate(MessageUtil.Tcompose("l.curr_count", new object[]
		{
			num
		}));
		component4.gameObject.SetActive(num >= 0);
		if (!purchasable.avail())
		{
			component.ChangeStyle("ListEntryUnavail");
			component3.material = this.unavailIconMat;
			return;
		}
		component.ChangeStyle("ListEntry");
		component3.material = null;
	}

	// Token: 0x06001FF3 RID: 8179 RVA: 0x00079E24 File Offset: 0x00078024
	private GameObject CreateCost(GadgetDefinition.CraftCost cost)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.costListItemPrefab);
		TMP_Text component = gameObject.transform.Find("Content/Name").gameObject.GetComponent<TMP_Text>();
		Image component2 = gameObject.transform.Find("Content/Icon").gameObject.GetComponent<Image>();
		TMP_Text component3 = gameObject.transform.Find("CountsOuterPanel/CountsPanel/Counts").gameObject.GetComponent<TMP_Text>();
		Sprite icon = SRSingleton<GameContext>.Instance.LookupDirector.GetIcon(cost.id);
		int refineryCount = SRSingleton<SceneContext>.Instance.GadgetDirector.GetRefineryCount(cost.id);
		component.text = this.actorBundle.Xlate("l." + cost.id.ToString().ToLowerInvariant());
		component2.sprite = icon;
		component3.text = this.uiBundle.Xlate(MessageUtil.Tcompose("m.count_of_required", new object[]
		{
			refineryCount,
			cost.amount
		}));
		if (refineryCount < cost.amount)
		{
			component3.color = Color.red;
		}
		return gameObject;
	}

	// Token: 0x06001FF4 RID: 8180 RVA: 0x00079F44 File Offset: 0x00078144
	public void Select(PurchaseUI.Purchasable purchasable)
	{
		this.actionPanel.SetActive(true);
		this.placeholderPanel.SetActive(false);
		this.selected = purchasable;
		this.selectedImg.sprite = purchasable.mainImg;
		this.selectedTitle.text = this.pediaBundle.Xlate(purchasable.nameKey);
		this.selectedDesc.text = this.pediaBundle.Xlate(purchasable.descKey);
		this.selectedCost.text = purchasable.cost.ToString();
		this.selectedCostPanel.SetActive(purchasable.cost > 0 && !this.hideNubuckCosts);
		this.selectedNoCostPanel.SetActive(purchasable.cost <= 0 && !this.hideNubuckCosts);
		this.selectedPediaBtn.gameObject.SetActive(purchasable.pediaId != null);
		this.SetupActivePurchaseButton(purchasable);
		if (purchasable.craftCosts != null)
		{
			this.SetCosts(purchasable.craftCosts);
		}
		this.warningText.gameObject.SetActive(purchasable.warning != null);
		if (purchasable.warning != null)
		{
			this.warningText.text = this.uiBundle.Xlate(purchasable.warning);
		}
		if (this.onSelected != null)
		{
			this.onSelected(purchasable);
		}
		if (purchasable.onSelected != null)
		{
			purchasable.onSelected(purchasable);
		}
	}

	// Token: 0x06001FF5 RID: 8181 RVA: 0x0007A0AC File Offset: 0x000782AC
	private void SetupActivePurchaseButton(PurchaseUI.Purchasable purchasable)
	{
		string text = this.uiBundle.Xlate((purchasable.btnOverride != null) ? purchasable.btnOverride : (purchasable.avail() ? this.purchaseMsg : this.purchaseUnavailMsg));
		bool interactable = purchasable.avail();
		if (purchasable.requireHoldToPurchase)
		{
			this.purchaseBtn.gameObject.SetActive(false);
			this.holdToPurchaseBtn.gameObject.SetActive(true);
			this.holdToPurchaseBtn.interactable = interactable;
			this.holdToPurchaseBtnText.text = text;
			return;
		}
		this.purchaseBtn.gameObject.SetActive(true);
		this.holdToPurchaseBtn.gameObject.SetActive(false);
		this.purchaseBtn.interactable = interactable;
		this.purchaseBtnText.text = text;
	}

	// Token: 0x06001FF6 RID: 8182 RVA: 0x0007A17C File Offset: 0x0007837C
	public void SetCategories(List<PurchaseUI.Category> categories)
	{
		this.tabsPanel.SetActive(true);
		Toggle toggle = null;
		this.categories = new Dictionary<string, PurchaseUI.Category>();
		foreach (PurchaseUI.Category category in categories)
		{
			this.categories[category.name] = category;
		}
		this.selectedCategory = ((categories.Count > 0) ? categories[0] : null);
		foreach (PurchaseUI.Category category2 in categories)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.catTabPrefab);
			gameObject.transform.SetParent(this.tabsPanel.transform, false);
			gameObject.GetComponentInChildren<XlateText>().SetKey("b." + category2.name);
			Toggle component = gameObject.GetComponent<Toggle>();
			if (toggle == null)
			{
				toggle = component;
			}
			component.group = this.tabsPanel.GetComponent<ToggleGroup>();
			PurchaseUI.Category fCategory = category2;
			component.onValueChanged.AddListener(delegate(bool isOn)
			{
				if (isOn)
				{
					this.ActivateCategory(fCategory);
					this.SelectFirst();
				}
			});
			this.categoryMap[category2.name] = component;
		}
		if (toggle != null)
		{
			toggle.isOn = true;
		}
	}

	// Token: 0x06001FF7 RID: 8183 RVA: 0x0007A2F8 File Offset: 0x000784F8
	private void ActivateCategory(PurchaseUI.Category category)
	{
		bool flag = false;
		foreach (KeyValuePair<Toggle, PurchaseUI.Purchasable> keyValuePair in this.toggleMap)
		{
			bool flag2 = Array.IndexOf<PurchaseUI.Purchasable>(category.items, keyValuePair.Value) != -1 && keyValuePair.Value.unlocked();
			keyValuePair.Key.gameObject.SetActive(flag2);
			keyValuePair.Key.isOn = false;
			flag = (flag || flag2);
		}
		this.actionPanel.SetActive(flag);
		this.placeholderPanel.SetActive(!flag);
		this.selectedCategory = category;
	}

	// Token: 0x06001FF8 RID: 8184 RVA: 0x0007A3B8 File Offset: 0x000785B8
	public void SelectFirst()
	{
		for (int i = 0; i < this.buttonListPanel.transform.childCount; i++)
		{
			GameObject gameObject = this.buttonListPanel.transform.GetChild(i).gameObject;
			if (gameObject.activeSelf)
			{
				gameObject.GetComponent<Toggle>().Select();
				return;
			}
		}
		for (int j = 0; j < this.unavailButtonListPanel.transform.childCount; j++)
		{
			GameObject gameObject2 = this.unavailButtonListPanel.transform.GetChild(j).gameObject;
			if (gameObject2.activeSelf)
			{
				gameObject2.GetComponent<Toggle>().Select();
				return;
			}
		}
		this.actionPanel.SetActive(false);
		this.placeholderPanel.SetActive(true);
		this.ClearCostListPanel(false);
		this.selected = null;
	}

	// Token: 0x06001FF9 RID: 8185 RVA: 0x0007A478 File Offset: 0x00078678
	public void Pedia()
	{
		if (!this.waitForPedia && this.selected != null && this.selected.pediaId != null)
		{
			this.waitForPedia = true;
			PediaUI component = SRSingleton<SceneContext>.Instance.PediaDirector.ShowPedia(this.selected.pediaId.Value).GetComponent<PediaUI>();
			component.onDestroy = (BaseUI.OnDestroyDelegate)Delegate.Combine(component.onDestroy, new BaseUI.OnDestroyDelegate(delegate()
			{
				if (SRSingleton<SceneContext>.Instance != null)
				{
					this.ReselectOnReturnFromPedia();
					this.waitForPedia = false;
				}
			}));
		}
	}

	// Token: 0x06001FFA RID: 8186 RVA: 0x0007A4F3 File Offset: 0x000786F3
	public void Purchase()
	{
		if (!this.waitForPedia && this.selected != null)
		{
			this.selected.onPurchase();
		}
	}

	// Token: 0x06001FFB RID: 8187 RVA: 0x0007A515 File Offset: 0x00078715
	public override void Close()
	{
		base.Close();
		if (this.onClose != null)
		{
			this.onClose();
		}
	}

	// Token: 0x06001FFC RID: 8188 RVA: 0x0007A530 File Offset: 0x00078730
	public PurchaseUI.Category GetSelectedCategory()
	{
		return this.selectedCategory;
	}

	// Token: 0x06001FFD RID: 8189 RVA: 0x0007A538 File Offset: 0x00078738
	public void Resize(float widthSelection, float widthAction)
	{
		this.selectionScroller.GetComponent<LayoutElement>().preferredWidth = widthSelection;
		this.actionPanel.GetComponent<LayoutElement>().preferredWidth = widthAction;
		this.loadingPanel.GetComponent<LayoutElement>().preferredWidth = widthSelection + widthAction;
	}

	// Token: 0x06001FFE RID: 8190 RVA: 0x0007A56F File Offset: 0x0007876F
	public void ReselectOnReturnFromPedia()
	{
		if (this.selected != null && this.buttonDict.ContainsKey(this.selected))
		{
			this.buttonDict[this.selected].GetComponent<Toggle>().Select();
			return;
		}
		this.SelectFirst();
	}

	// Token: 0x06001FFF RID: 8191 RVA: 0x0007A5B0 File Offset: 0x000787B0
	public void SetActivePanels(PurchaseUI.Panel active)
	{
		this.selectionPanel.SetActive((active & PurchaseUI.Panel.SELECTION) > PurchaseUI.Panel.NONE);
		this.actionPanel.SetActive((active & PurchaseUI.Panel.ACTION) > PurchaseUI.Panel.NONE);
		this.placeholderPanel.SetActive((active & PurchaseUI.Panel.PLACEHOLDER) > PurchaseUI.Panel.NONE);
		this.costsPanel.SetActive((active & PurchaseUI.Panel.COSTS) > PurchaseUI.Panel.NONE);
		this.loadingPanel.SetActive((active & PurchaseUI.Panel.LOADING) > PurchaseUI.Panel.NONE);
	}

	// Token: 0x04001F14 RID: 7956
	[Tooltip("Internal title image")]
	public Image titleImg;

	// Token: 0x04001F15 RID: 7957
	[Tooltip("Internal title text")]
	public TMP_Text titleText;

	// Token: 0x04001F16 RID: 7958
	[Tooltip("Internal button content panel")]
	public GameObject buttonListPanel;

	// Token: 0x04001F17 RID: 7959
	[Tooltip("Internal unavailable-item button content panel")]
	public GameObject unavailButtonListPanel;

	// Token: 0x04001F18 RID: 7960
	[Tooltip("Internal cost content panel")]
	public GameObject costListPanel;

	// Token: 0x04001F19 RID: 7961
	[Tooltip("Internal selected panel title image")]
	public Image selectedImg;

	// Token: 0x04001F1A RID: 7962
	[Tooltip("Internal selected panel title text")]
	public TMP_Text selectedTitle;

	// Token: 0x04001F1B RID: 7963
	[Tooltip("Internal selected panel description text")]
	public TMP_Text selectedDesc;

	// Token: 0x04001F1C RID: 7964
	[Tooltip("Internal selected panel purchase cost text")]
	public TMP_Text selectedCost;

	// Token: 0x04001F1D RID: 7965
	[Tooltip("Internal selected panel purchase cost panel")]
	public GameObject selectedCostPanel;

	// Token: 0x04001F1E RID: 7966
	[Tooltip("Internal selected panel purchase no-cost placeholder panel")]
	public GameObject selectedNoCostPanel;

	// Token: 0x04001F1F RID: 7967
	[Tooltip("Internal selected panel pedia button")]
	public Button selectedPediaBtn;

	// Token: 0x04001F20 RID: 7968
	[Tooltip("Internal selected panel purchase button")]
	public Button purchaseBtn;

	// Token: 0x04001F21 RID: 7969
	[Tooltip("Internal selected panel purchase button text")]
	public TMP_Text purchaseBtnText;

	// Token: 0x04001F22 RID: 7970
	[Tooltip("Internal hold to purchase button (for example, used when demolishing silos).")]
	public Button holdToPurchaseBtn;

	// Token: 0x04001F23 RID: 7971
	[Tooltip("Internal hold to purchase button text (for example, used when demolishing silos).")]
	public TMP_Text holdToPurchaseBtnText;

	// Token: 0x04001F24 RID: 7972
	[Tooltip("Internal main action right-side panel")]
	public GameObject actionPanel;

	// Token: 0x04001F25 RID: 7973
	[Tooltip("Internal placeholder right-side panel")]
	public GameObject placeholderPanel;

	// Token: 0x04001F26 RID: 7974
	[Tooltip("Internal panel for type-specific customizations")]
	public GameObject customizationPanel;

	// Token: 0x04001F27 RID: 7975
	[Tooltip("Internal costs panel far-right, not always active.")]
	public GameObject costsPanel;

	// Token: 0x04001F28 RID: 7976
	[Tooltip("Internal category tabs panel.")]
	public GameObject tabsPanel;

	// Token: 0x04001F29 RID: 7977
	[Tooltip("Internal item warning text.")]
	public TMP_Text warningText;

	// Token: 0x04001F2A RID: 7978
	[Tooltip("Internal scrolling region for selection list.")]
	public ScrollRect selectionScroller;

	// Token: 0x04001F2B RID: 7979
	[Tooltip("Internal loading panel.")]
	public GameObject loadingPanel;

	// Token: 0x04001F2C RID: 7980
	[Tooltip("Internal selection panel.")]
	public GameObject selectionPanel;

	// Token: 0x04001F2D RID: 7981
	public GameObject buttonListItemPrefab;

	// Token: 0x04001F2E RID: 7982
	public GameObject costListItemPrefab;

	// Token: 0x04001F2F RID: 7983
	public GameObject catTabPrefab;

	// Token: 0x04001F30 RID: 7984
	public Material unavailIconMat;

	// Token: 0x04001F31 RID: 7985
	public GameObject purchaseFX;

	// Token: 0x04001F32 RID: 7986
	private PurchaseUI.Purchasable selected;

	// Token: 0x04001F33 RID: 7987
	private Dictionary<string, PurchaseUI.Category> categories;

	// Token: 0x04001F34 RID: 7988
	private PurchaseUI.Category selectedCategory;

	// Token: 0x04001F35 RID: 7989
	public PurchaseUI.OnSelected onSelected;

	// Token: 0x04001F36 RID: 7990
	private PurchaseUI.OnClose onClose;

	// Token: 0x04001F37 RID: 7991
	private MessageBundle pediaBundle;

	// Token: 0x04001F38 RID: 7992
	private MessageBundle actorBundle;

	// Token: 0x04001F39 RID: 7993
	private Dictionary<Toggle, PurchaseUI.Purchasable> toggleMap = new Dictionary<Toggle, PurchaseUI.Purchasable>();

	// Token: 0x04001F3A RID: 7994
	private Dictionary<string, Toggle> categoryMap = new Dictionary<string, Toggle>();

	// Token: 0x04001F3B RID: 7995
	private Dictionary<PurchaseUI.Purchasable, GameObject> buttonDict = new Dictionary<PurchaseUI.Purchasable, GameObject>();

	// Token: 0x04001F3C RID: 7996
	private bool hideNubuckCosts;

	// Token: 0x04001F3D RID: 7997
	private bool waitForPedia;

	// Token: 0x04001F3E RID: 7998
	private string purchaseMsg = "b.purchase";

	// Token: 0x04001F3F RID: 7999
	private string purchaseUnavailMsg = "b.sold_out";

	// Token: 0x020005F2 RID: 1522
	// (Invoke) Token: 0x06002003 RID: 8195
	public delegate void OnClose();

	// Token: 0x020005F3 RID: 1523
	// (Invoke) Token: 0x06002007 RID: 8199
	public delegate void OnSelected(PurchaseUI.Purchasable purchasable);

	// Token: 0x020005F4 RID: 1524
	public class Category
	{
		// Token: 0x0600200A RID: 8202 RVA: 0x0007A66E File Offset: 0x0007886E
		public Category(string name, params PurchaseUI.Purchasable[] items)
		{
			this.name = name;
			this.items = items;
		}

		// Token: 0x04001F40 RID: 8000
		public string name;

		// Token: 0x04001F41 RID: 8001
		public PurchaseUI.Purchasable[] items;
	}

	// Token: 0x020005F5 RID: 1525
	public class Purchasable
	{
		// Token: 0x0600200B RID: 8203 RVA: 0x0007A684 File Offset: 0x00078884
		public Purchasable(string nameKey, Sprite icon, Sprite mainImg, string descKey, int cost, PediaDirector.Id? pediaId, UnityAction onPurchase, Func<bool> unlocked, Func<bool> avail, string btnOverride = null, string warning = null, Func<int> currCount = null, GadgetDefinition.CraftCost[] craftCosts = null, bool requireHoldToPurchase = false)
		{
			this.nameKey = nameKey;
			this.icon = icon;
			this.mainImg = mainImg;
			this.descKey = descKey;
			this.cost = cost;
			this.pediaId = pediaId;
			this.onPurchase = onPurchase;
			this.unlocked = unlocked;
			this.avail = avail;
			this.btnOverride = btnOverride;
			this.warning = warning;
			this.currCount = currCount;
			this.craftCosts = craftCosts;
			this.requireHoldToPurchase = requireHoldToPurchase;
		}

		// Token: 0x0600200C RID: 8204 RVA: 0x000053FC File Offset: 0x000035FC
		public Purchasable()
		{
		}

		// Token: 0x04001F42 RID: 8002
		public string nameKey;

		// Token: 0x04001F43 RID: 8003
		public Sprite icon;

		// Token: 0x04001F44 RID: 8004
		public Sprite mainImg;

		// Token: 0x04001F45 RID: 8005
		public string descKey;

		// Token: 0x04001F46 RID: 8006
		public int cost;

		// Token: 0x04001F47 RID: 8007
		public PediaDirector.Id? pediaId;

		// Token: 0x04001F48 RID: 8008
		public UnityAction onPurchase;

		// Token: 0x04001F49 RID: 8009
		public Func<bool> unlocked;

		// Token: 0x04001F4A RID: 8010
		public Func<bool> avail;

		// Token: 0x04001F4B RID: 8011
		public string btnOverride;

		// Token: 0x04001F4C RID: 8012
		public string warning;

		// Token: 0x04001F4D RID: 8013
		public Func<int> currCount;

		// Token: 0x04001F4E RID: 8014
		public GadgetDefinition.CraftCost[] craftCosts;

		// Token: 0x04001F4F RID: 8015
		public PurchaseUI.OnSelected onSelected;

		// Token: 0x04001F50 RID: 8016
		public bool requireHoldToPurchase;
	}

	// Token: 0x020005F6 RID: 1526
	[Flags]
	public enum Panel
	{
		// Token: 0x04001F52 RID: 8018
		NONE = 0,
		// Token: 0x04001F53 RID: 8019
		SELECTION = 1,
		// Token: 0x04001F54 RID: 8020
		ACTION = 2,
		// Token: 0x04001F55 RID: 8021
		PLACEHOLDER = 4,
		// Token: 0x04001F56 RID: 8022
		COSTS = 8,
		// Token: 0x04001F57 RID: 8023
		LOADING = 16,
		// Token: 0x04001F58 RID: 8024
		DEFAULT = 3
	}
}
