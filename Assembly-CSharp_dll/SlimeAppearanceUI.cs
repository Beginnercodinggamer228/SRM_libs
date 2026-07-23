using System;
using System.Collections.Generic;
using System.Linq;
using DLCPackage;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000620 RID: 1568
public class SlimeAppearanceUI : BaseUI
{
	// Token: 0x060020E5 RID: 8421 RVA: 0x0007DB25 File Offset: 0x0007BD25
	public static bool IsEnabled()
	{
		return SRSingleton<GameContext>.Instance.DLCDirector.IsPackageInstalledAndEnabled(Id.SECRET_STYLE);
	}

	// Token: 0x060020E6 RID: 8422 RVA: 0x0007DB38 File Offset: 0x0007BD38
	public override void Awake()
	{
		base.Awake();
		this.contentPanel.SetActive(false);
		this.placeholderPanel.SetActive(true);
		this.tutorialDirector = SRSingleton<SceneContext>.Instance.TutorialDirector;
		this.pediaDirector = SRSingleton<SceneContext>.Instance.PediaDirector;
		if (!this.tutorialDirector.IsCompletedOrDisabled(TutorialDirector.Id.APPEARANCE_UI))
		{
			this.ShowTutorial(false);
		}
		else
		{
			this.HideTutorial();
		}
		this.slimeAppearanceCarouselCamSetup = UnityEngine.Object.Instantiate<GameObject>(this.slimeAppearanceCarouselPrefab);
		this.slimeAppearanceCarousel = this.slimeAppearanceCarouselCamSetup.GetComponentInChildren<SlimeAppearanceCarousel>();
		this.toggleButton.onClick.AddListener(delegate()
		{
			if (this.slimeAppearanceDirector.GetChosenSlimeAppearance(this.currentSlime) == this.currentSlime.Appearances.ElementAt(0))
			{
				this.slimeAppearanceCarousel.ConfirmSlimeAppearance(1);
				this.SetAppearanceSelectableStates(true, false);
				return;
			}
			this.slimeAppearanceCarousel.ConfirmSlimeAppearance(0);
			this.SetAppearanceSelectableStates(false, true);
		});
		this.selectAreaOne.onClick.AddListener(delegate()
		{
			this.slimeAppearanceCarousel.ConfirmSlimeAppearance(0);
			this.SetAppearanceSelectableStates(false, true);
		});
		this.selectAreaTwo.onClick.AddListener(delegate()
		{
			this.slimeAppearanceCarousel.ConfirmSlimeAppearance(1);
			this.SetAppearanceSelectableStates(true, false);
		});
		this.slimeAppearanceCarousel.onSlimeAppearanceConfirmed += delegate(SlimeDefinition definition, SlimeAppearance appearance)
		{
			base.Play(this.confirmCue);
			this.slimeAppearanceDirector.UpdateChosenSlimeAppearance(definition, appearance);
		};
		List<SlimeDefinition> list = this.slimeDefinitions.Slimes.Where(new Func<SlimeDefinition, bool>(this.ShouldShowSlimeInList)).ToList<SlimeDefinition>();
		this.PediaSortSlimes(list);
		bool flag = false;
		for (int i = 0; i < list.Count; i++)
		{
			SlimeDefinition slime = list[i];
			GameObject gameObject = this.AddButton(slime);
			if (!flag && this.IsSlimeAppearanceMenuUnlocked(slime))
			{
				gameObject.AddComponent<InitSelected>();
				flag = true;
			}
		}
	}

	// Token: 0x060020E7 RID: 8423 RVA: 0x0007DC91 File Offset: 0x0007BE91
	private void SetAppearanceSelectableStates(bool appearanceOne, bool appearanceTwo)
	{
		this.selectAreaOne.interactable = appearanceOne;
		this.selectAreaTwo.interactable = appearanceTwo;
	}

	// Token: 0x060020E8 RID: 8424 RVA: 0x0007DCAB File Offset: 0x0007BEAB
	private void PediaSortSlimes(List<SlimeDefinition> slimes)
	{
		slimes.Sort((SlimeDefinition slimeOne, SlimeDefinition slimeTwo) => this.pediaDirector.GetPediaId(slimeOne.IdentifiableId).Value.CompareTo(this.pediaDirector.GetPediaId(slimeTwo.IdentifiableId).Value));
	}

	// Token: 0x060020E9 RID: 8425 RVA: 0x0007DCBF File Offset: 0x0007BEBF
	private bool ShouldShowSlimeInList(SlimeDefinition slime)
	{
		return !slime.IsLargo && slime.Appearances.Count<SlimeAppearance>() > 1;
	}

