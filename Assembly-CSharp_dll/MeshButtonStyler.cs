using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200063F RID: 1599
[ExecuteInEditMode]
[RequireComponent(typeof(Button))]
public class MeshButtonStyler : SRBehaviour
{
	// Token: 0x06002183 RID: 8579 RVA: 0x00080BB1 File Offset: 0x0007EDB1
	public void Awake()
	{
		if (Application.isPlaying)
		{
			this.inputDir = SRSingleton<GameContext>.Instance.InputDirector;
		}
	}

	// Token: 0x06002184 RID: 8580 RVA: 0x00080BCA File Offset: 0x0007EDCA
	public void OnDestroy()
	{
		if (Application.isPlaying)
		{
			InputDirector inputDirector = this.inputDir;
			inputDirector.onKeysChanged = (InputDirector.OnKeysChanged)Delegate.Remove(inputDirector.onKeysChanged, new InputDirector.OnKeysChanged(this.OnInputDeviceChanged));
		}
	}

	// Token: 0x06002185 RID: 8581 RVA: 0x00080BFA File Offset: 0x0007EDFA
	public void OnEnable()
	{
		this.styleDir = UIStyleDirector.Instance;
		this.button = base.GetComponent<Button>();
		this.ApplyStyle();
	}

	// Token: 0x06002186 RID: 8582 RVA: 0x00080C1C File Offset: 0x0007EE1C
	private void ApplyStyle()
	{
		UIStyleDirector.MeshButtonStyle meshButtonStyle = this.styleDir.GetMeshButtonStyle(this.styleName);
		if (meshButtonStyle == null)
		{
			if (Application.isPlaying)
			{
				Log.Warning("Unknown button style: " + this.styleName, Array.Empty<object>());
			}
			return;
		}
		if (Application.isPlaying)
		{
			InputDirector inputDirector = this.inputDir;
			inputDirector.onKeysChanged = (InputDirector.OnKeysChanged)Delegate.Remove(inputDirector.onKeysChanged, new InputDirector.OnKeysChanged(this.OnInputDeviceChanged));
			if (meshButtonStyle.hideIfGamepad)
			{
				InputDirector inputDirector2 = this.inputDir;
				inputDirector2.onKeysChanged = (InputDirector.OnKeysChanged)Delegate.Combine(inputDirector2.onKeysChanged, new InputDirector.OnKeysChanged(this.OnInputDeviceChanged));
			}
			this.OnInputDeviceChanged();
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
			MeshTextStyler.ApplyTextStyle(text, meshButtonStyle);
		}
		if (meshButtonStyle.bgSprite.apply)
		{
			this.button.image.enabled = (meshButtonStyle.bgSprite.value != null);
		}
		if (meshButtonStyle.bgColor.apply)
		{
			this.button.image.color = meshButtonStyle.bgColor.value;
		}
		ColorBlock colors = this.button.colors;
		if (meshButtonStyle.normalTint.apply)
		{
			colors.normalColor = meshButtonStyle.normalTint.value;
		}
		if (meshButtonStyle.highlightedTint.apply)
		{
			colors.highlightedColor = meshButtonStyle.highlightedTint.value;
		}
		if (meshButtonStyle.pressedTint.apply)
		{
			colors.pressedColor = meshButtonStyle.pressedTint.value;
		}
		if (meshButtonStyle.disabledTint.apply)
		{
			colors.disabledColor = meshButtonStyle.disabledTint.value;
		}
		this.button.colors = colors;
		SpriteState spriteState = this.button.spriteState;
		if (meshButtonStyle.disabledSprite.apply)
		{
			spriteState.disabledSprite = meshButtonStyle.disabledSprite.value;
		}
		if (meshButtonStyle.highlightedSprite.apply)
		{
			spriteState.highlightedSprite = meshButtonStyle.highlightedSprite.value;
		}
		if (meshButtonStyle.pressedSprite.apply)
		{
			spriteState.pressedSprite = meshButtonStyle.pressedSprite.value;
		}
		this.button.spriteState = spriteState;
		if (meshButtonStyle.transition.apply)
		{
			this.button.transition = meshButtonStyle.transition.value;
		}
		if (meshButtonStyle.bgSprite.apply)
		{
			this.button.image.sprite = meshButtonStyle.bgSprite.value;
		}
		if (Application.isPlaying && meshButtonStyle.includeChild.apply && meshButtonStyle.includeChild.value != null)
		{
			if (this.includeChildInstance != null)
			{
				Destroyer.Destroy(this.includeChildInstance, "MeshButtonStyler.ApplyStyle");
				this.includeChildInstance = null;
			}
			this.includeChildInstance = UnityEngine.Object.Instantiate<GameObject>(meshButtonStyle.includeChild.value);
			this.includeChildInstance.transform.SetParent(base.transform, false);
			this.includeChildInstance.name = meshButtonStyle.includeChild.value.name;
		}
	}

