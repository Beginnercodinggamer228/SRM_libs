using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000639 RID: 1593
[ExecuteInEditMode]
[RequireComponent(typeof(Button))]
public class ButtonStyler : SRBehaviour
{
	// Token: 0x0600216E RID: 8558 RVA: 0x0007FEB7 File Offset: 0x0007E0B7
	public void Awake()
	{
		if (Application.isPlaying)
		{
			this.inputDir = SRSingleton<GameContext>.Instance.InputDirector;
		}
	}

	// Token: 0x0600216F RID: 8559 RVA: 0x0007FED0 File Offset: 0x0007E0D0
	public void OnDestroy()
	{
		if (Application.isPlaying)
		{
			InputDirector inputDirector = this.inputDir;
			inputDirector.onKeysChanged = (InputDirector.OnKeysChanged)Delegate.Remove(inputDirector.onKeysChanged, new InputDirector.OnKeysChanged(this.OnInputDeviceChanged));
		}
	}

	// Token: 0x06002170 RID: 8560 RVA: 0x0007FF00 File Offset: 0x0007E100
	public void OnEnable()
	{
		this.styleDir = UIStyleDirector.Instance;
		this.button = base.GetComponent<Button>();
		this.ApplyStyle();
	}

	// Token: 0x06002171 RID: 8561 RVA: 0x0007FF20 File Offset: 0x0007E120
	private void ApplyStyle()
	{
		UIStyleDirector.ButtonStyle buttonStyle = this.styleDir.GetButtonStyle(this.styleName);
		if (buttonStyle == null)
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
			if (buttonStyle.hideIfGamepad)
			{
				InputDirector inputDirector2 = this.inputDir;
				inputDirector2.onKeysChanged = (InputDirector.OnKeysChanged)Delegate.Combine(inputDirector2.onKeysChanged, new InputDirector.OnKeysChanged(this.OnInputDeviceChanged));
			}
			this.OnInputDeviceChanged();
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
			TextStyler.ApplyTextStyle(text2, buttonStyle);
		}
		if (buttonStyle.bgSprite.apply)
		{
			this.button.image.enabled = (buttonStyle.bgSprite.value != null);
		}
		if (buttonStyle.bgColor.apply)
		{
			this.button.image.color = buttonStyle.bgColor.value;
		}
		ColorBlock colors = this.button.colors;
		if (buttonStyle.normalTint.apply)
		{
			colors.normalColor = buttonStyle.normalTint.value;
		}
		if (buttonStyle.highlightedTint.apply)
		{
			colors.highlightedColor = buttonStyle.highlightedTint.value;
		}
		if (buttonStyle.pressedTint.apply)
		{
			colors.pressedColor = buttonStyle.pressedTint.value;
		}
		if (buttonStyle.disabledTint.apply)
		{
			colors.disabledColor = buttonStyle.disabledTint.value;
		}
		this.button.colors = colors;
		SpriteState spriteState = this.button.spriteState;
		if (buttonStyle.disabledSprite.apply)
		{
			spriteState.disabledSprite = buttonStyle.disabledSprite.value;
		}
		if (buttonStyle.highlightedSprite.apply)
		{
			spriteState.highlightedSprite = buttonStyle.highlightedSprite.value;
		}
		if (buttonStyle.pressedSprite.apply)
		{
			spriteState.pressedSprite = buttonStyle.pressedSprite.value;
		}
		this.button.spriteState = spriteState;
		if (buttonStyle.transition.apply)
		{
			this.button.transition = buttonStyle.transition.value;
		}
		if (buttonStyle.bgSprite.apply)
		{
			this.button.image.sprite = buttonStyle.bgSprite.value;
		}
		if (Application.isPlaying && buttonStyle.includeChild.apply && buttonStyle.includeChild.value != null)
		{
			Transform transform = base.transform.Find(buttonStyle.includeChild.value.name);
			GameObject gameObject = (transform == null) ? null : transform.gameObject;
			if (gameObject != null)
			{
				Destroyer.Destroy(gameObject, "ButtonStyler.ApplyStyle");
			}
			GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(buttonStyle.includeChild.value);
			gameObject2.transform.SetParent(base.transform, false);
			gameObject2.name = buttonStyle.includeChild.value.name;
		}
	}

	// Token: 0x06002172 RID: 8562 RVA: 0x000802A0 File Offset: 0x0007E4A0
	private void OnInputDeviceChanged()
	{
		UIStyleDirector.ButtonStyle buttonStyle = this.styleDir.GetButtonStyle(this.styleName);
		base.gameObject.SetActive(!buttonStyle.hideIfGamepad || !InputDirector.UsingGamepad());
	}

	// Token: 0x040020C1 RID: 8385
	[StyleName(typeof(UIStyleDirector.ButtonStyle))]
	public string styleName = "Default";

	// Token: 0x040020C2 RID: 8386
	private InputDirector inputDir;

	// Token: 0x040020C3 RID: 8387
	private UIStyleDirector styleDir;

	// Token: 0x040020C4 RID: 8388
	private Button button;
}
