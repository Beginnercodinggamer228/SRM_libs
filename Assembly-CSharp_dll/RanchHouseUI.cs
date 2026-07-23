using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000600 RID: 1536
public class RanchHouseUI : BaseUI, LocationalUI
{
	// Token: 0x0600202E RID: 8238 RVA: 0x0007AD4C File Offset: 0x00078F4C
	public override void Awake()
	{
		base.Awake();
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.mailDir = SRSingleton<SceneContext>.Instance.MailDirector;
		this.musicDir = SRSingleton<GameContext>.Instance.MusicDirector;
		this.camDisabler = SRSingleton<SceneContext>.Instance.Player.GetComponentInChildren<CameraDisabler>();
		this.mainUI.SetActive(false);
		this.fadeMode = RanchHouseUI.FadeMode.IN_FADE_IN;
		this.mailButton.SetActive(SRSingleton<SceneContext>.Instance.GameModeConfig.GetModeSettings().AllowMail());
		this.mailInitY = this.mailIcon.rectTransform.anchoredPosition.y;
		this.progressDir = SRSingleton<SceneContext>.Instance.ProgressDirector;
		SRSingleton<SceneContext>.Instance.PlayerState.onEndGame += this.OnEndGame;
		SRSingleton<SceneContext>.Instance.PopupDirector.RegisterSuppressor();
		this.bgMat = new Material(this.backgroundImg.material);
		this.backgroundImg.material = this.bgMat;
		this.OnButtonsEnabled();
		SECTR_AudioSystem.PauseNonUISFX(true);
		this.DLCButton.SetActive(DLCManageUI.IsEnabled());
		this.appearanceButton.SetActive(SlimeAppearanceUI.IsEnabled());
	}

	// Token: 0x0600202F RID: 8239 RVA: 0x0007AE7E File Offset: 0x0007907E
	public void OnEnable()
	{
		this.musicDir.SetHouseMode(true);
		SRSingleton<PopupElementsUI>.Instance.RegisterBlocker(base.gameObject);
		this.beatrixObj = UnityEngine.Object.Instantiate<GameObject>(this.beatrixPrefab, Vector3.zero, Quaternion.identity);
		SECTR_AudioSystem.PauseNonUISFX(true);
	}

	// Token: 0x06002030 RID: 8240 RVA: 0x0007AEC0 File Offset: 0x000790C0
	public void OnDisable()
	{
		this.musicDir.SetHouseMode(false);
		if (SRSingleton<PopupElementsUI>.Instance != null)
		{
			SRSingleton<PopupElementsUI>.Instance.DeregisterBlocker(base.gameObject);
		}
		Destroyer.Destroy(this.beatrixObj, "RanchHouseUI.OnDisable");
		if (this.camDisabler != null)
		{
			this.camDisabler.RemoveBlocker(this);
			this.camDisabler = null;
		}
		SECTR_AudioSystem.PauseNonUISFX(false);
	}

	// Token: 0x06002031 RID: 8241 RVA: 0x0007AF2D File Offset: 0x0007912D
	public void OnButtonsEnabled()
	{
		this.partnerButton.SetActive(SRSingleton<SceneContext>.Instance.RanchDirector.IsPartnerUnlocked());
	}

	// Token: 0x06002032 RID: 8242 RVA: 0x0007AF4C File Offset: 0x0007914C
	public override void OnDestroy()
	{
		base.OnDestroy();
		SECTR_AudioSystem.PauseNonUISFX(false);
		Destroyer.Destroy(this.bgMat, "RanchHouseUI.onDestroy");
		if (SRSingleton<SceneContext>.Instance != null)
		{
			SRSingleton<SceneContext>.Instance.PlayerState.onEndGame -= this.OnEndGame;
			SRSingleton<SceneContext>.Instance.PopupDirector.UnregisterSuppressor();
		}
	}

	// Token: 0x06002033 RID: 8243 RVA: 0x0007AFAC File Offset: 0x000791AC
	private void OnEndGame()
	{
		this.Close();
		this.sleepButton.interactable = false;
	}

