using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020005DC RID: 1500
public class PediaUI : BaseUI
{
	// Token: 0x06001F81 RID: 8065 RVA: 0x00077A78 File Offset: 0x00075C78
	public override void Awake()
	{
		base.Awake();
		this.pediaDir = SRSingleton<SceneContext>.Instance.PediaDirector;
		SRSingleton<GameContext>.Instance.MessageDirector.RegisterBundlesListener(new MessageDirector.BundlesListener(this.InitBundles));
		bool flag = true;
		foreach (PediaDirector.Id id in PediaUI.SCIENCE_ENTRIES)
		{
			if (this.pediaDir.IsUnlocked(id))
			{
				flag = false;
			}
		}
		if (flag)
		{
			this.scienceTab.gameObject.SetActive(false);
		}
		this.SelectEntry(PediaUI.TUTORIALS_ENTRIES[0], true, PediaUI.TUTORIALS_ENTRIES[0]);
	}

	// Token: 0x06001F82 RID: 8066 RVA: 0x00077B09 File Offset: 0x00075D09
	public void InitBundles(MessageDirector msgDir)
	{
		this.pediaBundle = msgDir.GetBundle("pedia");
	}

	// Token: 0x06001F83 RID: 8067 RVA: 0x00077B1C File Offset: 0x00075D1C
	public void SelectVacpack(GameObject toggleObj)
	{
		if (toggleObj.GetComponent<Toggle>().isOn)
		{
			this.SelectEntry(PediaUI.TUTORIALS_ENTRIES[0], true, PediaUI.TUTORIALS_ENTRIES[0]);
		}
	}

	// Token: 0x06001F84 RID: 8068 RVA: 0x00077B40 File Offset: 0x00075D40
	public void SelectSlimes(GameObject toggleObj)
	{
		if (toggleObj.GetComponent<Toggle>().isOn)
		{
			this.SelectEntry(PediaUI.SLIMES_ENTRIES[0], true, PediaUI.SLIMES_ENTRIES[0]);
		}
	}

	// Token: 0x06001F85 RID: 8069 RVA: 0x00077B64 File Offset: 0x00075D64
	public void SelectResources(GameObject toggleObj)
	{
		if (toggleObj.GetComponent<Toggle>().isOn)
		{
			this.SelectEntry(PediaUI.RESOURCES_ENTRIES[0], true, PediaUI.RESOURCES_ENTRIES[0]);
		}
	}

	// Token: 0x06001F86 RID: 8070 RVA: 0x00077B88 File Offset: 0x00075D88
	public void SelectRanch(GameObject toggleObj)
	{
		if (toggleObj.GetComponent<Toggle>().isOn)
		{
			this.SelectEntry(PediaUI.RANCH_ENTRIES[0], true, PediaUI.RANCH_ENTRIES[0]);
		}
	}

	// Token: 0x06001F87 RID: 8071 RVA: 0x00077BAC File Offset: 0x00075DAC
	public void SelectWorld(GameObject toggleObj)
	{
		if (toggleObj.GetComponent<Toggle>().isOn)
		{
			this.SelectEntry(PediaUI.WORLD_ENTRIES[0], true, PediaUI.WORLD_ENTRIES[0]);
		}
	}

	// Token: 0x06001F88 RID: 8072 RVA: 0x00077BD0 File Offset: 0x00075DD0
	public void SelectScience(GameObject toggleObj)
	{
		if (toggleObj.GetComponent<Toggle>().isOn)
		{
			this.SelectEntry(PediaUI.SCIENCE_ENTRIES[0], true, PediaUI.SCIENCE_ENTRIES[0]);
		}
	}

