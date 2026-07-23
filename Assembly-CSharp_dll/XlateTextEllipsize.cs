using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000676 RID: 1654
public class XlateTextEllipsize : MonoBehaviour
{
	// Token: 0x06002240 RID: 8768 RVA: 0x0008481C File Offset: 0x00082A1C
	public void Awake()
	{
		this.text = base.GetComponent<Text>();
		this.meshText = base.GetComponent<TMP_Text>();
		SRSingleton<GameContext>.Instance.MessageDirector.RegisterBundlesListener(new MessageDirector.BundlesListener(this.InitBundles));
	}

	// Token: 0x06002241 RID: 8769 RVA: 0x00084851 File Offset: 0x00082A51
	public void OnDestroy()
	{
		if (SRSingleton<GameContext>.Instance != null && SRSingleton<GameContext>.Instance.MessageDirector != null)
		{
			SRSingleton<GameContext>.Instance.MessageDirector.UnregisterBundlesListener(new MessageDirector.BundlesListener(this.InitBundles));
		}
	}

	// Token: 0x06002242 RID: 8770 RVA: 0x00084890 File Offset: 0x00082A90
	public void InitBundles(MessageDirector msgDir)
	{
		this.bundle = msgDir.GetBundle(this.bundlePath);
		string[] array = new string[this.args.Length];
		for (int i = 0; i < this.args.Length; i++)
		{
			array[i] = this.bundle.Xlate(this.args[i]);
		}
		this.unellipsizedText = this.bundle.Get(this.key, array);
		if (this.text != null)
		{
			this.text.text = this.unellipsizedText;
		}
		if (this.meshText != null)
		{
			this.meshText.text = this.unellipsizedText;
		}
	}

	// Token: 0x06002243 RID: 8771 RVA: 0x0008493C File Offset: 0x00082B3C
	public void Update()
	{
		if (Time.unscaledTime > this.ellipsisChangeTime)
		{
			if (this.text != null)
			{
				this.text.text = this.bundle.Xlate(MessageUtil.Compose("m.ellipsize" + this.ellipsisCount, new string[]
				{
					MessageUtil.Taint(this.unellipsizedText)
				}));
			}
			if (this.meshText != null)
			{
				this.meshText.text = this.bundle.Xlate(MessageUtil.Compose("m.ellipsize" + this.ellipsisCount, new string[]
				{
					MessageUtil.Taint(this.unellipsizedText)
				}));
			}
			this.ellipsisCount = (this.ellipsisCount + 1) % 4;
			this.ellipsisChangeTime = Time.unscaledTime + this.timePerChange;
		}
	}

	// Token: 0x04002221 RID: 8737
	public string bundlePath = "ui";

	// Token: 0x04002222 RID: 8738
	public string key;

	// Token: 0x04002223 RID: 8739
	public string[] args;

	// Token: 0x04002224 RID: 8740
	[Tooltip("Time between ellipsis steps (in real-world clock time).")]
	public float timePerChange = 1f;

	// Token: 0x04002225 RID: 8741
	private MessageBundle bundle;

	// Token: 0x04002226 RID: 8742
	private Text text;

	// Token: 0x04002227 RID: 8743
	private TMP_Text meshText;

	// Token: 0x04002228 RID: 8744
	private float ellipsisChangeTime;

	// Token: 0x04002229 RID: 8745
	private int ellipsisCount;

	// Token: 0x0400222A RID: 8746
	private string unellipsizedText;
}
