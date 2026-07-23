using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200064E RID: 1614
[ExecuteInEditMode]
[RequireComponent(typeof(Toggle))]
public class ToggleButtonStyler : SRBehaviour
{
	// Token: 0x060021BE RID: 8638 RVA: 0x00082B7C File Offset: 0x00080D7C
	public void OnEnable()
	{
		this.styleDir = UIStyleDirector.Instance;
		this.toggle = base.GetComponent<Toggle>();
		this.ApplyStyle();
	}

	// Token: 0x060021BF RID: 8639 RVA: 0x00082B9B File Offset: 0x00080D9B
	public void ChangeStyle(string styleName)
	{
		this.styleName = styleName;
		this.ApplyStyle();
	}

	// Token: 0x060021C0 RID: 8640 RVA: 0x00082BAC File Offset: 0x00080DAC
	private void ApplyStyle()
	{
		UIStyleDirector.ToggleButtonStyle toggleButtonStyle = this.styleDir.GetToggleButtonStyle(this.styleName);
		if (toggleButtonStyle == null)
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
			TextStyler.ApplyTextStyle(text2, toggleButtonStyle);
		}
		if (toggleButtonStyle.bgSprite.apply && this.toggle.targetGraphic != null)
		{
			this.toggle.targetGraphic.enabled = (toggleButtonStyle.bgSprite != null);
		}
		if (toggleButtonStyle.bgColor.apply && this.toggle.targetGraphic != null)
		{
			this.toggle.targetGraphic.color = toggleButtonStyle.bgColor.value;
		}
		if (toggleButtonStyle.bgSprite.apply && this.toggle.targetGraphic is Image)
		{
			((Image)this.toggle.targetGraphic).sprite = toggleButtonStyle.bgSprite.value;
		}
		if (toggleButtonStyle.selectedColor.apply && this.toggle.graphic != null)
		{
			this.toggle.graphic.color = toggleButtonStyle.selectedColor.value;
		}
		if (toggleButtonStyle.selectedSprite.apply && this.toggle.graphic is Image)
		{
			((Image)this.toggle.graphic).sprite = toggleButtonStyle.selectedSprite.value;
		}
		ColorBlock colors = this.toggle.colors;
		if (toggleButtonStyle.normalTint.apply)
		{
			colors.normalColor = toggleButtonStyle.normalTint.value;
		}
		if (toggleButtonStyle.highlightedTint.apply)
		{
			colors.highlightedColor = toggleButtonStyle.highlightedTint.value;
		}
		if (toggleButtonStyle.pressedTint.apply)
		{
			colors.pressedColor = toggleButtonStyle.pressedTint.value;
		}
		if (toggleButtonStyle.disabledTint.apply)
		{
			colors.disabledColor = toggleButtonStyle.disabledTint.value;
		}
		this.toggle.colors = colors;
	}

	// Token: 0x04002104 RID: 8452
	[StyleName(typeof(UIStyleDirector.ToggleButtonStyle))]
	public string styleName = "Default";

	// Token: 0x04002105 RID: 8453
	private UIStyleDirector styleDir;

	// Token: 0x04002106 RID: 8454
	private Toggle toggle;
}