	// Token: 0x06001F89 RID: 8073 RVA: 0x00077BF4 File Offset: 0x00075DF4
	public void SelectEntry(PediaDirector.Id id, bool selectTab, PediaDirector.Id listingId)
	{
		PediaDirector.IdEntry idEntry = this.pediaDir.Get(id);
		if (selectTab)
		{
			this.SelectTabForId(id);
		}
		if (idEntry == null)
		{
			Debug.Log("Missing Pedia entry, using fallback icons and text.");
		}
		string text = (idEntry == null) ? "*UNKNOWN*" : Enum.GetName(typeof(PediaDirector.Id), idEntry.id).ToLowerInvariant();
		this.titleText.text = this.pediaBundle.Get("t." + text);
		this.introText.text = this.pediaBundle.Get("m.intro." + text);
		this.image.sprite = ((idEntry == null) ? this.pediaDir.unknownIcon : idEntry.icon);
		this.genericDescPanel.gameObject.SetActive(false);
		this.vacpackDescPanel.gameObject.SetActive(false);
		this.slimesDescPanel.gameObject.SetActive(false);
		this.resourcesDescPanel.gameObject.SetActive(false);
		this.ranchDescPanel.gameObject.SetActive(false);
		if (Array.IndexOf<PediaDirector.Id>(PediaUI.TUTORIALS_ENTRIES, id) != -1 && id != PediaDirector.Id.SPLASH)
		{
			this.PopulateVacpackDesc(text);
		}
		else if (Array.IndexOf<PediaDirector.Id>(PediaUI.SLIMES_ENTRIES, id) != -1)
		{
			this.PopulateSlimesDesc(text);
		}
		else if (Array.IndexOf<PediaDirector.Id>(PediaUI.RESOURCES_ENTRIES, id) != -1)
		{
			this.PopulateResourcesDesc(text);
		}
		else if (Array.IndexOf<PediaDirector.Id>(PediaUI.RANCH_ENTRIES, id) != -1)
		{
			this.PopulateRanchDesc(text);
		}
		else
		{
			this.PopulateGenericDesc(text);
		}
		if (this.listItems.ContainsKey(listingId))
		{
			Toggle toggle = this.listItems[listingId];
			if (toggle != null && EventSystem.current.currentSelectedGameObject != toggle.gameObject)
			{
				toggle.Select();
			}
			toggle.isOn = true;
		}
		base.StartCoroutine(this.DelayedResetScroller(this.descScroller));
	}

	// Token: 0x06001F8A RID: 8074 RVA: 0x00077DD0 File Offset: 0x00075FD0
	private void PopulateVacpackDesc(string lowerName)
	{
		this.vacpackDescPanel.gameObject.SetActive(true);
		string str = "m.instructions.gamepad.";
		if (InputDirector.UsingGamepad() && this.pediaBundle.Exists(str + lowerName))
		{
			this.instructionsText.text = this.pediaBundle.Get(str + lowerName);
		}
		else
		{
			this.instructionsText.text = this.pediaBundle.Get("m.instructions." + lowerName);
		}
		this.vacDescText.text = this.pediaBundle.Get("m.desc." + lowerName);
	}

	// Token: 0x06001F8B RID: 8075 RVA: 0x00077E70 File Offset: 0x00076070
	private void PopulateSlimesDesc(string lowerName)
	{
		this.slimesDescPanel.gameObject.SetActive(true);
		this.dietText.text = this.pediaBundle.Get("m.diet." + lowerName);
		this.favoriteText.text = this.pediaBundle.Get("m.favorite." + lowerName);
		this.biologyText.text = this.pediaBundle.Get("m.slimeology." + lowerName);
		this.risksText.text = this.pediaBundle.Get("m.risks." + lowerName);
		this.plortText.text = this.pediaBundle.Get("m.plortonomics." + lowerName);
	}

	// Token: 0x06001F8C RID: 8076 RVA: 0x00077F34 File Offset: 0x00076134
	private void PopulateResourcesDesc(string lowerName)
	{
		this.resourcesDescPanel.gameObject.SetActive(true);
		this.resourceTypeText.text = this.pediaBundle.Get("m.resource_type." + lowerName);
		if (this.pediaBundle.Exists("l.favored_by." + lowerName))
		{
			this.favoredByLabel.text = this.pediaBundle.Get("l.favored_by." + lowerName);
		}
		else
		{
			this.favoredByLabel.text = this.uiBundle.Get("l.favored_by");
		}
		this.favoredByText.text = this.pediaBundle.Get("m.favored_by." + lowerName);
		bool flag = this.pediaBundle.Exists("m.how_to_use." + lowerName);
		if (flag)
		{
			this.howToUseText.text = this.pediaBundle.Get("m.how_to_use." + lowerName);
		}
		this.howToUseArea.SetActive(flag);
		this.resourceDescText.text = this.pediaBundle.Get("m.desc." + lowerName);
	}

