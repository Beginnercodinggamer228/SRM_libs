using System;
using InControl;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000674 RID: 1652
public class XlateKeyText : MonoBehaviour
{
	// Token: 0x0600222D RID: 8749 RVA: 0x00084372 File Offset: 0x00082572
	public void Awake()
	{
		this.text = base.GetComponent<Text>();
		this.meshText = base.GetComponent<TMP_Text>();
		SRSingleton<GameContext>.Instance.MessageDirector.RegisterBundlesListener(new MessageDirector.BundlesListener(this.InitBundles));
	}

	// Token: 0x0600222E RID: 8750 RVA: 0x000843A8 File Offset: 0x000825A8
	public void InitBundles(MessageDirector msgDir)
	{
		this.bundle = msgDir.GetBundle(this.bundlePath);
		InputDirector inputDirector = SRSingleton<GameContext>.Instance.InputDirector;
		inputDirector.onKeysChanged = (InputDirector.OnKeysChanged)Delegate.Remove(inputDirector.onKeysChanged, new InputDirector.OnKeysChanged(this.OnKeysChanged));
		InputDirector inputDirector2 = SRSingleton<GameContext>.Instance.InputDirector;
		inputDirector2.onKeysChanged = (InputDirector.OnKeysChanged)Delegate.Combine(inputDirector2.onKeysChanged, new InputDirector.OnKeysChanged(this.OnKeysChanged));
		this.OnKeysChanged();
	}

	// Token: 0x0600222F RID: 8751 RVA: 0x00084424 File Offset: 0x00082624
	public void OnDestroy()
	{
		if (SRSingleton<GameContext>.Instance != null)
		{
			InputDirector inputDirector = SRSingleton<GameContext>.Instance.InputDirector;
			inputDirector.onKeysChanged = (InputDirector.OnKeysChanged)Delegate.Remove(inputDirector.onKeysChanged, new InputDirector.OnKeysChanged(this.OnKeysChanged));
			SRSingleton<GameContext>.Instance.MessageDirector.UnregisterBundlesListener(new MessageDirector.BundlesListener(this.InitBundles));
		}
	}

	// Token: 0x06002230 RID: 8752 RVA: 0x00084484 File Offset: 0x00082684
	public void OnKeysChanged()
	{
		if (this.text != null)
		{
			this.text.text = this.bundle.Get(this.key, new string[]
			{
				XlateKeyText.GetKeyString(this.bundle, this.inputKey, false, false)
			});
		}
		if (this.meshText != null)
		{
			this.meshText.text = this.bundle.Get(this.key, new string[]
			{
				XlateKeyText.GetKeyString(this.bundle, this.inputKey, false, false)
			});
		}
	}

	// Token: 0x06002231 RID: 8753 RVA: 0x0008451D File Offset: 0x0008271D
	public static string GetKeyString(MessageBundle bundle, string inputKey, bool primaryOnly, bool ignoreGamepad = false)
	{
		return XlateKeyText.GetKeyString(bundle, SRInput.Actions.Get(inputKey), primaryOnly, ignoreGamepad);
	}

	// Token: 0x06002232 RID: 8754 RVA: 0x00084534 File Offset: 0x00082734
	public static string GetKeyString(MessageBundle bundle, PlayerAction inputKey, bool primaryOnly, bool ignoreGamepad = false)
	{
		string buttonKey = SRInput.GetButtonKey(inputKey, SRInput.ButtonType.PRIMARY);
		string buttonKey2 = SRInput.GetButtonKey(inputKey, SRInput.ButtonType.SECONDARY);
		string buttonKey3 = SRInput.GetButtonKey(inputKey, SRInput.ButtonType.GAMEPAD);
		string compoundKey = "m.keys.0";
		if (InputDirector.UsingGamepad() && !ignoreGamepad && buttonKey3 != null)
		{
			compoundKey = MessageUtil.Tcompose("m.keys.1", new string[]
			{
				XlateKeyText.XlateKey(buttonKey3)
			});
		}
		else if (buttonKey != null && buttonKey2 != null && !primaryOnly)
		{
			compoundKey = MessageUtil.Tcompose("m.keys.2", new string[]
			{
				XlateKeyText.XlateKey(buttonKey),
				XlateKeyText.XlateKey(buttonKey2)
			});
		}
		else if (buttonKey != null)
		{
			compoundKey = MessageUtil.Tcompose("m.keys.1", new string[]
			{
				XlateKeyText.XlateKey(buttonKey)
			});
		}
		else if (buttonKey2 != null)
		{
			compoundKey = MessageUtil.Tcompose("m.keys.1", new string[]
			{
				XlateKeyText.XlateKey(buttonKey2)
			});
		}
		return bundle.Xlate(compoundKey);
	}

	// Token: 0x06002233 RID: 8755 RVA: 0x000845F9 File Offset: 0x000827F9
	public static string XlateKey(KeyCode key)
	{
		return XlateKeyText.XlateKey(key.ToString());
	}

	// Token: 0x06002234 RID: 8756 RVA: 0x00084610 File Offset: 0x00082810
	public static string XlateKey(string keyStr)
	{
		MessageBundle messageBundle = SRSingleton<GameContext>.Instance.MessageDirector.GetBundle("keys");
		if (messageBundle.Exists("m." + XlateKeyText.GetPlatformStr() + "." + keyStr))
		{
			return messageBundle.Get("m." + XlateKeyText.GetPlatformStr() + "." + keyStr);
		}
		if (messageBundle.Exists("m." + keyStr))
		{
			return messageBundle.Get("m." + keyStr);
		}
		return keyStr;
	}

	// Token: 0x06002235 RID: 8757 RVA: 0x00084691 File Offset: 0x00082891
	public static string XlateKey(InputControlType key)
	{
		return XlateKeyText.XlateKey(key.ToString());
	}

	// Token: 0x06002236 RID: 8758 RVA: 0x000846A5 File Offset: 0x000828A5
	public static string XlateKey(Key key)
	{
		return XlateKeyText.XlateKey(key.ToString());
	}

	// Token: 0x06002237 RID: 8759 RVA: 0x000846B9 File Offset: 0x000828B9
	public static string XlateKey(Mouse key)
	{
		return XlateKeyText.XlateKey(key.ToString());
	}

	// Token: 0x06002238 RID: 8760 RVA: 0x000846CD File Offset: 0x000828CD
	private static string GetPlatformStr()
	{
		return "win";
	}

	// Token: 0x04002212 RID: 8722
	public string bundlePath = "ui";

	// Token: 0x04002213 RID: 8723
	public string key;

	// Token: 0x04002214 RID: 8724
	public string inputKey;

	// Token: 0x04002215 RID: 8725
	private MessageBundle bundle;

	// Token: 0x04002216 RID: 8726
	private Text text;

	// Token: 0x04002217 RID: 8727
	private TMP_Text meshText;
}
