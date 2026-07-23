using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000670 RID: 1648
[ExecuteInEditMode]
public class WorldStatusBar : MonoBehaviour
{
	// Token: 0x1700022F RID: 559
	// (set) Token: 0x06002224 RID: 8740 RVA: 0x000840E1 File Offset: 0x000822E1
	public Color barColor
	{
		set
		{
			if (this.statusImage != null)
			{
				this.statusImage.color = value;
			}
		}
	}

	// Token: 0x06002225 RID: 8741 RVA: 0x000840FD File Offset: 0x000822FD
	public void Start()
	{
		this.OnChanged();
	}

	// Token: 0x06002226 RID: 8742 RVA: 0x00084108 File Offset: 0x00082308
	public void Update()
	{
		if (this.minValue != this.lastMinValue || this.maxValue != this.lastMaxValue || this.currValue != this.lastCurrValue)
		{
			this.OnChanged();
			this.lastMinValue = this.minValue;
			this.lastMaxValue = this.maxValue;
			this.lastCurrValue = this.currValue;
		}
	}

	// Token: 0x06002227 RID: 8743 RVA: 0x0008416C File Offset: 0x0008236C
	private void OnChanged()
	{
		float num = (this.currValue - this.minValue) / (this.maxValue - this.minValue);
		float num2 = Mathf.Clamp01(num);
		if (this.label != null)
		{
			this.label.text = this.ApplyFormat((num <= 0f && !string.IsNullOrEmpty(this.emptyFormat)) ? this.emptyFormat : ((num == 1f && !string.IsNullOrEmpty(this.fullFormat)) ? this.fullFormat : ((num > 1f && !string.IsNullOrEmpty(this.overflowFormat)) ? this.overflowFormat : this.format)), num2);
		}
		if (this.statusImage != null)
		{
			this.statusImage.fillAmount = num2;
		}
	}

	// Token: 0x06002228 RID: 8744 RVA: 0x00084234 File Offset: 0x00082434
	protected virtual string ApplyFormat(string format, float pct)
	{
		format = format.Replace("{cur}", string.Concat(this.currValue));
		format = format.Replace("{min}", string.Concat(this.minValue));
		format = format.Replace("{max}", string.Concat(this.maxValue));
		format = format.Replace("{cur%}", string.Format("{0:00}", pct * 100f));
		format = format.Replace("{cur2%}", string.Format("{0:00.0}", pct * 100f));
		if (this.translate && Application.isPlaying)
		{
			if (this.uiBundle == null && SRSingleton<GameContext>.Instance != null)
			{
				this.uiBundle = SRSingleton<GameContext>.Instance.MessageDirector.GetBundle("ui");
			}
			if (this.uiBundle != null)
			{
				format = this.uiBundle.Xlate(format);
			}
		}
		return format;
	}

	// Token: 0x04002201 RID: 8705
	[Tooltip("The image for the bar which we're filling up.")]
	public Image statusImage;

	// Token: 0x04002202 RID: 8706
	[Tooltip("The text we will fill in.")]
	public Text label;

	// Token: 0x04002203 RID: 8707
	public float minValue;

	// Token: 0x04002204 RID: 8708
	public float maxValue = 100f;

	// Token: 0x04002205 RID: 8709
	public float currValue = 50f;

	// Token: 0x04002206 RID: 8710
	[Tooltip("The formatting to use to form our text.")]
	public string format;

	// Token: 0x04002207 RID: 8711
	[Tooltip("The formatting to use to form our text when empty, or null if we should use default.")]
	public string emptyFormat;

	// Token: 0x04002208 RID: 8712
	[Tooltip("The formatting to use to form our text when full, or null if we should use default.")]
	public string fullFormat;

	// Token: 0x04002209 RID: 8713
	[Tooltip("The formatting to use to form our text when overflowing, or null if we should use default.")]
	public string overflowFormat;

	// Token: 0x0400220A RID: 8714
	public bool translate;

	// Token: 0x0400220B RID: 8715
	private float lastMinValue = float.NaN;

	// Token: 0x0400220C RID: 8716
	private float lastMaxValue = float.NaN;

	// Token: 0x0400220D RID: 8717
	private float lastCurrValue = float.NaN;

	// Token: 0x0400220E RID: 8718
	private MessageBundle uiBundle;
}
