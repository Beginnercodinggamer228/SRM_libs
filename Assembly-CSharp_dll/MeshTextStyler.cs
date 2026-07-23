using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000645 RID: 1605
[ExecuteInEditMode]
[RequireComponent(typeof(TMP_Text))]
public class MeshTextStyler : SRBehaviour
{
	// Token: 0x0600219B RID: 8603 RVA: 0x00081CC2 File Offset: 0x0007FEC2
	public void OnEnable()
	{
		this.styleDir = UIStyleDirector.Instance;
		this.text = base.GetComponent<TMP_Text>();
		this.ApplyStyle();
	}

	// Token: 0x0600219C RID: 8604 RVA: 0x00081CE1 File Offset: 0x0007FEE1
	public void SetStyle(string styleName)
	{
		this.styleName = styleName;
		this.ApplyStyle();
	}

	// Token: 0x0600219D RID: 8605 RVA: 0x00081CF0 File Offset: 0x0007FEF0
	private void ApplyStyle()
	{
		UIStyleDirector.MeshTextStyle meshTextStyle = this.styleDir.GetMeshTextStyle(this.styleName);
		if (meshTextStyle == null)
		{
			if (Application.isPlaying)
			{
				Log.Warning("Unknown text style: " + this.styleName + " in: " + base.gameObject.name, Array.Empty<object>());
			}
			return;
		}
		MeshTextStyler.ApplyTextStyle(this.text, meshTextStyle);
	}

	// Token: 0x0600219E RID: 8606 RVA: 0x00081D50 File Offset: 0x0007FF50
	public static void ApplyTextStyle(TMP_Text text, UIStyleDirector.MeshTextStyle style)
	{
		if (style.textColor.apply)
		{
			text.color = style.textColor.value;
		}
		if (style.font.apply)
		{
			text.font = style.font.value;
		}
		if (style.fontSize.apply)
		{
			text.fontSize = (float)style.fontSize.value;
		}
		if (style.italic.apply)
		{
			if (style.italic.value)
			{
				text.fontStyle |= FontStyles.Italic;
			}
			else
			{
				text.fontStyle &= (FontStyles)(-3);
			}
		}
		if (style.bold.apply)
		{
			if (style.bold.value)
			{
				text.fontStyle |= FontStyles.Bold;
			}
			else
			{
				text.fontStyle &= (FontStyles)(-2);
			}
		}
		if (style.materialPreset.apply && style.materialPreset.value != null)
		{
			text.fontMaterial = style.materialPreset.value;
		}
	}

	// Token: 0x0600219F RID: 8607 RVA: 0x00081E5C File Offset: 0x0008005C
	protected static TextAlignmentOptions Convert(TextAnchor align)
	{
		switch (align)
		{
		case TextAnchor.UpperLeft:
			return TextAlignmentOptions.TopLeft;
		case TextAnchor.UpperCenter:
			return TextAlignmentOptions.Top;
		case TextAnchor.UpperRight:
			return TextAlignmentOptions.TopRight;
		case TextAnchor.MiddleLeft:
			return TextAlignmentOptions.Left;
		case TextAnchor.MiddleCenter:
			return TextAlignmentOptions.Center;
		case TextAnchor.MiddleRight:
			return TextAlignmentOptions.Right;
		case TextAnchor.LowerLeft:
			return TextAlignmentOptions.BottomLeft;
		case TextAnchor.LowerCenter:
			return TextAlignmentOptions.Bottom;
		case TextAnchor.LowerRight:
			return TextAlignmentOptions.BottomRight;
		default:
			return TextAlignmentOptions.Center;
		}
	}

	// Token: 0x060021A0 RID: 8608 RVA: 0x00081ED0 File Offset: 0x000800D0
	public static void Convert(GameObject obj)
	{
		TextStyler[] componentsInChildren = obj.GetComponentsInChildren<TextStyler>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			MeshTextStyler.Convert(componentsInChildren[i]);
		}
	}

	// Token: 0x060021A1 RID: 8609 RVA: 0x00081EFC File Offset: 0x000800FC
	public static void Convert(TextStyler styler)
	{
		Text component = styler.GetComponent<Text>();
		MeshTextStyler.TextData textData = new MeshTextStyler.TextData(component, styler);
		GameObject gameObject = styler.gameObject;
		UnityEngine.Object.DestroyImmediate(styler);
		UnityEngine.Object.DestroyImmediate(component);
		TextMeshProUGUI textComp = gameObject.AddComponent<TextMeshProUGUI>();
		MeshTextStyler styler2 = gameObject.AddComponent<MeshTextStyler>();
		textData.ApplyTo(textComp, styler2);
	}

	// Token: 0x040020E8 RID: 8424
	[StyleName(typeof(UIStyleDirector.MeshTextStyle))]
	public string styleName = "Default";

	// Token: 0x040020E9 RID: 8425
	private UIStyleDirector styleDir;

	// Token: 0x040020EA RID: 8426
	private TMP_Text text;

	// Token: 0x02000646 RID: 1606
	public class TextData
	{
		// Token: 0x060021A3 RID: 8611 RVA: 0x00081F50 File Offset: 0x00080150
		public TextData(Text textComp, TextStyler styler)
		{
			this.text = textComp.text;
			this.align = textComp.alignment;
			this.bestFit = textComp.resizeTextForBestFit;
			if (styler != null)
			{
				this.styleName = styler.styleName;
			}
			this.minSize = textComp.resizeTextMinSize;
			this.maxSize = textComp.resizeTextMaxSize;
		}

		// Token: 0x060021A4 RID: 8612 RVA: 0x00081FB4 File Offset: 0x000801B4
		public virtual void ApplyTo(TextMeshProUGUI textComp, MeshTextStyler styler)
		{
			textComp.text = this.text;
			if (this.styleName != null)
			{
				styler.styleName = this.styleName;
			}
			textComp.fontSizeMin = (float)this.minSize;
			textComp.fontSizeMax = (float)this.maxSize;
			textComp.alignment = MeshTextStyler.Convert(this.align);
			textComp.enableAutoSizing = this.bestFit;
		}

		// Token: 0x040020EB RID: 8427
		private string styleName;

		// Token: 0x040020EC RID: 8428
		private string text;

		// Token: 0x040020ED RID: 8429
		private bool bestFit;

		// Token: 0x040020EE RID: 8430
		private int minSize;

		// Token: 0x040020EF RID: 8431
		private int maxSize;

		// Token: 0x040020F0 RID: 8432
		private TextAnchor align;
	}
}
