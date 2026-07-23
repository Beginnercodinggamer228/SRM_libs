using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200063B RID: 1595
[ExecuteInEditMode]
[RequireComponent(typeof(Dropdown))]
public class DropdownStyler : SRBehaviour
{
	// Token: 0x06002177 RID: 8567 RVA: 0x000805A3 File Offset: 0x0007E7A3
	public void OnEnable()
	{
		this.styleDir = UIStyleDirector.Instance;
		this.dropdown = base.GetComponent<Dropdown>();
		this.ApplyStyle();
	}

	// Token: 0x06002178 RID: 8568 RVA: 0x000805C4 File Offset: 0x0007E7C4
	private void ApplyStyle()
	{
		UIStyleDirector.DropdownStyle dropdownStyle = this.styleDir.GetDropdownStyle(this.styleName);
		if (dropdownStyle == null)
		{
			if (Application.isPlaying)
			{
				Log.Warning("Unknown dropdown style: " + this.styleName, Array.Empty<object>());
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
			TextStyler.ApplyTextStyle(text2, dropdownStyle);
		}
		if (dropdownStyle.bgSprite.apply)
		{
			this.dropdown.image.enabled = (dropdownStyle.bgSprite.value != null);
		}
		if (dropdownStyle.bgColor.apply)
		{
			this.dropdown.image.color = dropdownStyle.bgColor.value;
		}
		ColorBlock colors = this.dropdown.colors;
		if (dropdownStyle.normalTint.apply)
		{
			colors.normalColor = dropdownStyle.normalTint.value;
		}
		if (dropdownStyle.highlightedTint.apply)
		{
			colors.highlightedColor = dropdownStyle.highlightedTint.value;
		}
		if (dropdownStyle.pressedTint.apply)
		{
			colors.pressedColor = dropdownStyle.pressedTint.value;
		}
		if (dropdownStyle.disabledTint.apply)
		{
			colors.disabledColor = dropdownStyle.disabledTint.value;
		}
		this.dropdown.colors = colors;
		if (dropdownStyle.bgSprite.apply)
		{
			this.dropdown.image.sprite = dropdownStyle.bgSprite.value;
		}
		Image component = this.dropdown.template.GetComponent<Image>();
		if (dropdownStyle.menuBgSprite.apply)
		{
			bool flag = dropdownStyle.menuBgSprite.value != null;
			component.enabled = flag;
			if (!flag)
			{
				component.sprite = dropdownStyle.menuBgSprite.value;
			}
		}
		if (dropdownStyle.menuBgColor.apply)
		{
			component.color = dropdownStyle.menuBgColor.value;
		}
		Toggle toggle = component.GetComponentsInChildren<Toggle>(true)[0];
		Image image = (Image)toggle.targetGraphic;
		if (dropdownStyle.itemBgSprite.apply)
		{
			bool flag2 = dropdownStyle.itemBgSprite.value != null;
			image.enabled = flag2;
			if (!flag2)
			{
				image.sprite = dropdownStyle.itemBgSprite.value;
			}
		}
		if (dropdownStyle.itemBgColor.apply)
		{
			image.color = dropdownStyle.itemBgColor.value;
		}
		ColorBlock colors2 = toggle.colors;
		if (dropdownStyle.itemNormalTint.apply)
		{
			colors2.normalColor = dropdownStyle.itemNormalTint.value;
		}
		if (dropdownStyle.itemHighlightedTint.apply)
		{
			colors2.highlightedColor = dropdownStyle.itemHighlightedTint.value;
		}
		if (dropdownStyle.itemPressedTint.apply)
		{
			colors2.pressedColor = dropdownStyle.itemPressedTint.value;
		}
		if (dropdownStyle.itemDisabledTint.apply)
		{
			colors2.disabledColor = dropdownStyle.itemDisabledTint.value;
		}
		toggle.colors = colors2;
	}

	// Token: 0x040020C8 RID: 8392
	[StyleName(typeof(UIStyleDirector.DropdownStyle))]
	public string styleName = "Default";

	// Token: 0x040020C9 RID: 8393
	private UIStyleDirector styleDir;

	// Token: 0x040020CA RID: 8394
	private Dropdown dropdown;
}