	// Token: 0x06001F8D RID: 8077 RVA: 0x00078054 File Offset: 0x00076254
	private void PopulateRanchDesc(string lowerName)
	{
		this.ranchDescPanel.gameObject.SetActive(true);
		for (int i = this.upgradesPanel.transform.childCount - 1; i >= 0; i--)
		{
			Destroyer.Destroy(this.upgradesPanel.GetChild(i).gameObject, "PediaUI.PopulateRanchDesc");
		}
		foreach (object obj in Enum.GetValues(typeof(LandPlot.Upgrade)))
		{
			LandPlot.Upgrade upgrade = (LandPlot.Upgrade)obj;
			string str = Enum.GetName(typeof(LandPlot.Upgrade), upgrade).ToLowerInvariant();
			string key = "m.upgrade.name." + lowerName + "." + str;
			if (this.pediaBundle.Exists(key))
			{
				GameObject gameObject = new GameObject("UpgradeNameText");
				gameObject.AddComponent<TextMeshProUGUI>().text = this.pediaBundle.Get(key);
				gameObject.AddComponent<MeshTextStyler>().SetStyle("LargeBold");
				gameObject.transform.SetParent(this.upgradesPanel, false);
				GameObject gameObject2 = new GameObject("UpgradeDescText");
				TMP_Text tmp_Text = gameObject2.AddComponent<TextMeshProUGUI>();
				string key2 = "m.upgrade.desc." + lowerName + "." + str;
				tmp_Text.text = this.pediaBundle.Get(key2);
				gameObject2.AddComponent<MeshTextStyler>().SetStyle("Default");
				gameObject2.transform.SetParent(this.upgradesPanel, false);
			}
		}
		this.ranchDescText.text = this.pediaBundle.Get("m.desc." + lowerName);
	}

	// Token: 0x06001F8E RID: 8078 RVA: 0x000781FC File Offset: 0x000763FC
	private void PopulateGenericDesc(string lowerName)
	{
		this.genericDescPanel.gameObject.SetActive(true);
		this.longDescText.text = this.pediaBundle.Get("m.desc." + lowerName);
	}

	// Token: 0x06001F8F RID: 8079 RVA: 0x00078230 File Offset: 0x00076430
	private void SelectTabForId(PediaDirector.Id id)
	{
		if (id != PediaDirector.Id.LOCKED)
		{
			if (Array.IndexOf<PediaDirector.Id>(PediaUI.TUTORIALS_ENTRIES, id) != -1 || id == PediaDirector.Id.TUTORIALS)
			{
				this.vacpackTab.isOn = true;
				this.BuildListing(PediaUI.TUTORIALS_ENTRIES);
			}
			else if (Array.IndexOf<PediaDirector.Id>(PediaUI.SLIMES_ENTRIES, id) != -1 || id == PediaDirector.Id.SLIMES)
			{
				this.slimesTab.isOn = true;
				this.BuildListing(PediaUI.SLIMES_ENTRIES);
			}
			else if (Array.IndexOf<PediaDirector.Id>(PediaUI.RESOURCES_ENTRIES, id) != -1 || id == PediaDirector.Id.RESOURCES)
			{
				this.resourcesTab.isOn = true;
				this.BuildListing(PediaUI.RESOURCES_ENTRIES);
			}
			else if (Array.IndexOf<PediaDirector.Id>(PediaUI.RANCH_ENTRIES, id) != -1 || id == PediaDirector.Id.RANCH)
			{
				this.ranchTab.isOn = true;
				this.BuildListing(PediaUI.RANCH_ENTRIES);
			}
			else if (Array.IndexOf<PediaDirector.Id>(PediaUI.WORLD_ENTRIES, id) != -1 || id == PediaDirector.Id.WORLD)
			{
				this.worldTab.isOn = true;
				this.BuildListing(PediaUI.WORLD_ENTRIES);
			}
			else if (Array.IndexOf<PediaDirector.Id>(PediaUI.SCIENCE_ENTRIES, id) != -1 || id == PediaDirector.Id.SCIENCE)
			{
				this.scienceTab.isOn = true;
				this.BuildListing(PediaUI.SCIENCE_ENTRIES);
			}
			else
			{
				Log.Debug("Could not find tab for pedia ID, skipping.", new object[]
				{
					"id",
					id
				});
			}
		}
		this.tabs.RecalcSelected();
	}