	// Token: 0x06002034 RID: 8244 RVA: 0x0007AFC0 File Offset: 0x000791C0
	public override void Update()
	{
		base.Update();
		this.timeText.text = this.timeDir.CurrTimeString();
		this.dayText.text = this.timeDir.CurrDayString();
		this.timeIcon.sprite = this.timeDir.CurrTimeIcon();
		this.mailIcon.enabled = this.mailDir.HasNewMail();
		this.mailHighlightText.text = this.mailDir.GetNewMailCount().ToString();
		this.mailHighlight.SetActive(this.mailDir.HasNewMail());
		int progress = this.progressDir.GetProgress(ProgressDirector.ProgressType.CORPORATE_PARTNER);
		this.partnerArea.SetActive(progress > 0);
		this.partnerLevelText.text = progress.ToString();
		Vector3 v = this.mailIcon.rectTransform.anchoredPosition;
		v.y = this.mailInitY + ((progress > 0) ? 0f : 50f);
		this.mailIcon.rectTransform.anchoredPosition = v;
		switch (this.fadeMode)
		{
		case RanchHouseUI.FadeMode.IN_FADE_IN:
		case RanchHouseUI.FadeMode.OUT_FADE_IN:
			this.obscurer.color = new Color(this.obscurer.color.r, this.obscurer.color.g, this.obscurer.color.b, Math.Min(1f, this.obscurer.color.a + Time.unscaledDeltaTime * 2f));
			break;
		case RanchHouseUI.FadeMode.IN_FADE_OUT:
		case RanchHouseUI.FadeMode.OUT_FADE_OUT:
			this.obscurer.color = new Color(this.obscurer.color.r, this.obscurer.color.g, this.obscurer.color.b, Math.Max(0f, this.obscurer.color.a - Time.unscaledDeltaTime * 2f));
			break;
		}
		if (this.fadeMode == RanchHouseUI.FadeMode.IN_FADE_IN && this.obscurer.color.a == 1f)
		{
			this.mainUI.SetActive(true);
			this.fadeMode = RanchHouseUI.FadeMode.IN_FADE_OUT;
			this.camDisabler.AddBlocker(this);
		}
		else if (this.fadeMode == RanchHouseUI.FadeMode.IN_FADE_OUT && this.obscurer.color.a == 0f)
		{
			this.fadeMode = RanchHouseUI.FadeMode.NONE;
		}
		else if (this.fadeMode == RanchHouseUI.FadeMode.OUT_FADE_IN && this.obscurer.color.a == 1f)
		{
			this.mainUI.SetActive(false);
			this.fadeMode = RanchHouseUI.FadeMode.OUT_FADE_OUT;
			this.camDisabler.RemoveBlocker(this);
		}
		else if (this.fadeMode == RanchHouseUI.FadeMode.OUT_FADE_OUT && this.obscurer.color.a == 0f)
		{
			this.fadeMode = RanchHouseUI.FadeMode.NONE;
			base.Close();
		}
		this.obscurer.gameObject.SetActive(this.obscurer.color.a > 0f);
		this.UpdateBackgroundMaterial();
	}

	// Token: 0x06002035 RID: 8245 RVA: 0x0007B2D8 File Offset: 0x000794D8
	public void Mail()
	{
		this.currMailUI = UnityEngine.Object.Instantiate<GameObject>(this.mailUI).GetComponent<MailUI>();
		this.buttonPanel.SetActive(false);
		MailUI mailUI = this.currMailUI;
		mailUI.onDestroy = (BaseUI.OnDestroyDelegate)Delegate.Combine(mailUI.onDestroy, new BaseUI.OnDestroyDelegate(delegate()
		{
			if (this.buttonPanel != null)
			{
				this.buttonPanel.SetActive(true);
				this.OnButtonsEnabled();
			}
		}));
	}

	// Token: 0x06002036 RID: 8246 RVA: 0x0007B330 File Offset: 0x00079530
	public void CorporatePartner()
	{
		this.currPartnerUI = UnityEngine.Object.Instantiate<GameObject>(this.partnerUI).GetComponent<CorporatePartnerUI>();
		this.buttonPanel.SetActive(false);
		CorporatePartnerUI corporatePartnerUI = this.currPartnerUI;
		corporatePartnerUI.onDestroy = (BaseUI.OnDestroyDelegate)Delegate.Combine(corporatePartnerUI.onDestroy, new BaseUI.OnDestroyDelegate(delegate()
		{
			if (this.buttonPanel != null)
			{
				this.buttonPanel.SetActive(true);
				this.OnButtonsEnabled();
			}
		}));
	}

