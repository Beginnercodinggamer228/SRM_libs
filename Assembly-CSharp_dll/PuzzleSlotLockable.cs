using System;
using UnityEngine;

// Token: 0x0200075F RID: 1887
public class PuzzleSlotLockable : SRBehaviour
{
	// Token: 0x06002769 RID: 10089 RVA: 0x00095980 File Offset: 0x00093B80
	public void Awake()
	{
		PuzzleSlot[] array = this.slots;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].RegisterLock(this);
		}
	}

	// Token: 0x0600276A RID: 10090 RVA: 0x000959AB File Offset: 0x00093BAB
	public virtual void NotifySlotChanged(bool immediate = false)
	{
		if (this.ShouldUnlock())
		{
			this.ActivateOnUnlock();
		}
	}

	// Token: 0x0600276B RID: 10091 RVA: 0x000959BC File Offset: 0x00093BBC
	private void ActivateOnUnlock()
	{
		if (this.activateOnUnlock != null)
		{
			GameObject[] array = this.activateOnUnlock;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(true);
				if (this.autoSaveOnUnlock)
				{
					SRSingleton<GameContext>.Instance.AutoSaveDirector.SaveAllNow();
				}
			}
		}
	}

	// Token: 0x0600276C RID: 10092 RVA: 0x00095A08 File Offset: 0x00093C08
	public bool ShouldUnlock()
	{
		PuzzleSlot[] array = this.slots;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].IsLocked())
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0600276D RID: 10093 RVA: 0x00095A38 File Offset: 0x00093C38
	public SECTR_AudioCue GetCueForLastSlot()
	{
		int num = 0;
		PuzzleSlot[] array = this.slots;
		for (int i = 0; i < array.Length; i++)
		{
			if (!array[i].IsLocked())
			{
				num++;
			}
		}
		return this.slotCues[Math.Max(0, Math.Min(this.slotCues.Length - 1, num - 1))];
	}

	// Token: 0x0600276E RID: 10094 RVA: 0x0003EB49 File Offset: 0x0003CD49
	public void PlayCue(SECTR_AudioCue cue)
	{
		SECTR_AudioSystem.Play(cue, base.transform.position, false);
	}

	// Token: 0x04002728 RID: 10024
	[Tooltip("The slots we monitor.")]
	public PuzzleSlot[] slots;

	// Token: 0x04002729 RID: 10025
	[Tooltip("Any objects we need to activate on unlock.")]
	public GameObject[] activateOnUnlock;

	// Token: 0x0400272A RID: 10026
	[Tooltip("The sounds to play when the slots are opened.")]
	public SECTR_AudioCue[] slotCues;

	// Token: 0x0400272B RID: 10027
	[Tooltip("If true, the game will be automatically save when the puzzle is unlocked.")]
	public bool autoSaveOnUnlock;
}
