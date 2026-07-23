using System;
using System.Collections.Generic;
using TMPro;
using UiParticles;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200053B RID: 1339
public class AmmoSlotUI : SRSingleton<AmmoSlotUI>
{
	// Token: 0x06001BD3 RID: 7123 RVA: 0x0006A798 File Offset: 0x00068998
	public override void Awake()
	{
		base.Awake();
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.lookupDir = SRSingleton<GameContext>.Instance.LookupDirector;
		SRSingleton<GameContext>.Instance.MessageDirector.RegisterBundlesListener(new MessageDirector.BundlesListener(this.OnBundlesAvailable));
		this.animSelectedId = Animator.StringToHash("selected");
		for (int i = 0; i < this.slots.Length; i++)
		{
			this.slots[i].keyBinding.SetActive(true);
		}
	}

	// Token: 0x06001BD4 RID: 7124 RVA: 0x0006A81C File Offset: 0x00068A1C
	public void Start()
	{
		this.selected = UnityEngine.Object.Instantiate<GameObject>(this.selectedPrefab);
		this.player = SRSingleton<SceneContext>.Instance.PlayerState;
		this.slimeAppearanceDirector.onSlimeAppearanceChanged += this.OnSlimeAppearanceChanged;
	}

	// Token: 0x06001BD5 RID: 7125 RVA: 0x0006A858 File Offset: 0x00068A58
	public override void OnDestroy()
	{
		base.OnDestroy();
		this.slimeAppearanceDirector.onSlimeAppearanceChanged -= this.OnSlimeAppearanceChanged;
		if (SRSingleton<GameContext>.Instance != null)
		{
			SRSingleton<GameContext>.Instance.MessageDirector.UnregisterBundlesListener(new MessageDirector.BundlesListener(this.OnBundlesAvailable));
		}
	}

	// Token: 0x06001BD6 RID: 7126 RVA: 0x0006A8AC File Offset: 0x00068AAC
	public void OnBundlesAvailable(MessageDirector msgDir)
	{
		this.cachedNames.Clear();
		foreach (VacItemDefinition vacItemDefinition in this.lookupDir.VacItemDefinitions)
		{
			this.GetName(vacItemDefinition.Id, true);
		}
		this.cachedNames[Identifiable.Id.NONE] = " ";
		for (int i = 0; i < this.lastSlotIds.Length; i++)
		{
			this.lastSlotIds[i] = Identifiable.Id.NONE;
		}
	}

	// Token: 0x06001BD7 RID: 7127 RVA: 0x0006A940 File Offset: 0x00068B40
	private string GetName(Identifiable.Id id, bool recache = false)
	{
		if (recache || !this.cachedNames.ContainsKey(id))
		{
			this.cachedNames[id] = Identifiable.GetName(id, true);
		}
		return this.cachedNames[id];
	}