	// Token: 0x06001F90 RID: 8080 RVA: 0x00078380 File Offset: 0x00076580
	private void BuildListing(PediaDirector.Id[] ids)
	{
		for (int i = 0; i < this.listingPanel.childCount; i++)
		{
			Destroyer.Destroy(this.listingPanel.GetChild(i).gameObject, "PediaUI.BuildListing");
		}
		this.listItems.Clear();
		ToggleGroup component = this.listingPanel.GetComponent<ToggleGroup>();
		bool flag = true;
		foreach (PediaDirector.Id id in ids)
		{
			if (!PediaDirector.HIDDEN_ENTRIES.Contains(id) || this.pediaDir.IsUnlocked(id))
			{
				PediaDirector.IdEntry entry = this.pediaDir.Get(id);
				GameObject gameObject = this.CreateListing(this.pediaDir.pediaListingPrefab, entry, id);
				gameObject.transform.SetParent(this.listingPanel, false);
				if (flag && gameObject.activeSelf)
				{
					flag = false;
					gameObject.AddComponent<InitSelected>();
				}
				gameObject.GetComponent<Toggle>().group = component;
				this.listItems[id] = gameObject.GetComponent<Toggle>();
			}
		}
		base.StartCoroutine(this.DelayedResetScroller(this.listingScroller));
	}

	// Token: 0x06001F91 RID: 8081 RVA: 0x00078497 File Offset: 0x00076697
	private IEnumerator DelayedResetScroller(ScrollRect scroller)
	{
		yield return new WaitForEndOfFrame();
		scroller.verticalNormalizedPosition = 1f;
		yield break;
	}

