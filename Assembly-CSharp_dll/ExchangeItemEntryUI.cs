using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200057D RID: 1405
public class ExchangeItemEntryUI : MonoBehaviour
{
	// Token: 0x06001D39 RID: 7481 RVA: 0x0006F079 File Offset: 0x0006D279
	public void Awake()
	{
		this.lookupDir = SRSingleton<GameContext>.Instance.LookupDirector;
		this.uiBundle = SRSingleton<GameContext>.Instance.MessageDirector.GetBundle("ui");
	}

	// Token: 0x06001D3A RID: 7482 RVA: 0x0006F0A8 File Offset: 0x0006D2A8
	public void SetEntry(ExchangeDirector.ItemEntry entry)
	{
		if (entry == null)
		{
			base.gameObject.SetActive(false);
			return;
		}
		base.gameObject.SetActive(true);
		this.icon.sprite = this.lookupDir.GetIcon(entry.id);
		this.progressText.text = this.uiBundle.Get("l.ammo", new object[]
		{
			entry.count
		});
		this.nameText.text = Identifiable.GetName(entry.id, true);
	}

	// Token: 0x04001C4F RID: 7247
	public Image icon;

	// Token: 0x04001C50 RID: 7248
	public TMP_Text progressText;

	// Token: 0x04001C51 RID: 7249
	public TMP_Text nameText;

	// Token: 0x04001C52 RID: 7250
	private LookupDirector lookupDir;

	// Token: 0x04001C53 RID: 7251
	private MessageBundle uiBundle;
}