	// Token: 0x06002037 RID: 8247 RVA: 0x0007B388 File Offset: 0x00079588
	public void SleepUntilMorning()
	{
		if (this.sleeping)
		{
			Debug.Log("Attempted to sleep while sleeping. Ignore.");
			return;
		}
		AnalyticsUtil.CustomEvent("PlayerSlept", null, true);
		this.sleeping = true;
		this.timeDir.Unpause(false, false);
		this.beatrixImg.DOFade(0f, 0.5f).SetUpdate(true);
		SRSingleton<LockOnDeath>.Instance.LockUntil(this.timeDir.GetNextDawn(), 0f, delegate
		{
			this.beatrixImg.DOFade(1f, 0.5f).SetUpdate(true);
			this.timeDir.Pause(false, false);
			this.sleeping = false;
		});
	}

	// Token: 0x06002038 RID: 8248 RVA: 0x0007B40C File Offset: 0x0007960C
	public void OnButtonDLC()
	{
		this.currDLCManageUI = UnityEngine.Object.Instantiate<GameObject>(this.manageDLCPrefab).GetComponent<DLCManageUI>();
		this.buttonPanel.SetActive(false);
		DLCManageUI dlcmanageUI = this.currDLCManageUI;
		dlcmanageUI.onDestroy = (BaseUI.OnDestroyDelegate)Delegate.Combine(dlcmanageUI.onDestroy, new BaseUI.OnDestroyDelegate(delegate()
		{
			if (this.buttonPanel != null)
			{
				this.buttonPanel.SetActive(true);
				this.OnButtonsEnabled();
			}
		}));
	}

	// Token: 0x06002039 RID: 8249 RVA: 0x0007B464 File Offset: 0x00079664
	public void OnButtonAppearances()
	{
		this.currAppearanceUI = UnityEngine.Object.Instantiate<GameObject>(this.appearancePrefab).GetComponent<SlimeAppearanceUI>();
		this.buttonPanel.SetActive(false);
		SlimeAppearanceUI slimeAppearanceUI = this.currAppearanceUI;
		slimeAppearanceUI.onDestroy = (BaseUI.OnDestroyDelegate)Delegate.Combine(slimeAppearanceUI.onDestroy, new BaseUI.OnDestroyDelegate(delegate()
		{
			if (this.buttonPanel != null)
			{
				this.buttonPanel.SetActive(true);
				this.OnButtonsEnabled();
			}
		}));
	}

	// Token: 0x0600203A RID: 8250 RVA: 0x0007B4BC File Offset: 0x000796BC
	protected override bool Closeable()
	{
		return !this.sleeping && !(this.currMailUI != null) && !(this.currPartnerUI != null) && !(this.currDLCManageUI != null) && !(this.currAppearanceUI != null) && !this.isClosing && base.Closeable();
	}

	// Token: 0x0600203B RID: 8251 RVA: 0x0007B520 File Offset: 0x00079720
	public override void Close()
	{
		this.progressDir.NoteReturnedToRanch();
		SECTR_AudioSystem.Play(this.closeCue, this.worldPos, false);
		SRSingleton<SceneContext>.Instance.Player.GetComponent<PlayerDeathHandler>().ResetPlayerLocation(0f, delegate
		{
			this.fadeMode = RanchHouseUI.FadeMode.OUT_FADE_IN;
		});
		this.isClosing = true;
	}

	// Token: 0x0600203C RID: 8252 RVA: 0x0007B578 File Offset: 0x00079778
	private void UpdateBackgroundMaterial()
	{
		float value = this.timeDir.CurrDayFraction();
		this.bgMat.SetFloat("_DayFraction", value);
	}

	// Token: 0x0600203D RID: 8253 RVA: 0x0007B5A2 File Offset: 0x000797A2
	public void SetPosition(Vector3 pos)
	{
		this.worldPos = pos;
		SECTR_AudioSystem.Play(this.openCue, this.worldPos, false);
	}

	// Token: 0x04001F77 RID: 8055
	public GameObject mailUI;

	// Token: 0x04001F78 RID: 8056
	public GameObject partnerUI;

	// Token: 0x04001F79 RID: 8057
	public GameObject manageDLCPrefab;

	// Token: 0x04001F7A RID: 8058
	public GameObject appearancePrefab;

