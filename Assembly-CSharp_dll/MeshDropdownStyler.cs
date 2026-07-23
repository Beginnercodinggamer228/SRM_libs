using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000643 RID: 1603
[ExecuteInEditMode]
[RequireComponent(typeof(TMP_Dropdown))]
public class MeshDropdownStyler : SRBehaviour
{
	// Token: 0x06002194 RID: 8596 RVA: 0x0008161E File Offset: 0x0007F81E
	public void OnEnable()
	{
		this.styleDir = UIStyleDirector.Instance;
		this.dropdown = base.GetComponent<TMP_Dropdown>();
		this.ApplyStyle();
	}

	// Token: 0x06002195 RID: 8597 RVA: 0x00081640 File Offset: 0x0007F840
	private void ApplyStyle()
	{
		UIStyleDirector.MeshDropdownStyle meshDropdownStyle = this.styleDir.GetMeshDropdownStyle(this.styleName);
		if (meshDropdownStyle == null)
		{
			if (Application.isPlaying)
			{
				Log.Warning("Unknown dropdown style: " + this.styleName, Array.Empty<object>());
			}
			return;
		}
		List<TMP_Text> list = new List<TMP_Text>();
		foreach (TMP_Text tmp_Text in base.GetComponentsInChildren<TMP_Text>())
		{
			if (!tmp_Text.GetComponent<TextStyler>() && !tmp_Text.GetComponent<MeshTextStyler>())
			{
				list.Add(tmp_Text);
			}
		}
		foreach (TMP_Text text in list)
		{
			MeshTextStyler.ApplyTextStyle(text, meshDropdownStyle);
		}
		if (meshDropdownStyle.bgSprite.apply)
		{
			this.dropdown.image.enabled = (meshDropdownStyle.bgSprite.value != null);
		}
		if (meshDropdownStyle.bgColor.apply)
		{
			this.dropdown.image.color = meshDropdownStyle.bgColor.value;
		}
		ColorBlock colors = this.dropdown.colors;
		if (meshDropdownStyle.normalTint.apply)
		{
			colors.normalColor = meshDropdownStyle.normalTint.value;
		}
		if (meshDropdownStyle.highlightedTint.apply)
		{
			colors.highlightedColor = meshDropdownStyle.highlightedTint.value;
		}
		if (meshDropdownStyle.pressedTint.apply)
		{
			colors.pressedColor = meshDropdownStyle.pressedTint.value;
		}
		if (meshDropdownStyle.disabledTint.apply)
		{
			colors.disabledColor = meshDropdownStyle.disabledTint.value;
		}
		this.dropdown.colors = colors;
		if (meshDropdownStyle.bgSprite.apply)
		{
			this.dropdown.image.sprite = meshDropdownStyle.bgSprite.value;
		}
		Image component = this.dropdown.template.GetComponent<Image>();
		if (meshDropdownStyle.menuBgSprite.apply)
		{
			bool flag = meshDropdownStyle.menuBgSprite.value != null;
			component.enabled = flag;
			if (!flag)
			{
				component.sprite = meshDropdownStyle.menuBgSprite.value;
			}
		}
		if (meshDropdownStyle.menuBgColor.apply)
		{
			component.color = meshDropdownStyle.menuBgColor.value;
		}
		Toggle toggle = component.GetComponentsInChildren<Toggle>(true)[0];
		Image image = (Image)toggle.targetGraphic;
		if (meshDropdownStyle.itemBgSprite.apply)
		{
			bool flag2 = meshDropdownStyle.itemBgSprite.value != null;
			image.enabled = flag2;
			if (!flag2)
			{
				image.sprite = meshDropdownStyle.itemBgSprite.value;
			}
		}
		if (meshDropdownStyle.itemBgColor.apply)
		{
			image.color = meshDropdownStyle.itemBgColor.value;
		}
		ColorBlock colors2 = toggle.colors;
		if (meshDropdownStyle.itemNormalTint.apply)
		{
			colors2.normalColor = meshDropdownStyle.itemNormalTint.value;
		}
		if (meshDropdownStyle.itemHighlightedTint.apply)
		{
			colors2.highlightedColor = meshDropdownStyle.itemHighlightedTint.value;
		}
		if (meshDropdownStyle.itemPressedTint.apply)
		{
			colors2.pressedColor = meshDropdownStyle.itemPressedTint.value;
		}
		if (meshDropdownStyle.itemDisabledTint.apply)
		{
			colors2.disabledColor = meshDropdownStyle.itemDisabledTint.value;
		}
		toggle.colors = colors2;
	}

