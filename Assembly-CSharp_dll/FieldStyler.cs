using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200063C RID: 1596
[ExecuteInEditMode]
[RequireComponent(typeof(InputField))]
public class FieldStyler : SRBehaviour
{
	// Token: 0x0600217A RID: 8570 RVA: 0x00080917 File Offset: 0x0007EB17
	public void OnEnable()
	{
		this.styleDir = UIStyleDirector.Instance;
		this.field = base.GetComponent<InputField>();
		this.ApplyStyle();
	}

	// Token: 0x0600217B RID: 8571 RVA: 0x00080938 File Offset: 0x0007EB38
	private void ApplyStyle()
	{
		UIStyleDirector.FieldStyle fieldStyle = this.styleDir.GetFieldStyle(this.styleName);
		if (fieldStyle == null)
		{
			if (Application.isPlaying)
			{
				Log.Warning("Unknown button style: " + this.styleName, Array.Empty<object>());
			}
			return;
		}
		TextStyler.ApplyTextStyle(this.field.textComponent, fieldStyle);
		if (this.field.placeholder is Text)
		{
			TextStyler.ApplyTextStyle((Text)this.field.placeholder, fieldStyle.placeholderTextColor, fieldStyle.placeholderFont, fieldStyle.placeholderFontSize, fieldStyle.placeholderFontStyle, fieldStyle.placeholderOutlineColor, fieldStyle.placeholderOutlineWidth);
		}
		if (fieldStyle.bgColor.apply)
		{
			this.field.image.color = fieldStyle.bgColor.value;
		}
		ColorBlock colors = this.field.colors;
		if (fieldStyle.normalTint.apply)
		{
			colors.normalColor = fieldStyle.normalTint.value;
		}
		if (fieldStyle.highlightedTint.apply)
		{
			colors.highlightedColor = fieldStyle.highlightedTint.value;
		}
		if (fieldStyle.pressedTint.apply)
		{
			colors.pressedColor = fieldStyle.pressedTint.value;
		}
		if (fieldStyle.disabledTint.apply)
		{
			colors.disabledColor = fieldStyle.disabledTint.value;
		}
		this.field.colors = colors;
		if (fieldStyle.bgSprite.apply)
		{
			this.field.image.sprite = fieldStyle.bgSprite.value;
		}
		if (fieldStyle.selectionColor.apply)
		{
			this.field.selectionColor = fieldStyle.selectionColor.value;
		}
	}

	// Token: 0x040020CB RID: 8395
	[StyleName(typeof(UIStyleDirector.FieldStyle))]
	public string styleName = "Default";

	// Token: 0x040020CC RID: 8396
	private UIStyleDirector styleDir;

	// Token: 0x040020CD RID: 8397
	private InputField field;

	// Token: 0x040020CE RID: 8398
	private const string PLACEHOLDER_NAME = "Placeholder";
}