	// Token: 0x060020EA RID: 8426 RVA: 0x0007DCD9 File Offset: 0x0007BED9
	private bool IsSlimeAppearanceMenuUnlocked(SlimeDefinition slime)
	{
		return this.slimeAppearanceDirector.GetUnlockedAppearances(slime).Count<SlimeAppearance>() > 1;
	}

	// Token: 0x060020EB RID: 8427 RVA: 0x0007DCEF File Offset: 0x0007BEEF
	public void OnEnable()
	{
		base.Play(this.openCue);
	}

	// Token: 0x060020EC RID: 8428 RVA: 0x0007DCFD File Offset: 0x0007BEFD
	public void OnDisable()
	{
		UnityEngine.Object.Destroy(this.slimeAppearanceCarouselCamSetup);
		base.Play(this.closeCue);
	}

	// Token: 0x060020ED RID: 8429 RVA: 0x0007DD16 File Offset: 0x0007BF16
	public void ShowTutorial(bool onTop)
	{
		this.appearancePanel.SetActive(false);
		this.tutorialPanel.SetActive(true);
		this.showTutorialButton.gameObject.SetActive(false);
		this.tutorialOnStack = onTop;
	}

	// Token: 0x060020EE RID: 8430 RVA: 0x0007DD48 File Offset: 0x0007BF48
	public void HideTutorial()
	{
		this.tutorialDirector.MarkTutorialCompleted(TutorialDirector.Id.APPEARANCE_UI);
		this.appearancePanel.SetActive(true);
		this.tutorialPanel.SetActive(false);
		this.showTutorialButton.gameObject.SetActive(true);
		this.tutorialOnStack = false;
	}

	// Token: 0x060020EF RID: 8431 RVA: 0x0007DD88 File Offset: 0x0007BF88
	private GameObject AddButton(SlimeDefinition slime)
	{
		GameObject gameObject = this.CreateButton(slime);
		gameObject.transform.SetParent(this.buttonListPanel.transform, false);
		gameObject.GetComponent<Toggle>().group = this.buttonListPanel.GetComponent<ToggleGroup>();
		return gameObject;
	}

	// Token: 0x060020F0 RID: 8432 RVA: 0x0007DDC0 File Offset: 0x0007BFC0
	private GameObject CreateButton(SlimeDefinition slime)
	{
		GameObject buttonObj = UnityEngine.Object.Instantiate<GameObject>(this.buttonListItemPrefab);
		Toggle component = buttonObj.GetComponent<Toggle>();
		TMP_Text component2 = buttonObj.transform.Find("Name").gameObject.GetComponent<TMP_Text>();
		Image component3 = buttonObj.transform.Find("Icon").gameObject.GetComponent<Image>();
		if (this.IsSlimeAppearanceMenuUnlocked(slime))
		{
			component2.text = Identifiable.GetName(slime.IdentifiableId, true);
			component3.sprite = slime.Appearances.First<SlimeAppearance>().Icon;
			UnityAction<bool> onButton = delegate(bool isOn)
			{
				if (isOn)
				{
					this.Select(slime);
				}
			};
			component.onValueChanged.AddListener(onButton);
			OnSelectDelegator.Create(buttonObj, delegate
			{
				onButton(true);
				buttonObj.GetComponent<Toggle>().isOn = true;
			});
		}
		else
		{
			component2.text = SRSingleton<GameContext>.Instance.MessageDirector.GetBundle("pedia").Get("t." + Enum.GetName(typeof(PediaDirector.Id), this.pediaDirector.lockedEntry.id).ToLowerInvariant());
			component3.sprite = this.pediaDirector.lockedEntry.icon;
			component.interactable = false;
		}
		return buttonObj;
	}

	// Token: 0x060020F1 RID: 8433 RVA: 0x0007DF5D File Offset: 0x0007C15D
	protected override void OnCancelPressed()
	{
		if (this.tutorialOnStack)
		{
			this.HideTutorial();
			return;
		}
		base.OnCancelPressed();
	}

