using System;
using TMPro;

// Token: 0x020005A2 RID: 1442
public class JournalUI : BaseUI
{
	// Token: 0x06001DF6 RID: 7670 RVA: 0x00071FD7 File Offset: 0x000701D7
	public void OnEnable()
	{
		base.Play(this.openCue);
	}

	// Token: 0x06001DF7 RID: 7671 RVA: 0x00071FE5 File Offset: 0x000701E5
	public void OnDisable()
	{
		base.Play(this.closeCue);
	}

	// Token: 0x06001DF8 RID: 7672 RVA: 0x00071FF4 File Offset: 0x000701F4
	public void SetJournalKey(string journalKey)
	{
		MessageBundle bundle = SRSingleton<GameContext>.Instance.MessageDirector.GetBundle("mail");
		this.journalText.text = bundle.Get("m.journal." + journalKey);
	}

	// Token: 0x04001D18 RID: 7448
	public TMP_Text journalText;

	// Token: 0x04001D19 RID: 7449
	public SECTR_AudioCue openCue;

	// Token: 0x04001D1A RID: 7450
	public SECTR_AudioCue closeCue;
}
