using System;
using UnityEngine;

// Token: 0x0200026F RID: 623
public class PlaySoundOnStart : MonoBehaviour
{
	// Token: 0x06000D0F RID: 3343 RVA: 0x00035951 File Offset: 0x00033B51
	public void Start()
	{
		SECTR_AudioSystem.Play(this.cue, base.transform.position, false);
	}

	// Token: 0x04000C40 RID: 3136
	[Tooltip("SFX played when this object is started.")]
	public SECTR_AudioCue cue;
}