	// Token: 0x04001F7B RID: 8059
	public TMP_Text dayText;

	// Token: 0x04001F7C RID: 8060
	public TMP_Text timeText;

	// Token: 0x04001F7D RID: 8061
	public Image timeIcon;

	// Token: 0x04001F7E RID: 8062
	public Image mailIcon;

	// Token: 0x04001F7F RID: 8063
	public GameObject mailHighlight;

	// Token: 0x04001F80 RID: 8064
	public TMP_Text mailHighlightText;

	// Token: 0x04001F81 RID: 8065
	public GameObject partnerArea;

	// Token: 0x04001F82 RID: 8066
	public TMP_Text partnerLevelText;

	// Token: 0x04001F83 RID: 8067
	public GameObject mainUI;

	// Token: 0x04001F84 RID: 8068
	public Image obscurer;

	// Token: 0x04001F85 RID: 8069
	public GameObject buttonPanel;

	// Token: 0x04001F86 RID: 8070
	public GameObject mailButton;

	// Token: 0x04001F87 RID: 8071
	public Button sleepButton;

	// Token: 0x04001F88 RID: 8072
	public GameObject partnerButton;

	// Token: 0x04001F89 RID: 8073
	public GameObject DLCButton;

	// Token: 0x04001F8A RID: 8074
	public GameObject appearanceButton;

	// Token: 0x04001F8B RID: 8075
	public Image backgroundImg;

	// Token: 0x04001F8C RID: 8076
	public Sprite dayBg;

	// Token: 0x04001F8D RID: 8077
	public Sprite nightBg;

	// Token: 0x04001F8E RID: 8078
	public Sprite dawnBg;

	// Token: 0x04001F8F RID: 8079
	public Sprite duskBg;

	// Token: 0x04001F90 RID: 8080
	public RawImage beatrixImg;

	// Token: 0x04001F91 RID: 8081
	public SECTR_AudioCue openCue;

	// Token: 0x04001F92 RID: 8082
	public SECTR_AudioCue closeCue;

	// Token: 0x04001F93 RID: 8083
	public GameObject beatrixPrefab;

	// Token: 0x04001F94 RID: 8084
	public bool isClosing;

	// Token: 0x04001F95 RID: 8085
	private RanchHouseUI.FadeMode fadeMode;

	// Token: 0x04001F96 RID: 8086
	private TimeDirector timeDir;

	// Token: 0x04001F97 RID: 8087
	private MailDirector mailDir;

	// Token: 0x04001F98 RID: 8088
	private ProgressDirector progressDir;

	// Token: 0x04001F99 RID: 8089
	private MailUI currMailUI;

	// Token: 0x04001F9A RID: 8090
	private CorporatePartnerUI currPartnerUI;

	// Token: 0x04001F9B RID: 8091
	private DLCManageUI currDLCManageUI;

	// Token: 0x04001F9C RID: 8092
	private SlimeAppearanceUI currAppearanceUI;

	// Token: 0x04001F9D RID: 8093
	private bool sleeping;

	// Token: 0x04001F9E RID: 8094
	private Material bgMat;

	// Token: 0x04001F9F RID: 8095
	private Vector3 worldPos;

	// Token: 0x04001FA0 RID: 8096
	private MusicDirector musicDir;

	// Token: 0x04001FA1 RID: 8097
	private float mailInitY;

	// Token: 0x04001FA2 RID: 8098
	private GameObject beatrixObj;

	// Token: 0x04001FA3 RID: 8099
	private CameraDisabler camDisabler;

	// Token: 0x04001FA4 RID: 8100
	private const float OFFSET_SPACING = 50f;

	// Token: 0x04001FA5 RID: 8101
	private const float FADE_RATE = 2f;

	// Token: 0x04001FA6 RID: 8102
	private const float BEATRIX_FADE_TIME = 0.5f;

	// Token: 0x02000601 RID: 1537
	private enum FadeMode
	{
		// Token: 0x04001FA8 RID: 8104
		NONE,
		// Token: 0x04001FA9 RID: 8105
		IN_FADE_IN,
		// Token: 0x04001FAA RID: 8106
		IN_FADE_OUT,
		// Token: 0x04001FAB RID: 8107
		OUT_FADE_IN,
		// Token: 0x04001FAC RID: 8108
		OUT_FADE_OUT
	}
}
