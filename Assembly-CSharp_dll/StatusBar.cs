using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000624 RID: 1572
[ExecuteInEditMode]
public class StatusBar : MonoBehaviour
{
	// Token: 0x1700022B RID: 555
	// (set) Token: 0x060020FF RID: 8447 RVA: 0x0007E289 File Offset: 0x0007C489
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

	// Token: 0x06002100 RID: 8448 RVA: 0x0007E2A5 File Offset: 0x0007C4A5
	public void Start()
	{
		if (Application.isPlaying)
		{
			SRSingleton<GameContext>.Instance.MessageDirector.RegisterBundlesListener(new MessageDirector.BundlesListener(this.OnBundlesAvailable));
		}
	}

	// Token: 0x06002101 RID: 8449 RVA: 0x0007E2C9 File Offset: 0x0007C4C9
	public void OnDestroy()
	{
		if (Application.isPlaying && SRSingleton<GameContext>.Instance != null)
		{
			SRSingleton<GameContext>.Instance.MessageDirector.UnregisterBundlesListener(new MessageDirector.BundlesListener(this.OnBundlesAvailable));
		}
	}

	// Token: 0x06002102 RID: 8450 RVA: 0x0007E2FA File Offset: 0x0007C4FA
	private void OnBundlesAvailable(MessageDirector msgDir)
	{
		this.uiBundle = msgDir.GetBundle("ui");
		this.OnChanged();
	}

	// Token: 0x06002103 RID: 8451 RVA: 0x0007E314 File Offset: 0x0007C514
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

	// Token: 0x06002104 RID: 8452 RVA: 0x0007E378 File Offset: 0x0007C578
	private void OnChanged()
	{
		float num = (this.currValue - this.minValue) / (this.maxValue - this.minValue);
		string text = this.format;
		if (num == 0f && this.emptyFormat != null && this.emptyFormat != "")
		{
			text = this.emptyFormat;
		}
		else if (num == 1f && this.fullFormat != null && this.fullFormat != "")
		{
			text = this.fullFormat;
		}
		if (this.label != null)
		{
			this.label.text = this.ApplyFormat(text, num);
		}
		if (this.statusImage != null)
		{
			this.statusImage.fillAmount = num;
		}
	}

	// Token: 0x06002105 RID: 8453 RVA: 0x0007E438 File Offset: 0x0007C638
	protected virtual string ApplyFormat(string format, float pct)
	{
		format = format.Replace("{cur}", string.Concat(this.currValue));
		format = format.Replace("{min}", string.Concat(this.minValue));
		format = format.Replace("{max}", string.Concat(this.maxValue));
		format = format.Replace("{cur%}", string.Format("{0:00}", pct * 100f));
		format = format.Replace("{cur2%}", string.Format("{0:00.0}", pct * 100f));
		if (this.translate && Application.isPlaying && this.uiBundle != null)
		{
			format = this.uiBundle.Xlate(format);
		}
		return format;
	}

	// Token: 0x04002055 RID: 8277
	[Tooltip("The image for the bar which we're filling up.")]
	public Image statusImage;

	// Token: 0x04002056 RID: 8278
	[Tooltip("The text we will fill in.")]
	public TMP_Text label;

	// Token: 0x04002057 RID: 8279
	public float minValue;

	// Token: 0x04002058 RID: 8280
	public float maxValue = 100f;

	// Token: 0x04002059 RID: 8281
	public float currValue = 50f;

	// Token: 0x0400205A RID: 8282
	[Tooltip("The formatting to use to form our text.")]
	public string format;

	// Token: 0x0400205B RID: 8283
	[Tooltip("The formatting to use to form our text when empty, or null if we should use default.")]
	public string emptyFormat;

	// Token: 0x0400205C RID: 8284
	[Tooltip("The formatting to use to form our text when full, or null if we should use default.")]
	public string fullFormat;

	// Token: 0x0400205D RID: 8285
	public bool translate;

	// Token: 0x0400205E RID: 8286
	private float lastMinValue = float.NaN;

	// Token: 0x0400205F RID: 8287
	private float lastMaxValue = float.NaN;

	// Token: 0x04002060 RID: 8288
	private float lastCurrValue = float.NaN;

	// Token: 0x04002061 RID: 8289
	private MessageBundle uiBundle;
}
