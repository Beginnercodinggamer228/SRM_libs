using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000675 RID: 1653
public class XlateText : MonoBehaviour
{
	// Token: 0x0600223A RID: 8762 RVA: 0x000846E7 File Offset: 0x000828E7
	public void Awake()
	{
		this.text = base.GetComponent<Text>();
		this.meshText = base.GetComponent<TMP_Text>();
		SRSingleton<GameContext>.Instance.MessageDirector.RegisterBundlesListener(new MessageDirector.BundlesListener(this.InitBundles));
	}

	// Token: 0x0600223B RID: 8763 RVA: 0x0008471C File Offset: 0x0008291C
	public void OnDestroy()
	{
		if (SRSingleton<GameContext>.Instance != null)
		{
			SRSingleton<GameContext>.Instance.MessageDirector.UnregisterBundlesListener(new MessageDirector.BundlesListener(this.InitBundles));
		}
	}

	// Token: 0x0600223C RID: 8764 RVA: 0x00084746 File Offset: 0x00082946
	public void SetKey(string key)
	{
		this.key = key;
		this.UpdateText();
	}

	// Token: 0x0600223D RID: 8765 RVA: 0x00084758 File Offset: 0x00082958
	private void UpdateText()
	{
		string[] array = new string[this.args.Length];
		for (int i = 0; i < this.args.Length; i++)
		{
			array[i] = this.bundle.Xlate(this.args[i]);
		}
		string text = this.key;
		if (this.text != null)
		{
			this.text.text = this.bundle.Get(text, array);
		}
		if (this.meshText != null)
		{
			this.meshText.text = this.bundle.Get(text, array);
		}
	}

	// Token: 0x0600223E RID: 8766 RVA: 0x000847EF File Offset: 0x000829EF
	public void InitBundles(MessageDirector msgDir)
	{
		this.bundle = msgDir.GetBundle(this.bundlePath);
		this.UpdateText();
	}

	// Token: 0x04002218 RID: 8728
	private const string XBOX_SUFFIX = "_xbox";

	// Token: 0x04002219 RID: 8729
	private const string XBOX_GAME_PREVIEW_SUFFIX = "_xboxgp";

	// Token: 0x0400221A RID: 8730
	public string bundlePath = "ui";

	// Token: 0x0400221B RID: 8731
	public string key;

	// Token: 0x0400221C RID: 8732
	public string[] args;

	// Token: 0x0400221D RID: 8733
	public bool addPlatformSuffix;

	// Token: 0x0400221E RID: 8734
	private MessageBundle bundle;

	// Token: 0x0400221F RID: 8735
	private Text text;

	// Token: 0x04002220 RID: 8736
	private TMP_Text meshText;
}