	// Token: 0x06002187 RID: 8583 RVA: 0x00080F88 File Offset: 0x0007F188
	private void OnInputDeviceChanged()
	{
		UIStyleDirector.MeshButtonStyle meshButtonStyle = this.styleDir.GetMeshButtonStyle(this.styleName);
		base.gameObject.SetActive(!meshButtonStyle.hideIfGamepad || !InputDirector.UsingGamepad());
	}

	// Token: 0x06002188 RID: 8584 RVA: 0x00080FC8 File Offset: 0x0007F1C8
	public static void Convert(GameObject obj)
	{
		foreach (ButtonStyler buttonStyler in obj.GetComponentsInChildren<ButtonStyler>(true))
		{
			Button component = buttonStyler.GetComponent<Button>();
			MeshButtonStyler.ButtonData buttonData = new MeshButtonStyler.ButtonData(component, buttonStyler);
			GameObject gameObject = buttonStyler.gameObject;
			UnityEngine.Object.DestroyImmediate(buttonStyler);
			MeshButtonStyler styler = gameObject.AddComponent<MeshButtonStyler>();
			foreach (Text text in gameObject.GetComponentsInChildren<Text>())
			{
				if (!text.GetComponent<TextStyler>() && !text.GetComponent<MeshTextStyler>())
				{
					GameObject gameObject2 = text.gameObject;
					UnityEngine.Object.DestroyImmediate(text);
					gameObject2.AddComponent<TextMeshProUGUI>();
				}
			}
			buttonData.ApplyTo(component, styler);
		}
	}

	// Token: 0x040020D2 RID: 8402
	[StyleName(typeof(UIStyleDirector.MeshButtonStyle))]
	public string styleName = "Default";

	// Token: 0x040020D3 RID: 8403
	private InputDirector inputDir;

	// Token: 0x040020D4 RID: 8404
	private UIStyleDirector styleDir;

	// Token: 0x040020D5 RID: 8405
	private Button button;

	// Token: 0x040020D6 RID: 8406
	private GameObject includeChildInstance;

	// Token: 0x02000640 RID: 1600
	protected class ButtonData
	{
		// Token: 0x0600218A RID: 8586 RVA: 0x00081088 File Offset: 0x0007F288
		public ButtonData(Button button, ButtonStyler styler)
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

		// Token: 0x0600218B RID: 8587 RVA: 0x00081103 File Offset: 0x0007F303
		private string GetPath(Transform trans)
		{
			if (trans.parent != null)
			{
				return this.GetPath(trans.parent) + "/" + trans.name;
			}
			return trans.name;
		}

		// Token: 0x0600218C RID: 8588 RVA: 0x00081138 File Offset: 0x0007F338
		public void ApplyTo(Button button, MeshButtonStyler styler)
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

		// Token: 0x040020D7 RID: 8407
		private Dictionary<string, MeshTextStyler.TextData> textDict;

		// Token: 0x040020D8 RID: 8408
		private string styleName;
	}
}
