using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200064D RID: 1613
[ExecuteInEditMode]
[RequireComponent(typeof(Text))]
public class TextStyler : SRBehaviour
{
	// Token: 0x060021B8 RID: 8632 RVA: 0x000829E7 File Offset: 0x00080BE7
	public void OnEnable()
	{
		this.styleDir = UIStyleDirector.Instance;
		this.text = base.GetComponent<Text>();
		this.ApplyStyle();
	}

	// Token: 0x060021B9 RID: 8633 RVA: 0x00082A06 File Offset: 0x00080C06
	public void SetStyle(string styleName)
	{
		this.styleName = styleName;
		this.ApplyStyle();
	}

	// Token: 0x060021BA RID: 8634 RVA: 0x00082A18 File Offset: 0x00080C18
	private void ApplyStyle()
	{
		UIStyleDirector.TextStyle textStyle = this.styleDir.GetTextStyle(this.styleName);
		if (textStyle == null)
		{
			if (Application.isPlaying)
			{
				Log.Warning("Unknown text style: " + this.styleName + " in: " + base.gameObject.name, Array.Empty<object>());
			}
			return;
		}
		TextStyler.ApplyTextStyle(this.text, textStyle);
	}

	// Token: 0x060021BB RID: 8635 RVA: 0x00082A78 File Offset: 0x00080C78
	public static void ApplyTextStyle(Text text, UIStyleDirector.TextStyle style)
	{
		TextStyler.ApplyTextStyle(text, style.textColor, style.font, style.fontSize, style.fontStyle, style.outlineColor, style.outlineWidth);
	}

	// Token: 0x060021BC RID: 8636 RVA: 0x00082AA4 File Offset: 0x00080CA4
	public static void ApplyTextStyle(Text text, UIStyleDirector.ColorSetting textColor, UIStyleDirector.FontSetting font, UIStyleDirector.IntSetting fontSize, UIStyleDirector.FontStyleSetting fontStyle, UIStyleDirector.ColorSetting outlineColor, UIStyleDirector.FloatSetting outlineWidth)
	{
		if (textColor.apply)
		{
			text.color = textColor.value;
		}
		if (font.apply)
		{
			text.font = font.value;
		}
		if (fontSize.apply)
		{
			text.fontSize = fontSize.value;
		}
		if (fontStyle.apply)
		{
			text.fontStyle = fontStyle.value;
		}
		if (outlineColor.apply || outlineWidth.apply)
		{
			Outline outline = text.GetComponent<Outline>();
			if (outline == null)
			{
				outline = text.gameObject.AddComponent<Outline>();
			}
			if (outlineColor.apply)
			{
				outline.effectColor = outlineColor.value;
			}
			if (outlineWidth.apply)
			{
				outline.effectDistance = new Vector2(outlineWidth.value, outlineWidth.value);
			}
		}
	}

	// Token: 0x04002101 RID: 8449
	[StyleName(typeof(UIStyleDirector.TextStyle))]
	public string styleName = "Default";

	// Token: 0x04002102 RID: 8450
	private UIStyleDirector styleDir;

	// Token: 0x04002103 RID: 8451
	private Text text;
}
