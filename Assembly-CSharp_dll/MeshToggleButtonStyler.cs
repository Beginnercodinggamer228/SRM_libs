using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000647 RID: 1607
[ExecuteInEditMode]
[RequireComponent(typeof(Toggle))]
public class MeshToggleButtonStyler : SRBehaviour
{
	// Token: 0x060021A5 RID: 8613 RVA: 0x00082018 File Offset: 0x00080218
	public void OnEnable()
	{
		this.styleDir = UIStyleDirector.Instance;
		this.toggle = base.GetComponent<Toggle>();
		this.ApplyStyle();
	}

	// Token: 0x060021A6 RID: 8614 RVA: 0x00082037 File Offset: 0x00080237
	public void ChangeStyle(string styleName)
	{
		this.styleName = styleName;
		this.ApplyStyle();
	}

	// Token: 0x060021A7 RID: 8615 RVA: 0x00082048 File Offset: 0x00080248
	private void ApplyStyle()
	{
		UIStyleDirector.MeshToggleButtonStyle meshToggleButtonStyle = this.styleDir.GetMeshToggleButtonStyle(this.styleName);
		if (meshToggleButtonStyle == null)
		{
			if (Application.isPlaying)
			{
				Log.Warning("Unknown panel style: " + this.styleName, Array.Empty<object>());
			}
			return;
		}
		List<TMP_Text> list = new List<TMP_Text>();
		foreach (TMP_Text tmp_Text in base.GetComponentsInChildren<TMP_Text>())
		{
			if (!tmp_Text.GetComponent<MeshTextStyler>())
			{
				list.Add(tmp_Text);
			}
		}
		foreach (TMP_Text text in list)
		{
			MeshTextStyler.ApplyTextStyle(text, meshToggleButtonStyle);
		}
		if (meshToggleButtonStyle.bgSprite.apply && this.toggle.targetGraphic != null)
		{
			this.toggle.targetGraphic.enabled = (meshToggleButtonStyle.bgSprite != null);
		}
		if (meshToggleButtonStyle.bgColor.apply && this.toggle.targetGraphic != null)
		{
			this.toggle.targetGraphic.color = meshToggleButtonStyle.bgColor.value;
		}
		if (meshToggleButtonStyle.bgSprite.apply && this.toggle.targetGraphic is Image)
		{
			((Image)this.toggle.targetGraphic).sprite = meshToggleButtonStyle.bgSprite.value;
		}
		ColorBlock colors = this.toggle.colors;
		if (meshToggleButtonStyle.normalTint.apply)
		{
			colors.normalColor = meshToggleButtonStyle.normalTint.value;
		}
		if (meshToggleButtonStyle.highlightedTint.apply)
		{
			colors.highlightedColor = meshToggleButtonStyle.highlightedTint.value;
		}
		if (meshToggleButtonStyle.pressedTint.apply)
		{
			colors.pressedColor = meshToggleButtonStyle.pressedTint.value;
		}
		if (meshToggleButtonStyle.disabledTint.apply)
		{
			colors.disabledColor = meshToggleButtonStyle.disabledTint.value;
		}
		this.toggle.colors = colors;
	}

	// Token: 0x060021A8 RID: 8616 RVA: 0x00082250 File Offset: 0x00080450
	public static void Convert(GameObject obj)
	{
		foreach (ToggleButtonStyler toggleButtonStyler in obj.GetComponentsInChildren<ToggleButtonStyler>(true))
		{
			Toggle component = toggleButtonStyler.GetComponent<Toggle>();
			MeshToggleButtonStyler.ToggleButtonData toggleButtonData = new MeshToggleButtonStyler.ToggleButtonData(component, toggleButtonStyler);
			GameObject gameObject = toggleButtonStyler.gameObject;
			UnityEngine.Object.DestroyImmediate(toggleButtonStyler);
			MeshToggleButtonStyler styler = gameObject.AddComponent<MeshToggleButtonStyler>();
			foreach (Text text in gameObject.GetComponentsInChildren<Text>())
			{
				if (!text.GetComponent<TextStyler>() && !text.GetComponent<MeshTextStyler>())
				{
					GameObject gameObject2 = text.gameObject;
					UnityEngine.Object.DestroyImmediate(text);
					gameObject2.AddComponent<TextMeshProUGUI>();
				}
			}
			toggleButtonData.ApplyTo(component, styler);
		}
	}

	// Token: 0x040020F1 RID: 8433
	[StyleName(typeof(UIStyleDirector.MeshToggleButtonStyle))]
	public string styleName = "Default";

	// Token: 0x040020F2 RID: 8434
	private UIStyleDirector styleDir;

	// Token: 0x040020F3 RID: 8435
	private Toggle toggle;

	// Token: 0x02000648 RID: 1608
	protected class ToggleButtonData
	{
		// Token: 0x060021AA RID: 8618 RVA: 0x00082310 File Offset: 0x00080510
		public ToggleButtonData(Toggle ToggleButton, ToggleButtonStyler styler)
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
		}

		// Token: 0x060021AB RID: 8619 RVA: 0x0008238B File Offset: 0x0008058B
		private string GetPath(Transform trans)
		{
			if (trans.parent != null)
			{
				return this.GetPath(trans.parent) + "/" + trans.name;
			}
			return trans.name;
		}

		// Token: 0x060021AC RID: 8620 RVA: 0x000823C0 File Offset: 0x000805C0
		public void ApplyTo(Toggle ToggleButton, MeshToggleButtonStyler styler)
		{
			foreach (TextMeshProUGUI textMeshProUGUI in styler.GetComponentsInChildren<TextMeshProUGUI>())
			{
				if (!textMeshProUGUI.GetComponent<TextStyler>() && !textMeshProUGUI.GetComponent<MeshTextStyler>())
				{
					this.textDict[this.GetPath(textMeshProUGUI.transform)].ApplyTo(textMeshProUGUI, null);
				}
			}
			styler.styleName = this.styleName;
		}

		// Token: 0x040020F4 RID: 8436
		private Dictionary<string, MeshTextStyler.TextData> textDict;

		// Token: 0x040020F5 RID: 8437
		private string styleName;
	}
}
