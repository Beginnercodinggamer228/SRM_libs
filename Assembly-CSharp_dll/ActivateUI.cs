using System;
using TMPro;
using UnityEngine;

// Token: 0x0200053A RID: 1338
public class ActivateUI : MonoBehaviour
{
	// Token: 0x06001BCC RID: 7116 RVA: 0x0006A5C8 File Offset: 0x000687C8
	public void Awake()
	{
		this.messageDir = SRSingleton<GameContext>.Instance.MessageDirector;
		this.messageDir.RegisterBundlesListener(new MessageDirector.BundlesListener(this.OnBundlesAvailable));
		this.inputDir = SRSingleton<GameContext>.Instance.InputDirector;
		InputDirector inputDirector = this.inputDir;
		inputDirector.onKeysChanged = (InputDirector.OnKeysChanged)Delegate.Combine(inputDirector.onKeysChanged, new InputDirector.OnKeysChanged(this.OnKeysChanged));
		this.ResetActivePrompt();
	}

	// Token: 0x06001BCD RID: 7117 RVA: 0x0006A639 File Offset: 0x00068839
	public void Update()
	{
		this.ResetActivePrompt();
	}

	// Token: 0x06001BCE RID: 7118 RVA: 0x0006A641 File Offset: 0x00068841
	public void OnDestroy()
	{
		this.messageDir.UnregisterBundlesListener(new MessageDirector.BundlesListener(this.OnBundlesAvailable));
		InputDirector inputDirector = this.inputDir;
		inputDirector.onKeysChanged = (InputDirector.OnKeysChanged)Delegate.Remove(inputDirector.onKeysChanged, new InputDirector.OnKeysChanged(this.OnKeysChanged));
	}

	// Token: 0x06001BCF RID: 7119 RVA: 0x0006A681 File Offset: 0x00068881
	private void OnBundlesAvailable(MessageDirector messageDir)
	{
		this.OnKeysChanged();
	}

	// Token: 0x06001BD0 RID: 7120 RVA: 0x0006A68C File Offset: 0x0006888C
	private void OnKeysChanged()
	{
		MessageBundle bundle = this.messageDir.GetBundle("ui");
		this.normalPromptText.text = bundle.Get(this.key, new string[]
		{
			XlateKeyText.GetKeyString(bundle, SRInput.Actions.interact, true, false)
		});
		if (this.preGamepadText != null)
		{
			this.preGamepadText.text = bundle.Get(this.key + ".pre_gamepad");
		}
		if (this.postGamepadText != null)
		{
			this.postGamepadText.text = bundle.Get(this.key + ".post_gamepad");
		}
	}

	// Token: 0x06001BD1 RID: 7121 RVA: 0x0006A73C File Offset: 0x0006893C
	private void ResetActivePrompt()
	{
		bool flag = this.useGamepadAlt && InputDirector.UsingGamepad();
		this.normalPrompt.SetActive(!flag);
		if (this.gamepadPrompt != null)
		{
			this.gamepadPrompt.SetActive(flag);
		}
	}

	// Token: 0x04001AE8 RID: 6888
	public string key = "m.press_to_activate";

	// Token: 0x04001AE9 RID: 6889
	public GameObject normalPrompt;

	// Token: 0x04001AEA RID: 6890
	public GameObject gamepadPrompt;

	// Token: 0x04001AEB RID: 6891
	public TMP_Text normalPromptText;

	// Token: 0x04001AEC RID: 6892
	public TMP_Text preGamepadText;

	// Token: 0x04001AED RID: 6893
	public TMP_Text postGamepadText;

	// Token: 0x04001AEE RID: 6894
	public bool useGamepadAlt;

	// Token: 0x04001AEF RID: 6895
	private MessageDirector messageDir;

	// Token: 0x04001AF0 RID: 6896
	private InputDirector inputDir;
}
