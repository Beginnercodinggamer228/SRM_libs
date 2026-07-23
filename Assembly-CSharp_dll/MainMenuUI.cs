using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x020005BB RID: 1467
public class MainMenuUI : SRBehaviour
{
	// Token: 0x06001E64 RID: 7780 RVA: 0x00073413 File Offset: 0x00071613
	public void Awake()
	{
		SRSingleton<GameContext>.Instance.MessageDirector.RegisterBundlesListener(new MessageDirector.BundlesListener(this.OnBundlesAvailable));
	}

	// Token: 0x06001E65 RID: 7781 RVA: 0x00073434 File Offset: 0x00071634
	public void Start()
	{
		Log.Debug("MainMenuUI.Start", Array.Empty<object>());
		this.DLCButton.gameObject.SetActive(DLCManageUI.IsEnabled());
		if (this.quitBtn != null)
		{
			this.quitBtn.gameObject.SetActive(true);
			this.expoQuitBtn.gameObject.SetActive(true);
			this.gdkUserPanel.gameObject.SetActive(false);
		}
		else
		{
			Log.Debug("quit button was null", Array.Empty<object>());
		}
		this.standardModePanel.SetActive(true);
		this.expoModePanel.SetActive(false);
		this.MaybeShowContinue();
	}

	// Token: 0x06001E66 RID: 7782 RVA: 0x000734D5 File Offset: 0x000716D5
	public void OnEnable()
	{
		this.MaybeShowContinue();
	}

	// Token: 0x06001E67 RID: 7783 RVA: 0x000734E0 File Offset: 0x000716E0
	private void MaybeShowContinue()
	{
		if (SRSingleton<GameContext>.Instance.AutoSaveDirector.HasContinue())
		{
			this.continueBtn.gameObject.SetActive(true);
			InitSelected component = this.loadBtn.gameObject.GetComponent<InitSelected>();
			if (component != null)
			{
				UnityEngine.Object.Destroy(component);
			}
			this.continueBtn.gameObject.AddComponent<InitSelected>();
			return;
		}
		this.continueBtn.gameObject.SetActive(false);
		InitSelected component2 = this.continueBtn.gameObject.GetComponent<InitSelected>();
		if (component2 != null)
		{
			UnityEngine.Object.Destroy(component2);
		}
		this.loadBtn.gameObject.AddComponent<InitSelected>();
	}

	// Token: 0x06001E68 RID: 7784 RVA: 0x00073583 File Offset: 0x00071783
	public virtual void OnDestroy()
	{
		if (SRSingleton<GameContext>.Instance != null)
		{
			SRSingleton<GameContext>.Instance.MessageDirector.UnregisterBundlesListener(new MessageDirector.BundlesListener(this.OnBundlesAvailable));
		}
	}

	// Token: 0x06001E69 RID: 7785 RVA: 0x000735AE File Offset: 0x000717AE
	public virtual void OnBundlesAvailable(MessageDirector msgDir)
	{
		this.uiBundle = msgDir.GetBundle("ui");
		this.SetupLanguages();
	}

	// Token: 0x06001E6A RID: 7786 RVA: 0x000735C7 File Offset: 0x000717C7
	public void OnButtonDLC()
	{
		this.InstantiateAndWaitForDestroy(this.manageDLCUI);
	}

	// Token: 0x06001E6B RID: 7787 RVA: 0x000735D8 File Offset: 0x000717D8
	public void ContinueGame()
	{
		this.SetInteractable(false);
		GameData.Summary saveToContinue = SRSingleton<GameContext>.Instance.AutoSaveDirector.GetSaveToContinue();
		SRSingleton<GameContext>.Instance.AutoSaveDirector.BeginLoad(saveToContinue.name, saveToContinue.saveName, delegate
		{
			this.SetInteractable(true);
			base.gameObject.SetActive(false);
			base.gameObject.SetActive(true);
		});
	}

	// Token: 0x06001E6C RID: 7788 RVA: 0x00073623 File Offset: 0x00071823
	public void LoadGame()
	{
		this.InstantiateAndWaitForDestroy(this.loadGameUI);
	}

	// Token: 0x06001E6D RID: 7789 RVA: 0x00073632 File Offset: 0x00071832
	public void SelectGame()
	{
		UnityEngine.Object.Instantiate<GameObject>(this.expoSelectGameUI);
		Destroyer.Destroy(base.gameObject, "MainMenuUI.SelectGame");
	}

	// Token: 0x06001E6E RID: 7790 RVA: 0x00073650 File Offset: 0x00071850
	public void NewGame()
	{
		this.InstantiateAndWaitForDestroy(this.newGameUI);
	}

	// Token: 0x06001E6F RID: 7791 RVA: 0x0007365F File Offset: 0x0007185F
	public void Quit()
	{
		Application.Quit();
	}

	// Token: 0x06001E70 RID: 7792 RVA: 0x00073666 File Offset: 0x00071866
	public void Options()
	{
		this.InstantiateAndWaitForDestroy(this.optionsUI);
	}

	// Token: 0x06001E71 RID: 7793 RVA: 0x00073675 File Offset: 0x00071875
	public void Credits()
	{
		this.InstantiateAndWaitForDestroy(this.creditsUI);
	}

