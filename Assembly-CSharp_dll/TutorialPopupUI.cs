using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000633 RID: 1587
public class TutorialPopupUI : PopupUI<TutorialDirector.IdEntry>
{
	// Token: 0x06002144 RID: 8516 RVA: 0x0007F354 File Offset: 0x0007D554
	public virtual void Awake()
	{
		this.tutorialDir = SRSingleton<SceneContext>.Instance.TutorialDirector;
		InputDirector inputDirector = SRSingleton<GameContext>.Instance.InputDirector;
		inputDirector.onKeysChanged = (InputDirector.OnKeysChanged)Delegate.Combine(inputDirector.onKeysChanged, new InputDirector.OnKeysChanged(this.OnKeysChanged));
		this.anim = base.GetComponent<Animator>();
	}

	// Token: 0x06002145 RID: 8517 RVA: 0x0007F3A8 File Offset: 0x0007D5A8
	public override void OnDestroy()
	{
		base.OnDestroy();
		if (SRSingleton<GameContext>.Instance != null)
		{
			InputDirector inputDirector = SRSingleton<GameContext>.Instance.InputDirector;
			inputDirector.onKeysChanged = (InputDirector.OnKeysChanged)Delegate.Remove(inputDirector.onKeysChanged, new InputDirector.OnKeysChanged(this.OnKeysChanged));
		}
		if (this.tutorialDir != null)
		{
			this.tutorialDir.PopupDeactivated(this, this.wasCompleted);
		}
	}

	// Token: 0x06002146 RID: 8518 RVA: 0x0007F413 File Offset: 0x0007D613
	public void Complete()
	{
		this.wasCompleted = true;
		this.anim.SetTrigger("Complete");
		base.StartCoroutine(this.DestroyDelayed(0.167f));
	}

	// Token: 0x06002147 RID: 8519 RVA: 0x0007F43E File Offset: 0x0007D63E
	public void Hide()
	{
		this.wasCompleted = false;
		this.anim.SetTrigger("Close");
		base.StartCoroutine(this.DestroyDelayed(0.167f));
	}

	// Token: 0x06002148 RID: 8520 RVA: 0x0007F469 File Offset: 0x0007D669
	private IEnumerator DestroyDelayed(float delay)
	{
		yield return new WaitForSeconds(delay);
		Destroyer.Destroy(base.gameObject, "TutorialPopupUI.DestroyDelayed");
		yield break;
	}

	// Token: 0x06002149 RID: 8521 RVA: 0x0007F480 File Offset: 0x0007D680
	public override void Init(TutorialDirector.IdEntry tutorialIdEntry)
	{
		this.idEntry = tutorialIdEntry;
		base.Init(tutorialIdEntry);
		this.imgCycler.transform.localPosition += this.idEntry.imageOffset;
		this.imgCycler.transform.localScale = this.idEntry.imageScale;
	}

	// Token: 0x0600214A RID: 8522 RVA: 0x0007F4DC File Offset: 0x0007D6DC
	private void OnKeysChanged()
	{
		this.UpdateTutorial();
	}

	// Token: 0x0600214B RID: 8523 RVA: 0x0007F4E4 File Offset: 0x0007D6E4
	public override void OnBundleAvailable(MessageDirector msgDir)
	{
		this.tutorialBundle = msgDir.GetBundle("tutorial");
		this.UpdateTutorial();
	}

	// Token: 0x0600214C RID: 8524 RVA: 0x0007F500 File Offset: 0x0007D700
	private void UpdateTutorial()
	{
		if (this.tutorialBundle == null)
		{
			return;
		}
		string lowerName = Enum.GetName(typeof(TutorialDirector.Id), this.idEntry.id).ToLowerInvariant();
		this.UpdateTutorialInfo(lowerName);
		this.UpdateButtonLines(lowerName);
	}

