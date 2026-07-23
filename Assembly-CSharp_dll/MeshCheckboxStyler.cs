using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000641 RID: 1601
[ExecuteInEditMode]
[RequireComponent(typeof(Toggle))]
public class MeshCheckboxStyler : SRBehaviour
{
	// Token: 0x0600218D RID: 8589 RVA: 0x000811A2 File Offset: 0x0007F3A2
	public void OnEnable()
	{
		this.styleDir = UIStyleDirector.Instance;
		this.toggle = base.GetComponent<Toggle>();
		this.ApplyStyle();
	}

	// Token: 0x0600218E RID: 8590 RVA: 0x000811C4 File Offset: 0x0007F3C4
	private void ApplyStyle()
	{
		UIStyleDirector.MeshCheckboxStyle meshCheckboxStyle = this.styleDir.GetMeshCheckboxStyle(this.styleName);
		if (meshCheckboxStyle == null)
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
			MeshTextStyler.ApplyTextStyle(text, meshCheckboxStyle);
		}
		if (meshCheckboxStyle.bgSprite.apply && this.toggle.targetGraphic != null)
		{
			this.toggle.targetGraphic.enabled = (meshCheckboxStyle.bgSprite != null);
		}
		if (meshCheckboxStyle.bgColor.apply && this.toggle.targetGraphic != null)
		{
			this.toggle.targetGraphic.color = meshCheckboxStyle.bgColor.value;
		}
		if (meshCheckboxStyle.bgSprite.apply && this.toggle.targetGraphic is Image)
		{
			((Image)this.toggle.targetGraphic).sprite = meshCheckboxStyle.bgSprite.value;
		}
		if (meshCheckboxStyle.markColor.apply && this.toggle.graphic != null)
		{
			this.toggle.graphic.color = meshCheckboxStyle.markColor.value;
		}
		if (meshCheckboxStyle.markSprite.apply && this.toggle.graphic is Image)
		{
			((Image)this.toggle.graphic).sprite = meshCheckboxStyle.markSprite.value;
		}
		ColorBlock colors = this.toggle.colors;
		if (meshCheckboxStyle.normalTint.apply)
		{
			colors.normalColor = meshCheckboxStyle.normalTint.value;
		}
		if (meshCheckboxStyle.highlightedTint.apply)
		{
			colors.highlightedColor = meshCheckboxStyle.highlightedTint.value;
		}
		if (meshCheckboxStyle.pressedTint.apply)
		{
			colors.pressedColor = meshCheckboxStyle.pressedTint.value;
		}
		if (meshCheckboxStyle.disabledTint.apply)
		{
			colors.disabledColor = meshCheckboxStyle.disabledTint.value;
		}
		this.toggle.colors = colors;
	}

	// Token: 0x0600218F RID: 8591 RVA: 0x00081444 File Offset: 0x0007F644
	public static void Convert(GameObject obj)
	{
		foreach (CheckboxStyler checkboxStyler in obj.GetComponentsInChildren<CheckboxStyler>(true))
		{
			Toggle component = checkboxStyler.GetComponent<Toggle>();
			MeshCheckboxStyler.CheckboxData checkboxData = new MeshCheckboxStyler.CheckboxData(component, checkboxStyler);
			GameObject gameObject = checkboxStyler.gameObject;
			UnityEngine.Object.DestroyImmediate(checkboxStyler);
			MeshCheckboxStyler styler = gameObject.AddComponent<MeshCheckboxStyler>();
			foreach (Text text in gameObject.GetComponentsInChildren<Text>())
			{
				if (!text.GetComponent<TextStyler>() && !text.GetComponent<MeshTextStyler>())
				{
					GameObject gameObject2 = text.gameObject;
					UnityEngine.Object.DestroyImmediate(text);
					gameObject2.AddComponent<TextMeshProUGUI>();
				}
			}
			checkboxData.ApplyTo(component, styler);
		}
	}

	// Token: 0x040020D9 RID: 8409
	[StyleName(typeof(UIStyleDirector.MeshCheckboxStyle))]
	public string styleName = "Default";

	// Token: 0x040020DA RID: 8410
	private UIStyleDirector styleDir;

	// Token: 0x040020DB RID: 8411
	private Toggle toggle;

	// Token: 0x02000642 RID: 1602
	protected class CheckboxData
	{
		// Token: 0x06002191 RID: 8593 RVA: 0x00081504 File Offset: 0x0007F704
		public CheckboxData(Toggle checkbox, CheckboxStyler styler)
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

		// Token: 0x06002192 RID: 8594 RVA: 0x0008157F File Offset: 0x0007F77F
		private string GetPath(Transform trans)
		{
			if (trans.parent != null)
			{
				return this.GetPath(trans.parent) + "/" + trans.name;
			}
			return trans.name;
		}

		// Token: 0x06002193 RID: 8595 RVA: 0x000815B4 File Offset: 0x0007F7B4
		public void ApplyTo(Toggle checkbox, MeshCheckboxStyler styler)
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

		// Token: 0x040020DC RID: 8412
		private Dictionary<string, MeshTextStyler.TextData> textDict;

		// Token: 0x040020DD RID: 8413
		private string styleName;
	}
}