	// Token: 0x06001E72 RID: 7794 RVA: 0x00073684 File Offset: 0x00071884
	public void Forums()
	{
		Application.OpenURL("http://forums.monomipark.com");
	}

	// Token: 0x06001E73 RID: 7795 RVA: 0x00073690 File Offset: 0x00071890
	public void SupportEmail()
	{
		Application.OpenURL("https://support.slimerancher.com/hc");
	}

	// Token: 0x06001E74 RID: 7796 RVA: 0x0007369C File Offset: 0x0007189C
	public GameObject InstantiateAndWaitForDestroy(GameObject prefab)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab);
		BaseUI component = gameObject.GetComponent<BaseUI>();
		base.gameObject.SetActive(false);
		component.onDestroy = (BaseUI.OnDestroyDelegate)Delegate.Combine(component.onDestroy, new BaseUI.OnDestroyDelegate(delegate()
		{
			if (this != null && base.gameObject != null)
			{
				base.gameObject.SetActive(true);
			}
		}));
		return gameObject;
	}

	// Token: 0x06001E75 RID: 7797 RVA: 0x000736D8 File Offset: 0x000718D8
	private void SetInteractable(bool interactable)
	{
		Selectable[] componentsInChildren = base.GetComponentsInChildren<Selectable>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].interactable = interactable;
		}
	}

	// Token: 0x06001E76 RID: 7798 RVA: 0x00073704 File Offset: 0x00071904
	private void SetupLanguages()
	{
		this.SetupDropdown<MessageDirector.Lang>(this.languageDropdown, "l.lang_", delegate(MessageDirector.Lang lang)
		{
			CultureInfo culture = SRSingleton<GameContext>.Instance.MessageDirector.GetCulture();
			return Enum.GetName(typeof(MessageDirector.Lang), lang).ToLowerInvariant() == culture.TwoLetterISOLanguageName;
		}, delegate(MessageDirector.Lang lang)
		{
			SRSingleton<GameContext>.Instance.MessageDirector.SetCulture(lang);
			SRSingleton<GameContext>.Instance.AutoSaveDirector.SaveProfile();
		});
	}

	// Token: 0x06001E77 RID: 7799 RVA: 0x00073760 File Offset: 0x00071960
	private void SetupDropdown<T>(TMP_Dropdown dropdown, string msgPrefix, Predicate<T> isLevel, UnityAction<T> assignLevel)
	{
		int num = 0;
		dropdown.options.Clear();
		dropdown.onValueChanged.RemoveAllListeners();
		foreach (object obj in Enum.GetValues(typeof(T)))
		{
			T t = (T)((object)obj);
			string name = Enum.GetName(typeof(T), t);
			string text = this.uiBundle.Xlate(msgPrefix + name.ToLowerInvariant());
			dropdown.options.Add(new TMP_Dropdown.OptionData(text));
			T fLevel = t;
			int fIdx = num;
			if (isLevel(fLevel))
			{
				dropdown.value = fIdx;
				dropdown.captionText.text = text;
			}
			dropdown.onValueChanged.AddListener(delegate(int val)
			{
				if (val == fIdx)
				{
					assignLevel(fLevel);
				}
			});
			num++;
		}
		UnityAction<int> call = delegate(int value)
		{
			Transform transform = dropdown.transform.Find("Dropdown List");
			if (transform != null)
			{
				Destroyer.Destroy(transform.gameObject, "MainMenuUI.SetupDropdown");
			}
		};
		dropdown.onValueChanged.AddListener(call);
	}

	// Token: 0x04001D7C RID: 7548
	public GameObject loadGameUI;

	// Token: 0x04001D7D RID: 7549
	public GameObject expoSelectGameUI;

	// Token: 0x04001D7E RID: 7550
	public GameObject newGameUI;

	// Token: 0x04001D7F RID: 7551
	public GameObject optionsUI;

	// Token: 0x04001D80 RID: 7552
	public GameObject creditsUI;

	// Token: 0x04001D81 RID: 7553
	[Tooltip("DLCManageUI prefab.")]
	public GameObject manageDLCUI;

	// Token: 0x04001D82 RID: 7554
	public GameObject expoModePanel;

	// Token: 0x04001D83 RID: 7555
	public GameObject standardModePanel;

	// Token: 0x04001D84 RID: 7556
	public Button continueBtn;

	// Token: 0x04001D85 RID: 7557
	public Button quitBtn;

	// Token: 0x04001D86 RID: 7558
	public Button loadBtn;

	// Token: 0x04001D87 RID: 7559
	public TMP_Text statusText;

	// Token: 0x04001D88 RID: 7560
	public Button expoQuitBtn;

	// Token: 0x04001D89 RID: 7561
	[Tooltip("DLC button.")]
	public Button DLCButton;

	// Token: 0x04001D8A RID: 7562
	public GameObject gdkUserPanel;

	// Token: 0x04001D8B RID: 7563
	public TMP_Dropdown languageDropdown;

	// Token: 0x04001D8C RID: 7564
	private MessageBundle uiBundle;

	// Token: 0x04001D8D RID: 7565
	private const string FORUMS_URL = "http://forums.monomipark.com";

	// Token: 0x04001D8E RID: 7566
	private const string SUPPORT_URL = "https://support.slimerancher.com/hc";
}