	// Token: 0x060020F2 RID: 8434 RVA: 0x0007DF74 File Offset: 0x0007C174
	private void Select(SlimeDefinition slime)
	{
		this.currentSlime = slime;
		this.placeholderPanel.SetActive(false);
		this.contentPanel.SetActive(true);
		this.slimeAppearanceCarousel.ShowSlime(slime);
		this.appearanceOneNameText.SetKey(slime.Appearances.ElementAt(0).NameXlateKey);
		this.appearanceTwoNameText.SetKey(slime.Appearances.ElementAt(1).NameXlateKey);
		if (this.slimeAppearanceDirector.GetChosenSlimeAppearance(slime) == slime.Appearances.ElementAt(0))
		{
			this.SetAppearanceSelectableStates(false, true);
		}
		else
		{
			this.SetAppearanceSelectableStates(true, false);
		}
		this.confirmCue = ScriptableObject.CreateInstance<SECTR_AudioCue>();
		List<SECTR_AudioCue.ClipData> list = new List<SECTR_AudioCue.ClipData>();
		if (this.tabbySoundSlimes.Contains(slime.IdentifiableId))
		{
			list.AddRange(this.tabbySelectionCue.AudioClips);
		}
		else if (slime.IdentifiableId == Identifiable.Id.SABER_SLIME)
		{
			list.AddRange(this.saberSelectionCue.AudioClips);
		}
		else
		{
			list.AddRange(slime.Sounds.voiceFunCue.AudioClips);
		}
		this.confirmCue.Bus = this.sfxBus;
		this.confirmCue.Volume = this.confirmCueVolumeRange;
		this.confirmCue.MaxInstances = this.confirmCueMaxInstances;
		this.confirmCue.Pitch = new Vector2(0.9f, 1.1f);
		this.confirmCue.AudioClips = list;
		this.confirmCue.Spatialization = SECTR_AudioCue.Spatializations.Simple2D;
	}

	// Token: 0x04002030 RID: 8240
	public GameObject contentPanel;

	// Token: 0x04002031 RID: 8241
	public GameObject placeholderPanel;

	// Token: 0x04002032 RID: 8242
	public GameObject buttonListPanel;

	// Token: 0x04002033 RID: 8243
	public GameObject appearancePanel;

	// Token: 0x04002034 RID: 8244
	public GameObject tutorialPanel;

	// Token: 0x04002035 RID: 8245
	public Button showTutorialButton;

	// Token: 0x04002036 RID: 8246
	public GameObject buttonListItemPrefab;

	// Token: 0x04002037 RID: 8247
	public SECTR_AudioBus sfxBus;

	// Token: 0x04002038 RID: 8248
	public SECTR_AudioCue openCue;

	// Token: 0x04002039 RID: 8249
	public SECTR_AudioCue closeCue;

	// Token: 0x0400203A RID: 8250
	public Vector2 confirmCueVolumeRange = Vector2.one;

	// Token: 0x0400203B RID: 8251
	public SlimeDefinitions slimeDefinitions;

	// Token: 0x0400203C RID: 8252
	public SlimeAppearanceDirector slimeAppearanceDirector;

	// Token: 0x0400203D RID: 8253
	public XlateText appearanceOneNameText;

	// Token: 0x0400203E RID: 8254
	public XlateText appearanceTwoNameText;

	// Token: 0x0400203F RID: 8255
	public Button toggleButton;

	// Token: 0x04002040 RID: 8256
	public Button selectAreaOne;

	// Token: 0x04002041 RID: 8257
	public Button selectAreaTwo;

	// Token: 0x04002042 RID: 8258
	public GameObject slimeAppearanceCarouselPrefab;

	// Token: 0x04002043 RID: 8259
	public SECTR_AudioCue tabbySelectionCue;

	// Token: 0x04002044 RID: 8260
	public SECTR_AudioCue saberSelectionCue;

	// Token: 0x04002045 RID: 8261
	public int confirmCueMaxInstances = 5;

	// Token: 0x04002046 RID: 8262
	private readonly HashSet<Identifiable.Id> tabbySoundSlimes = new HashSet<Identifiable.Id>
	{
		Identifiable.Id.TABBY_SLIME,
		Identifiable.Id.LUCKY_SLIME,
		Identifiable.Id.HUNTER_SLIME
	};

	// Token: 0x04002047 RID: 8263
	private GameObject slimeAppearanceCarouselCamSetup;

	// Token: 0x04002048 RID: 8264
	private SlimeAppearanceCarousel slimeAppearanceCarousel;

	// Token: 0x04002049 RID: 8265
	private PediaDirector pediaDirector;

	// Token: 0x0400204A RID: 8266
	private TutorialDirector tutorialDirector;

	// Token: 0x0400204B RID: 8267
	private SlimeDefinition currentSlime;

	// Token: 0x0400204C RID: 8268
	private SECTR_AudioCue confirmCue;

	// Token: 0x0400204D RID: 8269
	private bool tutorialOnStack;
}
