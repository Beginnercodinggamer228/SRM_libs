using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200063A RID: 1594
[ExecuteInEditMode]
[RequireComponent(typeof(Toggle))]
public class CheckboxStyler : SRBehaviour
{
	// Token: 0x06002174 RID: 8564 RVA: 0x000802F0 File Offset: 0x0007E4F0
	public void OnEnable()
	{
		this.styleDir = UIStyleDirector.Instance;
		this.toggle = base.GetComponent<Toggle>();
		this.ApplyStyle();
	}

	// Token: 0x06002175 RID: 8565 RVA: 0x00080310 File Offset: 0x0007E510
	private void ApplyStyle()
	{
		UIStyleDirector.CheckboxStyle checkboxStyle = this.styleDir.GetCheckboxStyle(this.styleName);
		if (checkboxStyle == null)
		{
			if (Application.isPlaying)
			{
				Log.Warning("Unknown panel style: " + this.styleName, Array.Empty<object>());
			}
			return;
		}
		List<Text> list = new List<Text>();
		foreach (Text text in base.GetComponentsInChildren<Text>())
		{
			if (!text.GetComponent<TextStyler>())
			{
				list.Add(text);
			}
		}
		foreach (Text text2 in list)
		{
			TextStyler.ApplyTextStyle(text2, checkboxStyle);
		}
		if (checkboxStyle.bgSprite.apply && this.toggle.targetGraphic != null)
		{
			this.toggle.targetGraphic.enabled = (checkboxStyle.bgSprite != null);
		}
		if (checkboxStyle.bgColor.apply && this.toggle.targetGraphic != null)
		{
			this.toggle.targetGraphic.color = checkboxStyle.bgColor.value;
		}
		if (checkboxStyle.bgSprite.apply && this.toggle.targetGraphic is Image)
		{
			((Image)this.toggle.targetGraphic).sprite = checkboxStyle.bgSprite.value;
		}
		if (checkboxStyle.markColor.apply && this.toggle.graphic != null)
		{
			this.toggle.graphic.color = checkboxStyle.markColor.value;
		}
		if (checkboxStyle.markSprite.apply && this.toggle.graphic is Image)
		{
			((Image)this.toggle.graphic).sprite = checkboxStyle.markSprite.value;
		}
		ColorBlock colors = this.toggle.colors;
		if (checkboxStyle.normalTint.apply)
		{
			colors.normalColor = checkboxStyle.normalTint.value;
		}
		if (checkboxStyle.highlightedTint.apply)
		{
			colors.highlightedColor = checkboxStyle.highlightedTint.value;
		}
		if (checkboxStyle.pressedTint.apply)
		{
			colors.pressedColor = checkboxStyle.pressedTint.value;
		}
		if (checkboxStyle.disabledTint.apply)
		{
			colors.disabledColor = checkboxStyle.disabledTint.value;
		}
		this.toggle.colors = colors;
	}

	// Token: 0x040020C5 RID: 8389
	[StyleName(typeof(UIStyleDirector.CheckboxStyle))]
	public string styleName = "Default";

	// Token: 0x040020C6 RID: 8390
	private UIStyleDirector styleDir;

	// Token: 0x040020C7 RID: 8391
	private Toggle toggle;
}