	// Token: 0x06001BD8 RID: 7128 RVA: 0x0006A974 File Offset: 0x00068B74
	public void Update()
	{
		int usableSlotCount = this.player.Ammo.GetUsableSlotCount();
		int selectedAmmoIdx = this.player.Ammo.GetSelectedAmmoIdx();
		for (int i = 0; i < this.slots.Length; i++)
		{
			AmmoSlotUI.Slot slot = this.slots[i];
			if (usableSlotCount != this.lastUsableSlotCount)
			{
				this.ToggleSlotUsability(slot, i, usableSlotCount);
			}
			if (i < usableSlotCount)
			{
				Identifiable.Id slotName = this.player.Ammo.GetSlotName(i);
				int slotMaxCount = this.player.Ammo.GetSlotMaxCount(i);
				int slotCount = this.player.Ammo.GetSlotCount(i);
				if (this.lastSlotCounts[i] != slotCount || this.lastSlotMaxAmmos[i] != slotMaxCount)
				{
					if (slotName != Identifiable.Id.NONE)
					{
						slot.bar.currValue = (float)slotCount;
						slot.bar.maxValue = (float)slotMaxCount;
						this.lastSlotCounts[i] = slotCount;
					}
					else
					{
						slot.bar.currValue = 0f;
						slot.bar.maxValue = (float)slotMaxCount;
						this.lastSlotCounts[i] = 0;
					}
					this.lastSlotMaxAmmos[i] = slotMaxCount;
				}
				if (this.lastSlotIds[i] != slotName)
				{
					slot.icon.enabled = (slotName > Identifiable.Id.NONE);
					slot.icon.sprite = this.GetCurrentIcon(slotName);
					slot.bar.barColor = this.GetCurrentColor(slotName);
					Sprite sprite = (i == this.slots.Length - 1) ? ((slot.bar.currValue == 0f) ? this.frontEmptyWater : this.frontFilledWater) : ((slot.bar.currValue == 0f) ? this.frontEmpty : this.frontFilled);
					Sprite sprite2 = (i == this.slots.Length - 1) ? ((slot.bar.currValue == 0f) ? this.backEmptyWater : this.backFilledWater) : ((slot.bar.currValue == 0f) ? this.backEmpty : this.backFilled);
					if (slot.front.sprite != sprite)
					{
						slot.front.sprite = sprite;
					}
					if (slot.back.sprite != sprite2)
					{
						slot.back.sprite = sprite2;
					}
					slot.label.text = this.GetName(slotName, false);
					this.lastSlotIds[i] = slotName;
				}
				if (this.lastSelectedAmmoIndex != selectedAmmoIdx)
				{
					this.slots[i].anim.SetBool(this.animSelectedId, selectedAmmoIdx == i);
					if (selectedAmmoIdx == i)
					{
						this.selected.transform.SetParent(slot.bar.transform);
						this.selected.transform.localPosition = Vector3.zero;
						this.selected.transform.localScale = Vector3.one;
						this.selected.transform.SetSiblingIndex(0);
					}
				}
			}
		}
		double remainingWaterIsMagicMins = this.player.Ammo.GetRemainingWaterIsMagicMins();
		if (remainingWaterIsMagicMins > 0.0)
		{
			this.liquidValueTimer.text = this.timeDir.FormatTime((int)Math.Floor(remainingWaterIsMagicMins));
			this.liquidFXObj.SetActive(true);
			this.liquidValueText.gameObject.SetActive(false);
			this.liquidValueTimer.gameObject.SetActive(true);
		}
		else
		{
			this.liquidValueText.gameObject.SetActive(true);
			this.liquidValueTimer.gameObject.SetActive(false);
			this.liquidFXObj.SetActive(false);
		}
		this.lastUsableSlotCount = usableSlotCount;
		this.lastSelectedAmmoIndex = selectedAmmoIdx;
	}

	// Token: 0x06001BD9 RID: 7129 RVA: 0x0006AD0C File Offset: 0x00068F0C
	private void OnSlimeAppearanceChanged(SlimeDefinition definition, SlimeAppearance appearance)
	{
		for (int i = 0; i < this.player.Ammo.GetUsableSlotCount(); i++)
		{
			AmmoSlotUI.Slot slot = this.slots[i];
			Identifiable.Id slotName = this.player.Ammo.GetSlotName(i);
			if (slotName == definition.IdentifiableId)
			{
				slot.icon.sprite = this.GetCurrentIcon(slotName);
				slot.bar.barColor = this.GetCurrentColor(slotName);
			}
		}
	}

	// Token: 0x06001BDA RID: 7130 RVA: 0x0006AD7C File Offset: 0x00068F7C
	private void ToggleSlotUsability(AmmoSlotUI.Slot slot, int slotIndex, int usableSlotCount)
	{
		GameObject gameObject = slot.label.transform.parent.gameObject;
		if (slotIndex >= usableSlotCount)
		{
			if (gameObject.activeSelf)
			{
				gameObject.SetActive(false);
			}
			return;
		}
		if (!gameObject.activeSelf)
		{
			gameObject.SetActive(true);
		}
	}

	// Token: 0x06001BDB RID: 7131 RVA: 0x0006ADC4 File Offset: 0x00068FC4
	public void SpawnAndPlayFX(GameObject prefab, int index, int count)
	{
		GameObject gameObject = SRBehaviour.SpawnAndPlayFX(prefab, this.slots[index].anim.gameObject);
		gameObject.GetComponent<ParticleSystem>().emission.SetBursts(new ParticleSystem.Burst[]
		{
			new ParticleSystem.Burst(0f, (float)count)
		});
		Sprite currentIcon = this.GetCurrentIcon(this.player.Ammo.GetSlotName(index));
		UiParticles component = gameObject.GetComponent<UiParticles>();
		component.materialForRendering.mainTexture = currentIcon.texture;
		component.SetMaterialDirty();
	}

