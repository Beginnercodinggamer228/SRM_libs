using System;
using DG.Tweening;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200054D RID: 1357
public class CoinsPopupUI : MonoBehaviour
{
	// Token: 0x06001C4B RID: 7243 RVA: 0x0006BDD3 File Offset: 0x00069FD3
	private void Start()
	{
		SECTR_AudioSystem.Play(this.cue, Vector3.zero, false);
		this.AnimateCoinPopup();
	}

	// Token: 0x06001C4C RID: 7244 RVA: 0x0006BDF0 File Offset: 0x00069FF0
	public void Init(int amount, Sprite overrideIcon, Color? overrideColor, SECTR_AudioCue overrideCue)
	{
		this.amountText.text = ((amount >= 0) ? "+" : "") + amount;
		if (overrideIcon != null)
		{
			this.icon.sprite = overrideIcon;
		}
		if (overrideColor != null)
		{
			this.amountText.color = overrideColor.Value;
		}
		if (overrideCue != null)
		{
			this.cue = overrideCue;
		}
	}

	// Token: 0x06001C4D RID: 7245 RVA: 0x0006BE68 File Offset: 0x0006A068
	private void AnimateCoinPopup()
	{
		DOTween.Sequence().Append(base.transform.DOBlendableMoveBy(Vector3.up * 180f, 1f, false)).Join(this.canvasGroup.DOFade(1f, 0.1f).From(0f, true)).Append(this.canvasGroup.DOFade(0f, 0.3f)).OnComplete(delegate
		{
			Destroyer.Destroy(base.gameObject, "CoinsPopupUI.DoSequence");
		}).SetUpdate(true);
	}

	// Token: 0x04001B4D RID: 6989
	public TMP_Text amountText;

	// Token: 0x04001B4E RID: 6990
	public Image icon;

	// Token: 0x04001B4F RID: 6991
	public SECTR_AudioCue cue;

	// Token: 0x04001B50 RID: 6992
	public CanvasGroup canvasGroup;

	// Token: 0x04001B51 RID: 6993
	private const float FADE_IN_TIME = 0.1f;

	// Token: 0x04001B52 RID: 6994
	private const float FADE_OUT_TIME = 0.3f;

	// Token: 0x04001B53 RID: 6995
	private const float MOVE_TIME = 1f;

	// Token: 0x04001B54 RID: 6996
	private const float MOVE_AMOUNT = 180f;
}
