using System;
using DG.Tweening;
using UnityEngine;

// Token: 0x020005A1 RID: 1441
public class IntroUI : BaseUI
{
	// Token: 0x06001DEF RID: 7663 RVA: 0x00071E4F File Offset: 0x0007004F
	public override void Awake()
	{
		base.Awake();
		this.introCanvasGroup = base.GetComponent<CanvasGroup>();
	}

	// Token: 0x06001DF0 RID: 7664 RVA: 0x00071E63 File Offset: 0x00070063
	public void OnEnable()
	{
		SECTR_AudioSystem.PauseNonUISFX(true);
		SRSingleton<SceneContext>.Instance.PopupDirector.RegisterSuppressor();
		SRSingleton<SceneContext>.Instance.TutorialDirector.SuppressTutorials();
		this.AnimateIntro();
	}

	// Token: 0x06001DF1 RID: 7665 RVA: 0x00071E8F File Offset: 0x0007008F
	public void OnDisable()
	{
		SECTR_AudioSystem.PauseNonUISFX(false);
		SRSingleton<SceneContext>.Instance.PopupDirector.UnregisterSuppressor();
		SRSingleton<SceneContext>.Instance.TutorialDirector.UnsuppressTutorials();
	}

	// Token: 0x06001DF2 RID: 7666 RVA: 0x00071EB8 File Offset: 0x000700B8
	private void AnimateIntro()
	{
		DOTween.Sequence().PrependInterval(0.25f).Append(this.introLine1.DOFade(1f, 1f)).AppendInterval(1.5f).Append(this.introLine2.DOFade(1f, 1f)).AppendInterval(1.5f).Append(this.introLine3.DOFade(1f, 1f)).AppendInterval(6f).Append(this.introLine1.DOFade(0f, 1f)).Join(this.introLine2.DOFade(0f, 1f)).Join(this.introLine3.DOFade(0f, 1f)).Append(this.introCanvasGroup.DOFade(0f, 1f)).OnComplete(delegate
		{
			this.endReached = true;
			this.Close();
		}).SetUpdate(true);
	}

	// Token: 0x06001DF3 RID: 7667 RVA: 0x00071FC0 File Offset: 0x000701C0
	protected override bool Closeable()
	{
		return this.endReached;
	}

	// Token: 0x04001D0D RID: 7437
	public GameObject background;

	// Token: 0x04001D0E RID: 7438
	public CanvasGroup introLine1;

	// Token: 0x04001D0F RID: 7439
	public CanvasGroup introLine2;

	// Token: 0x04001D10 RID: 7440
	public CanvasGroup introLine3;

	// Token: 0x04001D11 RID: 7441
	public const float fadeTime = 1f;

	// Token: 0x04001D12 RID: 7442
	public const float fadeTimeMargin = 0.25f;

	// Token: 0x04001D13 RID: 7443
	public const float introLineFadeTime = 1f;

	// Token: 0x04001D14 RID: 7444
	public const float introLineTime = 2.5f;

	// Token: 0x04001D15 RID: 7445
	public const float introLastLineTime = 7f;

	// Token: 0x04001D16 RID: 7446
	private bool endReached;

	// Token: 0x04001D17 RID: 7447
	private CanvasGroup introCanvasGroup;
}
