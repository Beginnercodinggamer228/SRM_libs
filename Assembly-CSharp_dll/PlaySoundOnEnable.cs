using System;
using UnityEngine;

// Token: 0x0200026D RID: 621
public class PlaySoundOnEnable : MonoBehaviour
{
	// Token: 0x06000D08 RID: 3336 RVA: 0x0003587D File Offset: 0x00033A7D
	public void OnEnable()
	{
		this.instance = SECTR_AudioSystem.Play(this.cue, base.transform.position, false);
	}

	// Token: 0x06000D09 RID: 3337 RVA: 0x0003589C File Offset: 0x00033A9C
	public void OnDisable()
	{
		this.instance.Stop(false);
	}

	// Token: 0x04000C39 RID: 3129
	[Tooltip("SFX played when this object is enabled.")]
	public SECTR_AudioCue cue;

	// Token: 0x04000C3A RID: 3130
	private SECTR_AudioCueInstance instance;
}
