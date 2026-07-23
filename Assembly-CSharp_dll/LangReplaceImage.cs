using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020005A7 RID: 1447
public class LangReplaceImage : MonoBehaviour
{
	// Token: 0x06001E07 RID: 7687 RVA: 0x00072208 File Offset: 0x00070408
	public void Awake()
	{
		this.img = base.GetComponent<Image>();
		this.orig = this.img.sprite;
		this.msgDir = SRSingleton<GameContext>.Instance.MessageDirector;
		foreach (LangReplaceImage.Entry entry in this.replacements)
		{
			this.replacementDict[entry.lang] = entry.sprite;
		}
		this.msgDir.RegisterBundlesListener(new MessageDirector.BundlesListener(this.OnBundlesAvailable));
	}

	// Token: 0x06001E08 RID: 7688 RVA: 0x00072289 File Offset: 0x00070489
	public void OnBundlesAvailable(MessageDirector messageDir)
	{
		this.UpdateImage();
	}

	// Token: 0x06001E09 RID: 7689 RVA: 0x00072294 File Offset: 0x00070494
	private void UpdateImage()
	{
		string key = this.msgDir.GetCultureLang().ToString();
		if (this.replacementDict.ContainsKey(key))
		{
			this.img.sprite = this.replacementDict[key];
			return;
		}
		this.img.sprite = this.orig;
	}

	// Token: 0x04001D24 RID: 7460
	public LangReplaceImage.Entry[] replacements;

	// Token: 0x04001D25 RID: 7461
	private Image img;

	// Token: 0x04001D26 RID: 7462
	private MessageDirector msgDir;

	// Token: 0x04001D27 RID: 7463
	private Dictionary<string, Sprite> replacementDict = new Dictionary<string, Sprite>();

	// Token: 0x04001D28 RID: 7464
	private Sprite orig;

	// Token: 0x020005A8 RID: 1448
	[Serializable]
	public class Entry
	{
		// Token: 0x04001D29 RID: 7465
		public string lang;

		// Token: 0x04001D2A RID: 7466
		public Sprite sprite;
	}
}