	// Token: 0x06001F92 RID: 8082 RVA: 0x000784A8 File Offset: 0x000766A8
	private GameObject CreateListing(GameObject prefab, PediaDirector.IdEntry entry, PediaDirector.Id listingId)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab);
		TMP_Text component = gameObject.transform.Find("NameText").GetComponent<TMP_Text>();
		Image component2 = gameObject.transform.Find("Image").GetComponent<Image>();
		if (entry == null)
		{
			Debug.Log("Missing Pedia entry, using fallback icons and text.");
		}
		string str = (entry == null) ? "*UNKNOWN*" : Enum.GetName(typeof(PediaDirector.Id), entry.id).ToLowerInvariant();
		component.text = this.pediaBundle.Xlate("t." + str);
		component2.sprite = ((entry == null) ? this.pediaDir.unknownIcon : entry.icon);
		PediaListingUI listingUI = gameObject.GetComponent<PediaListingUI>();
		listingUI.id = ((entry == null) ? PediaDirector.Id.PINK_SLIME : entry.id);
		OnSelectDelegator.Create(gameObject, delegate
		{
			this.SelectEntry(listingUI.id, false, listingId);
		});
		return gameObject;
	}

	// Token: 0x06001F93 RID: 8083 RVA: 0x00013CC5 File Offset: 0x00011EC5
	protected override bool Closeable()
	{
		return true;
	}

	// Token: 0x04001EA3 RID: 7843
	public RectTransform listingPanel;

	// Token: 0x04001EA4 RID: 7844
	public ScrollRect listingScroller;

	// Token: 0x04001EA5 RID: 7845
	public TMP_Text titleText;

	// Token: 0x04001EA6 RID: 7846
	public TMP_Text introText;

	// Token: 0x04001EA7 RID: 7847
	public Image image;

	// Token: 0x04001EA8 RID: 7848
	public ScrollRect descScroller;

	// Token: 0x04001EA9 RID: 7849
	public TMP_Text longDescText;

	// Token: 0x04001EAA RID: 7850
	public TMP_Text dietText;

	// Token: 0x04001EAB RID: 7851
	public TMP_Text favoriteText;

	// Token: 0x04001EAC RID: 7852
	public TMP_Text biologyText;

	// Token: 0x04001EAD RID: 7853
	public TMP_Text risksText;

	// Token: 0x04001EAE RID: 7854
	public TMP_Text plortText;

	// Token: 0x04001EAF RID: 7855
	public TMP_Text instructionsText;

	// Token: 0x04001EB0 RID: 7856
	public TMP_Text vacDescText;

	// Token: 0x04001EB1 RID: 7857
	public TMP_Text resourceTypeText;

	// Token: 0x04001EB2 RID: 7858
	public TMP_Text favoredByLabel;

	// Token: 0x04001EB3 RID: 7859
	public TMP_Text favoredByText;

	// Token: 0x04001EB4 RID: 7860
	public TMP_Text howToUseText;

	// Token: 0x04001EB5 RID: 7861
	public GameObject howToUseArea;

	// Token: 0x04001EB6 RID: 7862
	public TMP_Text resourceDescText;

	// Token: 0x04001EB7 RID: 7863
	public RectTransform upgradesPanel;

	// Token: 0x04001EB8 RID: 7864
	public TMP_Text ranchDescText;

	// Token: 0x04001EB9 RID: 7865
	public Toggle vacpackTab;

	// Token: 0x04001EBA RID: 7866
	public Toggle slimesTab;

	// Token: 0x04001EBB RID: 7867
	public Toggle resourcesTab;

	// Token: 0x04001EBC RID: 7868
	public Toggle ranchTab;

	// Token: 0x04001EBD RID: 7869
	public Toggle worldTab;

	// Token: 0x04001EBE RID: 7870
	public Toggle scienceTab;

	// Token: 0x04001EBF RID: 7871
	public TabByMenuKeys tabs;

	// Token: 0x04001EC0 RID: 7872
	public RectTransform genericDescPanel;

	// Token: 0x04001EC1 RID: 7873
	public RectTransform vacpackDescPanel;

	// Token: 0x04001EC2 RID: 7874
	public RectTransform slimesDescPanel;

	// Token: 0x04001EC3 RID: 7875
	public RectTransform resourcesDescPanel;

	// Token: 0x04001EC4 RID: 7876
	public RectTransform ranchDescPanel;

	// Token: 0x04001EC5 RID: 7877
	private PediaDirector pediaDir;

	// Token: 0x04001EC6 RID: 7878
	private MessageBundle pediaBundle;

	// Token: 0x04001EC7 RID: 7879
	private static PediaDirector.Id[] TUTORIALS_ENTRIES = new PediaDirector.Id[]
	{
		PediaDirector.Id.BASICS,
		PediaDirector.Id.VACING,
		PediaDirector.Id.CAPTURETANKS,
		PediaDirector.Id.ENERGY,
		PediaDirector.Id.CORRALLING,
		PediaDirector.Id.FEEDING,
		PediaDirector.Id.PLORTS,
		PediaDirector.Id.SSBASICS,
		PediaDirector.Id.GADGETMODE,
		PediaDirector.Id.WILDS_TUTORIAL,
		PediaDirector.Id.VALLEY_TUTORIAL,
		PediaDirector.Id.SLIMULATIONS_TUTORIAL
	};

	// Token: 0x04001EC8 RID: 7880
	private static PediaDirector.Id[] SLIMES_ENTRIES = new PediaDirector.Id[]
	{
		PediaDirector.Id.PINK_SLIME,
		PediaDirector.Id.ROCK_SLIME,
		PediaDirector.Id.TABBY_SLIME,
		PediaDirector.Id.PHOSPHOR_SLIME,
		PediaDirector.Id.RAD_SLIME,
		PediaDirector.Id.BOOM_SLIME,
		PediaDirector.Id.HONEY_SLIME,
		PediaDirector.Id.PUDDLE_SLIME,
		PediaDirector.Id.CRYSTAL_SLIME,
		PediaDirector.Id.HUNTER_SLIME,
		PediaDirector.Id.QUANTUM_SLIME,
		PediaDirector.Id.FIRE_SLIME,
		PediaDirector.Id.DERVISH_SLIME,
		PediaDirector.Id.TANGLE_SLIME,
		PediaDirector.Id.MOSAIC_SLIME,
		PediaDirector.Id.SABER_SLIME,
		PediaDirector.Id.QUICKSILVER_SLIME,
		PediaDirector.Id.GLITCH_SLIME,
		PediaDirector.Id.GOLD_SLIME,
		PediaDirector.Id.LUCKY_SLIME,
		PediaDirector.Id.LARGO_SLIME,
		PediaDirector.Id.GORDO_SLIME,
		PediaDirector.Id.PARTY_GORDO_SLIME,
		PediaDirector.Id.ECHO_NOTE_GORDO_SLIME,
		PediaDirector.Id.FERAL_SLIME,
		PediaDirector.Id.TARR_SLIME
	};

	// Token: 0x04001EC9 RID: 7881
	private static PediaDirector.Id[] RESOURCES_ENTRIES = new PediaDirector.Id[]
	{
		PediaDirector.Id.CARROT,
		PediaDirector.Id.OCAOCA,
		PediaDirector.Id.BEET,
		PediaDirector.Id.PARSNIP,
		PediaDirector.Id.ONION,
		PediaDirector.Id.GINGER,
		PediaDirector.Id.POGO,
		PediaDirector.Id.MANGO,
		PediaDirector.Id.CUBERRY,
		PediaDirector.Id.LEMON,
		PediaDirector.Id.PEAR,
		PediaDirector.Id.KOOKADOBA,
		PediaDirector.Id.CHICKADOO,
		PediaDirector.Id.HENHEN,
		PediaDirector.Id.ROOSTRO,
		PediaDirector.Id.STONY_CHICKADOO,
		PediaDirector.Id.STONY_HEN,
		PediaDirector.Id.BRIAR_CHICKADOO,
		PediaDirector.Id.BRIAR_HEN,
		PediaDirector.Id.PAINTED_CHICKADOO,
		PediaDirector.Id.PAINTED_HEN,
		PediaDirector.Id.ELDER_HEN,
		PediaDirector.Id.ELDER_ROOSTRO,
		PediaDirector.Id.SPICY_TOFU,
		PediaDirector.Id.MANIFOLD_CUBE_CRAFT,
		PediaDirector.Id.PRIMORDY_OIL_CRAFT,
		PediaDirector.Id.DEEP_BRINE_CRAFT,
		PediaDirector.Id.SILKY_SAND_CRAFT,
		PediaDirector.Id.SPIRAL_STEAM_CRAFT,
		PediaDirector.Id.LAVA_DUST_CRAFT,
		PediaDirector.Id.BUZZ_WAX_CRAFT,
		PediaDirector.Id.WILD_HONEY_CRAFT,
		PediaDirector.Id.PEPPER_JAM_CRAFT,
		PediaDirector.Id.HEXACOMB_CRAFT,
		PediaDirector.Id.ROYAL_JELLY_CRAFT,
		PediaDirector.Id.JELLYSTONE_CRAFT,
		PediaDirector.Id.INDIGONIUM_CRAFT,
		PediaDirector.Id.GLASS_SHARD_CRAFT,
		PediaDirector.Id.SLIME_FOSSIL_CRAFT,
		PediaDirector.Id.STRANGE_DIAMOND_CRAFT,
		PediaDirector.Id.ECHOES,
		PediaDirector.Id.SLIME_TOYS,
		PediaDirector.Id.ECHO_NOTES,
		PediaDirector.Id.ORNAMENTS
	};

	// Token: 0x04001ECA RID: 7882
	private static PediaDirector.Id[] RANCH_ENTRIES = new PediaDirector.Id[]
	{
		PediaDirector.Id.CORRAL,
		PediaDirector.Id.COOP,
		PediaDirector.Id.GARDEN,
		PediaDirector.Id.SILO,
		PediaDirector.Id.INCINERATOR,
		PediaDirector.Id.POND,
		PediaDirector.Id.PLORT_MARKET,
		PediaDirector.Id.OVERGROWTH,
		PediaDirector.Id.GROTTO,
		PediaDirector.Id.DOCKS,
		PediaDirector.Id.LAB,
		PediaDirector.Id.OGDEN_RETREAT,
		PediaDirector.Id.MOCHI_MANOR,
		PediaDirector.Id.VIKTOR_LAB,
		PediaDirector.Id.PARTNER,
		PediaDirector.Id.CHROMA
	};

	// Token: 0x04001ECB RID: 7883
	private static PediaDirector.Id[] WORLD_ENTRIES = new PediaDirector.Id[]
	{
		PediaDirector.Id.THE_RANCH,
		PediaDirector.Id.REEF,
		PediaDirector.Id.QUARRY,
		PediaDirector.Id.MOSS,
		PediaDirector.Id.RUINS,
		PediaDirector.Id.DESERT,
		PediaDirector.Id.WILDS,
		PediaDirector.Id.VALLEY,
		PediaDirector.Id.SLIMULATIONS_WORLD,
		PediaDirector.Id.SEA,
		PediaDirector.Id.KEYS
	};

	// Token: 0x04001ECC RID: 7884
	public static PediaDirector.Id[] SCIENCE_ENTRIES = new PediaDirector.Id[]
	{
		PediaDirector.Id.REFINERY,
		PediaDirector.Id.FABRICATOR,
		PediaDirector.Id.BLUEPRINTS,
		PediaDirector.Id.EXTRACTORS,
		PediaDirector.Id.UTILITIES,
		PediaDirector.Id.WARP_TECH,
		PediaDirector.Id.DECORATIONS,
		PediaDirector.Id.CURIOS,
		PediaDirector.Id.DRONES
	};

	// Token: 0x04001ECD RID: 7885
	private Dictionary<PediaDirector.Id, Toggle> listItems = new Dictionary<PediaDirector.Id, Toggle>();
}