	// Token: 0x06002196 RID: 8598 RVA: 0x00081990 File Offset: 0x0007FB90
	public static void Convert(GameObject obj)
	{
		foreach (DropdownStyler dropdownStyler in obj.GetComponentsInChildren<DropdownStyler>(true))
		{
			Dropdown component = dropdownStyler.GetComponent<Dropdown>();
			MeshDropdownStyler.DropdownData dropdownData = new MeshDropdownStyler.DropdownData(component, dropdownStyler);
			if (component.itemText.GetComponent<TextStyler>())
			{
				MeshTextStyler.Convert(component.itemText.GetComponent<TextStyler>());
			}
			if (component.captionText.GetComponent<TextStyler>())
			{
				MeshTextStyler.Convert(component.captionText.GetComponent<TextStyler>());
			}
			GameObject gameObject = dropdownStyler.gameObject;
			InitSelected component2 = component.GetComponent<InitSelected>();
			bool flag = component2 != null;
			if (component2 != null)
			{
				UnityEngine.Object.DestroyImmediate(component2);
			}
			UnityEngine.Object.DestroyImmediate(dropdownStyler);
			UnityEngine.Object.DestroyImmediate(component);
			TMP_Dropdown tmp_Dropdown = gameObject.AddComponent<TMP_Dropdown>();
			MeshDropdownStyler styler = gameObject.AddComponent<MeshDropdownStyler>();
			foreach (Text text in gameObject.GetComponentsInChildren<Text>())
			{
				if (!text.GetComponent<TextStyler>() && !text.GetComponent<MeshTextStyler>())
				{
					GameObject gameObject2 = text.gameObject;
					UnityEngine.Object.DestroyImmediate(text);
					gameObject2.AddComponent<TextMeshProUGUI>();
				}
			}
			dropdownData.ApplyTo(tmp_Dropdown, styler);
			if (flag)
			{
				gameObject.AddComponent<InitSelected>();
			}
		}
	}

	// Token: 0x040020DE RID: 8414
	[StyleName(typeof(UIStyleDirector.MeshDropdownStyle))]
	public string styleName = "Default";

	// Token: 0x040020DF RID: 8415
	private UIStyleDirector styleDir;

	// Token: 0x040020E0 RID: 8416
	private TMP_Dropdown dropdown;

	// Token: 0x02000644 RID: 1604
	protected class DropdownData
	{
		// Token: 0x06002198 RID: 8600 RVA: 0x00081AD8 File Offset: 0x0007FCD8
		public DropdownData(Dropdown dropdown, DropdownStyler styler)
		{
			this.textDict = new Dictionary<string, MeshTextStyler.TextData>();
			foreach (Text text in styler.GetComponentsInChildren<Text>())
			{
				if (!text.GetComponent<TextStyler>() && !text.GetComponent<MeshTextStyler>())
				{
					this.textDict[this.GetPath(text.transform)] = new MeshTextStyler.TextData(text, null);
				}
			}
			this.styleName = styler.styleName;
			this.template = dropdown.template;
			this.captionTextObj = ((dropdown.captionText == null) ? null : dropdown.captionText.gameObject);
			this.captionImage = dropdown.captionImage;
			this.itemTextObj = ((dropdown.itemText == null) ? null : dropdown.itemText.gameObject);
			this.itemImage = dropdown.itemImage;
		}

		// Token: 0x06002199 RID: 8601 RVA: 0x00081BBB File Offset: 0x0007FDBB
		private string GetPath(Transform trans)
		{
			if (trans.parent != null)
			{
				return this.GetPath(trans.parent) + "/" + trans.name;
			}
			return trans.name;
		}

		// Token: 0x0600219A RID: 8602 RVA: 0x00081BF0 File Offset: 0x0007FDF0
		public void ApplyTo(TMP_Dropdown dropdown, MeshDropdownStyler styler)
		{
			foreach (TextMeshProUGUI textMeshProUGUI in styler.GetComponentsInChildren<TextMeshProUGUI>())
			{
				if (!textMeshProUGUI.GetComponent<TextStyler>() && !textMeshProUGUI.GetComponent<MeshTextStyler>())
				{
					this.textDict[this.GetPath(textMeshProUGUI.transform)].ApplyTo(textMeshProUGUI, null);
				}
			}
			styler.styleName = this.styleName;
			dropdown.template = this.template;
			dropdown.captionText = ((this.captionTextObj == null) ? null : this.captionTextObj.GetComponent<TMP_Text>());
			dropdown.captionImage = this.captionImage;
			dropdown.itemText = ((this.itemTextObj == null) ? null : this.itemTextObj.GetComponent<TMP_Text>());
			dropdown.itemImage = this.itemImage;
		}

		// Token: 0x040020E1 RID: 8417
		private Dictionary<string, MeshTextStyler.TextData> textDict;

		// Token: 0x040020E2 RID: 8418
		private string styleName;

		// Token: 0x040020E3 RID: 8419
		private RectTransform template;

		// Token: 0x040020E4 RID: 8420
		private GameObject captionTextObj;

		// Token: 0x040020E5 RID: 8421
		private Image captionImage;

		// Token: 0x040020E6 RID: 8422
		private GameObject itemTextObj;

		// Token: 0x040020E7 RID: 8423
		private Image itemImage;
	}
}
