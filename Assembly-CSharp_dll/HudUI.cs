using System;
using System.Collections;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200059A RID: 1434
public class HudUI : SRSingleton<HudUI>
{
	// Token: 0x06001DCC RID: 7628 RVA: 0x00071658 File Offset: 0x0006F858
	public override void Awake()
	{
		base.Awake();
		this.optionsDir = SRSingleton<GameContext>.Instance.OptionsDirector;
		this.player = SRSingleton<SceneContext>.Instance.PlayerState;
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.mailDir = SRSingleton<SceneContext>.Instance.MailDirector;
		this.progressDir = SRSingleton<SceneContext>.Instance.ProgressDirector;
		this.autosaveDir = SRSingleton<GameContext>.Instance.AutoSaveDirector;
		this.mailInitY = this.mailIcon.rectTransform.anchoredPosition.y;
	}

	// Token: 0x06001DCD RID: 7629 RVA: 0x000716E6 File Offset: 0x0006F8E6
	public void Start()
	{
		this.Update();
		this.debugText.gameObject.SetActive(false);
		this.debugText.text = string.Empty;
	}

	// Token: 0x06001DCE RID: 7630 RVA: 0x00071710 File Offset: 0x0006F910
	public void Update()
	{
		bool showMinimalHUD = this.optionsDir.GetShowMinimalHUD();
		if (this.priorShowMinimalHud != showMinimalHUD)
		{
			bool active = !showMinimalHUD;
			for (int i = 0; i < this.disabledOnMinimalHud.Length; i++)
			{
				GameObject gameObject = this.disabledOnMinimalHud[i];
				if (gameObject != null)
				{
					gameObject.SetActive(active);
				}
			}
			this.priorShowMinimalHud = showMinimalHUD;
		}
		if (!showMinimalHUD)
		{
			int num = this.timeDir.CurrTime();
			if (num != this.lastTime)
			{
				this.dayText.text = this.timeDir.CurrDayString();
				this.timeText.text = this.timeDir.CurrTimeString();
				this.timeIcon.sprite = this.timeDir.CurrTimeIcon();
				this.lastTime = num;
			}
			int displayedCurrency = this.player.GetDisplayedCurrency();
			if (displayedCurrency != this.lastCurrency)
			{
				this.currencyText.text = displayedCurrency.ToString();
				this.lastCurrency = displayedCurrency;
			}
			int keys = this.player.GetKeys();
			if (keys != this.lastKeys)
			{
				if (keys > 0)
				{
					string text = keys.ToString();
					this.ScaleUpAndBack(this.keysIcon.gameObject);
					this.ScaleUpAndBack(this.keysText.gameObject);
					this.keysText.text = text;
				}
				this.keysIcon.enabled = (keys > 0);
				this.keysText.enabled = (keys > 0);
				this.lastKeys = keys;
			}
			bool flag = false;
			int progress = this.progressDir.GetProgress(ProgressDirector.ProgressType.CORPORATE_PARTNER);
			if (progress != this.lastPartnerLevel)
			{
				this.partnerArea.SetActive(progress > 0);
				this.partnerLevelText.text = progress.ToString();
				this.lastPartnerLevel = progress;
				flag = true;
			}
			bool flag2 = this.mailDir.HasNewMail();
			if (flag2 != this.mailIcon.enabled)
			{
				this.mailIcon.enabled = flag2;
				flag = true;
			}
			if (flag)
			{
				Vector3 v = this.mailIcon.rectTransform.anchoredPosition;
				v.y = this.mailInitY + ((progress > 0) ? 0f : 50f);
				this.mailIcon.rectTransform.anchoredPosition = v;
			}
		}
		this.autosaveImg.enabled = (Time.time - this.autosaveDir.GetLastSaveTime() < 5f);
	}

	// Token: 0x06001DCF RID: 7631 RVA: 0x0007196C File Offset: 0x0006FB6C
	private void ScaleUpAndBack(GameObject gameObject)
	{
		DOTween.Sequence().Append(gameObject.transform.DOScale(2f, 0.25f).SetEase(Ease.Linear)).Append(gameObject.transform.DOScale(1f, 0.25f).SetEase(Ease.Linear));
	}

	// Token: 0x04001CDC RID: 7388
	[Tooltip("UIContainer parent GameObject.")]
	public GameObject uiContainer;

	// Token: 0x04001CDD RID: 7389
	[Tooltip("Reference to the EnergyMeter child.")]
	public EnergyMeter energyMeter;

	// Token: 0x04001CDE RID: 7390
	public TMP_Text currencyText;

	// Token: 0x04001CDF RID: 7391
	public TMP_Text keysText;

	// Token: 0x04001CE0 RID: 7392
	public TMP_Text dayText;

	// Token: 0x04001CE1 RID: 7393
	public TMP_Text timeText;

	// Token: 0x04001CE2 RID: 7394
	public TMP_Text debugText;

	// Token: 0x04001CE3 RID: 7395
	public Image timeIcon;

	// Token: 0x04001CE4 RID: 7396
	public Image mailIcon;

	// Token: 0x04001CE5 RID: 7397
	public Image keysIcon;

	// Token: 0x04001CE6 RID: 7398
	public GameObject partnerArea;

	// Token: 0x04001CE7 RID: 7399
	public TMP_Text partnerLevelText;

	// Token: 0x04001CE8 RID: 7400
	public Image autosaveImg;

	// Token: 0x04001CE9 RID: 7401
	public GameObject keyGainFX;

	// Token: 0x04001CEA RID: 7402
	private PlayerState player;

	// Token: 0x04001CEB RID: 7403
	private TimeDirector timeDir;

	// Token: 0x04001CEC RID: 7404
	private MailDirector mailDir;

	// Token: 0x04001CED RID: 7405
	private ProgressDirector progressDir;

	// Token: 0x04001CEE RID: 7406
	private AutoSaveDirector autosaveDir;

	// Token: 0x04001CEF RID: 7407
	private OptionsDirector optionsDir;

	// Token: 0x04001CF0 RID: 7408
	private Hashtable scaleToTweenArgs;

	// Token: 0x04001CF1 RID: 7409
	private Hashtable scaleBackTweenArgs;

	// Token: 0x04001CF2 RID: 7410
	private float mailInitY;

	// Token: 0x04001CF3 RID: 7411
	private const float OFFSET_SPACING = 50f;

	// Token: 0x04001CF4 RID: 7412
	private const float GAME_SAVED_NOTIFICATION_DURATION = 5f;

	// Token: 0x04001CF5 RID: 7413
	private bool priorShowMinimalHud;

	// Token: 0x04001CF6 RID: 7414
	[Tooltip("Game objects in this list will be hidden when Minimal HUD is enabled.")]
	[SerializeField]
	private GameObject[] disabledOnMinimalHud;

	// Token: 0x04001CF7 RID: 7415
	private int lastTime = -1;

	// Token: 0x04001CF8 RID: 7416
	private int lastCurrency = -1;

	// Token: 0x04001CF9 RID: 7417
	private int lastKeys = -1;

	// Token: 0x04001CFA RID: 7418
	private int lastPartnerLevel = -1;
}
