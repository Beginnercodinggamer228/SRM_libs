using System;
using UnityEngine;

// Token: 0x0200026C RID: 620
public class PlaySoundOnDestroy : MonoBehaviour
{
	// Token: 0x06000D06 RID: 3334 RVA: 0x00035863 File Offset: 0x00033A63
	public void OnDestroy()
	{
		SECTR_AudioSystem.Play(this.cue, base.transform.position, false);
	}

	// Token: 0x04000C38 RID: 3128
	[Tooltip("SFX played when this object is destroyed.")]
	public SECTR_AudioCue cue;
}
