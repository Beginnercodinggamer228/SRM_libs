using System;
using UnityEngine;

// Token: 0x02000092 RID: 146
[AddComponentMenu("SECTR/Audio/SECTR Start Music")]
public class SECTR_StartMusic : MonoBehaviour
{
	// Token: 0x0600030B RID: 779 RVA: 0x00013992 File Offset: 0x00011B92
	private void Start()
	{
		SECTR_AudioSystem.PlayMusic(this.Cue);
		Destroyer.Destroy(this, "SECTR_StartMusic.Start");
	}

	// Token: 0x0400032B RID: 811
	[SECTR_ToolTip("The music to play on Start.")]
	public SECTR_AudioCue Cue;
}