	// Token: 0x0600214D RID: 8525 RVA: 0x0007F54C File Offset: 0x0007D74C
	private void UpdateTutorialInfo(string lowerName)
	{
		this.titleText.text = this.tutorialBundle.Get("t." + lowerName);
		string key = "m.text." + lowerName;
		if (this.tutorialBundle.Exists(key))
		{
			this.introText.text = this.tutorialBundle.Get(key);
			this.introText.gameObject.SetActive(true);
		}
		else
		{
			this.introText.gameObject.SetActive(false);
		}
		this.imgCycler.SetSprites(this.idEntry.images);
	}

	// Token: 0x0600214E RID: 8526 RVA: 0x0007F5E8 File Offset: 0x0007D7E8
	private void UpdateButtonLines(string lowerName)
	{
		string text = InputDirector.UsingGamepad() ? "gamepad." : "";
		for (int i = 0; i < this.buttonLines.Length; i++)
		{
			int num = i + 1;
			TutorialButtonLine tutorialButtonLine = this.buttonLines[i];
			string key = string.Concat(new object[]
			{
				"m.input.",
				text,
				lowerName,
				".",
				num
			});
			string key2 = string.Concat(new object[]
			{
				"m.inputdesc.",
				text,
				lowerName,
				".",
				num
			});
			if (this.tutorialBundle.Exists(key))
			{
				tutorialButtonLine.gameObject.SetActive(true);
				tutorialButtonLine.Init(this.tutorialBundle.Get(key), this.tutorialBundle.Get(key2));
			}
			else
			{
				tutorialButtonLine.gameObject.SetActive(false);
			}
		}
		this.buttonLinesPanel.SetActive(this.buttonLines.Length != 0);
	}

	// Token: 0x0600214F RID: 8527 RVA: 0x0007F6E9 File Offset: 0x0007D8E9
	public TutorialDirector.Id GetId()
	{
		return this.idEntry.id;
	}

	// Token: 0x06002150 RID: 8528 RVA: 0x0007F6F6 File Offset: 0x0007D8F6
	public void CompletedAction()
	{
		this.completedImg.gameObject.SetActive(true);
		if (this.onCompletedCue != null)
		{
			SECTR_AudioSystem.Play(this.onCompletedCue, Vector3.zero, false);
			this.onCompletedCue = null;
		}
	}

	// Token: 0x06002151 RID: 8529 RVA: 0x0007F730 File Offset: 0x0007D930
	public static GameObject CreateTutorialPopup(TutorialDirector.IdEntry tutorialIdEntry)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(SRSingleton<SceneContext>.Instance.TutorialDirector.tutorialPopupPrefab);
		gameObject.GetComponent<TutorialPopupUI>().Init(tutorialIdEntry);
		return gameObject;
	}

	// Token: 0x0400209C RID: 8348
	public GameObject buttonLinesPanel;

	// Token: 0x0400209D RID: 8349
	public TutorialButtonLine[] buttonLines;

	// Token: 0x0400209E RID: 8350
	public TMP_Text titleText;

	// Token: 0x0400209F RID: 8351
	public TMP_Text introText;

	// Token: 0x040020A0 RID: 8352
	public ImageCycler imgCycler;

	// Token: 0x040020A1 RID: 8353
	public Image completedImg;

	// Token: 0x040020A2 RID: 8354
	[Tooltip("SFX played when the tutorial action is completed. [2D, non-looping]")]
	public SECTR_AudioCue onCompletedCue;

	// Token: 0x040020A3 RID: 8355
	protected TutorialDirector tutorialDir;

	// Token: 0x040020A4 RID: 8356
	private Animator anim;

	// Token: 0x040020A5 RID: 8357
	private bool wasCompleted;

	// Token: 0x040020A6 RID: 8358
	private const string ANIM_COMPLETE = "Complete";

	// Token: 0x040020A7 RID: 8359
	private const string ANIM_CLOSE = "Close";

	// Token: 0x040020A8 RID: 8360
	private MessageBundle tutorialBundle;
}
