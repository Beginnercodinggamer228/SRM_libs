using System;
using UnityEngine;

// Token: 0x02000725 RID: 1829
public class JournalEntry : UIActivator
{
	// Token: 0x06002633 RID: 9779 RVA: 0x000924A0 File Offset: 0x000906A0
	public void Start()
	{
		if (SRSingleton<SceneContext>.Instance.GameModeConfig.GetModeSettings().suppressStory)
		{
			base.gameObject.SetActive(false);
			return;
		}
		base.gameObject.SetActive(true);
	}

	// Token: 0x06002634 RID: 9780 RVA: 0x000924D4 File Offset: 0x000906D4
	public override GameObject Activate()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.uiPrefab);
		gameObject.GetComponent<JournalUI>().SetJournalKey(this.entryKey);
		foreach (ProgressDirector.ProgressType type in this.ensureProgress)
		{
			SRSingleton<SceneContext>.Instance.ProgressDirector.SetProgress(type, 1);
		}
		return gameObject;
	}

	// Token: 0x0400258A RID: 9610
	[Tooltip("The key used to specify which journal entry to display on interaction.")]
	public string entryKey;

	// Token: 0x0400258B RID: 9611
	[Tooltip("If set, ensure the player has these progresses when they read this journal.")]
	public ProgressDirector.ProgressType[] ensureProgress;
}