	// Token: 0x06001BDC RID: 7132 RVA: 0x0006AE4F File Offset: 0x0006904F
	private Sprite GetCurrentIcon(Identifiable.Id id)
	{
		if (Identifiable.IsSlime(id))
		{
			return this.slimeAppearanceDirector.GetCurrentSlimeIcon(id);
		}
		if (id != Identifiable.Id.NONE)
		{
			return this.lookupDir.GetIcon(id);
		}
		return null;
	}

	// Token: 0x06001BDD RID: 7133 RVA: 0x0006AE77 File Offset: 0x00069077
	private Color GetCurrentColor(Identifiable.Id id)
	{
		if (Identifiable.IsSlime(id))
		{
			return this.slimeAppearanceDirector.GetChosenSlimeAppearance(id).ColorPalette.Ammo;
		}
		if (id != Identifiable.Id.NONE)
		{
			return this.lookupDir.GetColor(id);
		}
		return Color.clear;
	}

	// Token: 0x04001AF1 RID: 6897
	public GameObject selectedPrefab;

	// Token: 0x04001AF2 RID: 6898
	private const int MAX_SLOTS = 5;

	// Token: 0x04001AF3 RID: 6899
	public AmmoSlotUI.Slot[] slots;

	// Token: 0x04001AF4 RID: 6900
	public Sprite backEmpty;

	// Token: 0x04001AF5 RID: 6901
	public Sprite backFilled;

	// Token: 0x04001AF6 RID: 6902
	public Sprite frontEmpty;

	// Token: 0x04001AF7 RID: 6903
	public Sprite frontFilled;

	// Token: 0x04001AF8 RID: 6904
	public Sprite backEmptyWater;

	// Token: 0x04001AF9 RID: 6905
	public Sprite backFilledWater;

	// Token: 0x04001AFA RID: 6906
	public Sprite frontEmptyWater;

	// Token: 0x04001AFB RID: 6907
	public Sprite frontFilledWater;

	// Token: 0x04001AFC RID: 6908
	public TMP_Text liquidValueText;

	// Token: 0x04001AFD RID: 6909
	public TMP_Text liquidValueTimer;

	// Token: 0x04001AFE RID: 6910
	public GameObject liquidFXObj;

	// Token: 0x04001AFF RID: 6911
	public SlimeAppearanceDirector slimeAppearanceDirector;

	// Token: 0x04001B00 RID: 6912
	private GameObject selected;

	// Token: 0x04001B01 RID: 6913
	private PlayerState player;

	// Token: 0x04001B02 RID: 6914
	private TimeDirector timeDir;

	// Token: 0x04001B03 RID: 6915
	private LookupDirector lookupDir;

	// Token: 0x04001B04 RID: 6916
	private int animSelectedId;

	// Token: 0x04001B05 RID: 6917
	private Dictionary<Identifiable.Id, string> cachedNames = new Dictionary<Identifiable.Id, string>(Identifiable.idComparer);

	// Token: 0x04001B06 RID: 6918
	private int lastUsableSlotCount = -1;

	// Token: 0x04001B07 RID: 6919
	private int lastSelectedAmmoIndex = -1;

	// Token: 0x04001B08 RID: 6920
	private int[] lastSlotCounts = new int[5];

	// Token: 0x04001B09 RID: 6921
	private int[] lastSlotMaxAmmos = new int[5];

	// Token: 0x04001B0A RID: 6922
	private Identifiable.Id[] lastSlotIds = new Identifiable.Id[5];

	// Token: 0x0200053C RID: 1340
	[Serializable]
	public class Slot
	{
		// Token: 0x04001B0B RID: 6923
		public Image icon;

		// Token: 0x04001B0C RID: 6924
		public StatusBar bar;

		// Token: 0x04001B0D RID: 6925
		public Animator anim;

		// Token: 0x04001B0E RID: 6926
		public Image back;

		// Token: 0x04001B0F RID: 6927
		public Image front;

		// Token: 0x04001B10 RID: 6928
		public TMP_Text label;

		// Token: 0x04001B11 RID: 6929
		public GameObject keyBinding;
	}
}
